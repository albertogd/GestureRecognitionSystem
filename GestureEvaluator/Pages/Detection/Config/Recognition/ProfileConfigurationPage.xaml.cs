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
using Models;

namespace GestureEvaluator.Pages.Detection.Config.Recognition
{
    /// <summary>
    /// Interaction logic for ProfileConfigurationPage.xaml
    /// </summary>
    public partial class ProfileConfigurationPage : Page
    {
        private int ConfigurationID;


        public ProfileConfigurationPage(int confID)
        {
            InitializeComponent();
            ConfigurationID = confID;

            Context ctx = new Context();
            Configuration conf = ctx.Configurations.Find(confID);
            TitleBlock.Text = conf.name;
        }

        private void DTWConfiguration_Click(object sender, RoutedEventArgs e)
        {
            Page dtwConfiguration = new DTWConfigurationPage(ConfigurationID);
            NavigationService navService = NavigationService.GetNavigationService(this);
            dtwConfiguration.Resources.MergedDictionaries.Clear();
            dtwConfiguration.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            navService.Navigate(dtwConfiguration);
        }

        private void MovementConfiguration_Click(object sender, RoutedEventArgs e)
        {
            Page movementConfiguration = new MovementConfigurationPage(ConfigurationID);
            NavigationService navService = NavigationService.GetNavigationService(this);
            movementConfiguration.Resources.MergedDictionaries.Clear();
            movementConfiguration.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            navService.Navigate(movementConfiguration);
        }

        private void PreprocessConfiguration_Click(object sender, RoutedEventArgs e)
        {
            Page preprocessConfiguration = new PreprocessConfigurationPage(ConfigurationID);
            NavigationService navService = NavigationService.GetNavigationService(this);
            preprocessConfiguration.Resources.MergedDictionaries.Clear();
            preprocessConfiguration.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            navService.Navigate(preprocessConfiguration);
        }
    }
}
