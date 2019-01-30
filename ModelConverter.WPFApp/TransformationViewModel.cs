using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using ModelConverter.Math;
using ModelConverter.Model;

namespace ModelConverter.WPFApp
{
    public class TransformationViewModel : INotifyPropertyChanged
    {
        private double _scaleX = 1;
        private double _scaleY = 1;
        private double _scaleZ = 1;
        private double _translationX;
        private double _translationY;
        private double _translationZ;
        private double _rotationX;
        private double _rotationY;
        private double _rotationZ;

        public double ScaleX
        {
            get => _scaleX;
            set => SetBackingField(ref _scaleX, value);
        }

        public double ScaleY
        {
            get => _scaleY;
            set => SetBackingField(ref _scaleY, value);
        }

        public double ScaleZ
        {
            get => _scaleZ;
            set => SetBackingField(ref _scaleZ, value);
        }

        public double TranslationX
        {
            get => _translationX;
            set => SetBackingField(ref _translationX, value);
        }

        public double TranslationY
        {
            get => _translationY;
            set => SetBackingField(ref _translationY, value);
        }

        public double TranslationZ
        {
            get => _translationZ;
            set => SetBackingField(ref _translationZ, value);
        }

        public double RotationX
        {
            get => _rotationX;
            set => SetBackingField(ref _rotationX, value);
        }

        public double RotationY
        {
            get => _rotationY;
            set => SetBackingField(ref _rotationY, value);
        }

        public double RotationZ
        {
            get => _rotationZ;
            set => SetBackingField(ref _rotationZ, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool SetBackingField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        public Matrix Matrix => Matrix.Identity * 
                                Matrix.Scale(ScaleX, ScaleY, ScaleZ) * 
                                Matrix.RotationDeg(RotationX, RotationY, RotationZ) * 
                                Matrix.Translate(TranslationX, TranslationY, TranslationZ); 
    }
}
