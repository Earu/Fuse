using System.Collections.Generic;
using Fuse.Models;
using SteamKit2;

namespace Fuse
{
    internal class FuseUser
    {
        private SteamFriends     _FriendHandler;
        private List<Friend>     _Friends;
        private List<Friend>     _OnlineFriends;
        private List<Friend>     _OfflineFriends;
        private List<Discussion> _Discussions;
        private Discussion       _CurrentDiscussion;

        internal FuseUser(SteamFriends handler)
        {
            this._Friends = new List<Friend>();
            this._OnlineFriends = new List<Friend>();
            this._OfflineFriends = new List<Friend>();
            this._Discussions = new List<Discussion>();
            this._FriendHandler = handler;
            this.UpdateFriends();
        }
    
        internal List<Friend> Friends        { get => this._Friends;        }
        internal List<Friend> OnlineFriends  { get => this._OnlineFriends;  }
        internal List<Friend> OfflineFriends { get => this._OfflineFriends; }

        internal void UpdateFriends()
        {
            int total = this._FriendHandler.GetFriendCount();
            for (int i = 0; i < total; i++)
            {
                SteamID id = this._FriendHandler.GetFriendByIndex(i);
                this.UpdateFriend(id);
            }
        }

        internal void UpdateFriend(SteamID id)
        {
            if (this._FriendHandler.GetFriendRelationship(id) != EFriendRelationship.Friend) return;

            string name = this._FriendHandler.GetFriendPersonaName(id);
            string steamid = id.Render();
            ulong id64 = id.ConvertToUInt64();
            EPersonaState state = this._FriendHandler.GetFriendPersonaState(id);
            byte[] bhash = _FriendHandler.GetFriendAvatar(id);

            int findex = this.GetFriendIndex(id64);
            if (findex == -1)
            {
                Friend _new = new Friend(name, steamid, id64, state, bhash);
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
                Friend friend    = this.Friends[findex];
                friend.Name      = name;
                friend.SteamID   = steamid;
                friend.SteamID64 = id64;
                friend.SetAvatarHash(bhash);

                if (friend.State != state)
                {
                    if (friend.State == EPersonaState.Offline)
                    {
                        int i = this.OfflineFriends.IndexOf(friend);
                        this._OfflineFriends.RemoveAt(i);
                    }
                    else
                    {
                        int i = this.OnlineFriends.IndexOf(friend);
                        this._OnlineFriends.RemoveAt(i);
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

                friend.State = state;
                this.Friends[findex] = friend;
            }
        }

        internal int GetFriendIndex(ulong id64)
        {
            for (int i = 0; i < this._Friends.Count; i++)
            {
                Friend f = this._Friends[i];
                if (f.SteamID64 == id64) return i;
            }

            return -1;
        }
    }
}
