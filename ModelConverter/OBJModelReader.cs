using System;
using System.Collections.Generic;
using System.IO;

namespace ModelConverter
{
    internal class OBJModelReader : IModelReader
    {
        private int _currentLineNumber;
        private string _currentLine;
        private Model _model;

        public Model Read(Stream input)
        {
            _model = new Model();
            var tr = new StreamReader(input);

            _currentLineNumber = 0;
            while (!tr.EndOfStream)
            {
                _currentLine = tr.ReadLine();
                if (_currentLine == null)
                    break;

                _currentLineNumber++;
                if (string.IsNullOrEmpty(_currentLine))
                    continue;

                if (IsComment())
                    continue;

                if (IsVertexDefinition())
                    ReadVertex();

                else if (IsFaceDefinition())
                    ReadFace();

                else if (IsVertexNormalDefinition())
                    ReadVertexNormal();

                else if (IsTextureCoordDefinition())
                    ReadTextureCoord();
            }

            return _model;
        }

        private bool IsComment() => _currentLine.StartsWith("#");

        private bool IsVertexDefinition() => _currentLine.StartsWith("v");

        private void ReadVertex()
        {
            var parts = _currentLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 4 || parts.Length > 5)
                throw new InvalidModelFormatException($"Wrong vertex format in line : {_currentLineNumber}");

            if (!double.TryParse(parts[1], out var x))
                ThrowParseException("vertex X");
            if (!double.TryParse(parts[2], out var y))
                ThrowParseException("vertex Y");
            if (!double.TryParse(parts[3], out var z))
                ThrowParseException("vertex Z");
            var w = 1.0;
            if (parts.Length > 2 && !double.TryParse(parts[3], out w))
                ThrowParseException("vertex W");

            _model.AddVertex(new Vertex {X = x, Y = y, Z = z, W = w});
        }

        private bool IsVertexNormalDefinition() => _currentLine.StartsWith("vn");

        private void ReadVertexNormal()
        {
            var parts = _currentLine.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries );
            if (parts.Length != 4)
                throw new InvalidModelFormatException($"Wrong vertex normal format in line : {_currentLineNumber}");

            if (!double.TryParse(parts[1], out var x))
                ThrowParseException("vertex normal X");
            if (!double.TryParse(parts[2], out var y))
                ThrowParseException("vertex normal Y");
            if (!double.TryParse(parts[3], out var z))
                ThrowParseException("vertex normal Z");

            _model.AddVertexNormal(new VertexNormal { X = x, Y = y, Z = z });
        }

        private bool IsTextureCoordDefinition() => _currentLine.StartsWith("vt");

        private void ReadTextureCoord()
        {
            var parts = _currentLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2 || parts.Length > 4)
                throw new InvalidModelFormatException($"Wrong texture coordinate format in line : {_currentLineNumber}");

            if (!double.TryParse(parts[1], out var u))
                ThrowParseException("texture coord U");

            double v = 0;
            if (parts.Length > 2 && !double.TryParse(parts[2], out v))
                ThrowParseException("texture coord V");

            double w = 0;
            if (parts.Length > 3 && !double.TryParse(parts[3], out w))
                ThrowParseException("texture coord W");

            _model.AddTextureCoord(new TextureCoord {U = u, V = v, W = w});
        }

        private bool IsFaceDefinition() => _currentLine.StartsWith("f");

        private void ReadFace()
        {
            var parts = _currentLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 4)
                throw new InvalidModelFormatException($"Too few parameters for face in line : {_currentLineNumber}");

            var vertexIndices = new List<int>();
            var textureCoordIndices = new List<int>();
            var normalIndices = new List<int>();
            foreach (var part in parts)
            {
                var indices = part.Split(' ');
                if (indices.Length < 1)
                    throw new InvalidModelFormatException($"Too few parameters for face in line : {_currentLineNumber}");

                if (!int.TryParse(indices[0], out var vertexIndex))
                    ThrowParseException("face vertex index");
                vertexIndices.Add(vertexIndex);

                var textureCoordIndex = -1;
                if (indices.Length > 1 && !int.TryParse(indices[1], out textureCoordIndex))
                    ThrowParseException("face texture index");
                if (textureCoordIndex >= 0)
                    textureCoordIndices.Add(textureCoordIndex);

                var normalIndex = -1;
                if (indices.Length > 2 && !int.TryParse(indices[2], out normalIndex))
                    ThrowParseException("face normal index");
                if (normalIndex > 0)
                    normalIndices.Add(normalIndex);
            }

            if (textureCoordIndices.Count > 0 && textureCoordIndices.Count != vertexIndices.Count)
                throw new InvalidModelFormatException($"Inconsistent face definition: not all vertices have texture coords in line: {_currentLineNumber}");

            if (normalIndices.Count > 0 && normalIndices.Count != vertexIndices.Count)
                throw new InvalidModelFormatException($"Inconsistent face definition: not all vertices have normal coords in line: {_currentLineNumber}");



            _model.AddFace(new Face
            {
                VertexIndices = vertexIndices.ToArray(),
                NormalIndices = normalIndices.ToArray(),
                TextureCoordIndices = textureCoordIndices.ToArray()
            });
        }

        private void ThrowParseException(string component)
        {
            throw new InvalidModelFormatException($"Can't parse {component} in line ({_currentLineNumber}):" + Environment.NewLine + _currentLine);
        }
    }
}