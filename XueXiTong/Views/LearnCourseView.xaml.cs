using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using XueXiTong.Models;
using XueXiTong.Util;

namespace XueXiTong.Views
{
    public sealed partial class LearnCourseView : Page
    {
        public LearnCourseView()
        {
            this.InitializeComponent();
        }
        public static readonly DependencyProperty CourseProperty =
          DependencyProperty.Register("Course", typeof(LearnCourse), typeof(LearnCourseView), null);
        public LearnCourse Course
        {
            get { return (LearnCourse)GetValue(CourseProperty); }
            set { SetValue(CourseProperty, value); }
        }


        public ObservableCollection<ActiveTask> TaskList
        {
            get; set;
        } = new ObservableCollection<ActiveTask>();

        public ObservableCollection<CourseFile> FileList
        {
            get; set;
        } = new ObservableCollection<CourseFile>();

        public ObservableCollection<Chapter> ListItems
        {
            get; set;
        } = new ObservableCollection<Chapter>();

        private bool isUnload = false;
        public void LoadList(List<ActiveTask> list)
        {
            try
            {
                TaskList.Clear();
                foreach (var item in list)
                {
                    if (!isUnload)
                        TaskList.Add(item);
                }
            }
            catch (Exception)
            {
            }

        }

        public void LoadList2(List<Chapter> list)
        {
            try
            {
                if (!isUnload)
                    ListItems.Clear();
                foreach (var item in list)
                {
                    if (!isUnload)
                        ListItems.Add(item);
                }
            }
            catch (Exception)
            {
            }

        }

        public void LoadList3(List<CourseFile> list)
        {
            try
            {
                if (!isUnload)
                    FileList.Clear();
                foreach (var item in list)
                {
                    if (!isUnload)
                        FileList.Add(item);
                }
            }
            catch (Exception)
            {
            }

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                Course = e.Parameter as LearnCourse;
                ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("portrait");
                if (animation != null)
                {
                    animation.TryStart(Dest);
                }

            }
            catch (Exception)
            {

            }

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            isUnload = true;
            base.OnNavigatedFrom(e);
        }

        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var at = e.ClickedItem as ActiveTask;
            if (at.activeType == 2)
            {
                var res = await at.Sign();
                MessageDialog messageDialog = new MessageDialog(res);
                await messageDialog.ShowAsync();
            }
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            myTabButton.Content = (sender as MenuFlyoutItem).Text;
            pivot.SelectedIndex = Convert.ToInt16((sender as MenuFlyoutItem).Tag);
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TreeView_ItemInvoked(Microsoft.UI.Xaml.Controls.TreeView sender, Microsoft.UI.Xaml.Controls.TreeViewItemInvokedEventArgs args)
        {
            if (args.InvokedItem is Chapter)
            {
                var chap = args.InvokedItem as Chapter;
                if (chap.Children.Count == 0)
                {
                    MainPage.MainFrame.Navigate(typeof(ChapterView), chap);
                }
            }
        }



        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadList2(await Course.GetChapters());
            LoadList(await Course.GetActiveTasks());
            LoadList3(await Course.GetResourseFiles());
        }

        private async void FileClick(object sender, ItemClickEventArgs e)
        {
            var file = e.ClickedItem as CourseFile;
            if(file.cataid!= "100000017")
            {
                await Launcher.LaunchUriAsync(new Uri(String.Format(RequestApis.GetObject, file.content.objectId)));
            }
            else
            {
                LoadList3(await Course.GetResourseFiles(file.key));
            }
        }
    }
}
