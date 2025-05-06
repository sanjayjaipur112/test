using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WASender.Models;
using WASender.Services;

namespace WASender
{
    public class SmartReplyService
    {
        private AIService _aiService;
        private Dictionary<string, DateTime> _lastReplies = new Dictionary<string, DateTime>();
        private int _cooldownMinutes = 30; // Prevent too frequent replies
        
        public SmartReplyService(AIService aiService)
        {
            _aiService = aiService;
        }
        
        public async Task<string> GenerateReply(string incomingMessage, string contactName, string conversationHistory = "")
        {
            // Check cooldown to prevent spam
            if (_lastReplies.ContainsKey(contactName))
            {
                TimeSpan timeSinceLastReply = DateTime.Now - _lastReplies[contactName];
                if (timeSinceLastReply.TotalMinutes < _cooldownMinutes)
                {
                    return null; // Still in cooldown period
                }
            }
            
            string prompt = $"Generate a helpful and natural WhatsApp reply to this message: \"{incomingMessage}\"";
            
            if (!string.IsNullOrEmpty(conversationHistory))
            {
                prompt += $"\nHere's some context from the conversation history: {conversationHistory}";
            }
            
            string reply = await _aiService.GenerateContent(prompt);
            
            // Update cooldown timer
            _lastReplies[contactName] = DateTime.Now;
            
            return reply;
        }
        
        public async Task<List<string>> GenerateQuickReplySuggestions(string incomingMessage, int count = 3)
        {
            string prompt = $"Generate {count} short, helpful reply options (each under 100 characters) to this WhatsApp message: \"{incomingMessage}\". Format as a numbered list.";
            
            string result = await _aiService.GenerateContent(prompt);
            List<string> suggestions = new List<string>();
            
            // Simple parsing of numbered list
            string[] lines = result.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (trimmed.Length > 2 && char.IsDigit(trimmed[0]) && (trimmed[1] == '.' || trimmed[1] == ')'))
                {
                    suggestions.Add(trimmed.Substring(2).Trim());
                }
            }
            
            return suggestions;
        }
        
        public void SetCooldownMinutes(int minutes)
        {
            _cooldownMinutes = minutes;
        }
    }
}