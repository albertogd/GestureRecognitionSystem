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
    /// Interaction logic for PreprocessConfigurationPage.xaml
    /// </summary>
    public partial class PreprocessConfigurationPage : Page
    {
        private Context ctx;
        private ConfigurationSkeletonPreprocess conf;


        public PreprocessConfigurationPage(int ConfigurationID)
        {
            InitializeComponent();

            ctx = new Context();
            conf = ctx.Configurations.Find(ConfigurationID).skeletonPreprocessConfiguration;

            FrontMethod.SelectedItem = conf.Rotate;
            
            if (conf.Center == true)
                CenterYes.IsChecked = true;
            else if (conf.Center == false)
                CenterNo.IsChecked = true;

            if (conf.Scale == true)
                ScaleYes.IsChecked = true;
            else if (conf.Scale == false)
                ScaleNo.IsChecked = true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (FrontMethod.SelectedItem != null)
                conf.Rotate = (RotateType)FrontMethod.SelectedItem;

            if (CenterYes.IsChecked == true)
                conf.Center = true;
            else if (CenterNo.IsChecked == false)
                conf.Center = true;

            if (ScaleYes.IsChecked == true)
                conf.Scale = true;
            else if (ScaleNo.IsChecked == false)
                conf.Scale = true;

            ctx.SaveChanges();

            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.GoBack();
        }

    }
}
