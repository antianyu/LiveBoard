using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace LiveBoard.Class
{
    public class Room:INotifyPropertyChanged
    {
        private string roomtitle;
        public string RoomTitle 
        { 
            get
            {
                return roomtitle;
            }
            set
            {
                if (value != roomtitle)
                {
                    roomtitle = value;
                    NotifyPropertyChanged("RoomTitle"); 
                }
            }
        }

        private string roompassword;
        public string RoomPassword
        {
            get
            {
                return roompassword;
            }
            set
            {
                if (value != roompassword)
                {
                    roompassword = value;
                    NotifyPropertyChanged("RoomPassword");
                }
            }
        }

        private int roomid;
        public int RoomID
        {
            get
            {
                return roomid;
            }
            set
            {
                if (value != roomid)
                {
                    roomid = value;
                    NotifyPropertyChanged("RoomID");
                }
            }
        }

        private int maximumnumber;
        public int MaximumNumber
        {
            get
            {
                return maximumnumber;
            }
            set
            {
                if (value != maximumnumber)
                {
                    maximumnumber = value;
                    NotifyPropertyChanged("MaximumNumber");
                }
            }
        }

        private int presentnumber;
        public int PresentNumber
        {
            get
            {
                return presentnumber;
            }
            set
            {
                if (value != presentnumber)
                {
                    presentnumber = value;
                    NotifyPropertyChanged("PresentNumber");
                }
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
