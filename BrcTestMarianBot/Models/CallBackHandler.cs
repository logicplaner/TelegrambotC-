using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BrcTestMarianBot.Models
{
    public static class CallBackHandler
    {
        public static async Task callbackHandler(CallbackQuery cb, TelegramBotClient client, List<User> users, int index)
        {
            switch (cb.Data)
            {
                case "select_sql":
                    users[index].status = Status.EnterLogin;
                    await client.EditMessageTextAsync(cb.Message.Chat.Id, cb.Message.MessageId,
                        "Ви вибрали SQL\nВведіть логін:");
                    await client.AnswerCallbackQueryAsync(cb.Id);

                    break;
                case "select_hana":
                    await client.EditMessageTextAsync(cb.Message.Chat.Id, cb.Message.MessageId,
                        "Доступ до HANA заборонено.\nБудь ласка, виберіть іншу Базу даних");
                    /*users[index].status = Status.EnterLogin;
                    await client.EditMessageTextAsync(cb.Message.Chat.Id, cb.Message.MessageId,
                        "Ви вибрали HANA\nВведіть логін:");
                    await client.AnswerCallbackQueryAsync(cb.Id);*/
                    break;
            }
        }
    }
}