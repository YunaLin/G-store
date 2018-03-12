using Windows.System;
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
using System;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Windows.UI.Core;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace G_Store
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class QueryId : Page
    {
        public QueryId()
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
      
        private void Json_Click(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e)
        {
            location.Text = "";
            sex.Text = "";
            birthdate.Text = "";
            queryAsyncJson(number.Text);
        }
        async void queryAsyncJson(string id)
        {
            string url = "http://api.k780.com:88/?app=idcard.get&idcard=" + id + "&appkey=10003&sign=b59bc3ef6191eb9f747dd4e83c99f2a4&format=json";
            HttpClient client = new HttpClient();
            //发送GET请求
            HttpResponseMessage response = await client.GetAsync(url);

            // 确保返回值为成功状态
            response.EnsureSuccessStatusCode();

            // 返回的字节流中含有中文，需要进行编码才可正常显示
            Byte[] getByte = await response.Content.ReadAsByteArrayAsync();

            // 采用UTF-8进行编码
            Encoding code = Encoding.GetEncoding("UTF-8");
            string result = code.GetString(getByte, 0, getByte.Length);

            // 反序列化结果字符串
            JObject res = (JObject)JsonConvert.DeserializeObject(result);

            if (res["success"].ToString() != "1")
            {
                var j = new MessageDialog("身份证有误").ShowAsync();
            }

            if (res["result"] != null)
            {
                location.Text = res["result"]["att"].ToString();
                sex.Text = res["result"]["sex"].ToString();
                birthdate.Text = res["result"]["born"].ToString();
            }
        }
    
    }
}
