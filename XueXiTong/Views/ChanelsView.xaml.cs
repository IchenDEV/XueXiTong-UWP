using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using XueXiTong.Models;

namespace XueXiTong.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChanelView : Page
    {
        public ChanelView()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty ListItemsProperty =
           DependencyProperty.Register("ListItems", typeof(ObservableCollection<Channel>), typeof(ChanelView), new PropertyMetadata(new ObservableCollection<Channel>()));
        public ObservableCollection<Channel> ListItems
        {
            get { return (ObservableCollection<Channel>)GetValue(ListItemsProperty); }
            set { SetValue(ListItemsProperty, value); }
        }

        public void LoadList(List<Channel> list)
        {
            try
            {
                ListItems.Clear();
                foreach (var item in list)
                {
                    ListItems.Add(item);
                }
            }
            catch (Exception)
            {

            }

        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (State.CurrentUser != null)
            {
             
                var list = await State.CurrentUser?.GetLearnCourse();
                LoadList(list);
            }
            else
            {
                MainPage.MainFrame.Navigate(typeof(LoginView));
            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var ck = e.ClickedItem as Channel;
            if (ck is LearnCourse)
            {
                var learn = ck as LearnCourse;
                gv.PrepareConnectedAnimation("portrait", learn, "SourceImage");
                MainPage.MainFrame.Navigate(typeof(LearnCourseView), learn);

            }
        }


    }
}
