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
using ModelConverter.WPFApp.Commands;

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
        private string _selectedOutputFormat;
        private IModelWriterAsync _selectedWriter;
        private CancellationTokenSource _cancellationTokenSource;

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

        public string SelectedOutputFormat
        {
            get => _selectedOutputFormat;
            set
            {
                _selectedOutputFormat = value;
                _selectedWriter = _writers.SingleOrDefault(w => w.FormatDescription == value);
            }
        }

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
            set
            {
                SetBackingField(ref _errorMessage, value);
                HasError = !string.IsNullOrEmpty(_errorMessage);
            }
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
        public ICommand CancelCommand { get; }

        public MainViewModel(IEnumerable<IModelReaderAsync> readers,
                             IEnumerable<IModelWriterAsync> writers)
        {
            if (!readers.Any())
                throw new ArgumentException(nameof(readers));

            if (!writers.Any())
                throw new ArgumentException(nameof(writers));

            _readers = readers;
            _writers = writers;

            OutputFormats = _writers.Select(w => w.FormatDescription).ToArray();
            SelectedOutputFormat = OutputFormats.First();
            
            BrowseInputFileCommand = new BasicCommand(DoBrowseInput);
            BrowseOutputFileCommand = new BasicCommand(DoBrowseOutput);
            ExitCommand = new BasicCommand(DoExit);
            _convertCommand = new AsyncCommand(Convert, CanConvert);
            CancelCommand = new BasicCommand(DoCancel);
        }

        private void DoCancel()
        {
            _cancellationTokenSource?.Cancel();
        }

        private void DoExit()
        {
            DoCancel();
            Application.Current.Shutdown(0);
        }

        private bool CanConvert() => !string.IsNullOrEmpty(InputFilePath) && !string.IsNullOrEmpty(OutputFilePath) && !Converting;

        private async Task Convert()
        {
            Area = string.Empty;
            Volume = string.Empty;
            Converting = true;
            ErrorMessage = string.Empty;

            try
            {
                var input = new FileStream(InputFilePath, FileMode.Open);
                var output = new FileStream(OutputFilePath, FileMode.OpenOrCreate);
                using (input)
                using (output)
                {
                    var reader = _readers.FirstOrDefault(r => r.SupportedExtension == Path.GetExtension(InputFilePath));
                    if (reader == null)
                        throw new ApplicationException("Internal error");

                    _cancellationTokenSource = new CancellationTokenSource();
                    var model = await reader.ReadAsync(input, _cancellationTokenSource.Token, new Progress<double>(d => ProgressValue = d * 0.5));
                    await new STLBinaryModelWriter().WriteAsync(output, _cancellationTokenSource.Token, new Progress<double>(d => ProgressValue = (100 + d) * 0.5), model);

                    Area = new AreaCalculator().Calculate(model).ToString("#.###", CultureInfo.InvariantCulture);
                    Volume = new VolumeCalculator().Calculate(model).ToString("#.###", CultureInfo.InvariantCulture);
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
            finally
            {
                if (_cancellationTokenSource?.IsCancellationRequested == true)
                {
                    ErrorMessage = "Canceled";
                }
                Converting = false;
            }
        }

        private void DoBrowseInput()
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
            OutputFilePath = GenerateOutFilePath(basePath);
        }

        private string GetFilterString() => _readers.Aggregate("", (current, reader) => current + $"{reader.FormatDescription}|*{reader.SupportedExtension}");

        private void DoBrowseOutput()
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
                OutputFilePath = GenerateOutFilePath(dlg.FileName);
        }

        private string GenerateOutFilePath(string basePath)
        {
            return Path.GetDirectoryName(basePath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(InputFilePath) + _selectedWriter.SupportedExtension;
        }

        private bool SetBackingField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

    }

}