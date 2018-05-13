using Fuse.Controls;
using Fuse.Models;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
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
        private Timer _TypingTimer = new Timer(30000);

        internal ClientWindow(FuseClient client)
        {
            this.InitializeComponent();
            this._Client = client;
            this._TypingTimer.AutoReset = false;
            this._TypingTimer.Elapsed += (s,e) => this._Client.RunOnSTA(() => this.HideTyping());
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
                this.AppendChatMessage(msg);
                this.TBMessage.Text = string.Empty;
            }
            this.TBMessage.Focus();
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

            this.UpdateRecentDiscussions(search);
            FuseUser fuseuser = this._Client.User;
            this.ClearOnlineFriends();
            this.ClearOfflineFriends();

            Dictionary<uint,User> onlinefriends = fuseuser.OnlineFriends;
            int oncount = onlinefriends.Count;
            Dictionary<uint, User> offlinefriends = fuseuser.OfflineFriends;
            int offcount = offlinefriends.Count;

            if (search == null)
            {
                foreach (KeyValuePair<uint, User> u in onlinefriends)
                    this.AddOnlineFriend(u.Value);
                foreach (KeyValuePair<uint, User> u in offlinefriends)
                    this.AddOfflineFriend(u.Value);
            }
            else
            {
                onlinefriends = onlinefriends
                    .Where(x => x.Value.Name.ToLower().Contains(search.ToLower()))
                    .ToDictionary(k => k.Key,v => v.Value);
                offlinefriends = offlinefriends
                    .Where(x => x.Value.Name.ToLower().Contains(search.ToLower()))
                    .ToDictionary(k => k.Key,v => v.Value);

                foreach (KeyValuePair<uint, User> u in onlinefriends)
                    this.AddOnlineFriend(u.Value);
                foreach (KeyValuePair<uint, User> u in offlinefriends)
                    this.AddOfflineFriend(u.Value);
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

        internal void AppendChatMessage(Message msg)
        {
            //if (this.GLPlaceholder.Visibility == Visibility.Visible)
            //    this.GLPlaceholder.Visibility = Visibility.Hidden;

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

        internal void AppendChatNotification(Message msg)
        {
            //if (this.GLPlaceholder.Visibility == Visibility.Visible)
            //    this.GLPlaceholder.Visibility = Visibility.Hidden;
            NotificationMessageControl ctrl = new NotificationMessageControl(msg);
            ctrl.Update();
            this.ICCurrentMessages.Items.Add(ctrl);
            this.SVCurrentMessages.ScrollToEnd();
            this._LastMessageAuthor = null;
        }

        internal void ClearDiscussion()
        {
            this.ICCurrentMessages.Items.Clear();
            this._LastMessageAuthor = null;
            this.HideTyping();
        }

        internal bool IsRecentChat(Discussion disc)
        {
            Dictionary<uint,Discussion> discs = this._Client.User.Discussions;
            if (!disc.IsGroup)
            {
                return discs.Any(x => !x.Value.IsGroup && x.Value.Recipient.AccountID == disc.Recipient.AccountID);
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
                this.ICRecentDiscussions.Items.Add(ctrl);
            }
            else
            {
                //Group implementation
            }
        }

        private void UpdateRecentDiscussions(string search=null)
        {
            this.ICRecentDiscussions.Items.Clear();
            if (search == null)
            {
                foreach (KeyValuePair<uint, Discussion> disc in this._Client.User.Discussions)
                    if (disc.Value.IsRecent)
                        this.AddRecentDiscussion(disc.Value);
            }
            else
            {
                foreach (KeyValuePair<uint, Discussion> disc in this._Client.User.Discussions)
                {
                    if (disc.Value.IsRecent)
                    {
                        if (disc.Value.IsGroup)
                        {
                            string title = string.Empty;
                            disc.Value.Recipients.ForEach(x => title = $"{title},{x.Name}");
                            title = title.Length >= 50 ? $"{title.Substring(0, 50)}..." : title;
                            if (title.ToLower().Contains(search.ToLower()))
                            {
                                this.AddRecentDiscussion(disc.Value);
                            }
                        }
                        else
                        {
                            if (disc.Value.Recipient.Name.ToLower().Contains(search.ToLower()))
                            {
                                this.AddRecentDiscussion(disc.Value);
                            }
                        }
                    }
                }
            }
        }

        internal void LoadDiscussion(Discussion disc)
        {
            if (!disc.IsGroup)
                this.TBCurrentDiscussionName.Text = $"@{disc.Recipient.Name}";
            else
            {
                string title = string.Empty;
                disc.Recipients.ForEach(x => title = $"{title},{x.Name}");
                title = title.Length >= 50 ? $"{title.Substring(0, 50)}..." : title;
                this.TBCurrentDiscussionName.Text = title;
            }

            List<Message> msgs = disc.Open();
            foreach (Message msg in msgs)
            {
                if (msg.IsNotification)
                    this.AppendChatNotification(msg);
                else
                    this.AppendChatMessage(msg);
            }
        }

        internal void ShowTyping(User user)
        {
            this.RCTyping.Visibility = Visibility.Visible;
            this.TBTyping.Visibility = Visibility.Visible;
            this.TBTyping.Text = $"{user.Name} is typing...";
            this.SVCurrentMessages.Margin = new Thickness(0,0,0,62);
            this._TypingTimer.Start();
        }

        internal void ShowTyping(List<User> users)
        {
            this.RCTyping.Visibility = Visibility.Visible;
            this.TBTyping.Visibility = Visibility.Visible;
            string display = string.Empty;
            users.ForEach(x => display = $"{display},{x.Name}");
            display = $"{display} are typing...";
            this.TBTyping.Text = display;
            this.SVCurrentMessages.Margin = new Thickness(0, 0, 0, 62);
            this._TypingTimer.Start();
        }

        internal void HideTyping()
        {
            this.RCTyping.Visibility = Visibility.Hidden;
            this.TBTyping.Visibility = Visibility.Hidden;
            this.SVCurrentMessages.Margin = new Thickness(0, 0, 0, 41);
            this._TypingTimer.Stop();
        }

        internal void MessageBoxFocus()
        {
            this.TBMessage.Focus();
            Keyboard.Focus(this.TBMessage);
        }

        internal void UpdateLocalUser()
        {
            User localuser = this._Client.User.LocalUser;
            this.IMLocalUserAvatar.Source = localuser.Avatar;
            this.RCLocalUserState.Fill = FriendControl.StateToColor(localuser.State);
            this.TBLocalUserName.Text = localuser.Name;
            this.TBLocalUserState.Text = localuser.State.ToString(); 
        }

        private void AddFriends(object sender, RoutedEventArgs e)
        {
            FriendRequestWindow win;
            if (FriendRequestWindow.Opened)
            {
                win = FriendRequestWindow.CurrentWindow;
                win.Activate();
            }
            else
            {
                win = new FriendRequestWindow(this._Client);
                win.Show();
            }
        }
    }
}
