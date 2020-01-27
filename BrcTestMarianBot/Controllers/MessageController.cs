using System.Web.Http;
using System.Web.Http.Results;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using BrcTestMarianBot.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Telegram.Bot;
using System.Data.SqlClient;
using System;
using Telegram.Bot.Types.ReplyMarkups;
using SAPbobsCOM;

namespace BrcTestMarianBot.Controllers
{
    public class MessageController : ApiController
    {
        [Route(@"api/message/update")] // webhook uri part
        public async Task<OkResult> Update([FromBody]Update update)
        {
            var commands = Bot.Commands;
            var client = await Bot.Get();
            Telegram.Bot.Types.Message message;
            int index;
            long chatId;

            switch (update.Type)
            {
                case UpdateType.Message:
                    message = update.Message;
                    chatId = message.Chat.Id;
                    var messageId = message.MessageId;
                    index = checkUser(chatId);
                    if (message.Text[0] == '/')
                    {
                        Bot.users[index].status = Status.Usual;
                    }
                    if (Bot.users[index].status == Status.Usual)
                    {
                        foreach (var command in commands)
                        {
                            if (command.Contains(message.Text))
                            {
                                await command.Execute(message, client);
                                break;
                            }
                        }
                    }
                    else await anotherStatus(message, index, client);

                    break;
                case UpdateType.CallbackQuery:
                    chatId = update.CallbackQuery.Message.Chat.Id;

                    index = checkUser(chatId);

                    await CallBackHandler.callbackHandler(update.CallbackQuery, client, Bot.users, index);
                    break;
            }
            return Ok();
        }

        private int checkUser(long chatId)
        {
            int index = Bot.users.FindIndex(user => chatId == user.Id);

            if (index == -1)
            {
                Bot.users.Add(new Models.User(chatId));
                index = Bot.users.Count - 1;
            }
            return index;
        }
        private async Task anotherStatus(Telegram.Bot.Types.Message message, int index, TelegramBotClient client)
        {
           
            switch (Bot.users[index].status)
            {
                case Status.EnterLogin:
                    Bot.users[index].status = Status.EnterPassword;
                    Bot.users[index].login = message.Text;
                    await client.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                    await client.SendTextMessageAsync(message.Chat.Id, "Введіть пароль:");
                    break;
                case Status.EnterPassword:
                    Bot.users[index].password = message.Text;
                    Bot.users[index].status = Status.SelectDB;
                    await client.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                    string serverName = "";
                    SqlConnection connection;
                    string connetionString = $"Data Source={serverName};User ID={Bot.users[index].login};Password={Bot.users[index].password}";
                    connection = new SqlConnection(connetionString);
                    try
                    {
                        connection.Open();
                        Bot.users[index].auth = true;
                        Bot.users[index].status = Status.SelectDB;
                       // await client.SendTextMessageAsync(message.Chat.Id, "З'єднання встановлено.\nДоступні бази даних:");
                        string answerdb = "";
                        string querydb = "Select * from sys.databases";
                        SqlCommand command = new SqlCommand(querydb, connection);
                        int buttonsCount = 0;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                for (int i = 1; i >= 0; i--)
                                {
                                    answerdb += reader.GetValue(i).ToString() + " ";
                                }
                                answerdb += "\n";
                                buttonsCount++;
                            }
                        }
                        await client.SendTextMessageAsync(message.Chat.Id, "З'єднання встановлено.\nСкористайтеся клавіатурою та введіть номер необхідної бази даних:");
                        await client.SendTextMessageAsync(message.Chat.Id, answerdb, replyMarkup: createKeyboardbd(buttonsCount));
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        Bot.users[index].status = Status.Usual;
                        await client.SendTextMessageAsync(message.Chat.Id, "EXCEPTION: " + ex.Message);
                    }
                    break;
                case Status.SelectDB:
                    string msg = message.Text;
                    int database_id;
                    if (Int32.TryParse(msg, out database_id))
                    {
                       
                       string server = "";
                        SqlConnection connections;
                        string connetionstr = $"Data Source={server};User ID={Bot.users[index].login};Password={Bot.users[index].password}";
                        connections = new SqlConnection(connetionstr);
                        try
                        {
                            connections.Open();
                            string nameDB = "";
                            string queryd = $"Select name from sys.databases where database_id={database_id}";
                            SqlCommand command = new SqlCommand(queryd, connections);
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                reader.Read();
                                nameDB = reader.GetValue(0).ToString();
                            }
                            Bot.users[index].nameDB = nameDB;
                            Bot.users[index].status = Status.EnterSapLogin;
                            string answerd = $"Вхід в {Bot.users[index].nameDB}.\nВведіть логін в SAP:";
                            await client.SendTextMessageAsync(message.Chat.Id, answerd);
                            connections.Close();
                        }
                        catch (Exception ex)
                        {
                            Bot.users[index].status = Status.Usual;
                            await client.SendTextMessageAsync(message.Chat.Id, "EXCEPTION: " + ex.Message);
                        }
                    }
                    else
                    {
                            await client.SendTextMessageAsync(message.Chat.Id, "Помилка. Введіть число:");
                        }
                       
                    break;
                case Status.EnterSapLogin:
                    Bot.users[index].status = Status.EnterSapPassword;
                    Bot.users[index].saplogin = message.Text;
                    await client.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                    await client.SendTextMessageAsync(message.Chat.Id, "Введіть пароль для підключення до SAP:");
                    break;
                case Status.EnterSapPassword:
                    Bot.users[index].sappassword = message.Text;
                    await client.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                    Company oCompany = new Company();
                    oCompany.Server = "";
                    oCompany.CompanyDB = Bot.users[index].nameDB;
                    oCompany.DbUserName = Bot.users[index].login;
                    oCompany.DbPassword = Bot.users[index].password;
                    oCompany.UserName = Bot.users[index].saplogin;
                    oCompany.Password = Bot.users[index].sappassword;
                    oCompany.DbServerType = BoDataServerTypes.dst_MSSQL2017;
                    
                    int isConnect = oCompany.Connect();
                    string error = "error(0)";
                    if (isConnect != 0)
                    {
                        error = oCompany.GetLastErrorDescription();
                        await client.SendTextMessageAsync(message.Chat.Id, error);
                    }
                    else
                    {
                        await client.SendTextMessageAsync(message.Chat.Id, "З'єднання з SAP успішне");
                        await createUsers(message, index, client, oCompany);
                      
                    }
                   
                    break;
                case Status.CreateSapUser:
                   
                    Bot.users[index].status = Status.Usual;
                    break;

            }
        }
        private async Task createUsers(Telegram.Bot.Types.Message message, int index, TelegramBotClient client, Company ocompany)
        {
            Bot.users[index].status = Status.CreateSapUser;
            string servername = "";
            string userCode;
            string allUsers = "";
            string createUsers = "";
            string noCreatedUsers = "";
            string path = @"C:\inetpub\wwwroot\mdolynskyi\users.txt";
            string query;
            string answer;
            await client.SendTextMessageAsync(message.Chat.Id, "Створюю нових користувачів...");
            SqlConnection conn;
            string connectionStr = $"Data Source={servername};Initial Catalog={Bot.users[index].nameDB};User ID={Bot.users[index].login};Password={Bot.users[index].password}";
            conn = new SqlConnection(connectionStr);
            try
            {
                conn.Open();
                using (System.IO.StreamReader read = new System.IO.StreamReader(path))
                {
                    string line;
                    while ((line = read.ReadLine()) != null)
                    {
                        allUsers += line + "\n";
                        userCode = line;
                        query = $"SELECT * FROM OUSR WHERE USER_CODE='{userCode}'";

                        SqlCommand command = new SqlCommand(query, conn);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            reader.Read();
                            if (!reader.HasRows)
                            {
                                createUsers += line + "\n";
                                Users user = ocompany.GetBusinessObject(BoObjectTypes.oUsers);
                                user.UserCode = line;
                                user.UserName = line;
                                user.UserPassword = "Pract_2019";
                                user.Superuser = BoYesNoEnum.tYES;
                                user.Add();
                            }
                            else
                            {
                                noCreatedUsers += line + "\n";
                             }
                        }
                    }

                    answer = "Вказані:\n";
                    answer += allUsers;
                    answer += "\nДобавлені:\n";
                    answer += createUsers;
                    answer += "\nНе добавлені:\n";
                    answer += noCreatedUsers;

                    await client.SendTextMessageAsync(message.Chat.Id, answer);

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                await client.SendTextMessageAsync(message.Chat.Id, "EXCEPTION " + ex.Message);
            }
        }
        private ReplyKeyboardMarkup createKeyboardbd(int buttonsCount)
        {
            int cols = 4; // скільки кнопок в ряді
            int lastRow = buttonsCount % cols; //скільки кнопок в останньому рядку
            int rows = buttonsCount / cols + 1; // скільки рядів
            int num = 1; // число текст кнопки

            Telegram.Bot.Types.ReplyMarkups.KeyboardButton[][] buttons = new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[rows][];
            for (int i = 0; i < rows - 1; i++)
            {
                var btns = new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[cols];
                for (int j = 0; j < cols; j++)
                {
                    btns[j] = new Telegram.Bot.Types.ReplyMarkups.KeyboardButton { Text = (num++).ToString() };
                }
                buttons[i] = btns;
            }
            var btn = new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[lastRow];
            for (int i = 0; i < lastRow; i++)
            {
                btn[i] = new Telegram.Bot.Types.ReplyMarkups.KeyboardButton { Text = (num++).ToString() };
            }
            buttons[rows - 1] = btn;
            var keybrd = new ReplyKeyboardMarkup(buttons);
            keybrd.OneTimeKeyboard = true;
            keybrd.ResizeKeyboard = true;

            return keybrd;
        }
    }
}