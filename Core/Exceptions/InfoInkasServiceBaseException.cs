using System;

namespace InfoInkasService.Core.Exceptions
{
    [Serializable]
    public class InfoInkasServiceBaseException : Exception
    {
        decimal Code { get; }

        public InfoInkasServiceBaseException(decimal code, string message) : base(message)
        {
            Code = code;
        }
    }
}