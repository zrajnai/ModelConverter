using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ModelConverter.Math;
using ModelConverter.Model;

namespace ModelConverter.ModelReaders
{
    public class OBJModelReader : IModelReader, IModelReaderAsync
    {
        private Stream _input;
        private int _currentLineNumber;
        private string _currentLine;
        private int _lastReportedProgress;
        private Model.Model _model;

        public string SupportedExtension => ".obj";

        public string FormatDescription => "Wavefront .obj file";

        public IModel Read(Stream input) => InternalRead(input, CancellationToken.None);

        public Task<IModel> ReadAsync(Stream input, CancellationToken token, IProgress<double> progress) => Task.Run(() => InternalRead(input, token, progress));

        private IModel InternalRead(Stream input, CancellationToken token, IProgress<double> progress = null)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));

            if (!_input.CanRead)
                throw new ArgumentException("Input stream must be readable");

            _model = new Model.Model();
            _currentLineNumber = 0;
            _lastReportedProgress = 0;

            using (var tr = new StreamReader(_input))
            {
                while (!tr.EndOfStream)
                {
                    ReportProgress(progress);

                    if (token.IsCancellationRequested)
                        break;

                    if (!TryParseLine(tr))
                        break;
                }
            }
            return _model;
        }

        private bool TryParseLine(TextReader tr)
        {
            _currentLine = tr.ReadLine();
            if (_currentLine == null)
                return false;

            _currentLineNumber++;
            if (_currentLine.Length == 0)
                return true;

            if (IsComment())
                return true;

            if (IsVertexDefinition())
                ReadVertex();

            else if (IsFaceDefinition())
                ReadFace();

            else if (IsVertexNormalDefinition())
                ReadVertexNormal();

            else if (IsTextureCoordDefinition())
                ReadTextureCoord();

            return true;
        }

        private bool IsComment() => _currentLine.StartsWith("#");

        private bool IsVertexDefinition() => _currentLine.StartsWith("v ");

        private void ReadVertex()
        {
            var parts = SplitLine();
            if (parts.Length < 4 || parts.Length > 5)
                ThrowInvalidModelFormatException("Wrong vertex format");

            if (!TryStrictDoubleParse(parts[1], out var x))
                ThrowParseException("vertex X");
            if (!TryStrictDoubleParse(parts[2], out var y))
                ThrowParseException("vertex Y");
            if (!TryStrictDoubleParse(parts[3], out var z))
                ThrowParseException("vertex Z");
            var w = 1.0;
            if (parts.Length > 4 && !TryStrictDoubleParse(parts[4], out w))
                ThrowParseException("vertex W");

            _model.AddVertex(new Vertex { X = x, Y = y, Z = z, W = w });
        }

        private bool IsVertexNormalDefinition() => _currentLine.StartsWith("vn ");

        private void ReadVertexNormal()
        {
            var parts = SplitLine();
            if (parts.Length != 4)
                ThrowInvalidModelFormatException("Wrong vertex normal format");

            if (!TryStrictDoubleParse(parts[1], out var x))
                ThrowParseException("vertex normal X");
            if (!TryStrictDoubleParse(parts[2], out var y))
                ThrowParseException("vertex normal Y");
            if (!TryStrictDoubleParse(parts[3], out var z))
                ThrowParseException("vertex normal Z");

            _model.AddVertexNormal(new Vector(x, y, z));
        }

        private bool IsTextureCoordDefinition() => _currentLine.StartsWith("vt ");

        private void ReadTextureCoord()
        {
            var parts = SplitLine();
            if (parts.Length < 2 || parts.Length > 4)
                ThrowInvalidModelFormatException("Wrong texture coordinate format");

            if (!TryStrictDoubleParse(parts[1], out var u))
                ThrowParseException("texture coord U");

            double v = 0;
            if (parts.Length > 2 && !TryStrictDoubleParse(parts[2], out v))
                ThrowParseException("texture coord V");

            double w = 0;
            if (parts.Length > 3 && !TryStrictDoubleParse(parts[3], out w))
                ThrowParseException("texture coord W");

            _model.AddTextureCoord(new TextureCoord { U = u, V = v, W = w });
        }

        private bool IsFaceDefinition() => _currentLine.StartsWith("f ");

        private void ReadFace()
        {
            var parts = SplitLine();
            if (parts.Length < 4)
                ThrowInvalidModelFormatException("Too few parameters for face");
            if (parts.Length > 5)
                return; //ThrowInvalidModelFormatException("Sorry, only triangular and quad faces are supported");

            var vertexIndices = new List<int>(3);
            var textureCoordIndices = new List<int>(3);
            var normalIndices = new List<int>(3);
            foreach (var part in parts.Skip(1))
            {
                ParseFaceIndexBundle(part, vertexIndices, textureCoordIndices, normalIndices);
            }

            ValidateTextureCoordIndices(textureCoordIndices, vertexIndices);

            ValidateNormalIndices(normalIndices, vertexIndices);

            _model.AddFace(new Face
            {
                VertexIndices = vertexIndices.ToArray(),
                NormalIndices = normalIndices.ToArray(),
                TextureCoordIndices = textureCoordIndices.ToArray(),
            });
        }
        
        private void ParseFaceIndexBundle(string part, ICollection<int> vertexIndices, List<int> textureCoordIndices, List<int> normalIndices)
        {
            var indices = part.Split('/');
            if (indices.Length < 1)
                ThrowInvalidModelFormatException("Too few parameters for face");

            ParseVertexIndex(indices, vertexIndices);

            ParseTextureCoordIndex(indices, textureCoordIndices);

            ParseNormalIndex(indices, normalIndices);
        }

        private void ParseVertexIndex(IReadOnlyList<string> indices, ICollection<int> vertexIndices)
        {
            if (!int.TryParse(indices[0], out var vertexIndex))
                ThrowParseException("face vertex index");
            if (vertexIndex > _model.Vertices.Count)
                ThrowInvalidModelFormatException("Illegal vertex reference");
            if (vertexIndex < 0)
                vertexIndex = _model.Vertices.Count - vertexIndex + 1;

            vertexIndices.Add(vertexIndex - 1);
        }

        private void ParseNormalIndex(IReadOnlyList<string> indices, ICollection<int> normalIndices)
        {
            var normalIndex = -1;
            if (indices.Count > 2 && indices[2].Length > 0 && !int.TryParse(indices[2], out normalIndex))
                ThrowParseException("face normal index");
            if (normalIndex > 0)
                normalIndices.Add(normalIndex - 1);
        }

        private void ParseTextureCoordIndex(IReadOnlyList<string> indices, ICollection<int> textureCoordIndices)
        {
            var textureCoordIndex = -1;
            if (indices.Count > 1 && indices[1].Length > 0 && !int.TryParse(indices[1], out textureCoordIndex))
                ThrowParseException("face texture index");
            if (textureCoordIndex >= 0)
                textureCoordIndices.Add(textureCoordIndex - 1);
        }

        private void ValidateNormalIndices(IReadOnlyCollection<int> normalIndices, IReadOnlyCollection<int> vertexIndices)
        {
            if (normalIndices.Count > 0 && normalIndices.Count != vertexIndices.Count)
                ThrowInvalidModelFormatException("Inconsistent face definition: not all vertices have normal coords");
        }

        private void ValidateTextureCoordIndices(IReadOnlyCollection<int> textureCoordIndices, IReadOnlyCollection<int> vertexIndices)
        {
            if (textureCoordIndices.Count > 0 && textureCoordIndices.Count != vertexIndices.Count)
                ThrowInvalidModelFormatException("Inconsistent face definition: not all vertices have texture coords");
        }

        private string[] SplitLine()
        {
            return _currentLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static bool TryStrictDoubleParse(string input, out double output)
        {
            return double.TryParse(input, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out output);
        }
        private void ThrowParseException(string component)
        {
            throw new InvalidModelFormatException($"Can't parse {component} {CurrentLineMessage()}");
        }

        private void ThrowInvalidModelFormatException(string message)
        {
            throw new InvalidModelFormatException(message + Environment.NewLine + CurrentLineMessage());
        }

        private string CurrentLineMessage() => $"({_currentLineNumber}): " + _currentLine;

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