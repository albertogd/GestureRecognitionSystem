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
using System.Windows.Shapes;
using Models;

namespace MindstormController
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private Context ctx;

        public RegisterWindow()
        {
            InitializeComponent();
            ctx = new Context();

            string language = Thread.CurrentThread.CurrentCulture.ToString();

            if (language == "es-ES")
                spanish.IsChecked = true;
            else
                english.IsChecked = true;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string name = this.username.Text;
            string password = this.password.Password;
            Language language = Models.Language.English;

            if (spanish.IsChecked == true)
                language = Models.Language.Spanish;
            else if (english.IsChecked == true)
                language = Models.Language.English;

            errormsg.Visibility = System.Windows.Visibility.Hidden;

            if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(password))
            {
                errormsg.Text = "Error: Campo Vacío";
                errormsg.Visibility = System.Windows.Visibility.Visible;
                return;
            }

            if (!ctx.Users.Any(u => u.name == name))
            {
                User.AddDefaultUser(name, password, language, ctx);

                MainWindow main = new MainWindow();
                App.Current.MainWindow = main;
                this.Close();
                main.Show();
            }
            else
            {
                errormsg.Text = "Error: Usuario ya existe";
                errormsg.Visibility = System.Windows.Visibility.Visible;
                return;
            }
        }

    }
}
