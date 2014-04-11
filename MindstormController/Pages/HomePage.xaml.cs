using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using Microsoft.Speech.Recognition;
using Algorithms.Audio;
using Models;

namespace MindstormController.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private Context ctx;
        private SpeechRecognizer sr;
        private MindstormConfiguration conf;


        /// <summary>
        /// Constructor
        /// </summary>
        public HomePage()
        {
            InitializeComponent();

            sr = new SpeechRecognizer(SpeechRecognized, HomeWindow.language);
            ctx = new Context();
            conf = ctx.MindstormConfigurations.FirstOrDefault(c => c.user.name == HomeWindow.username);

            if (conf.recognition == RecognitionType.Speech)
                Task.Factory.StartNew(() => { Thread.Sleep(1000); this.Dispatcher.Invoke((Action)(() => { sr.start(); })); }); 
        }

        private void Configuration_Click(object sender, RoutedEventArgs e)
        {
            if (conf.recognition == RecognitionType.Speech)
                sr.close();

            Page configurationPage = new ConfigurationPage();
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(configurationPage); 
        }

        private void RobotController_Click(object sender, RoutedEventArgs e)
        {
            if (conf.recognition == RecognitionType.Speech)
                sr.close();

            Page robotControllerPage = new RobotControllerPage();
            NavigationService navService = NavigationService.GetNavigationService(this);
            navService.Navigate(robotControllerPage); 
        }

        private void CloseSession_Click(object sender, RoutedEventArgs e)
        {
            if (conf.recognition == RecognitionType.Speech)
                sr.close();

            MainWindow main = new MainWindow();
            HomeWindow.username = "";
            App.Current.MainWindow = main;
            Window.GetWindow(this).Close();
            main.Show();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (conf.recognition == RecognitionType.Speech)
                sr.close();

            Application.Current.Shutdown();
        }

        /// <summary>
        /// Handler for recognized speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.3;

            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                switch (e.Result.Semantics.Value.ToString())
                {
                    case "CONFIGURATION":
                        Configuration_Click(null, null);
                        break;

                    case "ROBOT CONTROLLER":
                        RobotController_Click(null, null);
                        break;

                    case "EXIT":
                        Exit_Click(null, null);
                        break;

                    case "CLOSE SESSION":
                        CloseSession_Click(null, null);
                        break;
                }
            }
        }
    }
}
