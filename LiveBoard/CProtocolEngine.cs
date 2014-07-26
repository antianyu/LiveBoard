using LiveBoard.ProtocolUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Data.Json;
using Windows.Storage.Streams;
using Windows.ApplicationModel.Core;
using Windows.UI.Popups;
using LiveBoard.Class;

namespace LiveBoard
{
    public class CProtocolEngine:ProtocolLabel,IProtocolEngine
    {
        StreamSocket clientSocket = new StreamSocket();

        public void Request(SchemaDef schema, User User, Object obj)
        {
            switch (schema)
            {
                case SchemaDef.CONNECT:
                    {
                        RequestConnect();
                        break;
                    }
                case SchemaDef.REGISTER:
                    {
                        RequestRegister(User);
                        break;
                    }
                case SchemaDef.LOGIN:
                    {
                        RequestLogin(User);
                        break;
                    }
                case SchemaDef.LOGOUT:
                    {
                        RequestLogout(User);
                        break;
                    }
                case SchemaDef.ROOMLIST:
                    {
                        RequestRoomList(User);
                        break;
                    }
                case SchemaDef.ROOMJOIN:
                    {
                        RequestRoomJoin(User,obj);
                        break;
                    }
                case SchemaDef.ROOMEXIT:
                    {
                        RequestRoomExit(User, obj);
                        break;
                    }
                case SchemaDef.ROOMCREATE:
                    {
                        RequestRoomCreate(User,obj);
                        break;
                    }
                case SchemaDef.ACTION:
                    {
                        RequestAction(User,obj);
                        break;
                    }
            }
        }

        private async void RequestConnect()
        {
            //连接到服务器
            try
            {
                if (!CoreApplication.Properties.ContainsKey("clientSocket"))
                {
                    await clientSocket.ConnectAsync(URLDef.IP, URLDef.PortNumber);
                    CoreApplication.Properties.Add("clientSocket", clientSocket);
                    CoreApplication.Properties.Add("Connected", null);
                }
            }
            catch(Exception exception)
            {
                App.isConnectError = true;
            }
        }

        private void RequestRegister(User User)
        {
            JsonObject tempJO = new JsonObject();
            JsonConstructor Jconstructor = new JsonConstructor();
            Jconstructor.setCommandValue(Label_Command_Register);
            tempJO = Jconstructor.setBasicInfo(tempJO, User);
            SendData(tempJO);
        }

        private void RequestLogin(User User)
        {
            JsonObject tempJO = new JsonObject();
            JsonConstructor Jconstructor = new JsonConstructor();
            Jconstructor.setCommandValue(Label_Command_Login);
            tempJO = Jconstructor.setBasicInfo(tempJO, User);
            SendData(tempJO);
        }

        private void RequestLogout(User User)
        {
            JsonObject tempJO = new JsonObject();
            JsonConstructor Jconstructor = new JsonConstructor();
            Jconstructor.setCommandValue(Label_Command_Logout);
            tempJO = Jconstructor.setBasicInfo(tempJO, User);
            SendData(tempJO);
        }

        private void RequestRoomList(User User)
        {
            JsonObject tempJO = new JsonObject();
            JsonConstructor Jconstructor = new JsonConstructor();
            Jconstructor.setCommandValue(Label_Command_RoomList);
            tempJO = Jconstructor.setBasicInfo(tempJO, User);
            SendData(tempJO);
        }

        private void RequestRoomJoin(User User, Object obj)
        {
            JsonObject tempJO = new JsonObject();
            JsonConstructor Jconstructor = new JsonConstructor();
            Jconstructor.setCommandValue(Label_Command_RoomJoin);
            tempJO = Jconstructor.setBasicInfo(tempJO, User);
            tempJO = Jconstructor.setRoomInfo(tempJO, (Room)obj);
            SendData(tempJO);
        }

        private void RequestRoomExit(User User, Object obj)
        {
            JsonObject tempJO = new JsonObject();
            JsonConstructor Jconstructor = new JsonConstructor();
            Jconstructor.setCommandValue(Label_Command_RoomExit);
            tempJO = Jconstructor.setBasicInfo(tempJO, User);
            tempJO = Jconstructor.setRoomInfo(tempJO, (Room)obj);
            SendData(tempJO);
        }

        private void RequestRoomCreate(User User, Object obj)
        {
            JsonObject tempJO = new JsonObject();
            JsonConstructor Jconstructor = new JsonConstructor();
            Jconstructor.setCommandValue(Label_Command_RoomCreate);
            tempJO = Jconstructor.setBasicInfo(tempJO,User);
            tempJO = Jconstructor.setRoomInfo(tempJO, (Room)obj);
            SendData(tempJO);            
        }

        private void RequestAction(User User, Object obj)
        {
            JsonObject tempJO = new JsonObject();
            JsonConstructor Jconstructor = new JsonConstructor();
            Jconstructor.setCommandValue(Label_Command_DrawAction);
            tempJO = Jconstructor.setBasicInfo(tempJO, User);
            tempJO = Jconstructor.setDrawActionInfo(tempJO, (DrawAction)obj);
            SendData(tempJO);
        }

        private async void SendData(JsonObject obj)
        {
            try
            {
                //判断是否连接
                if (!CoreApplication.Properties.ContainsKey("Connected"))
                {
                    App.ShowMessage("未连接到服务器。", "Error");
                    return;
                }

                //读出StreamSocket
                object outValue;
                StreamSocket socket;
                if (!CoreApplication.Properties.TryGetValue("clientSocket", out outValue))
                {
                    App.ShowMessage("未登录", "Error");
                    return;
                }
                socket = (StreamSocket)outValue;

                //写入数据
                DataWriter writer;
                if (!CoreApplication.Properties.TryGetValue("clientDataWriter", out outValue))
                {
                    writer = new DataWriter(socket.OutputStream);
                    CoreApplication.Properties.Add("clientDataWriter", writer);
                }
                else
                    writer = (DataWriter)outValue;

                string jsonstring = obj.Stringify();
                writer.WriteString(jsonstring);
                
                //发送数据
                await writer.StoreAsync();
            }
            catch(Exception exception)
            {
                App.ShowMessage(exception.Message, "Error");
            }
        }

        public async void ReceiveData(SchemaDef schema)
        {
            try
            {
                #region Get DataReader
                object outValue;
                DataReader reader;
                if (!CoreApplication.Properties.TryGetValue("clientDataReader", out outValue))
                {
                    reader = new DataReader(clientSocket.InputStream);
                    CoreApplication.Properties.Add("clientDataReader", reader);
                }
                else
                    reader = (DataReader)outValue;
                #endregion

                reader.InputStreamOptions = InputStreamOptions.Partial;
                await reader.LoadAsync(65536);
                String tempstr = reader.ReadString(reader.UnconsumedBufferLength);
                JsonParser parser = new JsonParser(tempstr);
                App.responseResult = parser.getResult(App.responseResult);

                #region Data Process
                switch (schema)
                {
                    case SchemaDef.CONNECT:
                        {
                            App.isConnected = Convert.ToBoolean(App.responseResult.code);
                            break;
                        }
                    case SchemaDef.REGISTER:
                        {
                            App.currentUser = parser.getUserInfo(App.currentUser);
                            break;
                        }
                    case SchemaDef.LOGIN:
                        {
                            App.currentUser = parser.getUserInfo(App.currentUser);
                            break;
                        }
                    case SchemaDef.LOGOUT:
                        {
                            App.currentUser = parser.getUserInfo(App.currentUser);
                            break;
                        }
                    case SchemaDef.ROOMLIST:
                        {
                            List<Room> tempList = new List<Room>();
                            tempList = parser.getRoomList(tempList);
                            App.newRoomVM.LoadData(tempList);
                            break;
                        }
                    case SchemaDef.ROOMJOIN:
                        {
                            App.currentUser = parser.getUserInfo(App.currentUser);
                            App.currentRoom = parser.getRoom(App.currentRoom);
                            break;
                        }
                    case SchemaDef.ROOMEXIT:
                        {
                            App.currentUser = parser.getUserInfo(App.currentUser);
                            App.currentRoom = parser.getRoom(App.currentRoom);
                            break;
                        }
                    case SchemaDef.ROOMCREATE:
                        {
                            App.currentUser = parser.getUserInfo(App.currentUser);
                            App.currentRoom = parser.getRoom(App.currentRoom);
                            break;
                        }
                    case SchemaDef.ACTION:
                        {
                            App.receivedAction = parser.getAction(App.receivedAction);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                #endregion
            }
            catch (Exception exception)
            {
                //App.ShowMessage(exception.Message, "Error");
            }
        }
    }
}