using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WASender.Models;
using WASender.Services;

namespace WASender
{
    public partial class ChatbotManagerForm : MyMaterialPopOp
    {
        private ChatbotService _chatbotService;
        private AIService _aiService;
        private List<ChatbotModel> _chatbots;
        
        public ChatbotManagerForm(ChatbotService chatbotService, AIService aiService)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            _chatbotService = chatbotService;
            _aiService = aiService;
            InitializeLanguage();
            LoadChatbots();
        }
        
        private void InitializeLanguage()
        {
            this.Text = "Chatbot Manager";
            btnNewChatbot.Text = "Create New Chatbot";
            btnEdit.Text = "Edit";
            btnDelete.Text = "Delete";
            btnActivate.Text = "Activate";
            btnDeactivate.Text = "Deactivate";
            btnClose.Text = Strings.Close;
        }
        
        private void LoadChatbots()
        {
            _chatbots = _chatbotService.GetAllChatbots();
            listViewChatbots.Items.Clear();
            
            foreach (var chatbot in _chatbots)
            {
                ListViewItem item = new ListViewItem(chatbot.Name);
                item.SubItems.Add(chatbot.Description);
                item.SubItems.Add(chatbot.IsActive ? "Active" : "Inactive");
                item.SubItems.Add(chatbot.Intents.Count.ToString());
                item.Tag = chatbot.Id;
                
                listViewChatbots.Items.Add(item);
            }
        }
        
        private void btnNewChatbot_Click(object sender, EventArgs e)
        {
            ChatbotEditorForm editorForm = new ChatbotEditorForm(_aiService);
            if (editorForm.ShowDialog() == DialogResult.OK)
            {
                _chatbotService.SaveChatbot(editorForm.Chatbot);
                LoadChatbots();
            }
        }
        
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (listViewChatbots.SelectedItems.Count == 0)
            {
                MaterialSnackBar SnackBarMessage = new MaterialSnackBar("Please select a chatbot to edit", Strings.OK, true);
                SnackBarMessage.Show(this);
                return;
            }
            
            string chatbotId = listViewChatbots.SelectedItems[0].Tag.ToString();
            ChatbotModel chatbot = _chatbots.Find(b => b.Id == chatbotId);
            
            if (chatbot != null)
            {
                ChatbotEditorForm editorForm = new ChatbotEditorForm(_aiService, chatbot);
                if (editorForm.ShowDialog() == DialogResult.OK)
                {
                    _chatbotService.SaveChatbot(editorForm.Chatbot);
                    LoadChatbots();
                }
            }
        }
        
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listViewChatbots.SelectedItems.Count == 0)
            {
                MaterialSnackBar SnackBarMessage = new MaterialSnackBar("Please select a chatbot to delete", Strings.OK, true);
                SnackBarMessage.Show(this);
                return;
            }
            
            string chatbotId = listViewChatbots.SelectedItems[0].Tag.ToString();
            
            DialogResult result = MessageBox.Show("Are you sure you want to delete this chatbot?", "Confirm Delete", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
            if (result == DialogResult.Yes)
            {
                _chatbotService.DeleteChatbot(chatbotId);
                LoadChatbots();
            }
        }
        
        private void btnActivate_Click(object sender, EventArgs e)
        {
            if (listViewChatbots.SelectedItems.Count == 0)
            {
                MaterialSnackBar SnackBarMessage = new MaterialSnackBar("Please select a chatbot to activate", Strings.OK, true);
                SnackBarMessage.Show(this);
                return;
            }
            
            string chatbotId = listViewChatbots.SelectedItems[0].Tag.ToString();
            ChatbotModel chatbot = _chatbots.Find(b => b.Id == chatbotId);
            
            if (chatbot != null)
            {
                // Deactivate all other chatbots
                foreach (var bot in _chatbots)
                {
                    bot.IsActive = false;
                }
                
                // Activate selected chatbot
                chatbot.IsActive = true;
                
                // Save all chatbots
                foreach (var bot in _chatbots)
                {
                    _chatbotService.SaveChatbot(bot);
                }
                
                LoadChatbots();
            }
        }
        
        private void btnDeactivate_Click(object sender, EventArgs e)
        {
            if (listViewChatbots.SelectedItems.Count == 0)
            {
                MaterialSnackBar SnackBarMessage = new MaterialSnackBar("Please select a chatbot to deactivate", Strings.OK, true);
                SnackBarMessage.Show(this);
                return;
            }
            
            string chatbotId = listViewChatbots.SelectedItems[0].Tag.ToString();
            ChatbotModel chatbot = _chatbots.Find(b => b.Id == chatbotId);
            
            if (chatbot != null)
            {
                chatbot.IsActive = false;
                _chatbotService.SaveChatbot(chatbot);
                LoadChatbots();
            }
        }
        
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}