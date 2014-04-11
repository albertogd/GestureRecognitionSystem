using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Microsoft.Kinect;

namespace Algorithms.Extensions
{
    public class SkeletonExtension
    {

        /// <summary>
        /// Center a skeleton
        /// </summary>
        /// <param name="skeleton">The Skeleton</param>
        /// <returns>Returns centered skeleton</returns>
        public static Skeleton Center(Skeleton skeleton, SkeletonPoint center)
        {
            Skeleton sk = new Skeleton();
            SkeletonExtension.CopySkeleton(skeleton, sk);
            SkeletonPoint hipCenter = sk.Joints[JointType.HipCenter].Position; 
            Vector3D v = new Vector3D(center.X - hipCenter.X, center.Y - hipCenter.Y, center.Z - hipCenter.Z);

            foreach (JointType type in Enum.GetValues(typeof(JointType)))
            {
                Joint joint = sk.Joints[type];
                joint.Position = Translate(joint.Position, v);
                sk.Joints[type] = joint;
            }

            sk.Position = Translate(sk.Position, v);

            return sk;
        }

        /// <summary>
        /// Translate a skeleton point to another point  
        /// </summary>
        /// <param name="point">The Skeleton Point</param>
        /// <param name="center">The Skeleton Point center</param>
        /// <returns>Returns the Vector3D position.</returns>
        public static SkeletonPoint Translate(SkeletonPoint point, Vector3D translation)
        {
            return new SkeletonPoint() { X = point.X + (float)translation.X, Y = point.Y + (float)translation.Y, Z = point.Z + (float)translation.Z };
        }

        /// <summary>
        /// SkeletonPointToVector3 converts Skeleton Point to Vector3D.  
        /// </summary>
        /// <param name="pt">The Skeleton Point position.</param>
        /// <returns>Returns the Vector3D position.</returns>
        public static Vector3D SkeletonPointToVector3D(SkeletonPoint pt)
        {
            return new Vector3D(pt.X, pt.Y, pt.Z);
        }

        /// <summary>
        /// Vector3ToSkeletonPoint convert Vector3D to Skeleton Point.  
        /// </summary>
        /// <param name="pt">The Vector3D position.</param>
        /// <returns>Returns the Skeleton Point position.</returns>
        public static SkeletonPoint Vector3DToSkeletonPoint(Vector3D pt)
        {
            SkeletonPoint skelPt = new SkeletonPoint();
            skelPt.X = (float)pt.X;
            skelPt.Y = (float)pt.Y;
            skelPt.Z = (float)pt.Z;

            return skelPt;
        }

        /// <summary>
        /// VectorBetween calculates the Vector3D from start to end == subtract start from end 
        /// (a->b = b - a)
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <param name="startJoint">Start joint.</param>
        /// <param name="endJoint">End joint.</param>
        /// <returns>Returns a Vector3D</returns>
        public static Vector3D VectorBetween(Skeleton skeleton, JointType startJoint, JointType endJoint)
        {
            if (null == skeleton)
            {
                return new Vector3D(0, 0, 0);
            }

            Vector3D startPosition = new Vector3D(skeleton.Joints[startJoint].Position.X, skeleton.Joints[startJoint].Position.Y, skeleton.Joints[startJoint].Position.Z);
            Vector3D endPosition = new Vector3D(skeleton.Joints[endJoint].Position.X, skeleton.Joints[endJoint].Position.Y, skeleton.Joints[endJoint].Position.Z);

            return Vector3D.Subtract(endPosition, startPosition);
        }

        /// <summary>
        /// GetShortestRotationBetweenVectors finds the shortest rotation between two vectors and return in quaternion form
        /// </summary>
        /// <param name="vector1">First Vector3</param>
        /// <param name="vector2">Second Vector3</param>
        /// <returns>Returns a Rotation Quaternion</returns>
        public static Quaternion GetShortestRotationBetweenVectors(Vector3D vector1, Vector3D vector2)
        {
            vector1.Normalize();
            vector2.Normalize();
            double angle = Vector3D.AngleBetween(vector1, vector2);
            Vector3D axis = new Vector3D(0, 1, 0);

            // Check to see if the angle is very small, in which case, the cross product becomes unstable,
            // so set the axis to a default.  It doesn't matter much what this axis is, as the rotation angle 
            // will be near zero anyway.
            if (angle < 0.001f)
            {
                axis = new Vector3D(0.0f, 0.0f, 1.0f);
            }

            if (axis.Length < .001f)
            {
                return Quaternion.Identity;
            }

            axis.Normalize();
            Quaternion rot = new Quaternion(axis, angle);

            return rot;
        }

        /// <summary>
        /// CopySkeleton copies the data from another skeleton.
        /// </summary>
        /// <param name="source">The source skeleton.</param>
        /// <param name="destination">The destination skeleton.</param>
        public static void CopySkeleton(Skeleton source, Skeleton destination)
        {
            if (null == source)
            {
                return;
            }

            if (null == destination)
            {
                destination = new Skeleton();
            }

            destination.TrackingState = source.TrackingState;
            destination.TrackingId = source.TrackingId;
            destination.Position = source.Position;
            destination.ClippedEdges = source.ClippedEdges;

            Array jointTypeValues = Enum.GetValues(typeof(JointType));

            // This must copy before the joint orientations
            foreach (JointType j in jointTypeValues)
            {
                Joint temp = destination.Joints[j];
                temp.Position = source.Joints[j].Position;
                temp.TrackingState = source.Joints[j].TrackingState;
                destination.Joints[j] = temp;
            }

            if (null != source.BoneOrientations)
            {
                foreach (JointType j in jointTypeValues)
                {
                    BoneOrientation temp = destination.BoneOrientations[j];
                    temp.HierarchicalRotation.Matrix = source.BoneOrientations[j].HierarchicalRotation.Matrix;
                    temp.HierarchicalRotation.Quaternion = source.BoneOrientations[j].HierarchicalRotation.Quaternion;
                    temp.AbsoluteRotation.Matrix = source.BoneOrientations[j].AbsoluteRotation.Matrix;
                    temp.AbsoluteRotation.Quaternion = source.BoneOrientations[j].AbsoluteRotation.Quaternion;
                    destination.BoneOrientations[j] = temp;
                }
            }
        }
    }
}
