using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace G_Store.Models
{
    class TodoItem : INotifyPropertyChanged
    {

        public string id { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public bool completed { get; set; }
        public DateTimeOffset date { get; set; }
        //日期字段自己写

        public TodoItem(string title, string description, DateTimeOffset date, string imgname = "", bool finish = false)
        {
            this.id = Guid.NewGuid().ToString(); //生成id
            this.title = title;
            this.description = description;
            this.date = date;
            this.imgname = imgname;
            this.completed = finish; //默认为未完成
            setImg();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
        private string _imgname;
        public string imgname
        {
            set
            {
                _imgname = value;
                NotifyPropertyChanged("imgname");
                setImg();
            }
            get
            {
                return _imgname;
            }
        }
        private ImageSource _img;
        public ImageSource img
        {
            set
            {
                _img = value;
                NotifyPropertyChanged("img");
            }
            get
            {
                return _img;
            }
        }
        public async void setImg()
        {
            if (imgname == "")
            {
                this.img = new BitmapImage(new Uri("ms-appx:///Assets/20150171504jpg"));
            }
            else
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(imgname);
                IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);
                BitmapImage bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(fileStream);
                this.img = bitmapImage;
            }
        }
    }
}
