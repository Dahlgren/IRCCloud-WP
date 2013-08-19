using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace IRCCloud
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            LockScreenToggle.IsChecked = Settings.GetRunUnderLockScreen();
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
    }
}