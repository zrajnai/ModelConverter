using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ModelConverter.Model;

namespace ModelConverter.ModelWriters
{
    public class STLBinaryModelWriter : IModelWriterAsync
    {

        #region Member Variables

        private int _lastReportedProgress;
        private IModel _model;
        private Stream _output;

        #endregion

        #region Public Properties

        public string SupportedExtension => ".stl";

        public string FormatDescription => "Binary STL .stl file";

        #endregion

        #region Public Methods

        public Task WriteAsync(Stream stream, CancellationToken token, IProgress<double> progress, IModel model)
        {
            return Task.Run(() => InternalWrite(stream, token, progress, model));
        }

        #endregion

        #region Private Methods

        private void InternalWrite(Stream output, CancellationToken token, IProgress<double> progress, IModel model)
        {
            _output = output ?? throw new ArgumentNullException(nameof(output));

            if (!_output.CanWrite)
                throw new ArgumentException("Output stream must be writable");

            _model = model ?? throw new ArgumentNullException(nameof(model));

            using (var bw = new BinaryWriter(_output))
            {
                bw.Write(new byte[80]);
                bw.Write((uint)_model.Faces.Count);

                _lastReportedProgress = 0;
                foreach (var face in _model.Faces)
                {
                    ReportProgress(progress);

                    if (token.IsCancellationRequested)
                        break;

                    WriteNormal(bw, face);

                    WriteVertex(bw, face.VertexIndices[0]);
                    WriteVertex(bw, face.VertexIndices[1]);
                    WriteVertex(bw, face.VertexIndices[2]);

                    bw.Write((ushort)0);
                }
            }

        }

        private void WriteNormal(BinaryWriter bw, Face face)
        {
            bw.Write((float)face.Normal.X);
            bw.Write((float)face.Normal.Y);
            bw.Write((float)face.Normal.Z);
        }

        private void WriteVertex(BinaryWriter bw, int vertexIndex)
        {
            var v1 = _model.Vertices[vertexIndex];
            bw.Write((float)v1.X);
            bw.Write((float)v1.Y);
            bw.Write((float)v1.Z);
        }

        private void ReportProgress(IProgress<double> progress)
        {
            var currentProgress = (int)(100d * _output.Position / _output.Length);
            if (currentProgress == _lastReportedProgress)
                return;
            progress?.Report(currentProgress);
            _lastReportedProgress = currentProgress;
        }

        #endregion

    }
}