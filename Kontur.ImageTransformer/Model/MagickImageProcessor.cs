using System;
using Rectangle = System.Drawing.Rectangle;
using System.IO;
using ImageMagick;
using Kontur.ImageTransformer.Model.Interfaces;

namespace Kontur.ImageTransformer.Model
{
    public sealed class MagickImageProcessor : IImageProcessor
    {
        private MagickImage _image;
        private IImageComparator _comparator;
        private ImageInfo _imageInfo;

        private bool _initialized;
        private long _fileSize;

        public MagickImageProcessor()
        {
            _initialized = false;
        }

        public byte[] ImageBytes
        {
            get
            {
                if (!_initialized)
                {
                    throw new Exception("Image not initialized!");
                }

                return _image.ToByteArray(MagickFormat.Png32);
            }
        }

        public void Init(IImageComparator comparator,Stream data)
        {
            _fileSize = data.Length;
            _comparator = comparator;
            _image = new MagickImage(data);
            _initialized = true;

            _imageInfo = new ImageInfo
            {
                Height = _image.Height,
                Width = _image.Width,
                //image format is always png
                Format = ImageFormat.Png,
                //Bytes to KiB
                Size = _fileSize >> 10
            };
        }
        
        public bool CheckImage()
        {
            if (!_initialized)
            {
                throw new Exception("Image not initialized!");
            }
            
            return _comparator.Compare(_imageInfo);
        }

        public void WriteImage(Stream outputStream)
        {
            _image.Write(outputStream, MagickFormat.Png32);
        }

        public void ProcessImage(TransformType type, ITransformCoords coords)
        {
            ProcessRotate(type);
            ProcessCrop(coords);
        }

        private void ProcessCrop(ITransformCoords coords)
        {
            var original = new Rectangle(0, 0, _image.Width, _image.Height);
            var cropArea = new Rectangle(coords.X, coords.Y, coords.W, coords.H);
            
            var croppedRect = Rectangle.Intersect(original, cropArea);

            if ((croppedRect == Rectangle.Empty 
                && cropArea != Rectangle.Empty)
                || croppedRect.X <= 0
                || croppedRect.Y <= 0)
            {
                throw new RectangleNoIntersectException();
            }

            _image.Crop(new MagickGeometry(
                croppedRect.X,
                croppedRect.Y,
                croppedRect.Width,
                croppedRect.Height
                ));
        }

        private void ProcessRotate(TransformType type)
        {
            switch (type)
            {
                case TransformType.FlipH:
                    _image.Flip();
                    break;
                case TransformType.FlipV:
                    _image.Flop();
                    break;
                case TransformType.RotateCw:
                    _image.Rotate(90d);
                    break;
                case TransformType.RotateCCw:
                    _image.Rotate(-90d);
                    break;
                default:
                    return;
            }
            
        }
        public void Dispose()
        {
            _image.Dispose();
        }
    }
}