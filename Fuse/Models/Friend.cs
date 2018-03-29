using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using SteamKit2;

namespace Fuse.Models
{
    internal class Friend
    {
        private string _Name;
        private string _SteamID;
        private BitmapImage _Avatar;
        private ulong _SteamID64;
        private EPersonaState _State;
        private string _Game;
        private List<Message> _Messages;

        internal Friend(string name, string steamid, ulong id64, EPersonaState state,byte[] bhash,List<Message> msgs=null)
        {
            this._Game = null;
            if (msgs == null) this._Messages = new List<Message>();
            else this._Messages = msgs;
            this._Name = name ?? steamid;
            this._SteamID = steamid;
            this._SteamID64 = id64;
            this._State = state;

            this._Avatar = new BitmapImage();
            this._Avatar.BeginInit();
            this._Avatar.DecodePixelHeight = 200;
            this._Avatar.DecodePixelWidth = 200;
            if (bhash == null)
            {
                this._Avatar.UriSource = new Uri("Resources/default_avatar.png", UriKind.Relative);
            }
            else
            {
                string hash = this.BytesToHash(bhash);
                string link = $"https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/dd/{hash}_full.jpg";
                this._Avatar.UriSource = new Uri(link, UriKind.Absolute);
            }
            this._Avatar.EndInit();
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
            string hash = this.BytesToHash(bhash);
            string link = $"https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/dd/{hash}_full.jpg";
            this._Avatar.UriSource = new Uri(link, UriKind.Absolute);
        }

        internal string Name            { get => this._Name;      set => this._Name      = value; }
        internal string SteamID         { get => this._SteamID;   set => this._Name      = value; }
        internal ulong SteamID64        { get => this._SteamID64; set => this._SteamID64 = value; }
        internal EPersonaState State    { get => this._State;     set => this._State     = value; }
        internal string Game            { get => this._Game;      set => this._Game      = value; }
        internal List<Message> Messages { get => this._Messages; }
        internal BitmapImage Avatar     { get => this._Avatar; }
    }
}
