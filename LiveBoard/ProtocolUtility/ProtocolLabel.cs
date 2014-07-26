using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveBoard.ProtocolUtility
{
    public abstract class ProtocolLabel
    {
        //基本信息
        public static String Label_Protocol = "protocol";
        public static String Label_Protocol_Version = "1.2.2";
        public static String Label_ProtocolType = "protocolType";
        public static String Label_Protocol_Request = "request";

        //请求类型
        public static String Label_Command = "command";
        public static String Label_Command_Connection = "connection";
        public static String Label_Command_Register = "register";
        public static String Label_Command_Login = "login";
        public static String Label_Command_Logout = "logout";
        public static String Label_Command_RoomList = "roomList";
        public static String Label_Command_RoomCreate = "roomCreate";
        public static String Label_Command_RoomJoin = "roomJoin";
        public static String Label_Command_RoomExit = "roomExit";
        public static String Label_Command_DrawAction = "draw";

        //服务器返回
        public static String Label_Result = "result";
        public static String Label_Result_Code = "code";
        public static String Label_Result_Tips = "tips";

        //用户信息
        public static String Label_User = "userInfo";
        public static String Label_User_Username = "username";
        public static String Label_User_Password = "password";
        public static String Label_User_Nickname = "nickname";
        public static String Label_User_UID = "uid";
        public static String Label_User_Status = "status";
        public static String Label_User_Host = "host";
        public static String Label_User_Port = "port";

        //房间信息
        public static String Label_Room = "roomInfo";
        public static String Label_Room_List = "roomList";
        public static String Label_Room_RoomTitle = "title";
        public static String Label_Room_RoomPassword = "password";
        public static String Label_Room_RoomID = "rid";
        public static String Label_Room_Maximum = "max";
        public static String Label_Room_Present = "status";

        //动作信息
        public static String Label_Action = "action";
        public static String Label_Action_Shape = "shape";
        public static String Label_Action_Coordinate = "coordinate";
        public static String Label_Action_CoordinateX = "x";
        public static String Label_Action_CoordinateY = "y";
        public static String Label_Action_Thickness = "thickness";
        public static String Label_Action_PointSize = "pointsize";
        public static String Label_Action_Color = "color";
        public static String Label_Action_Text = "text";
    }
}
