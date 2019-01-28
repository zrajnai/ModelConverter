using System.IO;

namespace ModelConverter
{
    internal class OBJModelReader : IModelReader
    {
        public Model Read(Stream input)
        {
            var model = new Model();
            var tr = new StreamReader(input);

            while (!tr.EndOfStream)
            {
                var line = tr.ReadLine();
                if (line == null)
                    break;

                if (string.IsNullOrEmpty(line))
                    continue;

                if (line.StartsWith("#"))
                    continue;

                if (line.StartsWith("v"))
                    ReadVertex();

                else if (line.StartsWith("f"))
                    ReadFace();

                else if (line.StartsWith("vn"))
                    ReadVertexNormal();

                else if (line.StartsWith("vt"))
                    ReadTextureCoord();
            }

            return model;
        }

        private void ReadTextureCoord()
        {
            throw new System.NotImplementedException();
        }

        private void ReadVertexNormal()
        {
            throw new System.NotImplementedException();
        }

        private void ReadFace()
        {
            throw new System.NotImplementedException();
        }

        private void ReadVertex()
        {
            throw new System.NotImplementedException();
        }
    }
}