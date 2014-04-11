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
    /// Interaction logic for RandomEvaluationConfiguration.xaml
    /// </summary>
    public partial class RandomEvaluationConfigurationPage : Page
    {
        private Context ctx;


        public RandomEvaluationConfigurationPage()
        {
            InitializeComponent();

            ctx = new Context();
            SystemConfiguration conf = ctx.SystemConfigurations.FirstOrDefault(c => c.user.name == HomeWindow.username);
            GestureNumber.Text = conf.randomEvaluationNumOfGestures.ToString();

            if (conf.randomEvaluationAutomatic == true)
                AutomaticYes.IsChecked = true;
            else if (conf.randomEvaluationAutomatic == false)
                AutomaticNo.IsChecked = true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SystemConfiguration conf = ctx.SystemConfigurations.FirstOrDefault(c => c.user.name == HomeWindow.username);

            int num;
            if(!String.IsNullOrEmpty(GestureNumber.Text) && Int32.TryParse(GestureNumber.Text, out num))
                conf.randomEvaluationNumOfGestures = num;

            if (AutomaticYes.IsChecked == true)
                conf.randomEvaluationAutomatic = true;
            else if (AutomaticNo.IsChecked == true)
                conf.randomEvaluationAutomatic = false;

            ctx.SaveChanges();

            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.GoBack();
        }
    }
}
