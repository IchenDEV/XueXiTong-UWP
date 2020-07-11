using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XueXiTong.Models
{

    public class CourseFile
    {
        public int cfid { get; set; }
        public int dataId { get; set; }
        public int orderId { get; set; }
        public int norder { get; set; }
        public string cataName { get; set; }
        public string cataid { get; set; }
        public string dataName { get; set; }
        public int forbidDownload { get; set; }
        public string key { get; set; }
        public Content content { get; set; }
    }

    public class Content
    {
        public string resTitle { get; set; }
        public string resUrl { get; set; }
        public string resLogo { get; set; }
        public string resUid { get; set; }
        public int toolbarType { get; set; }
        public string objectId { get; set; }
    }

}
