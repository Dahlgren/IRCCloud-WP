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
using System.IO.IsolatedStorage;

namespace IRCCloud
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            if (IsolatedStorageSettings.ApplicationSettings.Contains("UserName")) {
                UserNameBox.Text = (String) IsolatedStorageSettings.ApplicationSettings["UserName"];
            }

            if (IsolatedStorageSettings.ApplicationSettings.Contains("Password"))
            {
                PasswordBox.Password = (String) IsolatedStorageSettings.ApplicationSettings["Password"];
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["UserName"] = UserNameBox.Text;
            IsolatedStorageSettings.ApplicationSettings["Password"] = PasswordBox.Password;
        }
    }
}