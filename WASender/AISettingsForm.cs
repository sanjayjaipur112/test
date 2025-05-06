using MaterialSkin.Controls;
using System;
using System.Windows.Forms;
using WASender.Models;
using WASender.Services;

namespace WASender
{
    public partial class AISettingsForm : MyMaterialPopOp
    {
        private AISettingsModel _settings;
        
        public AISettingsForm()
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            LoadSettings();
            InitializeLanguage();
        }
        
        private void InitializeLanguage()
        {
            this.Text = "AI Settings";
            lblApiKey.Text = "Gemini API Key:";
            chkEnableAI.Text = "Enable AI Features";
            lblDefaultPrompt.Text = "Default Prompt:";
            btnSave.Text = Strings.Save;
            btnCancel.Text = Strings.Cancel;
            lnkGetApiKey.Text = "Get Free API Key";
            btnTestKey.Text = "Test Key";
        }
        
        private void LoadSettings()
        {
            _settings = Config.GetAISettings();
            txtApiKey.Text = _settings.GeminiApiKey ?? "";
            chkEnableAI.Checked = _settings.EnableAIFeatures;
            txtDefaultPrompt.Text = _settings.DefaultPrompt ?? "Write a friendly WhatsApp message";
        }
        
        private async void btnSave_Click(object sender, EventArgs e)
        {
            string apiKey = txtApiKey.Text.Trim();
            
            if (chkEnableAI.Checked && string.IsNullOrEmpty(apiKey))
            {
                MaterialSnackBar SnackBarMessage = new MaterialSnackBar("API Key is required to enable AI features", Strings.OK, true);
                SnackBarMessage.Show(this);
                return;
            }
            
            _settings.GeminiApiKey = apiKey;
            _settings.EnableAIFeatures = chkEnableAI.Checked;
            _settings.DefaultPrompt = txtDefaultPrompt.Text.Trim();
            
            Config.SaveAISettings(_settings);
            
            MaterialSnackBar successMessage = new MaterialSnackBar("AI Settings saved successfully", Strings.OK, true);
            successMessage.Show(this);
            
            this.DialogResult = DialogResult.OK;
        }
        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        
        private void lnkGetApiKey_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://makersuite.google.com/app/apikey");
        }
        
        private async void btnTestKey_Click(object sender, EventArgs e)
        {
            string apiKey = txtApiKey.Text.Trim();
            
            if (string.IsNullOrEmpty(apiKey))
            {
                MaterialSnackBar SnackBarMessage = new MaterialSnackBar("Please enter an API Key to test", Strings.OK, true);
                SnackBarMessage.Show(this);
                return;
            }
            
            btnTestKey.Enabled = false;
            btnTestKey.Text = "Testing...";
            
            try
            {
                var geminiService = new GeminiService(apiKey);
                bool isValid = await geminiService.ValidateApiKey();
                
                if (isValid)
                {
                    MaterialSnackBar SnackBarMessage = new MaterialSnackBar("API Key is valid!", Strings.OK, true);
                    SnackBarMessage.Show(this);
                }
                else
                {
                    MaterialSnackBar SnackBarMessage = new MaterialSnackBar("API Key validation failed", Strings.OK, true);
                    SnackBarMessage.Show(this);
                }
            }
            catch (Exception ex)
            {
                MaterialSnackBar SnackBarMessage = new MaterialSnackBar("Error: " + ex.Message, Strings.OK, true);
                SnackBarMessage.Show(this);
            }
            finally
            {
                btnTestKey.Enabled = true;
                btnTestKey.Text = "Test Key";
            }
        }
    }
}