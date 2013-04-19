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
using WebSocket4Net;
using SuperSocket.ClientEngine;
using System.Text;
using Newtonsoft.Json.Linq;

namespace IRCCloud
{
    public partial class MainPage : PhoneApplicationPage
    {
        private WebSocket _websocket;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            if (IsolatedStorageSettings.ApplicationSettings.Contains("UserName")) {
                UserNameBox.Text = (String) IsolatedStorageSettings.ApplicationSettings["UserName"];
            }

            if (IsolatedStorageSettings.ApplicationSettings.Contains("Password"))
            {
                PasswordBox.Password = (String) IsolatedStorageSettings.ApplicationSettings["Password"];
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
            Debug.WriteLine("completed");
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
                    OpenWebSocket(session);
                }
            }
        }

        private void OpenWebSocket(String session)
        {
            List<KeyValuePair<string, string>> cookies = new List<KeyValuePair<string, string>>();
            cookies.Add(new KeyValuePair<string, string>("session", session));

            Debug.WriteLine(session);

            _websocket = new WebSocket("wss://www.irccloud.com", string.Empty, cookies, null, string.Empty, string.Empty, WebSocketVersion.None);
            _websocket.AutoSendPingInterval = 15;
            _websocket.Opened += new EventHandler(websocket_Opened);
            _websocket.Error += new EventHandler<ErrorEventArgs>(websocket_Error);
            _websocket.Closed += new EventHandler(websocket_Closed);
            _websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
            _websocket.Open();
        }

        private void websocket_Opened(object sender, EventArgs e)
        {
            Debug.WriteLine("Socket opened");
        }

        private void websocket_Error(object sender, ErrorEventArgs e)
        {
            Debug.WriteLine(e.Exception);
        }

        private void websocket_Closed(object sender, EventArgs e)
        {
            Debug.WriteLine("Socket closed");
        }

        private void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Debug.WriteLine(e.Message);
        }
    }
}