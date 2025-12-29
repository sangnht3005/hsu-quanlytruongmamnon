using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KindergartenManagement.Converters;

public class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            return visibility == Visibility.Visible;
        }
        return false;
    }
}

public class InverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return true;
    }
}

public class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string stringValue)
        {
            return string.IsNullOrWhiteSpace(stringValue) ? Visibility.Collapsed : Visibility.Visible;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class PageToTagConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string currentPage && parameter is string targetPage)
        {
            return currentPage == targetPage ? "Selected" : "NotSelected";
        }
        return "NotSelected";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class StatusToVietnameseConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string status)
        {
            return status switch
            {
                "Pending" => "Chờ duyệt",
                "Approved" => "Đã duyệt",
                "Rejected" => "Từ chối",
                "Vacation" => "Nghỉ phép",
                "Sick" => "Nghỉ ốm",
                "Personal" => "Nghỉ cá nhân",
                "Unpaid" => "Nghỉ không lương",
                _ => status
            };
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string vietnamese)
        {
            return vietnamese switch
            {
                "Chờ duyệt" => "Pending",
                "Đã duyệt" => "Approved",
                "Từ chối" => "Rejected",
                "Nghỉ phép" => "Vacation",
                "Nghỉ ốm" => "Sick",
                "Nghỉ cá nhân" => "Personal",
                "Nghỉ không lương" => "Unpaid",
                _ => vietnamese
            };
        }
        return value;
    }
}

public class DayOfWeekToVietnameseConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int dayOfWeek)
        {
            return dayOfWeek switch
            {
                0 => "Chủ nhật",
                1 => "Thứ hai",
                2 => "Thứ ba",
                3 => "Thứ tư",
                4 => "Thứ năm",
                5 => "Thứ sáu",
                6 => "Thứ bảy",
                _ => dayOfWeek.ToString()
            };
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
