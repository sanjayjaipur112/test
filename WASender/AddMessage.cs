using FluentValidation.Results;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaAutoReplyBot.Models;
using WaAutoReplyBot.Validators;
using WASender;
using WASender.Models;

namespace WaAutoReplyBot
{
    public partial class AddMessage : MyMaterialPopOp
    {
        MessageModel messageModel;
        AddRule addRule;
        List<ButtonsModel> buttonsModelList1;
        List<PollModel> pollsModelList1;
        GeneralSettingsModel generalSettingsModel;
        public AddMessage(MessageModel _messageModel, AddRule _addRule)
        {
            messageModel = _messageModel;
            addRule = _addRule;
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            generalSettingsModel = Config.GetSettings();
            if (_messageModel.buttons == null)
            {
                buttonsModelList1 = new List<ButtonsModel>();
            }
            else
            {
                buttonsModelList1 = _messageModel.buttons;
            }
            if (_messageModel.polls == null)
            {
                pollsModelList1 = new List<PollModel>();
            }
            else
            {
                pollsModelList1 = _messageModel.polls;
            }

            webBrowser1.DocumentText = Storage.DocumentHtmlString;
            this.webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser1_DocumentCompleted);
            if (_messageModel.polls != null && _messageModel.polls.Count() > 0)
            {
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(500);
                    this.Invoke(new Action(() =>
                        geteratePolls()));
                });
            }
            if (generalSettingsModel.browserType == 1)
            {
                groupBox11.Visible = false;
            }
            else
            {
                groupBox11.Visible = true;
            }
        }

        private void browser1_DocumentCompleted(Object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this.webBrowser1.Document.Body.MouseDown += new HtmlElementEventHandler(Body_MouseDown1);
        }

        private void Body_MouseDown1(Object sender, HtmlElementEventArgs e)
        {
            switch (e.MouseButtonsPressed)
            {
                case MouseButtons.Left:
                    HtmlElement element = this.webBrowser1.Document.GetElementFromPoint(e.ClientMousePosition);
                    var btnId = element.GetAttribute("id");
                    if (btnId != "")
                    {
                        try
                        {
                            PollModel b = pollsModelList1.Where(x => x.id == btnId).FirstOrDefault();
                            b.editMode = true;
                            AddPoll addButton = new AddPoll(b, this);
                            addButton.ShowDialog();
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    break;
            }
        }

        private void materialButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            Utils.selectFileForMessage(lstViewFiles);
        }

        private void lstViewFiles_KeyDown(object sender, KeyEventArgs e)
        {
            Utils.removeListViewItem(e, lstViewFiles);
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            this.messageModel.LongMessage = txtLongMessage.Text;
            this.messageModel.Files = new List<string>();


            foreach (ListViewItem item in lstViewFiles.Items)
            {
                this.messageModel.Files.Add(item.Text);
            }
            this.messageModel.buttons = buttonsModelList1;
            this.messageModel.polls = pollsModelList1;

            List<ValidationFailure> errors = new MessageModelValidator().Validate(this.messageModel).Errors.ToList();
            if (errors.Count() > 0)
            {
                MaterialSnackBar SnackBarMessage = new MaterialSnackBar(errors[0].ErrorMessage, Strings.OK, true);
                SnackBarMessage.Show(this);
            }
            else
            {
                this.addRule.AddNewMesage(this.messageModel);
                this.Close();
            }

        }

        private void AddMessage_Load(object sender, EventArgs e)
        {
            init();
            initLanguage();
        }

        private void initLanguage()
        {
            this.Text = Strings.ReplyMessage;
            txtLongMessage.Hint = Strings.TypeYourMessagehere;
            materialButton1.Text = Strings.Addfile;
            materialButton3.Text = Strings.Cancel;
            materialButton2.Text = Strings.Add;
            lstViewFiles.Columns[0].Text = Strings.Files;
            groupBox11.Text = Strings.Polls;
            materialButton19.Text = Strings.AddPoll;
        }

        private void init()
        {
            this.txtLongMessage.Text = this.messageModel.LongMessage;
            if (this.messageModel.Files == null)
                this.messageModel.Files = new List<string>();

            foreach (var item in this.messageModel.Files)
            {
                lstViewFiles.Items.Add(item);
            }
        }

        private void AddMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void materialButton19_Click(object sender, EventArgs e)
        {
            ShowAddButtonDialog();
        }

        private void ShowAddButtonDialog()
        {
            PollModel pollmodel = new PollModel();
            AddPoll addPoll = new AddPoll(pollmodel,this);
            addPoll.ShowDialog();
        }


        public void RecievPolls(PollModel _buttonsModel)
        {
            if (_buttonsModel.editMode == true)
            {
                int index = pollsModelList1.FindIndex(x => x.id == _buttonsModel.id);
                _buttonsModel.editMode = false;
                pollsModelList1[index] = _buttonsModel;
            }
            else
            {
                pollsModelList1.Add(_buttonsModel);
            }

            geteratePolls();
        }

        private void geteratePolls()
        {
            string buttontext = Storage.DocumentHtmlString;
            string cssStyle = Storage.DocumentButtonStypeStrig;

            foreach (var item in pollsModelList1)
            {
                string txt = "📊 " + item.PollName;

                buttontext += "<button style='margin:5px;" + cssStyle + "' type='button' id='" + item.id + "' >" + txt + "</button>";
            }
            webBrowser1.DocumentText = buttontext + "</body></html>";
        }

      

        public void RemovePoll(PollModel _pollModel)
        {
            int index = pollsModelList1.FindIndex(x => x.id == _pollModel.id);
            pollsModelList1.Remove(pollsModelList1[index]);
            geteratePolls();
        }

        private void btnAIGenerate_Click(object sender, EventArgs e)
        {
            AIMessageGenerator aiGenerator = new AIMessageGenerator();
            if (aiGenerator.ShowDialog() == DialogResult.OK)
            {
                string generatedMessage = aiGenerator.GeneratedMessage;
                if (!string.IsNullOrEmpty(generatedMessage))
                {
                    txtLongMessage.Text = generatedMessage;
                }
            }
        }

        private void AddAIButton()
        {
            btnAIGenerate = new MaterialSkin.Controls.MaterialButton();
            btnAIGenerate.AutoSize = false;
            btnAIGenerate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            btnAIGenerate.Depth = 0;
            btnAIGenerate.DrawShadows = true;
            btnAIGenerate.HighEmphasis = true;
            btnAIGenerate.Icon = global::WASender.Properties.Resources.robot;
            btnAIGenerate.Location = new System.Drawing.Point(520, 10);
            btnAIGenerate.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            btnAIGenerate.MouseState = MaterialSkin.MouseState.HOVER;
            btnAIGenerate.Name = "btnAIGenerate";
            btnAIGenerate.Size = new System.Drawing.Size(120, 36);
            btnAIGenerate.TabIndex = 15;
            btnAIGenerate.Text = "AI";
            btnAIGenerate.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnAIGenerate.UseAccentColor = true;
            btnAIGenerate.UseVisualStyleBackColor = true;
            btnAIGenerate.Click += new System.EventHandler(this.btnAIGenerate_Click);
            
            // Add to the form
            this.Controls.Add(btnAIGenerate);
        }
    }
}
