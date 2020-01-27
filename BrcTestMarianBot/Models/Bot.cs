using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using BrcTestMarianBot.Models.Commands;
using System;
using Telegram.Bot.Args;

namespace BrcTestMarianBot.Models
{
    public static class Bot
    {
        private static TelegramBotClient client;
        private static List<Command> commandsList;
        public static List<Models.User> users = new List<User>();
        public static IReadOnlyList<Command> Commands {
            get
            {
                return commandsList.AsReadOnly();
            }
        }

        public static Func<object, CallbackQueryEventArgs, Task> OnCallbackQuery { get; internal set; }

        //initializing client for our Bot
        public static async Task<TelegramBotClient> Get()
        {
            if(client != null)
            {
                return client;
            }

            commandsList = new List<Command>();
            //our commands
            commandsList.Add(new HelloCommand());
            commandsList.Add(new InlineButton());
            commandsList.Add(new SetUsers());
            commandsList.Add(new Start());
            commandsList.Add(new SetSap());

            client = new TelegramBotClient(AppSettings.Key);
            var hook = string.Format(AppSettings.Url, "api/message/update");
            await client.SetWebhookAsync(hook);

            return client;
        }

        internal static Task SendTextMessageAsync(long id, string v, int replyToMessageId)
        {
            throw new NotImplementedException();
        }

        internal static Task AnswerCallbackQueryAsync(string id)
        {
            throw new NotImplementedException();
        }

        internal static Task AnswerCallbackQueryAsync(string id, string v1, bool v2)
        {
            throw new NotImplementedException();
        }
    }
}