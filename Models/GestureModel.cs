using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class GestureModel
    {
        // User ID
        public int ID { get; set; }

        // Configuration associated
        public virtual User user { get; set; }

        // Gesture Type
        public GestureType type { get; set; }

        // Gesture Name
        public string name { get; set; }

        // Img Path
        public string image { get; set; }

        // Gesture File path
        public string file { get; set; }


        // Add Enabled Training Gesture
        public static GestureModel AddEnabledTrainingGesture(Context ctx, User user, string gesturename, string imagedirectory)
        {
            return AddGesture(ctx, user, GestureType.TrainingEnabled, gesturename, imagedirectory);
        }

        // Add Disabled Training Gesture
        public static GestureModel AddDisabledTrainingGesture(Context ctx, User user, string gesturename, string imagedirectory)
        {
            return AddGesture(ctx, user, GestureType.TrainingDisabled, gesturename, imagedirectory);
        }

        // Add Gesture
        public static GestureModel AddGesture(Context ctx, User user, GestureType type, string gesturename, string imagedirectory)
        {
            return ctx.Gestures.Add(new GestureModel() { user = user, name = gesturename, image = imagedirectory, type = type });
        }

        // Add Default Training Gestures
        public static List<GestureModel> AddDefaultTrainingGestures(Context ctx, User user)
        {
            String baseDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            String imgDirectory = Path.Combine(baseDirectory, "Resources", "img", "Gestures");

            // Training Gestures
            List<GestureModel> gestures = new List<GestureModel>(30);

            gestures.Add(AddDisabledTrainingGesture(ctx, user, "Ambas Manos Arriba - Bajar", Path.Combine(imgDirectory, "AmbasManoAbajo.gif")));
            gestures.Add(AddDisabledTrainingGesture(ctx, user, "Ambas Manos Arriba - Bajar de Frente", Path.Combine(imgDirectory, "AmbasManoAbajoDeFrente.gif")));
            gestures.Add(AddDisabledTrainingGesture(ctx, user, "Ambas Manos Abajo - Subir", Path.Combine(imgDirectory, "AmbasManoArriba.gif")));
            gestures.Add(AddDisabledTrainingGesture(ctx, user, "Ambas Manos Abajo - Subir de Frente", Path.Combine(imgDirectory, "AmbasManoArribaDeFrente.gif")));
            gestures.Add(AddEnabledTrainingGesture(ctx, user, "Ambas Manos Abajo - Subir y Bajar", Path.Combine(imgDirectory, "AmbasManoArribaYAbajo.gif")));
            gestures.Add(AddEnabledTrainingGesture(ctx, user, "Ambas Manos Abajo - Subir y Bajar de Frente", Path.Combine(imgDirectory, "AmbasManoArribaYabajoDeFrente.gif")));
            gestures.Add(AddEnabledTrainingGesture(ctx, user, "Ambas Manos Abajo - Subir por completo y Bajar de Frente", Path.Combine(imgDirectory, "AmbasManoArribaYabajoCompletoDeFrente.gif")));

            gestures.Add(AddDisabledTrainingGesture(ctx, user, "Mano Derecha Arriba - Bajar", Path.Combine(imgDirectory, "ManoDerechaAbajo.gif")));
            gestures.Add(AddDisabledTrainingGesture(ctx, user, "Mano Derecha Arriba - Bajar y Subir", Path.Combine(imgDirectory, "ManoDerechaAbajoArriba.gif")));
            gestures.Add(AddDisabledTrainingGesture(ctx, user, "Mano Derecha Arriba - Bajar de Frente", Path.Combine(imgDirectory, "ManoDerechaAbajoDeFrente.gif")));
            gestures.Add(AddDisabledTrainingGesture(ctx, user, "Mano Derecha Abajo - Subir", Path.Combine(imgDirectory, "ManoDerechaArriba.gif")));
            gestures.Add(AddEnabledTrainingGesture(ctx, user, "Mano Derecha Abajo - Subir y Bajar", Path.Combine(imgDirectory, "ManoDerechaArribaAbajo.gif")));
            gestures.Add(AddDisabledTrainingGesture(ctx, user, "Mano Derecha Abajo - Subir de Frente", Path.Combine(imgDirectory, "ManoDerechaArribaDeFrente.gif")));
            gestures.Add(AddEnabledTrainingGesture(ctx, user, "Mano Derecha Abajo - Subir y Bajar de Frente", Path.Combine(imgDirectory, "ManoDerechaArribaYAbajoDeFrente.gif")));
            gestures.Add(AddEnabledTrainingGesture(ctx, user, "Mano Derecha Abajo - Subir por completo y Bajar de Frente", Path.Combine(imgDirectory, "ManoDerechaArribaYAbajoCompletoDeFrente.gif")));
            gestures.Add(AddDisabledTrainingGesture(ctx, user, "Mano Derecha Abajo - Subir Cruzado", Path.Combine(imgDirectory, "ManoDerechaDeAbajoAarribaCruzado.gif")));
            gestures.Add(AddDisabledTrainingGesture(ctx, user, "Mano Derecha Abajo - Subir y Bajar Cruzado", Path.Combine(imgDirectory, "ManoDerechaDeAbajoAarribaAbajoCruzado.gif")));
            gestures.Add(AddDisabledTrainingGesture(ctx, user, "Mano Derecha Abajo - Mover a Izquierda", Path.Combine(imgDirectory, "ManoAbajoDeDerechaAIzquierda.gif")));
            gestures.Add(AddDisabledTrainingGesture(ctx, user, "Mano Derecha Abajo - Mover a Izquierda y a Derecha", Path.Combine(imgDirectory, "ManoAbajoDeDerechaAIzquierdaADerecha.gif")));

            gestures.Add(AddDisabledTrainingGesture(ctx, user, "Mano Izquierda Arriba - Bajar", Path.Combine(imgDirectory, "ManoIzquierdaAbajo.gif")));
            gestures.Add(AddDisabledTrainingGesture(ctx, user, "Mano Izquierda Arriba - Bajar y Subir", Path.Combine(imgDirectory, "ManoIzquierdaAbajoArriba.gif")));
            gestures.Add(AddDisabledTrainingGesture(ctx, user, "Mano Izquierda Arriba - Bajar de Frente", Path.Combine(imgDirectory, "ManoIzquierdaAbajoDeFrente.gif")));
            gestures.Add(AddDisabledTrainingGesture(ctx, user, "Mano Izquierda Abajo - Subir", Path.Combine(imgDirectory, "ManoIzquierdaArriba.gif")));
            gestures.Add(AddEnabledTrainingGesture(ctx, user, "Mano Izquierda Abajo - Subir y Bajar", Path.Combine(imgDirectory, "ManoIzquierdaArribaAbajo.gif")));
            gestures.Add(AddDisabledTrainingGesture(ctx, user, "Mano Izquierda Abajo - Subir de Frente", Path.Combine(imgDirectory, "ManoIzquierdaArribaDeFrente.gif")));
            gestures.Add(AddEnabledTrainingGesture(ctx, user, "Mano Izquierda Abajo - Subir y Bajar de Frente", Path.Combine(imgDirectory, "ManoIzquierdaArribaYAbajoDeFrente.gif")));
            gestures.Add(AddEnabledTrainingGesture(ctx, user, "Mano Izquierda Abajo - Subir por completo y Bajar de Frente", Path.Combine(imgDirectory, "ManoIzquierdaArribaYAbajoCompletoDeFrente.gif")));
            gestures.Add(AddDisabledTrainingGesture(ctx, user, "Mano Izquierda Abajo - Subir Cruzado", Path.Combine(imgDirectory, "ManoIzquierdaDeAbajoAarribaCruzado.gif")));
            gestures.Add(AddDisabledTrainingGesture(ctx, user, "Mano Izquierda Abajo - Subir y Bajar Cruzado", Path.Combine(imgDirectory, "ManoIzquierdaDeAbajoAarribaAabajoCruzado.gif")));
            gestures.Add(AddDisabledTrainingGesture(ctx, user, "Mano Izquierda Abajo - Mover a Derecha", Path.Combine(imgDirectory, "ManoAbajoIzquierdaAderecha.gif")));
            gestures.Add(AddDisabledTrainingGesture(ctx, user, "Mano Izquierda Abajo - Mover a Derecha y a Izquierda", Path.Combine(imgDirectory, "ManoAbajoIzquierdaAderechaAizquierda.gif")));

            gestures.Add(AddEnabledTrainingGesture(ctx, user, "Saludo Mano Derecha", Path.Combine(imgDirectory, "SaludoManoDerecha.gif")));
            gestures.Add(AddEnabledTrainingGesture(ctx, user, "Saludo Mano Izquierda", Path.Combine(imgDirectory, "SaludoManoIzquierda.gif")));

            return gestures;
        }
    }
}
