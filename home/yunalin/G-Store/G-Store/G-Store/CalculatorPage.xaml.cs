using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

namespace G_Store
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CalculatorPage : Page
    {
        public CalculatorPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // TODO
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
        private void CalcButton_Click(object sender, RoutedEventArgs e)
        {

            if (result_box.Text != "")
            {
                result_box.Text = "";
                curr_box.Text = "";
            }
            Button b = (Button)sender;
            curr_box.FontSize = 20;
            curr_box.Text += b.Content;

        }
        private async void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            //result_box.Text += b.Content;
            String str = curr_box.Text.ToString();
            if (str == "")
            {
                var dialog = new MessageDialog("输入为空！请重新输入");
                var message = await dialog.ShowAsync();

                return;
            }
            String op1, op2, result;
            op1 = "";
            op2 = "";
            result = "";
            double result_double = 0;
            char cal = '\0';
            int i;
            for (i = 0; i < str.Length; i++)
            {
                if (str[i] == '+' || str[i] == '-' || str[i] == '*' || str[i] == '/')
                    break;
                op1 += str[i];
            }
            if (i == str.Length)
            {

                result_box.Text = op1;

                return;
            }
            cal = str[i];
            i++;
            for (; i < str.Length; i++)
            {
                op2 += str[i];
            }
            if (op1 == "" && op2 == "")
            {
                var dialog = new MessageDialog("输入参数错误！请重新输入");
                var message = await dialog.ShowAsync();


                result_box.Text = "";
                curr_box.Text = "";
                return;
            }
            else if (op1 == "")
            {
                if (cal == '+' || cal == '-')
                {
                    result_box.Text += cal;
                    result_box.Text += op2;
                }
                else
                {
                    var dialog = new MessageDialog("输入参数错误！请重新输入");
                    var message = await dialog.ShowAsync();


                    result_box.Text = "";
                    curr_box.Text = "";
                }
                return;
            }
            else if (op2 == "")
            {
                var dialog = new MessageDialog("输入参数错误！请重新输入");
                var message = await dialog.ShowAsync();


                result_box.Text = "";
                curr_box.Text = "";
                return;
            }

            double op1_double;
            double op2_double;
            op1_double = Convert.ToDouble(op1);
            op2_double = Convert.ToDouble(op2);
            switch (cal)
            {
                case '+':
                    result_double = op1_double + op2_double;
                    break;
                case '-':
                    result_double = op1_double - op2_double;
                    break;
                case '*':
                    result_double = op1_double * op2_double;
                    break;
                case '/':
                    result_double = op1_double / op2_double;
                    break;
            }
            result = result_double.ToString();
            result_box.Text = result;
        }
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            result_box.Text = "";
            curr_box.Text = "";

        }
    }
}