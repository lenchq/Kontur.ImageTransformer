using System;
using System.IO;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Model.Interfaces
{
    public interface IImageProcessor : IDisposable
    {
        byte[] ImageBytes { get; }
        void Init(IImageComparator comparator, Stream data);
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