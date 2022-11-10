using System;
using System.IO;
using Kontur.ImageTransformer.Processors;
//using Microsoft.Extensions.Configuration;

namespace Kontur.ImageTransformer
{
    public class EntryPoint
    {
        private const int Port = 8080;
        public static void Main(string[] args)
        {
            //NativeImageProcessor a bit faster than ImageMagick
            using (var server = new AsyncHttpServer<NativeImageProcessor, DefaultImageComparator>())
            {
                var prefix = $"http://+:{Port}/";
                server.Start(prefix);
                Console.WriteLine("Listening port :" + Port);

                Console.ReadKey(true);
            }
        }
    }
}
