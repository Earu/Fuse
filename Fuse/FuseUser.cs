using System.Collections.Generic;
using Fuse.Models;
using SteamKit2;

namespace Fuse
{
    internal class FuseUser
    {
        private User             _Localuser;
        private SteamFriends     _FriendsHandler;
        private List<User>       _Friends;
        private List<User>       _OnlineFriends;
        private List<User>       _OfflineFriends;
        private List<Discussion> _Discussions;
        private Discussion       _CurrentDiscussion;

        internal FuseUser(SteamFriends handler)
        {
            this._Friends = new List<User>();
            this._OnlineFriends = new List<User>();
            this._OfflineFriends = new List<User>();
            this._Discussions = new List<Discussion>();
            this._CurrentDiscussion = null;
            this._FriendsHandler = handler;
            this._Localuser = null;
        }
    
        internal User       LocalUser         { get => this._Localuser; }
        internal List<User> Friends           { get => this._Friends;        }
        internal List<User> OnlineFriends     { get => this._OnlineFriends;  }
        internal List<User> OfflineFriends    { get => this._OfflineFriends; }
        internal List<Discussion> Discussions { get => this._Discussions;    }
        internal Discussion CurrentDiscussion { get => this._CurrentDiscussion; set => this._CurrentDiscussion = value; }

        internal void UpdateFriends()
        {
            int total = this._FriendsHandler.GetFriendCount();
            for (int i = 0; i < total; i++)
            {
                SteamID id = this._FriendsHandler.GetFriendByIndex(i);
                this.UpdateFriend(id);
            }
        }

        internal void UpdateFriend(SteamID id)
        {
            if (this._FriendsHandler.GetFriendRelationship(id) != EFriendRelationship.Friend) return;

            string name = this._FriendsHandler.GetFriendPersonaName(id);
            uint accountid = id.AccountID;
            EPersonaState state = this._FriendsHandler.GetFriendPersonaState(id);
            byte[] bhash = _FriendsHandler.GetFriendAvatar(id);

            int findex = this.GetFriendIndex(accountid);
            if (findex == -1)
            {
                User _new = new User(name, id, state, bhash);
                this._Friends.Add(_new);
                if (state != EPersonaState.Offline)
                {
                    this._OnlineFriends.Add(_new);
                }
                else
                {
                    this._OfflineFriends.Add(_new);
                }
            }
            else
            {
                User old = this.Friends[findex];
                User friend = new User(name, id, state, bhash, old.Messages, old.NewMessages);
                this.Friends[findex] = friend;

                if (old.State == EPersonaState.Offline)
                {
                    int i = this.OfflineFriends.FindIndex(x => x.AccountID == old.AccountID);
                    if(i != -1) this._OfflineFriends.RemoveAt(i);
                }
                else
                {
                    int i = this.OnlineFriends.FindIndex(x => x.AccountID == old.AccountID);
                    if (i != -1) this._OnlineFriends.RemoveAt(i);
                }

                if (state != EPersonaState.Offline)
                {
                    this._OnlineFriends.Add(friend);
                }
                else
                {
                    this._OnlineFriends.Add(friend);
                }
            }
        }

        internal int GetFriendIndex(uint accountid)
        {
            return this._Friends.FindIndex(x => x.AccountID == accountid);
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
