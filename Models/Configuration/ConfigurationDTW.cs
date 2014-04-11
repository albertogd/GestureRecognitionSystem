using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ConfigurationDTW
    {
        // Configuration Field ID
        public int ID { get; set; }

        // Distance measure used (how distance between skeletons is calculated)
        public DistanceType distance { get; set; }

        // Apply boundary constraint at (1, 1)
        public bool boundaryConstraintStart { get; set; }

        // Apply boundary constraint at (m, n)
        public bool boundaryConstraintEnd { get; set; }

        // Diagonal steps in local window for calculation. Results in Ikatura paralelogram shaped dtw-candidate space. Use in combination with slopeStepSizeAside parameter. Leave null for no constraint.
        public int? slopeStepSizeDiagonal { get; set; }

        // Side steps in local window for calculation. Results in Ikatura paralelogram shaped dtw-candidate space. Use in combination with slopeStepSizeDiagonal parameter. Leave null for no constraint.
        public int? slopeStepSizeAside { get; set; }

        // Sakoe-Chiba max shift constraint (side steps). Leave null for no constraint.
        public int? sakoeChibaMaxShift { get; set; }


        // Get Default DTW Configuration
        public static ConfigurationDTW DefaultDTWConf { get { return new ConfigurationDTW() { distance = DistanceType.SelectiveUpperBody, boundaryConstraintStart = true, boundaryConstraintEnd = true, slopeStepSizeDiagonal = null, slopeStepSizeAside = null, sakoeChibaMaxShift = null }; } }
    
        // Add Default DTW Configuration
        public static ConfigurationDTW AddDefaultDTWConf(Context ctx)
        {
            ConfigurationDTW conf = DefaultDTWConf;
            ctx.DTWConfigurations.Add(conf);

            return conf;
        }
    }
}
