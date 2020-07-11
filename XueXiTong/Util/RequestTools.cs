using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace XueXiTong.Util
{
    public class RequestTools
    {
        public static async Task<string> GetWithCookieAsync(string Uri)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Cookie", string.Join(';', State.CurrentUser.Cookie));
            var res = await httpClient.GetAsync(Uri);
            var responseContent = await res.Content.ReadAsStringAsync();

            if (responseContent == "")
            {
                State.CurrentUser = null;
            }
            return responseContent;
        } 
    }
}
