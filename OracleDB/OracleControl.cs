using InfoInkasService.Core.DataModels;
using InfoInkasService.Core.Exceptions;
using InfoInkasService.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NLog;
using System;
using System.Xml;

namespace InfoInkasService.OracleDB
{

    public class OracleControl : IDBControl
    {
        OracleDbContext _ora;

        public DbContext OraContext { get { return _ora; } }

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public OracleControl(IOptions<OraConfig> oraConfig)
        {
            _ora = new OracleDbContext(oraConfig.Value.ConnectionStringZukus);
        }

        /// <summary>
        /// Получение инкассаций за дату inCashoutInfo.InDateCashOut (формата ddMMyyyy) по терминалу inCashoutInfo.InSalePointId от inCashoutInfo.InChatId. Процедура проверяет inCashoutInfo.InChatId на наличие в белом списке и в случае успеха возвращает xml:
        /// 
        /// в возвращаемом xml содержиться ноды <cashout_info> в количестве равному числу найденных инкассаций и    содержащие под-ноды
        /// <email_recipient> (Адрес получателя)
        ///<email_sender> (Адрес отправителя) (в реестре \CHAT_CASHOUT_INFO\E-MAILS)
        ///<subject> (Тема письма)
        ///<cashout_data> (Данные чека)
        ///
        /// в случае неуспеха:
        /// 
        /// в возвращаемом xml содержиться нода "error" с описанием ошибки
        /// </summary>
        /// <param name="inCashoutInfo"></param>
        /// <returns>xml документ</returns>
        public XmlDocument GetCashOutInfo(ICashoutInfoInputs inCashoutInfo)
        {
            try
            {
                //вызов процедуры получения инкассаций
                var resFromDb = _ora.GETCASHOUTINFO(inCashoutInfo.InChatId, inCashoutInfo.InSalePointId, inCashoutInfo.InDateCashOut);

                return resFromDb.Result;
            }
            catch (InfoInkasServiceBaseException be)
            {
                throw be;
            }
            catch (Exception e)
            {
                throw e.ToInternalErrorException("Ошибка запроса в БД");
            }
        }
    }
}