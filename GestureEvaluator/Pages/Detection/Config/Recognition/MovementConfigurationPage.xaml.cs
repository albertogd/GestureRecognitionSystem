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
    /// Interaction logic for MovementConfigurationPage.xaml
    /// </summary>
    public partial class MovementConfigurationPage : Page
    {
        private Context ctx;
        private ConfigurationMovement conf;


        public MovementConfigurationPage(int ConfigurationID)
        {
            InitializeComponent();

            ctx = new Context();
            conf = ctx.Configurations.Find(ConfigurationID).movementConfiguration;

            ET_R_START.Text = conf.ET_RestingToStartMoving.ToString();
            ET_START_R.Text = conf.ET_StartMovingToResting.ToString();
            ET_START_M.Text = conf.ET_StartMovingToMoving.ToString();
            ET_M_STOP.Text = conf.ET_MovingToStopMoving.ToString();
            ET_STOP_M.Text = conf.ET_StopMovingToMoving.ToString();
            ET_STOP_R.Text = conf.ET_StopMovingToResting.ToString();

            TT_R_START.Text = conf.TT_RestingToStartMoving.ToString();
            TT_START_R.Text = conf.TT_StartMovingToResting.ToString();
            TT_START_M.Text = conf.TT_StartMovingToMoving.ToString();
            TT_M_STOP.Text = conf.TT_MovingToStopMoving.ToString();
            TT_STOP_M.Text = conf.TT_StopMovingToMoving.ToString();
            TT_STOP_R.Text = conf.TT_StopMovingToResting.ToString();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            conf.ET_RestingToStartMoving = Double.Parse(ET_R_START.Text);
            conf.ET_StartMovingToResting = Double.Parse(ET_START_R.Text);
            conf.ET_StartMovingToMoving = Double.Parse(ET_START_M.Text);
            conf.ET_MovingToStopMoving = Double.Parse(ET_M_STOP.Text);
            conf.ET_StopMovingToMoving = Double.Parse(ET_STOP_M.Text);
            conf.ET_StopMovingToResting = Double.Parse(ET_STOP_R.Text);

            conf.TT_RestingToStartMoving = Int32.Parse(TT_R_START.Text);
            conf.TT_StartMovingToResting = Int32.Parse(TT_START_R.Text);
            conf.TT_StartMovingToMoving = Int32.Parse(TT_START_M.Text);
            conf.TT_MovingToStopMoving = Int32.Parse(TT_M_STOP.Text);
            conf.TT_StopMovingToMoving = Int32.Parse(TT_STOP_M.Text);
            conf.TT_StopMovingToResting = Int32.Parse(TT_STOP_R.Text);

            ctx.SaveChanges();

            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.GoBack();
        }
    }
}
