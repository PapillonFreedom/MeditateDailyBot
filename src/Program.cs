//@MeditateDailyBot - Bot for daily meditation now in Telegram!
//Nuget ~2021 year


using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MeditateBotDev {
      internal static class Program {
        private static ITelegramBotClient? _meditateDailyBot;


        [Obsolete("Obsolete")]
        private static void Main() {
            _meditateDailyBot = new TelegramBotClient("YOUR_TOKEN_HERE");

         User mePappibot = _meditateDailyBot.GetMeAsync().Result;

            Console.WriteLine(
                $"Session started. Bot {mePappibot.FirstName} is UP."
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
            if (e.Message.Text == "/start")
            {
                await _meditateDailyBot.SendTextMessageAsync(e.Message.Chat.Id, "Okey, Let's do some meditation!", default, null, true, true);
            }
            //получить данные о пользователях, которые воспользовались ботом
            if (e.Message.Text != null)
            {
                Console.WriteLine($"At {e.Message.Date} In chat {e.Message.Chat.Id} was a new User: {e.Message.From.Username}, {e.Message.From.Id}, {e.Message.From.FirstName}, {e.Message.From.LastName}.");
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
            await _meditateDailyBot.SendTextMessageAsync(e.Message.Chat.Id, "Just play audio below:", default, null, true, false, 0, false, rkm);
            
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
                    await _meditateDailyBot.SendVoiceAsync(e.Message.Chat, ($"https://github.com/PapillonFreedom/MeditateDailyBot/raw/master/sounds/{e.Message.Text}min.ogg"), $"Have a nice meditation for next {e.Message.Text} minutes");
                    break;
                
                case "/start":
                    await _meditateDailyBot.SendTextMessageAsync(e.Message.Chat, $"Please, enter the following count of minutes: 1, 5, 10, 15, 20, 30, 45, 60");
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