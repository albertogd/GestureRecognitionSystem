using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Models;
using Algorithms.Extensions;

namespace Algorithms.Preprocessing
{
    public class Preprocessor
    {
        private int confID;


        public Preprocessor(int ConfID)
        {
            confID = ConfID;
        }

        public Skeleton Preprocess(Skeleton skeleton)
        {
            Skeleton sk = new Skeleton();
            SkeletonExtension.CopySkeleton(skeleton, sk);

            Context ctx = new Context();
            ConfigurationSkeletonPreprocess conf = ctx.Configurations.Find(confID).skeletonPreprocessConfiguration;

            if (conf.Rotate != RotateType.None)
            {
                if (conf.Rotate == RotateType.ChangeBase)
                {
                    ChangeBasePreprocessor p = new ChangeBasePreprocessor();
                    sk = p.Preprocess(sk);
                }
                else if (conf.Rotate == RotateType.Rotate)
                {
                    RotatePreprocessor p = new RotatePreprocessor();
                    sk = p.Preprocess(sk);
                }
            }

            if (conf.Scale)
            {
                ScalePreprocessor p = new ScalePreprocessor();
                sk = p.Preprocess(sk);
            }

            if (conf.Center)
            {
                CenterPreprocessor p = new CenterPreprocessor();
                sk = p.Preprocess(sk);
            }

            return sk;
        }
    }
}
