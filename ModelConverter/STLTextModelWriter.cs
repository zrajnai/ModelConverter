using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ModelConverter
{
    public class STLTextModelWriter : IModelWriter, IModelWriterAsync
    {
        private Stream _output;
        private IModel _model;
        private int _lastReportedProgress;

        public string SupportedExtension => ".stl";

        public string FormatDescription => "ASCII STL .stl file";

        public void Write(Stream output, IModel model)
        {
            InternalWrite(output, CancellationToken.None, null, model);
        }

        public Task WriteAsync(Stream stream, CancellationToken token, IProgress<double> progress, IModel model)
        {
            return Task.Run(() => InternalWrite(stream, token, progress, model));
        }

        private void InternalWrite(Stream output, CancellationToken token, IProgress<double> progress, IModel model)
        {
            _output = output ?? throw new ArgumentNullException(nameof(output));

            if (!_output.CanWrite)
                throw new ArgumentException("Output stream must be writable");

            _model = model ?? throw new ArgumentNullException(nameof(model));
            
            using (var streamWriter = new StreamWriter(_output))
            {
                streamWriter.Write("solid ");
                
                _lastReportedProgress = 0;
                foreach (var face in _model.Faces)
                {
                    ReportProgress(progress);

                    if (token.IsCancellationRequested)
                        break;

                    streamWriter.Write($"facet normal {face.Normal.X} {face.Normal.Y} {face.Normal.Z}");

                    WriteFaceVertices(streamWriter, face);

                    streamWriter.Write(@"endfacet");
                }
            }

        }

        private void WriteFaceVertices(TextWriter tw, Face face)
        {
            tw.Write(@"    outer loop");
            WriteVertex(tw, face.VertexIndices[0]);
            WriteVertex(tw, face.VertexIndices[1]);
            WriteVertex(tw, face.VertexIndices[2]);
            tw.Write(@"    endloop");
        }

        private void WriteVertex(TextWriter bw, int vertexIndex)
        {
            var v = _model.Vertices[vertexIndex];
            bw.Write($"        vertex {v.X} {v.Y} {v.Z}");
        }

        private void ReportProgress(IProgress<double> progress)
        {
            var currentProgress = (int)(100d * _output.Position / _output.Length);
            if (currentProgress == _lastReportedProgress)
                return;
            progress?.Report(currentProgress);
            _lastReportedProgress = currentProgress;
        }
    }
}