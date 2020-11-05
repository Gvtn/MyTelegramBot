using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoInkasService.InfoInkasServiceAPI.Models.Connfiguration
{
    public class BotConfigurationOptions
    {
        public const string BotConfiguration = "BotConfiguration";
        public string BotToken { get; set; }

        public string Socks5Host { get; set; }

        public int Socks5Port { get; set; }

        //public static string Name { get; set; } = "invoice_messege_bot";

        //public static string Key { get; set; } = "1376052306:AAHLM-H7ouNl7_Czb4y5dsnBTAWZGxgu0fw";
    }
}
