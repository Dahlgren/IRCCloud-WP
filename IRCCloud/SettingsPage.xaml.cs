using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace IRCCloud
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            LockScreenToggle.IsChecked = IsolatedStorageSettings.ApplicationSettings.Contains("RunUnderLockScreen") && (bool)IsolatedStorageSettings.ApplicationSettings["RunUnderLockScreen"];
        }

        private void LockScreenEnabledToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Disabled;
            IsolatedStorageSettings.ApplicationSettings["RunUnderLockScreen"] = true;
        }

        private void LockScreenEnabledToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["RunUnderLockScreen"] = false;
        }
    }
}