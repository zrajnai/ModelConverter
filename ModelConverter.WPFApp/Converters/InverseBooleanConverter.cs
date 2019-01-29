using System.Windows.Data;

namespace ModelConverter.WPFApp.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : BooleanConverter<bool>
    {
        public InverseBooleanConverter() : base(false, true) { }
    }

}