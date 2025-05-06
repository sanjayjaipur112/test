using System;
using System.Threading.Tasks;
using Google.Generative.AI;
using System.Collections.Generic;

namespace WASender.Services
{
    public class GeminiService
    {
        private readonly string _apiKey;
        private GenerativeModel _model;
        
        public GeminiService(string apiKey)
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
                    MaxOutputTokens = 800,
                    TopP = 0.95f
                };
                
                _model = new GenerativeModel(
                    modelName: "gemini-1.5-flash",
                    apiKey: _apiKey,
                    generationConfig: modelConfig);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to initialize Gemini: " + ex.Message);
            }
        }
        
        public async Task<string> GenerateContent(string prompt)
        {
            try
            {
                var response = await _model.GenerateContentAsync(prompt);
                return response.Text;
            }
            catch (Exception ex)
            {
                throw new Exception("Gemini API error: " + ex.Message);
            }
        }
        
        public async Task<string> GenerateWhatsAppMessage(string prompt, string context = "")
        {
            string fullPrompt = string.IsNullOrEmpty(context) 
                ? $"Create a WhatsApp message based on this prompt: {prompt}"
                : $"Create a WhatsApp message based on this prompt: {prompt}\nAdditional context: {context}";
                
            return await GenerateContent(fullPrompt);
        }
        
        public async Task<bool> ValidateApiKey()
        {
            try
            {
                // Simple test prompt to validate the API key
                var response = await _model.GenerateContentAsync("Hello");
                return !string.IsNullOrEmpty(response.Text);
            }
            catch
            {
                return false;
            }
        }
    }
}
