private void InitializeAISettings()
{
    var settings = Config.GetAppSettings();
    txtGeminiApiKey.Text = settings.GeminiApiKey ?? "";
    chkEnableAI.Checked = settings.EnableAIFeatures;
    
    // Set radio button based on provider
    if (settings.AIProvider == "OpenAI")
        rbOpenAI.Checked = true;
    else
        rbGemini.Checked = true;
}

private void btnSaveAISettings_Click(object sender, EventArgs e)
{
    var settings = Config.GetAppSettings();
    settings.GeminiApiKey = txtGeminiApiKey.Text.Trim();
    settings.EnableAIFeatures = chkEnableAI.Checked;
    settings.AIProvider = rbOpenAI.Checked ? "OpenAI" : "Gemini";
    
    Config.SaveAppSettings(settings);
    
    MaterialSnackBar SnackBarMessage = new MaterialSnackBar(Strings.SettingsSaved, Strings.OK, true);
    SnackBarMessage.Show(this);
}