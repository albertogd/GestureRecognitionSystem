using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Models;

namespace Algorithms.DTW
{
    public class Distances
    {
        /// <summary>
        /// Skeletion Distance Type
        /// </summary>
        public DistanceType Type;

        /// <summary>
        /// Skeletion Distance Type
        /// </summary>
        public double Threshold;

        /// <summary>
        /// Skeletion Distance Type
        /// </summary>
        public List<JointType> Joints;


        public Distances(DistanceType type = DistanceType.Normal, List<JointType> joints = null, double threshold = 0.3)
        {
            Type = type;
            Joints = joints;
            Threshold = threshold;

            if (type == DistanceType.SelectiveUpperBody || type == DistanceType.SelectiveUpperBodySquared || type == DistanceType.SelectiveUpperBodyCube || type == DistanceType.SelectiveWithThreshold)
                Joints = new List<JointType>() { JointType.ElbowLeft, JointType.ElbowRight, JointType.HandLeft, JointType.HandRight, JointType.ShoulderLeft, JointType.ShoulderRight, JointType.WristLeft, JointType.WristRight };

            if (type == DistanceType.SelectiveArms)
                Joints = new List<JointType>() { JointType.ElbowLeft, JointType.ElbowRight, JointType.HandLeft, JointType.HandRight, JointType.WristLeft, JointType.WristRight };
        }


        /// <summary>
        /// Computes distance between 2 skeletons
        /// </summary>
        /// <param name="a">Skeleton a</param>
        /// <param name="b">Skeleton b</param>
        /// <returns>Type Distance between two skeletons</returns>
        public double Distance(Skeleton a, Skeleton b)
        {
            switch(Type)
            {
                case DistanceType.Normal:
                    return NormalDistance(a, b);
                
                case DistanceType.Selective:
                    return SelectiveDistance(a, b, Joints);
                
                case DistanceType.Threshold:
                    return ThresholdDistance(a, b, Threshold);
                
                case DistanceType.SelectiveWithThreshold:
                    return SelectiveDistanceWithThreshold(a, b, Joints, Threshold);

                case DistanceType.SelectiveUpperBody:
                case DistanceType.SelectiveArms:
                    return SelectiveDistance(a, b, Joints);

                case DistanceType.SelectiveUpperBodySquared:
                    return SelectiveDistancePow(a, b, Joints, 2);

                case DistanceType.SelectiveUpperBodyCube:
                    return SelectiveDistancePow(a, b, Joints, 3);
            }

            throw new Exception("Distance Type not defined");
        }

        /// <summary>
        /// Computes normal distance between 2 skeletons
        /// </summary>
        /// <param name="a">Skeleton a</param>
        /// <param name="b">Skeleton b</param>
        /// <returns>Normal distance between two skeletons</returns>
        public static double NormalDistance(Skeleton a, Skeleton b)
        {
            return a.Joints.Join(b.Joints, aj => aj.JointType, bj => bj.JointType, (aj, bj) => EuclideanDistance(aj, bj)).Sum();
        }

        /// <summary>
        /// Computes selective distance between 2 skeletons
        /// </summary>
        /// <param name="a">Skeleton a</param>
        /// <param name="b">Skeleton b</param>
        /// <param name="joints">Selective Joints</param>
        /// <returns>Normal distance between two skeletons</returns>
        public static double SelectiveDistance(Skeleton a, Skeleton b, List<JointType> JointType)
        {
            List<Joint> selectiveJoints = a.Joints.Where(joint => JointType.Contains(joint.JointType)).ToList();

            return selectiveJoints.Join(b.Joints, aj => aj.JointType, bj => bj.JointType, (aj, bj) => EuclideanDistance(aj, bj)).Sum();
        }

        /// <summary>
        /// Computes selective distance between 2 skeletons
        /// </summary>
        /// <param name="a">Skeleton a</param>
        /// <param name="b">Skeleton b</param>
        /// <param name="joints">Selective Joints</param>
        /// <returns>Normal distance between two skeletons</returns>
        public static double SelectiveDistancePow(Skeleton a, Skeleton b, List<JointType> JointType, double pow)
        {
            List<Joint> selectiveJoints = a.Joints.Where(joint => JointType.Contains(joint.JointType)).ToList();

            double sum = selectiveJoints.Join(b.Joints, aj => aj.JointType, bj => bj.JointType, (aj, bj) => EuclideanDistance(aj, bj)).Select(d => Math.Pow(d, pow)).Sum();

            return Math.Pow(sum, 1.0 / pow);
        }

        /// <summary>
        /// Computes distance with threshold between 2 skeletons
        /// </summary>
        /// <param name="a">Skeleton a</param>
        /// <param name="b">Skeleton b</param>
        /// <param name="threshold">Threshold</param>
        /// <returns>Distance with threshold between 2 skeletons</returns>
        public static double ThresholdDistance(Skeleton a, Skeleton b, double threshold)
        {
            return a.Joints.Join(b.Joints, aj => aj.JointType, bj => bj.JointType, (aj, bj) => EuclideanDistance(aj, bj)).Where(d => d > threshold).Sum();
        }

        /// <summary>
        /// Computes selective distance with threshold between 2 skeletons
        /// </summary>
        /// <param name="a">Skeleton a</param>
        /// <param name="b">Skeleton b</param>
        /// <param name="Joints">Selective Joints</param>
        /// <param name="threshold">Threshold</param>
        /// <returns>Distance with threshold between 2 skeletons</returns>
        public static double SelectiveDistanceWithThreshold(Skeleton a, Skeleton b, List<JointType> JointType, double threshold)
        {
            List<Joint> selectiveJoints = a.Joints.Where(joint => JointType.Contains(joint.JointType)).ToList();
            List<double> distances = selectiveJoints.Join(b.Joints, aj => aj.JointType, bj => bj.JointType, (aj, bj) => EuclideanDistance(aj, bj)).ToList();
            List<double> filter = distances.Where(d => d > threshold).ToList();
            double sum = filter.Sum();
            return sum;

            //return selectiveJoints.Join(b.Joints, aj => aj.JointType, bj => bj.JointType, (aj, bj) => EuclideanDistance(aj, bj)).Where(d => d > threshold).Sum();
        }

        /// <summary>
        /// Computes selective distance with threshold between 2 skeletons
        /// </summary>
        /// <param name="a">Skeleton a</param>
        /// <param name="b">Skeleton b</param>
        /// <param name="Joints">Selective Joints</param>
        /// <param name="threshold">Threshold</param>
        /// <returns>Distance with threshold between 2 skeletons</returns>
        public static double SelectiveDistanceWithThresholdAndPow(Skeleton a, Skeleton b, List<JointType> JointType, double threshold, double pow)
        {
            List<Joint> selectiveJoints = a.Joints.Where(joint => JointType.Contains(joint.JointType)).ToList();

            return selectiveJoints.Join(b.Joints, aj => aj.JointType, bj => bj.JointType, (aj, bj) => EuclideanDistance(aj, bj)).Where(d => d > threshold).Select(d => Math.Pow(d, pow)).Sum();
        }

        /// <summary>
        /// Computes euclidean between 2 joints
        /// </summary>
        /// <param name="a">Joint a</param>
        /// <param name="b">Joint b</param>
        /// <returns>Euclidean Distance between 2 joints</returns>
        public static double EuclideanDistance(Joint a, Joint b)
        {
            float x = (a.Position.X - b.Position.X);
            float y = (a.Position.Y - b.Position.Y);
            float z = (a.Position.Z - b.Position.Z);

            return Math.Sqrt(x*x + y*y + z*z);
        }
    }
}
