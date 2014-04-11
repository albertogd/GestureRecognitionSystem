using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
using Models;

namespace GestureEvaluator.Pages.Detection.Config
{
    /// <summary>
    /// Interaction logic for CustomEvaluationConfiguration.xaml
    /// </summary>
    public partial class CustomEvaluationConfigurationPage : Page
    {
        private Context ctx;
        private List<GestureModel> evaluationGestures;
        private List<GestureModel> trainingGestures;
        private List<Configuration> availableProfiles;
        private List<Configuration> configuredProfiles;


        public CustomEvaluationConfigurationPage()
        {
            InitializeComponent();

            ctx = new Context();
            SystemConfiguration conf = ctx.SystemConfigurations.FirstOrDefault(c => c.user.name == HomeWindow.username);
            evaluationGestures = conf.customEvaluationGestures(ctx);
            configuredProfiles = conf.customEvaluationProfiles(ctx);
            trainingGestures = ctx.Gestures.Where(gesture => gesture.user.name == HomeWindow.username && (gesture.type == GestureType.TrainingEnabled || gesture.type == GestureType.TrainingDisabled) ).ToList().Where(gesture => !evaluationGestures.Contains(gesture)).ToList();
            availableProfiles = ctx.Configurations.Where(configuration => configuration.user.name == HomeWindow.username).ToList().Where(configuration => !configuredProfiles.Contains(configuration)).ToList();

            EvaluationGesturesListView.ItemsSource = evaluationGestures;
            TrainingGesturesListView.ItemsSource = trainingGestures;
            ProfileAvailableListView.ItemsSource = availableProfiles;
            ProfileConfiguredListView.ItemsSource = configuredProfiles;

            if (conf.customEvaluationAutomatic == true)
                AutomaticYes.IsChecked = true;
            else if (conf.customEvaluationAutomatic == false)
                AutomaticNo.IsChecked = true;

            UpdateGestures();
            UpdateProfiles();
        }

        private void AddGesture_Click(object sender, RoutedEventArgs e)
        {
            if (TrainingGesturesListView.SelectedItems == null || TrainingGesturesListView.SelectedItems.Count == 0)
                return;

            IList gestures = TrainingGesturesListView.SelectedItems;

            foreach (GestureModel gesture in gestures)
            {
                if (trainingGestures.Contains(gesture))
                    trainingGestures.Remove(gesture);

                if (!evaluationGestures.Contains(gesture))
                    evaluationGestures.Add(gesture);
            }

            UpdateGestures();
        }

        private void DeleteGesture_Click(object sender, RoutedEventArgs e)
        {
            if (EvaluationGesturesListView.SelectedItems == null || EvaluationGesturesListView.SelectedItems.Count == 0)
                return;

            IList gestures = EvaluationGesturesListView.SelectedItems;

            foreach (GestureModel gesture in gestures)
            {
                if (evaluationGestures.Contains(gesture))
                    evaluationGestures.Remove(gesture);

                if (!trainingGestures.Contains(gesture))
                    trainingGestures.Add(gesture);
            }

            UpdateGestures();
        }

        private void AddProfile_Click(object sender, RoutedEventArgs e)
        {
            if (ProfileAvailableListView.SelectedItems == null || ProfileAvailableListView.SelectedItems.Count == 0)
                return;

            IList configurations = ProfileAvailableListView.SelectedItems;

            foreach (Configuration configuration in configurations)
            {
                if (availableProfiles.Contains(configuration))
                    availableProfiles.Remove(configuration);

                if (!configuredProfiles.Contains(configuration))
                    configuredProfiles.Add(configuration);
            }

            UpdateProfiles();
        }

        private void DeleteProfile_Click(object sender, RoutedEventArgs e)
        {
            if (ProfileConfiguredListView.SelectedItems == null || ProfileConfiguredListView.SelectedItems.Count == 0)
                return;

            IList configurations = ProfileConfiguredListView.SelectedItems;

            foreach (Configuration configuration in configurations)
            {
                if (configuredProfiles.Contains(configuration))
                    configuredProfiles.Remove(configuration);

                if (!availableProfiles.Contains(configuration))
                    availableProfiles.Add(configuration);
            }

            UpdateProfiles();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SystemConfiguration conf = ctx.SystemConfigurations.FirstOrDefault(c => c.user.name == HomeWindow.username);
            conf.customEvaluationGestures(evaluationGestures.ToArray());
            conf.customEvaluationProfiles(configuredProfiles.ToArray());

            if (AutomaticYes.IsChecked == true)
                conf.customEvaluationAutomatic = true;
            else if (AutomaticNo.IsChecked == true)
                conf.customEvaluationAutomatic = false;

            ctx.SaveChanges();

            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.GoBack();
        }

        private void UpdateGestures()
        {
            ICollectionView view1 = CollectionViewSource.GetDefaultView(EvaluationGesturesListView.ItemsSource);
            ICollectionView view2 = CollectionViewSource.GetDefaultView(TrainingGesturesListView.ItemsSource);
            view1.Refresh();
            view2.Refresh();
        }

        private void UpdateProfiles()
        {
            ICollectionView view1 = CollectionViewSource.GetDefaultView(ProfileAvailableListView.ItemsSource);
            ICollectionView view2 = CollectionViewSource.GetDefaultView(ProfileConfiguredListView.ItemsSource);
            view1.Refresh();
            view2.Refresh();
        }
    }
}
