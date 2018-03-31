using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using SteamKit2;

namespace Fuse.Models
{
    internal class User
    {
        private string _Name;
        private SteamID _SteamID;
        private BitmapImage _Avatar;
        private ulong _SteamID64;
        private uint _AccountID;
        private EPersonaState _State;
        private string _Game;
        private List<Message> _Messages;
        private int _NewMessages;

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

            this._Avatar = new BitmapImage();
            this._Avatar.DecodeFailed += this.OnAvatarFail;
            this._Avatar.DownloadFailed += this.OnAvatarFail;
            this._Avatar.BeginInit();
            if (bhash == null)
            {
                this._Avatar.UriSource = new Uri("Resources/default_avatar.png", UriKind.Relative);
            }
            else
            {
                string hash = this.BytesToHash(bhash);
                string link = $"https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/dd/{hash}_full.jpg?width=200&height=200";
                this._Avatar.UriSource = new Uri(link, UriKind.Absolute);
            }
            this._Avatar.EndInit();
        }

        private void OnAvatarFail(object sender,EventArgs e)
        {
            this._Avatar.UriSource = new Uri("Resources/default_avatar.png", UriKind.Relative);
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

        internal string        Name        { get => this._Name;      }
        internal SteamID       SteamID     { get => this._SteamID;   }
        internal uint          AccountID   { get => this._AccountID; }
        internal ulong         SteamID64   { get => this._SteamID64; }
        internal EPersonaState State       { get => this._State;     }
        internal string        Game        { get => this._Game;      }
        internal List<Message> Messages    { get => this._Messages;  }
        internal BitmapImage   Avatar      { get => this._Avatar;    }
        internal int           NewMessages { get => this._NewMessages; set => this._NewMessages = value; }
    }
}
