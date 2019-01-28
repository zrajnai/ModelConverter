using System.IO;

namespace ModelConverter
{
    public interface IModelWriter
    {
        void Write(Model model);
    }
}