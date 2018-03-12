using System;
using System.Collections.Generic;
using System.IO;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace G_Store
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
            updatetile_Create();
        }
        private void updatetile_Create()
        {
            String tile_ = File.ReadAllText("tiles.xml");
            // var n = new MessageDialog(tile_).ShowAsync();
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(tile_);
            XmlNodeList Texttitle = xml.GetElementsByTagName("text");
            XmlNodeList Image = xml.GetElementsByTagName("image");
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);

            Texttitle[0].InnerText = Texttitle[2].InnerText = Texttitle[1].InnerText = text1.Text;


            TileNotification new_tile = new TileNotification(xml);

            TileUpdateManager.CreateTileUpdaterForApplication().Update(new_tile);

            Texttitle[0].InnerText = Texttitle[2].InnerText = Texttitle[1].InnerText = text2.Text;


            new_tile = new TileNotification(xml);

            TileUpdateManager.CreateTileUpdaterForApplication().Update(new_tile);
            Texttitle[0].InnerText = Texttitle[2].InnerText = Texttitle[1].InnerText = text3.Text;


            new_tile = new TileNotification(xml);

            TileUpdateManager.CreateTileUpdaterForApplication().Update(new_tile);
            Texttitle[0].InnerText = Texttitle[2].InnerText = Texttitle[1].InnerText = text4.Text;


            new_tile = new TileNotification(xml);

            TileUpdateManager.CreateTileUpdaterForApplication().Update(new_tile);
            Texttitle[0].InnerText = Texttitle[2].InnerText = Texttitle[1].InnerText = text5.Text;


            new_tile = new TileNotification(xml);

            TileUpdateManager.CreateTileUpdaterForApplication().Update(new_tile);

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }
        }

        private void TodoItem_ItemClicked1(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(NotePage));

        }
        private void TodoItem_ItemClicked2(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(CalendarPage));

        }
        private void TodoItem_ItemClicked3(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(CalculatorPage));

        }
        private void TodoItem_ItemClicked4(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(QueryPage));

        }
        private void TodoItem_ItemClicked5(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(PlayerPage));

        }
    }
}