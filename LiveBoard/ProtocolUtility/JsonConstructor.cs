using LiveBoard.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace LiveBoard.ProtocolUtility
{
    public class JsonConstructor:ProtocolLabel
    {
        public String commandValue;

        public JsonConstructor()
        {
 
        }

        public JsonObject setBasicInfo(JsonObject holder,User user)
        {
            //写入头信息
            holder.SetNamedValue(Label_Protocol, JsonValue.CreateStringValue(Label_Protocol_Version));
            holder.SetNamedValue(Label_ProtocolType, JsonValue.CreateStringValue(Label_Protocol_Request));
            holder.SetNamedValue(Label_Command, JsonValue.CreateStringValue(getCommandValue()));


            //写入用户信息
            JsonObject tempJO = new JsonObject();
            tempJO.SetNamedValue(Label_User_Username, JsonValue.CreateStringValue(user.Username));
            tempJO.SetNamedValue(Label_User_Password, JsonValue.CreateStringValue(user.Password));
            tempJO.SetNamedValue(Label_User_Nickname, JsonValue.CreateStringValue(user.Nickname));
            tempJO.SetNamedValue(Label_User_UID, JsonValue.CreateNumberValue(user.UID));
            tempJO.SetNamedValue(Label_User_Status, JsonValue.CreateNumberValue(user.Status));
            tempJO.SetNamedValue(Label_User_Host, JsonValue.CreateStringValue(user.Host));
            tempJO.SetNamedValue(Label_User_Port, JsonValue.CreateNumberValue(user.Port));

            holder.SetNamedValue(Label_User, tempJO);

            return holder;
        }

        public JsonObject setRoomInfo(JsonObject holder, Room room)
        {
            //写入房间信息
            JsonObject tempJO = new JsonObject();
            tempJO.SetNamedValue(Label_Room_RoomTitle, JsonValue.CreateStringValue(room.RoomTitle));
            tempJO.SetNamedValue(Label_Room_RoomPassword, JsonValue.CreateStringValue(room.RoomPassword));
            tempJO.SetNamedValue(Label_Room_RoomID, JsonValue.CreateNumberValue(room.RoomID));
            tempJO.SetNamedValue(Label_Room_Maximum, JsonValue.CreateNumberValue(room.MaximumNumber));
            tempJO.SetNamedValue(Label_Room_Present, JsonValue.CreateNumberValue(room.PresentNumber));

            holder.SetNamedValue(Label_Room, tempJO);

            return holder;
        }

        public JsonObject setDrawActionInfo(JsonObject holder, DrawAction action)
        {
            //写入动作信息
            JsonArray coordinateArray = new JsonArray();
            for (int i = 0; i < action.Coordinates.Count; i++)
            {
                JsonObject coordinate = new JsonObject();
                coordinate.SetNamedValue(Label_Action_CoordinateX, JsonValue.CreateNumberValue(action.Coordinates[i].X));
                coordinate.SetNamedValue(Label_Action_CoordinateY, JsonValue.CreateNumberValue(action.Coordinates[i].Y));
                coordinateArray.Add(coordinate);
            }

            JsonObject tempJO = new JsonObject();
            tempJO.SetNamedValue(Label_Action_Shape, JsonValue.CreateStringValue(action.Shape));
            tempJO.SetNamedValue(Label_Action_Color, JsonValue.CreateStringValue(action.Color));
            if (action.Shape == "Point" || action.Shape == "Eraser")
                tempJO.SetNamedValue(Label_Action_PointSize, JsonValue.CreateNumberValue(action.PointSize));
            if (action.Shape == "Line" || action.Shape == "Rectangle")
                tempJO.SetNamedValue(Label_Action_Thickness, JsonValue.CreateNumberValue(action.Thickness));
            if (action.Shape == "TextBox")
                tempJO.SetNamedValue(Label_Action_Text, JsonValue.CreateStringValue(action.Text));
            tempJO.SetNamedValue(Label_Action_Coordinate, coordinateArray);

            holder.SetNamedValue(Label_Action, tempJO);

            return holder;
        }

        public String getCommandValue()
        {
            return commandValue;
        }

        public void setCommandValue(String commandValue)
        {
            this.commandValue = commandValue;
        }
    }
}
