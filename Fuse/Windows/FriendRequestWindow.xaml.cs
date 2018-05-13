using Fuse.Controls;
using Fuse.Models;
using Fuse.Utils;
using SteamKit2;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;
using System.Windows.Input;

namespace Fuse.Windows
{
    /// <summary>
    /// Interaction logic for AddFriendWindow.xaml
    /// </summary>
    public partial class FriendRequestWindow : Window
    {
        private static bool _Opened = false;
        private static FriendRequestWindow _CurrentWindow = null;

        private FuseClient _Client;
        private FuseUser _User;
        private FuseUI _UI;
        private SteamUser.LogOnDetails _Details;

        internal FriendRequestWindow(FuseClient client)
        {
            this._Client = client;
            this._User = client.User;
            this._UI = client.UI;
            this.InitializeComponent();
            this.Topmost = true;
            _Opened = true;
            _CurrentWindow = this;
            this.Closing += this.OnClose;

            foreach (KeyValuePair<uint,User> u in this._User.Requesteds)
            {
                FriendRequestControl ctrl = new FriendRequestControl(u.Value, this._User);
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
                this.PLSearchPeople.Visibility = Visibility.Visible;
            else
                this.PLSearchPeople.Visibility = Visibility.Hidden;
        }

        private async Task Search()
        {
            this.ICPeople.Items.Clear();
            string search = this.TBSearchPeople.Text;
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                byte[] bytes = new byte[12];
                rng.GetBytes(bytes);

                string token = BitConverter.ToString(bytes).Replace("-", "").ToLower();
                ulong id64 = this._User.LocalUser.SteamID64;
                string domain = "https://steamcommunity.com";
                string url = $"{domain}/search/SearchCommunityAjax?&text={search}&search_filter=users&sessionid={token}&steamid_user={id64}";
                string res = await HTTP.Fetch(this._UI, url, null, req => {
                    CookieContainer container = new CookieContainer();
                    Uri uri = new Uri(domain);
                    container.Add(uri, new Cookie("sessionid", token));
                    req.CookieContainer = container;
                });

                UsersSearchResult uresults = JsonConvert.DeserializeObject<UsersSearchResult>(res);
                List<User> results = uresults.GetResults();
                foreach (User u in results)
                {
                    FriendControl ctrl = new FriendControl(this._Client, u);
                    ctrl.Update();
                    this.ICPeople.Items.Add(ctrl);
                }
            }
        }

        private async void OnSearch(object sender, RoutedEventArgs e)
        {
            await this.Search();
        }

        private async void OnSearchKeyDown(object sender,KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                await this.Search();
        }
    }
}
