using System;

namespace Kontur.ImageTransformer.Model.Interfaces
{
    public interface IImageComparator
    {
        bool Compare(IImage image);
        IImageComparator AddOperator(Func<IImage, bool> predicate);
    }
}