using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveBoard.ProtocolUtility
{
    public class ResponseResult
    {
        //请求类型
        public String command = "";

        //请求结果 1为成功，0为失败
        public int code;

        //返回提示语
        public String tips = "";
    }
}
