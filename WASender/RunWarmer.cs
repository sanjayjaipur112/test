using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
    public partial class RunWarmer : MyMaterialPopOp
    {

        WarmerModel warmerModel;
        private BackgroundWorker backgroundWorker1;
        WaSenderBrowser waMultiInstance;
        public static bool IsRunning = false;
        InitStatusEnum initStatusEnum;
        CampaignStatusEnum campaignStatusEnum;
        System.Windows.Forms.Timer timerInitChecker;
        int retryAttempt = 0;
        private TestClass _testClass;
        private System.ComponentModel.BackgroundWorker backgroundWorker_productChecker;
        Progressbar pgbar;

        public RunWarmer(WarmerModel _warmerModel)
        {
            InitializeComponent();
            this.warmerModel = _warmerModel;
            _testClass = Utils.testClass;
            _testClass.OnUpdateStatus += _testClass_OnUpdateStatus;
            this.Icon = Strings.AppIcon;
        }

        void _testClass_OnUpdateStatus(object sender, ProgressEventArgs e)
        {
            ChangeInitStatus(InitStatusEnum.Stopped);
            ChangeCampStatus(CampaignStatusEnum.Stopped);
        }

        private void ChangeCampStatus(CampaignStatusEnum _campaignStatus)
        {
            this.campaignStatusEnum = _campaignStatus;
            AutomationCommon.ChangeCampStatus(_campaignStatus, lblRunStatus);
        }

        List<List<T>> ChunkBy<T>(List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
        private async void materialButton1_Click(object sender, EventArgs e)
        {
            if (initStatusEnum != InitStatusEnum.Initialised)
            {
                Utils.showAlert(Strings.PleasefollowStepNo1FirstInitialiseWhatsapp, Alerts.Alert.enmType.Error);
                return;
            }
            if (campaignStatusEnum != CampaignStatusEnum.Running && campaignStatusEnum != CampaignStatusEnum.Paused)
            {
                ChangeCampStatus(CampaignStatusEnum.Starting);
                var Accounts = warmerModel.SelectedAccountNames;
                warmerModel.SelectedAccountNames = new List<WarmerContactModel>();
                Accounts.Shuffle();
                warmerModel.SelectedAccountNames = Accounts;


                foreach (var account in warmerModel.SelectedAccountNames)
                {
                    foreach (TabPage tab in waMultiInstance.tabControl1.TabPages)
                    {
                        if (tab.Text == account.Name)
                        {
                            WebView2 vw = (WebView2)tab.Controls.Find("webView21", true)[0];
                            if (!await WPPHelper.isWPPinjectedAsync(vw))
                            {
                                string _result = await WPPHelper.InjectWapi(vw, Config.GetSysFolderPath());
                                await Task.Delay(500);
                            }
                            string Number = await WPPHelper.getMyUserId(vw);
                            account.Number = Number;
                            account.webview = vw;
                            account.tabPage = tab;
                        }
                    }
                }


                var chunks = ChunkBy<WarmerContactModel>(warmerModel.SelectedAccountNames, 2).ToList() ;

                foreach (List<WarmerContactModel> chunk in chunks)
                {
                    if (chunk.Count() == 1)
                    {
                        chunk.Add(warmerModel.SelectedAccountNames.FirstOrDefault());
                    }
                }

                foreach (List<WarmerContactModel> chunk in chunks)
                {
                    
                    foreach (WarmerContactModel _selectedAccount in chunk)
                    {
                        _selectedAccount.toAccountId = chunk.Where(x => x.ID != _selectedAccount.ID).FirstOrDefault().ID;
                    }
                }
                
                backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
                backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
                IsRunning = true;
                ChangeCampStatus(CampaignStatusEnum.Running);
                backgroundWorker1.RunWorkerAsync();
            }
            
            
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker helperBW = sender as BackgroundWorker;
            e.Result = BackgroundProcessLogicMethod(helperBW);
            if (helperBW.CancellationPending)
            {
                e.Cancel = true;
            }

        }

        private async Task<int> BackgroundProcessLogicMethod(BackgroundWorker bw)
        {
            int result = 0;

            List<string> MessagesList = warmerModel.selectedText.Split('\n').ToList();

            while (IsRunning)
            {
                foreach (WarmerContactModel account in warmerModel.SelectedAccountNames)
                {
                    if (!await WPPHelper.isWPPinjectedAsync(account.webview))
                    {
                        string _result = await WPPHelper.InjectWapi(account.webview, Config.GetSysFolderPath());
                    }
                }

               
                foreach (WarmerContactModel fromAccount in warmerModel.SelectedAccountNames)
                {
                    if (IsRunning)
                    {

                        if (warmerModel.warmmingMethod == 1)
                        {
                            #region onetoOne
                            WarmerContactModel toAccount = warmerModel.SelectedAccountNames.Where(x => x.ID == fromAccount.toAccountId).FirstOrDefault();
                            {
                                if (IsRunning)
                                {

                                    Random r = new Random();
                                    int rInt = r.Next(warmerModel.delayFrom * 1000, warmerModel.delayTo * 1000);
                                    r = new Random();
                                    int randomMessageIndex = r.Next(0, MessagesList.Count() - 1);
                                    string randomMessage = MessagesList[randomMessageIndex];
                                    try
                                    {
                                        waMultiInstance.tabControl1.Invoke((MethodInvoker)delegate
                                        {
                                            waMultiInstance.tabControl1.SelectedTab = fromAccount.tabPage;
                                        });
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    bool _result = WPPHelper.openChatAtBottomLong(fromAccount.webview, toAccount.Number);
                                    if (_result == false)
                                    {
                                        WPPHelper.openChatAtBottomLong(fromAccount.webview, toAccount.Number);
                                    }

                                    await WPPHelper.SendMessageFull(fromAccount.webview, toAccount.Number, rInt, randomMessage);


                                    dataGridView1.Invoke((MethodInvoker)delegate
                                    {
                                        DateTime dt = DateTime.Now;
                                        dataGridView1.Rows.Add(new object[]{
                                    fromAccount.Name,
                                    toAccount.Name,
                                    dt.Hour + ":" + dt.Minute + ":" + dt.Second,
                                    randomMessage
                                    });

                                        dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
                                    });




                                }
                                {
                                    Random r = new Random();
                                    int rInt = r.Next(warmerModel.delayFrom * 1000, warmerModel.delayTo * 1000);
                                    r = new Random();
                                    Thread.Sleep(rInt);
                                }
                                if (IsRunning)
                                {
                                    Random r = new Random();
                                    int rInt = r.Next(warmerModel.delayFrom * 1000, warmerModel.delayTo * 1000);
                                    r = new Random();
                                    int randomMessageIndex = r.Next(0, MessagesList.Count() - 1);
                                    string randomMessage = MessagesList[randomMessageIndex];
                                    try
                                    {
                                        waMultiInstance.tabControl1.Invoke((MethodInvoker)delegate
                                        {
                                            waMultiInstance.tabControl1.SelectedTab = toAccount.tabPage;
                                        });
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                    bool _result = WPPHelper.openChatAtBottomLong(toAccount.webview, fromAccount.Number);
                                    if (_result == false)
                                    {
                                        WPPHelper.openChatAtBottomLong(toAccount.webview, fromAccount.Number);
                                    }
                                    await WPPHelper.SendMessageFull(toAccount.webview, fromAccount.Number, rInt, randomMessage);


                                    dataGridView1.Invoke((MethodInvoker)delegate
                                    {
                                        DateTime dt = DateTime.Now;
                                        dataGridView1.Rows.Add(new object[]{
                                    toAccount.Name,
                                    fromAccount.Name,
                                    dt.Hour + ":" + dt.Minute + ":" + dt.Second,
                                    randomMessage
                                    });

                                        dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
                                    });
                                }
                                {
                                    Random r = new Random();
                                    int rInt = r.Next(warmerModel.delayFrom * 1000, warmerModel.delayTo * 1000);
                                    r = new Random();
                                    Thread.Sleep(rInt);
                                }
                            }
                            #endregion

                        }
                        else // One TO Many
                        {
                            # region ONeToMany
                            List<WarmerContactModel> _restAccounts = warmerModel.SelectedAccountNames.Where(x => x != fromAccount).ToList();
                            foreach (var toAccount in _restAccounts)
                            {
                                if (IsRunning)
                                {

                                    Random r = new Random();
                                    int rInt = r.Next(warmerModel.delayFrom * 1000, warmerModel.delayTo * 1000);
                                    r = new Random();
                                    int randomMessageIndex = r.Next(0, MessagesList.Count() - 1);
                                    string randomMessage = MessagesList[randomMessageIndex];
                                    try
                                    {
                                        waMultiInstance.tabControl1.Invoke((MethodInvoker)delegate
                                        {
                                            waMultiInstance.tabControl1.SelectedTab = fromAccount.tabPage;
                                        });
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                    await WPPHelper.SendMessageFull(fromAccount.webview, toAccount.Number, rInt, randomMessage);


                                    dataGridView1.Invoke((MethodInvoker)delegate
                                    {
                                        DateTime dt = DateTime.Now;
                                        dataGridView1.Rows.Add(new object[]{
                                    fromAccount.Name,
                                    toAccount.Name,
                                    dt.Hour + ":" + dt.Minute + ":" + dt.Second,
                                    randomMessage
                                    });

                                        dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
                                    });




                                }
                                {
                                    Random r = new Random();
                                    int rInt = r.Next(warmerModel.delayFrom * 1000, warmerModel.delayTo * 1000);
                                    r = new Random();
                                    Thread.Sleep(rInt);
                                }
                                if (IsRunning)
                                {
                                    Random r = new Random();
                                    int rInt = r.Next(warmerModel.delayFrom * 1000, warmerModel.delayTo * 1000);
                                    r = new Random();
                                    int randomMessageIndex = r.Next(0, MessagesList.Count() - 1);
                                    string randomMessage = MessagesList[randomMessageIndex];
                                    try
                                    {
                                        waMultiInstance.tabControl1.Invoke((MethodInvoker)delegate
                                        {
                                            waMultiInstance.tabControl1.SelectedTab = toAccount.tabPage;
                                        });
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                    await WPPHelper.SendMessageFull(toAccount.webview, fromAccount.Number, rInt, randomMessage);


                                    dataGridView1.Invoke((MethodInvoker)delegate
                                    {
                                        DateTime dt = DateTime.Now;
                                        dataGridView1.Rows.Add(new object[]{
                                    toAccount.Name,
                                    fromAccount.Name,
                                    dt.Hour + ":" + dt.Minute + ":" + dt.Second,
                                    randomMessage
                                    });

                                        dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
                                    });
                                }
                                {
                                    Random r = new Random();
                                    int rInt = r.Next(warmerModel.delayFrom * 1000, warmerModel.delayTo * 1000);
                                    r = new Random();
                                    Thread.Sleep(rInt);
                                }
                            }
                            #endregion

                        }
                        
                    }
                }
            }

            return result;
        }

      

        private void materialButton2_Click(object sender, EventArgs e)
        {
            IsRunning = false;
            ChangeCampStatus(CampaignStatusEnum.Stopped);
        }

     

        private void RunWarmer_Load(object sender, EventArgs e)
        {
            initLanguages();
            if (Utils.waSenderBrowser != null)
            {
                Utils.waSenderBrowser.Close();
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
                WPPHelper.CheckExecutingAssembly();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }


        private void initLanguages()
        {
            this.Text = Strings.Run + " " + Strings.Warmer;
            materialButton1.Text = Strings.Start;
            materialButton2.Text = Strings.Stop;
            btnInitWA.Text = Strings.ClicktoInitiate;
            label5.Text = Strings.Status;

            dataGridView1.Columns[0].HeaderText = Strings.From;
            dataGridView1.Columns[1].HeaderText = Strings.to;
            dataGridView1.Columns[2].HeaderText = Strings.Time;
            dataGridView1.Columns[3].HeaderText = Strings.Message;
            lblInitStatus.Text = Strings.NotInitialised;
            lblRunStatus.Text = Strings.NotStarted;
            label7.Text = Strings.Status;
        }


        private void initWABrowser()
        {
            ChangeInitStatus(InitStatusEnum.Initialising);
            retryAttempt = 0;
            if (Utils.waSenderBrowser != null)
            {
                waMultiInstance = Utils.waSenderBrowser;
            }
            else
            {
                
            }

            List<ConnectedAccountModel> selectedAccounts = new List<ConnectedAccountModel>();
            ConnectedAccountModel selectedAccount;
            foreach (var item in warmerModel.SelectedAccountNames)
            {
                selectedAccount = new ConnectedAccountModel();
                selectedAccount.sessionName = item.Name;
                selectedAccount.ID = item.ID;
                selectedAccounts.Add(selectedAccount);
            }

            waMultiInstance = new WaSenderBrowser(selectedAccounts);
            Utils.waSenderBrowser = waMultiInstance;
            waMultiInstance.Show();
            checkQRScanDoneBrowser();
        }

        private void checkQRScanDoneBrowser()
        {
            Thread.Sleep(1000);
            timerInitChecker = new System.Windows.Forms.Timer();
            timerInitChecker.Interval = 1000;
            timerInitChecker.Tick += timerInitChecker_Tick;
            timerInitChecker.Start();
        }

        public async void timerInitChecker_Tick(object sender, EventArgs e)
        {
            try
            {
                bool IsAllinitiated = true;
                foreach (TabPage tab in waMultiInstance.tabControl1.TabPages)
                {
                    WebView2 vw = (WebView2)tab.Controls.Find("webView21", true)[0];
                    string name = tab.Text;
                    bool isInitiated = await WPPHelper.isWaInited(vw);
                    IsAllinitiated = isInitiated;
                }
                if (IsAllinitiated)
                {
                    ChangeInitStatus(InitStatusEnum.Initialised);
                    timerInitChecker.Stop();
                }
            }
            catch (Exception ex)
            {
                if (retryAttempt == 5)
                {
                    retryAttempt = 0;
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

        private void ChangeInitStatus(InitStatusEnum _initStatus)
        {
            this.initStatusEnum = _initStatus;
            AutomationCommon.ChangeInitStatus(_initStatus, lblInitStatus);
        }

        private void btnInitWA_Click(object sender, EventArgs e)
        {
            initWABrowser();
        }

        private void RunWarmer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Utils.waSenderBrowser != null)
            {
                Utils.waSenderBrowser.Close();
            }
        }
    }
}
