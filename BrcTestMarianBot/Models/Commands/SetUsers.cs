using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BrcTestMarianBot.Models.Commands
{
    public class SetUsers : Command
    {
        public override string Name => @"/createusers";

        public override async Task Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;

            var keyboard = new InlineKeyboardMarkup(
                new InlineKeyboardButton[][]
                {
                    // Рядок
                    new [] {
                        // Стовпчик
                        new InlineKeyboardButton
                        {
                            Text = "SQL",
                            CallbackData = "select_sql"
                        },

                        // Стовпчик
                        new InlineKeyboardButton
                        {
                            Text = "HANA",
                            CallbackData = "select_hana"
                        },
                    }
                }
            );

            await client.SendTextMessageAsync(chatId, "Виберіть тип бази", replyMarkup: keyboard);
        }
    }
}
