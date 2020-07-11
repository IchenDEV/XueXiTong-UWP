using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace XueXiTong.Views
{
    public sealed partial class LoginView : Page
    {
        public LoginView()
        {
            this.InitializeComponent();
        }

        private void UserLoginControl_LoginSucceed(object sender, Models.Args.LoginEventArgs e)
        {
            State.CurrentUser = e.user;
            MainPage.MainFrame.Navigate(typeof(ChanelView));
        }

        private async void UserLoginControl_LoginFailed(object sender, Models.Args.LoginEventArgs e)
        {
            MessageDialog messageDialog = new MessageDialog(e.message, "登录失败");
            await messageDialog.ShowAsync();
        }
    }
}
