using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using SteamKit2;

namespace Fuse.Models
{
    [DataContract]
    internal class UsersSearchResult
    {
        // JSON fields
        [DataMember]
        internal int success;
        [DataMember]
        internal string search_text;
        [DataMember]
        internal string search_result_count;
        [DataMember]
        internal bool search_filter;
        [DataMember]
        internal int search_page;
        [DataMember]
        internal string html;

        internal static bool TryDeserialize(string json,out UsersSearchResult result)
        {
            try
            {
                result = JsonConvert.DeserializeObject<UsersSearchResult>(json);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        private string GetProperHtml()
        {
            string html = this.html.Replace("\\n",string.Empty)
                .Replace("\\r", string.Empty)
                .Replace("\\t", string.Empty)
                .Replace("\\", string.Empty);
            return $"<html><head></head><body>{html}</body></html>";
        }

        internal List<User> GetResults()
        {
            List<User> results = new List<User>();
            string html = this.GetProperHtml();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            HtmlNodeCollection collection = doc.DocumentNode.SelectNodes("//div[@class='search_row']");
            foreach (HtmlNode node in collection)
            {
                string name = node.SelectSingleNode("div[2]/a[@class='searchPersonaName']").InnerText;
                string link = node.SelectSingleNode("div/div/a/img").GetAttributeValue("src", null);
                uint accid = uint.Parse(node.SelectSingleNode("div").GetAttributeValue("data-miniprofile","0"));
                SteamID id = new SteamID(accid, EUniverse.Public, EAccountType.Chat);
                User u = new User(name, id, EPersonaState.Offline, null);
                u.SetAvatarLink(link);

                results.Add(u);
            }

            return results;
        }
    }
}
