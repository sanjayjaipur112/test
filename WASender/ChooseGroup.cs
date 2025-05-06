using MaterialSkin.Controls;
using Microsoft.Web.WebView2.WinForms;
using OpenQA.Selenium;
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
using WASender.Models;

namespace WASender
{
    public partial class ChooseGroup : MyMaterialPopOp
    {
        GetGroupMember getGroupMember;
        GroupMemberAdder groupMemberAdder;
        List<WAPI_GroupModel> wAPI_GroupModel;
        GeneralSettingsModel generalSettingsModel;
        GrabGroupActiveMembers grabGroupActiveMembers;
        WaSenderBrowser browser;
        bool searchCommunity = false;
        IWebDriver driver;

        public ChooseGroup(GroupMemberAdder _groupMemberAdder, List<WAPI_GroupModel> _wAPI_GroupModel)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            groupMemberAdder = _groupMemberAdder;
            init(_wAPI_GroupModel);
        }

        public ChooseGroup(GrabGroupActiveMembers _grabGroupActiveMembers, List<WAPI_GroupModel> _wAPI_GroupModel)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            grabGroupActiveMembers = _grabGroupActiveMembers;
            init(_wAPI_GroupModel);
        }

        public ChooseGroup(GetGroupMember _getGroupMember, List<WAPI_GroupModel> _wAPI_GroupModel, bool _MultiSelect = false, bool _searchCommunity=false)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            getGroupMember = _getGroupMember;
            searchCommunity = _searchCommunity;
            if (_MultiSelect == true)
            {
                materialListBox1.SelectionMode = SelectionMode.MultiSimple;
            }

            init(_wAPI_GroupModel);
        }

        private void init(List<WAPI_GroupModel> _wAPI_GroupModel)
        {
            generalSettingsModel = Config.GetSettings();

            if (Utils.Driver != null)
            {
                if (generalSettingsModel.browserType == 1)
                {
                    Utils.SetDriver();
                    driver = Utils.Driver;
                }
            }
            if (Utils.waSenderBrowser != null)
            {
                browser = Utils.waSenderBrowser;
            }

           
            wAPI_GroupModel = _wAPI_GroupModel;
            initLanguage();

            try
            {
                foreach (var item in wAPI_GroupModel)
                {
                    MaterialSkin.MaterialListBoxItem lbitem = new MaterialSkin.MaterialListBoxItem();
                    lbitem.Text = item.GroupName;
                }
            }
            catch (Exception ex)
            {

            }
            materialListBox1.DataSource = wAPI_GroupModel;
            materialListBox1.ValueMember = "GroupId";
            materialListBox1.DisplayMember = "GroupName";
        }
       
        private void initLanguage()
        {
            this.Text = Strings.ChooseGroup;
            materialButton1.Text = Strings.Select;
            materialTextBox21.Hint = Strings.Search;
            materialButton2.Text = Strings.Refresh;
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            if (materialListBox1.SelectedIndex < 0)
            {
                MaterialSnackBar SnackBarMessage = new MaterialSnackBar(Strings.PleaseSelectGroup, Strings.OK, true);
                SnackBarMessage.Show(this);
            }
            else
            {
                if (this.getGroupMember != null)
                {
                    List<WAPI_GroupModel> _WAPI_GroupModel = new List<WAPI_GroupModel>();
                    foreach (WAPI_GroupModel item in materialListBox1.SelectedItems)
                    {
                        _WAPI_GroupModel.Add(item);
                    }
                    this.getGroupMember.ReturnBack(_WAPI_GroupModel);
                }
                else if (groupMemberAdder != null)
                {
                    var item = (WAPI_GroupModel)materialListBox1.SelectedItems[0];

                    this.groupMemberAdder.ReturnBack(item);
                }
                else if (grabGroupActiveMembers != null)
                {
                    var item = (WAPI_GroupModel)materialListBox1.SelectedItems[0];
                    this.grabGroupActiveMembers.ReturnBack(item);
                }

                //

                this.Hide();
            }
        }

        private void materialTextBox21_TextChanged(object sender, EventArgs e)
        {
            materialListBox1.DataSource = wAPI_GroupModel.Where(x => x.GroupName.ToUpper().Contains(materialTextBox21.Text.ToUpper())).ToList();
            materialListBox1.ValueMember = "GroupId";
            materialListBox1.DisplayMember = "GroupName";
        }

        private async void materialButton2_Click(object sender, EventArgs e)
        {
            materialButton2.Enabled = false;
            if (generalSettingsModel.browserType == 1)
            {
                if (Config.SendingType == 1)
                {
                    if (!WAPIHelper.IsWAPIInjected(driver))
                    {
                        ProjectCommon.injectWapi(driver);
                    }
                    if (searchCommunity)
                    {
                        wAPI_GroupModel = WAPIHelper.getMyCommunities(driver);
                    }
                    else
                    {
                        wAPI_GroupModel = await WAPIHelper.getMyGroups(driver);
                    }
                    
                    init(wAPI_GroupModel);
                }
            }
            else if (generalSettingsModel.browserType == 2)
            {
                WebView2 wv = Utils.GetActiveWebView(browser);

                if (!await WPPHelper.isWPPinjected(wv))
                {
                    await WPPHelper.InjectWapi(wv, Config.GetSysFolderPath());
                    Thread.Sleep(1000);
                }
                if (searchCommunity)
                {
                    wAPI_GroupModel = await WPPHelper.getMyCommunities(wv);
                }
                else
                {
                    wAPI_GroupModel = await WPPHelper.getMyGroups(wv);
                }
                init(wAPI_GroupModel);
              
            }
            if (this.groupMemberAdder != null)
            {
                this.groupMemberAdder.wAPI_GroupModel = wAPI_GroupModel;
            }
            materialButton2.Enabled = true;
        }
    }
}
