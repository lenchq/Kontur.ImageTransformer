using System;
using System.IO;

namespace Kontur.ImageTransformer.Model.Interfaces
{
    public interface IImageProcessor : IDisposable
    {
        void Init(IImageComparator comparator, Stream data);
        void InitWithFilesize(IImageComparator comparator, Stream data, long filesize);
        bool CheckImage();
        void ProcessImage(TransformType type, ITransformCoords coords);
        void WriteImage(Stream outputStream);
    }

    public enum TransformType
    {
        /// <summary>
        /// Rotate Clockwise
        /// </summary>
        RotateCw,
        /// <summary>
        /// Rotate CounterClockwise
        /// </summary>
        RotateCCw,
        /// <summary>
        /// Flip Vertically
        /// </summary>
        FlipV,
        /// <summary>
        /// Flip Horizontally
        /// </summary>
        FlipH
    }
}