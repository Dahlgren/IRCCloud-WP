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

namespace IRCCloud
{
    public partial class ChannelPage : PhoneApplicationPage
    {
        public Channel Channel { get; private set; }

        public ChannelPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            String serverId;
            String channelName;

            if (NavigationContext.QueryString.TryGetValue("Server", out serverId))
            {
                Server server = ((App)App.Current).Connection.Servers[int.Parse(serverId)];

                if (NavigationContext.QueryString.TryGetValue("Channel", out channelName))
                {
                    Channel = server.Channels[channelName];
                    ChannelTitle.Text = Channel.Name;
                    ListBox.ItemsSource = Channel.Buffer.Messages;
                    ListBox.ScrollToBottom();
                    Channel.Buffer.Messages.CollectionChanged += this.Messages_CollectionChanged;
                }
                
            }
        }

        void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
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
                ((App)App.Current).Connection.SendMessage(InputBox.Text, Channel.Buffer);
                InputBox.Text = "";
            }
        }

    }
}