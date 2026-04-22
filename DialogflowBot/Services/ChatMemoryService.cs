using DialogflowBot.Models;

namespace DialogflowBot.Services
{
    public class ChatMemoryService
    {

        private static Dictionary<string, List<ChatMessage>> chat = new();

        public void AddMessage(string sessionId, ChatMessage message)
        {
            if (!chat.ContainsKey(sessionId))
            {
                chat[sessionId] = new List<ChatMessage>();
            }
            chat[sessionId].Add(message);
        }

        public List<ChatMessage> GetMessages(string sessionId)
        {
            return chat.ContainsKey(sessionId) ? chat[sessionId] : new List<ChatMessage>();
        }
    }
}
