using System.IO;

namespace ModelConverter
{
    public interface IModelWriter
    {
        void Write(Stream output, Model model);
    }
}