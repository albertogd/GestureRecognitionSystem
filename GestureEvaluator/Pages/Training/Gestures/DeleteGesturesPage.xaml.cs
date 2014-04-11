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
using Microsoft.Kinect;
using Algorithms;
using Algorithms.Record;
using Models;

namespace GestureEvaluator.Pages.Training.Gestures
{
    /// <summary>
    /// Interaction logic for DeleteGestures.xaml
    /// </summary>
    public partial class DeleteGesturesPage : Page
    {
        private Context ctx;
        private Gesture gesture;


        public DeleteGesturesPage()
        {
            InitializeComponent();

            ctx = new Context();
            GestureList.ItemsSource = ctx.Gestures.Where(gesture => gesture.user.name == HomeWindow.username && gesture.type == GestureType.TrainingEnabled && !String.IsNullOrEmpty(gesture.file)).ToDictionary(gesture => gesture.ID, gesture => gesture.name);
            ConfigurationMovement conf = ctx.Configurations.FirstOrDefault(c => c.user.name == HomeWindow.username).movementConfiguration;
            EnergyBar.Maximum = conf.ET_RestingToStartMoving * 10;
        }

        private void DeleteGesture_Click(object sender, RoutedEventArgs e)
        {
            GestureModel gesture = ctx.Gestures.Find(((KeyValuePair<int, string>)GestureList.SelectedItem).Key);

            try
            {
                if (!String.IsNullOrEmpty(gesture.file) && File.Exists(gesture.file))
                    File.Delete(gesture.file);
            }
            catch
            {
            }

            gesture.file = null;
            ctx.SaveChanges();

            GestureList.ItemsSource = ctx.Gestures.Where(g => g.user.name == HomeWindow.username && g.type == GestureType.TrainingEnabled && !String.IsNullOrEmpty(g.file)).ToDictionary(g => g.ID, g => g.name);
        }

        private void GestureList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GestureList.SelectedItem == null)
                return;

            GestureModel model = ctx.Gestures.Find(((KeyValuePair<int, string>)GestureList.SelectedItem).Key);

            if (model == null)
                return;

            gesture = new Gesture(model);
            gesture.Replay(sensor_SkeletonFrameReadyReplay);
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
    }
}
