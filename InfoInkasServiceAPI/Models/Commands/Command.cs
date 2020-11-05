using InfoInkasService.Core.Interfaces;
using InfoInkasServiceAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace InfoInkasService.InfoInkasServiceAPI.Models.Commands
{
    public abstract class Command
    {
        public abstract string Name { get; }

        public abstract Task Execute(Message message, TelegramBotClient client, IDBControl dBControl, IEmailService emailService);

        public abstract bool Contains(Message message);
    }
}
