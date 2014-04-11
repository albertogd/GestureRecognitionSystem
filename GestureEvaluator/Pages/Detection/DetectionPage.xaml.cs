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
using GestureEvaluator.Pages.Detection.Config;
using GestureEvaluator.Pages.Detection.FreeMode;
using GestureEvaluator.Pages.Detection.EvaluationMode;

namespace GestureEvaluator.Pages.Detection
{
    /// <summary>
    /// Interaction logic for EvaluationHomePage.xaml
    /// </summary>
    public partial class DetectionPage : Page
    {
        public DetectionPage()
        {
            InitializeComponent();
        }

        private void FreeMode_Click(object sender, RoutedEventArgs e)
        {
            Page freemode = new FreeModePage();
            freemode.Resources.MergedDictionaries.Clear();
            freemode.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(freemode); 
        }

        private void Configuration_Click(object sender, RoutedEventArgs e)
        {
            Page configuration = new EvaluationConfigurationPage();
            configuration.Resources.MergedDictionaries.Clear();
            configuration.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(configuration);
        }

        private void Evaluation_Click(object sender, RoutedEventArgs e)
        {
            Page evaluation = new EvaluationMode.EvaluationHomePage();
            evaluation.Resources.MergedDictionaries.Clear();
            evaluation.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(evaluation);
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
