using System;
using System.Collections.Generic;
using System.Text;

namespace InfoInkasService.Core.Exceptions
{
    public static class ExceptionExtentions
    {
        // private static readonly Logger _looger = LogManager.GetCurrentClassLogger();
        public static InfoInkasServiceBaseException ToInternalErrorException(this Exception e, string errMsgHeader)
        {
            var errMessage = e.Message;
            // _looger.Debug($"{errMsgHeader} : {errMessage} | {e.StackTrace}");

            return new InternalErrorException();
        }
    }
}
