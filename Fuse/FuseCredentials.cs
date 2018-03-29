using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization;

namespace Fuse
{
    [DataContract]
    internal class FuseCredentials
    {
        [DataMember]
        private string _Username;
        [DataMember]
        private string _Password;
        [DataMember]
        private string _LoginKey;
        [DataMember]
        private uint? _LoginID;

        internal FuseCredentials(SteamUser.LogOnDetails details)
        {
            this._Username = details.Username;
            this._Password = details.Password;
            this._LoginKey = details.LoginKey;
            this._LoginID = details.LoginID;
        }

        internal string Username { get => this._Username; }
        internal string Password { get => this._Password; }
        internal string LoginKey { get => this._LoginKey; }
        internal uint? LoginID   { get => this._LoginID;  }

        internal void Save()
        {
            string tosave = JsonConvert.SerializeObject(this);
            tosave = Convert.ToBase64String(Encoding.UTF8.GetBytes(tosave));
            File.AppendAllText(".settings", tosave);
        }

        internal static bool TryLoad(out FuseCredentials creds)
        {
            if (File.Exists(".settings"))
            {
                string toload = File.ReadAllText(".settings");
                toload = Encoding.UTF8.GetString(Convert.FromBase64String(toload));
                creds = JsonConvert.DeserializeObject<FuseCredentials>(toload);

                return true;
            }

            creds = null;
            return false;
        }
    }
}
