using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WebSocket4Net;

namespace IRCCLoudLibrary
{
    public class IRCCloudConnection
    {
        private WebSocket _websocket;

        public void Connect(String session)
        {
            List<KeyValuePair<string, string>> cookies = new List<KeyValuePair<string, string>>();
            cookies.Add(new KeyValuePair<string, string>("session", session));

            _websocket = new WebSocket("wss://www.irccloud.com", string.Empty, cookies, null, string.Empty, "https://www.irccloud.com", WebSocketVersion.Rfc6455);
            _websocket.Opened += new EventHandler(websocket_Opened);
            _websocket.Closed += new EventHandler(websocket_Closed);
            _websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
            _websocket.Open();
        }

        private void websocket_Opened(object sender, EventArgs e)
        {
            Debug.WriteLine("Socket opened");
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
