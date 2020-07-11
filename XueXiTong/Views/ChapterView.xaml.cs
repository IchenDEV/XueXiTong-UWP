using System;
using Windows.Data.Json;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using XueXiTong.Models;
using XueXiTong.Util;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace XueXiTong.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChapterView : Page
    {
        public ChapterView()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty ChapterProperty =
         DependencyProperty.Register("Chapter", typeof(Chapter), typeof(LearnCourseView), null);
        public Chapter Chapter
        {
            get { return (Chapter)GetValue(ChapterProperty); }
            set { SetValue(ChapterProperty, value); }
        }



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
               
                if (e.Parameter is Chapter)
                {
                    Chapter = e.Parameter as Chapter;
                }
            }
            catch (Exception)
            {

         
            }
            
            base.OnNavigatedTo(e);
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Window.Current.SetTitleBar(MainPage.AppBar);
            base.OnNavigatedFrom(e);
        }
        private void TabView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                var text = Chapter.Cards[tb.SelectedIndex].description;
                text = text.Replace("iframe", "button").Replace("</button>", "附件</button>");
               
                text += "<script>Array.from(document.getElementsByTagName('button')).forEach(function(e, i){e.onclick=(s)=>{ window.external.notify(s.target.outerHTML)}});</script>";
                text += "<style>.ans-attach-online{background-color: #4CAF50; border: none;color: white;padding: 15px 32px;text-align: center;text-decoration: none; display:table; font-size: 16px;margin:auto;}</style>";
                wb.NavigateToString(text);
                wb.ScriptNotify += Wb_ScriptNotify;


            }
            catch (Exception)
            {


            }

        }

        private void Wb_ScriptNotify(object sender, NotifyEventArgs e)
        {
            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(e.Value);
            if(htmlDocument.DocumentNode.FirstChild.Attributes["module"].Value== "insertvideo")
            {
                var oid = JsonObject.Parse(htmlDocument.DocumentNode.FirstChild.Attributes["data"].Value)["objectid"].GetString();
                MainPage.MainFrame.Navigate(typeof(VideoViews), String.Format(RequestApis.GetObject, oid));
            }


        }
    }
}
