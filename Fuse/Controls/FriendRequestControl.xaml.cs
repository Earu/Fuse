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
    /// Interaction logic for FriendRequestControl.xaml
    /// </summary>
    public partial class FriendRequestControl : UserControl
    {
        private FuseUser _LocalUser;
        private User _Requester;

        internal FriendRequestControl(User requester, FuseUser luser)
        {
            this._LocalUser = luser;
            this._Requester = requester;
            this.InitializeComponent();
        }

        internal void Update(User requester = null)
        {
            if (requester != null) this._Requester = requester;
            requester = this._Requester;

            string name         = requester.Name;
            EPersonaState state = requester.State;
            string game         = requester.Game;
            BitmapImage source  = requester.Avatar;

            this.IMAvatar.Source = source;

            this.TBName.Text = name;
            if (game != null)
            {
                this.TBState.Text = $"Playing {game}";
                this.RCState.Fill = Brushes.Green;
            }
            else
            {
                this.TBState.Text = state.ToString();
                this.RCState.Fill = FriendControl.StateToColor(state);
            }
        }

        private void OnAcceptFriend(object sender, RoutedEventArgs e)
        {
            this._LocalUser.AcceptUser(this._Requester);
            ((ItemsControl)this.Parent).Items.Remove(this);
        }

        private void OnRefuseFriend(object sender, RoutedEventArgs e)
        {
            this._LocalUser.RefuseUser(this._Requester);
            ((ItemsControl)this.Parent).Items.Remove(this);
        }
    }
}
