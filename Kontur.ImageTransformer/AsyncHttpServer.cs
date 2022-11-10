using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Kontur.ImageTransformer.Model;
using Kontur.ImageTransformer.Processors.Interfaces;
using ImageFormat = Kontur.ImageTransformer.Processors.Interfaces.ImageFormat;
using static Kontur.ImageTransformer.Extensions.StatusHelper;

namespace Kontur.ImageTransformer
{
    internal sealed class AsyncHttpServer<TImageProcessor, TImageComparator> : IDisposable
        where TImageProcessor: IImageProcessor, new()
        //TODO: maybe move IImageComparator to IImageProcessor field 
        where TImageComparator : IImageComparator, new()
    {
        private readonly IImageComparator _comparator;

        /// <summary>
        /// Represents "100,100,100,100"-like string 
        /// </summary>
        private readonly Regex _coordsRegex = new Regex(@"[\-0-9]{1,10},[\-0-9]{1,10},[\-0-9]{1,10},[\-0-9]{1,10}",
            RegexOptions.Multiline);
        
        private const int RequestTimeoutMs = 1000;
        private readonly HttpListener _listener;
        private Thread _listenerThread;
        private bool _disposed;
        private volatile bool _isRunning;
        
        public AsyncHttpServer()
        {
            _listener = new HttpListener();
            _listener.TimeoutManager.RequestQueue = TimeSpan.FromSeconds(3);
            _listener.TimeoutManager.IdleConnection = TimeSpan.FromSeconds(2);
            _comparator = new TImageComparator();

            //Image comparison operators
            //image must be:
            // not higher than 1000x1000
            // weights not more than 100 Kb
            _comparator.AddOperator(img => img.Height <= 1000)
                .AddOperator(img => img.Width <= 1000)
                .AddOperator(img => img.Size <= 100)
                .AddOperator(img => img.Format == ImageFormat.Png);
        }
        
        public void Start(string prefix)
        {
            lock (_listener)
            {
                if (_isRunning) 
                    return;
                
                _listener.Prefixes.Clear();
                _listener.Prefixes.Add(prefix);
                _listener.Start();

                _listenerThread = new Thread(Listen)
                {
                    IsBackground = true,
                    Priority = ThreadPriority.Highest
                };
                _listenerThread.Start();
                    
                _isRunning = true;
            }
        }

        public void Stop()
        {
            lock (_listener)
            {
                if (!_isRunning)
                    return;

                _listener.Stop();

                _listenerThread.Abort();
                _listenerThread.Join();
                
                _isRunning = false;
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            Stop();

            _listener.Close();
        }
        
        private void Listen()
        {
            while (true)
            {
                try
                {
                    if (_listener.IsListening)
                    {
                        var ctx = _listener.GetContext();
                        var cts = new CancellationTokenSource();
                        cts.CancelAfter(RequestTimeoutMs);
                        Task.Run( async () =>
                        {
                            cts.Token.ThrowIfCancellationRequested();
                            var context = ctx;
                            try
                            {
                                var statusCode = await HandleContext(context);
                                context.Response.StatusCode = statusCode;
                            }
                            catch (TaskCanceledException)
                            {
                                ctx.Response.StatusCode = RequestTimeout();
                                ctx.Response.Close();
                            }
                            catch(Exception ex)
                            {
                                context.Response.StatusCode = InternalServerError();
                                Console.WriteLine("{0}\n\t{1}", ex.Message, ex.StackTrace);
                            }
                            finally
                            {
                                context.Response.Close();
                                cts.Dispose();
                            }
                        }, cts.Token);
                    }
                    else Thread.Sleep(0);
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception)
                {
#if DEBUG
                    throw;
#endif
                    return;
                }
            }
        }

        private async Task<int> HandleContext(HttpListenerContext listenerContext)
        {
            var path = listenerContext.Request.Url.Segments;
            if (path[1] == "process/")
            {
                return await HandleImageProcessing(listenerContext.Request, listenerContext.Response, path);
            }
            return BadRequest();
        }

        private async Task<int> HandleImageProcessing(HttpListenerRequest request, HttpListenerResponse response,
            string[] path)
        {
            #region query processing
            TransformType? GetTransformMethod(string query)
            {
                var requestedTransform = query.Remove(query.Length - 1);
                switch (requestedTransform)
                {
                    case "rotate-cw":
                        return TransformType.RotateCw;
                    case "rotate-ccw":
                        return TransformType.RotateCCw;
                    case "flip-v":
                        return TransformType.FlipV;
                    case "flip-h":
                        return TransformType.FlipH;
                    default:
                        return null;
                }
            }

            int[] GetCoords(string query)
            {
                if (_coordsRegex.IsMatch(query))
                {
                   return query
                        .Split(',')
                        .Select(int.Parse)
                        .ToArray();
                }
                return null;
            }
            
            #endregion
            
            IImageProcessor proc;

            using (var datastream = request.InputStream)
            {
                if (datastream == Stream.Null)
                {
                    return BadRequest();
                }
                
                try
                {
                    proc = new TImageProcessor();
                    var filesize = request.ContentLength64;
                    proc.InitWithFilesize(_comparator, request.InputStream, filesize);
                }
                catch (Exception)
                {
                    datastream.Close();
                    return InternalServerError();
                }
            }
            

            if (!proc.CheckImage())
            {
                return BadRequest();
            }

            TransformType? transformType = GetTransformMethod(path[2]);
            if (transformType is null)
            {
                return BadRequest();
            }
            var coordsArray = GetCoords(path[3]);
            if (coordsArray is null)
            {
                return BadRequest();
            }
            var coords = TransformCoords.FromArray(coordsArray);


            try
            {
                // transformType cannot be null here
                proc.ProcessImage((TransformType) transformType, coords);
            }
            catch (RectangleNoIntersectException)
            {
                return NoContent();
            }

            response.ContentType = "image/png";
            proc.WriteImage(response.OutputStream);
            proc.Dispose();

            return Ok();
        }
    }
}