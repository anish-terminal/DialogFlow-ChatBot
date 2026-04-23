using DialogflowBot.Models;
using DialogflowBot.Services;
using Microsoft.AspNetCore.SignalR;

namespace DialogflowBot.SignalR
{
    public class ChatHub : Hub
    {
        private readonly BotService _botservice;
        private readonly ChatMemoryService _chatMemoryService;
        public ChatHub(BotService botService, ChatMemoryService chatMemoryService) 
        {
            _botservice = botService;
            _chatMemoryService = chatMemoryService;
        }

        // Sending and Receiving messages through SignalR
        public async Task SendMessage(string sessionId, string message)
        {
            try
            {
                ChatMessage userMessage = new ChatMessage()
                {
                    Sender = "user",
                    Text = message,
                    Time = DateTime.Now
                };
                
                await Clients.Caller.SendAsync("ReceiveMessage", userMessage);

                // Adding user message to chat memory
                _chatMemoryService.AddMessage(sessionId, userMessage);

                var botReply = await _botservice.DetectIntent(sessionId, message);

                ChatMessage botMessage = new ChatMessage()
                {
                    Sender = "bot",
                    Text = botReply,
                    Time = DateTime.Now
                };

                await Clients.Caller.SendAsync("ReceiveMessage", botMessage);

                // Adding bot message to chat memory
                _chatMemoryService.AddMessage(sessionId, botMessage);
            }
            catch (Exception ex)
            {
                {
                    await Clients.Caller.SendAsync("Error", ex.Message);

                    Console.WriteLine($"SignalR Error: {ex.Message}");
                }
            }
        }

        //  Loading all the previous messages 
        public override async Task OnConnectedAsync()
        {
            try
            {
                var httpContext = Context.GetHttpContext();

                if (httpContext == null)
                    throw new Exception("HttpContext is null");

                var sessionId = httpContext.Request.Query["sessionId"].ToString();

                if (string.IsNullOrEmpty(sessionId))
                    throw new Exception("SessionId is missing");

                var history = _chatMemoryService.GetMessages(sessionId);

                await Clients.Caller.SendAsync("LoadMessages", history);

                await base.OnConnectedAsync(); 
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);

                Console.WriteLine($"SignalR Error: {ex.Message}");
            }
        }
    }
}
