using System;
using System.IO;

namespace ModelConverter.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var input = new FileStream("teapot.obj", FileMode.Open);
                var output = new FileStream("teapot.stl", FileMode.OpenOrCreate);
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

            }

            Console.ReadLine();
        }
    }
}
