using System;
using Microsoft.Kinect;

namespace Algorithms.Preprocessing
{
    public interface IPreprocessor
    {
        Skeleton Preprocess(Skeleton skeleton);
        string ToString();
    }
}
