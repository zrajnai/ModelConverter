using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModelConverter
{
    public class Converter
    {
        public void Convert(
            IModelReader reader,
            IModelWriter writer)
        {
            writer.Write(reader.Read());
        }
        
        public async Task ConvertAsync(
            CancellationToken token,
            IProgress<double> progress,
            IModelReaderAsync reader,
            IModelWriter writer)
        {
            writer.Write(await reader.ReadAsync(token, progress));
        }
    }
}
