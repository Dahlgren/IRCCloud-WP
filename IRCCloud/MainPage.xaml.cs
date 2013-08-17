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
using System.Collections.ObjectModel;
using IRCCloudLibrary;
using System.Diagnostics;

namespace IRCCloud
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            ((App)App.Current).Connection.OnServersUpdate += connection_serversUpdated;
            connection_serversUpdated(this, EventArgs.Empty);
        }

        void connection_serversUpdated(object sender, EventArgs args)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                Pivot.ItemsSource = ((App)App.Current).Connection.Servers.Values.ToArray();
            });
        }

        private void listBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;

            if (listBox.SelectedItem != null)
            {
                IRCCloudLibrary.Buffer buffer = (IRCCloudLibrary.Buffer) ((ListBox)sender).SelectedItem;
                String query = "Buffer=" + buffer.Id + "&Server=" + buffer.Server.Id;
                NavigationService.Navigate(new Uri("/BufferPage.xaml?" + query, UriKind.Relative));
                listBox.SelectedItem = null;
            }
        }

        private void ApplicationBarSettings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }
    }
}