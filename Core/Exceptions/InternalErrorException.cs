using System;

namespace InfoInkasService.Core.Exceptions
{
    /// <summary>
    ///Внутренняя ошибка процессинга
    /// </summary>
    [Serializable]
    public class InternalErrorException : InfoInkasServiceBaseException
    {
        public InternalErrorException() : base(1015, "Ошибка ПО процессинга")
        {
        }
    }
}