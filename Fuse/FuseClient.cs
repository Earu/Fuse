using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Fuse.Models;
using Fuse.Windows;
using SteamKit2;

namespace Fuse
{
    internal class FuseClient
    {
        private FuseUI   _UI;
        private FuseUser _User;

        private SteamConfiguration     _Config;
        private SteamClient            _ClientHandler;
        private CallbackManager        _Manager;
        private SteamUser              _UserHandler;
        private SteamFriends           _FriendsHandler;
        private SteamUser.LogOnDetails _Details;
        private Thread                 _CallbackThread;

        private bool _HasInitialized       = false;
        private bool _IsRunning            = true;
        private bool _IgnoreNextDisconnect = false;
        private bool _IsExit               = false;

        internal FuseClient()
        {
            this._Details = new SteamUser.LogOnDetails
            {
                ShouldRememberPassword = true
            };
            this._Config = SteamConfiguration.Create(builder =>
            {
                builder.WithConnectionTimeout(TimeSpan.FromSeconds(5));
                builder.WithProtocolTypes(ProtocolTypes.WebSocket);
            });

            this._ClientHandler = new SteamClient(this._Config);
            this._Manager = new CallbackManager(this._ClientHandler);
            this._UserHandler = this._ClientHandler.GetHandler<SteamUser>();
            this._FriendsHandler = this._ClientHandler.GetHandler<SteamFriends>();
            this._UI = new FuseUI(this);
            this._User = new FuseUser(this._FriendsHandler);

            this._CallbackThread = new Thread(AwaitCallbackResults);
            this._CallbackThread.Start();
            
            this._Manager.Subscribe<SteamClient.ConnectedCallback>(this.OnConnected);
            this._Manager.Subscribe<SteamClient.DisconnectedCallback>(this.OnDisconnected);
            this._Manager.Subscribe<SteamUser.LoggedOnCallback>(this.OnLoggedOn);
            this._Manager.Subscribe<SteamUser.LoginKeyCallback>(this.OnLoginKey);
            this._Manager.Subscribe<SteamUser.AccountInfoCallback>(this.OnAccountInfo);
            this._Manager.Subscribe<SteamFriends.PersonaStateCallback>(this.OnFriendPersonaStateChange);
            this._Manager.Subscribe<SteamFriends.FriendMsgCallback>(this.OnFriendMessage);
            
        }

        internal void Connect(string user,string pass,string code=null)
        {
            this._Details.LoginKey = null;
            this._Details.Username = user;
            this._Details.Password = pass;
            if (code != null) this._Details.TwoFactorCode = code;
            this._ClientHandler.Connect();
        }

        internal void Connect(FuseCredentials creds)
        {
            this._Details.LoginKey = creds.LoginKey;
            this._Details.Username = creds.Username;
            this._ClientHandler.Connect();
        }

        private void RunOnSTA(Action cb)
        {
            Application app = Application.Current;
            if(app != null)
                app.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(cb));
        }

        private void OnConnected(SteamClient.ConnectedCallback cb)
        {
            this._UserHandler.LogOn(_Details);
            this._HasInitialized = true;
        }

        private void OnDisconnected(SteamClient.DisconnectedCallback cb)
        {
            if (this._IgnoreNextDisconnect)
            {
                this._IgnoreNextDisconnect = false;
                return;
            }

            this.RunOnSTA(() =>
            {
                //Because STA is the main thread
                if (this._IsExit)
                {
                    this._CallbackThread.Abort();
                    Process.GetCurrentProcess().Kill();
                    return;
                }

                this._UI.ShowException("No connection could be made to steam network");
                if (this._UI.ClientWindow.IsVisible)
                    this._UI.ClientWindow.Close();
                this._UI.ShowLogin();
            });
        }

        private void OnLoggedOn(SteamUser.LoggedOnCallback cb)
        {
            this._IgnoreNextDisconnect = false;
            this.RunOnSTA(() =>
            {
                if (cb.Result == EResult.OK)
                {
                    this._UI.ClientWindow.Show();
                }
                else
                {
                    this._IgnoreNextDisconnect = true;
                    if (cb.Result == EResult.AccountLoginDeniedNeedTwoFactor)
                    {
                        _2FACWindow win = new _2FACWindow(this,this._UI,this._Details.Username, this._Details.Password);
                        win.ShowDialog();
                    }
                    else
                    {
                        this._UI.ShowException("There was an issue with your credentials: " +
                            $"{cb.Result} -> {cb.ExtendedResult}");
                        this._UI.ShowLogin();
                    }
                }
            });
        }

        private void OnLoginKey(SteamUser.LoginKeyCallback cb)
        {
            this._Details.LoginKey = cb.LoginKey;
            FuseCredentials creds = new FuseCredentials(this._Details);
            creds.Save();
            this._UserHandler.AcceptNewLoginKey(cb);
        }

        private void OnAccountInfo(SteamUser.AccountInfoCallback cb)
        {
            this._FriendsHandler.SetPersonaState(EPersonaState.Online);
        }

        private void OnFriendPersonaStateChange(SteamFriends.PersonaStateCallback cb)
        {
            this.RunOnSTA(() =>
            {
                ClientWindow win = this._UI.ClientWindow;
                if (win.IsSearchingFriends) return;
                this._User.UpdateFriend(cb.FriendID);

                win.ClearOnlineFriends();
                win.ClearOfflineFriends();

                List<Friend> onlinefriends = this._User.OnlineFriends;
                List<Friend> offlinefriends = this._User.OfflineFriends;

                onlinefriends.Sort((x,y) => x.Name.CompareTo(y.Name));
                onlinefriends.ForEach(x => win.AddOnlineFriend(x));
                offlinefriends.Sort((x, y) => x.Name.CompareTo(y.Name));
                offlinefriends.ForEach(x => win.AddOfflineFriend(x));

                win.SetOnlineFriendsCount(onlinefriends.Count, this._User.Friends.Count);
                win.SetOfflineFriendsCount(offlinefriends.Count, this._User.Friends.Count);
            });
        }

        private void OnFriendMessage(SteamFriends.FriendMsgCallback cb)
        {
            int index = this._User.GetFriendIndex(cb.Sender.ConvertToUInt64());
            if(index != -1) this._User.Friends[index].Messages.Add(new Message(cb.Message));
        }

        private void AwaitCallbackResults()
        {
            while (this._IsRunning)
                this._Manager.RunWaitCallbacks(TimeSpan.FromMilliseconds(100));
        }

        internal void Start()
        {
            this._IsRunning = true;
            if (FuseCredentials.TryLoad(out FuseCredentials creds))
            {
                this.Connect(creds);
            }
            else
            {
                this._UI.ShowLogin();
            }
        }

        internal void Stop(bool now=false)
        {
            this._IsRunning = false;
            if (now)
            {
                this._CallbackThread.Abort();
                Process.GetCurrentProcess().Kill();
            }
            else
            {
                this._UserHandler.LogOff();
                this._IsExit = true;
                this._ClientHandler.Disconnect();
            }
        }

        internal SteamClient ClientHandler { get => this._ClientHandler;  set => this._ClientHandler  = value; }
        internal SteamUser UserHandler     { get => this._UserHandler;    set => this._UserHandler    = value; }
        internal CallbackManager Manager   { get => this._Manager;        set => this._Manager        = value; }
        internal bool HasInitialized       { get => this._HasInitialized; set => this._HasInitialized = value; }
        internal FuseUser User             { get => this._User; }
    }
}
