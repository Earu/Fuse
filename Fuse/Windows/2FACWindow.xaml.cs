using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Fuse.Windows
{
    /// <summary>
    /// Interaction logic for _2FAC.xaml
    /// </summary>
    public partial class _2FACWindow : Window
    {
        private FuseClient _Client;
        private FuseUI _UI;
        private string _Password;
        private string _Username;

        internal _2FACWindow(FuseClient client,FuseUI ui,string user,string pass)
        {
            this.InitializeComponent();
            this.TBCode.Focus();
            this._Client = client;
            this._UI = ui;
            this._Username = user;
            this._Password = pass;
        }

        private void OnMouseDownDrag(object sender,EventArgs e)
        {
            this.DragMove();
        }

        private void OnClose(object sender, EventArgs e)
        {
            this.Close();
            this._UI.ShowLogin(this._Username,this._Password);
        }

        private void OnCodeKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
                this.OnSignIn(sender, e);
            }
        }

        private void OnSignIn(object sender, RoutedEventArgs e)
        {
            this.Close();
            this._Client.Connect(this._Username, this._Password, this.TBCode.Text);
        }

        private void OnCodeChanged(object sender, TextChangedEventArgs e)
        {
            string code = this.TBCode.Text.ToUpper();
            this.TBCode.Text = code;
            this.TBCode.CaretIndex = code.Length;
            if(string.IsNullOrWhiteSpace(code))
            {
                this.PLCode.Visibility = Visibility.Visible;
            }
            else
            {
                this.PLCode.Visibility = Visibility.Hidden;
            }
        }
    }
}
