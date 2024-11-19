using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Chaotic.Utilities
{
    public class ListViewExtension
    {
        public static readonly DependencyProperty AutoScrollToCurrentItemProperty =
            DependencyProperty.RegisterAttached("AutoScrollToCurrentItem",
                typeof(bool), typeof(ListViewExtension),
                new UIPropertyMetadata(default(bool), OnAutoScrollToCurrentItemChanged));

        public static bool GetAutoScrollToCurrentItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollToCurrentItemProperty);
        }

        public static void OnAutoScrollToCurrentItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var listView = (ListView)obj;
            var newValue = (bool)e.NewValue;

            if (listView == null) return;

            if (newValue)
                listView.SelectionChanged += ListViewSelectionChanged;
            else
                listView.SelectionChanged -= ListViewSelectionChanged;
        }

        static void ListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = (ListView)sender;
            if (listView == null || listView.SelectedItem == null || listView.Items == null) return;

            listView.Items.MoveCurrentTo(listView.SelectedItem);
            listView.ScrollIntoView(listView.SelectedItem);
        }

        public static void SetAutoScrollToCurrentItem(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollToCurrentItemProperty, value);
        }
    }
}
