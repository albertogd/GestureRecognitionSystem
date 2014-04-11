using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class User
    {
        // User ID
        public int ID { get; set; }

        // User Name
        public string name { get; set; }

        // User Password
        public string password { get; set; }

        // User Default Language
        public Language language { get; set; }

        // Method to generate password hash
        public static string GenerateHash(string value)
        {
            var data = System.Text.Encoding.ASCII.GetBytes(value);
            data = System.Security.Cryptography.MD5.Create().ComputeHash(data);
            return Convert.ToBase64String(data);
        }

        // Add user with default configuration
        public static User AddDefaultUser(string name, string password, Language language, Context ctx)
        {
            User user = new User() { name = name, password = User.GenerateHash(password), language = language };
            ctx.Users.Add(user);

            // Training Gestures
            List<GestureModel> gestures = GestureModel.AddDefaultTrainingGestures(ctx, user);
            ctx.SaveChanges();

            // Default Configuration
            Configuration profileDefault = Configuration.AddDefaultConfiguration(ctx, user);
            ctx.SaveChanges();

            // Custom Configurations
            Configuration profileC = Configuration.AddDefaultConfiguration(ctx, user, ConfigurationProfileType.Custom, "Esqueleto centrado");
            profileC.skeletonPreprocessConfiguration.Center = true;
            ctx.SaveChanges();

            Configuration profileBC = Configuration.AddDefaultConfiguration(ctx, user, ConfigurationProfileType.Custom, "Esqueleto Cambio Base");
            profileBC.skeletonPreprocessConfiguration.Rotate = RotateType.ChangeBase;
            ctx.SaveChanges();

            Configuration profileR = Configuration.AddDefaultConfiguration(ctx, user, ConfigurationProfileType.Custom, "Esqueleto Rotado");
            profileR.skeletonPreprocessConfiguration.Rotate = RotateType.Rotate;
            ctx.SaveChanges();

            Configuration profileS = Configuration.AddDefaultConfiguration(ctx, user, ConfigurationProfileType.Custom, "Esqueleto Escalado");
            profileS.skeletonPreprocessConfiguration.Scale = true;
            ctx.SaveChanges();

            Configuration profileCS = Configuration.AddDefaultConfiguration(ctx, user, ConfigurationProfileType.Custom, "Esqueleto Centrado y Escalado");
            profileCS.skeletonPreprocessConfiguration.Center = true;
            profileCS.skeletonPreprocessConfiguration.Scale = true;
            ctx.SaveChanges();

            Configuration profileCR = Configuration.AddDefaultConfiguration(ctx, user, ConfigurationProfileType.Custom, "Esqueleto Centrado y Rotado");
            profileCR.skeletonPreprocessConfiguration.Center = true;
            profileCR.skeletonPreprocessConfiguration.Rotate = RotateType.Rotate;
            ctx.SaveChanges();

            Configuration profileCBC = Configuration.AddDefaultConfiguration(ctx, user, ConfigurationProfileType.Custom, "Esqueleto Centrado y Cambio de Base");
            profileCBC.skeletonPreprocessConfiguration.Center = true;
            profileCBC.skeletonPreprocessConfiguration.Rotate = RotateType.ChangeBase;
            ctx.SaveChanges();

            Configuration profileSR = Configuration.AddDefaultConfiguration(ctx, user, ConfigurationProfileType.Custom, "Esqueleto Escalado y Rotado");
            profileSR.skeletonPreprocessConfiguration.Scale = true;
            profileSR.skeletonPreprocessConfiguration.Rotate = RotateType.Rotate;
            ctx.SaveChanges();

            Configuration profileSBC = Configuration.AddDefaultConfiguration(ctx, user, ConfigurationProfileType.Custom, "Esqueleto Escalado y Cambio de Base");
            profileSBC.skeletonPreprocessConfiguration.Scale = true;
            profileSBC.skeletonPreprocessConfiguration.Rotate = RotateType.ChangeBase;
            ctx.SaveChanges();

            Configuration profileCSBC = Configuration.AddDefaultConfiguration(ctx, user, ConfigurationProfileType.Custom, "Esqueleto Centrado, Escalado y Cambio de Base");
            profileCSBC.skeletonPreprocessConfiguration.Center = true;
            profileCSBC.skeletonPreprocessConfiguration.Scale = true;
            profileCSBC.skeletonPreprocessConfiguration.Rotate = RotateType.ChangeBase;
            ctx.SaveChanges();

            Configuration profileCSR = Configuration.AddDefaultConfiguration(ctx, user, ConfigurationProfileType.Custom, "Esqueleto Centrado, Escalado y Rotado");
            profileCSR.skeletonPreprocessConfiguration.Center = true;
            profileCSR.skeletonPreprocessConfiguration.Scale = true;
            profileCSR.skeletonPreprocessConfiguration.Rotate = RotateType.Rotate;
            ctx.SaveChanges();

            Configuration profileCOA = Configuration.AddDefaultConfiguration(ctx, user, ConfigurationProfileType.Custom, "Esqueleto Centrado - Distancia solo brazos");
            profileCOA.skeletonPreprocessConfiguration.Center = true;
            profileCOA.dtwConfiguration.distance = DistanceType.SelectiveArms;
            ctx.SaveChanges();

            Configuration profileCROA = Configuration.AddDefaultConfiguration(ctx, user, ConfigurationProfileType.Custom, "Esqueleto Centrado y Rotado - Distancia solo brazos");
            profileCROA.skeletonPreprocessConfiguration.Center = true;
            profileCROA.skeletonPreprocessConfiguration.Rotate = RotateType.Rotate;
            profileCROA.dtwConfiguration.distance = DistanceType.SelectiveArms;
            ctx.SaveChanges();

            Configuration profileCRMS1 = Configuration.AddDefaultConfiguration(ctx, user, ConfigurationProfileType.Custom, "CR - sakoeChibaMaxShift = 1");
            profileCRMS1.skeletonPreprocessConfiguration.Center = true;
            profileCRMS1.skeletonPreprocessConfiguration.Rotate = RotateType.Rotate;
            profileCRMS1.dtwConfiguration.sakoeChibaMaxShift = 1;
            ctx.SaveChanges();

            Configuration profileCRMS2 = Configuration.AddDefaultConfiguration(ctx, user, ConfigurationProfileType.Custom, "CR - sakoeChibaMaxShift = 2");
            profileCRMS2.skeletonPreprocessConfiguration.Center = true;
            profileCRMS2.skeletonPreprocessConfiguration.Rotate = RotateType.Rotate;
            profileCRMS2.dtwConfiguration.sakoeChibaMaxShift = 2;
            ctx.SaveChanges();

            Configuration profileCRSS1 = Configuration.AddDefaultConfiguration(ctx, user, ConfigurationProfileType.Custom, "CR - slopeStepSize = 1");
            profileCRSS1.skeletonPreprocessConfiguration.Center = true;
            profileCRSS1.skeletonPreprocessConfiguration.Rotate = RotateType.Rotate;
            profileCRSS1.dtwConfiguration.slopeStepSizeAside = 1;
            profileCRSS1.dtwConfiguration.slopeStepSizeDiagonal = 1;
            ctx.SaveChanges();

            Configuration profileCRSS2 = Configuration.AddDefaultConfiguration(ctx, user, ConfigurationProfileType.Custom, "CR - slopeStepSize = 2");
            profileCRSS2.skeletonPreprocessConfiguration.Center = true;
            profileCRSS2.skeletonPreprocessConfiguration.Rotate = RotateType.Rotate;
            profileCRSS2.dtwConfiguration.slopeStepSizeAside = 2;
            profileCRSS2.dtwConfiguration.slopeStepSizeDiagonal = 2;
            ctx.SaveChanges();

            Configuration profileCRSQ = Configuration.AddDefaultConfiguration(ctx, user, ConfigurationProfileType.Custom, "CR - Squared");
            profileCRSQ.skeletonPreprocessConfiguration.Center = true;
            profileCRSQ.skeletonPreprocessConfiguration.Rotate = RotateType.Rotate;
            profileCRSQ.dtwConfiguration.distance = DistanceType.SelectiveUpperBodySquared;
            ctx.SaveChanges();

            Configuration profileCRCUB = Configuration.AddDefaultConfiguration(ctx, user, ConfigurationProfileType.Custom, "CR - Cubed");
            profileCRCUB.skeletonPreprocessConfiguration.Center = true;
            profileCRCUB.skeletonPreprocessConfiguration.Rotate = RotateType.Rotate;
            profileCRCUB.dtwConfiguration.distance = DistanceType.SelectiveUpperBodyCube;
            ctx.SaveChanges();

            // Default System Configuration
            SystemConfiguration systemConf = SystemConfiguration.AddDefaultSystemConfiguration(ctx, user);
            systemConf.movementDetectionProfile = profileDefault;
            systemConf.freeModeDetectionProfile = profileCR;
            ctx.SaveChanges();
            
            //  Add profiles to Custom Evaluation
            systemConf.customEvaluationProfiles(profileDefault, profileC, profileBC, profileR, profileS, profileCS, profileCR, profileCBC, profileSR, profileSBC, profileCSBC, profileCSR, profileCOA, profileCROA, profileCRMS1, profileCRMS2, profileCRSS1, profileCRSS2, profileCRSQ, profileCRCUB);
            ctx.SaveChanges();

            // Default Mindstorm Configuration
            MindstormConfiguration.AddDefaultMindstormConfiguration(ctx, user);

            ctx.SaveChanges();

            return user;
        }
    }
}
