using Google.Apis.Auth.OAuth2;
using Google.Cloud.Dialogflow.V2Beta1;
using System.Text.Json;

namespace DialogflowBot.Services
{
    public class BotService
    {

        private readonly string projectId;
        // TODO: put your knowledge id here 
        private readonly string KNOWLEDGE_ID = "";

        public BotService(IConfiguration config)
        {
            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "credentials.json");

                var json = File.ReadAllText(path);

                var doc = JsonDocument.Parse(json);

                projectId = doc.RootElement.GetProperty("project_id").GetString();
            }
            catch (Exception ex)
            {
                throw new Exception("Error reading credentials.json", ex);
            }
            
        }

        public async Task<string> DetectIntent(string sessionId, string message)
        {
            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "credentials.json");

                var credential = GoogleCredential.FromJson(
                    await File.ReadAllTextAsync(path)
                );

                var builder = new SessionsClientBuilder
                {
                    Credential = credential
                };

                var sessionClient = await builder.BuildAsync();
                var sessionName = SessionName.FromProjectSession(projectId, sessionId);

                var queryInput = new QueryInput
                {
                    Text = new TextInput
                    {
                        Text = message,
                        LanguageCode = "en"
                    }
                };

                var queryParams = new QueryParameters
                {
                    KnowledgeBaseNames =
                    {
                    KnowledgeBaseName.FromProjectKnowledgeBase(projectId, KNOWLEDGE_ID).ToString()
                    }
                };

                var response = await sessionClient.DetectIntentAsync(sessionName, queryInput);
                return response.QueryResult?.FulfillmentText ?? "Sorry, I didn’t understand."; ;
            }
            catch 
            {
                return "Something went wrong please try again...";
            }
        }

    }
}
