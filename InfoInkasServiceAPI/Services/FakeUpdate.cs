using InfoInkasService.Core.DataModels;
using InfoInkasService.Core.Interfaces;
using InfoInkasService.InfoInkasServiceAPI.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace InfoInkasServiceAPI.Services
{
    public class FakeUpdate : IUpdateService
    {
        private IDBControl _dbConntrol;
        private static int i = 0;
        public FakeUpdate(IDBControl dbControl)
        {
            _dbConntrol = dbControl;
        }
        public Task EchoAsync(Update update)
        {
            ICashoutInfoInputs inputs = new CashOutInfoInputs();
            var result = _dbConntrol.GetCashOutInfo(inputs);
            i++;
            Task t = Task.Run(() => Console.WriteLine($"{i} attempt: {result.InnerText}"));
            t.Wait();
            return t;
        }
    }
}