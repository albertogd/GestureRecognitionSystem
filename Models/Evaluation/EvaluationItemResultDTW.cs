using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class EvaluationItemResultDTW : IComparable<EvaluationItemResultDTW>
    {
        // Evaluation Result ID
        public int ID { get; set; }

        // Evaluation Result
        public virtual EvaluationItemResult EvaluationItemResult { get; set; }

        // Gesture
        public virtual GestureModel Gesture { get; set; }

        // Distance
        public double distance { get; set; }

        // IComparable
        public int CompareTo(EvaluationItemResultDTW other)
        {
            if (other == null)
                return 1;

            return distance.CompareTo(other.distance);
        }

    }
}
