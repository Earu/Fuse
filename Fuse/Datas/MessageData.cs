using Fuse.Models;
using System;

namespace Fuse.Datas
{
    internal class MessageData
    {
        internal UserData Author    { get; set; }
        internal DateTime TimeStamp { get; set; }
        internal string   Content   { get; set; }

        internal Message ToMessage(User usr)
        {
            Message msg = new Message(usr, this.Content,this.TimeStamp);
            return msg;
        }
    }
}
