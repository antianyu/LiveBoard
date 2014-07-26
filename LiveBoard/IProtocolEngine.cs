using LiveBoard.Class;
using LiveBoard.ProtocolUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveBoard
{
    public interface IProtocolEngine
    {
        void Request(SchemaDef schema, User User, Object obj);
    }

    public enum SchemaDef
    {
        CONNECT = 0x00,   //连接
        REGISTER=0x01,  //注册
        LOGIN = 0x02,   //登陆
        LOGOUT = 0x03,  //注销
        ROOMLIST=0x04,  //请求房间列表
        ROOMCREATE=0x05,//创建房间
        ROOMJOIN=0x06,  //加入房间
        ROOMEXIT=0x07,  //退出房间
        ACTION=0x08     //动作消息
    }
}
