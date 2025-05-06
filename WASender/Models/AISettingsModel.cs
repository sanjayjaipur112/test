using System;

namespace WASender.Models
{
    public class AISettingsModel
    {
        public string GeminiApiKey { get; set; }
        public bool EnableAIFeatures { get; set; } = false;
        public string DefaultPrompt { get; set; } = "Write a friendly WhatsApp message";
    }
}
