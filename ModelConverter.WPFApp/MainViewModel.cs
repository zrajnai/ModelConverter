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
using ModelConverter.Calculators;
using ModelConverter.WPFApp.Commands;

namespace ModelConverter.WPFApp
{
    public class MainViewModel : INotifyPropertyChanged
    {

        #region Member Variables

        private readonly AsyncCommand _convertCommand;
        private readonly IEnumerable<IModelReaderAsync> _readers;
        private readonly IEnumerable<IModelWriterAsync> _writers;
        private string _area;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _converting;
        private bool _hasStatus;
        private string _inputFilePath;
        private string _outputFilePath;
        private double _progressValue;
        private string _selectedOutputFormat;
        private IModelWriterAsync _selectedWriter;
        private string _statusMessage;
        private string _volume;

        #endregion

        #region Constructors

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

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        public TransformationViewModel Transformation { get; } = new TransformationViewModel();

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

        public bool HasStatus
        {
            get => _hasStatus;
            set => SetBackingField(ref _hasStatus, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                SetBackingField(ref _statusMessage, value);
                HasStatus = !string.IsNullOrEmpty(_statusMessage);
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

        #endregion

        #region Private Methods

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
            StatusMessage = string.Empty;

            try
            {
                var input = new FileStream(InputFilePath, FileMode.Open);
                var output = new FileStream(OutputFilePath, FileMode.OpenOrCreate);
                using (input)
                using (output)
                {
                    var reader = SelectReader();

                    _cancellationTokenSource = new CancellationTokenSource();
                    var model = await reader.ReadAsync(input, _cancellationTokenSource.Token, new Progress<double>(d => ProgressValue = d * 0.5));

                    model = ModelTransformer.Transform(model, Transformation.Matrix);

                    await _selectedWriter.WriteAsync(output, _cancellationTokenSource.Token, new Progress<double>(d => ProgressValue = (100 + d) * 0.5), model);

                    Area = AreaCalculator.Calculate(model).ToString("#.###", CultureInfo.InvariantCulture);
                    Volume = VolumeCalculator.Calculate(model).ToString("#.###", CultureInfo.InvariantCulture);

                    StatusMessage = "Conversion successful.";
                }
            }
            catch (Exception e)
            {
                StatusMessage = e.Message;
            }
            finally
            {
                if (_cancellationTokenSource?.IsCancellationRequested == true)
                {
                    StatusMessage = "Canceled";
                }
                Converting = false;
            }
        }

        private IModelReaderAsync SelectReader()
        {
            var reader = _readers.FirstOrDefault(r => r.SupportedExtension == Path.GetExtension(InputFilePath));
            if (reader == null)
                throw new ApplicationException("Internal error!");
            return reader;
        }

        private void DoBrowseInput()
        {
            var d = new OpenFileDialog
            {
                Title = "Select input file",
                Multiselect = false,
                AddExtension = true,
                Filter = _readers.Aggregate("", (current, reader) => current + $"{reader.FormatDescription}|*{reader.SupportedExtension}|").TrimEnd('|')
            };

            if (d.ShowDialog() != true)
                return;

            InputFilePath = d.FileName;

            var basePath = string.IsNullOrEmpty(OutputFilePath) ? InputFilePath : OutputFilePath;
            OutputFilePath = GenerateOutFilePath(basePath);
        }

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
            var outputPath = Path.GetDirectoryName(basePath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(InputFilePath) + _selectedWriter.SupportedExtension;
            if (outputPath != InputFilePath)
                return outputPath;

            var fileName = Path.GetFileNameWithoutExtension(outputPath) + "(1)" + Path.GetExtension(outputPath);
            var dir = Path.GetDirectoryName(outputPath);
            outputPath = dir + Path.DirectorySeparatorChar + fileName;
            return outputPath;
        }

        private bool SetBackingField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        #endregion

    }

}