using System.IO;

namespace ModelConverter.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            new ModelConverter().Convert(
                new FileStream(args[0], FileMode.Open),
                new FileStream(args[1], FileMode.OpenOrCreate) );
        }
    }
}
