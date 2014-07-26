using LiveBoard.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace LiveBoard.ProtocolUtility
{
    public class JsonParser:ProtocolLabel
    {
        private JsonObject obj = new JsonObject(); 

        public JsonParser(String str)
        {
            obj = JsonObject.Parse(str);
        }

        //解析请求结果
        public ResponseResult getResult(ResponseResult result)
        {
            result.command = obj.GetNamedString(Label_Command);
            JsonObject tempJO = new JsonObject();
            tempJO = obj.GetNamedObject(Label_Result);
            result.code = Convert.ToInt32(tempJO.GetNamedNumber(Label_Result_Code));
            result.tips = tempJO.GetNamedString(Label_Result_Tips);
            return result;
        }

        //解析用户信息
        public User getUserInfo(User userinfo)
        {
            try
            {
                JsonObject tempJO = new JsonObject();
                tempJO = obj.GetNamedObject(Label_User);
                userinfo.Username = tempJO.GetNamedString(Label_User_Username);
                userinfo.Password = tempJO.GetNamedString(Label_User_Password);
                userinfo.UID = Convert.ToInt32(tempJO.GetNamedNumber(Label_User_UID));
                userinfo.Nickname = tempJO.GetNamedString(Label_User_Nickname);
                userinfo.Status = Convert.ToInt32(tempJO.GetNamedNumber(Label_User_Status));
                userinfo.Host = tempJO.GetNamedString(Label_User_Host);
                userinfo.Port = Convert.ToInt32(tempJO.GetNamedNumber(Label_User_Port));

                return userinfo;
            }
            catch
            {
                return userinfo;
            }
        }

        //解析房间信息
        public Room getRoom(Room Room)
        {
            try
            {
                JsonObject tempJO = new JsonObject();
                tempJO = obj.GetNamedObject(Label_Room);
                Room.RoomTitle = tempJO.GetNamedString(Label_Room_RoomTitle);
                Room.RoomPassword = tempJO.GetNamedString(Label_Room_RoomPassword);
                Room.RoomID = Convert.ToInt32(tempJO.GetNamedNumber(Label_Room_RoomID));
                Room.MaximumNumber = Convert.ToInt32(tempJO.GetNamedNumber(Label_Room_Maximum));
                Room.PresentNumber = Convert.ToInt32(tempJO.GetNamedNumber(Label_Room_Present));

                return Room;
            }
            catch
            {
                return Room; 
            }
        }

        //解析房间列表
        public List<Room> getRoomList(List<Room> aList)
        {
            try
            {
                JsonArray tempJO = new JsonArray();
                tempJO = obj.GetNamedArray(Label_Room_List);
                for (uint i = 0; i < tempJO.Count; i++)
                {
                    JsonObject roomJO = new JsonObject();
                    Room tempRoom = new Room();
                    roomJO = tempJO.GetObjectAt(i);
                    tempRoom.RoomTitle = roomJO.GetNamedString(Label_Room_RoomTitle);
                    tempRoom.RoomPassword = roomJO.GetNamedString(Label_Room_RoomPassword);
                    tempRoom.RoomID = Convert.ToInt32(roomJO.GetNamedNumber(Label_Room_RoomID));
                    tempRoom.MaximumNumber = Convert.ToInt32(roomJO.GetNamedNumber(Label_Room_Maximum));
                    tempRoom.PresentNumber = Convert.ToInt32(roomJO.GetNamedNumber(Label_Room_Present));
                    aList.Add(tempRoom);
                }

                return aList;
            }
            catch
            {
                return aList;
            }
        }

        //解析动作信息
        public DrawAction getAction(DrawAction action)
        {
            try
            {
                JsonObject tempJO = new JsonObject();
                tempJO = obj.GetNamedObject(Label_Action);
                action.Shape = tempJO.GetNamedString(Label_Action_Shape);
                action.Color = tempJO.GetNamedString(Label_Action_Color);
                IJsonValue jsonValue;
                if (tempJO.TryGetValue(Label_Action_PointSize, out jsonValue))
                    action.PointSize = jsonValue.GetNumber();
                if (tempJO.TryGetValue(Label_Action_Thickness, out jsonValue))
                    action.Thickness = jsonValue.GetNumber();
                if (tempJO.TryGetValue(Label_Action_Text, out jsonValue))
                    action.Text = jsonValue.GetString();
                JsonArray coordinateArray = tempJO.GetNamedArray(Label_Action_Coordinate);
                for (int i = 0; i < coordinateArray.Count; i++)
                {
                    JsonObject coordinate = new JsonObject();
                    coordinate = coordinateArray.GetObjectAt((uint)i);
                    Coordinate tempCoordinate = new Coordinate();
                    tempCoordinate.X = coordinate.GetNamedNumber(Label_Action_CoordinateX);
                    tempCoordinate.Y = coordinate.GetNamedNumber(Label_Action_CoordinateY);
                    action.Coordinates.Add(tempCoordinate);
                }
                return action;
            }
            catch
            {
                return action;
            }
        }
    }
}
