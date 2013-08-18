using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using IRCCloudLibrary;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Text.RegularExpressions;
using Microsoft.Phone.Tasks;

namespace IRCCloud
{
    public partial class BufferPage : PhoneApplicationPage
    {
        public IRCCloudLibrary.Buffer Buffer { get; private set; }

        public BufferPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            String serverId;
            String bufferId;

            if (NavigationContext.QueryString.TryGetValue("Server", out serverId))
            {
                Server server = ((App)App.Current).Connection.Servers[int.Parse(serverId)];

                if (NavigationContext.QueryString.TryGetValue("Buffer", out bufferId))
                {
                    Buffer = server.Buffers[int.Parse(bufferId)];
                    BufferTitle.Text = Buffer.Name;
                    ListBox.ItemsSource = Buffer.Messages;
                    ListBox.ScrollToBottom();
                    Buffer.Messages.CollectionChanged += this.Messages_CollectionChanged;
                }
                
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            Buffer.Messages.CollectionChanged -= this.Messages_CollectionChanged;
        }

        void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && ListBox.ScrollAtBottom)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    ListBox.ScrollToBottom();
                });
            }
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ((App)App.Current).Connection.SendMessage(InputBox.Text, Buffer);
                InputBox.Text = "";
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBox.SelectedItem != null)
            {
                Message msg = (Message)((ListBox)sender).SelectedItem;
                var listBoxItem = ListBox.ItemContainerGenerator.ContainerFromIndex(ListBox.SelectedIndex);
                List<string> urls = GetLinks(msg.Msg);
                if (urls.Count > 0)
                {
                    var contextMenu = ContextMenuService.GetContextMenu(listBoxItem);

                    if (contextMenu == null)
                    {
                        contextMenu = new ContextMenu();
                    }
                    else
                    {
                        contextMenu.Items.Clear();
                    }

                    foreach (string url in urls)
                    {
                        var menuItem = new MenuItem() {
                            Header = url
                        };
                        menuItem.Click += (o, args) =>
                        {
                            WebBrowserTask wbt = new WebBrowserTask();
                            wbt.URL = url;
                            wbt.Show();
                        };
                        contextMenu.Items.Add(menuItem);
                    }

                    ContextMenuService.SetContextMenu(listBoxItem, contextMenu);
                    contextMenu.IsOpen = true;
                }
                ListBox.SelectedItem = null;
            }
        }

        private List<string> GetLinks(string message)
        {
            List<string> list = new List<string>();
            Regex urlRx = new Regex(@"((https?|ftp|file)\://|www.)[A-Za-z0-9\.\-]+(/[A-Za-z0-9\?\&\=;\+!'\(\)\*\-\._~%]*)*", RegexOptions.IgnoreCase);

            MatchCollection matches = urlRx.Matches(message);
            foreach (Match match in matches)
            {
                list.Add(match.Value);
            }
            return list;
        }
    }
}