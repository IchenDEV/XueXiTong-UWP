using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using Windows.Data.Json;
using XueXiTong.Util;

namespace XueXiTong.Models
{

    public class attachment
    {
        public string extension { get; set; }
        public int id { get; set; }
        public string type { get; set; }
        public string objectid { get; set; }

        public attachment(JsonObject json)
        {
            extension = json["extension"].GetString();
            id = (int)json["id"].GetNumber();
            type = json["type"].GetString();
            objectid = json["objectid"].GetString();
        }
    }


    public class FrameObject
    {
        public string objectid { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public string hsize { get; set; }
        public string mid { get; set; }
        public long jobid { get; set; }
        public string fastforward { get; set; }
        public string switchwindow { get; set; }
    }

    public class Card
    {
        public int knowledgeid { get; set; }
        public int id { get; set; }
        public int cardorder { get; set; }
        public string title { get; set; }
        public string description { get; set; }


        public override string ToString()
        {
            if (title != "")
                return title;
            else
                return "未命名";
        }

        public Card(JsonObject json)
        {
            knowledgeid = (int)json["knowledgeid"].GetNumber();
            id = (int)json["id"].GetNumber();
            cardorder = (int)json["cardorder"].GetNumber();
            title = json["title"].GetString();
            description = json["description"].GetString();
        }
    }
    public class Chapter
    {
        public string Name { get; set; }
        public int CourseId { get; set; }
        public int id { get; set; }
        public string Label { get; set; }
        public int Parentnodeid { get; set; }
        public ObservableCollection<Chapter> Children { get; set; } = new ObservableCollection<Chapter>();
        public string Status { get; set; }
        public int Layer { get; set; }
        public List<attachment> Attachments { get; set; } = new List<attachment>();

        public List<Card> Cards { get; set; } = new List<Card>();

        private async void GetContent()
        {
            var responseContent = await RequestTools.GetWithCookieAsync(String.Format(RequestApis.GetKnowledge, id, CourseId));
            var json = JsonObject.Parse(responseContent);
            var cs = json["data"].GetArray()[0].GetObject()["card"].GetObject()["data"].GetArray();

            foreach (var item in cs)
                Cards.Add(new Card(item.GetObject()));

        }

        public static List<Chapter> PasteAsList(JsonObject json)
        {

            var list = new List<Chapter>();
            var cid = (int)json["data"].GetArray()[0].GetObject()["course"].GetObject()["data"].GetArray()[0].GetObject()["id"].GetNumber();
            var arr = json["data"].GetArray()[0].GetObject()["course"].GetObject()["data"].GetArray()[0].GetObject()["knowledge"].GetObject()["data"].GetArray();
            foreach (var item in arr)
            {
                var chapter = new Chapter(item.GetObject());
                chapter.CourseId = cid;
                list.Add(chapter);
                chapter.GetContent();
            }
            list = list.OrderBy(s => s.Layer).ToList();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].Parentnodeid != 0)
                {
                    var parent = list.Find(s => s.id.Equals(list[i].Parentnodeid));
                    parent.Children.Add(list[i]);
                    parent.Children = new ObservableCollection<Chapter>(parent.Children.OrderBy(s => s.Label).ToList());
                    list.Remove(list[i]);
                }
            }
            return list.OrderBy(s => Convert.ToInt32(s.Label)).ToList();
        }

        public Chapter(JsonObject json)
        {
            Name = json["name"].GetString();
            id = (int)json["id"].GetNumber();
            Label = json["label"].GetString();
            Layer = (int)json["layer"].GetNumber();
            Parentnodeid = (int)json["parentnodeid"].GetNumber();
            Status = json["status"].GetString();
            var atts = json["attachment"].GetObject()["data"].GetArray();
            foreach (var item in atts)
            {
                Attachments.Add(new attachment(item.GetObject()));
            }

        }

    }
}
