using System;
using System.Diagnostics;
using System.IO;

namespace ModelConverter.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                if (args.Length < 0)
                {
                    Console.WriteLine("Specify name of input file as argument.");
                    return;
                }

                if (!File.Exists(args[0]))
                {
                    Console.WriteLine("Input file not found.");
                    return;
                }

                var input = new FileStream(args[0], FileMode.Open);
                var output = new FileStream($"{Path.GetFileNameWithoutExtension(args[0])}.stl", FileMode.OpenOrCreate);
                using (input)
                using (output)
                {
                    var reader = new OBJModelReader(input);
                    var writer = new STLModelWriter(output);
                    new ModelConverter().Convert(reader, writer);
                }
                Console.WriteLine("Conversion successful.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error during conversion:" + Environment.NewLine + e.Message);

                if (Debugger.IsAttached)
                    Console.ReadLine();
            }

        }
    }
}
