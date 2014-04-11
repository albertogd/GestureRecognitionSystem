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

namespace MindstormController.Pages
{
    /// <summary>
    /// Interaction logic for ConfigurationPage.xaml
    /// </summary>
    public partial class ConfigurationPage : Page
    {
        private Context ctx;


        public ConfigurationPage()
        {
            InitializeComponent();

            ctx = new Context();
            List<GestureModel> gestures = ctx.Gestures.ToList();
            rightArmMove.ItemsSource = gestures;
            rightArmUp.ItemsSource = gestures;
            rightArmDown.ItemsSource = gestures;
            leftArmMove.ItemsSource = gestures;
            leftArmUp.ItemsSource = gestures;
            leftArmDown.ItemsSource = gestures;
            BothArmsMove.ItemsSource = gestures;
            BothArmsUp.ItemsSource = gestures;
            BothArmsDown.ItemsSource = gestures;
            rightFootMove.ItemsSource = gestures;
            rightFootForward.ItemsSource = gestures;
            rightFootBackward.ItemsSource = gestures;
            leftFootMove.ItemsSource = gestures;
            leftFootForward.ItemsSource = gestures;
            leftFootBackward.ItemsSource = gestures;
            BothFeetMove.ItemsSource = gestures;
            BothFeetForward.ItemsSource = gestures;
            BothFeetBackward.ItemsSource = gestures;

            MindstormConfiguration conf = ctx.MindstormConfigurations.FirstOrDefault(c => c.user.name == HomeWindow.username);

            rightArmMove.SelectedItem = conf.rightArmMove;
            rightArmUp.SelectedItem = conf.rightArmUp;
            rightArmDown.SelectedItem = conf.rightArmDown;
            leftArmMove.SelectedItem = conf.leftArmMove;
            leftArmUp.SelectedItem = conf.leftArmUp;
            leftArmDown.SelectedItem = conf.leftArmDown;
            BothArmsMove.SelectedItem = conf.bothArmsMove;
            BothArmsUp.SelectedItem = conf.bothArmsUp;
            BothArmsDown.SelectedItem = conf.bothArmsDown;
            rightFootMove.SelectedItem = conf.rightFootMove;
            rightFootForward.SelectedItem = conf.rightFootForward;
            rightFootBackward.SelectedItem = conf.rightFootBackward;
            leftFootMove.SelectedItem = conf.leftFootMove;
            leftFootForward.SelectedItem = conf.leftFootForward;
            leftFootBackward.SelectedItem = conf.leftFootBackward;
            BothFeetMove.SelectedItem = conf.bothFeetMove;
            BothFeetForward.SelectedItem = conf.bothFeetForward;
            BothFeetBackward.SelectedItem = conf.bothFeetBackward;

            ProfileList.ItemsSource = ctx.Configurations.Where(c => c.user.name == HomeWindow.username).ToList();
            ProfileList.SelectedItem = conf.profile;

            if (conf.connection == ConnectionType.USB)
                USB.IsChecked = true;
            else if (conf.connection == ConnectionType.WIFI)
                WIFI.IsChecked = true;

            if (conf.recognition == RecognitionType.Speech)
                SpeechCheckbox.IsChecked = true;
            else if (conf.recognition == RecognitionType.Visual)
                VisualCheckbox.IsChecked = true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            MindstormConfiguration conf = ctx.MindstormConfigurations.FirstOrDefault(c => c.user.name == HomeWindow.username);

            if (rightArmMove.SelectedItem != null)
                conf.rightArmMove = (GestureModel)rightArmMove.SelectedItem;

            if (rightArmUp.SelectedItem != null)
                conf.rightArmUp = (GestureModel)rightArmUp.SelectedItem;

            if (rightArmDown.SelectedItem != null)
                conf.rightArmDown = (GestureModel)rightArmDown.SelectedItem;

            if (leftArmMove.SelectedItem != null)
                conf.leftArmMove = (GestureModel)leftArmMove.SelectedItem;

            if (leftArmUp.SelectedItem != null)
                conf.leftArmUp = (GestureModel)leftArmUp.SelectedItem;

            if (leftArmDown.SelectedItem != null)
                conf.leftArmDown = (GestureModel)leftArmDown.SelectedItem;

            if (BothArmsMove.SelectedItem != null)
                conf.bothArmsMove = (GestureModel)BothArmsMove.SelectedItem;

            if (BothArmsUp.SelectedItem != null)
                conf.bothArmsUp = (GestureModel)BothArmsUp.SelectedItem;

            if (BothArmsDown.SelectedItem != null)
                conf.bothArmsDown = (GestureModel)BothArmsDown.SelectedItem;

            if (rightFootMove.SelectedItem != null)
                conf.rightFootMove = (GestureModel)rightFootMove.SelectedItem;

            if (rightFootForward.SelectedItem != null)
                conf.rightFootForward = (GestureModel)rightFootForward.SelectedItem;

            if (rightFootBackward.SelectedItem != null)
                conf.rightFootBackward = (GestureModel)rightFootBackward.SelectedItem;

            if (leftFootMove.SelectedItem != null)
                conf.leftFootMove = (GestureModel)leftFootMove.SelectedItem;

            if (leftFootForward.SelectedItem != null)
                conf.leftFootForward = (GestureModel)leftFootForward.SelectedItem;

            if (leftFootBackward.SelectedItem != null)
                conf.leftFootBackward = (GestureModel)leftFootBackward.SelectedItem;

            if (BothFeetMove.SelectedItem != null)
                conf.bothFeetMove = (GestureModel)BothFeetMove.SelectedItem;

            if (BothFeetForward.SelectedItem != null)
                conf.bothFeetForward = (GestureModel)BothFeetForward.SelectedItem;

            if (BothFeetBackward.SelectedItem != null)
                conf.bothFeetBackward = (GestureModel)BothFeetBackward.SelectedItem;

            if (ProfileList.SelectedItem != null)
                conf.profile = (Configuration) ProfileList.SelectedItem;

            if (USB.IsChecked == true)
                conf.connection = ConnectionType.USB;
            else if (WIFI.IsChecked == true)
                conf.connection = ConnectionType.WIFI;

            if (SpeechCheckbox.IsChecked == true)
                conf.recognition = RecognitionType.Speech;
            else if (VisualCheckbox.IsChecked == true)
                conf.recognition = RecognitionType.Visual;

            ctx.SaveChanges();

            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.GoBack();
        }
    }
}
