using System.Windows;
using ModelConverter.ModelReaders;
using ModelConverter.ModelWriters;

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

            var readers = new IModelReaderAsync[]
            {
                new OBJModelReader(),
                new STLBinaryModelReader()
            };
            var writers = new IModelWriterAsync[]
            {
                new STLBinaryModelWriter(),
                new STLTextModelWriter()
            };

            DataContext = new MainViewModel(readers, writers);
        }
    }

}
