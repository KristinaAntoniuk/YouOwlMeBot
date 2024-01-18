using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using YouOwlMeBot.Services;

[assembly: FunctionsStartup(typeof(YouOwlMeBot.Startup))]
namespace YouOwlMeBot
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddTransient<IUpdateService, UpdateService>();
        }
    }
}
