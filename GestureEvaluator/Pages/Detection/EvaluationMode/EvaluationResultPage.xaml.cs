using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
using Algorithms.Preprocessing;
using Models;
using Microsoft.Kinect;

namespace GestureEvaluator.Pages.Detection
{
    /// <summary>
    /// Interaction logic for EvaluationResultPage.xaml
    /// </summary>
    public partial class EvaluationResultPage : Page
    {
        private Context ctx;
        private User user;
        private Evaluation evaluation;
        private List<EvaluationResult> evaluationResults;
        private List<EvaluationItemResult> evaluationItemResults;
        private List<EvaluationItemResultDTW> evaluationResultsDTW;
        private Gesture gestureReplayed;
        private Gesture gestureRecorded;
        private Gesture gestureDetected;
        private Preprocessor preprocessor;


        public EvaluationResultPage(int evaluationID = -1)
        {
            InitializeComponent();

            evaluationResults = new List<EvaluationResult>();
            evaluationItemResults = new List<EvaluationItemResult>();
            evaluationResultsDTW = new List<EvaluationItemResultDTW>();

            ctx = new Context();
            user = ctx.Users.FirstOrDefault(u => u.name == HomeWindow.username);

            if (evaluationID == -1)
            {
                EvaluationTitle.Visibility = Visibility.Visible;
                EvaluationBackground.Visibility = Visibility.Visible;
                EvaluationRectangle.Visibility = Visibility.Visible;
                EvaluationListView.Visibility = Visibility.Visible;

                Dictionary<int, DateTime> dic = ctx.Evaluations.Where(ev => (ev.user.ID == user.ID) && (ev.completed == true)).ToDictionary(ev => ev.ID, ev => ev.date);
                EvaluationListView.ItemsSource = dic;
            }
            else
            {
                Initialize(evaluationID);
            }
        }

        public void Initialize(int evaluationID)
        {
            EvaluationTitle.Visibility = Visibility.Hidden;
            EvaluationBackground.Visibility = Visibility.Hidden;
            EvaluationRectangle.Visibility = Visibility.Hidden;
            EvaluationListView.Visibility = Visibility.Hidden;

            evaluation = ctx.Evaluations.Find(evaluationID);
            evaluationResults = ctx.EvaluationResults.Where(evr => evr.evaluation.ID == evaluation.ID).ToList();
            ProfileList.ItemsSource = evaluationResults;
        }

        private void GestureReplayedPlay_Click(object sender, RoutedEventArgs e)
        {
            PlayGestures();
        }

        private void GestureRecordedPlay_Click(object sender, RoutedEventArgs e)
        {
            PlayGestures();
        }

        private void GestureDetectedPlay_Click(object sender, RoutedEventArgs e)
        {
            PlayGestures();
        }

        private void EvaluationListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EvaluationListView.SelectedItem == null)
                return;

            int evaluationID = ((KeyValuePair<int, DateTime>)EvaluationListView.SelectedItem).Key;
            Initialize(evaluationID);
        }

        private void ProfileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProfileList.SelectedItem == null)
                return;

            Configuration conf = ((EvaluationResult)ProfileList.SelectedItem).conf;
            evaluationItemResults = ctx.EvaluationItemResults.Where(evir => evir.evaluationItem.evaluation.ID == evaluation.ID && evir.conf.ID == conf.ID).ToList();
            EvaluationResultsList.ItemsSource = evaluationItemResults;
            GestureReplayedCombobox.ItemsSource = evaluationItemResults.Select(ev => ev.evaluationItem.gesturePlayed).ToList();
            GestureRecordedCombobox.ItemsSource = evaluationItemResults.Select(ev => ev.evaluationItem.gestureRecorded).ToList();
            GestureDetectedCombobox.ItemsSource = evaluationItemResults.Select(ev => ev.GestureRecognized).ToList();
        }

        private void EvaluationResultsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EvaluationResultsList.SelectedItem == null)
                return;

            EvaluationItemResult evir = (EvaluationItemResult)EvaluationResultsList.SelectedItem;
            evaluationResultsDTW = ctx.DTWResults.Where(dtw => dtw.EvaluationItemResult.ID == evir.ID).OrderBy(dtw => dtw.distance).ToList();
            DTWResultsList.ItemsSource = evaluationResultsDTW;
            GestureReplayedCombobox.SelectedItem = evir.evaluationItem.gesturePlayed;
            GestureRecordedCombobox.SelectedItem = evir.evaluationItem.gestureRecorded;
            GestureDetectedCombobox.SelectedItem = evir.GestureRecognized;
        }

        private void DTWResultsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(DTWResultsList.SelectedItem == null)
                return;

            EvaluationItemResultDTW evirdtw = (EvaluationItemResultDTW) DTWResultsList.SelectedItem;
            GestureModel gestureReplayedModel = evirdtw.EvaluationItemResult.evaluationItem.gesturePlayed;
            GestureModel gestureRecordedModel = evirdtw.EvaluationItemResult.evaluationItem.gestureRecorded;
            GestureModel gestureDetectedModel = evirdtw.Gesture;

            GestureReplayedCombobox.SelectedItem = gestureReplayedModel;
            GestureRecordedCombobox.SelectedItem = gestureRecordedModel;
            GestureDetectedCombobox.SelectedItem = gestureDetectedModel;

            gestureReplayed = new Gesture(gestureReplayedModel);
            gestureRecorded = new Gesture(gestureRecordedModel);
            gestureDetected = new Gesture(gestureDetectedModel);
        }

        private void PlayGestures()
        {
            if (ProfileList.SelectedItem == null)
                return;

            // Preprocessor
            Configuration conf = ((EvaluationResult)ProfileList.SelectedItem).conf;
            preprocessor = new Preprocessor(conf.ID);

            // Energy Bar
            GestureReplayedEnergyBar.Maximum = conf.movementConfiguration.ET_RestingToStartMoving * 10;
            GestureRecordedEnergyBar.Maximum = conf.movementConfiguration.ET_RestingToStartMoving * 10;
            GestureDetectedEnergyBar.Maximum = conf.movementConfiguration.ET_RestingToStartMoving * 10;

            // Play
            PlayGestureReplayed();
            PlayGestureRecorded();
            PlayGestureDetected();
        }

        private void PlayGestureReplayed()
        {
            if (gestureReplayed == null)
                return;

            // Speed
            double speed = 1;
            if (!String.IsNullOrEmpty(GestureReplayedSpeed.Text))
                Double.TryParse(GestureReplayedSpeed.Text, out speed);

            // Play
            gestureReplayed.Replay(sensor_SkeletonFrameReadyGestureReplayed, speed);
        }

        private void sensor_SkeletonFrameReadyGestureReplayed(object sender, ReplaySkeletonFrameReadyEventArgs e)
        {
            ReplaySkeletonFrame frame = e.SkeletonFrame;

            if (frame == null)
                return;

            gestureReplayed.Replay(frame);
            GestureReplayedEnergyBar.Value = (gestureReplayed.energy > GestureReplayedEnergyBar.Maximum) ? GestureReplayedEnergyBar.Maximum : gestureReplayed.energy;
            GestureReplayedPainter.Draw(frame.Skeletons.Where(s => s.TrackingState == SkeletonTrackingState.Tracked).Select(s => preprocessor.Preprocess(s)).ToArray());
        }

        private void PlayGestureRecorded()
        {
            if (gestureRecorded == null)
                return;

            // Speed
            double speed = 1;
            if (!String.IsNullOrEmpty(GestureRecordedSpeed.Text))
                Double.TryParse(GestureRecordedSpeed.Text, out speed);

            // Play
            gestureRecorded.Replay(sensor_SkeletonFrameReadyGestureRecorded, speed);
        }

        private void sensor_SkeletonFrameReadyGestureRecorded(object sender, ReplaySkeletonFrameReadyEventArgs e)
        {
            ReplaySkeletonFrame frame = e.SkeletonFrame;

            if (frame == null)
                return;

            gestureRecorded.Replay(frame);
            GestureRecordedEnergyBar.Value = (gestureRecorded.energy > GestureRecordedEnergyBar.Maximum) ? GestureRecordedEnergyBar.Maximum : gestureRecorded.energy;
            GestureRecordedPainter.Draw(frame.Skeletons.Where(s => s.TrackingState == SkeletonTrackingState.Tracked).Select(s => preprocessor.Preprocess(s)).ToArray());
        }

        private void PlayGestureDetected()
        {
            if (gestureDetected == null)
                return;

            // Speed
            double speed = 1;
            if (!String.IsNullOrEmpty(GestureDetectedSpeed.Text))
                Double.TryParse(GestureDetectedSpeed.Text, out speed);

            // Play
            gestureDetected.Replay(sensor_SkeletonFrameReadyGestureDetected, speed);
        }

        private void sensor_SkeletonFrameReadyGestureDetected(object sender, ReplaySkeletonFrameReadyEventArgs e)
        {
            ReplaySkeletonFrame frame = e.SkeletonFrame;

            if (frame == null)
                return;

            gestureDetected.Replay(frame);
            GestureDetectedEnergyBar.Value = (gestureDetected.energy > GestureDetectedEnergyBar.Maximum) ? GestureDetectedEnergyBar.Maximum : gestureDetected.energy;
            GestureDetectedPainter.Draw(frame.Skeletons.Where(s => s.TrackingState == SkeletonTrackingState.Tracked).Select(s => preprocessor.Preprocess(s)).ToArray());
        }

        private void GestureReplayedCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GestureReplayedCombobox.SelectedItem == null)
                return;

            GestureModel gesture = (GestureModel)GestureReplayedCombobox.SelectedItem;
            gestureReplayed = new Gesture(gesture);
        }

        private void GestureRecordedCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GestureRecordedCombobox.SelectedItem == null)
                return;

            GestureModel gesture = (GestureModel)GestureRecordedCombobox.SelectedItem;
            gestureRecorded = new Gesture(gesture);
        }

        private void GestureDetectedCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GestureDetectedCombobox.SelectedItem == null)
                return;

            GestureModel gesture = (GestureModel)GestureDetectedCombobox.SelectedItem;
            gestureDetected = new Gesture(gesture);
        }
    }
}
