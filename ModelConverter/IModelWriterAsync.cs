using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ModelConverter
{

    public interface IModelWriterAsync
    {
        Task WriteAsync(Stream stream, CancellationToken token, IProgress<double> progress, IModel model);

        string SupportedExtension { get; }

        string FormatDescription { get; }

    }
}