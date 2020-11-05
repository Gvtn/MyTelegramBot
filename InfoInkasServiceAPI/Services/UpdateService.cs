using InfoInkasService.Core.Interfaces;
using InfoInkasServiceAPI.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace InfoInkasService.InfoInkasServiceAPI.Models.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly IBotService _botService;
        private readonly ILogger<UpdateService> _logger;
        private readonly IDBControl _dbControl;
        private readonly IEmailService _emailService;

        public UpdateService(IBotService botService, ILogger<UpdateService> logger, IDBControl dbControl, IEmailService emailService)
        {
            _botService = botService;
            _logger = logger;
            _dbControl = dbControl;
            _emailService = emailService;
        }

        public async Task EchoAsync(Update update)
        {
            if (update.Type != UpdateType.Message)
            {
                _logger.LogDebug("Не верный тип запроса");
                return;
            }

            var message = update.Message;

            _logger.LogInformation("Received Message from {0}", message.Chat.Id);

            foreach (var command in _botService.Commands)
            {
                if (command.Contains(message))
                {
                    await command.Execute(message, _botService.Client, _dbControl, _emailService);
                    return;
                }
            }
            _logger.LogDebug("Нет запрошенной команды");
            await _botService.Client.SendTextMessageAsync(message.Chat.Id, "error", replyToMessageId: message.MessageId);
        }
    }
}