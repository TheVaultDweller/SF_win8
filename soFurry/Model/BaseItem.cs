using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace soFurry.Model
{
    public class BaseItem : INotifyPropertyChanged
    {
        private int _pid;
        public int Pid { get { return _pid; } set { _pid = value; ContentChange("Pid"); } }
        
        private int _contentType;
        public int ContentType { get { return _contentType; } set { _contentType = value; ContentChange("ContentType"); } }

        private String _name;
        public String Name { get { return _name; } set { _name = value; ContentChange("Name"); } }

        private String _AuthorName;
        public String AuthorName { get { return _AuthorName; } set { _AuthorName = value; ContentChange("AuthorName"); } }

        private int _AuthorId;
        public int AuthorId { get { return _AuthorId; } set { _AuthorId = value; ContentChange("AuthorId"); } }

        private String _Thumb;
        public String Thumb { get { return _Thumb; } set { _Thumb = value; ContentChange("Thumb"); } }

        private int _ThumbnailDimension;
        public int ThumbnailDimension { get { return _ThumbnailDimension; } set { _ThumbnailDimension = value; ContentChange("ThumbnailDimension"); } }

        private DateTime _Date;
        public DateTime Date { get { return _Date; } set { _Date = value; ContentChange("Date"); } }

        private String _Keywords;
        public String Keywords { get { return _Keywords; } set { _Keywords = value; ContentChange("Keywords"); } }

        private void ContentChange(string s)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(s));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
