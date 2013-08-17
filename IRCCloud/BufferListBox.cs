using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace IRCCloud
{
    public class BufferListBox : ListBox
    {
        public void ScrollToBottom()
        {
            if (Items.Count > 0)
            {
                UpdateLayout();
                ScrollIntoView(Items.Last());
                UpdateLayout();
            }
        }
    }
}
