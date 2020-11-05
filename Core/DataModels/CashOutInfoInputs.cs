using InfoInkasService.Core.Interfaces;
using System;

namespace InfoInkasService.Core.DataModels
{
    public class CashOutInfoInputs : ICashoutInfoInputs
    {
        public long InChatId { get; set; }
        public int InSalePointId { get; set; }
        public DateTime InDateCashOut { get; set; }
    }
}