using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Net.Http;
using System.Diagnostics;
using Newtonsoft.Json;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.ObjectModel;
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;

namespace G_Store
{
    public sealed partial class QueryPage : Page
    {
        public QueryPage() {
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

        private async void queryPhone(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (queryphone.Text == "")
            {
                await new MessageDialog("手机号码不能为空！").ShowAsync();
                return;
            }
            Uri requestUri = new Uri(@"http://opendata.baidu.com/api.php?query=" + queryphone.Text.Trim() + "&co=&resource_id=6004&t=1460125093359&ie=utf8&oe=gbk&cb=op_aladdin_callback&format=json&tn=baidu&cb=jQuery11020106542830829915_1460112569047&_=1460112569072");
            try
            {
                //Send the GET request
                Windows.Web.Http.HttpClient httpclient = new Windows.Web.Http.HttpClient();
                var httpResponse = await httpclient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                var httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
                int start = httpResponseBody.IndexOf('{'), end = httpResponseBody.LastIndexOf(')');
                httpResponseBody = httpResponseBody.Substring(start, end - start);
                var body = new Dictionary<string, object>();
                body = JsonConvert.DeserializeObject<Dictionary<string, object>>(httpResponseBody);
                var arr = body["data"] as Newtonsoft.Json.Linq.JArray;
                try
                {
                    var obj = arr[0];
                    position.Text = obj["prov"] + " " + obj["city"];
                    phonetype.Text = obj["type"].ToString();
                    phonenum.Text = queryphone.Text.Trim();
                    phone_detail.Visibility = Visibility.Visible;
                }
                catch (Exception)
                {
                    queryphone.Text = "";
                    phone_detail.Visibility = Visibility.Collapsed;
                    await new MessageDialog("该号码不存在！请重新输入").ShowAsync();
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
     

    /*    private void check_phone(object sender, RoutedEventArgs e)
        {
            queryphone.Text = "";
            queryphone.Visibility = Visibility.Visible;
            queryphone.Focus(FocusState.Programmatic);
        }*/

        private void check_id(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(QueryId));
        }


        private void check_weather(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(QueryWeather));
        }
    }

}