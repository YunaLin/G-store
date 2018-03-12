using G_Store;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using G_Store.Models;
using G_Store.ViewModels;
// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace G_Store
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NewPage : Page
    {
        private ViewModels.TodoItemViewModel ViewModel;
        private SQLiteConnection conn = App.conn;
        public NewPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
            
        }

        /*public void tile()
        {
            Windows.Data.Xml.Dom.XmlDocument xdoc = new Windows.Data.Xml.Dom.XmlDocument();
            xdoc.LoadXml(File.ReadAllText("tile.xml"));
            Windows.Data.Xml.Dom.XmlNodeList tilelist = xdoc.GetElementsByTagName("text");
            tilelist[0].InnerText = title.Text;
            tilelist[2].InnerText = title.Text;
            tilelist[4].InnerText = title.Text;
            tilelist[1].InnerText = describe.Text;
            tilelist[3].InnerText = describe.Text;
            tilelist[5].InnerText = describe.Text;
            tilelist[6].InnerText = title.Text;
            tilelist[7].InnerText = describe.Text;
            TileNotification notification = new TileNotification(xdoc);
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
            TileUpdater updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.Update(notification);

        }
        */
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
                SystemNavigationManager.GetForCurrentView().BackRequested += (s, ee) =>
                {  // 注册后退按钮事件。
                    if (Frame.CanGoBack)
                    {
                        ViewModel.SelectedItem = null;
                        Frame.Navigate(typeof(NotePage), ViewModel);
                    }
                };

            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }

            ViewModel = ((ViewModels.TodoItemViewModel)e.Parameter);
            if (ViewModel.SelectedItem == null)
            {
                createButton.Content = "Create";
                var i = new MessageDialog("Welcome!").ShowAsync();
                DeleteAppBarButton.Opacity = 0;
            }
            else
            {
                createButton.Content = "Update";
                date.Date = ViewModel.SelectedItem.date;
                title.Text = ViewModel.SelectedItem.title;
                describe.Text = ViewModel.SelectedItem.description;

   

            }

        }
        public string imgname = "";
        private async void appBarButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                var Bi = new BitmapImage();
                Bi.SetSource(stream);
                image.Source = Bi;
                var name = file.Path.Substring(file.Path.LastIndexOf('\\') + 1);
                imgname = name;
                await file.CopyAsync(ApplicationData.Current.LocalFolder, name, NameCollisionOption.GenerateUniqueName);
            }
        }
        private async void create_Click(object sender, RoutedEventArgs e)
        {

            if (title.Text == "" && describe.Text == "")
            {
                var messageDialog = new MessageDialog("标题和描述不能为空!");
                messageDialog.Commands.Add(new UICommand("OK"));
                messageDialog.ShowAsync();
            }
            else if (title.Text == "")
            {
                var messageDialog = new MessageDialog("标题不能为空!");
                messageDialog.Commands.Add(new UICommand("OK"));
                messageDialog.ShowAsync();
            }
            else if (describe.Text == "")
            {
                var messageDialog = new MessageDialog("描述不能为空!");
                messageDialog.Commands.Add(new UICommand("OK"));
                messageDialog.ShowAsync();
            }

            else if (date.Date < DateTime.Today)
            {
                var messageDialog = new MessageDialog("日期至少为今天!");
                messageDialog.Commands.Add(new UICommand("OK"));
                messageDialog.ShowAsync();
            }
            else
            {
                if (createButton.Content.ToString() == "Create")
                {

                    var messageDialog = new MessageDialog("成功!");
                    messageDialog.Commands.Add(new UICommand("OK"));
                    ViewModel.AddTodoItem(title.Text, describe.Text, date.Date, imgname);
                    using (var res = conn.Prepare("INSERT INTO Todo (Id, Title, Detail, Date, finish, imgname) VALUES (?,?,?,?,?,?)"))
                    {

                        res.Bind(4, date.Date.ToString());
                        //res.Bind(2, title);
                        res.Bind(2, title.Text.Trim());
                        res.Bind(3, describe.Text.Trim());
                        res.Bind(5, "false");
                        res.Bind(1, ViewModel.AllItems.Last().id);
                        res.Bind(6, imgname);
                        res.Step();

                    }
                    //tile();
                    await messageDialog.ShowAsync();  //按确定之后清除原有信息
                    title.Text = "";
                    describe.Text = "";
                    date.Date = DateTime.Today;
                    BitmapImage bi = new BitmapImage();
                    bi.UriSource = new Uri(image.BaseUri, "Assets/timg.jpg");
                    image.Source = bi;  //恢复原来

                    Frame.Navigate(typeof(NotePage), ViewModel);
                    //可以设置跳回主页面

                }
                else if (createButton.Content.ToString() == "Update")
                {
                    TodoItem newitem = new TodoItem(title.Text, describe.Text, date.Date);
                    string sql = @"UPDATE Todo SET Date = ?, Title = ?, Detail = ? WHERE Id = ?";
                    using (var res = conn.Prepare(sql))
                    {
                        res.Bind(1, date.Date.Date.ToString());
                        //res.Bind(2, title);
                        res.Bind(2, title.Text.Trim());
                        res.Bind(3, describe.Text.Trim());
                        res.Bind(4, this.ViewModel.SelectedItem.id);

                        res.Step();

                    }


                    var messageDialog = new MessageDialog("成功!");
                    messageDialog.Commands.Add(new UICommand("OK"));
                    await messageDialog.ShowAsync();  //按确定之后清除原有信息
                    //tile();
                    title.Text = "";
                    describe.Text = "";
                    date.Date = DateTime.Today;

                    ViewModel.UpdateTodoItem(newitem.id, newitem.title, newitem.description, newitem.date);
                    Frame.Navigate(typeof(NotePage), ViewModel);  //可以设置跳回主页面

                }
            }
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            //  var messageDialog = new MessageDialog("清除成功!");
            //   messageDialog.Commands.Add(new UICommand("OK"));
            //  await messageDialog.ShowAsync();  //按确定之后清除原有信息
            title.Text = "";
            describe.Text = "";
            date.Date = DateTime.Today;
            BitmapImage bi = new BitmapImage();
            bi.UriSource = new Uri(image.BaseUri, "Assets/timg.jpg");
            image.Source = bi;
        }







        private async void DeleteButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                string sql = @"DELETE FROM Todo WHERE Id = ?";

                using (var res = conn.Prepare(sql))
                {
                    res.Bind(1, ViewModel.SelectedItem.id);
                    res.Step();
                    ViewModel.RemoveTodoItem(ViewModel.SelectedItem.id);

                    await new MessageDialog("Delete successfully！").ShowAsync();
                    Frame.Navigate(typeof(MainPage), ViewModel);

                }
            }
        }
    }
}
