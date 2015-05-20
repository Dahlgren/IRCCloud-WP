using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace IRCCloud
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            LockScreenToggle.IsChecked = Settings.GetRunUnderLockScreen();
            PushNotificationsToggle.IsChecked = Settings.GetPushNotifications();
            UserMail.Text = Settings.GetUserName();
        }

        private void LockScreenEnabledToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Disabled;
            Settings.SetRunUnderLockScreen(true);
        }

        private void LockScreenEnabledToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.SetRunUnderLockScreen(false);
        }

        private void PushNotificationsToggle_Checked(object sender, RoutedEventArgs e)
        {
            ((App)App.Current).PushNotifications.Register();
            Settings.SetPushNotifications(true);
        }

        private void PushNotificationsToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            ((App)App.Current).PushNotifications.Unregister();
            Settings.SetPushNotifications(false);
        }

        private void LogutButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.SetPushNotifications(false);
            Settings.SetSession(null);
            Settings.SetUserName(null);

            NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
        }

        private void SendFeedbackButton_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "Irkki Feedback";
            emailComposeTask.Body = "Write Feedback Here";
            emailComposeTask.To = "irkki@dahlgren.at";

            emailComposeTask.Show();
        }
    }
}