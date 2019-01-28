
namespace ModelConverter
{
    public class ModelConverter
    {
        public void Convert(
            IModelReader reader,
            IModelWriter writer)
        {
            writer.Write(reader.Read());
        }
    }
}
