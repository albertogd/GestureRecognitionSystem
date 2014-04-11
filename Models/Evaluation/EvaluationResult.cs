using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class EvaluationResult
    {
        // Evaluation Result ID
        public int ID { get; set; }

        // Evaluation
        public virtual Evaluation evaluation { get; set; }

        // Configuration Profile
        public virtual Configuration conf { get; set; }

        // Sucess Result
        public int Success { get; set; }

        // Total
        public int Total { get; set; }
    }
}
