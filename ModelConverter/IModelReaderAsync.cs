using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ModelConverter
{
    public interface IModelReaderAsync
    {

        #region Public Properties

        string SupportedExtension { get; }

        string FormatDescription { get; }

        #endregion

        #region Public Methods

        Task<IModel> ReadAsync(Stream stream, CancellationToken token, IProgress<double> progress);

        #endregion

    }
}