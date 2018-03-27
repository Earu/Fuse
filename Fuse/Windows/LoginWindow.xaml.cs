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
        private FuseUI _UI;
        private bool IsMain = false;

        internal LoginWindow(FuseClient client,FuseUI ui)
        {
            this.InitializeComponent();
            this._Client = client;
            this._UI = ui;
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
            string code = this.TBCode.Text == "" ? null : this.TBCode.Text; 

            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
            {
                this.TBUserName.Focus();
                this.TBUserName.BorderBrush = Brushes.IndianRed;
                this.PBPassword.BorderBrush = Brushes.IndianRed;
                return;
            }

            if (this.IsMain) this.Hide();
            else this.Close();

            this._Client.Connect(user, pass, code);
        }
         
        private void OnClose(object sender, RoutedEventArgs e)
        {
            this.Close();
            this._Client.Stop();
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

        private void On2FACChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string code = this.TBCode.Text.ToUpper();
            this.TBCode.Text = code;
            this.TBCode.CaretIndex = code.Length;
            if (string.IsNullOrWhiteSpace(code))
            {
                this.PL2FAC.Visibility = Visibility.Visible;
            }
            else
            {
                this.PL2FAC.Visibility = Visibility.Hidden;
            }
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.OnSignIn(sender,e);
            }
        }
    }
}
