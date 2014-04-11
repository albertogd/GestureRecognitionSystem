using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ConfigurationGestureRecognizer
    {
        // Configuration Field ID
        public int ID { get; set; }

        // Distance between first and second recognized gestures needed to consider a valid detection
        public double? distanceRecognizerThreshold { get; set; }


        // Get Default DTW Configuration
        public static ConfigurationGestureRecognizer DefaultGRConf { get { return new ConfigurationGestureRecognizer() { distanceRecognizerThreshold = 10 }; } }

        // Add Default DTW Configuration
        public static ConfigurationGestureRecognizer AddDefaultGRConf(Context ctx)
        {
            ConfigurationGestureRecognizer conf = DefaultGRConf;
            ctx.GRConfigurations.Add(conf);

            return conf;
        }
    }
}
