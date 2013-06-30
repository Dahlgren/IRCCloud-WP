﻿using System;
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
    }
}