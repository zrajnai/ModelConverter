using System;

namespace ModelConverter
{
    internal class InvalidModelFormatException : Exception
    {
        public InvalidModelFormatException(string message) : base(message)
        {
            
        }
    }
}