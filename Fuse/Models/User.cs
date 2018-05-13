using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using SteamKit2;

namespace Fuse.Models
{
    internal class User
    {
        private string        _Name;
        private SteamID       _SteamID;
        private BitmapImage   _Avatar;
        private ulong         _SteamID64;
        private uint          _AccountID;
        private EPersonaState _State;
        private string        _Game;
        private List<Message> _Messages;
        private int           _NewMessages;

        internal User(string name,SteamID id, EPersonaState state,byte[] bhash,List<Message> msgs=null,int newmsg=0)
        {
            this._Game = null;
            if (msgs == null) this._Messages = new List<Message>();
            else this._Messages = msgs;
            this._NewMessages = newmsg;
            string steamid = id.Render();
            this._AccountID = id.AccountID;
            ulong id64 = id.ConvertToUInt64();
            this._Name = name ?? steamid;
            this._SteamID = id;
            this._SteamID64 = id64;
            this._State = state;
            this._Avatar = this.CreateAvatar(bhash);
        }

        internal User(User user)
        {
            this._Name = user._Name;
            this._SteamID = user._SteamID;
            this._Avatar = user._Avatar;
            this._SteamID64 = user._SteamID64;
            this._AccountID = user._AccountID;
            this._State = user._State;
            this._Game = user._Game;
            this._Messages = user._Messages;
            this._NewMessages = user._NewMessages;
        }

        internal string        Name        { get => this._Name;      }
        internal SteamID       SteamID     { get => this._SteamID;   }
        internal uint          AccountID   { get => this._AccountID; }
        internal ulong         SteamID64   { get => this._SteamID64; }
        internal EPersonaState State       { get => this._State;     }
        internal BitmapImage   Avatar      { get => this._Avatar;    }
        internal string        Game        { get => this._Game;        set => this._Game        = value; }
        internal List<Message> Messages    { get => this._Messages;    set => this._Messages    = value; }
        internal int           NewMessages { get => this._NewMessages; set => this._NewMessages = value; }

        private BitmapImage CreateAvatar(byte[] bhash)
        {
            BitmapImage img = new BitmapImage
            {
                DecodePixelHeight = 32,
                DecodePixelWidth = 32,
                SourceRect = new Int32Rect(0, 0, 32, 32),
                CreateOptions = new BitmapCreateOptions(),
                CacheOption = BitmapCacheOption.OnLoad,
            };
            img.DecodeFailed += this.OnAvatarFail;
            img.DownloadFailed += this.OnAvatarFail;
            img.BeginInit();
            if (bhash == null)
                img.UriSource = new Uri("Resources/default_avatar.png", UriKind.Relative);
            else
            {
                string hash = this.BytesToHash(bhash);
                string link = $"https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/dd/{hash}_full.jpg?width=32&height=32";
                img.UriSource = new Uri(link, UriKind.Absolute);
            }
            img.EndInit();

            return img;
        }

        private BitmapImage CreateAvatar(string link)
        {
            BitmapImage img = new BitmapImage
            {
                DecodePixelHeight = 32,
                DecodePixelWidth = 32,
                SourceRect = new Int32Rect(0, 0, 32, 32),
                CreateOptions = new BitmapCreateOptions(),
                CacheOption = BitmapCacheOption.OnLoad,
            };
            img.DecodeFailed += this.OnAvatarFail;
            img.DownloadFailed += this.OnAvatarFail;
            img.BeginInit();
            if (link == null)
                img.UriSource = new Uri("Resources/default_avatar.png", UriKind.Relative);
            else
                img.UriSource = new Uri(link, UriKind.Absolute);
            img.EndInit();

            return img;
        }

        private void OnAvatarFail(object sender,EventArgs e)
        {
            this._Avatar = this.CreateAvatar(bhash: null);
        }

        private string BytesToHash(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        internal void SetAvatarHash(byte[] bhash)
        {
            this._Avatar = this.CreateAvatar(bhash);
        }

        internal void SetAvatarLink(string link)
        {
            this._Avatar = this.CreateAvatar(link);
        }


    }
}
