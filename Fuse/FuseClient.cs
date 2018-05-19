using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Fuse.Models;
using Fuse.Windows;
using Fuse.Properties;
using SteamKit2;
using Fuse.Utils;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

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
        private SteamApps              _AppsHandler;
        private SteamUser.LogOnDetails _Details;
        private Thread                 _CallbackThread;
        private StoredApps             _StoredApps;

        private bool _HasInitialized       = false;
        private bool _IsRunning            = true;
        private bool _IgnoreNextDisconnect = false;
        private bool _IsExit               = false;

        internal FuseClient()
        {
            this._Details = new SteamUser.LogOnDetails();
            this._Details.ShouldRememberPassword = true;

            this._Config = SteamConfiguration.Create(builder =>
            {
                builder.WithConnectionTimeout(TimeSpan.FromMinutes(1));
                builder.WithProtocolTypes(ProtocolTypes.WebSocket);
            });

            this._ClientHandler  = new SteamClient(this._Config);
            this._Manager        = new CallbackManager(this._ClientHandler);
            this._UserHandler    = this._ClientHandler.GetHandler<SteamUser>();
            this._FriendsHandler = this._ClientHandler.GetHandler<SteamFriends>();
            this._AppsHandler    = this._ClientHandler.GetHandler<SteamKit2.SteamApps>();
            this._User           = new FuseUser(this._FriendsHandler);
            this._UI             = new FuseUI(this);
            this._StoredApps     = null;

            this._CallbackThread = new Thread(AwaitCallbackResults);
            this._CallbackThread.Start();
            
            
            this._Manager.Subscribe<SteamClient.ConnectedCallback>(this.OnConnected);
            this._Manager.Subscribe<SteamClient.DisconnectedCallback>(this.OnDisconnected);
            this._Manager.Subscribe<SteamUser.LoggedOnCallback>(this.OnLoggedOn);
            this._Manager.Subscribe<SteamUser.LoginKeyCallback>(this.OnLoginKey);
            this._Manager.Subscribe<SteamUser.AccountInfoCallback>(this.OnAccountInfo);
            this._Manager.Subscribe<SteamFriends.PersonaStateCallback>(this.OnFriendPersonaStateChange);
            this._Manager.Subscribe<SteamFriends.FriendMsgCallback>(this.OnFriendMessage);
            this._Manager.Subscribe<SteamFriends.FriendMsgEchoCallback>(this.OnFriendMessageEcho);
        }

        internal SteamClient            ClientHandler    { get => this._ClientHandler;  }
        internal SteamUser              UserHandler      { get => this._UserHandler;    }
        internal SteamFriends           FriendsHandler   { get => this._FriendsHandler; }
        internal CallbackManager        Manager          { get => this._Manager;        }
        internal bool                   HasInitialized   { get => this._HasInitialized; }
        internal FuseUI                 UI               { get => this._UI;             }
        internal FuseUser               User             { get => this._User;           }

        internal void Connect(string user,string pass,string code=null,string authcode=null)
        {
            this._Details.LoginKey = null;
            this._Details.Username = user;
            this._Details.Password = pass;
            if (code != null) this._Details.TwoFactorCode = code;
            if (authcode != null) this._Details.AuthCode = authcode;
            this._ClientHandler.Connect();
        }

        internal void Connect(FuseCredentials creds)
        {
            this._Details.LoginKey = creds.LoginKey;
            this._Details.Username = creds.Username;
            this._ClientHandler.Connect();
        }

        internal void RunOnSTA(Action cb)
        {
            Application app = Application.Current;
            if(app != null)
                app.Dispatcher.InvokeAsync(cb);
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
            this.RunOnSTA(() =>
            {
                this._User.UpdateLocalUser(cb.ClientSteamID);
                if (cb.Result == EResult.OK)
                {
                    this._UI.ClientWindow.Show();
                    this._UI.ClientWindow.UpdateLocalUser();
                }
                else
                {
                    this._IgnoreNextDisconnect = true;
                    if (cb.Result == EResult.AccountLoginDeniedNeedTwoFactor|| cb.Result == EResult.AccountLogonDeniedVerifiedEmailRequired)
                    {
                        bool isphone = cb.Result == EResult.AccountLoginDeniedNeedTwoFactor;
                        _2FACWindow win = new _2FACWindow(this, this._Details.Username, this._Details.Password, isphone);
                        win.ShowDialog();
                    }
                    else
                    {
                        this._UI.ShowException($"There was an issue with your credentials: {cb.ExtendedResult}");
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

        private async void OnAccountInfo(SteamUser.AccountInfoCallback cb)
        {
            string json = await HTTP.Fetch(this._UI, "https://api.steampowered.com/ISteamApps/GetAppList/v2/");
            if(json != string.Empty)
            {
                try
                {
                    StoredApps apps = JsonConvert.DeserializeObject<StoredApps>(json);
                    this._StoredApps = apps;
                }
                catch
                {
                    this._StoredApps = null;
                    this._UI.ShowException("There was an issue while getting data from the steam network!");
                }
            }
            else
            {
                this._StoredApps = null;
                this._UI.ShowException("There was an issue while getting data from the steam network!");
            }

            #pragma warning disable CS4014
            this._FriendsHandler.SetPersonaState(EPersonaState.Online);
            #pragma warning restore CS4014 
        }

        private bool TryGetAppName(GameID id,out string name)
        {
            try
            {
                SteamApp app = this._StoredApps.GetApp(id.AppID);
                if (app != null)
                {
                    name = app.name;
                    return true;
                }
                else
                {
                    name = null;
                    return false;
                }
                    
            }
            catch
            {
                name = null;
                return false;
            }
        }

        private void OnFriendPersonaStateChange(SteamFriends.PersonaStateCallback cb)
        {
            this.RunOnSTA(() =>
            {
                if (cb.FriendID.AccountID == this._ClientHandler.SteamID.AccountID)
                {
                    GameID gid = this._FriendsHandler.GetFriendGamePlayed(cb.FriendID);
                    string game = this.TryGetAppName(gid, out string gn) ? gn : null;
                    this._User.UpdateLocalUser(cb.FriendID, cb.Name, cb.State, cb.AvatarHash, game);
                    this._UI.ClientWindow.UpdateLocalUser();
                }
                else
                {
                    EPersonaState? oldstate = this._User.GetFriend(cb.FriendID.AccountID)?.State;
                    GameID gid = this._FriendsHandler.GetFriendGamePlayed(cb.FriendID);
                    string game = this.TryGetAppName(gid, out string gn) ? gn : null;
                    this._User.UpdateFriend(cb.FriendID, cb.Name, cb.State, cb.AvatarHash, game);

                    ClientWindow win = this._UI.ClientWindow;
                    win.UpdateFriendList();

                    //We dont want to have notification if the state doesnt change
                    if (oldstate.HasValue && oldstate == cb.State) return;

                    Discussion cur = this._User.CurrentDiscussion;
                    if (cur == null) return;

                    if (!cur.IsGroup)
                    {
                        User friend = this._User.GetFriend(cb.FriendID.AccountID);
                        if (cur.Recipient.AccountID == friend.AccountID)
                        {
                            string state = cb.State.ToString().ToLower();
                            switch(cb.State)
                            {
                                case EPersonaState.LookingToPlay:
                                    state = "looking to play";
                                    break;
                                case EPersonaState.Max:
                                    state = "online";
                                    break;
                                case EPersonaState.LookingToTrade:
                                    state = "looking to trade";
                                    break;
                                default:
                                    break;
                            }

                            string content = $"{friend.Name} is now {state}";
                            Message msg = new Message(friend, content, null, true);
                            friend.Messages.Add(msg);
                            win.AppendChatNotification(msg);
                        }
                    }
                }
            });
        }

        private void HandleFriendMessage(SteamID friendid,string msgcontent,bool islocaluser=false)
        {
            this._User.UpdateFriend(friendid);
            User friend = this._User.GetFriend(friendid.AccountID);
            if (friend != null)
            {
                Message msg = islocaluser ? new Message(this._User.LocalUser, msgcontent) : new Message(friend, msgcontent);
                friend.Messages.Add(msg);

                Discussion cur = this._User.CurrentDiscussion;
                ClientWindow win = this._UI.ClientWindow;
                Discussion disc = this._User.GetDiscussion(friendid.AccountID);
                if (disc == null)
                    disc = new Discussion(this._FriendsHandler, friend);
                disc.IsRecent = true;
                this._User.UpdateDiscussion(friendid.AccountID, disc);

                if (cur != null)
                {
                    if (!cur.IsGroup)
                    {
                        if (cur.Recipient.AccountID == friendid.AccountID)
                        {
                            if (!islocaluser)
                                win.HideTyping();
                            win.AppendChatMessage(msg);
                        }
                        else
                        {
                            if (!islocaluser)
                            {
                                friend.NewMessages++;
                                
                                win.UpdateFriendList();
                                if (!win.IsFocused)
                                {
                                    this._UI.PlayStream(Resources.Message);
                                    User32Flash.FlashWindow(win);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (!islocaluser)
                    {
                        friend.NewMessages++;
                        win.UpdateFriendList();
                        if (!win.IsFocused)
                        {
                            this._UI.PlayStream(Resources.Message);
                            User32Flash.FlashWindow(win);
                        }
                    }
                }

            }
        }

        private void HandleFriendTyping(SteamID friendid)
        {
            this._User.UpdateFriend(friendid);

            User friend = this._User.GetFriend(friendid.AccountID);
            if (friend != null)
            {
                Discussion cur = this._User.CurrentDiscussion;
                ClientWindow win = this._UI.ClientWindow;
                if (cur != null && !cur.IsGroup)
                    if (cur.Recipient.AccountID == friendid.AccountID)
                        win.ShowTyping(friend);
            }
        }

        private void HandleFriendGameInvite(SteamID friendid)
        {
            this._User.UpdateFriend(friendid);

            User friend = this._User.GetFriend(friendid.AccountID);
            if (friend != null)
            {
                Discussion cur = this._User.CurrentDiscussion;
                ClientWindow win = this._UI.ClientWindow;
                if (cur != null && !cur.IsGroup)
                    if (cur.Recipient.AccountID == friendid.AccountID)
                    {
                        Message msg = new Message(friend, $"{friend.Name} invited you to play a game", null, true);
                        win.AppendChatNotification(msg);
                    }
            }
        }

        private void OnFriendMessage(SteamFriends.FriendMsgCallback cb)
        {
            this.RunOnSTA(() =>
            {
                switch (cb.EntryType)
                {
                    case EChatEntryType.Typing:
                        this.HandleFriendTyping(cb.Sender);
                        break;
                    case EChatEntryType.InviteGame:
                        this.HandleFriendGameInvite(cb.Sender);
                        break;
                    default:
                        break;
                }

                if (cb.EntryType != EChatEntryType.ChatMsg) return;
                this.HandleFriendMessage(cb.Sender, cb.Message);
            });
        }

        private void OnFriendMessageEcho(SteamFriends.FriendMsgEchoCallback cb)
        {
            if (cb.EntryType != EChatEntryType.ChatMsg) return;
            this.RunOnSTA(() => this.HandleFriendMessage(cb.Recipient, cb.Message, true));
        }

        private void AwaitCallbackResults()
        {
            while (this._IsRunning)
                this._Manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
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
    }
}
