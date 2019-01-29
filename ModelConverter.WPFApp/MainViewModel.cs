using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace ModelConverter.WPFApp
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IEnumerable<IModelReaderAsync> _readers;
        private readonly IEnumerable<IModelWriterAsync> _writers;
        private string _inputFilePath;
        private string _outputFilePath;
        private readonly AsyncCommand _convertCommand;
        private double _progressValue;
        private bool _converting;
        private string _area;
        private string _volume;
        private bool _hasError;
        private string _errorMessage;

        public event PropertyChangedEventHandler PropertyChanged;

        public string OutputFilePath
        {
            get => _outputFilePath;
            set
            {
                if (!SetBackingField(ref _outputFilePath, value))
                    _convertCommand.RaiseCanExecuteChanged();
            }
        }

        public string InputFilePath
        {
            get => _inputFilePath;
            set
            {
                if (!SetBackingField(ref _inputFilePath, value))
                    _convertCommand.RaiseCanExecuteChanged();
            }
        }

        public string[] OutputFormats { get; }
        public string SelectedOutputFormat { get; set; }

        public double ProgressValue
        {
            get => _progressValue;
            set => SetBackingField(ref _progressValue, value);
        }

        public bool Converting
        {
            get => _converting;
            set => SetBackingField(ref _converting, value);
        }

        public bool HasError
        {
            get => _hasError;
            set => SetBackingField(ref _hasError, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetBackingField(ref _errorMessage, value);
        }

        public string Area
        {
            get => _area;
            set => SetBackingField(ref _area, value);
        }

        public string Volume
        {
            get => _volume;
            set => SetBackingField(ref _volume, value);
        }

        public ICommand BrowseInputFileCommand { get; }
        public ICommand BrowseOutputFileCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand ConvertCommand => _convertCommand;

        public MainViewModel(IEnumerable<IModelReaderAsync> readers,
                             IEnumerable<IModelWriterAsync> writers)
        {
            if (!readers.Any())
                throw new ArgumentException(nameof(readers));

            if (!writers.Any())
                throw new ArgumentException(nameof(writers));

            _readers = readers;
            _writers = writers;

            HasError = false;
            OutputFormats = _writers.Select(w => w.FormatDescription).ToArray();
            SelectedOutputFormat = OutputFormats.First();
            
            BrowseInputFileCommand = new BasicCommand(BrowseInput);
            BrowseOutputFileCommand = new BasicCommand(BrowseOutput);
            ExitCommand = new BasicCommand(() => Application.Current.Shutdown(0));
            _convertCommand = new AsyncCommand(Convert, CanConvert);
        }

        private bool CanConvert() => !string.IsNullOrEmpty(InputFilePath) && !string.IsNullOrEmpty(OutputFilePath);

        private async Task Convert()
        {
            Converting = true;
            HasError = false;
            try
            {
                var input = new FileStream(InputFilePath, FileMode.Open);
                var output = new FileStream(OutputFilePath, FileMode.OpenOrCreate);
                using (input)
                using (output)
                {
                    var reader = _readers.FirstOrDefault(r => r.SupportedExtension == Path.GetExtension(InputFilePath));
                    if (reader == null)
                        return; // TODO : handle error

                    var cancellationToken = new CancellationToken();
                    var model = await reader.ReadAsync(input, cancellationToken, new Progress<double>(d => ProgressValue = d * 0.5));
                    await new STLModelWriter().WriteAsync(output, cancellationToken, new Progress<double>(d => ProgressValue = (100 + d) * 0.5), model);

                    Area = new AreaCalculator().Calculate(model).ToString("#.###", CultureInfo.InvariantCulture);
                    Volume = new VolumeCalculator().Calculate(model).ToString("#.###", CultureInfo.InvariantCulture);
                }
            }
            catch (Exception e)
            {
                Converting = false;
                HasError = true;
                ErrorMessage = e.Message;
            }
            finally
            {
                Converting = false;
            }
        }

        private void BrowseInput()
        {
            var d = new OpenFileDialog
            {
                Title = "Select input file",
                Multiselect = false,
                AddExtension = true,
                Filter = GetFilterString()
            };

            if (d.ShowDialog() != true)
                return;

            InputFilePath = d.FileName;

            var basePath = string.IsNullOrEmpty(OutputFilePath) ? InputFilePath : OutputFilePath;
            OutputFilePath = Path.GetDirectoryName(basePath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(InputFilePath) + ".stl";
        }

        private string GetFilterString() => _readers.Aggregate("", (current, reader) => current + $"{reader.FormatDescription}|*{reader.SupportedExtension}");

        private void BrowseOutput()
        {
            var dlg = new CommonOpenFileDialog
            {
                Title = "Select output folder",
                IsFolderPicker = true,
                AddToMostRecentlyUsedList = false,
                AllowNonFileSystemItems = false,
                EnsureFileExists = true,
                EnsurePathExists = true,
                EnsureReadOnly = false,
                EnsureValidNames = true,
                Multiselect = false,
                ShowPlacesList = true
            };

            if (dlg.ShowDialog() != CommonFileDialogResult.Ok)
                return;

            OutputFilePath = dlg.FileName + Path.DirectorySeparatorChar;
            if (!string.IsNullOrEmpty(InputFilePath))
                OutputFilePath += Path.GetFileNameWithoutExtension(InputFilePath) + ".stl";
        }

        private bool SetBackingField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

    }

}