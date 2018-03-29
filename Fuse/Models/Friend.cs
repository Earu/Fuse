using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;

namespace Fuse.Models
{
    internal class Friend
    {
        private string _Name;
        private string _SteamID;
        private string _AvatarLink;
        private ulong _SteamID64;
        private EPersonaState _State;
        private string _Game;
        private List<Message> _Messages;

        internal Friend(string name, string steamid, ulong id64, EPersonaState state,byte[] bhash)
        {
            this._Game = null;
            this._Messages = new List<Message>();
            this._Name = name ?? steamid;
            this._SteamID = steamid;
            this._SteamID64 = id64;
            this._State = state;
            if (bhash == null)
            {
                this._AvatarLink = "Ressources/default_avatar.png";
            }
            else
            {
                string hash = this.BytesToHash(bhash);
                this._AvatarLink = $"https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/dd/{hash}_full.jpg";
            }
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
            this._AvatarLink = $"https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/dd/{hash}_full.jpg";
        }

        internal string GetGame()
        {
            return this._Game;
        }

        internal string Name            { get => this._Name;      set => this._Name      = value; }
        internal string SteamID         { get => this._SteamID;   set => this._Name      = value; }
        internal ulong SteamID64        { get => this._SteamID64; set => this._SteamID64 = value; }
        internal EPersonaState State    { get => this._State;     set => this._State     = value; }
        internal string Game            { get => this._Game;      set => this._Game      = value; }
        internal List<Message> Messages { get => this._Messages; }
        internal string AvatarLink      { get => this._AvatarLink; }
    }
}
