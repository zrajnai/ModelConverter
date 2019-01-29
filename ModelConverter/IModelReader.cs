
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModelConverter
{
    public interface IModelReader
    {
        IModel Read();
    }

    public interface IModelReaderAsync
    {
        Task<IModel> ReadAsync(CancellationToken token, IProgress<double> progress);
    }
}