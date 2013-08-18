using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace IRCCloud
{
    public class BufferListBox : ListBox
    {
        public bool ScrollingToBottom { get; private set; }

        public void ScrollToBottom()
        {
            if (Items.Count > 0 && SelectedIndex < 0)
            {
                ScrollingToBottom = true;
                UpdateLayout();
                SelectedIndex = Items.Count-1;
                SelectedIndex = -1;
                ScrollingToBottom = false;
            }
        }
    }
}
