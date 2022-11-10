using System;

namespace Kontur.ImageTransformer.Processors.Interfaces
{
    public interface IImageComparator
    {
        bool Compare(IImage image);
        IImageComparator AddOperator(Func<IImage, bool> predicate);
    }
}