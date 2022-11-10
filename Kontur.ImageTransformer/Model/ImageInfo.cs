using Kontur.ImageTransformer.Processors.Interfaces;

namespace Kontur.ImageTransformer.Model
{
    public class ImageInfo : IImage
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public double Size { get; set; }
        public ImageFormat Format { get; set; }
    }
}