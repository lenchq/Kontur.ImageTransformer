using System;
using Kontur.ImageTransformer.Model;

namespace Kontur.ImageTransformer
{
    public class EntryPoint
    {
        public static void Main(string[] args)
        {
            using (var server = new AsyncHttpServer<NativeImageProcessor, DefaultImageComparator>())
            {
                server.Start("http://+:8080/");

                Console.ReadKey(true);
            }
        }
    }
}
