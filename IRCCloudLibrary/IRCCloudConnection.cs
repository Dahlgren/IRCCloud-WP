using Newtonsoft.Json.Linq;
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

namespace IRCCloudLibrary
{
    public class IRCCloudConnection
    {
        public Dictionary<int, Server> Servers { get; private set; }
        public event EventHandler OnServersUpdate;

        private Queue<JObject> _msgQueue = new Queue<JObject>();
        private Boolean _oobLoaded = false;
        private String _session;
        private WebSocket _websocket;

        public IRCCloudConnection()
        {
            Servers = new Dictionary<int, Server>();
        }

        public void Connect(String session)
        {
            this._session = session;

            List<KeyValuePair<string, string>> cookies = new List<KeyValuePair<string, string>>();
            cookies.Add(new KeyValuePair<string, string>("session", _session));

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
            try
            {
                JObject o = JObject.Parse(e.Message);
                switch (o["type"].ToString())
                {
                    case "oob_include":
                        FetchOOB(o["url"].ToString());
                        break;
                    default:
                        if (_oobLoaded)
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                ProcessMessage(o);
                            });
                        }
                        else
                        {
                            _msgQueue.Enqueue(o);
                        }
                        break;
                }
            }
            catch (Exception exc)
            {
                // ignore empty and invalid messages
            }
        }

        private void FetchOOB(String url)
        {
            WebClient webClient = new WebClient();
            webClient.Headers["Cookie"] = "session=" + _session;
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(oob_DownloadComplete);
            webClient.DownloadStringAsync(new Uri("https://www.irccloud.com" + url, UriKind.Absolute));
        }

        void oob_DownloadComplete(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Debug.WriteLine(e.Error);
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    JArray oobArr = JArray.Parse(e.Result);
                    foreach (JObject o in oobArr.Values<JObject>())
                    {
                        ProcessMessage(o);
                    }

                    ProcessMessageQueue();
                    _oobLoaded = true;
                });
            }
        }

        private void ProcessMessage(JObject o)
        {
            try
            {
                switch (o["type"].ToString())
                {
                    case "makeserver":
                        Servers[(int)o["cid"]] = new Server()
                        {
                            Id = (int)o["cid"],
                            Name = (string)o["name"],
                            Nick = (string)o["name"]
                        };
                        OnServersUpdate(this, EventArgs.Empty);
                        break;
                    case "makebuffer":
                        Servers[(int)o["cid"]].Buffers[(int)o["bid"]] = new Buffer()
                        {
                            Id = (int)o["bid"],
                            Server = Servers[(int)o["cid"]],
                            Name = (string)o["name"]
                        };
                        break;
                    case "channel_init":
                        Servers[(int)o["cid"]].Channels[(string)o["chan"]] = new Channel()
                        {
                            Buffer = Servers[(int)o["cid"]].Buffers[(int)o["bid"]],
                            Server = Servers[(int)o["cid"]],
                            Name = (string)o["chan"],
                            Topic = (string)o["topic"]["text"]
                        };
                        break;
                    case "buffer_msg":
                        Servers[(int)o["cid"]].Buffers[(int)o["bid"]].Messages.Add(new Message()
                        {
                            Buffer = Servers[(int)o["cid"]].Buffers[(int)o["bid"]],
                            Server = Servers[(int)o["cid"]],
                            Msg = (string)o["msg"],
                            Timestamp = (long)o["eid"],
                            User = (string)o["from"]
                        });
                        break;
                    default:
                        break;
                }
            }
            catch (Exception exc)
            {
            }
        }

        private void ProcessMessageQueue()
        {
            while (_msgQueue.Count > 0)
            {
                ProcessMessage(_msgQueue.Dequeue());
            }
        }
    }
}
