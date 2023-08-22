using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using YouOwlMeBot.Custom;
using YouOwlMeBot.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllers()
    .AddNewtonsoftJson();

// Add AWS Lambda support. When application is run in Lambda Kestrel is swapped out as the web server with Amazon.Lambda.AspNetCoreServer. This
// package will act as the webserver translating request and responses between the Lambda event source and ASP.NET Core.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

builder.Services.Configure<BotConfiguration>(builder.Configuration);
builder.Services.AddTransient<IUpdateService, UpdateService>();
builder.Services.AddTransient<ITgUserService, TgUserService>();
builder.Services.AddTransient<IProfileService, ProfileService>();
builder.Services.AddTransient<ITransactionService, TransactionService>();

builder.Services
    .AddHttpClient("tgwebhook")
    .AddTypedClient<ITelegramBotClient>((client, sp) =>
    {
        var configuration = sp.GetRequiredService<IOptionsMonitor<BotConfiguration>>();
        return new TelegramBotClient(configuration.CurrentValue.BotToken, client);
    });

AWSOptions awsOptions = new AWSOptions
{
    Credentials = new BasicAWSCredentials(Environment.GetEnvironmentVariable("AccessKey"),
                                          Environment.GetEnvironmentVariable("SecretKey")),
    Region = RegionEndpoint.EUNorth1
};

builder.Services.AddDefaultAWSOptions(awsOptions);
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddScoped<IDynamoDBContext, DynamoDBContext>();

var app = builder.Build();


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "Welcome to running ASP.NET Core Minimal API on AWS Lambda");

app.Run();
