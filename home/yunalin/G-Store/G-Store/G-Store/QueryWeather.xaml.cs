using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Net.Http;
using Windows.Data.Xml.Dom;
using Windows.UI.Popups;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Windows.UI.Core;


// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace G_Store
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class QueryWeather : Page
    {
        public QueryWeather()
        {
            this.InitializeComponent();
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

                        Frame.Navigate(typeof(QueryPage));
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
        private void Button_Click(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e)
        {
            location.Text = "";
            runner.Text = "";
            tomorrow.Text = "";
            tomorrow1.Text = "";
            queryAsync(number.Text);
        }

        async void queryAsync(string tel)
        {
            if (tel == "")
            {
                var i = new MessageDialog("输入的城市不能为空").ShowAsync();
            }
            else
            {
                string url = "http://api.k780.com:88/?app=weather.future&weaid=" + tel + "&appkey=10003&sign=b59bc3ef6191eb9f747dd4e83c99f2a4&format=xml";
                HttpClient client = new HttpClient();
                string result = await client.GetStringAsync(url);
                XmlDocument document = new XmlDocument();
                document.LoadXml(result);
                XmlNodeList list = document.GetElementsByTagName("success");
                IXmlNode node = list.Item(0);
                if (node != null)
                    if (node.InnerText == "0")
                    {
                        var i = new MessageDialog("所输入的城市不存在，请重新输入").ShowAsync();
                        number.Text = "";
                    }
                    else
                    {
                        list = document.GetElementsByTagName("temperature");
                        node = list.Item(0);
                        if (node != null)
                            location.Text = node.InnerText;
                        list = document.GetElementsByTagName("weather");
                        node = list.Item(0);
                        if (node != null)
                            runner.Text = node.InnerText;
                        list = document.GetElementsByTagName("temperature");
                        node = list.Item(1);
                        if (node != null)
                            tomorrow1.Text = node.InnerText;
                        list = document.GetElementsByTagName("weather");
                        node = list.Item(1);
                        if (node != null)
                            tomorrow.Text = node.InnerText;
                    }
            }
        }
      
       
    }
}
