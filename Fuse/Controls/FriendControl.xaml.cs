using Fuse.Models;
using Fuse.Windows;
using SteamKit2;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Fuse.Controls
{
    /// <summary>
    /// Interaction logic for FriendControl.xaml
    /// </summary>
    public partial class FriendControl : UserControl
    {
        private static Dictionary<EPersonaState, SolidColorBrush> _StateColors = new Dictionary<EPersonaState, SolidColorBrush>
        {
            [EPersonaState.Offline]        = Brushes.Gray,
            [EPersonaState.Online]         = Brushes.DodgerBlue,
            [EPersonaState.Away]           = Brushes.DarkOrange,
            [EPersonaState.Busy]           = Brushes.IndianRed,
            [EPersonaState.LookingToPlay]  = Brushes.DodgerBlue,
            [EPersonaState.LookingToTrade] = Brushes.DodgerBlue,
            [EPersonaState.Max]            = Brushes.DodgerBlue,
            [EPersonaState.Snooze]         = Brushes.DarkOrange,
        };

        private User _Friend;
        private FuseClient _Client;

        internal FriendControl(FuseClient client, User friend)
        {
            this.InitializeComponent();
            this._Friend = friend;
            this._Client = client;
        }

        internal User Friend { get => this._Friend; }

        internal void Update(User friend=null)
        {
            if(friend != null) this._Friend = friend;
            friend = this._Friend;

            string name = friend.Name;
            EPersonaState state = friend.State;
            string game = friend.Game;
            BitmapImage source = friend.Avatar;

            this.IMAvatar.Source = source;

            name = name.Length >= 15 ? name.Substring(0, 15) + "..." : name;
            this.TBName.Text = name;
            if (game != null)
            {
                game = game.Length >= 20 ? game.Substring(0, 20) + "..." : game;
                this.TBState.Text = $"Playing {game}";
                this.RCState.Fill = Brushes.Green;
            }
            else
            {
                this.TBState.Text = state.ToString();
                this.RCState.Fill = StateToColor(state);
            }

            if (friend.NewMessages > 0)
            {
                this.TBNewMessages.Text = friend.NewMessages.ToString();
            }
        }

        internal static SolidColorBrush StateToColor(EPersonaState state)
        {
            return _StateColors[state];
        }

        private void OnLeftClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ClientWindow win = this._Client.UI.ClientWindow;
            if (win.IsVisible)
            {
                win.ClearDiscussion();
                this._Friend.NewMessages = 0;
                this.Update();
                Discussion disc = new Discussion(this._Client, this._Friend)
                {
                    IsRecent = true
                };
                this._Client.User.CurrentDiscussion = disc;
                win.LoadDiscussion(disc);
            }
        }
    }
}
