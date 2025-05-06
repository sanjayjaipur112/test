using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaAutoReplyBot;
using WASender.Models;
using WASender.enums;
using FluentValidation.Results;
using MaterialSkin.Controls;
using WASender.Validators;

namespace WASender
{
    public partial class AddButton : MyMaterialPopOp
    {
        ButtonsModel buttonsModel;
        ButtonHolder buttonHolder;
        string buttonType = "1";



        public AddButton(ButtonsModel _buttonsModel, ButtonHolder _buttonHolder = null, string _buttonType = "1")
        {
            buttonsModel = _buttonsModel;
            buttonHolder = _buttonHolder;
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            this.buttonType = _buttonType;
            Run();
        }

        private void Run()
        {
            init();
            var ss = buttonsModel.buttonTypeEnum;
            if (buttonsModel.buttonTypeEnum == ButtonTypeEnum.NONE)
            {
                materialComboBox1.SelectedIndex = 0;
            }
            else if (buttonsModel.buttonTypeEnum == ButtonTypeEnum.URL)
            {
                materialComboBox1.SelectedIndex = 0;
                materialTextBox22.Hint = Strings.EnterURL;
                materialTextBox22.Text = buttonsModel.url;
            }
            else if (buttonsModel.buttonTypeEnum == ButtonTypeEnum.PHONE_NUMBER)
            {
                materialComboBox1.SelectedIndex = 1;
                materialTextBox22.Hint = Strings.EnterPhoneNumber;
                materialTextBox22.Text = buttonsModel.phoneNumber;
            }

            if (buttonsModel.editMode == false)
            {
                btnDelete.Hide();
            }

            materialTextBox21.Text = buttonsModel.text;
            ComboChange();
        }

        private void init()
        {
            //this.Text = Strings.AddButtons;
            //materialTextBox21.Hint = Strings.ButtonText;
            //materialLabel1.Text = Strings.ButtonType;
            

            materialComboBox1.DisplayMember = "Text";
            materialComboBox1.ValueMember = "Value";

            if (this.buttonType == "1")
            {
                materialComboBox1.Items.Add(new { Text = Strings.Link, Value = "1" });
                materialComboBox1.Items.Add(new { Text = Strings.PhoneNumber, Value = "2" });
            }
            else if (this.buttonType == "2")
            {
                materialComboBox1.Items.Add(new { Text = Strings.NormalButton, Value = "3" });
            }

            
            

          

            materialTextBox22.Hide();
        }

        private void materialComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboChange();
        }

        private void ComboChange()
        {
            string selectedValue = (materialComboBox1.SelectedItem as dynamic).Value;
            
             if (selectedValue == "1")
            {
                materialTextBox22.Show();
                materialTextBox22.Hint = Strings.EnterURL;
            }
            else if (selectedValue == "2")
            {
                materialTextBox22.Show();
                materialTextBox22.Hint = Strings.EnterPhoneNumber;
            }
             else if (selectedValue == "3")
             {
                 materialTextBox22.Hide();
             }
        }


        private void GenerateModel()
        {
            buttonsModel.text = materialTextBox21.Text;

            string selectedValue = "";

            try
            {
                selectedValue = (materialComboBox1.SelectedItem as dynamic).Value;
            }
            catch (Exception ex)
            {

            }

            if (selectedValue == "1")
            {
                buttonsModel.buttonTypeEnum = ButtonTypeEnum.URL;
                buttonsModel.url = materialTextBox22.Text;
            }
            else if (selectedValue == "2")
            {
                buttonsModel.buttonTypeEnum = ButtonTypeEnum.PHONE_NUMBER;
                buttonsModel.phoneNumber = materialTextBox22.Text;
            }
            else if (selectedValue == "3")
            {
                buttonsModel.buttonTypeEnum = ButtonTypeEnum.NONE;
            }
            
            List<ValidationFailure> validator = new ButtonModelValidator().Validate(buttonsModel).Errors.ToList();


            MaterialSnackBar SnackBarMessage;
            if (validator.Count() > 0)
            {
                foreach (var item in validator)
                {
                    SnackBarMessage = new MaterialSnackBar(item.ErrorMessage, Strings.OK, true);
                    SnackBarMessage.Show(this);
                }
            }
            else
            {
                buttonsModel.id = Guid.NewGuid().ToString();
               
                if (buttonHolder != null)
                {
                    buttonHolder.RecievButton(buttonsModel);
                }
                //else if (addMessage != null)
                //{
                //    addMessage.RecievButton(buttonsModel);
                //}

                this.Hide();
            }

        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            GenerateModel();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            
            if (buttonHolder != null)
            {
                buttonHolder.RemoveButton(buttonsModel);
            }

            this.Hide();
        }

        private void AddButton_Load(object sender, EventArgs e)
        {
            initLanguages();
        }

        private void initLanguages()
        {
            this.Text = Strings.AddButtons;
            materialTextBox21.Hint = Strings.ButtonText;
            materialLabel1.Text = Strings.ButtonType;
            btnDelete.Text = Strings.Delete;
            btnSave.Text = Strings.Save;
        }
    }
}
