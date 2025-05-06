private object _aiService;

private void InitializeAIService()
{
    _aiService = AIServiceFactory.CreateAIService();
}

private async void btnGenerateAIContent_Click(object sender, EventArgs e)
{
    if (_aiService == null)
    {
        MaterialSnackBar SnackBarMessage = new MaterialSnackBar(Strings.AINotConfigured, Strings.OK, true);
        SnackBarMessage.Show(this);
        return;
    }
    
    try
    {
        btnGenerateAIContent.Enabled = false;
        progressBar.Visible = true;
        
        string prompt = txtAIPrompt.Text.Trim();
        if (string.IsNullOrEmpty(prompt))
        {
            MaterialSnackBar SnackBarMessage = new MaterialSnackBar(Strings.PromptCannotBeEmpty, Strings.OK, true);
            SnackBarMessage.Show(this);
            return;
        }
        
        string generatedContent;
        if (_aiService is OpenAIService openAIService)
        {
            generatedContent = await openAIService.GenerateMessageContent(prompt);
        }
        else if (_aiService is GeminiAIService geminiService)
        {
            generatedContent = await geminiService.GenerateMessageContent(prompt);
        }
        else
        {
            throw new Exception("Unknown AI service type");
        }
        
        txtMessageBody.Text = generatedContent;
    }
    catch (Exception ex)
    {
        MaterialSnackBar SnackBarMessage = new MaterialSnackBar(ex.Message, Strings.OK, true);
        SnackBarMessage.Show(this);
    }
    finally
    {
        btnGenerateAIContent.Enabled = true;
        progressBar.Visible = false;
    }
}