using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using YouOwlMeBot.Services;

namespace YouOwlMeBot
{
    public class HandleUpdate
    {
        private readonly IUpdateService _updateService;

        public HandleUpdate(IUpdateService updateService)
        {
            _updateService = updateService;
        }

        [FunctionName("HandleUpdate")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Update update = JsonConvert.DeserializeObject<Update>(requestBody);

            await _updateService.HandleUpdate(update);

            return new OkObjectResult("");
        }
    }
}
