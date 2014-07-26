using LiveBoard.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class RoomListPage : Page
    {
        public RoomListPage()
        {
            this.InitializeComponent();
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.isConnected)
            {
                try
                {
                    if (App.responseResult.command == "roomList" && App.responseResult.code == 1)
                    {
                        RoomListView.ItemsSource = App.newRoomVM.rooms;
                        RoomListView.SelectedIndex = -1;
                    }
                    else
                        RoomListRefresh();
                }
                catch (Exception exception)
                {
                    App.ShowMessage(exception.Message, "Error");
                }
            }
            NicknameTextBlock.Text = App.currentUser.Nickname;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            RoomCreatePopup.IsOpen = true;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            App.responseResult.code = 0;
            RoomListRefresh();
        }

        private async void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CProtocolEngine engine = new CProtocolEngine();
                engine.Request(SchemaDef.LOGOUT, App.currentUser, null);
                engine.ReceiveData(SchemaDef.LOGIN);
                bool isLoop = true;
                while (isLoop)
                {
                    await Task.Delay(100);
                    if (App.responseResult.command == "logout" && App.responseResult.code == 1)
                    {
                        isLoop = false;
                        Frame.Navigate(typeof(MainPage));
                    }
                    if (App.responseResult.command == "logout" && App.responseResult.code == 0)
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

        private async void RoomListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.currentRoom = (sender as ListView).SelectedItem as Room;//获取当前房间
            if (App.currentRoom!=null)
            {
                if (App.currentRoom.RoomPassword != "")//判断是否有密码
                    EnterPasswordPopup.IsOpen = true;
                else
                {
                    try
                    {
                        CProtocolEngine engine = new CProtocolEngine();//发送加入房间请求
                        engine.Request(SchemaDef.ROOMJOIN, App.currentUser, App.currentRoom);
                        engine.ReceiveData(SchemaDef.ROOMJOIN);
                        bool isLoop = true;
                        while (isLoop)//根据回复做出动作
                        {
                            await Task.Delay(100);
                            if (App.responseResult.command == "roomJoin" && App.responseResult.code == 1)//加入房间成功
                            {
                                isLoop = false;
                                Frame.Navigate(typeof(BoardPage));//跳转到绘图页
                            }
                            if (App.responseResult.command == "roomJoin" && App.responseResult.code == 0)//加入房间失败
                            {
                                isLoop = false;
                                App.ShowMessage(App.responseResult.tips, "Error");//弹出提示
                            }
                        }
                    }
                    catch
                    {
                        App.ShowMessage("请求错误", "Warning");
                    }
                } 
            }
        }

        private async void RoomListRefresh()
        {
            App.newRoomVM.rooms.Clear();
            if (App.isConnected)
            {
                try
                {
                    CProtocolEngine engine = new CProtocolEngine();
                    engine.Request(SchemaDef.ROOMLIST, App.currentUser, null);
                    engine.ReceiveData(SchemaDef.ROOMLIST);
                    bool isLoop = true;
                    while (isLoop)
                    {
                        await Task.Delay(100);
                        if (App.responseResult.command == "roomList" && App.responseResult.code == 1)
                        {
                            isLoop = false;
                            RoomListView.ItemsSource = App.newRoomVM.rooms;
                            RoomListView.SelectedIndex = -1;
                        }
                        if (App.responseResult.command == "roomList" && App.responseResult.code == 0)
                        {
                            isLoop = false;
                            App.ShowMessage(App.responseResult.tips, "Error");
                        }
                    }
                }
                catch (Exception exception)
                {
                    App.ShowMessage(exception.Message, "Error");
                }
            }
            else
                App.ShowMessage("未连接到服务器", "Error");
        }

        #region Room Create Popup
        private async void CreateRoom(int maximum)
        {
            App.currentRoom.RoomTitle = RoomTitleTextBox.Text;
            App.currentRoom.RoomPassword = RoomPasswordTextBox.Password;
            App.currentRoom.MaximumNumber = maximum;
            App.currentRoom.PresentNumber = 1;

            try
            {
                CProtocolEngine engine = new CProtocolEngine();
                engine.Request(SchemaDef.ROOMCREATE, App.currentUser, App.currentRoom);
                engine.ReceiveData(SchemaDef.ROOMCREATE);
                bool isLoop = true;
                while (isLoop)
                {
                    await Task.Delay(100);
                    if (App.responseResult.command == "roomCreate" && App.responseResult.code == 1)
                    {
                        isLoop = false;
                        Frame.Navigate(typeof(BoardPage));
                    }
                    if (App.responseResult.command == "roomCreate" && App.responseResult.code == 0)
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

        private void RoomCreatePopup_Opened(object sender, object e)
        {
            RoomTitleTextBox.Text = "Join Us";
            RoomPasswordTextBox.Password = "";
            RoomMaximumNumberTextBox.Text = "最大为16";
        }

        private void PopupCreateButton_Click(object sender, RoutedEventArgs e)
        {
            int MaximumParseOut = 0;

            if (RoomTitleTextBox.Text == "")
            {
                App.ShowMessage("房间名不能为空，请改正", "Warning");
            }
            else if(RoomMaximumNumberTextBox.Text=="最大为16")
            {
                CreateRoom(16);
            }
            else if (int.TryParse(RoomMaximumNumberTextBox.Text, out MaximumParseOut))
            {
                if (MaximumParseOut < 1 || MaximumParseOut > 16)
                {
                    App.ShowMessage("最大人数超出范围，请改正", "Warning");
                }
                else
                {
                    CreateRoom(MaximumParseOut);
                }
            }
            else
            {
                App.ShowMessage("最大人数输入错误，请改正", "Warning");
            }
        }

        private void PopupResetButton_Click(object sender, RoutedEventArgs e)
        {
            RoomTitleTextBox.Text = "Join Us";
            RoomPasswordTextBox.Password = "";
            RoomMaximumNumberTextBox.Text = "最大为16";
        }

        private void PopupCancelButton_Click(object sender, RoutedEventArgs e)
        {
            RoomCreatePopup.IsOpen = false;
        }

        #region Room Title TextBox
        private void RoomTitleTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (RoomTitleTextBox.Text == "Join Us")
                RoomTitleTextBox.Select(0, 0);
        }

        private void RoomTitleTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (RoomTitleTextBox.Text == "")
                RoomTitleTextBox.Text = "Join Us";
        }

        private void RoomTitleTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (RoomTitleTextBox.Text == "Join Us")
                RoomTitleTextBox.Text = "";
        }
        #endregion

        #region Room Maximum Number TextBox
        private void RoomMaximumNumberTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (RoomMaximumNumberTextBox.Text == "最大为16")
                RoomMaximumNumberTextBox.Select(0, 0);

        }

        private void RoomMaximumNumberTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (RoomMaximumNumberTextBox.Text == "")
                RoomMaximumNumberTextBox.Text = "16";
        }

        private void RoomMaximumNumberTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (RoomMaximumNumberTextBox.Text == "最大为16")
                RoomMaximumNumberTextBox.Text = "";

        }
        #endregion
        #endregion

        #region Enter Password Popup
        private void EnterPasswordPopup_Opened(object sender, object e)
        {
            EnterPasswordBox.Password = "";
            EnterPasswordBox.Focus(FocusState.Programmatic);
        }

        private void EnterPasswordBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
                PasswordConfirmButton_Click(sender, e);
        }

        private async void PasswordConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (EnterPasswordBox.Password == App.currentRoom.RoomPassword)
            {
                try
                {
                    CProtocolEngine engine = new CProtocolEngine();
                    engine.Request(SchemaDef.ROOMJOIN, App.currentUser, App.currentRoom);
                    engine.ReceiveData(SchemaDef.ROOMJOIN);
                    bool isLoop = true;
                    while (isLoop)
                    {
                        await Task.Delay(100);
                        if (App.responseResult.command == "roomJoin" && App.responseResult.code == 1)
                        {
                            isLoop = false;
                            Frame.Navigate(typeof(BoardPage));
                        }
                        if (App.responseResult.command == "roomJoin" && App.responseResult.code == 0)
                        {
                            isLoop = false;
                            App.ShowMessage(App.responseResult.tips, "Error");
                        }
                    }
                }
                catch
                {
                    App.ShowMessage("请求错误", "Warning");
                }
            }
            else
            {
                App.ShowMessage("密码错误，请重新输入", "Error");
                EnterPasswordBox.Focus(FocusState.Programmatic);
                EnterPasswordBox.SelectAll();
            }
        }

        private void PasswordClearButton_Click(object sender, RoutedEventArgs e)
        {
            EnterPasswordBox.Password = "";
        }

        private void PasswordCancelButton_Click(object sender, RoutedEventArgs e)
        {
            EnterPasswordPopup.IsOpen = false;
            RoomListView.SelectedIndex = -1;            
        }
        #endregion
    }
}