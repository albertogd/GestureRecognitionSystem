using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using Microsoft.Kinect;
using Models;
using Algorithms.Extensions;

namespace Algorithms.Preprocessing
{
    public class RotatePreprocessor : IPreprocessor
    {

        /// <summary>
        /// Rotate skeleton to be always in front of Kinect
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        public Skeleton Preprocess(Skeleton skeleton)
        {
            // Rotate skeleton
            Skeleton sk = new Skeleton();
            SkeletonExtension.CopySkeleton(skeleton, sk);

            // Get Rotation Quaternion
            Vector3D i = SkeletonExtension.VectorBetween(sk, JointType.ShoulderRight, JointType.ShoulderLeft);
            //Vector3D i = SkeletonExtension.VectorBetween(sk, JointType.FootLeft, JointType.FootRight);
            i.Normalize();

            Matrix3D m = Matrix3D.Identity;
            Quaternion rot = SkeletonExtension.GetShortestRotationBetweenVectors(new Vector3D(-1, 0, 0), i);
            m.RotateAt(rot, new Point3D(sk.Position.X, sk.Position.Y, sk.Position.Z));

            foreach (JointType type in Enum.GetValues(typeof(JointType)))
            {
                Joint joint = sk.Joints[type];
                Vector3D v = SkeletonExtension.SkeletonPointToVector3D(joint.Position);
                joint.Position = SkeletonExtension.Vector3DToSkeletonPoint(m.Transform(v));
                sk.Joints[type] = joint;
            }

            return sk;
        }

        public override string ToString()
        {
            return "Rotate";
        }
    }
}

