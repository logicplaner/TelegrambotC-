using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace BrcTestMarianBot.Models.Commands
{
    public class Start : Command
    {
        public override string Name => "start";
        private ReplyKeyboardMarkup createKeyboard()
        {
            int cols = 1; // скільки кнопок в ряді
            int rows = 3;
            string command; // число текст кнопки

            Telegram.Bot.Types.ReplyMarkups.KeyboardButton[][] buttons = new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[rows][];

            for (int i = 0; i < rows - 1; i++)
            {
                var btns = new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[cols];
               
                
                    btns[1] = new Telegram.Bot.Types.ReplyMarkups.KeyboardButton { Text = ("/hello").ToString() };
                
                buttons[i] = btns;
            }
            var keybrd = new ReplyKeyboardMarkup(buttons);

            keybrd.OneTimeKeyboard = true;
            keybrd.ResizeKeyboard = true;

            return keybrd;
        }
        public override async Task Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var messageId = message.MessageId;
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                                               {
                        new [] // first row
                        {
                            InlineKeyboardButton.WithCallbackData("Hello!", "/hello"),
                            InlineKeyboardButton.WithCallbackData("Inline Button", "/inline"),
                             InlineKeyboardButton.WithCallbackData("Inline Button", "/setusers")
                        }
                    });

            await client.SendTextMessageAsync(chatId, "Привіт!\n Скористайтеся командою /createusers \nдля створення нових користувачів", replyToMessageId: messageId
              //  replyMarkup: createKeyboard()
                );
            //   return;
           // await client.SendTextMessageAsync(chatId, "Hello!", replyToMessageId: messageId);
        }
    }
}