using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Configuration
    {
        // Algorithms Configuration ID
        public int ID { get; set; }

        // User Configuration
        public virtual User user { get; set; }

        // Configuration Profile Name
        public string name { get; set; }

        // Configuration Profile Type (Default or Custom)
        public ConfigurationProfileType type { get; set; }

        // Gesture Recognizer Configuration
        public virtual ConfigurationGestureRecognizer gestureRecognizerConfiguration { get; set; }

        // DTW Configuration
        public virtual ConfigurationDTW dtwConfiguration { get; set; }

        // Movement Configuration
        public virtual ConfigurationMovement movementConfiguration { get; set; }

        // Skeleton Preprocess Configuration
        public virtual ConfigurationSkeletonPreprocess skeletonPreprocessConfiguration { get; set; }

        // Add Default Configuration
        public static Configuration AddDefaultConfiguration(Context ctx, User user, ConfigurationProfileType profileType = ConfigurationProfileType.Default, string profileName = "Default")
        {
            ConfigurationGestureRecognizer grConf = ConfigurationGestureRecognizer.AddDefaultGRConf(ctx);
            ConfigurationDTW dConf = ConfigurationDTW.AddDefaultDTWConf(ctx);
            ConfigurationMovement mConf = ConfigurationMovement.AddDefaultMovementConfiguration(ctx);
            ConfigurationSkeletonPreprocess sConf = ConfigurationSkeletonPreprocess.AddDefaultDTWConf(ctx);
            Configuration conf = new Configuration() { user = user, name = profileName, type = profileType, movementConfiguration = mConf, gestureRecognizerConfiguration = grConf, dtwConfiguration = dConf, skeletonPreprocessConfiguration = sConf };

            return ctx.Configurations.Add(conf);
        }
    }
}
