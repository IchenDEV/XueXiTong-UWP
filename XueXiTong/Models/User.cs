using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Data.Json;
using XueXiTong.Util;

namespace XueXiTong.Models
{
    public class User
    {

        public List<string> Cookie;

        public User(List<string> cookie)
        {
            Cookie = cookie;
        }

        public async Task<List<Channel>> GetChannels()
        {

            var responseContent = await RequestTools.GetWithCookieAsync(RequestApis.GetCourseAPi);
            var json = JsonObject.Parse(responseContent);

            var status = (int)json["result"].GetNumber();
            if (status == 1)
            {
                return Channel.PasteAsList(json);
            }
            else
            {
                return null;
            }
        }


        public async Task<List<Channel>> GetLearnCourse()
        {
            var AllChannels = await GetChannels();
            for (int i = 0; i < AllChannels.Count; i++)
            {
                if (!(AllChannels[i] is LearnCourse))
                {
                    AllChannels.RemoveAt(i);i--;
                }
            }
            return AllChannels;
        }

        public static async Task<User> Login(string uname, string code)
        {

            HttpClient httpClient = new HttpClient();
            List<KeyValuePair<string, string>> contents = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string,string>( "uname", uname),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("loginType", "1"),
                new KeyValuePair<string, string>("roleSelect", "1")
            };

            FormUrlEncodedContent urlEncodedContent = new FormUrlEncodedContent(contents);

            var res = await httpClient.PostAsync(new Uri(RequestApis.LoginRequestApi), urlEncodedContent);
            var responseContent = await res.Content.ReadAsStringAsync();
            var json = JsonObject.Parse(responseContent);
            var status = json["status"].GetBoolean();

            if (res.IsSuccessStatusCode && status)
            {
                var Cookie = res.Headers.GetValues("Set-Cookie").ToList();
                for (int i = 0; i < Cookie.Count; i++)
                    Cookie[i] = Cookie[i].Split(';')[0];
                User user = new User(Cookie);
                return user;
            }
            else
            {
                var message = json["mes"].GetString();
                if (message != null)
                {
                    throw new Exception(message);
                }
                throw new Exception("网络错误");
            }
        }

    }
}
