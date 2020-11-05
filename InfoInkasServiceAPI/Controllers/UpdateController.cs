using InfoInkasService.Core.DataModels;
using InfoInkasService.Core.Interfaces;
using InfoInkasService.InfoInkasServiceAPI.Models.Services;
using InfoInkasService.OracleDB;
using InfoInkasServiceAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Telegram.Bot.Types;
using InfoInkasServiceAPI.Models.Configuration;
using Elasticsearch.Net;

namespace InfoInkasService.InfoInkasServiceAPI.Controllers
{
    [ApiController]
    [Route(Constants.UpdatePath)]
    public class UpdateController : Controller
    {
        private readonly ILogger<UpdateController> _logger;

        private readonly IUpdateService _updateService;
        private readonly IEmailService _emailService;
        private readonly IBotService _botService;

        public UpdateController(IUpdateService updateService, ILogger<UpdateController> logger, IEmailService emailService, IBotService botService)
        {
            _updateService = updateService;
            _logger = logger;
            _emailService = emailService;
            _botService = botService;
        }

        // 1. Метод для проверки версии приложения 

        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            return await Task<ActionResult<string>>.Run(() => $"Current assebly number is {Assembly.GetExecutingAssembly().GetName().Version.ToString(4)}").ConfigureAwait(false);
        }

        [Route(Constants.SetWebHook)]
        [HttpGet]
        public async Task<ActionResult<string>> SetWebHook()
        {
            await _botService.SetUpWebHook();
            return $"Webhook url set to: {_botService.WebHookUrl}";
        }

        [Route(Constants.RemoveWebHoook)]
        [HttpGet]
        public async Task<ActionResult<string>> RemoveWebHoook()
        {
            await _botService.RemoveWebHook();
            return $"Webhook url set to: {_botService.WebHookUrl}";
        }

        [Route(Constants.GetWebHook)]
        [HttpGet]
        public ActionResult<string> GetWebHook()
        {
            return $"Webhook url set to: {_botService.WebHookUrl}";
        }

        [Route(Constants.SendEmail)]
        [HttpGet]
        public OkResult Send()
        {
            IEmailData mailRequest = new EmailData() { EmailSender = "someemail@gmail.com", EmailRecipient = "someemail@gmail.com", Subject = "some subject", CashOutData = "somme data" };
            Task t = Task.Run(async () => { await _emailService.SendEmailAsync(mailRequest); });
            t.Wait();
            return Ok();
        }

        // 2. Обработчик запроса инкассационных чеков

        [HttpPost]
        public async void Post([FromBody] Update update)
        {
            await _updateService.EchoAsync(update);
        }
    }
}

// TODO: Tests
// TODO: Config (spam controll parameters, connection strings, logging)
// TODO: Add cert