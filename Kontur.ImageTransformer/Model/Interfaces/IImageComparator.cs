using System;

namespace Kontur.ImageTransformer.Model.Interfaces
{
    public interface IImageComparator
    {
        bool Compare(IImage image);
        void AddOperator(Func<IImage, bool> predicate);
    }
}