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

namespace GestureEvaluator.Pages.Training.Configuration
{
    /// <summary>
    /// Interaction logic for ConfigurationPage.xaml
    /// </summary>
    public partial class ConfigurationPage : Page
    {
        public ConfigurationPage()
        {
            InitializeComponent();
        }

        private void Configure_Click(object sender, RoutedEventArgs e)
        {
            Page configure = new ConfigureTrainingPage();
            configure.Resources.MergedDictionaries.Clear();
            configure.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(configure);
        }

        private void RecognitionConfiguration_Click(object sender, RoutedEventArgs e)
        {
            Page recognitionConfiguration_Click = new RecognitionConfigurationPage();
            recognitionConfiguration_Click.Resources.MergedDictionaries.Clear();
            recognitionConfiguration_Click.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(recognitionConfiguration_Click); 
        }
    }
}
