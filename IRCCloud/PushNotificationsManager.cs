using Microsoft.Phone.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace IRCCloud
{
    public class PushNotificationsManager
    {
        private const string ChannelName = "ToastPushNotifications";

        public void Register()
        {
            /// Holds the push channel that is created or found.
            HttpNotificationChannel pushChannel;

            // Try to find the push channel.
            pushChannel = HttpNotificationChannel.Find(ChannelName);

            // If the channel was not found, then create a new connection to the push service.
            if (pushChannel == null)
            {
                pushChannel = new HttpNotificationChannel(ChannelName);

                // Register for all the events before attempting to open the channel.
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                // Register for this notification only if you need to receive the notifications while your application is running.
                pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);
                
                pushChannel.Open();

                // Bind this new channel for toast events.
                pushChannel.BindToShellToast();

            }
            else
            {
                // The channel was already open, so just register for all the events.
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                // Register for this notification only if you need to receive the notifications while your application is running.
                pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                RegisterChannelUriAtBackend(pushChannel.ChannelUri.ToString());
            }
        }

        public void Unregister()
        {
            HttpNotificationChannel pushChannel = HttpNotificationChannel.Find(ChannelName);

            // If the channel was found, close it
            if (pushChannel != null)
            {
                pushChannel.Close();
            }
        }

        void PushChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            RegisterChannelUriAtBackend(e.ChannelUri.ToString());
        }

        void PushChannel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            // Error handling logic for your particular application would be here.
        }

        void PushChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            // Show notification while inside application
        }

        private void RegisterChannelUriAtBackend(string channelUri)
        {
            // Display the URI for testing purposes. Normally, the URI would be passed back to your web service at this point.
            System.Diagnostics.Debug.WriteLine(channelUri);
        }
    }
}
