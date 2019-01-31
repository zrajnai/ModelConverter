using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace ModelConverter.WPFApp.Converters
{
    public class BooleanConverter<T> : IValueConverter
    {

        #region Constructors

        public BooleanConverter(T trueValue, T falseValue)
        {
            True = trueValue;
            False = falseValue;
        }

        #endregion

        #region Public Properties

        public T True { get; }

        public T False { get; }

        #endregion

        #region Public Methods

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? True : False;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is T && EqualityComparer<T>.Default.Equals((T)value, True);
        }

        #endregion

    }
}