using System.IO;

namespace ModelConverter
{
    public interface IModelReader
    {
        Model Read(Stream input);
    }
}