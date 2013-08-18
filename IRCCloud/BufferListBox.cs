using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace IRCCloud
{
    public class BufferListBox : ListBox
    {
        public bool ScrollAtBottom { get; set; }
        private ScrollViewer ScrollViewer { get; set; }

        private readonly DependencyProperty ListVerticalOffsetProperty = DependencyProperty.Register("ListVerticalOffset",
        typeof(double), typeof(BufferListBox), new PropertyMetadata(new PropertyChangedCallback(OnListVerticalOffsetChanged)));

        public double ListVerticalOffset
        {
            get { return (double)this.GetValue(ListVerticalOffsetProperty); }
            set { this.SetValue(ListVerticalOffsetProperty, value); }
        }

        public BufferListBox() : base()
        {
            this.Loaded += BufferListBox_Loaded;
        }

        void BufferListBox_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ScrollViewer = this.Descendents().OfType<ScrollViewer>().FirstOrDefault();
            ScrollToBottom();

            Binding binding = new Binding();
            binding.Source = ScrollViewer;
            binding.Path = new PropertyPath("VerticalOffset");
            binding.Mode = BindingMode.OneWay;
            this.SetBinding(ListVerticalOffsetProperty, binding);
        }

        public void ScrollToBottom()
        {
            if (Items.Count > 0 && SelectedIndex < 0 && ScrollViewer != null)
            {
                UpdateLayout();
                ScrollViewer.ScrollToVerticalOffset(ScrollViewer.ExtentHeight);
            }
        }

        private static void OnListVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BufferListBox listBox = d as BufferListBox;

            // Checks if the Scroll is at bottom
            listBox.ScrollAtBottom = listBox.ScrollViewer.VerticalOffset + listBox.ScrollViewer.ViewportHeight >= listBox.ScrollViewer.ExtentHeight;
        }
    }
}
