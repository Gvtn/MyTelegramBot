using InfoInkasService.InfoInkasServiceAPI.Models.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;

namespace InfoInkasService.InfoInkasServiceAPI.Models.Services
{
    public interface IBotService
    {
        TelegramBotClient Client { get; }
        IReadOnlyList<Command> Commands { get; }
        public string WebHookUrl { get; }
        public Task SetUpWebHook();
        public Task RemoveWebHook();
    }
}