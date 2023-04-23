//@MeditateDailyBot - Bot for daily meditation now in Telegram!
//Place Your token to file token.txt and point path in tokenFilePath

using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace MeditateBotDev {
      internal static class Program {
        private static ITelegramBotClient? _meditateDailyBot;


        [Obsolete("Obsolete")]
        private static void Main()
        {
            //string tokenFilePath = "/home/papillon/Projects/MeditateBotDev/token.txt";  //test
            string tokenFilePath = "/home/ubuntu/token.txt";  //prod

            string botToken = null;

            if (File.Exists(tokenFilePath))
            { 
                botToken = File.ReadAllText(tokenFilePath).Trim();
                Console.WriteLine("File token.txt {0} exists.", botToken);
            }
            else
            {
                Console.WriteLine("File token.txt not exists.");
                Environment.Exit(0);
            }
            
            _meditateDailyBot = new TelegramBotClient(botToken);

            User botName = _meditateDailyBot.GetMeAsync().Result;

            Console.WriteLine(
                $"Session started. Bot {botName.FirstName} is UP."
            );

            if (_meditateDailyBot != null)
            {
                _meditateDailyBot.OnMessage += Bot_OnMessage!;
                _meditateDailyBot.StartReceiving();
            }

            Thread.Sleep(int.MaxValue);
        }
        
        private static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            /*if (e.Message.Text == "/start")
            {
                await _meditateDailyBot.SendTextMessageAsync(e.Message.Chat.Id, "Okey, Let's do some meditation!", default, null, true, true);
            }*/
            
            //получаем данные о пользователях, которые воспользовались ботом
            if (e.Message.Text != null)
            {
                Console.WriteLine($"At {e.Message.Date} In chat {e.Message.Chat.Id} was a new User: {e.Message.From.FirstName}, {e.Message.From.LastName}.");
            }

            //начало создания клавиатуры предлагаемых вариантов времени
            var rkm = new ReplyKeyboardMarkup
            {
                Keyboard = new KeyboardButton[][]
                {
                   new KeyboardButton[] {new KeyboardButton("1"), new KeyboardButton("5"), new KeyboardButton("10")},
                   new KeyboardButton[] {new KeyboardButton("15"), new KeyboardButton("20")},
                   new KeyboardButton[] {new KeyboardButton("30"), new KeyboardButton("45"), new KeyboardButton("60")}
                }
            };
            //Строка ниже регулярно обновляет экранную клавиатуру с выбором минут. Если её закомментировать, то клавиатура у новых пользователей не появится
            //await _meditateDailyBot.SendTextMessageAsync(e.Message.Chat.Id, "Just play audio below:", default, null, true, false, 0, false, rkm);
            
            //сами варианты для выбора
            switch (e.Message.Text)
            {
                case null:
                    return;

                case "1":
                case "5":
                case "10":
                case "15":
                case "20":
                case "30":
                case "45":
                case "60":
                    // в ответ на кнопку N выводим аудио в N минут
                    //var voicePathFile = $"/home/{user}/Projects/MeditateDailyBot/sounds/{e.Message.Text}min.ogg"; //test
                    var voicePathFile = $"/home/ubuntu/sounds/{e.Message.Text}min.ogg"; //prod
                    await _meditateDailyBot.SendVoiceAsync(e.Message.Chat,
                      new InputOnlineFile(File.Open(voicePathFile, FileMode.Open)), $"{e.Message.Text}mins", default, null,Convert.ToInt32(e.Message.Text) * 60 + 4);

                    break;
                case "/start":
                    await _meditateDailyBot.SendTextMessageAsync(e.Message.Chat, $"Please, enter the following count of minutes: 1, 5, 10, 15, 20, 30, 45, 60",default, null, true, false, 0, false, rkm);
                    break;
                default:
                    // ответ на любое другое значение просим ввести корректное значение
                    await _meditateDailyBot.SendTextMessageAsync(e.Message.Chat, $"{e.Message.Text} is not support. Please, select the following count of minutes: 1, 5, 10, 15, 20, 30, 45, 60");
                    break;
                
                
            }
            //конец создания клавиатуры предлагаемых вариантов времени
        }
    }
    }