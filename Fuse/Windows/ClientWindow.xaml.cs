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
using System.Windows.Shapes;

namespace Fuse.Windows
{
    /// <summary>
    /// Interaction logic for ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private FuseClient _Client;
        private FuseUI _UI;

        internal ClientWindow(FuseClient client,FuseUI ui)
        {
            this.InitializeComponent();
            this._Client = client;
            this._UI = ui;
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            this.Close();
            this._Client.Stop();
        }

        private void OnMaximize(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? 
                WindowState.Normal : WindowState.Maximized;
        }

        private void OnMinimize(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void OnTitleBarDrag(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void OnMessageSend(object sender, RoutedEventArgs e)
        {
            string msg = this.TBMessage.Text;
            this._UI.ShowException($"Message content was: {msg}\n" +
                $"But steam message sending is not implemented yet");
            this.TBMessage.Text = string.Empty;
            this.TBMessage.Focus();
        }

        private void OnClickMore(object sender, RoutedEventArgs e)
        {
            MoreWindow form = new MoreWindow();
            form.Show();
            form.Left = (this.Left + this.Width) - (this.BTNMore.Margin.Right + this.BTNMore.Width);
            form.Top = (this.Top + 20 + this.BTNMore.Margin.Top + this.BTNMore.Height);
        }

        private void OnSearchFriendChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.TBSearchFriends.Text))
            {
                this.PLSearchFriends.Visibility = Visibility.Visible;
            }
            else
            {
                this.PLSearchFriends.Visibility = Visibility.Hidden;
            }
        }

        private void OnMessageKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.OnMessageSend(sender, e);
            }
        }

        private void OnMessageTextChanged(object sender, TextChangedEventArgs e)
        {
            string msg = this.TBMessage.Text;
            if (this.PLMessage != null)
            {
                if (string.IsNullOrWhiteSpace(msg))
                {
                    this.PLMessage.Visibility = Visibility.Visible;
                }
                else
                {
                    this.PLMessage.Visibility = Visibility.Hidden;
                }
            }
        }

        public void SetOnlineFriendsCount(int count,int total)
        {
            this.TBOnlineFriendCount.Text = $"Online - {count}/{total}";
        }

        public void SetOfflineFriendsCount()
        {
        }

        public void ClearOnlineFriends()
        {
            this.ListFriends.Items.Clear();
        }

        public void AddOnlineFriend(string name, string status)
        {
            BrushConverter converter = new BrushConverter();
            Rectangle rec = new Rectangle
            {
                Height = 40,
                Fill = (SolidColorBrush)converter.ConvertFrom("#191919"),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 190
            };

            FontFamily family = new FontFamily("Agency FB");
            TextBlock txt = new TextBlock
            {
                Text = $"{name} - {status}",
                Foreground = Brushes.White,
                FontFamily = family,
                FontSize = 18,
                TextAlignment = TextAlignment.Center,
                Width = 190,
                Height = 20,
                Margin = new Thickness(0,0,0,0)
            };

            //this.ListFriends.Items.Add(rec);
            this.ListFriends.Items.Add(txt);
        }
    }
}
