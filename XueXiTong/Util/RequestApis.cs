using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XueXiTong.Util
{
    public static class RequestApis
    {
        public static string HTTPHost = "https://passport2.chaoxing.com";

        public static string UserAgent = "Dalvik/2.1.0 (Linux; U; Android 9; 16s Build/PKQ1.190202.001) com.chaoxing.mobile/ChaoXingStudy_3_4.4.6_android_phone_580_40 (@Kalimdor)_6afba0782a2d4325ab089c0ae9c56c13";

        public static string LoginRequestApi = "https://passport2-api.chaoxing.com/v11/loginregister?cx_xxt_passport=json";

        public static string LoginUrl = "https://passport2.chaoxing.com/login?fid=&newversion=true&refer=http%3A%2F%2Fi.chaoxing.com";

        public static string GetQRCodeAPi = "https://passport2.chaoxing.com/createqr?uuid=070590e940cd42d8b1c5cbd682286f86&fid=-1";

        public static string GetCourseAPi = "https://mooc1-api.chaoxing.com/mycourse/backclazzdata?view=json&mcode=";

        public static string GetQRAuthStatus = "https://passport2.chaoxing.com/getauthstatus";

        public static string GetObject = "http://d0.ananas.chaoxing.com/download/{0}";

        public static string GetChapters = "https://mooc1-api.chaoxing.com/gas/clazz?id={0}&personid={1}&fields=id,bbsid,classscore,isstart,allowdownload,chatid,name,state,isthirdaq,isfiled,information,discuss,visiblescore,begindate,coursesetting.fields(id,courseid,hiddencoursecover,hiddenwrongset),course.fields(id,name,infocontent,objectid,app,bulletformat,mappingcourseid,imageurl,teacherfactor,knowledge.fields(id,name,indexOrder,parentnodeid,status,layer,label,begintime,endtime,attachment.fields(id,type,objectid,extension).type(video)))&view=json";
        public static string GetKnowledge = "https://mooc1-api.chaoxing.com/gas/knowledge?id={0}&courseid={1}&fields=id,parentnodeid,indexorder,label,layer,name,begintime,createtime,lastmodifytime,status,jobUnfinishedCount,clickcount,openlock,card.fields(id,knowledgeid,title,knowledgeTitile,description,cardorder).contentcard(all)&view=json";

        public static string GetTaskActiveList = "https://mobilelearn.chaoxing.com/ppt/activeAPI/taskactivelist?courseId={0}&classId={1}";
        public static string Sign = "https://mobilelearn.chaoxing.com/pptSign/stuSignajax?activeId={0}&appType=15";

        public static string GetCourseFilesList = "https://mooc1-api.chaoxing.com/phone/data/student-datalist?courseId={0}&classId={1}&rootId={2}&pageNum=1";

        public static string DefImg = "http://mooc1-1.chaoxing.com/images/img_default.png";
    }
}
