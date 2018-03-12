using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using G_Store.Models;
using G_Store.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Notifications;
using Windows.Storage;
using Windows.ApplicationModel;
using System.Diagnostics;
using Windows.Storage.Streams;
using System.Xml;
using G_Store.Models;
using G_Store.ViewModels;
using SQLitePCL;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Shapes;
using Windows.Data.Xml.Dom;
//using Microsoft.Toolkit.Uwp.Notifications;
//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace G_Store
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    /// 

    public sealed partial class NotePage : Page
    {

        //  private StorageFile _tempExportFile;
        DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
        private SQLiteConnection conn = App.conn;
        public NotePage()
        {


            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
            this.ViewModel = new ViewModels.TodoItemViewModel();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataRequested;
            ShowToastNotification();
        }
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

                        Frame.Navigate(typeof(MainPage));
                    }
                };

            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }



        }

        ViewModels.TodoItemViewModel ViewModel { get; set; }
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
                    ViewModel.AddTodoItem(title.Text, describe.Text, date.Date, "");
                    //string sql = @"INSERT INTO Todo (Id,Title, Detail, Date, finish) VALUES (?,?,?,?,?)";
                    using (var res = conn.Prepare("INSERT INTO Todo (Id, Title, Detail, Date, finish, imgname) VALUES (?,?,?,?,?,?)"))
                    {

                        res.Bind(4, date.Date.ToString());
                        //res.Bind(2, title);
                        res.Bind(2, title.Text.Trim());
                        res.Bind(3, describe.Text.Trim());
                        res.Bind(5, "false");
                        res.Bind(6, "");
                        res.Bind(1, ViewModel.AllItems.Last().id);
                        res.Step();

                    }
                    tile();
                    await messageDialog.ShowAsync();  //按确定之后清除原有信息
                    title.Text = "";
                    describe.Text = "";
                    date.Date = DateTime.Today;

                    Frame.Navigate(typeof(NotePage), ViewModel);  //可以设置跳回主页面
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
                    tile();
                    await messageDialog.ShowAsync();  //按确定之后清除原有信息

                    title.Text = "";
                    describe.Text = "";
                    date.Date = DateTime.Today;

                    ViewModel.UpdateTodoItem(newitem.id, newitem.title, newitem.description, newitem.date);
                    createButton.Content = "Create";
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

        }
        private void EditClick(object sender, RoutedEventArgs e)
        {
            var dc = (sender as FrameworkElement).DataContext;
            var item = ToDoListView.ContainerFromItem(dc) as ListViewItem;
            ViewModel.SelectedItem = item.Content as TodoItem;
            Frame.Navigate(typeof(NewPage), ViewModel);
        }
        private async void DeleteClick(object sender, RoutedEventArgs e)
        {
            var dc = (sender as FrameworkElement).DataContext;
            var item = ToDoListView.ContainerFromItem(dc) as ListViewItem;
            ViewModel.SelectedItem = item.Content as TodoItem;
            if (ViewModel.SelectedItem != null)
            {
                string sql = @"DELETE FROM Todo WHERE Id = ?";

                using (var res = conn.Prepare(sql))
                {
                    res.Bind(1, ViewModel.SelectedItem.id);
                    res.Step();
                    ViewModel.RemoveTodoItem(ViewModel.SelectedItem.id);

                    await new MessageDialog("Delete successfully！").ShowAsync();
                    Frame.Navigate(typeof(NotePage), ViewModel);

                }
            }
        }
        /*protected override void OnNavigatedTo(NavigationEventArgs e)
        {


            if (e.Parameter.GetType() == typeof(ViewModels.TodoItemViewModel))
            {
                this.ViewModel = (ViewModels.TodoItemViewModel)(e.Parameter);
            }
        }
        */

        private void TodoItem_ItemClicked(object sender, ItemClickEventArgs e)
        {
            ViewModel.SelectedItem = (Models.TodoItem)(e.ClickedItem);

            if (Window.Current.Bounds.Width < 800)
                Frame.Navigate(typeof(NewPage), ViewModel);
            else
            {

                title.Text = ViewModel.SelectedItem.title;
                describe.Text = ViewModel.SelectedItem.description;
                date.Date = ViewModel.SelectedItem.date;
                createButton.Content = "Update";
                pic.Source = this.ViewModel.selectedItem.img;
            }
        }

        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Bounds.Width > 800)
                return;

            Frame.Navigate(typeof(NewPage), ViewModel);
        }

        private void CheckBox2_checked(object sender, RoutedEventArgs e)
        {

            //if (ViewModel.SelectedItem != null) { 
            //  string sql = @"UPDATE Todo SET finish = ? WHERE Id = ?";
            //  using (var res = conn.Prepare(sql))
            //  {
            //       res.Bind(1, "true");
            //       res.Bind(2, ViewModel.SelectedItem.id);
            //        res.Step();
            //    }
            //   }
            try
            {
                var dc = (sender as FrameworkElement).DataContext;
                var listitem = ToDoListView.ContainerFromItem(dc) as ListViewItem;
                var item = listitem.Content as TodoItem;

                string sql = @"UPDATE Todo SET finish = ? WHERE Id = ?";
                using (var res = conn.Prepare(sql))
                {
                    res.Bind(1, "true");
                    res.Bind(2, item.id);
                    res.Step();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            // if (ViewModel.SelectedItem != null)
            //      {

            //
            //     this.ViewModel.SelectedItem.completed = true;
            //this.ViewModel.SelectedItem = null;
            //   if (Window.Current.Bounds.Width < 800)
            //   Frame.Navigate(typeof(NewPage), ViewModel);
            //  else
            //  Frame.Navigate(typeof(MainPage), ViewModel);


            //        }

        }

        private void CheckBox2_unchecked(object sender, RoutedEventArgs e)
        {
            var dc = (sender as FrameworkElement).DataContext;
            var item = ToDoListView.ContainerFromItem(dc) as ListViewItem;
            ViewModel.SelectedItem = item.Content as TodoItem;
            if (ViewModel.SelectedItem != null)
            {
                string sql = @"UPDATE Todo SET finish = ? WHERE Id = ?";
                using (var res = conn.Prepare(sql))
                {
                    res.Bind(1, "false");
                    res.Bind(2, ViewModel.SelectedItem.id);
                    res.Step();
                }
                //
                //  if (ViewModel.SelectedItem != null)
                //   {
                //     this.ViewModel.SelectedItem.completed = false;
                //this.ViewModel.SelectedItem = null;
                // if (Window.Current.Bounds.Width < 800)
                //        Frame.Navigate(typeof(NewPage), ViewModel);
                //   else
                //   Frame.Navigate(typeof(MainPage), ViewModel);


                //      }
            }
        }


        private string sharetitle = "", sharedescription = "";
        private StorageFile shareimg;
        private async void share_click(object sender, RoutedEventArgs e)
        {

            var dc = (sender as FrameworkElement).DataContext;
            var item = (ToDoListView.ContainerFromItem(dc) as ListViewItem).Content as TodoItem;
            sharetitle = item.title;
            sharedescription = "Todo's description: " + item.description;

            shareimg = await Package.Current.InstalledLocation.GetFileAsync("Assets\\2015071504.jpg");
            DataTransferManager.ShowShareUI();
        }
        private void DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            Debug.WriteLine(e.Request.ToString());
            DataRequest request = e.Request;
            DataPackage requestData = request.Data;
            requestData.Properties.Title = sharetitle;
            requestData.SetText(sharedescription);

            // Because we are making async calls in the DataRequested event handler,
            //  we need to get the deferral first.
            DataRequestDeferral deferral = request.GetDeferral();

            // Make sure we always call Complete on the deferral.
            try
            {
                requestData.SetBitmap(RandomAccessStreamReference.CreateFromFile(shareimg));
            }
            finally
            {
                deferral.Complete();
            }
        }
        private ObservableCollection<Models.TodoItem> search_items = new ObservableCollection<Models.TodoItem>();
        private ObservableCollection<Models.TodoItem> Search_items { get { return this.search_items; } }

        private async void search_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var text = args.QueryText.Trim();
            if (text == "")
                return;
            string result = "";
            try
            {
                var sql = "SELECT Date, Title, Detail FROM Todo WHERE Date LIKE ? OR Title LIKE ? OR Detail LIKE ?";
                using (var statement = conn.Prepare(sql))
                {
                    statement.Bind(1, "%%" + text + "%%");
                    statement.Bind(2, "%%" + text + "%%");
                    statement.Bind(3, "%%" + text + "%%");
                    while (SQLiteResult.ROW == statement.Step())
                    {
                        var date = statement[0].ToString();
                        date = date.Substring(0, date.IndexOf(' '));
                        string title = statement[1] as string;
                        string detail = statement[2] as string;
                        result += "Title: " + title + ";\nDetail: " + detail + ";\nDate: " + statement[0].ToString() + "\n\n";
                    }
                    if (result == "")
                        result = "No result!\n";
                    await new MessageDialog(result).ShowAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void tile()
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

        private void ShowToastNotification()
        {
            for (int i = 0; i < ViewModel.AllItems.Count; i++)
            {
                if (ViewModel.AllItems[i].date.Day == DateTime.Now.Day)
                {
                    ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText01;
                    Windows.Data.Xml.Dom.XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
                    Windows.Data.Xml.Dom.XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
                    toastTextElements[0].AppendChild(toastXml.CreateTextNode(ViewModel.AllItems[i].title + "\n" + ViewModel.AllItems[i].description));
                    Windows.Data.Xml.Dom.XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");
                    IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
                    ((Windows.Data.Xml.Dom.XmlElement)toastImageAttributes[0]).SetAttribute("src", "Assets//2015071504.jpg");
                    Windows.Data.Xml.Dom.XmlElement audio = toastXml.CreateElement("audio");
                    ((Windows.Data.Xml.Dom.XmlElement)toastNode).SetAttribute("duration", "long");
                    audio.SetAttribute("silent", "true");
                    ToastNotification toast = new ToastNotification(toastXml);
                    ToastNotificationManager.CreateToastNotifier().Show(toast);
                }
            }
        }

    }
}
