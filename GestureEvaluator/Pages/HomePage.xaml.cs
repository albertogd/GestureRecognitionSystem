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
using GestureEvaluator.Pages.Detection;
using GestureEvaluator.Pages.Training;
using Models;

namespace GestureEvaluator.Pages
{
    /// <summary>
    /// Interaction logic for Homepage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();

            Context ctx = new Context();
            Language language = ctx.Users.FirstOrDefault(u => u.name == HomeWindow.username).language;
            SetLanguageDictionary(language);
        }

        private void Training_Click(object sender, RoutedEventArgs e)
        {
            Page training = new TrainingHomePage();
            training.Resources.MergedDictionaries.Clear();
            training.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(training);
        }

        private void Evaluation_Click(object sender, RoutedEventArgs e)
        {
            Page evaluation = new DetectionPage();
            evaluation.Resources.MergedDictionaries.Clear();
            evaluation.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(evaluation);
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

        private void English_Click(object sender, RoutedEventArgs e)
        {
            SetLanguageDictionary("en-US");
        }

        private void Spanish_Click(object sender, RoutedEventArgs e)
        {
            SetLanguageDictionary("es-ES");
        }

        private void SetLanguageDictionary(Language language)
        {
            if (language == Models.Language.Spanish)
                SetLanguageDictionary("es-ES");

            if (language == Models.Language.English)
                SetLanguageDictionary("en-US");
        }

        private void SetLanguageDictionary(String code)
        {
            ResourceDictionary dict = new ResourceDictionary();

            switch (code)
            {
                case "en-US":
                    dict.Source = new Uri("pack://application:,,,/Resources/StringResources.xaml");
                    break;

                case "es-ES":
                    dict.Source = new Uri("pack://application:,,,/Resources/StringResources.es-ES.xaml");
                    break;

                default:
                    dict.Source = new Uri("pack://application:,,,/Resources/StringResources.xaml");
                    break;
            }

            this.Resources.MergedDictionaries.Clear();
            this.Resources.MergedDictionaries.Add(dict);
        } 
    }
}
