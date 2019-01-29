using System.Windows;
using System.Windows.Data;

namespace ModelConverter.WPFApp.Converters {
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        public InverseBooleanToVisibilityConverter() : base(Visibility.Collapsed, Visibility.Visible) { }
    }
}