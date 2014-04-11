using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Microsoft.Kinect;

namespace Algorithms.Preprocessing
{
    public class Base
    {
        private Matrix3D vBase;
        private Matrix3D vChangeBase;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="i">Base Vector i</param>
        /// <param name="j">Base Vector j</param>
        /// <param name="k">Base Vector k</param>
        public Base(Vector3D i, Vector3D j, Vector3D k)
        {
            vBase = new Matrix3D(i.X, j.X, k.X, 0, i.Y, j.Y, k.Y, 0, i.Z, j.Z, k.Z, 0, 0, 0, 0, 1);
            vChangeBase = new Matrix3D(i.X, j.X, k.X, 0, i.Y, j.Y, k.Y, 0, i.Z, j.Z, k.Z, 0, 0, 0, 0, 1);

            if(vChangeBase.HasInverse)
                vChangeBase.Invert();
        }

        public Point3D ChangeBase(Point3D point)
        {
            return Multiply(vChangeBase, point);
        }

        public SkeletonPoint ChangeBase(SkeletonPoint p)
        {
            Point3D p3d = new Point3D(p.X, p.Y, p.Z);
            Point3D np3d = ChangeBase(p3d);

            return new SkeletonPoint() { X = (float)np3d.X, Y = (float)np3d.Y, Z = (float)np3d.Z };
        }

        public Point3D Multiply(Matrix3D m, Point3D p)
        {
            double p1 = m.M11 * p.X + m.M12 * p.Y + m.M13 * p.Z;
            double p2 = m.M21 * p.X + m.M22 * p.Y + m.M23 * p.Z;
            double p3 = m.M31 * p.X + m.M32 * p.Y + m.M33 * p.Z;

            return new Point3D(p1, p2, p3);
        }
    }
}
