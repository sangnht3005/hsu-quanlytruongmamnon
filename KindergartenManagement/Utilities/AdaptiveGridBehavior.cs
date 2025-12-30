using System;
using System.Windows;
using System.Windows.Controls;

namespace KindergartenManagement.Utilities
{
    public static class AdaptiveGridBehavior
    {
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(AdaptiveGridBehavior),
            new PropertyMetadata(false, OnIsEnabledChanged));

        public static readonly DependencyProperty BreakpointProperty = DependencyProperty.RegisterAttached(
            "Breakpoint",
            typeof(double),
            typeof(AdaptiveGridBehavior),
            new PropertyMetadata(1100d));

        public static readonly DependencyProperty StackSpacingProperty = DependencyProperty.RegisterAttached(
            "StackSpacing",
            typeof(double),
            typeof(AdaptiveGridBehavior),
            new PropertyMetadata(16d));

        public static void SetIsEnabled(DependencyObject element, bool value) => element.SetValue(IsEnabledProperty, value);
        public static bool GetIsEnabled(DependencyObject element) => (bool)element.GetValue(IsEnabledProperty);

        public static void SetBreakpoint(DependencyObject element, double value) => element.SetValue(BreakpointProperty, value);
        public static double GetBreakpoint(DependencyObject element) => (double)element.GetValue(BreakpointProperty);

        public static void SetStackSpacing(DependencyObject element, double value) => element.SetValue(StackSpacingProperty, value);
        public static double GetStackSpacing(DependencyObject element) => (double)element.GetValue(StackSpacingProperty);

        private static readonly DependencyProperty OriginalCol0WidthProperty = DependencyProperty.RegisterAttached(
            "OriginalCol0Width",
            typeof(GridLength?),
            typeof(AdaptiveGridBehavior),
            new PropertyMetadata(null));

        private static readonly DependencyProperty OriginalCol1WidthProperty = DependencyProperty.RegisterAttached(
            "OriginalCol1Width",
            typeof(GridLength?),
            typeof(AdaptiveGridBehavior),
            new PropertyMetadata(null));

        private static readonly DependencyProperty OriginalColumnProperty = DependencyProperty.RegisterAttached(
            "OriginalColumn",
            typeof(int?),
            typeof(AdaptiveGridBehavior),
            new PropertyMetadata(null));



        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Grid grid)
                return;

            grid.SizeChanged -= GridOnSizeChanged;

            if ((bool)e.NewValue)
            {
                grid.SizeChanged += GridOnSizeChanged;
                ApplyLayout(grid, grid.ActualWidth);
            }
            else
            {
                RestoreTwoColumns(grid);
            }
        }

        private static void GridOnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is Grid grid)
            {
                ApplyLayout(grid, e.NewSize.Width);
            }
        }

        private static void ApplyLayout(Grid grid, double width)
        {
            double breakpoint = GetBreakpoint(grid);
            if (grid.ColumnDefinitions.Count < 2)
                return;

            if (width <= 0)
                return;

            // Capture original widths once
            if (grid.GetValue(OriginalCol0WidthProperty) == null)
            {
                grid.SetValue(OriginalCol0WidthProperty, grid.ColumnDefinitions[0].Width);
            }
            if (grid.GetValue(OriginalCol1WidthProperty) == null)
            {
                grid.SetValue(OriginalCol1WidthProperty, grid.ColumnDefinitions[1].Width);
            }

            if (width < breakpoint)
            {
                // On narrow screens, shrink col 1 but keep it visible (3:2 ratio)
                grid.ColumnDefinitions[0].Width = new GridLength(3, GridUnitType.Star);
                grid.ColumnDefinitions[1].Width = new GridLength(2, GridUnitType.Star);
            }
            else
            {
                RestoreTwoColumns(grid);
            }
        }

        private static void RestoreTwoColumns(Grid grid)
        {
            if (grid.ColumnDefinitions.Count < 2)
                return;

            var original0 = grid.GetValue(OriginalCol0WidthProperty) as GridLength?;
            var original1 = grid.GetValue(OriginalCol1WidthProperty) as GridLength?;

            grid.ColumnDefinitions[0].Width = original0 ?? new GridLength(2, GridUnitType.Star);
            grid.ColumnDefinitions[1].Width = original1 ?? new GridLength(1, GridUnitType.Star);

            foreach (UIElement child in grid.Children)
            {
                var originalCol = child.GetValue(OriginalColumnProperty) as int?;
                if (originalCol.HasValue)
                {
                    Grid.SetColumn(child, originalCol.Value);
                }
            }
        }
    }
}
