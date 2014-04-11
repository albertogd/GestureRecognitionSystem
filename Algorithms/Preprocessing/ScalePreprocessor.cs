using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Kinect;
using Algorithms.Extensions;
using Models;

namespace Algorithms.Preprocessing
{
    public class ScalePreprocessor : IPreprocessor
    {

        /// <summary>
        /// Scale with Distance between shoulders
        /// </summary>
        /// <param name="skeleton">Skeleton</param>
        /// <returns>Scaled Skeleton</returns>
        public Skeleton Preprocess(Skeleton skeleton)
        {
            Skeleton sk = new Skeleton();
            SkeletonExtension.CopySkeleton(skeleton, sk);

            return sk;
        }

        public override string ToString()
        {
            return "Scale";
        }
    }
}
