using Fuse.Controls;
using Fuse.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Fuse.Windows
{
    /// <summary>
    /// Interaction logic for ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private FuseClient _Client;
        private bool _IsSearchingFriends = false;

        internal ClientWindow(FuseClient client)
        {
            this.InitializeComponent();
            this._Client = client;
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
            string content = this.TBMessage.Text;
            Message msg = new Message(this._Client.User.LocalUser, content);
            this._Client.User.CurrentDiscussion.SendMessage(content);
            this.AddCurrentMessage(msg);
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
            List<User> onlinefriends = this._Client.User.OnlineFriends;
            List<User> offlinefriends = this._Client.User.OfflineFriends;

            onlinefriends.Sort((x, y) => x.Name.CompareTo(y.Name));
            offlinefriends.Sort((x, y) => x.Name.CompareTo(y.Name));

            this.ClearOnlineFriends();
            this.ClearOfflineFriends();

            if (string.IsNullOrWhiteSpace(search))
            {
                this._IsSearchingFriends = false;
                this.PLSearchFriends.Visibility = Visibility.Visible;

                onlinefriends.ForEach(x => this.AddOnlineFriend(x));
                offlinefriends.ForEach(x => this.AddOfflineFriend(x));
            }
            else
            {
                this._IsSearchingFriends = true;
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
            if (msg.Length > 2048)
            {
                this.TBMessage.Text = msg.Substring(0, 2048);
            }

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
            this.ICOnlineFriends.Items.Clear();
        }

        internal void ClearOfflineFriends()
        {
            this.ICOfflineFriends.Items.Clear();
        }

        internal void AddOnlineFriend(User friend)
        {
            FriendControl ctrl = new FriendControl(this._Client, friend);
            ctrl.Update();
            int i = this.ICOnlineFriends.Items.Add(ctrl);
        }

        internal void AddOfflineFriend(User friend)
        {
            FriendControl ctrl = new FriendControl(this._Client, friend);
            ctrl.Update();
            this.ICOfflineFriends.Items.Add(ctrl);
        }

        private void AddCurrentMessage(Message msg)
        {
            MessageControl ctrl = new MessageControl(msg);
            ctrl.Update();
            this.ICCurrentMessages.Items.Add(ctrl);
        }

        internal void ClearDiscussion()
        {
            this.ICCurrentMessages.Items.Clear();
        }

        internal void LoadDiscussion(Discussion disc)
        {
            if (!disc.IsGroup)
            {
                this.TBCurrentDiscussionName.Text = $"@{disc.Recipient.Name}";
            }
            List<Message> msgs = disc.Open();
            foreach (Message msg in msgs)
            {
                this.AddCurrentMessage(msg);
            }
        }
    }
}
