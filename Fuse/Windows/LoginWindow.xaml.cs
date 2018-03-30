using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Fuse.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private static bool MainDefined = false;

        private FuseClient _Client;
        private bool IsMain = false;

        internal LoginWindow(FuseClient client)
        {
            this.InitializeComponent();
            this._Client = client;
            this.TBUserName.Focus();

            if (!MainDefined)
            {
                this.IsMain = true;
                MainDefined = true;
            }
            else
            {
                this.IsMain = false;
            }
        }

        private void OnSignIn(object sender, RoutedEventArgs e)
        {
            string user = this.TBUserName.Text;
            string pass = this.PBPassword.Password;

            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
            {
                this.TBUserName.Focus();
                this.TBUserName.BorderBrush = Brushes.IndianRed;
                this.PBPassword.BorderBrush = Brushes.IndianRed;
                return;
            }

            if (this.IsMain) this.Hide();
            else this.Close();

            this._Client.Connect(user, pass);
        }
         
        private void OnClose(object sender, RoutedEventArgs e)
        {
            this.Close();
            this._Client.Stop(true);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void OnUsernameChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.TBUserName.Text))
            {
                this.PLUserName.Visibility = Visibility.Visible;
            }
            else
            {
                this.PLUserName.Visibility = Visibility.Hidden;
            }
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.PBPassword.Password))
            {
                this.PLPassword.Visibility = Visibility.Visible;
            }
            else
            {
                this.PLPassword.Visibility = Visibility.Hidden;
            }
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.OnSignIn(sender,e);
            }
        }

        private void OnSignUp(object sender, RoutedEventArgs e)
        {
            Process.Start("https://store.steampowered.com/join/?");
        }
    }
}
