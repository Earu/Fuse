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
                Friend old = this.Friends[findex];
                Friend friend = new Friend(name, steamid, id64,state,bhash,old.Messages);
                this.Friends[findex] = friend;

                if (old.State == EPersonaState.Offline)
                {
                    int i = this.OfflineFriends.FindIndex(x => x.SteamID64 == old.SteamID64);
                    if(i != -1) this._OfflineFriends.RemoveAt(i);
                }
                else
                {
                    int i = this.OnlineFriends.FindIndex(x => x.SteamID64 == old.SteamID64);
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

        internal int GetFriendIndex(ulong id64)
        {
            return this._Friends.FindIndex(x => x.SteamID64 == id64);
        }
    }
}
