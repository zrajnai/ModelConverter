using System.Linq;
using System.Windows;

namespace ModelConverter.WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            var readers = new[] { new OBJModelReader() };
            var writers = new[] { new STLModelWriter() };
            DataContext = new MainViewModel(readers, writers);
        }
    }

}
