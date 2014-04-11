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

namespace GestureEvaluator.Pages.Detection.Config
{
    /// <summary>
    /// Interaction logic for FreeModeConfiguration.xaml
    /// </summary>
    public partial class FreeModeConfigurationPage : Page
    {
        private Context ctx;


        public FreeModeConfigurationPage()
        {
            InitializeComponent();

            ctx = new Context();
            SystemConfiguration conf = ctx.SystemConfigurations.FirstOrDefault(c => c.user.name == HomeWindow.username);
            ProfileListView.ItemsSource = ctx.Configurations.Where(c => c.user.name == HomeWindow.username).ToList();

            if (conf.freeModeDetectionProfile != null)
                ProfileListView.SelectedItem = conf.freeModeDetectionProfile;

            if (conf.freeModeAutomatic == true)
                AutomaticYes.IsChecked = true;
            else if (conf.freeModeAutomatic == false)
                AutomaticNo.IsChecked = true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SystemConfiguration conf = ctx.SystemConfigurations.FirstOrDefault(c => c.user.name == HomeWindow.username);

            if (ProfileListView.SelectedItem != null)
                conf.freeModeDetectionProfile = (Configuration)ProfileListView.SelectedItem;

            if (AutomaticYes.IsChecked == true)
                conf.freeModeAutomatic = true;
            else if (AutomaticNo.IsChecked == false)
                conf.freeModeAutomatic = true;

            ctx.SaveChanges();

            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.GoBack();
        }
    }
}
