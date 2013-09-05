using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;

namespace IRCCloud
{
    class Settings
    {
        private const string RunUnderLockScreenKey = "RunUnderLockScreen";
        private const string PushNotificationsKey = "PushNotifications";
        private const string SessionKey = "Session";
        private const string UserNameKey = "UserName";

        internal static bool GetRunUnderLockScreen()
        {
            return IsolatedStorageSettings.ApplicationSettings.Contains(RunUnderLockScreenKey) && (bool) IsolatedStorageSettings.ApplicationSettings[RunUnderLockScreenKey];
        }

        internal static void SetRunUnderLockScreen(bool runUnderLockScreen)
        {
            IsolatedStorageSettings.ApplicationSettings[RunUnderLockScreenKey] = runUnderLockScreen;
        }

        internal static bool GetPushNotifications()
        {
            return IsolatedStorageSettings.ApplicationSettings.Contains(PushNotificationsKey) && (bool)IsolatedStorageSettings.ApplicationSettings[PushNotificationsKey];
        }

        internal static void SetPushNotifications(bool pushNotifications)
        {
            IsolatedStorageSettings.ApplicationSettings[PushNotificationsKey] = pushNotifications;
        }
        internal static string GetSession()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(SessionKey))
            {
                return (String)IsolatedStorageSettings.ApplicationSettings[SessionKey];
            }

            return null;
        }

        internal static void SetSession(string session)
        {
            IsolatedStorageSettings.ApplicationSettings[SessionKey] = session;
        }


        internal static string GetUserName()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(UserNameKey))
            {
                return (String) IsolatedStorageSettings.ApplicationSettings[UserNameKey];
            }

            return null;
        }

        internal static void SetUserName(string userName)
        {
            IsolatedStorageSettings.ApplicationSettings[UserNameKey] = userName;
        }
    }
}
