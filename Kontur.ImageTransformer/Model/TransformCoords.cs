using System.Diagnostics;
using Kontur.ImageTransformer.Model.Interfaces;

namespace Kontur.ImageTransformer.Model
{
    [DebuggerDisplay("{X}, {Y}, {W}, {H}")]
    public class TransformCoords : ITransformCoords
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }

        public static TransformCoords FromArray(int[] values)
        {
            var coords = new TransformCoords
            {
                X = values[0],
                Y = values[1],
                W = values[2],
                H = values[3]
            };
            return coords;
        }
    }
}