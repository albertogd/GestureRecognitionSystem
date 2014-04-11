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
    /// Interaction logic for AddGestures.xaml
    /// </summary>
    public partial class AddGesturesPage : Page
    {
        private Context ctx;
        private KinectSensor sensor;
        private Gesture gesture;

        public AddGesturesPage()
        {
            InitializeComponent();

            this.Loaded += (s, e) =>  { NavigationService.Navigating += NavigationService_Navigating; };

            ErrorMsg.Visibility = System.Windows.Visibility.Hidden;
            Msg.Visibility = System.Windows.Visibility.Hidden;
            StateMsg.Visibility = System.Windows.Visibility.Hidden;
            EnergyBar.Visibility = System.Windows.Visibility.Hidden;
            ReRecord.Visibility = System.Windows.Visibility.Hidden;
            RepeatGesture.Visibility = System.Windows.Visibility.Hidden;
            RecordButton.Visibility = System.Windows.Visibility.Visible;
            GestureName.Visibility = System.Windows.Visibility.Visible;
            NameLabel.Visibility = System.Windows.Visibility.Visible;

            try
            {
                sensor = KinectSensor.KinectSensors.FirstOrDefault(k => k.Status == KinectStatus.Connected);

                if (sensor == null)
                {
                    ErrorMsg.Text = "Error: No se ha detectado ningún Kinect en el equipo";
                    ErrorMsg.Visibility = System.Windows.Visibility.Visible;
                    Msg.Visibility = System.Windows.Visibility.Hidden;
                    EnergyBar.Visibility = System.Windows.Visibility.Hidden;
                    StateMsg.Visibility = System.Windows.Visibility.Hidden;
                    ReRecord.Visibility = System.Windows.Visibility.Hidden;
                    RepeatGesture.Visibility = System.Windows.Visibility.Hidden;
                    RecordButton.Visibility = System.Windows.Visibility.Hidden;
                    GestureName.Visibility = System.Windows.Visibility.Hidden;
                    NameLabel.Visibility = System.Windows.Visibility.Hidden;
                    return;
                }


            }
            catch
            {
                ErrorMsg.Text = "Error al establecer comunicación con el Kinect";
                ErrorMsg.Visibility = System.Windows.Visibility.Visible;
                Msg.Visibility = System.Windows.Visibility.Hidden;
                EnergyBar.Visibility = System.Windows.Visibility.Hidden;
                StateMsg.Visibility = System.Windows.Visibility.Hidden;
                ReRecord.Visibility = System.Windows.Visibility.Hidden;
                RepeatGesture.Visibility = System.Windows.Visibility.Hidden;
                RecordButton.Visibility = System.Windows.Visibility.Hidden;
                GestureName.Visibility = System.Windows.Visibility.Hidden;
                NameLabel.Visibility = System.Windows.Visibility.Hidden;
                return;
            }

            ctx = new Context();
            ConfigurationMovement conf = ctx.Configurations.FirstOrDefault(c => c.user.name == HomeWindow.username).movementConfiguration;
            EnergyBar.Maximum = conf.ET_RestingToStartMoving * 10;

            sensor.SkeletonFrameReady += sensor_SkeletonFrameReadyRecord;
            sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
            sensor.SkeletonStream.Enable();
        }

        private void Record_Click(object sender, RoutedEventArgs e)
        {
            Msg.Text = "Grabando gesto " + GestureName.Text;
            Msg.Visibility = System.Windows.Visibility.Visible;
            StateMsg.Visibility = System.Windows.Visibility.Visible;
            EnergyBar.Visibility = System.Windows.Visibility.Visible;
            ErrorMsg.Visibility = System.Windows.Visibility.Hidden;
            ReRecord.Visibility = System.Windows.Visibility.Hidden;
            RepeatGesture.Visibility = System.Windows.Visibility.Hidden;
            RecordButton.Visibility = System.Windows.Visibility.Hidden;
            GestureName.Visibility = System.Windows.Visibility.Hidden;
            NameLabel.Visibility = System.Windows.Visibility.Hidden;

            gesture = new Gesture(GestureName.Text, HomeWindow.username, GestureType.TrainingEnabled);
            gesture.TrainingRecord();
            sensor.Start();
        }

        private void finishGesture()
        {
            Msg.Text = "Gesto grabado correctamente";
            Msg.Visibility = System.Windows.Visibility.Visible;
            StateMsg.Visibility = System.Windows.Visibility.Hidden;
            EnergyBar.Visibility = System.Windows.Visibility.Hidden;
            ReRecord.Visibility = System.Windows.Visibility.Visible;
            RepeatGesture.Visibility = System.Windows.Visibility.Visible;
            ErrorMsg.Visibility = System.Windows.Visibility.Hidden;
            RecordButton.Visibility = System.Windows.Visibility.Hidden;
            GestureName.Visibility = System.Windows.Visibility.Hidden;
            NameLabel.Visibility = System.Windows.Visibility.Hidden;

            sensor.Stop();
        }

        private void ReRecord_Click(object sender, RoutedEventArgs e)
        {
            Record_Click(sender, e);
        }

        private void RepeatGesture_Click(object sender, RoutedEventArgs e)
        {
            gesture.Replay(sensor_SkeletonFrameReadyReplay);
        }

        private void sensor_SkeletonFrameReadyRecord(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame == null)
                    return;
                
                gesture.Record(frame);
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

                Skeleton[] skeletons = new Skeleton[frame.SkeletonArrayLength];
                frame.CopySkeletonDataTo(skeletons);
                painter.Draw(skeletons);

                if (gesture.state == State.Finish)
                    finishGesture();
            }
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

        public void NavigationService_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                if (sensor != null)
                {
                    sensor.Stop();
                    sensor.Dispose();
                }
            }
        }

    }
}
