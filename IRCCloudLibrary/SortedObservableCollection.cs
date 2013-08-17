using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace IRCCloudLibrary
{
    public class SortedObservableCollection<T> : ObservableCollection<T> where T : IComparable
    {
        protected override void InsertItem(int index, T item)
        {
            index = Array.BinarySearch<T>(this.Items.ToArray<T>(), item);
            if (index >= 0) throw new ArgumentException("Cannot insert duplicated items");
            else base.InsertItem(~index, item);
        }
    }
}
