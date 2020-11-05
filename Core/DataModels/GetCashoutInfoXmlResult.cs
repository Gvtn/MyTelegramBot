using InfoInkasService.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace InfoInkasService.Core.DataModels
{
    [XmlRoot(elementName: "cashouts_info")]
    public class GetCashoutInfoXmlResult
    {
        [XmlElement(elementName: "cashout_info")]
        public List<CashOutInfoResult> CashOutInfo { get; set; }
        [XmlElement(ElementName = "error")]
        public string Error { get; set; }

        public class CashOutInfoResult : IEmailData
        {
            [XmlElement(ElementName = "email_recipient")]
            public string EmailRecipient { get; set; }
            [XmlElement(ElementName = "email_sender")]
            public string EmailSender { get; set; }
            [XmlElement(ElementName = "subject")]
            public string Subject { get; set; }
            [XmlElement(ElementName = "cashout_data")]
            public string CashOutData { get; set; }
            
        }
    }
}