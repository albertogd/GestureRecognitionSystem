using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace GestureEvaluator.Pages.Training.Configuration
{
    /// <summary>
    /// Interaction logic for ConfigureTrainingPage.xaml
    /// </summary>
    public partial class ConfigureTrainingPage : Page
    {
        private Context ctx;
        private string imagePath;
        private List<GestureModel> gesturesEnabled;
        private List<GestureModel> gesturesDisabled;


        public ConfigureTrainingPage()
        {
            InitializeComponent();

            ctx = new Context();
            imagePath = "";

            gesturesEnabled = ctx.Gestures.Where(g => g.user.name == HomeWindow.username && g.type == GestureType.TrainingEnabled).ToList();
            gesturesDisabled = ctx.Gestures.Where(g => g.user.name == HomeWindow.username && g.type == GestureType.TrainingDisabled).ToList();
            EnabledTrainingGestureListView.ItemsSource = gesturesEnabled;
            DisabledTrainingGestureListView.ItemsSource = gesturesDisabled;
            UpdateLists();

            SystemConfiguration conf = ctx.SystemConfigurations.FirstOrDefault(c => c.user.name == HomeWindow.username);

            if (conf.trainingAutomatic == true)
                AutomaticTrainingYes.IsChecked = true;
            else if (conf.trainingAutomatic == false)
                AutomaticTrainingNo.IsChecked = true;
        }

        private void GestureImage_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".gif";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                String baseDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                String imgDirectory = System.IO.Path.Combine(baseDirectory, "Resources", "img", HomeWindow.username, "Gestures");

                if(!Directory.Exists(imgDirectory))
                    Directory.CreateDirectory(imgDirectory);

                // Open document 
                imagePath = System.IO.Path.Combine(imgDirectory, System.IO.Path.GetFileName(dlg.FileName));
                File.Copy(dlg.FileName, imagePath, true);
            }
        }

        private void AddGesture_Click(object sender, RoutedEventArgs e)
        {
            string gesturename = GestureName.Text;

            if (String.IsNullOrEmpty(gesturename))
            {
                ErrorMsg.Text = "Es necesario que el gesto tenga un nombre";
                ErrorMsg.Visibility = System.Windows.Visibility.Visible;
                return;
            }
            else
            {
                ErrorMsg.Visibility = System.Windows.Visibility.Hidden;
                GestureName.Text = "";
            }

            User user = ctx.Users.FirstOrDefault(u => u.name == HomeWindow.username);
            GestureModel gesture = ctx.Gestures.Add(new GestureModel() { name = gesturename, user = user, image = imagePath, type = GestureType.TrainingEnabled });
            ctx.SaveChanges();

            UpdateLists();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SystemConfiguration conf = ctx.SystemConfigurations.FirstOrDefault(c => c.user.name == HomeWindow.username);

            if (AutomaticTrainingYes.IsChecked == true)
                conf.trainingAutomatic = true;
            else if (AutomaticTrainingNo.IsChecked == true)
                conf.trainingAutomatic = false;

            gesturesDisabled.ForEach(gesture => gesture.type = GestureType.TrainingDisabled);
            gesturesEnabled.ForEach(gesture => gesture.type = GestureType.TrainingEnabled);

            ctx.SaveChanges();
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.GoBack();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (DisabledTrainingGestureListView.SelectedItems == null || DisabledTrainingGestureListView.SelectedItems.Count == 0)
                return;

            IList gestures = DisabledTrainingGestureListView.SelectedItems;

            foreach (GestureModel gesture in gestures)
            {
                if (gesturesDisabled.Contains(gesture))
                    gesturesDisabled.Remove(gesture);

                if (!gesturesEnabled.Contains(gesture))
                    gesturesEnabled.Add(gesture);
            }

            UpdateLists();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (EnabledTrainingGestureListView.SelectedItems == null || EnabledTrainingGestureListView.SelectedItems.Count == 0)
                return;

            IList gestures = EnabledTrainingGestureListView.SelectedItems;

            foreach (GestureModel gesture in gestures)
            {
                if (gesturesEnabled.Contains(gesture))
                    gesturesEnabled.Remove(gesture);

                if (!gesturesDisabled.Contains(gesture))
                    gesturesDisabled.Add(gesture);
            }

            UpdateLists();
        }

        private void UpdateLists()
        {
            ICollectionView view1 = CollectionViewSource.GetDefaultView(DisabledTrainingGestureListView.ItemsSource);
            ICollectionView view2 = CollectionViewSource.GetDefaultView(EnabledTrainingGestureListView.ItemsSource);
            view1.Refresh();
            view2.Refresh();
        }

    }
}
