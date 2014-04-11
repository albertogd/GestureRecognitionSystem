using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Algorithms;
using Algorithms.Record;
using Models;

namespace GestureEvaluator.Pages.Training.Gestures
{
    /// <summary>
    /// Interaction logic for ModifyGestures.xaml
    /// </summary>
    public partial class PlayGesturePage : Page
    {
        private Context ctx;
        private Gesture gesture;


        public PlayGesturePage()
        {
            InitializeComponent();

            ctx = new Context();
            GestureList.ItemsSource = ctx.Gestures.Where(gesture => gesture.user.name == HomeWindow.username && gesture.type == GestureType.TrainingEnabled && !String.IsNullOrEmpty(gesture.file)).ToDictionary(gesture => gesture.ID, gesture => gesture.name);
            ConfigurationMovement conf = ctx.Configurations.FirstOrDefault(c => c.user.name == HomeWindow.username).movementConfiguration;
            EnergyBar.Maximum = conf.ET_RestingToStartMoving * 10;
        }

        private void sensor_SkeletonFrameReadyReplay(object sender, ReplaySkeletonFrameReadyEventArgs e)
        {
            ReplaySkeletonFrame frame = e.SkeletonFrame;

            if (frame == null)
                return;

            gesture.Replay(frame);
            EnergyBar.Value = (gesture.energy > EnergyBar.Maximum) ? EnergyBar.Maximum : gesture.energy;
            ProgressValue.Text = gesture.energy.ToString();

            switch (gesture.state)
            {
                case State.Resting:
                    StateText.Text = "Rest";
                    break;
                case State.StartMoving:
                    StateText.Text = "Start";
                    break;
                case State.Moving:
                    StateText.Text = "Moving";
                    break;
                case State.StopMoving:
                    StateText.Text = "Stop";
                    break;
                case State.Finish:
                    StateText.Text = "Finish";
                    break;
            }

            painter.Draw(frame.Skeletons);
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (GestureList.SelectedItem == null)
                return;

            GestureModel model = ctx.Gestures.Find(((KeyValuePair<int, string>)GestureList.SelectedItem).Key);

            if (model == null)
                return;

            double speed = 1;
            if (!String.IsNullOrEmpty(Speed.Text))
                Double.TryParse(Speed.Text, out speed);

            gesture = new Gesture(model);
            gesture.Replay(sensor_SkeletonFrameReadyReplay, speed);
        }
    }
}
