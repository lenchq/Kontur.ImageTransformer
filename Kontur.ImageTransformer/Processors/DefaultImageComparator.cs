﻿using System;
using System.Collections.Generic;
using Kontur.ImageTransformer.Processors.Interfaces;

namespace Kontur.ImageTransformer.Processors
{
    public sealed class DefaultImageComparator : IImageComparator
    {
        private readonly List<Func<IImage, bool>> _predicates;

        public DefaultImageComparator()
        {
            _predicates = new List<Func<IImage, bool>>();
        }

        public IImageComparator AddOperator(Func<IImage, bool> predicate)
        {
            _predicates.Add(predicate);
            return this;
        }
        
        public bool Compare(IImage image)
        {
            for (var i = 0; i < _predicates.Count; i++)
            {
                if (!_predicates[i].Invoke(image))
                {
                    return false;
                } 
            }

            return true;
        }
    }
}