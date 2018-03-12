using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace G_Store
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PlayerPage : Page
    {
        public PlayerPage()
        {
            this.InitializeComponent();
            image1.Stretch = Stretch.Fill;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    Windows.UI.Core.AppViewBackButtonVisibility.Visible;
                Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += (s, ee) =>
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
                Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }



        }
        private void Play_Clicked(object sender, RoutedEventArgs e)
        {
            myMediaElement.Play();
            InitializePropertyValues();
        }
        private void Pause_Clicked(object sender, RoutedEventArgs e)
        {
            myMediaElement.Pause();
        }
        private void Stop_Clicked(object sender, RoutedEventArgs e)
        {
            myMediaElement.Stop();
        }
        private void Zoom_Clicked(object sender, RoutedEventArgs e)
        {
            if (myMediaElement.IsFullWindow == false)
            {
                myMediaElement.IsFullWindow = true;
            }
            else
            {
                myMediaElement.IsFullWindow = false;
            }

        }
        private async void Select_Clicked(object sender, RoutedEventArgs e)
        {
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
            openPicker.SuggestedStartLocation =
            Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.FileTypeFilter.Add(".mkv");
            openPicker.FileTypeFilter.Add(".mp3");
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".wma");
            openPicker.FileTypeFilter.Add(".avi");
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                myMediaElement.SetSource(stream, file.ContentType);
            }
        }
        private void ChangeVolume(object sender, RangeBaseValueChangedEventArgs e)
        {
            myMediaElement.Volume = slider1.Value;
        }

        private void myMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            slider.Maximum = myMediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
        }

        private void myMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            myMediaElement.Stop();
        }

        private void slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            int SliderValue = (int)slider.Value;
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, SliderValue);
            myMediaElement.Position = ts;
        }
        void InitializePropertyValues()
        {
            myMediaElement.Volume = (double)slider1.Value;
        }
    }
}
