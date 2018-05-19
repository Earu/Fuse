using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Fuse.Models
{
    [DataContract]
    internal class StoredApps
    {
        #pragma warning disable 0649
        [DataMember]
        internal SteamAppList applist;
        #pragma warning restore 0649

        internal SteamApp GetApp(uint id)
        {
            return this.applist.apps.FirstOrDefault(x => x.appid == id);
        }
    }
}
