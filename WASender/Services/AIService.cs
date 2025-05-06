using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using WASender.Models;
using Google.Generative.AI;

namespace WASender.Services
{
    public class AIService
    {
        private readonly string _apiKey;
        private GenerativeModel _model;
        private Dictionary<string, string> _messageTemplates;
        
        public AIService(string apiKey)
        {
            _apiKey = apiKey;
            InitializeModel();
            InitializeTemplates();
        }
        
        private void InitializeModel()
        {
            try
            {
                var modelConfig = new GenerationConfig
                {
                    Temperature = 0.7f,
                    MaxOutputTokens = 1000,
                    TopP = 0.95f
                };
                
                _model = new GenerativeModel(
                    modelName: "gemini-1.5-flash",
                    apiKey: _apiKey,
                    generationConfig: modelConfig);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to initialize AI model: " + ex.Message);
            }
        }
        
        private void InitializeTemplates()
        {
            _messageTemplates = new Dictionary<string, string>
            {
                { "sales", "Create a persuasive sales message for WhatsApp that highlights benefits and includes a clear call to action." },
                { "support", "Create a helpful customer support message for WhatsApp that addresses common issues and provides solutions." },
                { "followup", "Create a friendly follow-up message for WhatsApp that reminds the recipient about previous interaction without being pushy." },
                { "promotion", "Create an exciting promotional message for WhatsApp that creates urgency and highlights special offers." },
                { "welcome", "Create a warm welcome message for WhatsApp that introduces your business and what you offer." }
            };
        }
        
        public async Task<string> GenerateMessage(string prompt, string context = "", string tone = "friendly")
        {
            string fullPrompt = $"Create a WhatsApp message with a {tone} tone based on this prompt: {prompt}";
            
            if (!string.IsNullOrEmpty(context))
                fullPrompt += $"\nAdditional context: {context}";
                
            return await GenerateContent(fullPrompt);
        }
        
        public async Task<string> GenerateTemplatedMessage(string templateType, Dictionary<string, string> variables)
        {
            if (!_messageTemplates.ContainsKey(templateType))
                throw new Exception($"Template type '{templateType}' not found");
                
            string basePrompt = _messageTemplates[templateType];
            string variablesText = "";
            
            foreach (var variable in variables)
            {
                variablesText += $"\n- {variable.Key}: {variable.Value}";
            }
            
            string fullPrompt = basePrompt + "\nUse these details:" + variablesText;
            return await GenerateContent(fullPrompt);
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
                throw new Exception("AI generation error: " + ex.Message);
            }
        }
        
        public async Task<bool> ValidateApiKey()
        {
            try
            {
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