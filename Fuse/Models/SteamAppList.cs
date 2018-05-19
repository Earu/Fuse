using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Fuse.Models
{
    [DataContract]
    internal class SteamAppList
    {
        #pragma warning disable 0649
        [DataMember]
        internal SteamApp[] apps;
        #pragma warning restore 0649
    }
}
