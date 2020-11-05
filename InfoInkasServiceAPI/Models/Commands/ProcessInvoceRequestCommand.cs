using InfoInkasService.Core.DataModels;
using InfoInkasService.Core.Interfaces;
using InfoInkasService.Core.Exceptions;
using InfoInkasService.Core.Tools;
using InfoInkasService.OracleDB;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using NLog;
using InfoInkasServiceAPI.Services;
using InfoInkasService.InfoInkasServiceAPI.Models.Services;
using System.Threading;

namespace InfoInkasService.InfoInkasServiceAPI.Models.Commands
{
    public class ProcessInvoceRequestCommand : Command
    {
        public override string Name => @"s";

        private static Dictionary<long, DateTime> chatIdDict = new Dictionary<long, DateTime>();

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ISpamControlService _spamControlSrvice;
        public ProcessInvoceRequestCommand(ISpamControlService spamControlSrvice)
        {
            _spamControlSrvice = spamControlSrvice;
        }
        public override bool Contains(Message message)
        {
            return message.Text.Contains(this.Name);
        }

        public override async Task Execute(Message message, TelegramBotClient client, IDBControl dBControl, IEmailService emailSerivice)
        {
            var chatId = message.Chat.Id;
            var messageId = message.MessageId;
            _logger.Info($"Запрос от {chatId} вида: {message.Text}");
            // 2.1. Спам контроль по chat_id (?)

            if (chatIdDict.ContainsKey(chatId))
            {
                if (chatIdDict[chatId] > DateTime.Now)
                {
                    await client.SendTextMessageAsync(chatId, "error", replyToMessageId: message.MessageId);
                    return;
                }
                // todo: add logging
            }
            else
            {
                chatIdDict.Add(chatId, DateTime.Now.AddSeconds(_spamControlSrvice.SpamDelay));
            }
            // 2.2. Проверка данных запроса на валидность (/s 7777777 23042020 запрос отправки чеков инкассации по терминалу 7777777 за дату 23.04.2020)

            Regex regex = new Regex(@"s\s*\d{7}\s*\d{8}");
            if (!regex.IsMatch(message.Text))
            {
                _logger.Info("Не правильный формат запроса");
                await client.SendTextMessageAsync(chatId, "error", replyToMessageId: message.MessageId);
                return;// todo: add logging
            }

            var args = message.Text.Split(" ");


            var salePointId = args[1];

            DateTime dateCashOut;

            if (!DateTime.TryParseExact(args[2], "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateCashOut))
            {
                _logger.Debug("Не правильный формат даты");
                await client.SendTextMessageAsync(chatId, "error", replyToMessageId: message.MessageId);
                return;
            }

            #region Описание функции БД

            // 2.3. Запрос на ZUKUS  

            //      Функция осуществляет контроль параметра chat_id пользователя бота в белом списке chat_id(зарегистрированных в системе пользователей от которых разрешено принимать запросы). Белый список chat_id ведется в базе Oracle ZUKUS.
            //      Входящие параметры:
            //      - chat_id пользователя бота;
            //      - salepoint_id - точка продажи;
            //      - дата инкассации.

            //      Данные отдаваемые функцией будут содержать :
            //      - Электронный адрес получаемый с договора инкассации действующего на указанную дату (таким образом идентифиуируется получатель в прошлом периоде) по указанному терминалу
            //      - Тему сообщения вида : "IBox инкассация, АСО:"||№АСО||" Дата:"||ДД.ММ.ГГГГ чч:мм:сс||" Сумма:"||Сумма инкассации
            //      - Параметры всех инкасационых чеков в течении указанной даты по терминалу согласно требований ТЗ п.3.7.1 и 3.7.2 ТЗ задачи DEVSPACE-4894

            #endregion

            var cashoutInfoInputs = new CashOutInfoInputs() { InChatId = chatId, InSalePointId = int.Parse(salePointId), InDateCashOut = dateCashOut };

            //var cashoutInfoInputs = new CashOutInfoInputs() { InChatId = 1, InSalePointId = 7777777, InDateCashOut = new DateTime(2013, 2, 15) }; test data

            GetCashoutInfoXmlResult desRes;
            if (!XmlSerializerHelper.TryDeserialize(dBControl.GetCashOutInfo(cashoutInfoInputs), out desRes))
            {
                await client.SendTextMessageAsync(chatId, "error", replyToMessageId: message.MessageId);
                _logger.Debug("Не правильный формат ответа");
                return;
            }

            if (!string.IsNullOrEmpty(desRes.Error))
            {
                await client.SendTextMessageAsync(chatId, "not found", replyToMessageId: message.MessageId);
                _logger.Info($"Получена ошибка при обращении в базу: {desRes.Error}");
                return;
            }

            var emailData = new EmailData()
            {
                EmailRecipient = desRes.CashOutInfo.First().EmailRecipient,
                EmailSender = desRes.CashOutInfo.First().EmailSender,
                Subject = desRes.CashOutInfo.First().Subject,
            };

            StringBuilder combinedData = new StringBuilder();

            foreach (var item in desRes.CashOutInfo)
            {
                combinedData.Append(item.CashOutData);
                combinedData.AppendLine();
                combinedData.AppendLine();
            }

            emailData.CashOutData = combinedData.ToString();
            var emailTask = await emailSerivice.SendEmailAsync(emailData);

            // 2.7. В случае успешной отправки почтового сообщения формирует ответ боту : “ok” в случае любой ошибки формирует ответ боту : error”

            if (!emailTask)
            {
                await client.SendTextMessageAsync(chatId, "error", replyToMessageId: message.MessageId);
                _logger.Debug("Не удалось отправить письмо");
                return;
            }

            // 2.8. Выполняет логирование запросов и ошибок в elasticsearch с учетом данных полученных из функции oracle

            await client.SendTextMessageAsync(chatId, "ok", replyToMessageId: message.MessageId);
        }
    }
}