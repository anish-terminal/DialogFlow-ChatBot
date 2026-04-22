namespace DialogflowBot.Models
{
    public class ChatMessage
    {
        public string Sender { get; set; } 
        public string Text { get; set; }
        public DateTime Time { get; set; }
    }
}
