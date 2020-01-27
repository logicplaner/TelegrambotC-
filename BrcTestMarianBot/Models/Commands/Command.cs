using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Threading.Tasks;

namespace BrcTestMarianBot.Models.Commands
{
    public abstract class Command
    {
        public abstract string Name { get; }

        public abstract Task Execute(Message message, TelegramBotClient client);
        
        public bool Contains (string command)
        {

            return command.Contains(this.Name);// && command.Contains(AppSettings.Name);
        }
    }
}