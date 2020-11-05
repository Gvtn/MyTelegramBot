using InfoInkasService.Core.Interfaces;
using System;

namespace InfoInkasService.Core.DataModels
{
    public class EmailData : IEmailData
    {
        public string EmailRecipient { get; set; }
        public string EmailSender { get; set; }
        public string Subject { get; set; }
        public string CashOutData { get; set; }
    }
}