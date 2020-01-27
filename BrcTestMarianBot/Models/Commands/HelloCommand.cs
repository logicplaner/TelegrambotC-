using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Threading.Tasks;

namespace BrcTestMarianBot.Models.Commands
{
    public class HelloCommand : Command
    {
        public override string Name => "hello";

        public override async Task Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var messageId = message.MessageId;

           await client.SendTextMessageAsync(chatId, "Hello!", replyToMessageId: messageId);
        }
    }
}