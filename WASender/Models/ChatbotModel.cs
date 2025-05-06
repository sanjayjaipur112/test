using System;
using System.Collections.Generic;

namespace WASender.Models
{
    public class ChatbotModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public string WelcomeMessage { get; set; }
        public string FallbackMessage { get; set; } = "I'm sorry, I couldn't understand that. Could you please rephrase?";
        public List<ChatbotIntent> Intents { get; set; } = new List<ChatbotIntent>();
        public bool UseAI { get; set; } = false;
        public string AIPrompt { get; set; } = "You are a helpful WhatsApp assistant. Keep responses brief and friendly.";
        public int MaxDailyMessages { get; set; } = 100;
        public List<string> BusinessHours { get; set; } = new List<string> { "9:00-17:00" };
        public string OutOfHoursMessage { get; set; } = "Thanks for your message. Our business hours are 9 AM to 5 PM. We'll get back to you during business hours.";
    }
    
    public class ChatbotIntent
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public List<string> Patterns { get; set; } = new List<string>();
        public List<string> Responses { get; set; } = new List<string>();
        public bool UseAI { get; set; } = false;
        public string AIResponsePrompt { get; set; }
    }
    
    public class ChatbotConversation
    {
        public string ContactNumber { get; set; }
        public string ContactName { get; set; }
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime LastMessageTime { get; set; } = DateTime.Now;
        public List<ChatbotMessage> Messages { get; set; } = new List<ChatbotMessage>();
        public Dictionary<string, string> Variables { get; set; } = new Dictionary<string, string>();
        public string CurrentState { get; set; } = "initial";
    }
    
    public class ChatbotMessage
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Content { get; set; }
        public bool IsFromUser { get; set; }
        public string IntentMatched { get; set; }
    }
}