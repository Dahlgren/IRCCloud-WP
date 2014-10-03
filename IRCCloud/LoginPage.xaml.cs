using System;
using System.Windows;
using IRCCloudLibrary;
using Microsoft.Phone.Controls;
using System.Diagnostics;

namespace IRCCloud
{
    public partial class LoginPage : PhoneApplicationPage
    {
        private Boolean hasAlreadyAutoLogin;

        // Constructor
        public LoginPage()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                if (NavigationService.CanGoBack)
                {
                    while (NavigationService.RemoveBackEntry() != null)
                    {
                        NavigationService.RemoveBackEntry();
                    }
                }

                if (Settings.GetUserName() != null)
                {
                    UserNameBox.Text = Settings.GetUserName();
                }

                if (Settings.GetSession() != null && !hasAlreadyAutoLogin)
                {
                    hasAlreadyAutoLogin = true;
                    SuccessfulLogin(Settings.GetSession());
                }

                if (((App) App.Current).Connection != null)
                {
                    ((App) App.Current).Connection.LoginEventHandler += LoginEvent;
                }
            };
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var userName = UserNameBox.Text;
            var password = PasswordBox.Password;

            Settings.SetUserName(userName);

            Login(userName, password);
        }

        private void Login(String username, String password)
        {
            LoginButton.IsEnabled = false;

            ((App) App.Current).Connection.Login(username, password);
        }

        void LoginEvent(object sender, LoginEventArgs e)
        {
            LoginButton.IsEnabled = true;

            if (e.Error != null)
            {
                Debug.WriteLine(e.Error);
            }
            else
            {
                Debug.WriteLine(e.Session);
                SuccessfulLogin(e.Session);
            }
        }

        private void SuccessfulLogin(string session)
        {
            Settings.SetSession(session);

            ((App)App.Current).Connection.Connect(session);

            if (Settings.GetPushNotifications())
            {
                ((App)App.Current).PushNotifications.Register();
            }

            if (((App)App.Current).Connection != null)
            {
                ((App)App.Current).Connection.LoginEventHandler -= LoginEvent;
            }

            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}