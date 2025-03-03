﻿namespace Kontur.ImageTransformer.Processors.Interfaces
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