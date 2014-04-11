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
using GestureEvaluator.Pages.Detection.Config.Recognition;

namespace GestureEvaluator.Pages.Detection.Config
{
    /// <summary>
    /// Interaction logic for EvaluationConfigurationPage.xaml
    /// </summary>
    public partial class EvaluationConfigurationPage : Page
    {
        public EvaluationConfigurationPage()
        {
            InitializeComponent();
        }

        private void FreeModeConfiguration_Click(object sender, RoutedEventArgs e)
        {
            Page freeModeConfiguration = new FreeModeConfigurationPage();
            freeModeConfiguration.Resources.MergedDictionaries.Clear();
            freeModeConfiguration.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(freeModeConfiguration); 
        }

        private void RandomEvaluationConfiguration_Click(object sender, RoutedEventArgs e)
        {
            Page randomEvaluationConfiguration = new RandomEvaluationConfigurationPage();
            randomEvaluationConfiguration.Resources.MergedDictionaries.Clear();
            randomEvaluationConfiguration.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(randomEvaluationConfiguration); 
        }

        private void CustomEvaluationConfiguration_Click(object sender, RoutedEventArgs e)
        {
            Page customEvaluationConfiguration = new CustomEvaluationConfigurationPage();
            customEvaluationConfiguration.Resources.MergedDictionaries.Clear();
            customEvaluationConfiguration.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(customEvaluationConfiguration); 
        }

        private void RecognitionConfiguration_Click(object sender, RoutedEventArgs e)
        {
            Page recognitionConfiguration = new RecognitionConfigurationPage();
            recognitionConfiguration.Resources.MergedDictionaries.Clear();
            recognitionConfiguration.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(recognitionConfiguration); 
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
