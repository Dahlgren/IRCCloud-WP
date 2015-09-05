using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace IRCCloud
{
    public abstract class DataTemplateSelector : ContentControl
    {
        public virtual DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return null;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            ContentTemplate = SelectTemplate(newContent, this);
        }
    }

    public class BufferTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Active
        {
            get;
            set;
        }

        public DataTemplate Archived
        {
            get;
            set;
        }

        public DataTemplate Console
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            IRCCloudLibrary.Buffer bufferItem = item as IRCCloudLibrary.Buffer;
            if (bufferItem != null)
            {
                if (bufferItem.Name == "*")
                {
                    return Console;
                }
                else if (bufferItem.Archived)
                {
                    return Archived;
                }
                else
                {
                    return Active;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
