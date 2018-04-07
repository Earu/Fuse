using System.Collections.Generic;
using Fuse.Models;
using SteamKit2;

namespace Fuse
{
    internal class FuseUser
    {
        private User                   _Localuser;
        private SteamFriends           _FriendsHandler;
        private Dictionary<uint,User>  _Friends;
        private Dictionary<uint, User> _OnlineFriends;
        private Dictionary<uint, User> _OfflineFriends;
        private List<Discussion>       _Discussions;
        private Discussion             _CurrentDiscussion;

        internal FuseUser(SteamFriends handler)
        {
            this._Friends = new Dictionary<uint, User>();
            this._OnlineFriends = new Dictionary<uint, User>();
            this._OfflineFriends = new Dictionary<uint, User>();
            this._Discussions = new List<Discussion>();
            this._CurrentDiscussion = null;
            this._FriendsHandler = handler;
            this._Localuser = null;
        }
    
        internal User                   LocalUser         { get => this._Localuser;      }
        internal Dictionary<uint, User> Friends           { get => this._Friends;        }
        internal Dictionary<uint, User> OnlineFriends     { get => this._OnlineFriends;  }
        internal Dictionary<uint, User> OfflineFriends    { get => this._OfflineFriends; }
        internal List<Discussion>       Discussions       { get => this._Discussions;    }
        internal Discussion             CurrentDiscussion { get => this._CurrentDiscussion; set => this._CurrentDiscussion = value; }

        internal void UpdateFriends()
        {
            int total = this._FriendsHandler.GetFriendCount();
            for (int i = 0; i < total; i++)
            {
                SteamID id = this._FriendsHandler.GetFriendByIndex(i);
                this.UpdateFriend(id);
            }
        }

        internal void UpdateFriend(SteamID id, string game=null)
        {
            if (this._FriendsHandler.GetFriendRelationship(id) != EFriendRelationship.Friend) return;

            string name = this._FriendsHandler.GetFriendPersonaName(id);
            uint accountid = id.AccountID;
            EPersonaState state = this._FriendsHandler.GetFriendPersonaState(id);
            byte[] bhash = _FriendsHandler.GetFriendAvatar(id);

            User old = this.GetFriend(accountid);
            if (old == null)
            {
                User _new = new User(name, id, state, bhash)
                {
                    Game = game
                };
                this._Friends[accountid] = _new;
                if (state != EPersonaState.Offline)
                    this._OnlineFriends[accountid] = _new;
                else
                    this._OfflineFriends[accountid] = _new;
            }
            else
            {
                User friend = new User(name, id, state, bhash, old.Messages, old.NewMessages)
                {
                    Game = game
                };
                this.Friends[accountid] = friend;

                if (old.State == EPersonaState.Offline)
                {
                    if (this._OfflineFriends.ContainsKey(old.AccountID))
                        this._OfflineFriends.Remove(old.AccountID);
                }
                else
                {
                    if (this._OnlineFriends.ContainsKey(old.AccountID))
                        this._OnlineFriends.Remove(old.AccountID);
                }

                if (state != EPersonaState.Offline)
                {
                    this._OnlineFriends[old.AccountID] = friend;
                }
                else
                {
                    this._OnlineFriends[old.AccountID] = friend;
                }
            }
        }

        internal User GetFriend(uint accountid)
        {
            return this._Friends.ContainsKey(accountid) ? this._Friends[accountid] : null;
        }

        internal void UpdateLocalUser(SteamID id)
        {
            string name = this._FriendsHandler.GetFriendPersonaName(id);
            ulong id64 = id.ConvertToUInt64();
            EPersonaState state = this._FriendsHandler.GetFriendPersonaState(id);
            byte[] bhash = this._FriendsHandler.GetFriendAvatar(id);
            User localuser = new User(name, id, state, bhash);
            this._Localuser = localuser;
        }
    }
}
