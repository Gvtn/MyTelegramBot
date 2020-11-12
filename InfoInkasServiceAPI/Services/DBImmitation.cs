using InfoInkasService.Core.Interfaces;
using System.Xml;

namespace InfoInkasServiceAPI.Services
{
    public class DBImmitation : IDBControl
    {
        private string s => @"<cashouts_info><cashout_info><email_recipient>m.rybalko@ukr.net</email_recipient><email_sender>tqdr00@gmail.com</email_sender><subject> IBoxинкассация,АСО: 7777777Дата: 15.02.20133 10 : 29 : 12Сумма: 133538 </subject><cashout_data> <![CDATA[<h1>Айбокс</h1 ><qrcode> ATM02.34046074;7777777;355409517;2013 - 02 - 15T10: 29 : 12;980 : 13353800 : 1 - 17#2 - 18#5 - 17#10 - 19#20 - 38#50 - 13#100 - 11#200 - 16#500 - 255</qrcode>Інкасація: 355409517 < br > Айбокс,КодЄДРПОУ34046074 < br > IDтермінала: 7777777Адреса: <br > Дата: 15.02.20133 10 : 29 : 12 < br > 1 : 17 50 : 13 < br > 2 : 18 100 : 11 < br > 5 : 17 200 : 16 < br > 10 : 19 500 : 255 < br > 20 : 38 1000 : <br > Сума: 133538 % ,Кол - вокупюр: 40]]></cashout_data></cashout_info ><cashout_info><email_recipient>m.rybalko@ukr.net</email_recipient><email_sender>tqdr00@gmail.com</email_sender> <subject> IBoxинкассация,АСО: 7777777Дата: 15.02.20133 14 : 58 : 00Сумма: 88753 </subject><cashout_data><![CDATA[<h1>Айбокс</h1 ><qrcode>ATM02.34046074;7777777;355570798;2013 - 02 - 15T14: 58 : 00;980 : 8875300 : 1 - 6#2 - 16#5 - 9#10 - 7#20 - 15#50 - 8#100 - 6#200 - 14#500 - 169</qrcode>Інкасація: 355570798 < br > Айбокс,КодЄДРПОУ34046074 < br > IDтермінала: 7777777Адреса: <br > Дата: 15.02.20133 14 : 58 : 00 < br > 1 : 6 50 : 8 < br > 2 : 16 100 : 6 < br > 5 : 9 200 : 14 < br > 10 : 7 500 : 169 < br > 20 : 15 1000 : <br > Сума: 88753 % ,Кол - вокупюр: 250 ]]></cashout_data></cashout_info></cashouts_info>";

        public XmlDocument GetCashOutInfo(ICashoutInfoInputs inCashoutInfo)
        {

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(s);
            return xmlDoc;
        }
    }
}
