using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Fuse.Models;
using SteamKit2;

namespace Fuse
{
    internal class FuseUser
    {
        private SteamFriends _FriendHandler;
        private List<Friend> _Friends;
        private List<Discussion> _Discussions;
        private Discussion _CurrentDiscussion;

        internal FuseUser(SteamFriends handler)
        {
            this._Friends = new List<Friend>();
            this._FriendHandler = handler;
            this.UpdateFriends();
        }
    
        internal List<Friend> Friends { get => this._Friends; }

        internal void UpdateFriends()
        {
            this._Friends.Clear();
            int total = this._FriendHandler.GetFriendCount();
            for (int i = 0; i < total; i++)
            {
                SteamID id = this._FriendHandler.GetFriendByIndex(i);
                if (this._FriendHandler.GetFriendRelationship(id) == EFriendRelationship.Friend)
                {
                    string name = this._FriendHandler.GetFriendPersonaName(id);
                    string steamid = id.Render();
                    ulong id64 = id.ConvertToUInt64();
                    EPersonaState state = this._FriendHandler.GetFriendPersonaState(id);
                    byte[] bhash = _FriendHandler.GetFriendAvatar(id);
                    Friend friend = new Friend(name, steamid, id64, state, bhash);
                    this._Friends.Add(friend);
                }
            }
        }

        internal void AddFriend(Friend friend)
        {
            this._Friends.Add(friend);
        }

        internal void RemoveFriend(ulong id64)
        {
            Friend friend = this.GetFriend(id64);
            if (friend != null)
            {
                int index = this._Friends.IndexOf(friend);
                this._Friends.RemoveAt(index);
            }
        }

        internal Friend GetFriend(ulong id64)
        {
            return this._Friends.FirstOrDefault(x => x.SteamID64 == id64);
        }

        internal List<Friend> GetOnlineFriends()
        {
            return this._Friends
                .Where(x => x.State != EPersonaState.Offline)
                .ToList();
        }

        internal List<Friend> GetOfflineFriends()
        {
            return this._Friends
                .Where(x => x.State == EPersonaState.Offline)
                .ToList();
        }
    }
}
