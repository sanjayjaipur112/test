using MaterialSkin.Controls;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaAutoReplyBot.enums;
using WaAutoReplyBot.Models;
using WASender;
using WASender.enums;
using WASender;
using OpenQA.Selenium.Interactions;
using WASender.Models;
using System.Web;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using WASender.Model;
using Newtonsoft.Json;


namespace WaAutoReplyBot
{
    public partial class WaAutoReplyForm : MyMaterialForm
    {
        List<RuleTransactionModel> ruleTransactionModelList;
        DataTable dtEmp;
        InitStatusEnum initStatusEnum;
        IWebDriver driver;
        System.Windows.Forms.Timer timerInitChecker;
        BackgroundWorker worker;
        private static bool IsRunning = false;
        private static string strLog = "";
        System.Windows.Forms.Timer timerRunner;
        WaSenderForm WaSenderForm;
        Logger logger;
        bool _AutoOpen = false;

        GeneralSettingsModel generalSettingsModel;
        WaSenderBrowser browser;
        private TestClass _testClass;
        WebView2 wv;

        private System.ComponentModel.BackgroundWorker backgroundWorker_productChecker;
        Progressbar pgbar;

        public WaAutoReplyForm(WaSenderForm _WaSenderForm, bool isAutoOpen = false)
        {

            logger = new Logger("AutoReplyBot");
            WaSenderForm = _WaSenderForm;
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            generalSettingsModel = Config.GetSettings();

            if (Utils.waSenderBrowser != null)
            {
                browser = Utils.waSenderBrowser;
                initWABrowser();
            }
            _testClass = Utils.testClass;
            _testClass.OnUpdateStatus += _testClass_OnUpdateStatus;
        }

        void _testClass_OnUpdateStatus(object sender, ProgressEventArgs e)
        {
            isWebViewRegistered = false;
            IsReceiveLoginDataRegistered = false;
            ChangeInitStatus(InitStatusEnum.Stopped);
        }

        private async void WaitForReopen()
        {
            await Task.Delay(TimeSpan.FromHours(3));
            ReOpen();
        }

        private void initWABrowser()
        {
            ChangeInitStatus(InitStatusEnum.Initialising);
            retryAttempt = 0;
            if (Utils.waSenderBrowser != null)
            {
                browser = Utils.waSenderBrowser;
            }
            else
            {
                browser = new WaSenderBrowser();
                Utils.waSenderBrowser = browser;
                browser.Show();
            }
            checkQRScanDoneBrowser();
        }

        private void checkQRScanDoneBrowser()
        {
            Thread.Sleep(1000);
            logger.WriteLog("checkQRScanDone");
            timerInitChecker = new System.Windows.Forms.Timer();
            timerInitChecker.Interval = 1000;
            timerInitChecker.Tick += timerInitChecker_Tick;
            timerInitChecker.Start();
        }

        private void initTimer()
        {
            timerRunner = new System.Windows.Forms.Timer();
            timerRunner.Interval = 1000;
            timerRunner.Tick += timerRunnerChecker_Tick;
            timerRunner.Start();
        }

        public void timerRunnerChecker_Tick(object sender, EventArgs e)
        {
            txtLog.Text = strLog;
        }

        public static void WriteLogg(string msg)
        {
            strLog = msg + Environment.NewLine + strLog;
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            AddRule addRule = new AddRule(new RuleTransactionModel(), this);
            addRule.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            init();
            initLanguage();
            if (this._AutoOpen)
            {
                RunNow();
            }
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                this.Invoke(new Action(() =>
                    CheckForActivation()));
            });

        }

        private void CheckForActivation()
        {
            pgbar = new Progressbar();
            this.backgroundWorker_productChecker = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker_productChecker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_productChecker_DoWork);
            this.backgroundWorker_productChecker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker2_RunWorkerCompleted); ;
            this.backgroundWorker_productChecker.RunWorkerAsync();
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pgbar.Close();
            if (e.Cancelled)
            {
                MessageBox.Show("Operation was canceled");
            }
            else if (e.Error != null)
            {
                MessageBox.Show("Operation was canceled");
            }
            else
            {
                try
                {
                    bool mode = (bool)e.Result;
                    if (mode == false)
                    {
                        MessageBox.Show(Strings.ProductIsNotActivated, Strings.ProductIsNotActivated, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                catch (Exception ex)
                {

                }

            }
        }

        private void backgroundWorker_productChecker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = CheckForActivationInternal();
        }

        private bool CheckForActivationInternal()
        {
            try
            {
                if (generalSettingsModel.browserType == 1)
                {
                    WAPIHelper.CheckExecutingAssembly();
                    return true;
                }
                else
                {
                    WPPHelper.CheckExecutingAssembly();
                    return true;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }


        private void initLanguage()
        {
            this.Text = Strings.WhatsAppBot;
            materialLabel1.Text = Strings.Rules;
            materialButton2.Text = Strings.AddRule;
            label7.Text = Strings.Status;
            materialLabel2.Text = Strings.Log;
            materialButton1.Text = Strings.Start;
            materialButton4.Text = Strings.Stop;
            label2.Text = Strings.Note;
            label1.Text = Strings.MarkAllchatasread;
            lblInitStatus.Text = Strings.NotInitialised;
            label3.Text = Strings.Toeditaruledoubleclickonit;
        }

        private void init()
        {
            ruleTransactionModelList = new List<RuleTransactionModel>();
            dtEmp = new DataTable();
            dtEmp.Columns.Add(Strings.IsActive, typeof(bool));
            dtEmp.Columns.Add(Strings.UserInput, typeof(string));
            dtEmp.Columns.Add(Strings.Type, typeof(string));
            dtEmp.Columns.Add(Strings.Messages, typeof(string));
            gridRulesets.DataSource = dtEmp;
            gridRulesets.Columns[0].Width = 60;
            gridRulesets.Columns[0].ReadOnly = false;
            gridRulesets.Columns[1].ReadOnly = true;
            gridRulesets.Columns[2].ReadOnly = true;
            gridRulesets.Columns[3].ReadOnly = true;

            string ObjData = Newtonsoft.Json.JsonConvert.SerializeObject(this.ruleTransactionModelList);
            String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            String path = Path.Combine(FolderPath, "WaAutoreplyRules.json");
            if (File.Exists(path))
            {
                string text = File.ReadAllText(path);
                var tempruleTransactionModelList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RuleTransactionModel>>(text);

                foreach (RuleTransactionModel ruleTransactionModel in tempruleTransactionModelList.ToList())
                {
                    AddRuleTRansaction(ruleTransactionModel);
                }
            }
        }

        public void RemoveItem()
        {
            var ss = gridRulesets.CurrentCell.RowIndex;
            this.ruleTransactionModelList.RemoveAt(ss);
            dtEmp.Rows.RemoveAt(ss);
        }

        public void HandleChieldEditMode()
        {
            try
            {
                var ss = gridRulesets.CurrentCell.RowIndex;
                this.ruleTransactionModelList[ss].IsEditMode = false;
            }
            catch (Exception ex)
            {

            }
        }

        public void AddRuleTRansaction(RuleTransactionModel _ruleTransactionModel, bool addtoTrans = true)
        {
            if (_ruleTransactionModel.IsEditMode != true)
            {
                if (addtoTrans == true)
                {
                    ruleTransactionModelList.Add(_ruleTransactionModel);
                }
                dtEmp.Rows.Add(true, _ruleTransactionModel.userInput, _ruleTransactionModel.operatorsEnum, _ruleTransactionModel.messages.Count());
            }
            else
            {
                var ss = gridRulesets.CurrentCell.RowIndex;
                this.ruleTransactionModelList[ss] = _ruleTransactionModel;
                this.ruleTransactionModelList[ss].IsEditMode = false;
                dtEmp.Rows[ss][1] = this.ruleTransactionModelList[ss].userInput;
                dtEmp.Rows[ss][2] = this.ruleTransactionModelList[ss].operatorsEnum;
                dtEmp.Rows[ss][3] = this.ruleTransactionModelList[ss].messages.Count();
            }
        }

        private void gridRulesets_DoubleClick(object sender, EventArgs e)
        {
            var ss = gridRulesets.CurrentCell.RowIndex;
            this.ruleTransactionModelList[ss].IsEditMode = true;
            AddRule addRule = new AddRule(this.ruleTransactionModelList[ss], this);
            addRule.ShowDialog();
        }

        private void ChangeInitStatus(InitStatusEnum _initStatus)
        {
            this.initStatusEnum = _initStatus;
            AutomationCommon.ChangeInitStatus(_initStatus, lblInitStatus);
            if (_initStatus == InitStatusEnum.Initialised || _initStatus == InitStatusEnum.Initialising || _initStatus == InitStatusEnum.Started)
            {
                materialCard1.Enabled = false;
            }
            else
            {
                materialCard1.Enabled = true;
            }
        }

        private void checkQRScanDone()
        {
            timerInitChecker = new System.Windows.Forms.Timer();
            timerInitChecker.Interval = 1000;
            timerInitChecker.Tick += timerInitChecker_Tick;
            timerInitChecker.Start();
        }
        int retryAttempt = 0;

        public async void timerInitChecker_Tick(object sender, EventArgs e)
        {
            if (generalSettingsModel.browserType == 1)
            {
                try
                {
                    bool isElementDisplayed = AutomationCommon.IsElementPresent(By.ClassName("_1XkO3"), driver);
                    if (isElementDisplayed == true)
                    {
                        ChangeInitStatus(InitStatusEnum.Initialised);
                        timerInitChecker.Stop();
                        initBackgroundWorker();
                        Activate();
                        WaAutoReplyForm.IsRunning = true;
                        worker.RunWorkerAsync();
                        initTimer();
                    }
                }
                catch (Exception ex)
                {
                    ChangeInitStatus(InitStatusEnum.Unable);
                    timerInitChecker.Stop();
                }

                try
                {
                    bool isElementDisplayed = AutomationCommon.IsElementPresent(By.ClassName("_1jJ70"), driver);
                    if (isElementDisplayed == true)
                    {
                        ChangeInitStatus(InitStatusEnum.Initialised);
                        timerInitChecker.Stop();
                        initBackgroundWorker();
                        Activate();
                        WaAutoReplyForm.IsRunning = true;
                        worker.RunWorkerAsync();
                        initTimer();
                    }
                }
                catch (Exception ex)
                {

                    if (retryAttempt == 5)
                    {
                        retryAttempt = 0;
                        ChangeInitStatus(InitStatusEnum.Unable);
                        timerInitChecker.Stop();
                    }
                    else
                    {
                        retryAttempt++;
                        Debug.WriteLine("Retry attempt ." + retryAttempt.ToString());
                        Thread.Sleep(1000);
                    }
                }

                try
                {
                    bool isElementDisplayed = AutomationCommon.IsElementPresent(By.ClassName("_aigu"), driver);
                    if (isElementDisplayed == true)
                    {
                        ChangeInitStatus(InitStatusEnum.Initialised);
                        timerInitChecker.Stop();
                        initBackgroundWorker();
                        Activate();
                        WaAutoReplyForm.IsRunning = true;
                        worker.RunWorkerAsync();
                        initTimer();
                    }
                }
                catch (Exception ex)
                {

                    if (retryAttempt == 5)
                    {
                        retryAttempt = 0;
                        ChangeInitStatus(InitStatusEnum.Unable);
                        timerInitChecker.Stop();
                    }
                    else
                    {
                        retryAttempt++;
                        Debug.WriteLine("Retry attempt ." + retryAttempt.ToString());
                        Thread.Sleep(1000);
                    }
                }
            }
            else if (generalSettingsModel.browserType == 2)
            {
                try
                {

                    WebView2 vw = Utils.GetActiveWebView(browser);
                    bool isWainited = await WPPHelper.isWaInited(vw);


                    if (isWainited)
                    {
                        timerInitChecker.Stop();
                    
                        if (isWebViewRegistered == false)
                        {
                            wv = new WebView2();
                        }
                        await Task.Delay(1000);

                        browser.Invoke((MethodInvoker)delegate
                        {
                            if (isWebViewRegistered == false)
                            {
                                wv = Utils.GetActiveWebView(browser);
                                wv.CoreWebView2.WebMessageReceived += ReceiveLoginData;
                                isWebViewRegistered = true;
                            }
                        });

                        if (!await WPPHelper.isWPPinjectedAsync(wv))
                        {
                            await WPPHelper.InjectWapi(wv, Config.GetSysFolderPath());
                            await Task.Delay(1000);
                        }
                        if (IsReceiveLoginDataRegistered == false)
                        {
                            WPPHelper.Registerpoll_response(wv);
                            await Task.Delay(1000);
                            WPPHelper.onNewMessage(wv);
                            IsReceiveLoginDataRegistered = true;
                        }

                        ChangeInitStatus(InitStatusEnum.Initialised);
                        
                        //initBackgroundWorker();
                        Activate();
                        WaAutoReplyForm.IsRunning = true;
                        //worker.RunWorkerAsync();
                        initTimer();
                    }
                }
                catch (Exception ex)
                {
                    timerInitChecker.Stop();
                }
            }

        }

        private void initBackgroundWorker()
        {
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            ChangeInitStatus(InitStatusEnum.Started);
        }


        private List<RuleTransactionModel> ExactStartsWith(List<RuleTransactionModel> _RuleTransactionModelList, string userInput)
        {
            List<RuleTransactionModel> matchedRules = new List<RuleTransactionModel>();
            foreach (var item in _RuleTransactionModelList)
            {
                if (userInput.ToUpper().StartsWith(item.userInput.ToUpper()))
                {
                    matchedRules.Add(item);
                }
            }
            return matchedRules;
        }

        private List<RuleTransactionModel> ExactEndsWith(List<RuleTransactionModel> _RuleTransactionModelList, string userInput)
        {
            List<RuleTransactionModel> matchedRules = new List<RuleTransactionModel>();
            foreach (var item in _RuleTransactionModelList)
            {
                if (userInput.ToUpper().EndsWith(item.userInput.ToUpper()))
                {
                    matchedRules.Add(item);
                }
            }
            return matchedRules;
        }

        private static List<UnReadMessagesModel> staticUnreadModel;
        void ReceiveLoginData(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            if (WaAutoReplyForm.IsRunning == true)
            {
                String loginDataAsJson = args.WebMessageAsJson;
                if (staticUnreadModel == null)
                {
                    staticUnreadModel = new List<UnReadMessagesModel>();
                }
                PostMessageModel result = JsonConvert.DeserializeObject<PostMessageModel>(loginDataAsJson);
                if (result.type == "poll")
                {
                    SelectedOptionModel pollData = JsonConvert.DeserializeObject<SelectedOptionModel>(result._data);
                    IncommingMessageModel unReadMessagesModel = new IncommingMessageModel();
                    //unReadMessagesModel.id = pollData.id;
                    unReadMessagesModel.body = pollData.selectedOption;
                    unReadMessagesModel.from = pollData.user;
                    //unReadMessagesModel.isPoll = true;
                    gotNewMessage(unReadMessagesModel);
                }
                else if (result.type == "newmsg")
                {
                    IncommingMessageModel _data = JsonConvert.DeserializeObject<IncommingMessageModel>(result._data);
                    if ((_data.body != null) && (_data.id.fromMe != true) && (!_data.id._serialized.Contains("@g.us")))
                    {
                        gotNewMessage(_data);
                    }
                }
            }
        }
        bool IsReceiveLoginDataRegistered = false;
        bool isWebViewRegistered = false;

        private RuleTransactionModel checkContains(List<RuleTransactionModel> containsMessageList, string userInput)
        {
            bool isMatch = false;
            RuleTransactionModel model = new RuleTransactionModel();
            foreach (RuleTransactionModel item in containsMessageList)
            {
                if (userInput.Trim().ToUpper().Contains(item.userInput.Trim().ToUpper()))
                {
                    model = item;
                    isMatch = true;
                }
            }
            if (!isMatch)
            {
                model = null;
            }
            return model;
        }

        public static bool ContainsAll(string source,List<string> values)
        {
            return values.All(x => source.Contains(x));
        }
        public void gotNewMessage(IncommingMessageModel incommingMessageModel)
        {
            List<RuleTransactionModel> exactMessageList = this.ruleTransactionModelList.Where(x => x.operatorsEnum == OperatorsEnum.Exact).ToList();
            List<RuleTransactionModel> containsMessageList = this.ruleTransactionModelList.Where(x => x.operatorsEnum == OperatorsEnum.Contains).ToList();
            List<RuleTransactionModel> startFromMessageList = this.ruleTransactionModelList.Where(x => x.operatorsEnum == OperatorsEnum.StartFrom).ToList();
            List<RuleTransactionModel> endsWithFromMessageList = this.ruleTransactionModelList.Where(x => x.operatorsEnum == OperatorsEnum.EndsWith).ToList();
            List<RuleTransactionModel> fallbackMesageList = this.ruleTransactionModelList.Where(x => x.IsFallBack == true).ToList();

            List<IncommingMessageModel> sendMessagesList = new List<IncommingMessageModel>();
            incommingMessageModel.from = incommingMessageModel.from.Replace("@c.us", "");

            

            if (sendMessagesList.Where(x => x.id._serialized == incommingMessageModel.id._serialized).Count() == 0)
            {
                bool foundContains = false;
                RuleTransactionModel containstmodel = null;
                foreach (RuleTransactionModel item in containsMessageList)
                {
                    bool currentFoundCOntainst = ContainsAll(incommingMessageModel.body, item.userInput.Split(' ').ToList());
                    if (currentFoundCOntainst)
                    {
                        foundContains = true;
                        containstmodel = item;
                    }
                    
                }
                //bool contatsSll = ContainsAll(incommingMessageModel.body, containsMessageList.Select(x=>x.userInput).ToList());

                string lastMessageText = incommingMessageModel.body;
                if (ExactStartsWith(startFromMessageList, lastMessageText).Count() > 0)
                {
                    RuleTransactionModel model = ExactStartsWith(startFromMessageList, lastMessageText).LastOrDefault();
                    sendMessage(model, incommingMessageModel.from);
                    sendMessagesList.Add(incommingMessageModel);
                }
                else if (ExactEndsWith(endsWithFromMessageList, lastMessageText).Count() > 0)
                {

                    RuleTransactionModel model = ExactEndsWith(endsWithFromMessageList, lastMessageText).LastOrDefault();
                    sendMessage(model, incommingMessageModel.from);
                    sendMessagesList.Add(incommingMessageModel);
                }
                else if (exactMessageList.Where(x => x.userInput.ToUpper() == lastMessageText.ToUpper()).Count() > 0)
                {

                    RuleTransactionModel model = exactMessageList.Where(x => x.userInput.ToUpper() == lastMessageText.ToUpper()).LastOrDefault();
                    sendMessage(model, incommingMessageModel.from);
                    sendMessagesList.Add(incommingMessageModel);
                }
                else if (foundContains)
                {
                    sendMessage(containstmodel, incommingMessageModel.from);
                    sendMessagesList.Add(incommingMessageModel);
                }
                else
                {
                    var splitter = lastMessageText.ToUpper().Split(' ');
                    bool found = false;
                    foreach (var itemo in containsMessageList)
                    {
                        if (splitter.Contains(itemo.userInput.ToUpper()))
                        {
                            sendMessage(itemo, incommingMessageModel.from);
                            sendMessagesList.Add(incommingMessageModel);
                            found = true;
                        }
                    }

                    if (found == false)
                    {
                        if (fallbackMesageList.Count > 0)
                        {
                            RuleTransactionModel model = fallbackMesageList[Utils.getRandom(0, fallbackMesageList.Count() - 1)];
                            sendMessage(model, incommingMessageModel.from);
                            sendMessagesList.Add(incommingMessageModel);
                        }

                    }
                }
            }
        }
        private async void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<RuleTransactionModel> exactMessageList = this.ruleTransactionModelList.Where(x => x.operatorsEnum == OperatorsEnum.Exact).ToList();
            List<RuleTransactionModel> containsMessageList = this.ruleTransactionModelList.Where(x => x.operatorsEnum == OperatorsEnum.Contains).ToList();
            List<RuleTransactionModel> startFromMessageList = this.ruleTransactionModelList.Where(x => x.operatorsEnum == OperatorsEnum.StartFrom).ToList();
            List<RuleTransactionModel> endsWithFromMessageList = this.ruleTransactionModelList.Where(x => x.operatorsEnum == OperatorsEnum.EndsWith).ToList();
            List<RuleTransactionModel> fallbackMesageList = this.ruleTransactionModelList.Where(x => x.IsFallBack == true).ToList();

            List<UnReadMessagesModel> sendMessagesList = new List<UnReadMessagesModel>();
            while (WaAutoReplyForm.IsRunning == true)
            {
                if (isWebViewRegistered == false)
                {
                    wv = new WebView2();
                }
                Thread.Sleep(1000);

                if (1 == 2)
                {

                }
                else if (Config.SendingType == 1)
                {

                    #region Chrome
                    try
                    {
                        if (generalSettingsModel.browserType == 1)
                        {
                            if (!WAPIHelper.IsWAPIInjected(driver))
                            {
                                WASender.ProjectCommon.injectWapi(driver);
                            }
                        }
                        if (generalSettingsModel.browserType == 2)
                        {
                            try
                            {
                                browser.Invoke((MethodInvoker)delegate
                                {
                                    if (isWebViewRegistered == false)
                                    {
                                        wv = Utils.GetActiveWebView(browser);
                                        wv.CoreWebView2.WebMessageReceived += ReceiveLoginData;
                                        isWebViewRegistered = true;
                                    }


                                });


                                if (!await WPPHelper.isWPPinjectedAsync(wv))
                                {
                                    await WPPHelper.InjectWapi(wv, Config.GetSysFolderPath());
                                    Thread.Sleep(1000);
                                }
                                if (IsReceiveLoginDataRegistered == false)
                                {
                                    WPPHelper.Registerpoll_response(wv);
                                    Thread.Sleep(1000);
                                    WPPHelper.onNewMessage(wv);
                                    IsReceiveLoginDataRegistered = true;
                                }

                            }
                            catch (Exception ex)
                            {

                            }
                        }

                        List<UnReadMessagesModel> UnReadMessages = new List<UnReadMessagesModel>();
                        if (generalSettingsModel.browserType == 1)
                        {
                            UnReadMessages = WAPIHelper.GetAllUnreadMessages2(driver);
                            UnReadMessages.AddRange(WAPIHelper._newMessagesBuffer(driver));
                        }
                        if (generalSettingsModel.browserType == 2)
                        {
                            UnReadMessages = WPPHelper.GetAllUnreadMessages2(wv);
                            UnReadMessages.AddRange(WPPHelper.GetMessagesBuffer(wv));
                        }
                        if (staticUnreadModel != null)
                        {
                            UnReadMessages.AddRange(staticUnreadModel);
                            staticUnreadModel = null;
                        }
                        foreach (var item in UnReadMessages)
                        {
                            try
                            {
                                Thread.Sleep(1000);
                                if ((sendMessagesList.Where(x => x.id == item.id).Count() == 0) || item.isPoll == true)
                                {
                                    if (item.body != null)
                                    {
                                        string lastMessageText = item.body;
                                        RuleTransactionModel contaisModel = checkContains(containsMessageList, lastMessageText);

                                        logger.WriteLog("lastMessageText = " + lastMessageText);
                                        if (ExactStartsWith(startFromMessageList, lastMessageText).Count() > 0)
                                        {
                                            logger.WriteLog("Match with ExactStartsWith");
                                            RuleTransactionModel model = ExactStartsWith(startFromMessageList, lastMessageText).LastOrDefault();
                                            sendMessage(model, item.chatId);
                                            sendMessagesList.Add(item);
                                        }
                                        else if (ExactEndsWith(endsWithFromMessageList, lastMessageText).Count() > 0)
                                        {
                                            logger.WriteLog("Match with ExactEndsWith");
                                            RuleTransactionModel model = ExactEndsWith(endsWithFromMessageList, lastMessageText).LastOrDefault();
                                            sendMessage(model, item.chatId);
                                            sendMessagesList.Add(item);
                                        }
                                        else if (exactMessageList.Where(x => x.userInput.ToUpper() == lastMessageText.ToUpper()).Count() > 0)
                                        {
                                            logger.WriteLog("Match with exactMessage");
                                            RuleTransactionModel model = exactMessageList.Where(x => x.userInput.ToUpper() == lastMessageText.ToUpper()).LastOrDefault();
                                            sendMessage(model, item.chatId);
                                            sendMessagesList.Add(item);
                                        }
                                        //else if (containsMessageList.Where(x => x.userInput.ToUpper().Contains(lastMessageText.ToUpper())).Count() > 0)
                                        else if (contaisModel != null)
                                        {
                                            logger.WriteLog("Match with contains");
                                            RuleTransactionModel model = containsMessageList.Where(x => x.userInput.ToUpper().Contains(lastMessageText.ToUpper())).LastOrDefault();
                                            sendMessage((RuleTransactionModel)contaisModel, item.chatId);
                                            sendMessagesList.Add(item);
                                        }
                                        else
                                        {
                                            var splitter = lastMessageText.ToUpper().Split(' ');
                                            bool found = false;
                                            foreach (var itemo in containsMessageList)
                                            {
                                                if (splitter.Contains(itemo.userInput.ToUpper()))
                                                {
                                                    sendMessage(itemo, item.chatId);
                                                    sendMessagesList.Add(item);
                                                    found = true;
                                                }
                                            }

                                            if (found == false)
                                            {
                                                logger.WriteLog("Fallback");
                                                if (fallbackMesageList.Count > 0)
                                                {
                                                    RuleTransactionModel model = fallbackMesageList[Utils.getRandom(0, fallbackMesageList.Count() - 1)];
                                                    sendMessage(model, item.chatId);
                                                    sendMessagesList.Add(item);
                                                }
                                                else
                                                {
                                                    if (generalSettingsModel.browserType == 1)
                                                    {
                                                        WAPIHelper.markIsRead(driver, item.chatId);
                                                        sendMessagesList.Add(item);
                                                    }
                                                    if (generalSettingsModel.browserType == 2)
                                                    {
                                                        WPPHelper.markIsRead(wv, item.chatId);
                                                        sendMessagesList.Add(item);
                                                    }

                                                }

                                            }
                                        }
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                logger.WriteLog(ex.Message);
                                if (fallbackMesageList.Count() > 0)
                                {
                                    logger.WriteLog("Fallback Found");
                                    RuleTransactionModel model = fallbackMesageList[Utils.getRandom(0, fallbackMesageList.Count() - 1)];
                                    sendMessage(model, item.chatId);
                                    sendMessagesList.Add(item);
                                }
                                else
                                {
                                    logger.WriteLog("Fallback not found");
                                }
                            }
                        }
                    }
                    catch (Exception edx)
                    {

                    }
                    #endregion



                }


            }
        }

        private async void sendMessage(RuleTransactionModel model, string number = "")
        {
            if (1 == 2)
            {

            }
            else if (Config.SendingType == 1)
            {
                if (model==null)
                {
                    return;
                }

                foreach (var mesageModel in model.messages)
                {
                    foreach (var file in mesageModel.Files)
                    {
                        Byte[] bytes = File.ReadAllBytes(file);
                        String filebase64 = Convert.ToBase64String(bytes);
                        string contentType = MimeMapping.GetMimeMapping(file);

                        string ext = Path.GetExtension(file);
                        if (ext == ".mp4")
                        {
                            contentType = "video/mp4";
                        }

                        string fullBase64 = "data:" + contentType + ";base64," + filebase64;
                        string FileName = file.Split('\\')[file.Split('\\').Length - 1];


                        if (ext == ".mp4")
                        {
                            try
                            {
                                if (generalSettingsModel.browserType == 1)
                                {
                                    //WAPIHelper.OpenChatopenChatBottom(driver, number);
                                    //Thread.Sleep(500);
                                    WAPIHelper.SendVideo(driver, number, fullBase64, "", FileName);
                                }
                                else if (generalSettingsModel.browserType == 2)
                                {
                                    //WPPHelper.OpenChatopenChatBottomAsync(wv, number);
                                    //Thread.Sleep(500);
                                    WPPHelper.SendVideoAsync(wv, number, fullBase64, "", FileName);
                                }

                            }
                            catch (Exception eeex)
                            {
                                logger.WriteLog("Is Number Valid-" + eeex.Message);
                            }
                        }
                        else if (ext == ".ogg")
                        {
                            fullBase64 = fullBase64.Replace("data:application/octet-stream;", "data:audio/mp3;");

                            try
                            {
                                if (generalSettingsModel.browserType == 1)
                                {
                                    //WAPIHelper.OpenChatopenChatBottom(driver, number);
                                    //Thread.Sleep(500);
                                    WAPIHelper.SendVideo(driver, number, fullBase64, "", FileName);
                                }
                                else if (generalSettingsModel.browserType == 2)
                                {
                                    //WPPHelper.OpenChatopenChatBottomAsync(wv, number);
                                    //Thread.Sleep(500);
                                    WPPHelper.SendVideoAsync(wv, number, fullBase64, "", FileName);
                                }

                            }
                            catch (Exception eeex)
                            {
                                logger.WriteLog("Is Number Valid-" + eeex.Message);
                            }
                        }
                        else
                        {
                            if (generalSettingsModel.browserType == 1)
                            {
                                WAPIHelper.SendAttachment(driver, number, fullBase64, FileName, "");
                            }
                            else if (generalSettingsModel.browserType == 2)
                            {
                                WPPHelper.SendAttachmentAsync(wv, number, fullBase64, FileName, "");
                            }
                            Thread.Sleep(2000);
                        }



                    }


                    //var messages = mesageModel.LongMessage.Split('\n');
                    string NewMessage = ProjectCommon.ReplaceKeyMarker(mesageModel.LongMessage);


                    if (mesageModel.polls != null && mesageModel.polls.Count() > 0)
                    {
                        foreach (PollModel item in mesageModel.polls)
                        {
                            try
                            {
                                if (generalSettingsModel.browserType == 1)
                                {

                                }
                                else if (generalSettingsModel.browserType == 2)
                                {
                                    WPPHelper.sendCreatePollMessageToNumber(wv, number, item);
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }


                    if (mesageModel.buttons != null && mesageModel.buttons.Count() > 0)
                    {
                        if (generalSettingsModel.browserType == 1)
                        {
                            WAPIHelper.sendButtonWithMessage(driver, mesageModel, number, NewMessage);
                        }
                    }
                    else
                    {
                        if (NewMessage.Contains("http://") || NewMessage.Contains("https://"))
                        {
                            if (generalSettingsModel.browserType == 1)
                            {
                                //WAPIHelper.OpenChatopenChatBottom(driver, number);
                                //Thread.Sleep(500);
                                WAPIHelper.sendTextMessage(driver, number, NewMessage);
                            }
                            else if (generalSettingsModel.browserType == 2)
                            {
                                //WPPHelper.OpenChatopenChatBottomAsync(wv, number);
                                //Thread.Sleep(500);
                                WPPHelper.sendTextMessage(wv, number, NewMessage);
                            }

                        }
                        else
                        {
                            if (generalSettingsModel.browserType == 1)
                            {
                                WAPIHelper.SendMessage(driver, number, NewMessage);
                            }
                            else if (generalSettingsModel.browserType == 2)
                            {
                                WPPHelper.SendMessage(wv, number, NewMessage);
                            }
                        }
                        Thread.Sleep(300);

                        if (generalSettingsModel.browserType == 1)
                        {
                            WAPIHelper.sendSeen(driver, number);
                        }
                        else if (generalSettingsModel.browserType == 2)
                        {
                            WPPHelper.sendSeen(wv, number);
                        }

                    }

                }


                if (!model.IsFallBack)
                {
                    WaAutoReplyForm.WriteLogg(number + "\t-- " + Strings.Match + " : \t" + model.operatorsEnum + "\t-- " + Strings.ReplySend + "!");
                }
                else
                {
                    WaAutoReplyForm.WriteLogg(number + "\t-- " + Strings.NotMatch + " :\t --" + Strings.Fallback + " \t--" + Strings.ReplySend + "!");
                }

            }
        }


        private async void RunNow()
        {
            if (!(ruleTransactionModelList.Count() > 0))
            {
                MaterialSnackBar SnackBarMessage = new MaterialSnackBar(Strings.PleaseaddRules, Strings.OK, true);
                SnackBarMessage.Show(this);
            }
            else
            {
                ChangeInitStatus(InitStatusEnum.Initialising);
                if (generalSettingsModel.browserType == 1)
                {
                    try
                    {
                        var s = driver.WindowHandles;
                    }
                    catch (Exception ex)
                    {
                        Utils.Driver = null;
                        this.driver = Utils.Driver;
                    }

                    try
                    {
                        Utils.SetDriver();
                        driver = Utils.Driver;

                        checkQRScanDone();
                    }
                    catch (Exception ex)
                    {
                        logger.WriteLog("error=" + ex.Message);
                        ChangeInitStatus(InitStatusEnum.Unable);
                        if (ex.Message.Contains("session not created"))
                        {
                            DialogResult dr = MessageBox.Show("Your Chrome Driver and Google Chrome Version Is not same, Click 'Yes botton' to Update it from Settings", "Error ", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);
                            if (dr == DialogResult.Yes)
                            {
                                this.Hide();
                                GeneralSettings generalSettings = new GeneralSettings();
                                generalSettings.ShowDialog();
                            }
                        }

                        else if (ex.Message.Contains("invalid argument: user data directory is already in use"))
                        {
                            _Config.KillChromeDriverProcess();
                            MaterialSnackBar SnackBarMessage = new MaterialSnackBar("Please Close All Previous Sessions and Browsers if open, Then try again", Strings.OK, true);
                            SnackBarMessage.Show(this);
                        }

                        if (ex.Message.Contains("The specified executable is not a valid application for this OS platform"))
                        {
                            if (generalSettingsModel.browserType == 2)
                            {
                                MessageBox.Show(Strings.MSEdgeDriversarenotDownloadedProperly, Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            if (generalSettingsModel.browserType == 1)
                            {
                                MessageBox.Show(Strings.ChromeDriversarenotDownloadedProperly, Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                else if (generalSettingsModel.browserType == 2)
                {
                    try
                    {

                        initWABrowser();
                    }
                    catch (Exception ex)
                    {

                    }
                }



            }
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            RunNow();
        }

        private void materialButton4_Click(object sender, EventArgs e)
        {
            if (initStatusEnum == InitStatusEnum.Initialised || initStatusEnum == InitStatusEnum.Started)
            {
                try
                {
                    WaAutoReplyForm.IsRunning = false;
                    timerRunner.Stop();
                    worker.CancelAsync();
                }
                catch (Exception Ex)
                {

                }

                if (Utils.waSenderBrowser != null)
                {
                    isWebViewRegistered = false;
                    IsReceiveLoginDataRegistered = false;
                    Utils.waSenderBrowser.Close();
                }
                ChangeInitStatus(InitStatusEnum.Stopped);
            }

        }

        private void WaAutoReplyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                string ObjData = Newtonsoft.Json.JsonConvert.SerializeObject(this.ruleTransactionModelList);
                String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                String path = Path.Combine(FolderPath, "WaAutoreplyRules.json");
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                }
                File.WriteAllText(path, ObjData);
            }
            catch (Exception ex)
            {

            }
            logger.Complete();
        }

        private void WaAutoReplyForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            WaSenderForm.formReturn(true);
        }

        private void ReOpen()
        {
            this.Hide();
            this.Close();
            this.WaSenderForm.reEnableAutoReply();
        }
    }
}
