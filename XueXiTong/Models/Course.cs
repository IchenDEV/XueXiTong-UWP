using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Data.Json;
using XueXiTong.Util;

namespace XueXiTong.Models
{
    public class Course : Channel
    {
        public string ClassName { get; set; }
        public string Norder { get; set; }
        public string ClassId { get; set; }
        public int Roletype { get; set; }


        public static Course Paste(JsonObject json)
        {
            var content = json["content"].GetObject();
            var roletype = (int)content["roletype"].GetNumber();
            switch (roletype)
            {
                case 1:
                    return new TeachCourse(json);
                case 3:
                    return new LearnCourse(json);
            }
            return null;
        }

        public Course(JsonObject json)
        {
            var content = json["content"].GetObject();
            Roletype = (int)content["roletype"].GetNumber();
            Norder = json["norder"].GetNumber().ToString();
            ClassName = content["name"].GetString();
            ClassId = content["id"].GetNumber().ToString();

        }
    }
    public class TeachCourse : Course
    {
        public TeachCourse(JsonObject json) : base(json)
        {

        }
    }
    public class LearnCourse : Course
    {
        public string Techerfactor { get; set; }
        public string CourseName { get; set; }
        public string CourseId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsStart { get; set; }
        public bool IsFiled { get; set; }

        public async Task<List<ActiveTask>> GetActiveTasks()
        {

            var responseContent = await RequestTools.GetWithCookieAsync(String.Format(RequestApis.GetTaskActiveList, CourseId, ClassId));
            var json = JsonObject.Parse(responseContent);

            var actlist = json["activeList"].GetArray();

            List<ActiveTask> list = new List<ActiveTask>();
            foreach (var item in actlist)
                list.Add(JsonConvert.DeserializeObject<ActiveTask>(item.ToString()));


            return list;
        }

        public async Task<List<CourseFile>> GetResourseFiles(string rootId = "null")
        {

            var responseContent = await RequestTools.GetWithCookieAsync(String.Format(RequestApis.GetCourseFilesList, CourseId, ClassId, rootId));
            var json = JsonObject.Parse(responseContent);

            var actlist = json["data"].GetArray();

            List<CourseFile> list = new List<CourseFile>();
            foreach (var item in actlist)
                try
                {
                    list.Add(JsonConvert.DeserializeObject<CourseFile>(item.ToString()));
                }
                catch (Exception)
                {
                    var jo = JsonObject.Parse(item.GetObject()["content"].GetString());
                    item.GetObject().Remove("content");
                    item.GetObject().Add("content", jo);
                    list.Add(JsonConvert.DeserializeObject<CourseFile>(item.ToString()));
                }



            return list;
        }

        public async Task<List<Chapter>> GetChapters()
        {

            HttpClient httpClient = new HttpClient();
            if (State.CurrentUser == null) return null;
            httpClient.DefaultRequestHeaders.Add("Cookie", String.Join(';', State.CurrentUser.Cookie));
            var res = await httpClient.GetAsync(String.Format(RequestApis.GetChapters, base.ClassId, ""));
            var responseContent = await res.Content.ReadAsStringAsync();
            var json = JsonObject.Parse(responseContent);


            if (res.IsSuccessStatusCode)
            {
                return Chapter.PasteAsList(json);
            }
            else
            {
                return null;
            }
        }

        public LearnCourse(JsonObject json) : base(json)
        {

            try
            {
                var content = json["content"].GetObject();
                var info = content["course"].GetObject()["data"].GetArray()[0].GetObject();
                Techerfactor = info["teacherfactor"].GetString();
                CourseName = info["name"].GetString();

                IsStart = content["isstart"].GetBoolean();
                IsFiled = Convert.ToBoolean(content["isFiled"].GetNumber());
                CourseId = info["id"].GetNumber().ToString();

                ImageUrl = info["imageurl"].GetString();

            }
            catch (Exception)
            {

                if (ImageUrl == null || ImageUrl == "")
                {
                    ImageUrl = RequestApis.DefImg;
                }
            }

        }

    }
}
