using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
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
using Models;


namespace GestureEvaluator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Context ctx;
        private BackgroundWorker bw = new BackgroundWorker();

        public MainWindow()
        { 
            InitializeComponent();

            SetLanguageDictionary(Thread.CurrentThread.CurrentCulture.ToString());

            ctx = new Context();
            Database.SetInitializer<Models.Context>(new Models.Initializer());

            // Database first connection is a bit slow (from 1 to 3 seconds),
            // so we do in a thread a query in order to start the database 
            // connection in background
            bw.DoWork += new DoWorkEventHandler(checkDatabase);
            bw.RunWorkerAsync();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (!isDatabaseUp())
                return;

            string name = this.username.Text;
            string password = this.password.Password;
            errormsg.Visibility = System.Windows.Visibility.Hidden;

            if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(password))
            {
                errormsg.Text = "Error: Campo Vacío";
                errormsg.Visibility = System.Windows.Visibility.Visible;
                return;
            }

            string hash =  User.GenerateHash(password);

            if (ctx.Users.Any(u => u.name == name && u.password == hash))
            {
                HomeWindow home = new HomeWindow();
                HomeWindow.username = name;
                App.Current.MainWindow = home;
                home.Resources.MergedDictionaries.Clear();
                home.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
                this.Close();
                home.Show();
            }
            else
            {
                if (ctx.Users.Any(u => u.name == name))
                    errormsg.Text = "Error: Contrseña errónea";
                else
                    errormsg.Text = "Error: El usuario no existe ";

                errormsg.Visibility = System.Windows.Visibility.Visible;
                return;
            }
        }

        private void NewUser_Click(object sender, RoutedEventArgs e)
        {
            if (!isDatabaseUp())
                return;

            RegisterWindow register = new RegisterWindow();
            App.Current.MainWindow = register;
            register.Resources.MergedDictionaries.Clear();
            register.Resources.MergedDictionaries.Add(this.Resources.MergedDictionaries.First());
            this.Close();
            register.Show();
        }

        public void checkDatabase(object sender, DoWorkEventArgs e)
        {
            if(!isDatabaseUp())
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    this.errormsg.Text = "Error: No se ha podido conectar con la base de datos";
                    this.errormsg.Visibility = System.Windows.Visibility.Visible;
                }));
            }
        }

        public bool isDatabaseUp()
        {
            try
            {
                ctx.Users.Count();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void English_Click(object sender, RoutedEventArgs e)
        {
            SetLanguageDictionary("en-US");
        }

        private void Spanish_Click(object sender, RoutedEventArgs e)
        {
            SetLanguageDictionary("es-ES");
        }

        private void SetLanguageDictionary(String code)
        {
            ResourceDictionary dict = new ResourceDictionary();

            switch (code)
            {
                case "en-US":
                    dict.Source = new Uri("..\\Resources\\StringResources.xaml", UriKind.Relative);
                    TitleImg.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/img/title.jpg"));
                    break;

                case "es-ES":
                    dict.Source = new Uri("..\\Resources\\StringResources.es-ES.xaml", UriKind.Relative);
                    TitleImg.Source = new BitmapImage(new Uri("..\\Resources\\img\\titulo.jpg", UriKind.Relative));
                    break;

                default:
                    dict.Source = new Uri("..\\Resources\\StringResources.xaml",UriKind.Relative);
                    TitleImg.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/img/title.jpg"));
                    break;
            }

            this.Resources.MergedDictionaries.Clear();
            this.Resources.MergedDictionaries.Add(dict);
        } 

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                Login_Click(sender, e);
        }
    }
}
