using System;

namespace ModelConverter
{
    internal class InvalidModelFormatException : Exception
    {

        #region Constructors

        public InvalidModelFormatException(string message) : base(message) { }

        #endregion

    }
}