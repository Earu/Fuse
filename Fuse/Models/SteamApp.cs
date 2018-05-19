using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Fuse.Models
{
    [DataContract]
    internal class SteamApp
    {
        #pragma warning disable 0649
        [DataMember]
        internal int appid;

        [DataMember]
        internal string name;
        #pragma warning restore 0649
    }
}
