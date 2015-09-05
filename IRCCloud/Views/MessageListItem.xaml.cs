using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Text.RegularExpressions;
using IRCCloudLibrary;
using Microsoft.Phone.Tasks;

namespace IRCCloud.Views
{
    public partial class MessageListItem : UserControl
    {
        public MessageListItem()
        {
            InitializeComponent();
        }

        private List<string> GetLinks(string message)
        {
            List<string> list = new List<string>();
            Regex urlRx = new Regex(@"((https?|ftp|file)\://|www.)[A-Za-z0-9\.\-]+(/[A-Za-z0-9\?\&\=;\+!'\(\)\*\-\._~%]*)*", RegexOptions.IgnoreCase);

            MatchCollection matches = urlRx.Matches(message);
            foreach (Match match in matches)
            {
                list.Add(match.Value);
            }
            return list;
        }

        private void Item_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Message msg = (Message)this.DataContext;
            List<string> urls = GetLinks(msg.Msg);
            if (urls.Count > 0)
            {
                var contextMenu = ContextMenuService.GetContextMenu(this);

                if (contextMenu == null)
                {
                    contextMenu = new ContextMenu();
                }
                else
                {
                    contextMenu.Items.Clear();
                }

                foreach (string url in urls)
                {
                    var menuItem = new MenuItem()
                    {
                        Header = url
                    };
                    menuItem.Click += (o, args) =>
                    {
                        WebBrowserTask wbt = new WebBrowserTask();
                        wbt.Uri = new Uri(url);
                        wbt.Show();
                    };
                    contextMenu.Items.Add(menuItem);
                }

                ContextMenuService.SetContextMenu(this, contextMenu);
                contextMenu.IsOpen = true;
            }
        }
    }
}
