
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ModelConverter
{
    public interface IModelReaderAsync
    {
        Task<IModel> ReadAsync(Stream stream, CancellationToken token, IProgress<double> progress);

        string SupportedExtension { get; }
        string FormatDescription { get; }
    }
}