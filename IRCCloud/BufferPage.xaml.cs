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
                ((App)App.Current).Connection.SendMessage(InputBox.Text, Buffer);
                InputBox.Text = "";
            }
        }

    }
}