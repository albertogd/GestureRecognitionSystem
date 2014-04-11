using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class EvaluationItem
    {
        // Evaluation Item ID
        public int ID { get; set; }

        // Evaluation
        public virtual Evaluation evaluation { get; set; }

        // Gesture played
        public virtual GestureModel gesturePlayed { get; set; }

        // Gesture recorded
        public virtual GestureModel gestureRecorded { get; set; }
    }
}
