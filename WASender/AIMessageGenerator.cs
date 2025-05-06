using MaterialSkin.Controls;
using System;
using System.Windows.Forms;
using WASender.Models;
using WASender.Services;

namespace WASender
{
    public partial class AIMessageGenerator : MyMaterialPopOp
    {
        private GeminiService _geminiService;
        private AISettingsModel _settings;
        private string _generatedMessage = "";
        
        public string GeneratedMessage => _generatedMessage;
        
        public AIMessageGenerator()
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            LoadSettings();
            InitializeLanguage();
            InitializeService();
        }
        
        private void InitializeLanguage()
        {
            this.Text = "AI Message Generator";
            lblPrompt.Text = "What kind of message do you want to create?";
            lblContext.Text = "Additional context (optional):";
            btnGenerate.Text = "Generate Message";
            btnUse.Text = "Use This Message";
            btnCancel.Text = Strings.Cancel;
            btnSettings.Text = "AI Settings";
        }
        
        private void LoadSettings()
        {
            _settings = Config.GetAISettings();
            txtPrompt.Text = _settings.DefaultPrompt;
        }
        
        private void InitializeService()
        {
            if (_settings.EnableAIFeatures && !string.IsNullOrEmpty(_settings.GeminiApiKey))
            {
                try
                {
                    _geminiService = new GeminiService(_settings.GeminiApiKey);
                    lblStatus.Text = "AI Ready";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                }
                catch (Exception ex)
                {
                    lblStatus.Text = "AI Error: " + ex.Message;
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                    btnGenerate.Enabled = false;
                }
            }
            else
            {
                lblStatus.Text = "AI Not Configured";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                btnGenerate.Enabled = false;
            }
        }
        
        private async void btnGenerate_Click(object sender, EventArgs e)
        {
            if (_geminiService == null)
            {
                MaterialSnackBar SnackBarMessage = new MaterialSnackBar("AI service not configured", Strings.OK, true);
                SnackBarMessage.Show(this);
                return;
            }
            
            string prompt = txtPrompt.Text.Trim();
            if (string.IsNullOrEmpty(prompt))
            {
                MaterialSnackBar SnackBarMessage = new MaterialSnackBar("Please enter a prompt", Strings.OK, true);
                SnackBarMessage.Show(this);
                return;
            }
            
            btnGenerate.Enabled = false;
            progressBar.Visible = true;
            lblStatus.Text = "Generating...";
            
            try
            {
                string context = txtContext.Text.Trim();
                _generatedMessage = await _geminiService.GenerateWhatsAppMessage(prompt, context);
                txtResult.Text = _generatedMessage;
                btnUse.Enabled = true;
                lblStatus.Text = "Message Generated";
            }
            catch (Exception ex)
            {
                MaterialSnackBar SnackBarMessage = new MaterialSnackBar("Error: " + ex.Message, Strings.OK, true);
                SnackBarMessage.Show(this);
                lblStatus.Text = "Generation Failed";
            }
            finally
            {
                btnGenerate.Enabled = true;
                progressBar.Visible = false;
            }
        }
        
        private void btnUse_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        
        private void btnSettings_Click(object sender, EventArgs e)
        {
            AISettingsForm settingsForm = new AISettingsForm();
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                LoadSettings();
                InitializeService();
            }
        }
    }
}

