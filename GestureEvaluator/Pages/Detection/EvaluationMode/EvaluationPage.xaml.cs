using System;
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
using Microsoft.Kinect;
using WpfAnimatedGif;
using Algorithms;
using Algorithms.Record;
using Models;

namespace GestureEvaluator.Pages.Detection.EvaluationMode
{
    /// <summary>
    /// Interaction logic for CustomEvaluationPage.xaml
    /// </summary>
    public partial class EvaluationPage : Page
    {
        private KinectSensor sensor;
        private Context ctx;
        private Evaluation evaluation;
        private IEnumerator<KeyValuePair<int, string>> trainingGestures;
        private Dictionary<int, string> images;
        private GestureModel currentTrainingGesture;
        private Gesture currentGesture;
        private bool automatic;
        private bool enableRecord;
        private int counter;
        private int numGestures;


        public EvaluationPage(EvaluationType evaluationType)
        {
            InitializeComponent();

            this.Loaded += (s, e) => { NavigationService.Navigating += NavigationService_Navigating; };

            ErrorMsg.Visibility = System.Windows.Visibility.Hidden;
            Msg.Visibility = System.Windows.Visibility.Visible;
            ReRecord.Visibility = System.Windows.Visibility.Hidden;
            Next.Visibility = System.Windows.Visibility.Hidden;
            Replay.Visibility = System.Windows.Visibility.Visible;
            BackgroundFinish.Visibility = System.Windows.Visibility.Hidden;
            ButtonFinish.Visibility = System.Windows.Visibility.Hidden;
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

                return;
            }

            sensor.SkeletonFrameReady += sensor_SkeletonFrameReadyRecord;
            sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
            sensor.SkeletonStream.Enable();
            sensor.Start();

            ctx = new Context();
            User user = ctx.Users.FirstOrDefault(u => u.name == HomeWindow.username);
            Configuration conf = ctx.Configurations.FirstOrDefault(c => c.user.ID == user.ID);
            SystemConfiguration sysConf = ctx.SystemConfigurations.FirstOrDefault(c => c.user.ID == user.ID);
            EnergyBar.Maximum = conf.movementConfiguration.ET_RestingToStartMoving * 10;
            automatic = sysConf.customEvaluationAutomatic;

            evaluation = ctx.Evaluations.Add(new Evaluation() { user = user, type = evaluationType, date = DateTime.Now });
            evaluation.profileSerialized = (evaluationType == EvaluationType.Custom) ? sysConf.customEvaluationProfileSerialized : sysConf.randomEvaluationProfileSerialized;
            ctx.SaveChanges();

            Random rnd = new Random();
            List<GestureModel> gestures = (evaluationType == EvaluationType.Custom) ? sysConf.customEvaluationGestures(ctx) : ctx.Gestures.Where(g => g.user.ID == user.ID && g.type == GestureType.TrainingEnabled).OrderBy(x => rnd.Next()).Take(sysConf.randomEvaluationNumOfGestures).ToList();
            trainingGestures = gestures.ToDictionary(gesture => gesture.ID, gesture => gesture.name).GetEnumerator();
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
            ErrorMsg.Visibility = System.Windows.Visibility.Hidden;
            Msg.Visibility = System.Windows.Visibility.Visible;
            ReRecord.Visibility = System.Windows.Visibility.Hidden;
            Next.Visibility = System.Windows.Visibility.Hidden;
            Replay.Visibility = System.Windows.Visibility.Hidden;

            bool isNext = trainingGestures.MoveNext();

            if (isNext)
            {
                counter++;
                currentTrainingGesture = ctx.Gestures.Find(trainingGestures.Current.Key);
                Msg.Text = currentTrainingGesture.name;
                AnimationTitle.Text = counter + " de " + numGestures;
                AnimationText.Text = trainingGestures.Current.Value.ToUpper();
                Msg.Text = "Grabando Gesto " + trainingGestures.Current.Value;
                Task.Factory.StartNew(() => { LoadGif(); });

                currentGesture = new Gesture(HomeWindow.username, GestureType.Evaluation);
                currentGesture.EvaluationRecord();
            }
            else
            {
                sensor.Stop();

                BackgroundFinish.Visibility = System.Windows.Visibility.Hidden;
                ButtonFinish.Visibility = System.Windows.Visibility.Hidden;
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
            }));

            enableRecord = true;
        }

        private void finishGesture()
        {
            ErrorMsg.Visibility = System.Windows.Visibility.Hidden;
            Msg.Visibility = System.Windows.Visibility.Hidden;
            ReRecord.Visibility = System.Windows.Visibility.Visible;
            Next.Visibility = System.Windows.Visibility.Visible;
            Replay.Visibility = System.Windows.Visibility.Visible;
            enableRecord = false;

            EvaluationItem eval = new EvaluationItem() { evaluation = evaluation, gesturePlayed = currentTrainingGesture, gestureRecorded = ctx.Gestures.Find(currentGesture.ID) };
            ctx.EvaluationItems.Add(eval);
            ctx.SaveChanges();

            if (automatic)
                NextGesture();
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
