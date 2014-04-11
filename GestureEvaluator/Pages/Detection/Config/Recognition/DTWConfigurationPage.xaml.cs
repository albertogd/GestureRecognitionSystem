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
    /// Interaction logic for DTWConfigurationPage.xaml
    /// </summary>
    public partial class DTWConfigurationPage : Page
    {
        private Context ctx;
        private ConfigurationDTW conf;


        public DTWConfigurationPage(int ConfigurationID)
        {
            InitializeComponent();

            ctx = new Context();
            conf = ctx.Configurations.Find(ConfigurationID).dtwConfiguration;

            Distance.SelectedItem = conf.distance;

            if (conf.boundaryConstraintStart == true)
                StartBoundaryYes.IsChecked = true;
            else if (conf.boundaryConstraintStart == false)
                StartBoundaryNo.IsChecked = true;

            if (conf.boundaryConstraintEnd == true)
                EndBoundaryYes.IsChecked = true;
            else if (conf.boundaryConstraintEnd == false)
                EndBoundaryNo.IsChecked = true;

            if (conf.slopeStepSizeDiagonal.HasValue)
                slopeStepSizeDiagonal.Text = conf.slopeStepSizeDiagonal.Value.ToString();

            if (conf.slopeStepSizeAside.HasValue)
                slopeStepSizeAside.Text = conf.slopeStepSizeAside.Value.ToString();

            if (conf.sakoeChibaMaxShift.HasValue)
                sakoeChibaMaxShift.Text = conf.sakoeChibaMaxShift.Value.ToString();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (Distance.SelectedItem != null)
                conf.distance = (DistanceType) Distance.SelectedItem;

            if (StartBoundaryYes.IsChecked == true)
                conf.boundaryConstraintStart = true;
            else if (StartBoundaryNo.IsChecked == true)
                conf.boundaryConstraintStart = false;

            if (EndBoundaryYes.IsChecked == true)
                conf.boundaryConstraintEnd = true;
            else if (EndBoundaryNo.IsChecked == true)
                conf.boundaryConstraintEnd = false;

            int stepSizeDiagonal;
            if (!String.IsNullOrEmpty(slopeStepSizeDiagonal.Text) && Int32.TryParse(slopeStepSizeDiagonal.Text, out stepSizeDiagonal))
                conf.slopeStepSizeDiagonal = stepSizeDiagonal;

            int stepSizeAside;
            if (!String.IsNullOrEmpty(slopeStepSizeAside.Text) && Int32.TryParse(slopeStepSizeAside.Text, out stepSizeAside))
                conf.slopeStepSizeAside = stepSizeAside;

            int maxShift;
            if (!String.IsNullOrEmpty(sakoeChibaMaxShift.Text) && Int32.TryParse(sakoeChibaMaxShift.Text, out maxShift))
                conf.sakoeChibaMaxShift = maxShift;

            ctx.SaveChanges();

            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.GoBack();
        }
    }
}
