using DialogflowBot.Models;
using DialogflowBot.Services;
using DialogflowBot.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DialogflowBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly BotService _botService;
        private readonly ChatMemoryService _chatService;
        private readonly ChatHub _chathub;

        public BotController(BotService botService, ChatMemoryService chatService, ChatHub chathub)
        {
            _botService = botService;
            _chatService = chatService;
            _chathub = chathub;
        }

        [HttpPost("send")]
        public async Task<IActionResult> sendMessage([FromBody] ChatRequest chatReqest)
        {
            var sessionId = chatReqest.SessionId ?? "default-user";

           await _chathub.SendMessage(sessionId, chatReqest.Message);

            return Ok();
        }

        [HttpGet("getMessages/{sessionId}")]
        public IActionResult GetChat(string sessionId)
        {
            var messages = _chatService.GetMessages(sessionId);
            return Ok(messages);
        }
    }
}
