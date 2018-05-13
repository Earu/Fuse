using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Fuse.Utils
{
    internal class HTTP
    {
        private static string UserAgent = "fuse_steam";

        public static async Task<string> Fetch(FuseUI ui, string url, string useragent = null, Action<HttpWebRequest> callback = null)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = 1000 * 60;
                request.UserAgent = useragent ?? UserAgent;
                callback?.Invoke(request);

                using (WebResponse answer = await request.GetResponseAsync())
                using (StreamReader reader = new StreamReader(answer.GetResponseStream(), Encoding.UTF8))
                {
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
            catch (WebException e)
            {
                ui.ShowException($"There was an issue querying \"{url}\":\n {e.Message}");
                return "";
            }
        }
    }
}
