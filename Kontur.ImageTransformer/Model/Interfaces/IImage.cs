using System.Net.Mime;

namespace Kontur.ImageTransformer.Model.Interfaces
{
    public interface IImage
    {
        int Height { get; set; }
        int Width { get; set; }
        
        /// <summary>
        /// Size in Kilobytes
        /// </summary>
        double Size { get; set; }
        ImageFormat Format { get; set; }
    }

    public enum ImageFormat
    {
        Bitmap,
        Png,
        Tiff,
        Jpeg,
        Gif
    }
}