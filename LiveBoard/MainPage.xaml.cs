using LiveBoard.Class;
using LiveBoard.ProtocolUtility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace LiveBoard
{
    public sealed partial class MainPage :Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            WaitRing.IsActive = true;
            UsernameTextBox.IsEnabled = false;
            LoginPasswordBox.IsEnabled = false;
            LoginButton.IsEnabled = false;
            RegisterButton.IsEnabled = false;
            try
            {
                while (!App.isConnected && !App.isConnectError)
                {
                    CProtocolEngine engine = new CProtocolEngine();
                    engine.Request(SchemaDef.CONNECT, null, null);
                    engine.ReceiveData(SchemaDef.CONNECT);
                    await Task.Delay(100);
                }
                if (!App.isConnected && App.isConnectError)
                {
                    App.ShowMessage("无法连接到服务器", "Error");
                    App.isConnectError = false;
                    //ReconnectButton.Visibility = Visibility.Visible;
                    ReconnectButton.IsEnabled = true;
                }
                else
                    ReconnectButton.Visibility = Visibility.Collapsed;
            }
            catch
            {
                App.ShowMessage("请求错误", "Error");
            }
            WaitRing.IsActive = false;
            UsernameTextBox.IsEnabled = true;
            LoginPasswordBox.IsEnabled = true;
            LoginButton.IsEnabled = true;
            RegisterButton.IsEnabled = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Storyboard story = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 0.3;
            animation.To = 1;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.Opacity)").Path);
            story.Children.Add(animation);
            story.Begin();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            WaitRing.IsActive = true;
            App.currentUser.Username = UsernameTextBox.Text;
            App.currentUser.Password = LoginPasswordBox.Password;
            try
            {
                if (App.isConnected)
                {
                    CProtocolEngine engine = new CProtocolEngine();
                    engine.Request(SchemaDef.LOGIN, App.currentUser, null);
                    engine.ReceiveData(SchemaDef.LOGIN);
                    bool isLoop = true;
                    while (isLoop)
                    {
                        await Task.Delay(100);
                        if (App.responseResult.command == "login" && App.responseResult.code == 1)
                        {
                            isLoop = false;
                            WaitRing.IsActive = false;
                            engine.Request(SchemaDef.ROOMLIST, App.currentUser, null);
                            engine.ReceiveData(SchemaDef.ROOMLIST);
                            await Task.Delay(300);
                            Frame.Navigate(typeof(RoomListPage));
                        }
                        if (App.responseResult.command == "login" && App.responseResult.code == 0)
                        {
                            isLoop = false;
                            WaitRing.IsActive = false;
                            App.ShowMessage(App.responseResult.tips, "Error");
                        }
                    }
                }
                else
                {
                    App.ShowMessage("未连接到服务器，请检查网络", "Warning");
                    WaitRing.IsActive = false;
                }
            }
            catch(Exception exception)
            {
                App.ShowMessage(exception.Message, "Warning");
            }           
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterPopup.IsOpen = true;
        }

        private void ReconnectButton_Click(object sender, RoutedEventArgs e)
        {
            App.isConnectError = false;
            ReconnectButton.IsEnabled = false;
            Page_Loaded(sender,e);
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage), "hello");
        }

        #region UsernameTextBox
        private void UsernameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UsernameTextBox.Text == "Username")
                UsernameTextBox.Select(0,0);
        }

        private void UsernameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (UsernameTextBox.Text == "")
                UsernameTextBox.Text = "Username";
        }

        private void UsernameTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (UsernameTextBox.Text == "Username")
                UsernameTextBox.Text = "";
        }

        private void UsernameTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
                LoginPasswordBox.Focus(FocusState.Programmatic);
        }
        #endregion

        #region PasswordBox
        private void LoginPasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (LoginPasswordBox.Password == "")
                LoginPasswordBox.Password = "Password";
        }

        private void LoginPasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (LoginPasswordBox.Password == "Password")
                LoginPasswordBox.Password = "";
        }

        private void LoginPasswordBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (LoginPasswordBox.Password == "Password")
                LoginPasswordBox.Password = "";
        }

        private void LoginPasswordBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
                LoginButton_Click(sender, e);
        }
        #endregion

        #region Register Popup
        private async void PopupConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.isConnected)
            {
                if (PopupUsernameTextBox.Text == "" || PopupPasswordBox.Password == "" || PopupNicknameTextBox.Text == "")
                {
                    App.ShowMessage("用户名、密码与昵称不能为空，请改正", "Warning");
                }
                else if (PopupPasswordBox.Password != PopupPasswordConfirmBox.Password)
                {
                    App.ShowMessage("两次密码输入不一致，请改正", "Warning");
                }
                else
                {
                    User tempUser = new User();
                    tempUser.Username = PopupUsernameTextBox.Text;
                    tempUser.Password = PopupPasswordBox.Password;
                    tempUser.Nickname = PopupNicknameTextBox.Text;

                    try
                    {
                        CProtocolEngine engine = new CProtocolEngine();
                        engine.Request(SchemaDef.REGISTER, tempUser, null);
                        engine.ReceiveData(SchemaDef.REGISTER);
                        bool isLoop = true;
                        while(isLoop)
                        {
                            await Task.Delay(100);
                            if (App.responseResult.command == "register" && App.responseResult.code == 1)
                            {
                                isLoop = false;
                                App.ShowMessage("Register Succeed！", "Tips");
                                RegisterPopup.IsOpen = false;
                                UsernameTextBox.Text = PopupUsernameTextBox.Text;
                                LoginPasswordBox.Password = PopupPasswordBox.Password;
                            }
                            if (App.responseResult.command == "register" && App.responseResult.code == 0)
                            {
                                isLoop = false;
                                App.ShowMessage(App.responseResult.tips, "Error");
                            }
                        }
                    }
                    catch
                    {
                        App.ShowMessage("请求错误", "Error");
                    }
                }
            }
            else
                App.ShowMessage("未连接到服务器，请检查网络", "Warning");
        }

        private void PopupResetButton_Click(object sender, RoutedEventArgs e)
        {
            PopupUsernameTextBox.Text = "";
            PopupPasswordBox.Password = "";
            PopupPasswordConfirmBox.Password = "";
            PopupNicknameTextBox.Text = "";
        }

        private void PopupCancelButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterPopup.IsOpen = false;
        }
        #endregion
    }
}
