using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace InfoInkasService.InfoInkasServiceAPI.Models.Services
{
    public interface IUpdateService
    {
        Task EchoAsync(Update update);
    }
}
