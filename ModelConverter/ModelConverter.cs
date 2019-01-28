using System.IO;

namespace ModelConverter
{
    public class ModelConverter
    {
        public void Convert(Stream input, Stream output)
        {
            var model = new OBJModelReader().Read(input);


            new STLModelWriter().Write(output, model);
        }
    }
}
