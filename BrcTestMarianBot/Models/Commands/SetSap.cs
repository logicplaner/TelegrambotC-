using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;

namespace BrcTestMarianBot.Models.Commands
{
    public class SetSap : Command
    {
         public override string Name => "/setsap";

        public override async Task Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;

            SAPbobsCOM.Company oCompany = new SAPbobsCOM.Company();
            int connect = int.MinValue;
            oCompany.Server = "";
            oCompany.CompanyDB = "";
            oCompany.DbUserName = "";
            oCompany.DbPassword = "";
            oCompany.UserName = "";
            await client.SendTextMessageAsync(chatId, "Введіть логін:");
            oCompany.UserName = message.Text;
            if (oCompany.UserName != null)
             {
            oCompany.Password = "";
             //   await client.DeleteMessageAsync(chatId, message.MessageId);
                oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2017;

                connect = oCompany.Connect();
                string error = "error(0)";
                if (connect != 0)
                {
                    error = oCompany.GetLastErrorDescription();
                    await client.SendTextMessageAsync(chatId, error);
                }
                else
                {
                    await client.SendTextMessageAsync(chatId, "З'єднання з SAP успішне");
                }
            }
        }
    }
}