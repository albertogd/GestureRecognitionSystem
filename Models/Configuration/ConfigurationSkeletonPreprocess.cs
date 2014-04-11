using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ConfigurationSkeletonPreprocess
    {
        // Configuration Field ID
        public int ID { get; set; }

        // Set Skeleton´s Shoulder Center as Center Point
        public bool Center { get; set; }

        // Scale Skeleton to always be same size
        public bool Scale { get; set; }

        // Change Base so Skeleton is always in front of Kinect
        public RotateType Rotate { get; set; }


        // Get Default DTW Configuration
        public static ConfigurationSkeletonPreprocess DefaultPreprocessConf { get { return new ConfigurationSkeletonPreprocess() { Center = false, Scale = false, Rotate = RotateType.None }; } }

        // Add Default DTW Configuration
        public static ConfigurationSkeletonPreprocess AddDefaultDTWConf(Context ctx)
        {
            ConfigurationSkeletonPreprocess conf = DefaultPreprocessConf;
            ctx.SkeletonPreprocessConfigurations.Add(conf);

            return conf;
        }
    }
}
