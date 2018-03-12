using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G_Store.Models;
using Windows.UI.Xaml.Media;

namespace G_Store.ViewModels
{
    class TodoItemViewModel
    {
        private ObservableCollection<Models.TodoItem> allItems = new ObservableCollection<Models.TodoItem>();
        public ObservableCollection<Models.TodoItem> AllItems { get { return this.allItems; } }

        public Models.TodoItem selectedItem = default(Models.TodoItem);
        public Models.TodoItem SelectedItem { get { return selectedItem; } set { this.selectedItem = value; } }

        public TodoItemViewModel()
        {
            // 加入两个用来测试的item
            //this.allItems.Add(new Models.TodoItem("123", "123", DateTime.Now));
            //this.allItems.Add(new Models.TodoItem("456", "456", DateTime.Now));
            var sql = "SELECT * FROM Todo";
            var conn = App.conn;
            using (var statement = conn.Prepare(sql))
            {
                while (SQLiteResult.ROW == statement.Step())
                {
                    var s = statement[3].ToString();
                    if (s != "")
                    {
                        s = s.Substring(0, s.IndexOf(' '));
                        string ID = (string)statement[0];
                        DateTime date = new DateTime(int.Parse(s.Split('/')[0]), int.Parse(s.Split('/')[1]), int.Parse(s.Split('/')[2]));
                        //string imgname = (string)statement[2];
                        string title = (string)statement[1];
                        string description = (string)statement[2];
                        bool finish = Boolean.Parse(statement[4] as string);
                        string imgname = (string)statement[5];
                        this.AddTodoItem(title, description, date, imgname);
                        this.AllItems.Last().id = ID;
                        this.AllItems.Last().completed = finish;
                    }

                }
            }
        }

        public void AddTodoItem(string title, string description, DateTimeOffset date, string imgname)
        {
            this.allItems.Add(new Models.TodoItem(title, description, date.Date, imgname));
        }

        public void RemoveTodoItem(string id)
        {
            // DIY
            if (this.selectedItem.id == id)
                this.allItems.Remove(selectedItem);
            // set selectedItem to null after remove
            this.selectedItem = null;
        }

        public void UpdateTodoItem(string id, string title, string description, DateTimeOffset date)
        {
            // DIY


            if (this.selectedItem != null)
            {
                this.selectedItem.description = description;
                this.selectedItem.title = title;
                // set selectedItem to null after update
                this.selectedItem.date = date;
                this.selectedItem = null;
            }
        }


    }
}
