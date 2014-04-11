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

namespace GestureEvaluator.Pages.Detection.EvaluationMode
{
    /// <summary>
    /// Interaction logic for EvaluationHomePage.xaml
    /// </summary>
    public partial class EvaluationHomePage : Page
    {
        public EvaluationHomePage()
        {
            InitializeComponent();
        }

        private void RandomEvaluation_Click(object sender, RoutedEventArgs e)
        {
            Page randomEvaluation = new EvaluationPage(EvaluationType.Random);
            randomEvaluation.Resources.MergedDictionaries.Clear();
            randomEvaluation.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(randomEvaluation);
        }

        private void CustomEvaluation_Click(object sender, RoutedEventArgs e)
        {
            Page customEvaluation = new EvaluationPage(EvaluationType.Custom);
            customEvaluation.Resources.MergedDictionaries.Clear();
            customEvaluation.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(customEvaluation);
        }

        private void EvaluationResults_Click(object sender, RoutedEventArgs e)
        {
            Page evaluation = new EvaluationResultPage();
            evaluation.Resources.MergedDictionaries.Clear();
            evaluation.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(evaluation);
        }

        private void CalculateEvaluationResults_Click(object sender, RoutedEventArgs e)
        {
            Page evaluation = new CalculateEvaluationResultsPage();
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
    }
}
