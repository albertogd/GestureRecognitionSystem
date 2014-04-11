using System;
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

namespace GestureEvaluator.Pages.Detection.Config.Recognition
{
    /// <summary>
    /// Interaction logic for RecognitionConfigurationPage.xaml
    /// </summary>
    public partial class RecognitionConfigurationPage : Page
    {
        private Context ctx;
        private SystemConfiguration sysConf;


        public RecognitionConfigurationPage()
        {
            InitializeComponent();

            ctx = new Context();
            User user = ctx.Users.FirstOrDefault(u => u.name == HomeWindow.username);
            List<Configuration> configurations = ctx.Configurations.Where(conf => conf.user.ID == user.ID).ToList();
            sysConf = ctx.SystemConfigurations.FirstOrDefault(conf => conf.user.ID == user.ID);
            DetectionGestureListView.ItemsSource = configurations;
            ProfileListView.ItemsSource = configurations;
            DetectionGestureListView.SelectedItem = sysConf.movementDetectionProfile;
        }

        private void DetectionGestureListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DetectionGestureListView.SelectedItem == null)
                return;

            Configuration conf = (Configuration)DetectionGestureListView.SelectedItem;
            sysConf.movementDetectionProfile = conf;
            ctx.SaveChanges();
        }

        private void AddProfile_Click(object sender, RoutedEventArgs e)
        {
            string profileName = ProfileName.Text;

            if (String.IsNullOrEmpty(profileName))
            {
                ErrorMsg.Text = "Es necesario que el perfil tenga un nombre";
                ErrorMsg.Visibility = System.Windows.Visibility.Visible;
                return;
            }
            else
            {
                ErrorMsg.Visibility = System.Windows.Visibility.Hidden;
                ProfileName.Text = "";
            }

            User user = ctx.Users.FirstOrDefault(u => u.name == HomeWindow.username);
            Configuration conf = Configuration.AddDefaultConfiguration(ctx, user, ConfigurationProfileType.Custom, profileName);
            ctx.SaveChanges();
            UpdateLists();
        }

        private void SelectProfile_Click(object sender, RoutedEventArgs e)
        {
            if (ProfileListView.SelectedItem == null)
                return;

            Configuration conf = (Configuration)ProfileListView.SelectedItem;

            Page profileConfigurationPage = new ProfileConfigurationPage(conf.ID);
            NavigationService navService = NavigationService.GetNavigationService(this);
            profileConfigurationPage.Resources.MergedDictionaries.Clear();
            profileConfigurationPage.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            navService.Navigate(profileConfigurationPage);
        }

        private void DeleteProfile_Click(object sender, RoutedEventArgs e)
        {
            if (ProfileListView.SelectedItem == null)
                return;

            Configuration conf = (Configuration)ProfileListView.SelectedItem;
            ctx.Configurations.Remove(conf);
            ctx.SaveChanges();
            UpdateLists();
        }

        private void UpdateLists()
        {
            ICollectionView view1 = CollectionViewSource.GetDefaultView(ProfileListView.ItemsSource);
            ICollectionView view2 = CollectionViewSource.GetDefaultView(DetectionGestureListView.ItemsSource);
            view1.Refresh();
            view2.Refresh();
        }
    }
}
