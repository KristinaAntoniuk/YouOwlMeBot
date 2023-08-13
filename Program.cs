using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace misha_kris_finance_bot
{
    internal class Program
    {
        static ITelegramBotClient bot = new TelegramBotClient("");
        static bool addingTransaction = false;
        static string? storeName = null;
        static decimal amount = 0;
        static string? userName;

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if(update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                Message? message = update.Message;
                
                if (message != null){
                    switch (message.Text){
                        case "/add":
                            await botClient.SendTextMessageAsync(message.Chat, "Store name:");
                            userName = message.Chat.Username;
                            addingTransaction = true;
                            storeName = null;
                            amount = 0;
                            break;
                        default:
                        if (addingTransaction)
                        {
                            decimal amount;
                            if(Decimal.TryParse(message.Text, out amount))
                            {
                                MySqlDataProvider dataProvider = new MySqlDataProvider();
                                
                                int? userID = dataProvider.GetUserIDByUsername(userName);
                                if (userID == null)
                                {
                                    bool success = dataProvider.AddUser(userName);
                                    if (success)
                                    {
                                        userID = dataProvider.GetUserIDByUsername(userName);
                                    }
                                }
                                        
                                bool successTran = dataProvider.AddTransaction(userID, storeName, amount);
                                if (successTran)
                                {
                                    await botClient.SendTextMessageAsync(message.Chat,
                                    String.Format("Record {0} - {1} has been saved.", storeName, amount.ToString()));
                                    userName = null;
                                    addingTransaction = false;
                                    storeName = null;
                                    amount = 0;
                                }
                            }
                            else{
                                storeName = message.Text;
                                await botClient.SendTextMessageAsync(message.Chat, "Amount:");
                            }
                        }
                        break;
                    }
                }
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
    }
}