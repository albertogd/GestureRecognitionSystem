using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using Algorithms.Preprocessing;
using Algorithms.Record;
using Models;
using MoreLinq;

namespace GestureEvaluator.Pages.Detection.FreeMode
{
    /// <summary>
    /// Interaction logic for FreeModePage.xaml
    /// </summary>
    public partial class FreeModePage : Page
    {
        private BackgroundWorker bw;
        private Context ctx;
        private KinectSensor sensor;
        private GestureRecognizer recognizer;
        private Gesture gesture;
        private Gesture detected;
        private bool automatic;
        private bool enableRecord;


        public FreeModePage()
        {
            InitializeComponent();
            enableRecord = false;

            this.Loaded += (s, e) => { NavigationService.Navigating += NavigationService_Navigating; };

            try
            {
                sensor = KinectSensor.KinectSensors.FirstOrDefault(k => k.Status == KinectStatus.Connected);

                if (sensor == null)
                {
                    ErrorMsg.Text = "Error: No se ha detectado ningún Kinect en el equipo";
                    ErrorMsg.Visibility = System.Windows.Visibility.Visible;

                    return;
                }
            }
            catch
            {
                ErrorMsg.Text = "Error: No se ha detectado ningún Kinect en el equipo";
                ErrorMsg.Visibility = System.Windows.Visibility.Visible;

                return;
            }

            ctx = new Context();
            automatic = ctx.SystemConfigurations.FirstOrDefault(c => c.user.name == HomeWindow.username).freeModeAutomatic;
            ConfigurationMovement confMovement = ctx.Configurations.FirstOrDefault(c => c.user.name == HomeWindow.username).movementConfiguration;
            EnergyBar.Maximum = confMovement.ET_RestingToStartMoving * 10;

            Configuration conf = ctx.SystemConfigurations.FirstOrDefault(c => c.user.name == HomeWindow.username).freeModeDetectionProfile;

            if (conf == null)
                conf = ctx.Configurations.FirstOrDefault(c => c.user.name == HomeWindow.username && c.type == ConfigurationProfileType.Default);

            bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(startGestureRecognizer);
            bw.RunWorkerAsync(conf.ID);

            sensor.SkeletonFrameReady += sensor_SkeletonFrameReadyRecord;
            sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
            sensor.SkeletonStream.Enable();
            sensor.Start();

            Detect_Click(null, null);
        }

        private void Detect_Click(object sender, RoutedEventArgs e)
        {
            Msg.Visibility = Visibility.Hidden;
            ErrorMsg.Visibility = Visibility.Hidden;
            painter.Visibility = Visibility.Visible;
            BackgroundWhite.Visibility = Visibility.Visible;
            painterDetectedGesture.Visibility = Visibility.Hidden;
            painterRecordedGesture.Visibility = Visibility.Hidden;
            Detect.Visibility = Visibility.Hidden;
            Replay.Visibility = Visibility.Hidden;

            gesture = new Gesture(HomeWindow.username, GestureType.Evaluation);
            gesture.EvaluationRecord();
            enableRecord = true;
        }

        private void Replay_Click(object sender, RoutedEventArgs e)
        {
            gesture.Replay(sensor_SkeletonFrameReadyRecordedReplay);
            detected.Replay(sensor_SkeletonFrameReadyDetectedReplay);
        }

        private void startGestureRecognizer(object sender, DoWorkEventArgs e)
        {
            int confID;
            bool result = Int32.TryParse(e.Argument.ToString(), out confID);
            
            if(result)
                recognizer = new GestureRecognizer(confID);
        }

        private void finishGesture(object sender, DoWorkEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                Msg.Visibility = Visibility.Visible;
                Msg.Text = "DETECTANDO GESTO...";
            }));

            if (recognizer == null)
                detected = null;
            else
                detected = recognizer.Recognize(gesture);
            
            gesture.Delete();

            if (automatic)
            {
                if (detected != null)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        Msg.Visibility = Visibility.Visible;
                        Msg.Text = "GESTO DETECTADO: " + detected.gesturename;
                    }));

                    Thread.Sleep(2000);
                }

                this.Dispatcher.Invoke((Action)(() =>
                {
                    Msg.Visibility = Visibility.Hidden;
                    gesture = new Gesture(HomeWindow.username, GestureType.Evaluation);
                    gesture.EvaluationRecord();
                    enableRecord = true;
                }));
            }
            else
            {
                painter.Visibility = Visibility.Hidden;
                BackgroundWhite.Visibility = Visibility.Hidden;
                painterDetectedGesture.Visibility = Visibility.Visible;
                painterRecordedGesture.Visibility = Visibility.Visible;
                Detect.Visibility = Visibility.Visible;
                Replay.Visibility = Visibility.Visible;
            }
        }

        private void sensor_SkeletonFrameReadyRecord(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame == null)
                    return;

                if (enableRecord == true)
                {
                    gesture.Record(frame);

                    if (gesture.state == State.Finish)
                    {
                        enableRecord = false;
                        bw = new BackgroundWorker();
                        bw.DoWork += new DoWorkEventHandler(finishGesture);
                        bw.RunWorkerAsync();
                    }
                }

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
            }
        }

        private void sensor_SkeletonFrameReadyRecordedReplay(object sender, ReplaySkeletonFrameReadyEventArgs e)
        {
            ReplaySkeletonFrame frame = e.SkeletonFrame;

            if (frame == null)
                return;

            painterRecordedGesture.Draw(frame.Skeletons);
        }

        private void sensor_SkeletonFrameReadyDetectedReplay(object sender, ReplaySkeletonFrameReadyEventArgs e)
        {
            ReplaySkeletonFrame frame = e.SkeletonFrame;

            if (frame == null)
                return;

            painterDetectedGesture.Draw(frame.Skeletons);
        }

        public void NavigationService_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                if (gesture != null)
                    gesture.Delete();

                if (sensor != null)
                    sensor.Stop();
            }
        }
    }
}
