using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XueXiTong.Util;

namespace XueXiTong.Models
{
    public enum activeType
    {

    }
    public class ActiveTask
    {
        public string nameTwo { get; set; }
        public int groupId { get; set; }
        public int isLook { get; set; }
        public int releaseNum { get; set; }
        public string url { get; set; }
        public string picUrl { get; set; }
        public int attendNum { get; set; }
        public int activeType { get; set; }
        public string nameOne { get; set; }
        public long startTime { get; set; }
        public int id { get; set; }
        public int status { get; set; }
        public string nameFour { get; set; }

        public async Task<string> Sign()
        {
            return await RequestTools.GetWithCookieAsync(String.Format(RequestApis.Sign, id));
        }
    }
}
