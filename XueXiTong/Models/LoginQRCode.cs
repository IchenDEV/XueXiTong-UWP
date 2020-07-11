using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using XueXiTong.Util;

namespace XueXiTong.Models
{
    public class LoginQRCode
    {
        public string uuid;
        public string fid = "-1";
        public string enc;
        public string quickCode;

        public async Task<bool> GetNewQRCode()
        {
            HttpClient httpClient = new HttpClient();
            var loginPage = await httpClient.GetAsync(RequestApis.LoginUrl);
            if (loginPage.IsSuccessStatusCode)
            {
                using (var htmlContent = await loginPage.Content.ReadAsStreamAsync())
                {
                    HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                    html.Load(htmlContent);
                    enc = html.GetElementbyId("enc")?.Attributes["value"]?.Value;
                    uuid = html.GetElementbyId("uuid")?.Attributes["value"]?.Value;
                    quickCode = RequestApis.HTTPHost + html.GetElementbyId("quickCode")?.Attributes["src"]?.Value;
                }

                if (quickCode == null || enc == null || uuid == null)
                {
                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }

        }

        public async Task<User> GetAuthstatus()
        {
            if (enc == null || uuid == null) return null;
            HttpClient httpClient = new HttpClient();
            List<KeyValuePair<string, string>> contents = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string,string>( "enc", enc),
                new KeyValuePair<string, string>("uuid", uuid),
            };

            FormUrlEncodedContent urlEncodedContent = new FormUrlEncodedContent(contents);
            var res = await httpClient.PostAsync(new Uri(RequestApis.GetQRAuthStatus), urlEncodedContent);
            var responseContent = await res.Content.ReadAsStringAsync();
            var json = JsonObject.Parse(responseContent);
            var status = json["status"].GetBoolean();
            Debug.WriteLine(responseContent);
            if (status)
            {
                var Cookie = res.Headers.GetValues("Set-Cookie").ToList();
                for (int i = 0; i < Cookie.Count; i++)
                    Cookie[i] = Cookie[i].Split(';')[0];
                User user = new User(Cookie);
                return user;
            }

            return null;

        }
    }
}
