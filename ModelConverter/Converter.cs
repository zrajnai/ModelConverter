using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModelConverter
{
    public class Converter
    {
        public void Convert(IModelReader reader, IModelWriter writer)
        {
            writer.Write(reader.Read());
        }
        
        public async Task<IModel> ConvertAsync(
            CancellationToken token,
            IProgress<double> progress,
            IModelReaderAsync reader,
            IModelWriter writer)
        {
            var model = await reader.ReadAsync(token, progress);
            writer.Write(model);
            return model;
        }
    }
}
