using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using XueXiTong.Models;
using XueXiTong.Models.Args;

namespace XueXiTong.Controls
{
    public sealed partial class UserLoginControl : UserControl
    {
        public UserLoginControl()
        {
            this.InitializeComponent();
        }
        //定义委托
        public delegate void LoginSuccessHandle(object sender, LoginEventArgs e);
        //定义事件
        public event LoginSuccessHandle LoginSucceed;


        //定义委托
        public delegate void LoginFailHandle(object sender, LoginEventArgs e);
        //定义事件
        public event LoginFailHandle LoginFailed;

        private LoginQRCode qRCode;

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = await Models.User.Login(PhoneTextBox.Text, CodePasswordBox.Password);
                if (user != null)
                {
                    LoginSucceed?.Invoke(this, new LoginEventArgs() { user = user, isSuccess = true });//把按钮自身作为参数传递
                }
                else
                {
                    LoginFailed?.Invoke(this, new LoginEventArgs() { isSuccess = false });
                }
            }
            catch (Exception excep)
            {

                LoginFailed?.Invoke(this, new LoginEventArgs() { isSuccess = false, message = excep.Message });
            }


        }
        DispatcherTimer timer = new DispatcherTimer();
        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            QRPanel.Visibility = Visibility.Visible;
            PassPanel.Visibility = Visibility.Collapsed;
            qRCode = new LoginQRCode();
            await UpdateQRCode();

        }

        private async Task UpdateQRCode()
        {
         
            bool isOK = await qRCode.GetNewQRCode();
            if (isOK && qRCode.quickCode != null)
            {
                QRImage.Source = new BitmapImage(new Uri(qRCode.quickCode));
                timer.Stop();
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Start();
                timer.Tick += Timer_Tick;
            }
        }

        private async void Timer_Tick(object sender, object e)
        {
            var Mysender = sender as DispatcherTimer;
            var sta = await qRCode.GetAuthstatus();
            if (Mysender == null || sta != null)
            {
                Mysender.Stop();
                LoginSucceed?.Invoke(this, new LoginEventArgs() { user = sta });//把按钮自身作为参数传递
            }
        }

        private void HyperlinkButton2_Click(object sender, RoutedEventArgs e)
        {
            QRPanel.Visibility = Visibility.Collapsed;
            PassPanel.Visibility = Visibility.Visible;
        }

        private void TextBlock_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                LoginButton_Click(sender, new RoutedEventArgs());
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (qRCode != null)
            {
                await UpdateQRCode();
            }
          
        }
    }
}
