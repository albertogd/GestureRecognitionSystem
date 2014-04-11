using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Kinect;
using Algorithms.Extensions;
using Models;

namespace Algorithms.Preprocessing
{
    public class CenterPreprocessor : IPreprocessor
    {
        /// <summary>
        /// Center a skeleton => HipCenter = (0,0,1)
        /// </summary>
        /// <param name="skeleton">Skeleton</param>
        /// <returns>Centered Skeleton</returns>
        public Skeleton Preprocess(Skeleton skeleton)
        {
            SkeletonPoint center = new SkeletonPoint() { X = 0, Y = 0, Z = 1 };

            return SkeletonExtension.Center(skeleton, center);
        }

        public override string ToString()
        {
            return "Center";
        }
    }
}

