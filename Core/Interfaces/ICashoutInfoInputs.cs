using System;

namespace InfoInkasService.Core.Interfaces
{
    public interface ICashoutInfoInputs
    {
        long InChatId { get; set; }
        int InSalePointId { get; set; }
        DateTime InDateCashOut { get; set; }
    }
}