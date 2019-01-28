using System;
using System.IO;

namespace ModelConverter
{
    public class STLModelWriter : IModelWriter
    {
        private readonly Stream _output;
        private Model _model;

        public STLModelWriter(Stream output)
        {
            _output = output ?? throw new ArgumentNullException(nameof(output));
        }

        public void Write(Model model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            
            using (var bw = new BinaryWriter(_output))
            {
                bw.Write(new byte[80]);
                bw.Write((uint) _model.Faces.Count);

                foreach (var face in _model.Faces)
                {

                    WriteNormal(bw, face);

                    WriteVertex(bw, face.VertexIndices[0]);
                    WriteVertex(bw, face.VertexIndices[1]);
                    WriteVertex(bw, face.VertexIndices[2]);

                    bw.Write((ushort) 0);
                }
            }

        }

        private void WriteNormal(BinaryWriter bw, Face face)
        {
            bw.Write((float) face.Normal.X);
            bw.Write((float) face.Normal.Y);
            bw.Write((float) face.Normal.Z);
        }

        private void WriteVertex(BinaryWriter bw, int vertexIndex)
        {
            var v1 = _model.Vertices[vertexIndex];
            bw.Write((float) v1.X);
            bw.Write((float) v1.Y);
            bw.Write((float) v1.Z);
        }
    }
}