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
        private User _LastMessageAuthor = null;

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
            Discussion disc = this._Client.User.CurrentDiscussion;
            if (!string.IsNullOrWhiteSpace(content) && disc != null)
            {
                Message msg = new Message(this._Client.User.LocalUser, content);
                disc.SendMessage(msg);
                this.AddCurrentMessage(msg);
                this.TBMessage.Text = string.Empty;
            }
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
            if (string.IsNullOrWhiteSpace(search))
            {
                this._IsSearchingFriends = false;
                this.PLSearchFriends.Visibility = Visibility.Visible;
            }
            else
            {
                this._IsSearchingFriends = true;
                this.PLSearchFriends.Visibility = Visibility.Hidden;
            }
            this.UpdateFriendList(search);
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

        internal void UpdateFriendList(string search=null)
        {
            if (search == null && this.IsSearchingFriends)
                search = this.TBSearchFriends.Text;

            FuseUser fuseuser = this._Client.User;
            this.ClearOnlineFriends();
            this.ClearOfflineFriends();

            List<User> onlinefriends = fuseuser.OnlineFriends;
            int oncount = onlinefriends.Count;
            List<User> offlinefriends = fuseuser.OfflineFriends;
            int offcount = offlinefriends.Count;

            if (search == null)
            {
                onlinefriends.Sort((x, y) => x.Name.CompareTo(y.Name));
                onlinefriends.ForEach(x => this.AddOnlineFriend(x));
                offlinefriends.Sort((x, y) => x.Name.CompareTo(y.Name));
                offlinefriends.ForEach(x => this.AddOfflineFriend(x));
            }
            else
            {
                onlinefriends = onlinefriends.Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();
                onlinefriends.Sort((x, y) => x.Name.CompareTo(y.Name));
                offlinefriends = offlinefriends.Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();
                offlinefriends.Sort((x, y) => x.Name.CompareTo(y.Name));

                onlinefriends.ForEach(x => this.AddOnlineFriend(x));
                offlinefriends.ForEach(x => this.AddOfflineFriend(x));
            }

            this.SetOnlineFriendsCount(oncount, fuseuser.Friends.Count);
            this.SetOfflineFriendsCount(offcount, fuseuser.Friends.Count);
        }

        private void SetOnlineFriendsCount(int count, int total)
        {
            this.TBOnlineFriendCount.Text = $"Online - {count}/{total}";
        }

        private void SetOfflineFriendsCount(int count, int total)
        {
            this.TBOfflineFriendCount.Text = $"Offline - {count}/{total}";
        }

        private void ClearOnlineFriends()
        {
            this.ICOnlineFriends.Items.Clear();
        }

        private void ClearOfflineFriends()
        {
            this.ICOfflineFriends.Items.Clear();
        }

        private void AddOnlineFriend(User friend)
        {
            FriendControl ctrl = new FriendControl(this._Client, friend);
            ctrl.Update();
            int i = this.ICOnlineFriends.Items.Add(ctrl);
        }

        private void AddOfflineFriend(User friend)
        {
            FriendControl ctrl = new FriendControl(this._Client, friend);
            ctrl.Update();
            this.ICOfflineFriends.Items.Add(ctrl);
        }

        internal void AddCurrentMessage(Message msg)
        {
            if (this.PLMessageBackground.Visibility == Visibility.Visible)
                this.PLMessageBackground.Visibility = Visibility.Hidden;

            User lastauthor = this._LastMessageAuthor;
            if (lastauthor != null && lastauthor.AccountID == msg.Author.AccountID)
            {
                CompactMessageControl ctrl = new CompactMessageControl(msg);
                ctrl.Update();
                this.ICCurrentMessages.Items.Add(ctrl);
            }
            else
            {
                MessageControl ctrl = new MessageControl(msg);
                ctrl.Update();
                this.ICCurrentMessages.Items.Add(ctrl);
            }
            this._LastMessageAuthor = msg.Author;
            this.SVCurrentMessages.ScrollToEnd();
        }

        internal void ClearDiscussion()
        {
            this.ICCurrentMessages.Items.Clear();
            this._LastMessageAuthor = null;
        }

        internal bool IsRecentChat(Discussion disc)
        {
            List<Discussion> discs = this._Client.User.Discussions;
            if (!disc.IsGroup)
            {
                return discs.Any(x => !x.IsGroup && x.Recipient.AccountID == disc.Recipient.AccountID);
            }
            else
            {
                //Group implementation here
                return false;
            }
        }

        private void AddRecentDiscussion(Discussion disc)
        {
            if (!disc.IsRecent) return;
            if (!disc.IsGroup)
            {
                FriendControl ctrl = new FriendControl(this._Client, disc.Recipient);
                ctrl.Update();
                this.LVRecentDiscussions.Items.Add(ctrl);
            }
            else
            {
                //Group implementation
            }
        }

        internal void UpdateRecentChats()
        {
            this._Client.User.Discussions.ForEach(x => this.AddRecentDiscussion(x));
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
