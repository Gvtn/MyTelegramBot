using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace InfoInkasService.Core.Interfaces
{
    public interface IEmailData
    {
        public string EmailRecipient { get; set; }
        public string EmailSender { get; set; }
        public string Subject { get; set; }
        public string CashOutData { get; set; }

        public List<IFile> Attachments { get; }
    }
}