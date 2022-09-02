using System;
using System.Drawing;
using System.IO;
using System.Net.Mime;
using Kontur.ImageTransformer.Model.Interfaces;

namespace Kontur.ImageTransformer.Model
{
    public sealed class DrawingImageProcessor : IImageProcessor
    {
        public byte[] ImageBytes { get; }

        private Graphics _graphics;
        private IImageComparator _comparator;
        private bool _initialized;
        private ImageInfo _imageInfo;
        private Image _image;
        private long _fileSize;

        public DrawingImageProcessor()
        {
            
        }

        public void Init(IImageComparator comparator, Stream data)
        {
            _comparator = comparator;
            _fileSize = data.Length;
            _image = Image.FromStream(data);
            data.Close();
            
            _imageInfo = new ImageInfo
            {
                Height = _image.Height,
                
                Width = _image.Width,
                
                //image format is always png
                Format = ImageFormat.Png,
                
                //Bytes to KiB
                Size = _fileSize >> 10
            };
            //_graphics = Graphics.FromImage(_image);
            
            _initialized = true;
        }

        public bool CheckImage()
        {
            if (!_initialized)
            {
                throw new Exception("Image not initialized!");
            }
            
            return _comparator.Compare(_imageInfo);
        }

        public void ProcessImage(TransformType type, ITransformCoords coords)
        {
            ProcessRotate(type);
            ProcessTransform(coords);
        }

        private void ProcessTransform(ITransformCoords coords)
        {
            var original = new Rectangle(0, 0, _image.Width, _image.Height);
            var cropArea = new Rectangle(coords.X, coords.Y, coords.W, coords.H);

            var croppedRect = Rectangle.Intersect(original, cropArea);

            if ((croppedRect == Rectangle.Empty 
                && cropArea != Rectangle.Empty)
                || croppedRect.X <= 0
                || croppedRect.Y <= 0
                || croppedRect.Height <= 0
                || croppedRect.Width <= 0)
            {
                throw new RectangleNoIntersectException();
            }
            
            var bmp = new Bitmap(croppedRect.Width, croppedRect.Height);

            using (var graphics = Graphics.FromImage(bmp))
            {
                graphics.DrawImage(_image, -croppedRect.X, -croppedRect.Y);
                //graphics.DrawIcon(SystemIcons.WinLogo, 25, 25);
                //graphics.FillRectangle(Brushes.Black, new Rectangle(0,0,100,100));
            }

            _image = bmp;

        }

        private void ProcessRotate(TransformType type)
        {
            switch (type)
            {
                case TransformType.FlipH:
                    FlipH();
                    break;
                case TransformType.FlipV:
                    FlipV();
                    break;
                case TransformType.RotateCw:
                    RotateCw();
                    break;
                case TransformType.RotateCCw:
                    RotateCCw();
                    break;
                default:
                    return;
            }
        }

        private void RotateCCw()
        {
            _image.RotateFlip(RotateFlipType.Rotate270FlipNone);
        }

        private void RotateCw()
        {
            _image.RotateFlip(RotateFlipType.Rotate90FlipNone);
        }

        private void FlipV()
        {
            _image.RotateFlip(RotateFlipType.RotateNoneFlipX);
        }

        private void FlipH()
        {
            _image.RotateFlip(RotateFlipType.RotateNoneFlipY);
        }
        

        public void WriteImage(Stream outputStream)
        {
            _image.Save(outputStream, System.Drawing.Imaging.ImageFormat.Png);
        }

        public void Dispose()
        {
            _image.Dispose();
        }
    }
}