using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json.Linq;

namespace IRCCloud
{
    public partial class LoginPage : PhoneApplicationPage
    {
        // Constructor
        public LoginPage()
        {
            InitializeComponent();

            if (IsolatedStorageSettings.ApplicationSettings.Contains("UserName")) {
                UserNameBox.Text = (String) IsolatedStorageSettings.ApplicationSettings["UserName"];
            }

            if (IsolatedStorageSettings.ApplicationSettings.Contains("Password"))
            {
                PasswordBox.Password = (String) IsolatedStorageSettings.ApplicationSettings["Password"];
            }

            if (UserNameBox.Text.Length > 0 && PasswordBox.Password.Length > 0)
            {
                Login(UserNameBox.Text, PasswordBox.Password);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var username = UserNameBox.Text;
            var password = PasswordBox.Password;

            IsolatedStorageSettings.ApplicationSettings["UserName"] = username;
            IsolatedStorageSettings.ApplicationSettings["Password"] = password;

            Login(username, password);
        }

        private void Login(String username, String password)
        {
            LoginButton.IsEnabled = false;

            WebClient webClient = new WebClient();

            StringBuilder postData = new StringBuilder();
            postData.AppendFormat("{0}={1}", "email", HttpUtility.UrlEncode(username));
            postData.AppendFormat("&{0}={1}", "password", HttpUtility.UrlEncode(password));

            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            webClient.Headers[HttpRequestHeader.ContentLength] = postData.Length.ToString();
            webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(webClient_UploadStringCompleted);
            webClient.UploadStringAsync(new Uri("https://www.irccloud.com/chat/login", UriKind.Absolute), "POST", postData.ToString());
        }

        void webClient_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            LoginButton.IsEnabled = true;

            if (e.Error != null)
            {
                Debug.WriteLine(e.Error);
            }
            else
            {
                Debug.WriteLine(e.Result);
                JObject o = JObject.Parse(e.Result);

                if ((bool)o["success"])
                {
                    String session = (string)o["session"];
                    ((App)App.Current).Connection.Connect(session);
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                }
            }
        }

        
    }
}