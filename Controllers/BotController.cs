﻿using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc;
using misha_kris_finance_lambda_bot.Services;
using Telegram.Bot.Types;

namespace misha_kris_finance_lambda_bot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BotController : ControllerBase
    {
        private readonly IUpdateService _updateService;
        private readonly IDynamoDBContext _context;

        public BotController(IUpdateService updateService, IDynamoDBContext context)
        {
            _updateService = updateService;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> HandleUpdate([FromBody] Update update,
            CancellationToken cancellationToken = default)
        {
            await _updateService.HandleUpdate(update, _context, cancellationToken);
            return Ok();
        }
    }
}
