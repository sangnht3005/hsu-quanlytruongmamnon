# Invoice Management Crash Fix

## Issue
When user navigated to "💰 Hóa Đơn" (Invoice) menu, the application crashed without any error message.

## Root Cause
The `StackPanel.IsEnabled` binding was using `InverseBooleanConverter` with `SelectedInvoice` (an object type), not a boolean value. This caused:
1. Conversion failure when trying to convert object to boolean
2. Unhandled exception during data binding
3. Application crash

## Solution Implemented

### 1. Added NullToBoolConverter
Created new converter in [Converters/Converters.cs](Converters/Converters.cs):
```csharp
public class NullToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null;  // Returns true if object is not null
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

### 2. Registered Converter in XAML
Added to [Views/MainWindow.xaml](Views/MainWindow.xaml) Window.Resources:
```xaml
<conv:NullToBoolConverter x:Key="NullToBoolConverter"/>
```

### 3. Fixed Data Binding
Changed from:
```xaml
IsEnabled="{Binding SelectedInvoice, Converter={StaticResource InverseBooleanConverter}, FallbackValue=false}"
```

To:
```xaml
IsEnabled="{Binding SelectedInvoice, Converter={StaticResource NullToBoolConverter}}"
```

## Logic
- **Before**: User selects invoice → null check fails → crash
- **After**: User selects invoice → converter checks if not null → form enables → no crash

## Files Modified
1. `Converters/Converters.cs` - Added NullToBoolConverter class
2. `Views/MainWindow.xaml` - Updated binding and registered converter

## Testing
- ✅ Build succeeded (0 errors, 0 warnings)
- ✅ Application starts without crash
- ✅ User can navigate to Invoice menu
- ✅ Form enables/disables properly when invoice selected/unselected

## Result
Invoice Management module now functions correctly without crashes! 🎉

The form is:
- **Disabled** when no invoice is selected (SelectedInvoice == null)
- **Enabled** when invoice is selected (SelectedInvoice != null)
