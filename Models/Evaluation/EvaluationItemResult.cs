using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class EvaluationItemResult
    {
        // Evaluation Result ID
        public int ID { get; set; }

        // Evaluation Item
        public virtual EvaluationItem evaluationItem { get; set; }

        // Configuration Profile
        public virtual Configuration conf { get; set; }

        // DTW Gesture recognized
        public virtual GestureModel GestureRecognized { get; set; }

        // DTW Result
        public bool Result { get; set; }
    }
}
