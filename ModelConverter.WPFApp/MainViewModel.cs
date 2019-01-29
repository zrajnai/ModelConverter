using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
        private string _inputFilePath;
        private string _outputFilePath;
        private readonly AsyncCommand _convertCommand;
        private double _progressValue;
        private bool _converting;

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

        public double ProgressValue
        {
            get => _progressValue;
            set => SetBackingField(ref _progressValue, value);
        }

        public ICommand BrowseInputFileCommand { get; }
        public ICommand BrowseOutputFileCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand ConvertCommand => _convertCommand;

        public bool Converting
        {
            get => _converting;
            set => SetBackingField(ref _converting, value);
        }

        public MainViewModel()
        {
            BrowseInputFileCommand = new BasicCommand(BrowseInput);
            BrowseOutputFileCommand = new BasicCommand(BrowseOutput);
            ExitCommand = new BasicCommand(() => Application.Current.Shutdown(0));
            _convertCommand = new AsyncCommand(Convert, CanConvert);
        }

        private bool CanConvert() => !string.IsNullOrEmpty(InputFilePath) && !string.IsNullOrEmpty(OutputFilePath);

        private async Task Convert()
        {
            Converting = true;
            try
            {
                var input = new FileStream(InputFilePath, FileMode.Open);
                var output = new FileStream(OutputFilePath, FileMode.OpenOrCreate);
                using (input)
                using (output)
                {
                    await new Converter().ConvertAsync(new CancellationToken(),
                                                       new Progress<double>(ProgressHandler),
                                                       new OBJModelReader(input),
                                                       new STLModelWriter(output));
                }
            }
            finally
            {
                Converting = false;
            }
        }

        private void ProgressHandler(double progress)
        {
            ProgressValue = progress;
        }

        private void BrowseInput()
        {
            var d = new OpenFileDialog
            {
                Title = "Select input file",
                Multiselect = false,
                AddExtension = true,
                Filter = "OBJ files (*.obj)|*.obj"
            };

            if (d.ShowDialog() != true)
                return;

            InputFilePath = d.FileName;

            if (string.IsNullOrEmpty(OutputFilePath))
                OutputFilePath = Path.GetDirectoryName(InputFilePath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(InputFilePath) + ".stl";
            else
                OutputFilePath = Path.GetDirectoryName(OutputFilePath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(InputFilePath) + ".stl";
        }

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