using System;

namespace WASender.Services
{
    public static class AIServiceFactory
    {
        public static object CreateAIService()
        {
            var settings = Config.GetAppSettings();
            
            if (!settings.EnableAIFeatures)
                return null;
                
            try
            {
                if (settings.AIProvider == "OpenAI" && !string.IsNullOrEmpty(settings.OpenAIApiKey))
                {
                    return new OpenAIService(settings.OpenAIApiKey);
                }
                else if (settings.AIProvider == "Gemini" && !string.IsNullOrEmpty(settings.GeminiApiKey))
                {
                    return new GeminiAIService(settings.GeminiApiKey);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog("AI Service Creation Error: " + ex.Message);
            }
            
            return null;
        }
    }
}