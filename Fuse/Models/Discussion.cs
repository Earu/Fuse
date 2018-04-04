using SteamKit2;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fuse.Models
{
    internal class Discussion
    {
        private FuseClient    _Client;
        private bool          _IsGroup;
        private User          _Recipient;
        private List<User>    _Recipients;
        private List<Message> _Messages;
        private int           _NewMessages;
        private DateTime      _LastOpened;
        private bool          _IsRecent;

        internal Discussion(FuseClient client, User recipient)
        {
            this._Client = client;
            this._IsGroup = false;
            this._Recipient = recipient;
            this._NewMessages = 0;
            this._LastOpened = DateTime.Now;
            this._IsRecent = true;
        }

        internal Discussion(FuseClient client, List<User> recipients)
        {
            this._Client = client;
            this._IsGroup = true;
            this._Recipients = recipients;
            this._Messages = new List<Message>();
            this._NewMessages = 0;
            this._LastOpened = DateTime.Now;
            this._IsRecent = true;
        }

        internal List<Message> Open()
        {
            this._LastOpened = DateTime.Now;
            this._NewMessages = 0;
            if (this._IsGroup) return this._Messages;
            else return this._Recipient.Messages;
        }

        internal void SendMessage(Message msg)
        {
            SteamFriends handler = this._Client.FriendsHandler;
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

        internal User     Recipient   { get => this._Recipient;   }
        internal int      NewMessages { get => this._NewMessages; }
        internal bool     IsGroup     { get => this._IsGroup;     }
        internal DateTime LastOpened  { get => this._LastOpened;  }
        internal bool     IsRecent    { get => this._IsRecent; set => this._IsRecent = value; }
    }
}
