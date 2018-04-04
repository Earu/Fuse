using Fuse.Models;
using SteamKit2;
using System.Collections.Generic;
using System.Linq;

namespace Fuse.Datas
{
    internal class UserData
    {
        internal string            Name        { get; set; }
        internal List<MessageData> Messages    { get; set; }
        internal uint              AccountID   { get; set; }
        internal ulong             SteamID64   { get; set; }
        internal int               NewMessages { get; set; }

        internal User ToUser()
        {
            SteamID id = new SteamID(this.AccountID,EUniverse.Public,EAccountType.Chat);
            User usr = new User(this.Name, id, EPersonaState.Offline, new byte[] { },null,this.NewMessages);
            List<Message> msgs = this.Messages.Select(x => x.ToMessage(usr)).ToList();
            usr.Messages = msgs;

            return usr;
        }
    }
}
