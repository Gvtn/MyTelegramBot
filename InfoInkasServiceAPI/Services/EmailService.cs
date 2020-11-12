using InfoInkasService.Core.Exceptions;
using InfoInkasService.Core.Interfaces;
using InfoInkasServiceAPI.Models.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InfoInkasServiceAPI.Services
{

    public class EmailService : IEmailService
    {

        // 2.4. По полученным данным(получен не пустой ответ) формирует почтовое сообщение с указанным адресом получателя
        //      (в случае отсутствия данных от процедуры формирует ответ боту :  “not found”.
        //      Тело письма содержит html страницу с текстовой информацией по инкассациям (без QR кодов – к обсуждению)
        //      а так же вложения представляющие собой pdf файлы с QR кодами согласно требований ТЗ п.3.7.1 и 3.7.2 ТЗ задачи DEVSPACE-4894

        // 2.5. Для сформированного почтового сообщения выполняет в стандарте S/MIME наложение электронной цифровой подписи (алгоритм RSA с SHA-256)
        //      Ключ хранится в ресурсах приложения и является одним для всех банков

        // 2.6. Выполняет отправку сообщения на smtp-сервер (в первом ДЦ (не боевом) - pcmx.pc.ibox.local, во втором ДЦ (боевом) - pc2mx.dc2.ibox.local.), настройка smtp должна быть конфигурируемой

        private readonly MailSettings _mailSettings;
        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task<bool> SendEmailAsync(IEmailData data)
        {
            try
            {
                var email = CreateMessage(data);
                await Send(email);

                return true;
            }
            catch { return false; }
        }

        private async Task Send(MimeMessage email)
        {
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        internal static void SignMessage(MimeMessage message)
        {
            // throw new NotImplementedException();
        }


        internal static MimeMessage CreateMessage(IEmailData data)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(MailboxAddress.Parse(data.EmailSender));
            emailMessage.To.Add(MailboxAddress.Parse(data.EmailRecipient));
            emailMessage.Subject = data.Subject;
            var builder = new BodyBuilder();

            if (data.Attachments != null)
            {
                foreach (var file in data.Attachments)
                {
                    builder.Attachments.Add(file.FileName, file.Content, ContentType.Parse(file.ContentType));
                }
            }

            builder.HtmlBody = data.CashOutData;
            emailMessage.Body = builder.ToMessageBody();

            return emailMessage;
        }
    }
}
