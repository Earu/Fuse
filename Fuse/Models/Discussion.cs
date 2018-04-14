using SteamKit2;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fuse.Models
{
    internal class Discussion
    {
        private SteamFriends  _FriendsHandler;
        private bool          _IsGroup;
        private User          _Recipient;
        private List<User>    _Recipients;
        private List<Message> _Messages;
        private int           _NewMessages;
        private DateTime      _LastOpened;
        private bool          _IsRecent;

        internal Discussion(SteamFriends handler, User recipient,bool requestedhistory=false)
        {
            this._FriendsHandler   = handler;
            this._IsGroup          = false;
            this._Recipient        = recipient;
            this._NewMessages      = 0;
            this._LastOpened       = DateTime.Now;
            this._IsRecent         = true;
        }

        internal Discussion(SteamFriends handler, List<User> recipients,bool requestedhistory=false)
        {
            this._FriendsHandler   = handler;
            this._IsGroup          = true;
            this._Recipients       = recipients;
            this._Messages         = new List<Message>();
            this._NewMessages      = 0;
            this._LastOpened       = DateTime.Now;
            this._IsRecent         = true;
        }

        internal Discussion(Discussion disc)
        {
            this._FriendsHandler   = disc._FriendsHandler;
            this._IsGroup          = disc._IsGroup;
            this._Recipients       = disc._Recipients;
            this._Recipient        = disc._Recipient;
            this._Messages         = disc._Messages;
            this._NewMessages      = disc._NewMessages;
            this._LastOpened       = disc._LastOpened;
            this._IsRecent         = disc._IsRecent;
        }

        internal List<Message> Open()
        {
            SteamFriends handler = this._FriendsHandler;
            this._LastOpened = DateTime.Now;
            this._NewMessages = 0;
            if (this._IsGroup)
            {
                return this._Messages;
            }
            else
            {
                return this._Recipient.Messages;
            }
        }

        internal void SendMessage(Message msg)
        {
            SteamFriends handler = this._FriendsHandler;
            if (!this._IsGroup)
            {
                this._Recipient.Messages.Add(msg);
                handler.SendChatMessage(this._Recipient.SteamID, EChatEntryType.ChatMsg, msg.Content);
            }
        }

        internal User GetLastAuthor()
        {
            Message msg = this._IsGroup ? this._Messages.Last() : this._Recipient.Messages.Last();
            return msg.Author;
        }

        internal User       Recipient   { get => this._Recipient;   }
        internal List<User> Recipients  { get => this._Recipients;  }
        internal int        NewMessages { get => this._NewMessages; }
        internal bool       IsGroup     { get => this._IsGroup;     }
        internal DateTime   LastOpened  { get => this._LastOpened;  }
        internal bool       IsRecent    { get => this._IsRecent; set => this._IsRecent = value; }
    }
}
