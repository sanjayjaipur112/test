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
using WaAutoReplyBot;
using WASender.enums;
using WASender.Models;

namespace WASender
{
    public partial class ButtonHolder : MyMaterialPopOp
    {
        WaSenderForm waSenderForm;
        ButtonHolderModel buttonHolderModel;
        public ButtonHolder(WaSenderForm _waSenderForm, ButtonHolderModel _buttonHolderModel)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            this.buttonHolderModel = _buttonHolderModel;
            waSenderForm = _waSenderForm;
            if (buttonHolderModel.buttons == null)
            {
                buttonHolderModel.buttons = new List<ButtonsModel>();
            }
            fillifrequired();
        }

        private void fillifrequired()
        {
            materialTextBox21.Text = buttonHolderModel.title;
            materialTextBox22.Text = buttonHolderModel.footer;


            
            
        }

        private void ButtonHolder_Load(object sender, EventArgs e)
        {
            initLanguages();
            this.webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser1_DocumentCompleted);

            materialComboBox1.DisplayMember = "Text";
            materialComboBox1.ValueMember = "Value";
            materialComboBox1.Items.Add(new { Text = Strings.CalltoActionButton, Value = "1" });
            materialComboBox1.Items.Add(new { Text = Strings.ReplyButtons, Value = "2" });

            var tmpBtons = buttonHolderModel.buttons;

            if (buttonHolderModel.buttonType == "1")
            {
                materialComboBox1.SelectedIndex = 0;
            }
            else if(buttonHolderModel.buttonType == "2")
            {
                materialComboBox1.SelectedIndex = 1;
            }
            buttonHolderModel.buttons = tmpBtons;

            if (buttonHolderModel.buttons != null && buttonHolderModel.buttons.Count() > 0)
            {
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(100);
                    this.Invoke(new Action(() =>
                        generateButtons()));
                });
                
            }
            if (buttonHolderModel.editMode)
                materialButton3.Visible = true;

            
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
                            string selectedValue = (materialComboBox1.SelectedItem as dynamic).Value;
                            ButtonsModel b = buttonHolderModel.buttons.Where(x => x.id == btnId).FirstOrDefault();
                            b.editMode = true;
                            AddButton addButton = new AddButton(b, this, selectedValue);
                            addButton.ShowDialog();
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    break;
            }
        }

        private void initLanguages()
        {
            this.Text = Strings.Buttons;
            materialTextBox21.Hint = Strings.title + "*";
            materialLabel1.Text = Strings.Buttons;
            materialButton1.Text = Strings.AddButton;
            materialTextBox22.Hint = Strings.FooterText;
            materialLabel2.Text = Strings.ButtonType;
        }

        public void RecievButton(ButtonsModel _buttonsModel)
        {
            if (_buttonsModel.editMode == true)
            {
                int index = buttonHolderModel.buttons.FindIndex(x => x.id == _buttonsModel.id);
                _buttonsModel.editMode = false;
                buttonHolderModel.buttons[index] = _buttonsModel;
            }
            else
            {
                buttonHolderModel.buttons.Add(_buttonsModel);
            }
            generateButtons();
        }

        private void generateButtons()
        {
            string buttontext = Storage.DocumentHtmlString;
            string cssStyle = Storage.DocumentButtonStypeStrig;

            foreach (var item in buttonHolderModel.buttons)
            {
                string txt = "";

                if (item.buttonTypeEnum == ButtonTypeEnum.PHONE_NUMBER)
                {
                    txt = "📞" + item.text;
                }
                else if (item.buttonTypeEnum == ButtonTypeEnum.URL)
                {
                    txt = "🔗 " + item.text;
                }
                else
                {
                    txt = item.text;
                }
                buttontext += "<button style='margin:5px;width:100%;" + cssStyle + "' type='button' id='" + item.id + "' >" + txt + "</button>";
            }
            webBrowser1.DocumentText = buttontext + "</body></html>";

            if (buttonHolderModel.buttons.Count() >= 3)
            {
                materialButton1.Enabled = false;
            }
            else
            {
                materialButton1.Enabled = true;
            }
        }
        private void materialButton1_Click(object sender, EventArgs e)
        {
            if (materialComboBox1.SelectedIndex < 0)
            {
                Utils.showError(Strings.PleaseSelectButtonType);
                return;
            }
            ButtonsModel buttonsModel = new ButtonsModel();
            buttonsModel.buttonTypeEnum = ButtonTypeEnum.NONE;

            string selectedValue = (materialComboBox1.SelectedItem as dynamic).Value;

            AddButton form = new AddButton( buttonsModel, this, selectedValue);
            form.ShowDialog();
        }

        public void RemoveButton(ButtonsModel _buttonsModel)
        {
            int index = buttonHolderModel.buttons.FindIndex(x => x.id == _buttonsModel.id);
            buttonHolderModel.buttons.Remove(buttonHolderModel.buttons[index]);

            generateButtons();
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            if (buttonHolderModel.buttons.Count() == 0)
            {
                Utils.showError(Strings.PleaseaddOneorMoreButtons);
                return;
            }
            if (materialTextBox21.Text == "")
            {
                Utils.showError(Strings.PleaseEnterTitle);
                return;
            }

            buttonHolderModel.title = materialTextBox21.Text;
            buttonHolderModel.footer = materialTextBox22.Text;

            this.waSenderForm.RecievButton(buttonHolderModel);
            this.Close();
        }

        private void materialButton3_Click(object sender, EventArgs e)
        {
            this.waSenderForm.RemoveButton(buttonHolderModel);
            this.Close();
        }

        private void materialComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string buttontext = Storage.DocumentHtmlString;
            


            webBrowser1.DocumentText = buttontext ;
            
            string selectedValue = (materialComboBox1.SelectedItem as dynamic).Value;


            this.buttonHolderModel.buttonType = selectedValue;
            this.buttonHolderModel.buttons = new List<ButtonsModel>();
        }
    }
}
