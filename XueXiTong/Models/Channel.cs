using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace XueXiTong.Models
{
    public class Channel
    {
        public string cataid { get; set; }
        public string key { get; set; }
        public string cfid { get; set; }


        public static List<Channel> PasteAsList(JsonObject json)
        {
            List<Channel> channels = new List<Channel>();
            var channelList = json["channelList"].GetArray();
            foreach (var item in channelList)
                channels.Add(Channel.Paste(item.GetObject()));
            channels.RemoveAll(s => s == null);
            return channels;
        }

        public static Channel Paste(JsonObject item)
        {
            IJsonValue jv;
            var hasCatalog = item.GetObject().TryGetValue("catalogId", out jv);
            if (hasCatalog)
                return null;
            else
                return Course.Paste(item.GetObject());

        }
    }
}
