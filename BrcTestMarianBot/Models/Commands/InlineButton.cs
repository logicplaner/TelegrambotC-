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
    public class InlineButton : Command
    {
        public override string Name => "inline";

        public override async Task Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var messageId = message.MessageId;


              var myInlineKeyboard = new InlineKeyboardMarkup(

                 new InlineKeyboardButton[][]
                 {
                     //row1
                     new []
                     {
                         //collumn1
                          new InlineKeyboardButton
                              {
                                  Text = "One",
                                  CallbackData = "callback1"
                              },

                          //collumnw
                          new InlineKeyboardButton
                              {
                                  Text = "Two",
                                  Url = "google.com"
                              }

                      }
                  }
             );
            Bot.OnCallbackQuery += async (object sc, Telegram.Bot.Args.CallbackQueryEventArgs ev) =>
            {
                var message1 = ev.CallbackQuery.Message;
                if (ev.CallbackQuery.Data == "callback1")
                {
                    await Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, "You hav choosen " + ev.CallbackQuery.Data, true);
                }
                else
                if (ev.CallbackQuery.Data == "callback2")
                {
                    await Bot.SendTextMessageAsync(message1.Chat.Id, "тест", replyToMessageId: message1.MessageId);
                    await Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id); // отсылаем пустое, чтобы убрать "частики" на кнопке
                }
            };
            await client.SendTextMessageAsync(chatId, "inline", replyMarkup: myInlineKeyboard);
        }
    }
}