using System;
using System.Threading.Tasks;
using Google.Generative.AI;
using System.Collections.Generic;

namespace WASender.Services
{
    public class GeminiAIService
    {
        private readonly string _apiKey;
        private GenerativeModel _model;
        
        public GeminiAIService(string apiKey)
        {
            _apiKey = apiKey;
            InitializeModel();
        }
        
        private void InitializeModel()
        {
            try
            {
                var modelConfig = new GenerationConfig
                {
                    Temperature = 0.7f,
                    MaxOutputTokens = 500
                };
                
                _model = new GenerativeModel(
                    modelName: "gemini-1.5-pro",
                    apiKey: _apiKey,
                    generationConfig: modelConfig);
            }
            catch (Exception ex)
            {
                Logger.WriteLog("Gemini AI Initialization Error: " + ex.Message);
                throw;
            }
        }
        
        public async Task<string> GenerateMessageContent(string prompt)
        {
            try
            {
                var response = await _model.GenerateContentAsync(prompt);
                return response.Text;
            }
            catch (Exception ex)
            {
                Logger.WriteLog("Gemini AI Error: " + ex.Message);
                return "Error generating content: " + ex.Message;
            }
        }
        
        public async Task<string> PersonalizeMessage(string messageTemplate, string contactName, string additionalContext = "")
        {
            try
            {
                var prompt = $"Personalize the following message for {contactName}. {additionalContext}\n\nMessage template: {messageTemplate}";
                
                var chatSession = _model.StartChat(new List<Content>
                {
                    Content.FromText("You are an assistant that personalizes message templates for individual recipients. Make the message sound natural and personal.")
                });
                
                var response = await chatSession.SendMessageAsync(prompt);
                return response.Text;
            }
            catch (Exception ex)
            {
                Logger.WriteLog("Gemini AI Error: " + ex.Message);
                return messageTemplate; // Return original template on error
            }
        }
        
        public bool ValidateApiKey()
        {
            try
            {
                // Simple validation by initializing the model
                InitializeModel();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}