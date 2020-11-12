using Core.Interfaces;
using InfoInkasService.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace InfoInkasService.Core.DataModels
{
    public class EmailData : IEmailData
    {
        public string EmailRecipient { get; set; }
        public string EmailSender { get; set; }
        public string Subject { get; set; }
        public string CashOutData { get; set; }

        public List<IFile> Attachments { get; }

        public EmailData()
        {
            Attachments = new List<IFile>();
        }
    }
}