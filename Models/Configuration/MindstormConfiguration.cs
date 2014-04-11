using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class MindstormConfiguration
    {
        // Configuration Field ID
        public int ID { get; set; }

        // User Configuration
        public virtual User user { get; set; }

        // USB or Wifi
        public ConnectionType connection { get; set; }

        // Recognition
        public RecognitionType recognition { get; set; }

        // Recognition Profile
        public Configuration profile { get; set; }

        // Move Right Arm Up or Down
        public GestureModel rightArmMove { get; set; }

        // Move Right Arm Up
        public GestureModel rightArmUp { get; set; }

        // Move Right Arm Down
        public GestureModel rightArmDown { get; set; }

        // Move Left Arm Up or Down
        public GestureModel leftArmMove { get; set; }

        // Move Left Arm Up
        public GestureModel leftArmUp { get; set; }

        // Move Left Arm Down
        public GestureModel leftArmDown { get; set; }

        // Move Left Arms Up or Down
        public GestureModel bothArmsMove { get; set; }

        // Move Left Arms Up
        public GestureModel bothArmsUp { get; set; }

        // Move Both Arms Down
        public GestureModel bothArmsDown { get; set; }

        // Move Right Foot Forward or Backward
        public GestureModel rightFootMove { get; set; }

        // Move Right Foot Forward
        public GestureModel rightFootForward { get; set; }

        // Move Right Foot Backward
        public GestureModel rightFootBackward { get; set; }

        // Move Left Foot Forward or Backward
        public GestureModel leftFootMove { get; set; }

        // Move Left Foot Forward
        public GestureModel leftFootForward { get; set; }

        // Move Left Foot Backward
        public GestureModel leftFootBackward { get; set; }

        // Move Both Arms Forward or Backward
        public GestureModel bothFeetMove { get; set; }

        // Move Both Arms Forward
        public GestureModel bothFeetForward { get; set; }

        // Move Both Arms Backward
        public GestureModel bothFeetBackward { get; set; }


        // Get Default Mindstorm Configuration
        public static MindstormConfiguration DefaultMindstormConfiguration(Context ctx, User user)
        {
            GestureModel rightArmMove = ctx.Gestures.FirstOrDefault(g => g.user.ID == user.ID && g.name == "Mano Derecha Abajo - Subir y Bajar de Frente");
            GestureModel leftArmMove = ctx.Gestures.FirstOrDefault(g => g.user.ID == user.ID && g.name == "Mano Izquierda Abajo - Subir y Bajar de Frente");
            GestureModel BothArmsMove = ctx.Gestures.FirstOrDefault(g => g.user.ID == user.ID && g.name == "Ambas Manos Abajo - Subir y Bajar de Frente");
            GestureModel rightFootMove = ctx.Gestures.FirstOrDefault(g => g.user.ID == user.ID && g.name == "Mano Derecha Abajo - Subir y Bajar");
            GestureModel leftFootMove = ctx.Gestures.FirstOrDefault(g => g.user.ID == user.ID && g.name == "Mano Izquierda Abajo - Subir y Bajar");
            GestureModel bothFeetMove = ctx.Gestures.FirstOrDefault(g => g.user.ID == user.ID && g.name == "Ambas Manos Abajo - Subir y Bajar");
            
            Configuration conf = ctx.Configurations.Where(c => c.user.ID == user.ID).FirstOrDefault(c => c.name == "Esqueleto centrado");
            conf = (conf != null) ? conf : ctx.Configurations.FirstOrDefault(c => c.user.ID == user.ID && c.type == ConfigurationProfileType.Default);

            return new MindstormConfiguration() { user = user, connection = ConnectionType.USB, profile = conf, recognition = RecognitionType.Visual, rightArmMove = rightArmMove, leftArmMove = leftArmMove, bothArmsMove = BothArmsMove, rightFootMove = rightFootMove, leftFootMove = leftFootMove, bothFeetMove = bothFeetMove };
        }

        // Add Default Configuration
        public static MindstormConfiguration AddDefaultMindstormConfiguration(Context ctx, User user)
        {
            return ctx.MindstormConfigurations.Add(MindstormConfiguration.DefaultMindstormConfiguration(ctx, user));
        }
    }
}
