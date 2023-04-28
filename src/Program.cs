//@MeditateDailyBot - Bot for daily meditation now in Telegram!
//Place Your token to file token.txt and point path in tokenFilePath

using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace MeditateBotDev {
      internal static class Program {
        private static ITelegramBotClient? _meditateDailyBot;
        private static string? _pathFile;

        [Obsolete("Obsolete")]
        public static void Main()
        {

            const string test = "/home/papillon/Projects/MeditateBotDev/token.txt";  //test
            const string prod = "/home/ubuntu/token.txt"; //prod
            string? botToken = null;
            _pathFile = "";
            
            if (File.Exists(test))
            {
                botToken = File.ReadAllText(test).Trim();
                Console.WriteLine("File token.txt {0} exists.\n Current ENV is TEST", botToken);
                _pathFile = "/home/papillon/Projects/MeditateDailyBot/sounds/";
            }
            else if (File.Exists(prod))
            {
                botToken = File.ReadAllText(prod).Trim();
                Console.WriteLine("File token.txt {0} exists.\n Current ENV is PROD", botToken);
                _pathFile = "/home/ubuntu/sounds/";
            }
            else
            {
                Console.WriteLine("File token.txt not exists on the test or prod paths.");
                Environment.Exit(0);
            }

            _meditateDailyBot = new TelegramBotClient(botToken);

            User botName = _meditateDailyBot.GetMeAsync().Result;

            Console.WriteLine($"Session started. Bot {botName.FirstName} is UP.");
            
            _meditateDailyBot.OnMessage += Bot_OnMessage;
            _meditateDailyBot.StartReceiving();

            Thread.Sleep(int.MaxValue);
        }
        
        [Obsolete("Obsolete")]
        private static async void Bot_OnMessage(object? sender, MessageEventArgs e)
        {
            
            
            //get data on users who have used the bot
            if (e.Message.Text != null)
            {
                Console.WriteLine($"At {e.Message.Date} In chat {e.Message.Chat.Id} was a new call by User: {e.Message.From.FirstName}, {e.Message.From.LastName}.");
            }

            //creating a keyboard of the proposed time options
            var rkm = new ReplyKeyboardMarkup
            {
                Keyboard = new[]
                {
                   new[] {new KeyboardButton("1"), new KeyboardButton("5"), new KeyboardButton("10")},
                   new[] {new KeyboardButton("15"), new KeyboardButton("20")},
                   new[] {new KeyboardButton("30"), new KeyboardButton("45"), new KeyboardButton("60")}
                }
            };
 
            // start of the options for choosing
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

                    // in response to button N, take audio in N minutes
                    var voicePathFile = $"{_pathFile+e.Message.Text}min.ogg";
                    var audioPathFile = $"{_pathFile+e.Message.Text}min.mp3";

                    try
                    {
                        // try to send a voice message
                        await using var fileStream = new FileStream(voicePathFile, FileMode.Open);
                        var inputOnlineFile = new InputOnlineFile(fileStream);
                        await _meditateDailyBot!.SendVoiceAsync(e.Message.Chat,
                            inputOnlineFile, $"{e.Message.Text}mins", default, null,
                            Convert.ToInt32(e.Message.Text) * 60 + 4);
                        await fileStream.DisposeAsync();
                        break;
                    }
                    catch (ApiRequestException ex)
                    {
                        if (ex.Message.Contains("Bad Request: VOICE_MESSAGES_FORBIDDEN"))
                        {
                            try
                            {
                                // try to send an audio message
                                await using var fileStream = new FileStream(audioPathFile, FileMode.Open);
                                var inputOnlineFile = new InputOnlineFile(fileStream);
                                await _meditateDailyBot!.SendAudioAsync(e.Message.Chat,
                                    inputOnlineFile, null, default, null,
                                    Convert.ToInt32(e.Message.Text) * 60 + 4, "Have a nice meditation", $"{e.Message.Text}mins");
                                await fileStream.DisposeAsync();
                                Console.WriteLine("Voice messages forbidden");
                            }
                            catch (ApiRequestException audioEx)
                            {
                                if (audioEx.Message.Contains("Bad Request: AUDIO_FILES_FORBIDDEN"))
                                {
                                    // if the user has banned the receipt of voice messages and audio files
                                    await _meditateDailyBot!.SendTextMessageAsync(e.Message.Chat,
                                        "The user blocked the receipt of voice messages and audio files.\nPlease change the privacy settings in Telegram so that the bot can send voice messages and audio files.");
                                    
                                    Console.WriteLine("Voice & Audio messages forbidden");

                                }
                                else
                                {
                                    // if another error is obtained when sending an audio file
                                    await _meditateDailyBot!.SendTextMessageAsync(e.Message.Chat,
                                        "Unexpected Error 1. Please contact with developer via @Naghual.");

                                    Console.WriteLine("Unexpected Error 1 in sending audio message");
                                }
                            }
                        }
                        else
                        {
                            // if another error is received when sending a voice message
                            await _meditateDailyBot!.SendTextMessageAsync(e.Message.Chat,
                                "Unexpected Error 2. Please contact with developer via @Naghual.");

                            Console.WriteLine("Unexpected Error 2 in sending audio message");
                        }
                        break;
                    }

                case "/start":
                    await _meditateDailyBot!.SendTextMessageAsync(e.Message.Chat, "Please, enter the following count of minutes:\n1, 5, 10, 15, 20, 30, 45, 60",default, null, true, false, 0, false, rkm);
                    break;
                
                case "/help":
                    await _meditateDailyBot!.SendTextMessageAsync(e.Message.Chat, 
                        "Hello, my Friend. This Bot may help You in Your meditation." +
                        "\n\nHow it works: choose count of minutes, which You would like to spend on Your meditation and enter this number in chat or click on the keyboard." +
                        "\n\nThen, You will receive an audio file, which contains absolutely silence, but have the start and end notifications." +
                        "\n\nThe point, that You may start Your meditation by play this audio file, blocking Your device and be quiet, because when timer is over, You will hear an end notification." +
                        "\n\nTelegram allows play audio in background, so it looks very comfortable, I hope." +
                        "\n\nFor any questions and proposals contact with me: @Naghual. Thank You and have a great relax time.");
                    break;
                
                default:
                    // the answer to any other meaning is asked to introduce the correct value
                    await _meditateDailyBot!.SendTextMessageAsync(e.Message.Chat, $"{e.Message.Text} is not support.\nPlease, select the following count of minutes:\n1, 5, 10, 15, 20, 30, 45, 60");
                    break;
            }
            // end of the options for choosing
        }
    }
    }