using Fuse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuse.Datas
{
    internal class DiscussionData
    {
        internal bool              IsGroup     { get; set; }
        internal UserData          Recipient   { get; set; }
        internal List<UserData>    Recipients  { get; set; }
        internal List<MessageData> Messages    { get; set; }
        internal int               NewMessages { get; set; }
        internal DateTime          LastOpened  { get; set; }
        internal bool              IsRecent    { get; set; }

        internal Discussion ToDiscussion()
        {
            /*if (this.IsGroup)
            {
                Discussion disc = new Discussion();
            }
            else
            {
            }*/
            return null;
        }
    }
}
