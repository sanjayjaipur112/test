using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WASender.Models;

namespace WASender.Services
{
    public class ChatbotService
    {
        private List<ChatbotModel> _chatbots = new List<ChatbotModel>();
        private Dictionary<string, ChatbotConversation> _activeConversations = new Dictionary<string, ChatbotConversation>();
        private AIService _aiService;
        private int _conversationTimeoutMinutes = 30;
        
        public ChatbotService(AIService aiService)
        {
            _aiService = aiService;
            LoadChatbots();
        }
        
        private void LoadChatbots()
        {
            // In a real implementation, load from database or file
            // For now, we'll create a sample chatbot
            ChatbotModel sampleBot = new ChatbotModel
            {
                Name = "Customer Support Bot",
                Description = "Handles basic customer inquiries",
                WelcomeMessage = "Hello! I'm your virtual assistant. How can I help you today?",
                Intents = new List<ChatbotIntent>
                {
                    new ChatbotIntent
                    {
                        Name = "greeting",
                        Patterns = new List<string> { "hello", "hi", "hey", "good morning", "good afternoon" },
                        Responses = new List<string> { "Hello! How can I help you today?", "Hi there! What can I assist you with?" }
                    },
                    new ChatbotIntent
                    {
                        Name = "goodbye",
                        Patterns = new List<string> { "bye", "goodbye", "see you", "talk later" },
                        Responses = new List<string> { "Goodbye! Have a great day!", "Thanks for chatting. Feel free to reach out again if you need help!" }
                    },
                    new ChatbotIntent
                    {
                        Name = "thanks",
                        Patterns = new List<string> { "thank you", "thanks", "appreciate it" },
                        Responses = new List<string> { "You're welcome!", "Happy to help!", "Anytime!" }
                    }
                }
            };
            
            _chatbots.Add(sampleBot);
        }
        
        public async Task<string> ProcessMessage(string contactNumber, string contactName, string message)
        {
            // Get or create conversation
            if (!_activeConversations.ContainsKey(contactNumber))
            {
                _activeConversations[contactNumber] = new ChatbotConversation
                {
                    ContactNumber = contactNumber,
                    ContactName = contactName
                };
                
                // Send welcome message
                ChatbotModel bot = _chatbots.FirstOrDefault(b => b.IsActive);
                if (bot != null)
                {
                    return bot.WelcomeMessage;
                }
                return null;
            }
            
            // Update existing conversation
            ChatbotConversation conversation = _activeConversations[contactNumber];
            
            // Check if conversation timed out
            TimeSpan timeSinceLastMessage = DateTime.Now - conversation.LastMessageTime;
            if (timeSinceLastMessage.TotalMinutes > _conversationTimeoutMinutes)
            {
                // Reset conversation
                conversation = new ChatbotConversation
                {
                    ContactNumber = contactNumber,
                    ContactName = contactName
                };
                _activeConversations[contactNumber] = conversation;
                
                // Send welcome message again
                ChatbotModel bot = _chatbots.FirstOrDefault(b => b.IsActive);
                if (bot != null)
                {
                    return bot.WelcomeMessage;
                }
                return null;
            }
            
            // Add user message to conversation
            conversation.Messages.Add(new ChatbotMessage
            {
                Content = message,
                IsFromUser = true
            });
            
            conversation.LastMessageTime = DateTime.Now;
            
            // Process message with active chatbot
            ChatbotModel activeBot = _chatbots.FirstOrDefault(b => b.IsActive);
            if (activeBot == null)
                return null;
                
            // Check business hours
            if (!IsWithinBusinessHours(activeBot.BusinessHours))
            {
                return activeBot.OutOfHoursMessage;
            }
            
            // Match intent
            string response = await MatchIntent(activeBot, message, conversation);
            
            // If no intent matched and AI is enabled, use AI
            if (response == null && activeBot.UseAI)
            {
                string conversationHistory = GetConversationHistory(conversation);
                string aiPrompt = $"{activeBot.AIPrompt}\n\nUser message: \"{message}\"\n\nConversation history: {conversationHistory}";
                
                response = await _aiService.GenerateContent(aiPrompt);
            }
            
            // Use fallback if still no response
            if (response == null)
            {
                response = activeBot.FallbackMessage;
            }
            
            // Add bot response to conversation
            conversation.Messages.Add(new ChatbotMessage
            {
                Content = response,
                IsFromUser = false
            });
            
            return response;
        }
        
        private async Task<string> MatchIntent(ChatbotModel bot, string message, ChatbotConversation conversation)
        {
            string lowercaseMessage = message.ToLower();
            
            foreach (ChatbotIntent intent in bot.Intents)
            {
                foreach (string pattern in intent.Patterns)
                {
                    if (Regex.IsMatch(lowercaseMessage, pattern, RegexOptions.IgnoreCase))
                    {
                        conversation.CurrentState = intent.Name;
                        
                        if (intent.UseAI && !string.IsNullOrEmpty(intent.AIResponsePrompt))
                        {
                            string aiPrompt = intent.AIResponsePrompt.Replace("{message}", message);
                            return await _aiService.GenerateContent(aiPrompt);
                        }
                        else if (intent.Responses.Count > 0)
                        {
                            Random random = new Random();
                            return intent.Responses[random.Next(intent.Responses.Count)];
                        }
                    }
                }
            }
            
            return null;
        }
        
        private string GetConversationHistory(ChatbotConversation conversation)
        {
            // Get last 5 messages for context
            var recentMessages = conversation.Messages.Skip(Math.Max(0, conversation.Messages.Count - 5)).ToList();
            
            string history = "";
            foreach (var msg in recentMessages)
            {
                string prefix = msg.IsFromUser ? "User" : "Bot";
                history += $"{prefix}: {msg.Content}\n";
            }
            
            return history;
        }
        
        private bool IsWithinBusinessHours(List<string> businessHours)
        {
            if (businessHours == null || businessHours.Count == 0)
                return true;
                
            DateTime now = DateTime.Now;
            DayOfWeek today = now.DayOfWeek;
            
            foreach (string hours in businessHours)
            {
                string[] parts = hours.Split('-');
                if (parts.Length == 2)
                {
                    TimeSpan start = TimeSpan.Parse(parts[0]);
                    TimeSpan end = TimeSpan.Parse(parts[1]);
                    TimeSpan current = now.TimeOfDay;
                    
                    if (current >= start && current <= end)
                        return true;
                }
            }
            
            return false;
        }
        
        public List<ChatbotModel> GetAllChatbots()
        {
            return _chatbots;
        }
        
        public void SaveChatbot(ChatbotModel chatbot)
        {
            int index = _chatbots.FindIndex(b => b.Id == chatbot.Id);
            if (index >= 0)
            {
                _chatbots[index] = chatbot;
            }
            else
            {
                _chatbots.Add(chatbot);
            }
            
            // In a real implementation, save to database or file
        }
        
        public void DeleteChatbot(string chatbotId)
        {
            _chatbots.RemoveAll(b => b.Id == chatbotId);
            // In a real implementation, delete from database or file
        }
    }
}