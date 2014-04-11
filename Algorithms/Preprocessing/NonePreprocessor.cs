using System;
using Microsoft.Kinect;

namespace Algorithms.Preprocessing
{
    public class NonePreprocessor : IPreprocessor
    {
        public Skeleton Preprocess(Skeleton skeleton)
        {
            return skeleton;
        }

        public override string ToString()
        {
            return "None";
        }
    }
}
