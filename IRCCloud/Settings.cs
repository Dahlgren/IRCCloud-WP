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
        private const string UserNameKey = "UserName";
        private const string PasswordKey = "Password";

        internal static bool GetRunUnderLockScreen()
        {
            return IsolatedStorageSettings.ApplicationSettings.Contains(RunUnderLockScreenKey) && (bool) IsolatedStorageSettings.ApplicationSettings[RunUnderLockScreenKey];
        }

        internal static void SetRunUnderLockScreen(bool runUnderLockScreen)
        {
            IsolatedStorageSettings.ApplicationSettings[RunUnderLockScreenKey] = runUnderLockScreen;
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

        internal static string GetPassword()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(PasswordKey))
            {
                return (String) IsolatedStorageSettings.ApplicationSettings[PasswordKey];
            }

            return null;
        }

        internal static void SetPassword(string password)
        {
            IsolatedStorageSettings.ApplicationSettings[PasswordKey] = password;
        }
    }
}
