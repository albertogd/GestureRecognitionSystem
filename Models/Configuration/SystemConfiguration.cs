using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class SystemConfiguration
    {
        // Configuration Field ID
        public int ID { get; set; }

        // User Configuration
        public virtual User user { get; set; }

        // Movement Detection Profile
        public Configuration movementDetectionProfile { get; set; }

        // Training Automatic
        public bool trainingAutomatic { get; set; }

        // Free Mode Automatic
        public bool freeModeAutomatic { get; set; }

        // Free Mode Detection Profile
        public Configuration freeModeDetectionProfile { get; set; }

        // Random Evaluation Number of gestures
        public int randomEvaluationNumOfGestures { get; set; }

        // Random Evaluation Automatic
        public bool randomEvaluationAutomatic { get; set; }

        // Random Evaluation Profiles
        public string randomEvaluationProfileSerialized { get; set; }

        public List<Configuration> randomEvaluationProfiles(Context ctx)
        {
            if (String.IsNullOrEmpty(randomEvaluationProfileSerialized))
                return new List<Configuration>();
            else
                return randomEvaluationProfileSerialized.Split(':').Select(id => ctx.Configurations.Find(Int32.Parse(id))).ToList();
        }

        public void randomEvaluationProfiles(params Configuration[] profiles)
        {
            if (profiles.Length == 0)
                randomEvaluationProfileSerialized = "";
            else if (profiles.Length == 1)
                randomEvaluationProfileSerialized = profiles[0].ID.ToString();
            else
                randomEvaluationProfileSerialized = profiles.Select(g => g.ID.ToString()).Aggregate((a, b) => a + ":" + b);
        }

        // Custom Evaluation Automatic
        public bool customEvaluationAutomatic { get; set; }

        // Custom Evaluation Gestures
        public string customEvaluationGestureSerialized { get; set; }

        public List<GestureModel> customEvaluationGestures(Context ctx)
        {
            if (String.IsNullOrEmpty(customEvaluationGestureSerialized))
                return new List<GestureModel>();
            else
                return customEvaluationGestureSerialized.Split(':').Select(id => ctx.Gestures.Find(Int32.Parse(id))).ToList();
        }

        public void customEvaluationGestures(params GestureModel[] gestures)
        {
            if (gestures.Length == 0)
                customEvaluationGestureSerialized = "";
            else if (gestures.Length == 1)
                customEvaluationGestureSerialized = gestures[0].ID.ToString();
            else
                customEvaluationGestureSerialized = gestures.Select(g => g.ID.ToString()).Aggregate((a, b) => a + ":" + b);
        }


        // Custom Evaluation Profiles
        public string customEvaluationProfileSerialized { get; set; }

        public List<Configuration> customEvaluationProfiles(Context ctx)
        {
            if (String.IsNullOrEmpty(customEvaluationProfileSerialized))
                return new List<Configuration>();
            else
                return customEvaluationProfileSerialized.Split(':').Select(id => ctx.Configurations.Find(Int32.Parse(id))).ToList();
        }

        public void customEvaluationProfiles(params Configuration[] profiles)
        {
            if (profiles.Length == 0)
                customEvaluationProfileSerialized = "";
            else if(profiles.Length == 1)
                customEvaluationProfileSerialized = profiles[0].ID.ToString();
            else
                customEvaluationProfileSerialized = profiles.Select(g => g.ID.ToString()).Aggregate((a, b) => a + ":" + b);
        }

        // Get Default System Configuration
        public static SystemConfiguration GetDefaultSystemConfiguration(Context ctx, User user)
        {
            List<GestureModel> gestures = ctx.Gestures.Where(g => g.user.name == user.name && g.type == GestureType.TrainingEnabled).ToList();

            SystemConfiguration conf = new SystemConfiguration() { user = user, trainingAutomatic = true, freeModeAutomatic = true, randomEvaluationAutomatic = true, customEvaluationAutomatic = true, randomEvaluationNumOfGestures = 10 };
            conf.customEvaluationGestures(gestures.ToArray());

            return conf;
        }

        // Add Default Configuration
        public static SystemConfiguration AddDefaultSystemConfiguration(Context ctx, User user)
        {
            return ctx.SystemConfigurations.Add(SystemConfiguration.GetDefaultSystemConfiguration(ctx, user));
        }

    }
}
