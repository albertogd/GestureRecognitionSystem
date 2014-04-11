using System;
using System.Collections.Generic;
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
    public partial class ModifyGesturesPage : Page
    {
        private Context ctx;
        private Gesture gesture;


        public ModifyGesturesPage()
        {
            InitializeComponent();

            ctx = new Context();
            GestureList.ItemsSource = ctx.Gestures.Where(gesture => gesture.user.name == HomeWindow.username && gesture.type == GestureType.TrainingEnabled).ToDictionary(gesture => gesture.ID, gesture => gesture.name);
            ConfigurationMovement conf = ctx.Configurations.FirstOrDefault(c => c.user.name == HomeWindow.username).movementConfiguration;
            EnergyBar.Maximum = conf.ET_RestingToStartMoving * 10;
        }

        private void GestureList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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

        private void ReRecord_Click(object sender, RoutedEventArgs e)
        {
            string gesturename = GestureList.SelectedItem.ToString();
            AddGesturesPage add = new AddGesturesPage();
            add.GestureName.Text = gesturename;
            add.GestureName.IsReadOnly = true;
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(add);
        }
    }
}
