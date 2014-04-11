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

namespace GestureEvaluator.Pages.Training.Gestures
{
    /// <summary>
    /// Interaction logic for GesturesPage.xaml
    /// </summary>
    public partial class GesturesPage : Page
    {
        public GesturesPage()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Page add = new AddGesturesPage();
            add.Resources.MergedDictionaries.Clear();
            add.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(add);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Page delete = new DeleteGesturesPage();
            delete.Resources.MergedDictionaries.Clear();
            delete.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(delete);
        }

        private void Modify_Click(object sender, RoutedEventArgs e)
        {
            Page modify = new ModifyGesturesPage();
            modify.Resources.MergedDictionaries.Clear();
            modify.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(modify);
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            Page play = new PlayGesturePage();
            play.Resources.MergedDictionaries.Clear();
            play.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(play);
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
