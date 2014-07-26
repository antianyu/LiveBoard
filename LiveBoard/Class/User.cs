using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveBoard.Class
{
    public class User
    {
        //用户名
        public string Username = "";

        //密码
        public string Password = "";

        //昵称
        public string Nickname = "";

        //UID
        public int UID = 0;

        //是否登录
        public int Status = 0;

        //服务器地址
        public string Host = "";

        //端口号
        public int Port = 0;

        //头像
        //public Windows.UI.Xaml.Media.Imaging.BitmapImage Photo { get; set; }
    }
}
