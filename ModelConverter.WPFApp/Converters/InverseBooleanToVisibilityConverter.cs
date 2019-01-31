using System.Windows;
using System.Windows.Data;

namespace ModelConverter.WPFApp.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanToVisibilityConverter : BooleanConverter<Visibility>
    {

        #region Constructors

        public InverseBooleanToVisibilityConverter() : base(Visibility.Collapsed, Visibility.Visible) { }

        #endregion

    }
}