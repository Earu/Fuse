using Fuse.Models;
using SteamKit2;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fuse.Controls
{
    /// <summary>
    /// Interaction logic for SearchedFriendControl.xaml
    /// </summary>
    public partial class SearchedFriendControl : UserControl
    {
        private SteamFriends _FriendsHandler;
        private User _User;

        internal SearchedFriendControl(SteamFriends handler,User user)
        {
            this._FriendsHandler = handler;
            this._User = user;
            this.InitializeComponent();
        }

        internal void Update(User user = null)
        {
            if (user != null) this._User = user;
            user = this._User;

            string name = user.Name;
            EPersonaState state = user.State;
            string game = user.Game;
            BitmapImage source = user.Avatar;
            this.IMAvatar.Source = source;
            this.TBName.Text = name;
            if(user.ExtraInfo.HasValue)
            {
                List<string> names = user.ExtraInfo.Value.PreviousNames;
                this.TBState.Text = names.Count > 0 ? string.Join(", ",names) : "No other names"; 
            }
        }

        private void OnRequestFriend(object sender, RoutedEventArgs e)
        {
            this._FriendsHandler.AddFriend(this._User.SteamID);
            ((ItemsControl)this.Parent).Items.Remove(this);
        }
    }
}
