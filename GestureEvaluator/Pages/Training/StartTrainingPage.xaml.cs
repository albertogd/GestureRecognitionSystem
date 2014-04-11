using System;
using System.Collections;
using System.Collections.Generic;
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
using System.Windows.Media.Animation;
using Microsoft.Kinect;
using WpfAnimatedGif;
using Algorithms;
using Algorithms.Record;
using Models;

namespace GestureEvaluator.Pages.Training
{
    /// <summary>
    /// Interaction logic for StartTrainingPage.xaml
    /// </summary>
    public partial class StartTrainingPage : Page
    {
        private Context ctx;
        private IEnumerator<KeyValuePair<int, string>> trainingGestures;
        private Dictionary<int, string> images;
        private KinectSensor sensor;
        private Gesture currentGesture;
        private bool automatic;
        private bool enableRecord;
        private int counter;
        private int numGestures;


        public StartTrainingPage()
        {
            InitializeComponent();

            this.Loaded += (s, e) => { NavigationService.Navigating += NavigationService_Navigating; };

            ErrorMsg.Visibility = Visibility.Hidden;
            Msg.Visibility = Visibility.Visible;
            ReRecord.Visibility = Visibility.Hidden;
            Next.Visibility = Visibility.Hidden;
            Replay.Visibility = Visibility.Visible;
            KinectRestart.Visibility = Visibility.Hidden;
            BackgroundFinish.Visibility = Visibility.Hidden;
            ButtonFinish.Visibility = Visibility.Hidden;
            enableRecord = false;

            try
            {
                sensor = KinectSensor.KinectSensors.FirstOrDefault(k => k.Status == KinectStatus.Connected);

                if (sensor == null)
                {
                    ErrorMsg.Text = "Error: No se ha detectado ningún Kinect en el equipo";
                    ErrorMsg.Visibility = System.Windows.Visibility.Visible;
                    Msg.Visibility = System.Windows.Visibility.Hidden;
                    ReRecord.Visibility = System.Windows.Visibility.Hidden;
                    Next.Visibility = System.Windows.Visibility.Hidden;
                    Replay.Visibility = System.Windows.Visibility.Hidden;
                    KinectRestart.Visibility = Visibility.Hidden;

                    return;
                }
            }
            catch
            {
                ErrorMsg.Text = "Error: No se ha detectado ningún Kinect en el equipo";
                ErrorMsg.Visibility = System.Windows.Visibility.Visible;
                Msg.Visibility = System.Windows.Visibility.Hidden;
                ReRecord.Visibility = System.Windows.Visibility.Hidden;
                Next.Visibility = System.Windows.Visibility.Hidden;
                Replay.Visibility = System.Windows.Visibility.Hidden;
                KinectRestart.Visibility = Visibility.Hidden;

                return;
            }

            sensor.SkeletonFrameReady += sensor_SkeletonFrameReadyRecord;
            sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
            sensor.SkeletonStream.Enable();
            sensor.Start();

            ctx = new Context();
            ConfigurationMovement conf = ctx.Configurations.FirstOrDefault(c => c.user.name == HomeWindow.username).movementConfiguration;
            EnergyBar.Maximum = conf.ET_RestingToStartMoving * 10;
            automatic = ctx.SystemConfigurations.FirstOrDefault(c => c.user.name == HomeWindow.username).trainingAutomatic;
            
            List<GestureModel> gestures = ctx.Gestures.Where(g => g.user.name == HomeWindow.username && g.type == GestureType.TrainingEnabled).ToList();
            // Delete Trained gestures with empty skeletons
            List<GestureModel> trainedGesgtures = gestures.Where(g => !(String.IsNullOrEmpty(g.file))).ToList();
            trainedGesgtures.Select(g => new Gesture(g)).Where(g => g.Skeletons.Count == 0).ToList().ForEach(g => g.DeleteSkeletonFile());
            // Get Training Gestures
            trainingGestures = gestures.Where(g => (String.IsNullOrEmpty(g.file))).ToDictionary(gesture => gesture.ID, gesture => gesture.name).GetEnumerator();
            images = gestures.ToDictionary(gesture => gesture.ID, gesture => gesture.image);
            numGestures = gestures.Count;
            counter = 0;

            NextGesture();
        }

        private void ReRecord_Click(object sender, RoutedEventArgs e)
        {
            Msg.Text = "Grabando Gesto " + trainingGestures.Current.Key;
            ErrorMsg.Visibility = System.Windows.Visibility.Hidden;
            Msg.Visibility = System.Windows.Visibility.Visible;
            ReRecord.Visibility = System.Windows.Visibility.Hidden;
            Next.Visibility = System.Windows.Visibility.Hidden;
            Replay.Visibility = System.Windows.Visibility.Hidden;

            currentGesture.TrainingRecord();
            enableRecord = true;
        }

        private void Replay_Click(object sender, RoutedEventArgs e)
        {
            currentGesture.Replay(sensor_SkeletonFrameReadyReplay);
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            NextGesture();
        }

        private void NextGesture()
        {
            ErrorMsg.Visibility = Visibility.Hidden;
            Msg.Visibility = Visibility.Visible;
            ReRecord.Visibility = Visibility.Hidden;
            Next.Visibility = Visibility.Hidden;
            Replay.Visibility = Visibility.Hidden;
            KinectRestart.Visibility = Visibility.Hidden;

            bool isNext = trainingGestures.MoveNext();

            if (isNext)
            {
                counter++;
                GestureModel model = ctx.Gestures.Find(trainingGestures.Current.Key);
                Msg.Text = model.name;
                AnimationTitle.Text = counter + " de " + numGestures;
                AnimationText.Text = trainingGestures.Current.Value.ToUpper();
                Msg.Text = "Grabando Gesto " + trainingGestures.Current.Value;
                Task.Factory.StartNew(() => { LoadGif(); });

                currentGesture = new Gesture(model);
                currentGesture.TrainingRecord();
            }
            else
            {
                sensor.Stop();

                BackgroundFinish.Visibility = Visibility.Visible;
                ButtonFinish.Visibility = Visibility.Visible;
                ErrorMsg.Visibility = Visibility.Hidden;
                Msg.Visibility = Visibility.Hidden;
                ReRecord.Visibility = Visibility.Hidden;
                Next.Visibility = Visibility.Hidden;
                Replay.Visibility = Visibility.Hidden;
                KinectRestart.Visibility = Visibility.Hidden;
            }
        }

        private void ReplayGif_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>{ LoadGif(); });
        }

        private void LoadGif()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                ImgBackground.Visibility = Visibility.Visible;
                GestureImg.Visibility = Visibility.Hidden;
                AnimationText.Visibility = Visibility.Visible;
                AnimationTitle.Visibility = Visibility.Visible;
            }));

            Thread.Sleep(2000);

            this.Dispatcher.Invoke((Action)(() =>
            {
                AnimationText.Visibility = Visibility.Hidden;
                AnimationTitle.Visibility = Visibility.Hidden;
                GestureImg.Visibility = Visibility.Visible;

                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(images[trainingGestures.Current.Key]);
                image.EndInit();
                ImageBehavior.SetAnimatedSource(GestureImg, image);
            }));

            Thread.Sleep(3000);

            this.Dispatcher.Invoke((Action)(() =>
            {
                ImgBackground.Visibility = Visibility.Hidden;
                GestureImg.Visibility = Visibility.Hidden;
                KinectRestart.Visibility = Visibility.Visible;
            }));

            enableRecord = true;
        }

        private void Exit()
        {
            Thread.Sleep(2000);
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.GoBack();
        }

        private void finishGesture()
        {
            ErrorMsg.Visibility = System.Windows.Visibility.Hidden;
            Msg.Visibility = System.Windows.Visibility.Hidden;
            ReRecord.Visibility = System.Windows.Visibility.Visible;
            Next.Visibility = System.Windows.Visibility.Visible;
            Replay.Visibility = System.Windows.Visibility.Visible;
            KinectRestart.Visibility = System.Windows.Visibility.Hidden;

            enableRecord = false;

            if (automatic)
                NextGesture();
        }

        private void KinectRestart_Click(object sender, RoutedEventArgs e)
        {
            sensor.Start();
        }

        private void sensor_SkeletonFrameReadyRecord(object sender, SkeletonFrameReadyEventArgs e)
        {
            if (enableRecord == false)
                return;

            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame == null)
                    return;

                currentGesture.Record(frame);

                EnergyBar.Value = (currentGesture.energy > EnergyBar.Maximum) ? EnergyBar.Maximum : currentGesture.energy;
                ProgressValue.Text = currentGesture.energy.ToString();

                switch (currentGesture.state)
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

                if (currentGesture.state == State.Finish)
                    finishGesture();
            }
        }

        private void sensor_SkeletonFrameReadyReplay(object sender, ReplaySkeletonFrameReadyEventArgs e)
        {
            ReplaySkeletonFrame frame = e.SkeletonFrame;

            if (frame == null)
                return;

            currentGesture.Replay(frame);
            EnergyBar.Value = (currentGesture.energy > EnergyBar.Maximum) ? EnergyBar.Maximum : currentGesture.energy;
            ProgressValue.Text = currentGesture.energy.ToString();

            switch (currentGesture.state)
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
                }
            }
        }

        private void ButtonFinish_Click(object sender, RoutedEventArgs e)
        {
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.GoBack();
        }
    }
}
