using System;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Chat;

namespace WASender.Services
{
    public class OpenAIService
    {
        private readonly string _apiKey;
        
        public OpenAIService(string apiKey)
        {
            _apiKey = apiKey;
            OpenAIConfiguration.ApiKey = apiKey;
        }

        public async Task<string> GenerateMessageContent(string prompt)
        {
            try
            {
                var chatService = new ChatService();
                var options = new ChatCreateOptions
                {
                    Model = "gpt-4o-mini",
                    Messages = new[]
                    {
                        new ChatMessage
                        {
                            Role = "user",
                            Content = prompt
                        }
                    },
                    MaxTokens = 500,
                    Temperature = 0.7
                };
                
                var response = chatService.Create(options);
                return response.Choices[0].Message.Content;
            }
            catch (Exception ex)
            {
                Logger.WriteLog("OpenAI API Error: " + ex.Message);
                return "Error generating content: " + ex.Message;
            }
        }
    }
}