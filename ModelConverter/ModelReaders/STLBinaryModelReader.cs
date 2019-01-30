using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ModelConverter.Math;
using ModelConverter.Model;

namespace ModelConverter.ModelWriters
{
    public class STLBinaryModelReader : IModelReaderAsync
    {
        private Stream _input;
        private Model.Model _model;
        private int _lastReportedProgress;

        public string SupportedExtension => ".stl";

        public string FormatDescription => "Binary STL .stl file";

        public Task<IModel> ReadAsync(Stream input, CancellationToken token, IProgress<double> progress)
        {
            return Task.Run(() => InternalRead(input, token, progress));
        }

        private IModel InternalRead(Stream input, CancellationToken token, IProgress<double> progress)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));

            if (!_input.CanRead)
                throw new ArgumentException("Input stream must be readable");

            _model = new Model.Model();
            
            using (var br = new BinaryReader(input))
            {
                br.ReadBytes(80);
                var faceNum = br.ReadUInt32();

                _lastReportedProgress = 0;
                for (var i = 0; i < faceNum; i++)
                {
                    ReportProgress(progress);

                    if (token.IsCancellationRequested)
                        break;

                    var normal = ReadVector(br);
                    var vertexIdx = _model.Vertices.Count;
                    _model.AddVertex((Vertex)ReadVector(br));
                    _model.AddVertex((Vertex)ReadVector(br));
                    _model.AddVertex((Vertex)ReadVector(br));
                    
                    br.ReadInt16();

                    var face = new Face
                    {
                        Normal = normal,
                        VertexIndices = new[] { vertexIdx, vertexIdx + 1, vertexIdx + 2 }
                    };
                    _model.AddFace(face);
                }
            }
            return _model;
        }

        private Vector ReadVector(BinaryReader br) => new Vector(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

        private void ReportProgress(IProgress<double> progress)
        {
            var currentProgress = (int)(100d * _input.Position / _input.Length);
            if (currentProgress == _lastReportedProgress)
                return;
            progress?.Report(currentProgress);
            _lastReportedProgress = currentProgress;
        }
    }
}