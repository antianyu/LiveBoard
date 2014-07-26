using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveBoard.Class
{
    public class DrawAction
    {
        //形状
        public String Shape = "";

        //坐标
        public List<Coordinate> Coordinates = new List<Coordinate>();

        //点大小
        public double PointSize = 0;

        //线宽度
        public double Thickness = 0;

        //颜色
        public String Color = "";

        //文字内容
        public String Text = "";
    }
}
