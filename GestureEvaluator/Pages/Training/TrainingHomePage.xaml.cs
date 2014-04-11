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
using GestureEvaluator.Pages.Training.Gestures;
using GestureEvaluator.Pages.Training.Configuration;

namespace GestureEvaluator.Pages.Training
{
    /// <summary>
    /// Interaction logic for TrainingHomePage.xaml
    /// </summary>
    public partial class TrainingHomePage : Page
    {
        public TrainingHomePage()
        {
            InitializeComponent();
        }

        private void ModifyGestures_Click(object sender, RoutedEventArgs e)
        {
            Page gestures = new GesturesPage();
            gestures.Resources.MergedDictionaries.Clear();
            gestures.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(gestures); 
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Page start = new StartTrainingPage();
            start.Resources.MergedDictionaries.Clear();
            start.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(start); 
        }

        private void Configure_Click(object sender, RoutedEventArgs e)
        {
            Page gestures = new ConfigurationPage();
            gestures.Resources.MergedDictionaries.Clear();
            gestures.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(gestures);
        }

        private void CloseSession_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            HomeWindow.username = "";
            App.Current.MainWindow = main;
            Window.GetWindow(this).Close();
            main.Show();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
