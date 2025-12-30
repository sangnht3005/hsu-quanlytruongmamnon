using System.Windows;
using System.Windows.Controls;

namespace KindergartenManagement.Utilities
{
    public static class DataGridBehaviors
    {
        public static readonly DependencyProperty EnableRowNumberProperty =
            DependencyProperty.RegisterAttached(
                "EnableRowNumber",
                typeof(bool),
                typeof(DataGridBehaviors),
                new PropertyMetadata(false, OnEnableRowNumberChanged));

        public static void SetEnableRowNumber(DependencyObject element, bool value) => element.SetValue(EnableRowNumberProperty, value);
        public static bool GetEnableRowNumber(DependencyObject element) => (bool)element.GetValue(EnableRowNumberProperty);

        private static void OnEnableRowNumberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not DataGrid grid)
                return;

            grid.LoadingRow -= GridOnLoadingRow;
            if ((bool)e.NewValue)
            {
                grid.LoadingRow += GridOnLoadingRow;
                UpdateExistingRows(grid);
            }
        }

        private static void GridOnLoadingRow(object? sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private static void UpdateExistingRows(DataGrid grid)
        {
            foreach (var item in grid.Items)
            {
                var row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (row != null)
                {
                    row.Header = (row.GetIndex() + 1).ToString();
                }
            }
        }
    }
}
