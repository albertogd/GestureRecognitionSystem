using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using Microsoft.Kinect;
using Algorithms.Extensions;
using Models;

namespace Algorithms.Preprocessing
{
    public class ChangeBasePreprocessor : IPreprocessor
    {

        /// <summary>
        /// Change base vectors:
        /// X => Line from shoulderLeft to shoulderRight
        /// Y => Line from ShoulderCenter to HipCenter
        /// Z => Perpendicular to X and Y (Z = X x Y)
        /// </summary>
        /// <param name="skeleton">Skeleton in new base</param>
        /// <returns></returns>
        public Skeleton Preprocess(Skeleton skeleton)
        {
            // Change Skeleton to new base
            Skeleton sk = new Skeleton();
            SkeletonExtension.CopySkeleton(skeleton, sk);

            // Get Base Vectors
            Vector3D i = SkeletonExtension.VectorBetween(sk, JointType.ShoulderLeft, JointType.ShoulderRight);
            Vector3D j = SkeletonExtension.VectorBetween(sk, JointType.Spine, JointType.ShoulderCenter);
            Vector3D k = Vector3D.CrossProduct(i, j);
            
            // Normalize
            i.Normalize();
            j.Normalize();
            k.Normalize();

            // Base
            Base v = new Base(i, j, k);

            foreach (JointType type in Enum.GetValues(typeof(JointType)))
            {
                Joint joint = sk.Joints[type];
                joint.Position = v.ChangeBase(joint.Position);
                sk.Joints[type] = joint;
            }

            return sk;
        }

        public override string ToString()
        {
            return "Change Base";
        }
    }
}

