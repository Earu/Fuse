using Fuse.Controls;
using Fuse.Models;
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
        private bool _IsSearchingFriends = false;

        internal ClientWindow(FuseClient client,FuseUI ui)
        {
            this.InitializeComponent();
            this._Client = client;
            this._UI = ui;
        }

        internal bool IsSearchingFriends { get => this._IsSearchingFriends; }

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
            string search = this.TBSearchFriends.Text;
            List<Friend> onlinefriends = this._Client.User.OnlineFriends;
            List<Friend> offlinefriends = this._Client.User.OfflineFriends;

            onlinefriends.Sort((x, y) => x.Name.CompareTo(y.Name));
            offlinefriends.Sort((x, y) => x.Name.CompareTo(y.Name));

            this.ClearOnlineFriends();
            this.ClearOfflineFriends();

            if (string.IsNullOrWhiteSpace(search))
            {
                this._IsSearchingFriends = true;
                this.PLSearchFriends.Visibility = Visibility.Visible;

                onlinefriends.ForEach(x => this.AddOnlineFriend(x));
                offlinefriends.ForEach(x => this.AddOfflineFriend(x));
            }
            else
            {
                this._IsSearchingFriends = false;
                this.PLSearchFriends.Visibility = Visibility.Hidden;
                onlinefriends = onlinefriends.Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();
                offlinefriends = offlinefriends.Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();

                onlinefriends.ForEach(x => this.AddOnlineFriend(x));
                offlinefriends.ForEach(x => this.AddOfflineFriend(x));
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

        internal void SetOnlineFriendsCount(int count,int total)
        {
            this.TBOnlineFriendCount.Text = $"Online - {count}/{total}";
        }

        internal void SetOfflineFriendsCount(int count, int total)
        {
            this.TBOfflineFriendCount.Text = $"Offline - {count}/{total}";
        }

        internal void ClearOnlineFriends()
        {
            this.LVOnlineFriends.Items.Clear();
        }

        internal void ClearOfflineFriends()
        {
            this.LVOfflineFriends.Items.Clear();
        }

        internal void AddOnlineFriend(Friend friend)
        {
            FriendControl ctrl = new FriendControl
            {
                IsHitTestVisible = false
            };
            ctrl.Update(friend);
            this.LVOnlineFriends.Items.Add(ctrl);
        }

        internal void AddOfflineFriend(Friend friend)
        {
            FriendControl ctrl = new FriendControl
            {
                IsHitTestVisible = false
            };
            ctrl.Update(friend);
            this.LVOfflineFriends.Items.Add(ctrl);
        }
    }
}
