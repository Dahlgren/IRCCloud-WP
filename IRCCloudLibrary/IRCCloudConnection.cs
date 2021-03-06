﻿using System.Text;
using Newtonsoft.Json;
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
using System.Net.Http;

namespace IRCCloudLibrary
{
    public class IRCCloudConnection
    {
        public Dictionary<int, Server> Servers { get; private set; }
        public event EventHandler OnServersUpdate;

        private Queue<JObject> _msgQueue = new Queue<JObject>();
        private Boolean _oobLoaded = false;
        private int _requestId = 0;
        private long _lastEventId = 0;
        private String _session;
        private WebSocket _websocket;
        private String _username;
        private String _password;

        public IRCCloudConnection()
        {
            
        }

        public void Connect(String session)
        {
            this._session = session;
            Servers = new Dictionary<int, Server>();

            Connect(0);
        }

        private void Connect(long lastEventId)
        {
            _oobLoaded = false;

            List<KeyValuePair<string, string>> cookies = new List<KeyValuePair<string, string>>();
            cookies.Add(new KeyValuePair<string, string>("session", _session));

            var url = "wss://api.irccloud.com";

            if (lastEventId > 0)
            {
                url += "?since_id=" + lastEventId;
            }

            _websocket = new WebSocket(url, string.Empty, cookies, null, string.Empty, "https://api.irccloud.com", WebSocketVersion.Rfc6455);
            _websocket.Opened += new EventHandler(websocket_Opened);
            _websocket.Closed += new EventHandler(websocket_Closed);
            _websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
            _websocket.Open();
        }

        public void SendMessage(String text, Buffer buffer)
        {
            Dictionary<string, object> message = new Dictionary<string, object>{
                { "_reqid", ++_requestId },
                { "_method", "say" },
                { "cid", buffer.Server.Id },
                { "to", buffer.Name },
                { "msg", text }
            };

            String json = JsonConvert.SerializeObject(message);

            if (_websocket.State == WebSocketState.Open)
            {
                _websocket.Send(json);
            }
        }

        private void websocket_Opened(object sender, EventArgs e)
        {
            Debug.WriteLine("Socket opened");
        }

        private void websocket_Closed(object sender, EventArgs e)
        {
            Debug.WriteLine("Socket closed");

            if (_session != null && _lastEventId > 0)
            {
                Connect(_lastEventId);
            }
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

        private async void FetchOOB(String url)
        {
            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }

            var webClient = new HttpClient(handler);
            webClient.DefaultRequestHeaders.Add("Cookie", "session=" + _session);
            var response = await webClient.GetAsync("https://www.irccloud.com" + url);

            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine(response.ReasonPhrase);
            }
            else
            {
                var body = await response.Content.ReadAsStringAsync();
                JArray oobArr = JArray.Parse(body);

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    foreach (JObject o in oobArr.Values<JObject>())
                    {
                        ProcessMessage(o);
                    }

                    ProcessMessageQueue();
                    _oobLoaded = true;
                });
            }
        }

        public async void Login(String username, String password)
        {
            _username = username;
            _password = password;

            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }

            var webClient = new HttpClient(handler);
            var response = await webClient.PostAsync("https://www.irccloud.com/chat/auth-formtoken", new StringContent(""));

            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine(response.ReasonPhrase);
            }
            else
            {
                var body = await response.Content.ReadAsStringAsync();
                JObject o = JObject.Parse(body);

                if ((bool)o["success"])
                {
                    String token = (string)o["token"];

                    var content = new FormUrlEncodedContent(new[] 
                    {
                        new KeyValuePair<string, string>("email", _username),
                        new KeyValuePair<string, string>("password", _password),
                        new KeyValuePair<string, string>("token", token),
                    });

                    webClient.DefaultRequestHeaders.Add("x-auth-formtoken", token);
                    response = await webClient.PostAsync("https://www.irccloud.com/chat/login", content);
                    LoginCompleted(response);
                }
            }
        }

        private async void LoginCompleted(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                if (LoginEventHandler != null)
                {
                    var loginEvent = new LoginEventArgs();
                    loginEvent.Error = new System.Exception(response.ReasonPhrase);
                    LoginEventHandler(this, loginEvent);
                }
            }
            else
            {
                var body = await response.Content.ReadAsStringAsync();
                JObject o = JObject.Parse(body);

                if ((bool)o["success"])
                {
                    String session = (string)o["session"];

                    if (LoginEventHandler != null)
                    {
                        var loginEvent = new LoginEventArgs();
                        loginEvent.Session = session;
                        LoginEventHandler(this, loginEvent);
                    }
                }
            }
        }

        private void ProcessMessage(JObject o)
        {
            try
            {
                switch (o["type"].ToString())
                {
                    case "makeserver":
                        Server server = new Server()
                        {
                            Id = (int)o["cid"],
                            Name = (string)o["name"],
                            Nick = (string)o["name"]
                        };

                        if (Servers.ContainsKey(server.Id) && Servers[server.Id] != null)
                        {
                            var existingServer = Servers[server.Id];

                            existingServer.Name = server.Name;
                            existingServer.Nick = server.Nick;
                        }
                        else
                        {
                            Servers[server.Id] = server;
                        }

                        OnServersUpdate(this, EventArgs.Empty);
                        break;
                    case "makebuffer":
                        Servers[(int)o["cid"]].AddBuffer(new Buffer()
                        {
                            Id = (int)o["bid"],
                            Server = Servers[(int)o["cid"]],
                            Name = (string)o["name"],
                            Type = (string)o["buffer_type"],
                            Archived = (bool)o["archived"]
                        });
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
                        Servers[(int)o["cid"]].Buffers[(int)o["bid"]].AddMessage(new Message()
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

                if (o["eid"] != null) {
                    long eventId = (long)o["eid"];

                    if (eventId > _lastEventId)
                    {
                        _lastEventId = eventId;
                    }
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

        public event EventHandler<LoginEventArgs> LoginEventHandler;
    }

    public class LoginEventArgs : EventArgs
    {
        public System.Exception Error { get; set; }
        public String Session { get; set; }
    }
}
