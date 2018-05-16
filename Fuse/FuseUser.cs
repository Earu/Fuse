using System.Collections.Generic;
using System.Threading.Tasks;
using Fuse.Models;
using SteamKit2;

namespace Fuse
{
    internal class FuseUser
    {
        private User                         _Localuser;
        private SteamFriends                 _FriendsHandler;
        private Dictionary<uint,User>        _Friends;
        private Dictionary<uint, User>       _OnlineFriends;
        private Dictionary<uint, User>       _OfflineFriends;
        private Dictionary<uint, User>       _Requesteds;
        private Dictionary<uint, Discussion> _Discussions;
        private Discussion                   _CurrentDiscussion;

        internal FuseUser(SteamFriends handler)
        {
            this._Friends = new Dictionary<uint, User>();
            this._OnlineFriends = new Dictionary<uint, User>();
            this._OfflineFriends = new Dictionary<uint, User>();
            this._Requesteds = new Dictionary<uint, User>();
            this._Discussions = new Dictionary<uint, Discussion>();
            this._CurrentDiscussion = null;
            this._FriendsHandler = handler;
            this._Localuser = null;
        }
    
        internal User                         LocalUser         { get => this._Localuser;      }
        internal Dictionary<uint, User>       Friends           { get => this._Friends;        }
        internal Dictionary<uint, User>       OnlineFriends     { get => this._OnlineFriends;  }
        internal Dictionary<uint, User>       OfflineFriends    { get => this._OfflineFriends; }
        internal Dictionary<uint, User>       Requesteds        { get => this._Requesteds;     }
        internal Dictionary<uint, Discussion> Discussions       { get => this._Discussions;    }
        internal Discussion                   CurrentDiscussion { get => this._CurrentDiscussion; set => this._CurrentDiscussion = value; }

        internal void UpdateFriends()
        {
            int total = this._FriendsHandler.GetFriendCount();
            Parallel.For(0, total, i =>
            {
                SteamID id = this._FriendsHandler.GetFriendByIndex(i);
                this.UpdateFriend(id);
            });
        }

        private void HandleOthers(User other, EFriendRelationship relation)
        {
            switch (relation)
            {
                case EFriendRelationship.RequestRecipient:
                    this._Requesteds[other.AccountID] = other;
                    break;
                default:
                    break;

            }
        }

        internal void AcceptUser(User user)
        {
            this._FriendsHandler.AddFriend(user.SteamID);
            this.Requesteds.Remove(user.AccountID);
        }

        internal void RefuseUser(User user)
        {
            this._FriendsHandler.IgnoreFriend(user.SteamID);
            this.Requesteds.Remove(user.AccountID);
        }

        internal void UpdateFriend(SteamID id,string name=null,EPersonaState state=EPersonaState.Offline,byte[] bhash=null,string game=null)
        {
            EFriendRelationship relation = this._FriendsHandler.GetFriendRelationship(id);
            name = name ?? this._FriendsHandler.GetFriendPersonaName(id);
            uint accountid = id.AccountID;
            state = state == EPersonaState.Offline ? this._FriendsHandler.GetFriendPersonaState(id) : state;
            bhash = bhash ?? _FriendsHandler.GetFriendAvatar(id);

            User old = this.GetFriend(accountid);
            User friend;
            if (old == null)
            {
                friend = new User(name, id, state, bhash);
                friend.Game = game;
                this._Friends[accountid] = friend;
                if (relation == EFriendRelationship.Friend)
                {
                    if (state != EPersonaState.Offline)
                        this._OnlineFriends[accountid] = friend;
                    else
                        this._OfflineFriends[accountid] = friend;
                }
            }
            else
            {
                friend = new User(name, id, state, bhash, old.Messages, old.NewMessages);
                friend.Game = game;
                this.Friends[accountid] = friend;

                if (relation == EFriendRelationship.Friend)
                {
                    if (old.State == EPersonaState.Offline)
                        if (this._OfflineFriends.ContainsKey(old.AccountID))
                            this._OfflineFriends.Remove(old.AccountID);
                    else
                        if (this._OnlineFriends.ContainsKey(old.AccountID))
                            this._OnlineFriends.Remove(old.AccountID);

                    if (state != EPersonaState.Offline)
                        this._OnlineFriends[old.AccountID] = friend;
                    else
                        this._OfflineFriends[old.AccountID] = friend;
                }
            }

            if (relation != EFriendRelationship.Friend)
                this.HandleOthers(friend, relation);
        }

        internal void UpdateDiscussion(uint accountid,Discussion disc)
        {
            Discussion _new = new Discussion(disc);
            this._Discussions[accountid] = _new;
        }

        internal User GetFriend(uint accountid)
        {
            return this._Friends.ContainsKey(accountid) ? this._Friends[accountid] : null;
        }

        internal Discussion GetDiscussion(uint accountid)
        {
            return this._Discussions.ContainsKey(accountid) ? this._Discussions[accountid] : null;
        }

        internal void UpdateLocalUser(SteamID id,string name=null,EPersonaState state=EPersonaState.Offline,byte[] bhash=null,string game=null)
        {
            name = name ?? this._FriendsHandler.GetFriendPersonaName(id);
            ulong id64 = id.ConvertToUInt64();
            state = state == EPersonaState.Offline ? this._FriendsHandler.GetFriendPersonaState(id) : state;
            bhash = bhash ?? this._FriendsHandler.GetFriendAvatar(id);
            User localuser = new User(name, id, state, bhash);
            localuser.Game = game;
            this._Localuser = localuser;
        }
    }
}
