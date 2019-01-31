using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ModelConverter
{

    public interface IModelWriterAsync
    {

        #region Public Properties

        string SupportedExtension { get; }

        string FormatDescription { get; }

        #endregion

        #region Public Methods

        Task WriteAsync(Stream stream, CancellationToken token, IProgress<double> progress, IModel model);

        #endregion

    }
}