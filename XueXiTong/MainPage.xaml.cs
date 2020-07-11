using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
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
using XueXiTong.Views;

namespace XueXiTong
{
    public sealed partial class MainPage : Page
    {

        public static Frame MainFrame = null;
        public static UIElement AppBar;
        public MainPage()
        {
            this.InitializeComponent();
            MainFrame = HomeFrame;
            Window.Current.SetTitleBar(Appbar);
            AppBar = Appbar;
            if (State.CurrentUser == null)
                HomeFrame.Navigate(typeof(LoginView));
            else
                HomeFrame.Navigate(typeof(ChanelView));
        }

        private void NvSample_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {

            var tag = (args.SelectedItem as Microsoft.UI.Xaml.Controls.NavigationViewItem)?.Tag?.ToString();
            if (tag != null)
            {
                switch (tag)
                {
                    case "Channels":
                        HomeFrame.Navigate(typeof(ChanelView),null, new DrillInNavigationTransitionInfo());
                        break;
                    default:
                        break;
                }
            }
        }

        private void NvSample_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            if (MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
            }
        }

        private async void NvSample_PaneOpened(Microsoft.UI.Xaml.Controls.NavigationView sender, object args)
        {
            await UserPictureStoryboard.BeginAsync();
     
           
        }

        private async void NvSample_PaneClosed(Microsoft.UI.Xaml.Controls.NavigationView sender, object args)
        {
            await UserPictureStoryboard2.BeginAsync();


        }
    }
}
