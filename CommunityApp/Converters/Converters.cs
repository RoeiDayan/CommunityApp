using System.Globalization;
using CommunityApp.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CommunityApp.Converters
{
    public class DateToAvailabilityColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                if (parameter is ContentPage page && page.BindingContext is TenantRoomViewModel viewModel)
                {
                    string status = viewModel.GetRoomStatusForDate(date);
                    return status switch
                    {
                        "Occupied" => Colors.Red,
                        "Requested" => Colors.Orange,
                        "Available" => Colors.Green,
                        _ => Colors.Gray,
                    };
                }
            }
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string colorParams)
            {
                string[] colors = colorParams.Split(',');
                if (colors.Length == 2 &&
                    Color.TryParse(colors[0], out var trueColor) &&
                    Color.TryParse(colors[1], out var falseColor))
                {
                    return boolValue ? trueColor : falseColor;
                }
            }

            return Colors.Gray; // fallback
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class NotNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class NullToBoolConverter : IValueConverter
    {
        public bool Invert { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = value != null;
            return Invert ? !result : result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class BalanceColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int balance)
            {
                return balance < 0 ? Colors.Red : balance > 0 ? Colors.Green : Colors.Gray;
            }
            else if (int.TryParse(value?.ToString(), out int parsed))
            {
                return parsed < 0 ? Colors.Red : parsed > 0 ? Colors.Green : Colors.Gray;
            }

            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }


    public class ApprovedStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool isApproved ? (isApproved ? "Approved" : "Pending Approval") : "Pending";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ApprovedColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool isApproved ? (isApproved ? Colors.Green : Colors.Orange) : Colors.Orange;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RoleColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool roleValue)
            {
                return roleValue ? Colors.Green : Colors.Red;
            }

            return Colors.Gray; // Default color if null or invalid value
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool boolValue ? !boolValue : true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool boolValue ? !boolValue : false;
        }
    }
    //New convs
    // Converter for DateOnly to display string
    public class DateOnlyDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateOnly dateOnly)
            {
                return dateOnly.ToString("MMM dd, yyyy");
            }
            return "Not specified";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    

    // PaymentStatusConverter for nullable bool WasPayed property
    public class PaymentStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "⏳ PENDING";

            if (value is bool boolValue)
            {
                return boolValue ? "✅ PAID" : "❌ UNPAID";
            }

            return "⏳ PENDING";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // PaymentStatusColorConverter for nullable bool WasPayed property
    public class PaymentStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Colors.Orange;

            if (value is bool boolValue)
            {
                return boolValue ? Colors.Green : Colors.Red;
            }

            return Colors.Orange;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // PaymentAmountConverter for int amount
    public class PaymentAmountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int amount)
            {
                return $"${amount:N0}";
            }
            if (value != null && int.TryParse(value.ToString(), out int parsedAmount))
            {
                return $"${parsedAmount:N0}";
            }
            return "$0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



}
