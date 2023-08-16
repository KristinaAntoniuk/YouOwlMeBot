using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace misha_kris_finance_bot
{
    internal class Program
    {
        static ITelegramBotClient bot = new TelegramBotClient("6274015108:AAHun_w1_CYM6SYRxVyxa0TJdfGF6jqcmd8");
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
                            if (Decimal.TryParse(message.Text, out amount))
                            {
                                MySqlDataProvider dataProvider = new MySqlDataProvider();

                                int? userId = dataProvider.GetUserIDByUsername(userName);

                                if (userId == null)
                                {
                                    userId = dataProvider.AddUser(userName);
                                }

                                Models.Transaction transaction = new Models.Transaction()
                                {
                                    UserID = userId,
                                    Store = storeName,
                                    Amount = amount,
                                    Date = DateTime.Now,
                                    Guid = Guid.NewGuid()
                                };

                                int? transactionId = dataProvider.AddTransaction(transaction);

                                if (transactionId != null)
                                {
                                    await botClient.SendTextMessageAsync(message.Chat,
                                                                    String.Format(Messages.RecordHasBeenSaved, storeName, amount.ToString()));
                                }
                                else
                                {
                                    await botClient.SendTextMessageAsync(message.Chat,
                                                                String.Format(Messages.RecordHasNotBennSaved, storeName, amount.ToString()));

                                }

                                userName = null;
                                addingTransaction = false;
                                storeName = null;
                                amount = 0;
                            }
                            else
                            {
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
            Console.WriteLine("Bot " + bot.GetMeAsync().Result.FirstName + " is running.");

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { },
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