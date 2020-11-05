using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace InfoInkasService.Core.Interfaces
{
    public interface IDBControl
    {
        public XmlDocument GetCashOutInfo(ICashoutInfoInputs inCashoutInfo);
    }
}
