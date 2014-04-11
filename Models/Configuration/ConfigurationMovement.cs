using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ConfigurationMovement
    {
        // Configuration Field ID
        public int ID { get; set; }

        /** ET = Energy thresholds **/
        // Energy Threshold from Resting to Moving;
        public double ET_RestingToStartMoving { get; set; }

        // Energy Threshold from Resting to Moving;
        public double ET_StartMovingToResting { get; set; }

        // Energy Threshold from Resting to Moving;
        public double ET_StartMovingToMoving { get; set; }

        // Energy Threshold from Resting to Moving;
        public double ET_MovingToStopMoving { get; set; }

        // Energy Threshold from Resting to Moving;
        public double ET_StopMovingToMoving { get; set; }

        // Energy Threshold from Resting to Moving;
        public double ET_StopMovingToResting { get; set; }

        /** TT = Time thresholds **/
        // Energy Threshold from Resting to Moving;
        public int TT_RestingToStartMoving { get; set; }

        // Energy Threshold from Resting to Moving;
        public int TT_StartMovingToResting { get; set; }

        // Energy Threshold from Resting to Moving;
        public int TT_StartMovingToMoving { get; set; }

        // Energy Threshold from Resting to Moving;
        public int TT_MovingToStopMoving { get; set; }

        // Energy Threshold from Resting to Moving;
        public int TT_StopMovingToMoving { get; set; }

        // Energy Threshold from Resting to Moving;
        public int TT_StopMovingToResting { get; set; }

        // Get Default Movement Configuration
        public static ConfigurationMovement DefaultMovementConfiguration { get { return new ConfigurationMovement() { ET_MovingToStopMoving = 0.08, ET_RestingToStartMoving = 0.08, ET_StartMovingToMoving = 0.14, ET_StartMovingToResting = 0.08, ET_StopMovingToMoving = 0.14, ET_StopMovingToResting = 0.08, TT_MovingToStopMoving = 6, TT_RestingToStartMoving = 2, TT_StartMovingToMoving = 4, TT_StartMovingToResting = 2, TT_StopMovingToMoving = 0, TT_StopMovingToResting = 4 }; } }

        // Add Default Movement Configuration
        public static ConfigurationMovement AddDefaultMovementConfiguration(Context ctx)
        {
            ConfigurationMovement conf = DefaultMovementConfiguration;
            ctx.MovementConfigurations.Add(conf);

            return conf;
        }
    }
}
