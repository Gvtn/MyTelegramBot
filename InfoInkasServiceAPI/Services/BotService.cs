using InfoInkasService.InfoInkasServiceAPI.Models.Commands;
using InfoInkasService.InfoInkasServiceAPI.Models.Connfiguration;
using InfoInkasService.InfoInkasServiceAPI.Models.Services;
using InfoInkasServiceAPI.Models.Configuration;
using InfoInkasServiceAPI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MihaZupan;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;

namespace InfoInkasService.InfoInkasServiceAPI.Models.Services
{
    public class BotService : IBotService
    {
        private readonly BotConfigurationOptions _botConfig;
        private readonly IConfiguration _config;

        private List<Command> commandsList;
        public IReadOnlyList<Command> Commands { get => commandsList.AsReadOnly(); }
        public TelegramBotClient Client { get; }
        public string WebHookUrl { get; private set; }

        public BotService(IConfiguration config, ISpamControlService spamControlService)
        {
            _config = config;
            _botConfig = new BotConfigurationOptions();
            //{
            //    BotToken = config.GetValue<string>("BotConfiguration:BotToken"),
            //    Socks5Host = config.GetValue<string>("BotConfiguration:Socks5Host"),
            //    Socks5Port = config.GetValue<int>("BotConfiguration:Socks5Port")
            //};

            config.GetSection(BotConfigurationOptions.BotConfiguration).Bind(_botConfig);
            // use proxy if configured in appsettings.*.json
            Client = string.IsNullOrEmpty(_botConfig.Socks5Host)
                ? new TelegramBotClient(_botConfig.BotToken)
                : new TelegramBotClient(
                    _botConfig.BotToken,
                    new HttpToSocks5Proxy(_botConfig.Socks5Host, _botConfig.Socks5Port));
            commandsList = new List<Command>();
            commandsList.Add(new ProcessInvoceRequestCommand(spamControlService));

            Task t = Task.Run(async () => await Client.SetWebhookAsync($"{_config.GetValue<string>("UrlForWebHook")}/{Constants.UpdatePath}"));
            t.Wait();
            t = Task.Run(async () => { var webHookInfo = await Client.GetWebhookInfoAsync(); WebHookUrl = webHookInfo.Url; });
            t.Wait();
        }

        public async Task SetUpWebHook()
        {
            var webHookInfo = await Client.GetWebhookInfoAsync();
            string url = $"{_config.GetValue<string>("UrlForWebHook")}/{Constants.UpdatePath}";
            if (string.Equals(webHookInfo.Url, url))
                return;
            else
            {
                await Client.SetWebhookAsync(url);
            }
        }

        public async Task RemoveWebHook()
        {
            await Client.DeleteWebhookAsync();
            WebHookUrl = "";
        }
    }
}