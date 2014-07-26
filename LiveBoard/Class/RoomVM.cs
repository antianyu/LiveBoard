using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LiveBoard.Class
{
    public class RoomVM:INotifyPropertyChanged
    {
        public ObservableCollection<Room> rooms { get; private set; }

        public RoomVM()
        {
            this.rooms = new ObservableCollection<Room>();
        }

        public void LoadData(Room tempRoom)
        {
            this.rooms.Add(new Room()
            {
                RoomTitle = tempRoom.RoomTitle,
                RoomID = tempRoom.RoomID,
                RoomPassword = tempRoom.RoomPassword,
                MaximumNumber = tempRoom.MaximumNumber,
                PresentNumber = tempRoom.PresentNumber
            });
        }

        public void LoadData(List<Room> aList)
        {
            rooms.Clear();
            for (int i = 0; i < aList.Count; i++)
            {
                this.rooms.Add(new Room()
                {
                    RoomTitle = aList[i].RoomTitle,
                    RoomID = aList[i].RoomID,
                    RoomPassword = aList[i].RoomPassword,
                    MaximumNumber = aList[i].MaximumNumber,
                    PresentNumber = aList[i].PresentNumber
                }); 
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
