using System;
using System.Text;
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
        private string _LoginKey;

        internal FuseCredentials(SteamUser.LogOnDetails details)
        {
            this._Username = details.Username;
            this._LoginKey = details.LoginKey;
        }

        internal FuseCredentials()
        {
            this._Username = string.Empty;
            this._LoginKey = string.Empty;
        }

        internal string Username { get => this._Username; }
        internal string LoginKey { get => this._LoginKey; }

        private string ToBase64(string input)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }

        private string FromBase64(string input)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(input));
        }

        private void EncodeFields()
        {
            this._Username = this._Username != null ? this.ToBase64(this._Username) : null;
            this._LoginKey = this._LoginKey != null ? this.ToBase64(this._LoginKey) : null;
        }

        private void DecodeFields()
        {
            this._Username = this.FromBase64(this._Username);
            this._LoginKey = this.FromBase64(this._LoginKey);
        }

        internal void Save()
        {
            this.EncodeFields();
            string tosave = JsonConvert.SerializeObject(this);
            File.WriteAllText(".settings", tosave);
        }

        internal static bool TryLoad(out FuseCredentials creds)
        {
            if (File.Exists(".settings"))
            {
                string toload = File.ReadAllText(".settings");
                creds = JsonConvert.DeserializeObject<FuseCredentials>(toload);
                creds.DecodeFields();

                return true;
            }

            creds = null;
            return false;
        }
    }
}
