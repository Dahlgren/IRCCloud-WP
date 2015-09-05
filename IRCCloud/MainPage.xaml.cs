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
using IRCCloud.ViewModels;

namespace IRCCloud
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPageViewModel ViewModel { get; private set; }

        public MainPage()
        {
            ViewModel = new MainPageViewModel(((App)App.Current).Connection);

            InitializeComponent();
            DrawerLayout.InitializeDrawerLayout();

            this.DataContext = ViewModel;

            Loaded += (s, e) =>
            {
                if (NavigationService.CanGoBack)
                {
                    while (NavigationService.RemoveBackEntry() != null)
                    {
                        NavigationService.RemoveBackEntry();
                    }
                }
            };
        }

        private void DrawerIcon_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (DrawerLayout.IsDrawerOpen)
                DrawerLayout.CloseDrawer();
            else
                DrawerLayout.OpenDrawer();
        }

        private void ApplicationBarSettings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }
    }
}
