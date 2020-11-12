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
using System.Net;
using DinkToPdf.Contracts;
using System.Text;
using DinkToPdf;

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
        private readonly IQRService _qrService;
        private readonly IConverter _converter;

        public UpdateController(IUpdateService updateService, ILogger<UpdateController> logger, IEmailService emailService, IBotService botService, IQRService qrService, IConverter converter)
        {
            _updateService = updateService;
            _logger = logger;
            _emailService = emailService;
            _botService = botService;
            _qrService = qrService;
            _converter = converter;
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

        [Route("getqr")]
        [HttpGet]
        public IActionResult GetQR()
        {
            string s = $"<html><head></head><body><img src=\"{ String.Format("data:image/png;base64, {0}", Convert.ToBase64String(_qrService.GetQRBytes("This is qr text")))}\"/></body></html>";
            byte[] array = Encoding.ASCII.GetBytes(s);
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "QRCode",
            };
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = s,
                WebSettings = { DefaultEncoding = "utf-8" },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            var result = _converter.Convert(pdf);

            return File(result, "application/pdf", "qrcode.pdf");
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