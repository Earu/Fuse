using Fuse.Controls;
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
using System.Windows.Shapes;

namespace Fuse.Windows
{
    /// <summary>
    /// Interaction logic for AddFriendWindow.xaml
    /// </summary>
    public partial class FriendRequestWindow : Window
    {
        private static bool _Opened = false;
        private static FriendRequestWindow _CurrentWindow = null;

        private FuseUser _User;

        internal FriendRequestWindow(FuseUser user)
        {
            this._User = user;
            this.InitializeComponent();
            this.Topmost = true;
            _Opened = true;
            _CurrentWindow = this;
            this.Closing += this.OnClose;

            foreach (KeyValuePair<uint,User> u in user.Requesteds)
            {
                FriendRequestControl ctrl = new FriendRequestControl(u.Value, user);
                ctrl.Update();
                this.ICRequests.Items.Add(ctrl);
            }
        }

        internal static bool                Opened        { get => _Opened;        }
        internal static FriendRequestWindow CurrentWindow { get => _CurrentWindow; }

        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _Opened = false;
            _CurrentWindow = null;
        }

        private void OnMouseDownDrag(object sender, EventArgs e)
        {
            this.DragMove();
        }

        private void OnClose(object sender,EventArgs e)
        {
            this.Close();
        }

        private void OnPeopleTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.TBSearchPeople.Text))
            {
                this.PLSearchPeople.Visibility = Visibility.Visible;
            }
            else
            {
                this.PLSearchPeople.Visibility = Visibility.Hidden;
            }
        }

        private void Search()
        {
        }
    }
}
