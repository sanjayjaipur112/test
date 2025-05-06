    using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WASender.Model;
using WASender.Models;

namespace WASender
{
    public class Strings
    {
        private static string languageJson = "";

        public readonly static string AppName = Config.AppName; // Don't change here , Moved under 'ProjectCommon' project in 'Config.cs' file 
        public readonly static Icon AppIcon = WASender.Properties.Resources.LogoWA;
        public static string selectedLanguage = getLanguage(LanguagesEnum.English);
        public static bool Allow_Users_to_Change_Language = true;
        public static bool EnableButtons= true;


        /// <summary>
        /// This Section Allow you to Disable any tool from Tools Section
        public readonly static bool Get_Group_Member                    = true; // Set true to false if you dont want this tool to be appear in Tools Section
        public readonly static bool Grab_WhatsApp_Group_Links_from_web  = true; // Set true to false if you dont want this tool to be appear in Tools Section
        public readonly static bool WhatsApp_Auto_Responder_Bot         = true; // Set true to false if you dont want this tool to be appear in Tools Section
        public readonly static bool Contact_List_Grabber                = true; // Set true to false if you dont want this tool to be appear in Tools Section
        public readonly static bool Google_Map_Data_Extractor           = true; // Set true to false if you dont want this tool to be appear in Tools Section
        public readonly static bool Auto_Group_Joiner                   = true; // Set true to false if you dont want this tool to be appear in Tools Section
        public readonly static bool WhatsApp_Number_Filter              = true; // Set true to false if you dont want this tool to be appear in Tools Section
        public readonly static bool Grab_Active_Group_Members           = true; // Set true to false if you dont want this tool to be appear in Tools Section
        public readonly static bool Grab_Chat_List                      = true; // Set true to false if you dont want this tool to be appear in Tools Section
        public readonly static bool Bulk_Add_Group_Members              = true; // Set true to false if you dont want this tool to be appear in Tools Section
        public readonly static bool Group_Finder                        = true; // Set true to false if you dont want this tool to be appear in Tools Section
        public readonly static bool Bulk_Group_Generator                = true; // Set true to false if you dont want this tool to be appear in Tools Section
        public readonly static bool Google_Contact_CSV_Generator        = true; // Set true to false if you dont want this tool to be appear in Tools Section
        public readonly static bool Website_Email_Mobile_Extractor      = true; // Set true to false if you dont want this tool to be appear in Tools Section
        public readonly static bool WhatsApp_Warmer                     = true; // Set true to false if you dont want this tool to be appear in Tools Section
        public readonly static bool Poll_Report                         = true; // Set true to false if you dont want this tool to be appear in Tools Section
        public readonly static bool Social_Media_Data_Extractor         = true; // Set true to false if you dont want this tool to be appear in Tools Section
        public readonly static bool All_Schedules                       = true; // Set true to false if you dont want this tool to be appear in Tools Section
        /// </summary>


        

        public readonly static string Column1 = GetValue("Column1");
        public readonly static string Number = GetValue("Number");
        public readonly static string Column2 = GetValue("Column2");
        public readonly static string DownloadSample = GetValue("DownloadSample");
        public readonly static string UploadSampleExcel = GetValue("UploadSampleExcel");
        public readonly static string ContectSender = GetValue("ContectSender");
        public readonly static string GroupSender = GetValue("GroupSender");
        public readonly static string Tools = GetValue("Tools");
        public readonly static string Filedownloadedsuccessfully = GetValue("Filedownloadedsuccessfully");
        public readonly static string WABulkSender = GetValue("WABulkSender");
        public readonly static string SelectExcel = GetValue("SelectExcel");
        public readonly static string PleaseselectExcelfilesformatonly = GetValue("PleaseselectExcelfilesformatonly");
        public readonly static string AddNew = GetValue("AddNew");
        public readonly static string KeyMarkers = GetValue("KeyMarkers");
        public readonly static string AddKeyMarker = GetValue("AddKeyMarker");
        public readonly static string SaveNClose = GetValue("SaveNClose");
        public readonly static string KeyMarkerFormatinIncorrect = GetValue("KeyMarkerFormatinIncorrect");
        public readonly static string OK = GetValue("OK");
        public readonly static string SelectedKeyMarkeralreadyExist = GetValue("SelectedKeyMarkeralreadyExist");
        public readonly static string Delete = GetValue("Delete");
        public readonly static string WrongKey = GetValue("WrongKey");
        public readonly static string Target = GetValue("Target");
        public readonly static string Messages = GetValue("Messages");
        public readonly static string MessageOne = GetValue("MessageOne");
        public readonly static string MessageTwo = GetValue("MessageTwo");
        public readonly static string MessageThree = GetValue("MessageThree");
        public readonly static string MessageFour = GetValue("MessageFour");
        public readonly static string MessageFive = GetValue("MessageFive");
        public readonly static string Attachments = GetValue("Attachments");
        public readonly static string Addfile = GetValue("Addfile");
        public readonly static string Yourfirstmessage = GetValue("Yourfirstmessage");
        public readonly static string YourSecondmessage = GetValue("YourSecondmessage");
        public readonly static string YourThirdmessage = GetValue("YourThirdmessage");
        public readonly static string YourFourthmessage = GetValue("YourFourthmessage");
        public readonly static string YourFifthmessage = GetValue("YourFifthmessage");
        public readonly static string DelaySettings = GetValue("DelaySettings");
        public readonly static string Wait = GetValue("Wait");
        public readonly static string secondsafterevery = GetValue("secondsafterevery");
        public readonly static string to = GetValue("to");
        public readonly static string Clear = GetValue("Clear");
        public readonly static string StartCampaign = GetValue("StartCampaign");
        public readonly static string secondsbeforeeverymessage = GetValue("secondsbeforeeverymessage");
        public readonly static string AddKeyMarkers = GetValue("AddKeyMarkers");
        public readonly static string RandomNumber = GetValue("RandomNumber");
        public readonly static string PleaseEnterCampaignName = GetValue("PleaseEnterCampaignName");
        public readonly static string CampaignName = GetValue("CampaignName");
        public readonly static string UntitledGroupCampaign = GetValue("UntitledGroupCampaign");
        public readonly static string UntitledCampaign = GetValue("UntitledCampaign");
        public readonly static string RowNo = GetValue("RowNo");
        public readonly static string Errors = GetValue("Errors");
        public readonly static string WhatsAppBot = GetValue("WhatsAppBot");
        public readonly static string Rules = GetValue("Rules");
        public readonly static string AddRule = GetValue("AddRule");
        public readonly static string Log = GetValue("Log");
        public readonly static string Status = GetValue("Status");
        public readonly static string Start = GetValue("Start");
        public readonly static string Stop = GetValue("Stop");
        public readonly static string IsActive = GetValue("IsActive");
        public readonly static string UserInput = GetValue("UserInput");
        public readonly static string Type = GetValue("Type");
        public readonly static string Match = GetValue("Match");
        public readonly static string ReplySend = GetValue("ReplySend");
        public readonly static string NotMatch = GetValue("NotMatch");
        public readonly static string Fallback = GetValue("Fallback");
        public readonly static string PleaseaddRules = GetValue("PleaseaddRules");
        public readonly static string Run = GetValue("Run");
        public readonly static string InitiateWhatsAppScaneQRCodefromyourrmobile = GetValue("InitiateWhatsAppScaneQRCodefromyourrmobile");
        public readonly static string ClicktoInitiate = GetValue("ClicktoInitiate");
        public readonly static string ChatName = GetValue("ChatName");
        public readonly static string ImportentNotes = GetValue("ImportentNotes");
        public readonly static string Keepapplicationopenwhilesendingmessagesanduntilallmessagesaresentfromyourmobile = GetValue("Keepapplicationopenwhilesendingmessagesanduntilallmessagesaresentfromyourmobile");
        public readonly static string ClearWhatsAppchathistoryafter5001000150020000messagesasperyourphoneconfiguration = GetValue("ClearWhatsAppchathistoryafter5001000150020000messagesasperyourphoneconfiguration");
        public readonly static string WaSendertendstosubmitmessagestoyourphoneisnotresponsiblefordeliveryofthemessage = GetValue("WaSendertendstosubmitmessagestoyourphoneisnotresponsiblefordeliveryofthemessage");
        public readonly static string Completed = GetValue("Completed");
        public readonly static string PleasefollowStepNo1FirstInitialiseWhatsapp = GetValue("PleasefollowStepNo1FirstInitialiseWhatsapp");
        public readonly static string Processisalreadyrunning = GetValue("Processisalreadyrunning");
        public readonly static string RunGroup = GetValue("RunGroup");
        public readonly static string KeyMarker = GetValue("KeyMarker");
        public readonly static string Select = GetValue("Select");
        public readonly static string GroupsJoiner = GetValue("GroupsJoiner");
        public readonly static string GroupJoin = GetValue("GroupJoin");
        public readonly static string secondsbeforeeveryGroupJoin = GetValue("secondsbeforeeveryGroupJoin");
        public readonly static string StartJoining = GetValue("StartJoining");
        public readonly static string GroupLink = GetValue("GroupLink");
        public readonly static string PleaseFollowStepnoThree = GetValue("PleaseFollowStepnoThree");
        public readonly static string GrabGroupLinks = GetValue("GrabGroupLinks");
        public readonly static string OpenBrowser = GetValue("OpenBrowser");
        public readonly static string Clickbellowbuttontoopenbrowser = GetValue("Clickbellowbuttontoopenbrowser");
        public readonly static string Navigatetoanywebsitewherelistedgrouplinkstheclickbellowbellowbuton = GetValue("Navigatetoanywebsitewherelistedgrouplinkstheclickbellowbellowbuton");
        public readonly static string StartGrabbing = GetValue("StartGrabbing");
        public readonly static string NoGroupLinkfoundincurrentPage = GetValue("NoGroupLinkfoundincurrentPage");
        public readonly static string GrabChatList = GetValue("GrabChatList");
        public readonly static string InitiateWhatsAppScaneQRCodefromyourmobile = GetValue("InitiateWhatsAppScaneQRCodefromyourmobile");
        public readonly static string GetGroupMember = GetValue("GetGroupMember");
        public readonly static string OpenanyGroupchatClickbuttonbellow = GetValue("OpenanyGroupchatClickbuttonbellow");
        public readonly static string PleaseGotoanygroupchat = GetValue("PleaseGotoanygroupchat");
        public readonly static string fallback = GetValue("fallback");
        public readonly static string Usefallback = GetValue("Usefallback");
        public readonly static string If = GetValue("If");
        public readonly static string UserSend = GetValue("UserSend");
        public readonly static string Condition = GetValue("Condition");
        public readonly static string Which = GetValue("Which");
        public readonly static string ThenReplywith = GetValue("ThenReplywith");
        public readonly static string AddMessage = GetValue("AddMessage");
        public readonly static string Message = GetValue("Message");
        public readonly static string Cancel = GetValue("Cancel");
        public readonly static string Save = GetValue("Save");
        public readonly static string Confirm = GetValue("Confirm");
        public readonly static string AreyousuretodeletethisRule = GetValue("AreyousuretodeletethisRule");
        public readonly static string ReplyMessage = GetValue("ReplyMessage");
        public readonly static string TypeYourMessagehere = GetValue("TypeYourMessagehere");
        public readonly static string Files = GetValue("Files");
        public readonly static string Add = GetValue("Add");
        public readonly static string GroupNames = GetValue("GroupNames");
        public readonly static string PleaseCheckMobileNumberyouhaveadded = GetValue("PleaseCheckMobileNumberyouhaveadded");
        public readonly static string ShouldNotbeempty = GetValue("ShouldNotbeempty");
        public readonly static string ContactNumberShouldNotbeEmpty = GetValue("ContactNumberShouldNotbeEmpty");
        public readonly static string MessageShouldNotbeEmpty = GetValue("MessageShouldNotbeEmpty");
        public readonly static string delayAfterMessagesShouldNotbeEmpty = GetValue("delayAfterMessagesShouldNotbeEmpty");
        public readonly static string delayAfterMessagesFromShouldNotbeEmpty = GetValue("delayAfterMessagesFromShouldNotbeEmpty");
        public readonly static string delayAfterMessagesTOShouldNotbeEmpty = GetValue("delayAfterMessagesTOShouldNotbeEmpty");
        public readonly static string delayAfterEveryMessageFromShouldNotbeEmpty = GetValue("delayAfterEveryMessageFromShouldNotbeEmpty");
        public readonly static string delayAfterEveryMessageToShouldNotbeEmpty = GetValue("delayAfterEveryMessageToShouldNotbeEmpty");
        public readonly static string ShouldGraterthenoero = GetValue("ShouldGraterthenoero");
        public readonly static string delayAfterMessages_ShouldGraterthenoero = GetValue("delayAfterMessages_ShouldGraterthenoero");
        public readonly static string delayAfterMessagesFrom_ShouldGraterthenoero = GetValue("delayAfterMessagesFrom_ShouldGraterthenoero");
        public readonly static string delayAfterMessagesTo_ShouldGraterthenoero = GetValue("delayAfterMessagesTo_ShouldGraterthenoero");
        public readonly static string delayAfterEveryMessageFrom_ShouldGraterthenoero = GetValue("delayAfterEveryMessageFrom_ShouldGraterthenoero");
        public readonly static string delayAfterEveryMessageTo_ShouldGraterthenoero = GetValue("delayAfterEveryMessageTo_ShouldGraterthenoero");
        public readonly static string thetoamountismustbegraterthenstartingamount = GetValue("thetoamountismustbegraterthenstartingamount");
        public readonly static string UsermessageMustEmptyincaseoffallback = GetValue("UsermessageMustEmptyincaseoffallback");
        public readonly static string PleaseAddatleastoneMessage = GetValue("PleaseAddatleastoneMessage");
        public readonly static string PleaseEnterAmessage = GetValue("PleaseEnterAmessage");
        public readonly static string PleaseaddatleastoneGroupinlist = GetValue("PleaseaddatleastoneGroupinlist");
        public readonly static string PleaseaddatleastoneContactinlist = GetValue("PleaseaddatleastoneContactinlist");
        public readonly static string ClickbellowButton = GetValue("ClickbellowButton");
        public readonly static string ScanQRCode = GetValue("ScanQRCode");
        public readonly static string WaitfortheExcel = GetValue("WaitfortheExcel");
        public readonly static string Now = GetValue("Now");
        public readonly static string GrabNow = GetValue("GrabNow");
        public readonly static string GrabGroupMembers = GetValue("GrabGroupMembers");
        public readonly static string ClickbellowButtonScanQRCode = GetValue("ClickbellowButtonScanQRCode");
        public readonly static string OpenAnyGroup = GetValue("OpenAnyGroup");
        public readonly static string GrabWhatsAppGroupLinksfromwebpage = GetValue("GrabWhatsAppGroupLinksfromwebpage");
        public readonly static string OpenAnywebpagewheregivengrouplinks = GetValue("OpenAnywebpagewheregivengrouplinks");
        public readonly static string ThenClickonSTARTButton = GetValue("ThenClickonSTARTButton");
        public readonly static string AutoGroupJoiner = GetValue("AutoGroupJoiner");
        public readonly static string AddUploadGroupLinks = GetValue("AddUploadGroupLinks");
        public readonly static string ScanWAQRCode = GetValue("ScanWAQRCode");
        public readonly static string StartNow = GetValue("StartNow");
        public readonly static string WhatsAppAutoResponderBot = GetValue("WhatsAppAutoResponderBot");
        public readonly static string SetRules = GetValue("SetRules");
        public readonly static string AddReplyMessages = GetValue("AddReplyMessages");
        public readonly static string GeneralSettings = GetValue("GeneralSettings");
        public readonly static string ChromeProfilePath = GetValue("ChromeProfilePath");
        public readonly static string InputPathisnotCorrectfolderpath = GetValue("InputPathisnotCorrectfolderpath");
        public readonly static string SettingsSavedSuccessfully = GetValue("SettingsSavedSuccessfully");
        public readonly static string MessageSendingType = GetValue("MessageSendingType");
        public readonly static string CopyPaste = GetValue("CopyPaste");
        public readonly static string Typeamessage = GetValue("Typeamessage");
        public readonly static string Activate = GetValue("Activate");
        public readonly static string ActivateAppName = GetValue("ActivateAppName");
        public readonly static string YourActivationCodeis = GetValue("YourActivationCodeis");
        public readonly static string ProvideYourActivationKeyHere = GetValue("ProvideYourActivationKeyHere");
        public readonly static string ActivateNow = GetValue("ActivateNow");
        public readonly static string Exit = GetValue("Exit");
        public readonly static string ActivationSuccessfull = GetValue("ActivationSuccessfull");
        public readonly static string InvalidActivationKey = GetValue("InvalidActivationKey");
        public readonly static string LanguageIsSet = GetValue("LanguageIsSet");
        public readonly static string Name = GetValue("Name");
        public readonly static string WhatsAppNumberFilter = GetValue("WhatsAppNumberFilter");
        public readonly static string AddUploadNumbers = GetValue("AddUploadNumbers");
        public readonly static string NumberCheck = GetValue("NumberCheck");
        public readonly static string secondsbeforeeveryNumberCheck = GetValue("secondsbeforeeveryNumberCheck");
        public readonly static string StartChecking = GetValue("StartChecking");
        public readonly static string ContactListGrabber = GetValue("ContactListGrabber");
        public readonly static string HitGrabNowButton = GetValue("HitGrabNowButton");
        public readonly static string GrabActiveGroupMembers = GetValue("GrabActiveGroupMembers");
        public readonly static string TotalFound = GetValue("TotalFound");
        public readonly static string Export = GetValue("Export");
        public readonly static string Nothingtoexport = GetValue("Nothingtoexport");
        public readonly static string GoogleMapDataEExtractor = GetValue("GoogleMapDataEExtractor");
        public readonly static string Usethatwindowtosearchforbusinessesandwhensearchresultsareshown = GetValue("Usethatwindowtosearchforbusinessesandwhensearchresultsareshown");
        public readonly static string PleaseSearchsomething = GetValue("PleaseSearchsomething");
        public readonly static string ImportInWaSender = GetValue("ImportInWaSender");
        public readonly static string EnterCountryCode = GetValue("EnterCountryCode");
        public readonly static string SearchYourQUeryonGMap = GetValue("SearchYourQUeryonGMap");
        public readonly static string HitStart = GetValue("HitStart");
        public readonly static string Count = GetValue("Count");
        public readonly static string ReviewCount = GetValue("ReviewCount");
        public readonly static string Catagory = GetValue("Catagory");
        public readonly static string MobileNumber = GetValue("MobileNumber");
        public readonly static string Address = GetValue("Address");
        public readonly static string Website = GetValue("Website");
        public readonly static string PlusCode = GetValue("PlusCode");
        public readonly static string AddCountryCode = GetValue("AddCountryCode");
        public readonly static string One = GetValue("One");
        public readonly static string Two = GetValue("Two");
        public readonly static string Three = GetValue("Three");
        public readonly static string AddButtons = GetValue("AddButtons");
        public readonly static string Buttons = GetValue("Buttons");
        public readonly static string ButtonMessage = GetValue("ButtonMessage");
        public readonly static string ButtonText = GetValue("ButtonText");
        public readonly static string ButtonType = GetValue("ButtonType");
        public readonly static string NormalButton = GetValue("NormalButton");
        public readonly static string Link = GetValue("Link");
        public readonly static string PhoneNumber = GetValue("PhoneNumber");
        public readonly static string EnterURL = GetValue("EnterURL");
        public readonly static string EnterPhoneNumber = GetValue("EnterPhoneNumber");
        public readonly static string AddButton = GetValue("AddButton");
        public readonly static string GrabAllSavedContacts = GetValue("GrabAllSavedContacts");
        public readonly static string GrabAllGroups = GetValue("GrabAllGroups");
        public readonly static string ChooseGroup = GetValue("GrabAllGroups");
        public readonly static string PleaseSelectGroup = GetValue("PleaseSelectGroup");
        public readonly static string CopyPasteNumber = GetValue("CopyPasteNumber");
        public readonly static string Import = GetValue("Import");
        public readonly static string YourSearchterm = GetValue("YourSearchterm");
        public readonly static string Softwarecompaniesintexas = "Software companies in texas";
        public readonly static string AddCaption = GetValue("AddCaption");
        public readonly static string Caption = GetValue("Caption");
        public readonly static string _File = GetValue("File");
        public readonly static string BulkAddGroupMembers = GetValue("BulkAddGroupMembers");
        public readonly static string NumberAdd = GetValue("NumberAdd");
        public readonly static string secondsbeforeeveryNumberAdd = GetValue("secondsbeforeeveryNumberAdd");
        public readonly static string StartAdding = GetValue("StartAdding");
        public readonly static string ClickButtonbellow = GetValue("ClickButtonbellow");
        public readonly static string UploadNumbersExcel = GetValue("UploadNumbersExcel");
        public readonly static string SelectGroupandGo = GetValue("SelectGroupandGo");
        public readonly static string GroupFinder = GetValue("GroupFinder");
        public readonly static string GroupSubject = GetValue("GroupSubject");
        public readonly static string Search = GetValue("Search");
        public readonly static string ImportInGroupJoiner = GetValue("ImportInGroupJoiner");
        public readonly static string EnterYourSubject = GetValue("EnterYourSubject");
        public readonly static string StartFinding = GetValue("StartFinding");
        public readonly static string GroupName = GetValue("GroupName");
        public readonly static string Pause = GetValue("Pause");
        public readonly static string RatingCount = GetValue("RatingCount");
        public readonly static string AttachWithMainMessage = GetValue("AttachWithMainMessage");
        public readonly static string clossinghour = GetValue("clossinghour");
        public readonly static string latitude = GetValue("latitude");
        public readonly static string longitude = GetValue("longitude");
        public readonly static string instagramprofile = GetValue("instagramprofile");
        public readonly static string facebookprofile = GetValue("facebookprofile");
        public readonly static string linkedinprofile = GetValue("linkedinprofile");
        public readonly static string twitterprofile = GetValue("twitterprofile");
        public readonly static string EmailId = GetValue("EmailId");
        public readonly static string GenerateNumbers = GetValue("GenerateNumbers");
        public readonly static string Increment = GetValue("Increment");
        public readonly static string Quentity = GetValue("Quentity");
        public readonly static string Generate = GetValue("Generate");
        public readonly static string CountryCode = GetValue("CountryCode");
        public readonly static string BulkGroupGenerator = GetValue("BulkGroupGenerator");
        public readonly static string GroupNameSettings = GetValue("GroupNameSettings");
        public readonly static string GroupNamePrefix = GetValue("GroupNamePrefix");
        public readonly static string GroupNameSuffix = GetValue("GroupNameSuffix");
        public readonly static string DefaultNumberAdd = GetValue("DefaultNumberAdd");
        public readonly static string GenerateTotalGroups = GetValue("GenerateTotalGroups");
        public readonly static string Validate = GetValue("Validate");
        public readonly static string GroupCreate = GetValue("GroupCreate");
        public readonly static string secondsbeforeeveryGroupCreate = GetValue("secondsbeforeeveryGroupCreate");
        public readonly static string IsNotValid = GetValue("IsNotValid");
        public readonly static string DontCheckNumberStatusBeforeSendingMessage = GetValue("DontCheckNumberStatusBeforeSendingMessage");
        public readonly static string ProvideInputs = GetValue("ProvideInputs");
        public readonly static string StartGenerating = GetValue("StartGenerating");
        public readonly static string ChromeEXEpath = GetValue("ChromeEXEpath");
        public readonly static string CheckforInternalUpdate = GetValue("CheckforInternalUpdate");
        public readonly static string UpdateChromeDriver = GetValue("UpdateChromeDriver");
        public readonly static string clearsessions = GetValue("clearsessions");
        public readonly static string ClearProfileCache = GetValue("ClearProfileCache");
        public readonly static string ClearBOTCache = GetValue("ClearBOTCache");
        public readonly static string About = GetValue("About");
        public readonly static string UpdateCHromeDriverMethodOne = GetValue("UpdateCHromeDriverMethodOne");
        public readonly static string UpdateCHromeDriverMethodTwo = GetValue("UpdateCHromeDriverMethodTwo");
        public readonly static string SendGroupInvitationcodeiffail = GetValue("SendGroupInvitationcodeiffail");
        public readonly static string GrabImages = GetValue("GrabImages");
        public readonly static string ImagesFolder = GetValue("ImagesFolder");
        public readonly static string GrabEmailId = GetValue("GrabEmailId");
        public readonly static string GoogleContactsCSVGenerator = GetValue("GoogleContactsCSVGenerator");
        public readonly static string GenrateRandomName = GetValue("GenrateRandomName");
        public readonly static string NamePrefix = GetValue("NamePrefix");
        public readonly static string NameSufix = GetValue("NameSufix");
        public readonly static string GenerateNow = GetValue("GenerateNow");
        public readonly static string ExportasGoogleContactCSV = GetValue("ExportasGoogleContactCSV");
        public readonly static string Loding = GetValue("Loding");
        public readonly static string DeActivateLicence = GetValue("DeActivateLicence");
        public readonly static string AreyouSure = GetValue("AreyouSure");
        public readonly static string Yes = GetValue("Yes");
        public readonly static string ManageAccounts = GetValue("ManageAccounts");
        public readonly static string AccountName = GetValue("AccountName");
        public readonly static string AreYouSuretodeletethisAccount = GetValue("AreYouSuretodeletethisAccount");
        public readonly static string AccountDeleted = GetValue("AccountDeleted");
        public readonly static string AddNewAccount = GetValue("AddNewAccount");
        public readonly static string SameNameAlreadyExists = GetValue("SameNameAlreadyExists");
        public readonly static string AccountAddedSuccessfully = GetValue("AccountAddedSuccessfully");
        public readonly static string CantDeleteDefaultAccount = GetValue("CantDeleteDefaultAccount");
        public readonly static string SetasDefaultAccount = GetValue("SetasDefaultAccount");
        public readonly static string Accounts = GetValue("Accounts");
        public readonly static string Load = GetValue("Load");
        public readonly static string Primary = GetValue("Primary");
        public readonly static string Launch = GetValue("Launch");
        public readonly static string SwipeAccountaftermessages = GetValue("SwipeAccountaftermessages");
        public readonly static string YouhaventaddedanyaccountyetToaddnewaccountpleaseuseACCOUNTSbutton = GetValue("YouhaventaddedanyaccountyetToaddnewaccountpleaseuseACCOUNTSbutton");
        public readonly static string Pleaseselectatleastoneaccount = GetValue("Pleaseselectatleastoneaccount");
        public readonly static string InvalidPurchaseCode = GetValue("InvalidPurchaseCode");
        public readonly static string AddPoll = GetValue("AddPoll");
        public readonly static string PollName = GetValue("PollName");
        public readonly static string Options = GetValue("Options");
        public readonly static string Polls = GetValue("Polls");
        public readonly static string selectableCount = GetValue("selectableCount");
        public readonly static string DuplicateValue = GetValue("DuplicateValue");
        public readonly static string SafeModeData1 = GetValue("SafeModeData1");
        public readonly static string SafeModeData2 = GetValue("SafeModeData2");
        public readonly static string SafeModeData3 = GetValue("SafeModeData3");
        public readonly static string SafeMode = GetValue("SafeMode");
        public readonly static string SendingMode = GetValue("SendingMode");
        public readonly static string UnSafeMode = GetValue("UnSafeMode");
        public readonly static string Loading = GetValue("Loading");
        public readonly static string PleaseWait = GetValue("PleaseWait");
        public readonly static string BlockList = GetValue("BlockList");
        public readonly static string WhatsAppWarmer = GetValue("WhatsAppWarmer");
        public readonly static string SelectAccounts = GetValue("SelectAccounts");
        public readonly static string Prepare = GetValue("Prepare");
        public readonly static string Conversessions = GetValue("Conversessions");
        public readonly static string Refresh = GetValue("Refresh");
        public readonly static string Selecttextfile = GetValue("Selecttextfile");
        public readonly static string Delaybetween = GetValue("Delaybetween");
        public readonly static string and = GetValue("and");
        public readonly static string Seconds = GetValue("Seconds");
        public readonly static string Finish = GetValue("Finish");
        public readonly static string Next = GetValue("Next");
        public readonly static string Ready = GetValue("Ready");
        public readonly static string isNotReadytouse = GetValue("isNotReadytouse");
        public readonly static string SelectedAccount = GetValue("SelectedAccount");
        public readonly static string PleaseSelectTWOormoreaccounts = GetValue("PleaseSelectTWOormoreaccounts");
        public readonly static string PleaseselctyourInputfile = GetValue("PleaseselctyourInputfile");
        public readonly static string PleaseenteryourDelaysettings = GetValue("PleaseenteryourDelaysettings");
        public readonly static string Returnstoapreviouspage = GetValue("Returnstoapreviouspage");
        public readonly static string Running = GetValue("Running");
        public readonly static string Stopped = GetValue("Stopped");
        public readonly static string Warmer = GetValue("Warmer");
        public readonly static string NotStarted = GetValue("NotStarted");
        public readonly static string From = GetValue("From");
        public readonly static string Time = GetValue("Time");
        public readonly static string WebsiteEMailMobileExtractor = GetValue("WebsiteEMailMobileExtractor");
        public readonly static string WebSiteUrls = GetValue("WebSiteUrls");
        public readonly static string DeepCheck = GetValue("DeepCheck");
        public readonly static string ExportNumbersOnly = GetValue("ExportNumbersOnly");
        public readonly static string ImportAllwebsitesinWebsiteEmailMobileExtractor = GetValue("ImportAllwebsitesinWebsiteEmailMobileExtractor");
        public readonly static string Nowbesitefound = GetValue("Nowbesitefound");
        public readonly static string Error = GetValue("Error");
        public readonly static string ImportNumbers = GetValue("ImportNumbers");
        public readonly static string RemoveDuplicateNumbers = GetValue("RemoveDuplicateNumbers");
        public readonly static string depth = GetValue("depth");
        public readonly static string SingleSelect = GetValue("SingleSelect");
        public readonly static string MultiSelect = GetValue("MultiSelect");
        public readonly static string MultiMessagingMode = GetValue("MultiMessagingMode");
        public readonly static string SendAllMessagestoeachnumber = GetValue("SendAllMessagestoeachnumber");
        public readonly static string RotateMessages = GetValue("RotateMessages");
        public readonly static string Randomly = GetValue("Randomly");
        public readonly static string Thisfieldstasdesabledifyouenteredonlyonemessage = GetValue("Thisfieldstasdesabledifyouenteredonlyonemessage");
        public readonly static string ProductIsNotActivated = GetValue("ProductIsNotActivated");
        public readonly static string DiffrentPurchaseCode = GetValue("DiffrentPurchaseCode");
        public readonly static string Searches = GetValue("Searches");
        public readonly static string UpdateEDGEDriver = GetValue("UpdateEDGEDriver");
        public readonly static string Version = GetValue("Version");
        public readonly static string LicenceInformation = GetValue("LicenceInformation");
        public readonly static string Activated = GetValue("Activated");
        public readonly static string LicenceExpiresOn = GetValue("LicenceExpiresOn");
        public readonly static string Note = GetValue("Note");
        public readonly static string MarkAllchatasread = GetValue("MarkAllchatasread");
        public readonly static string GrabAll = GetValue("GrabAll");
        public readonly static string GrabByLabel = GetValue("GrabByLabel");
        public readonly static string SelectLebel = GetValue("SelectLebel");
        public readonly static string LebelName = GetValue("LebelName");
        public readonly static string ChatCount = GetValue("ChatCount");
        public readonly static string PleaseSelectAnyOneLebel = GetValue("PleaseSelectAnyOneLebel");
        public readonly static string YouDonthaveanyLabeltoselect = GetValue("YouDonthaveanyLabeltoselect");
        public readonly static string GetPollResults = GetValue("GetPollResults");
        public readonly static string Result = GetValue("Result");
        public readonly static string PollOptions = GetValue("PollOptions");
        public readonly static string NonReactedNumbers = GetValue("NonReactedNumbers");
        public readonly static string Option = GetValue("Option");
        public readonly static string VotedNumbers = GetValue("VotedNumbers");
        public readonly static string BusinessAccount = GetValue("BusinessAccount");
        public readonly static string BusinessProfileExtractor = GetValue("BusinessProfileExtractor");
        public readonly static string TotalBusinessProfilestoExtract = GetValue("TotalBusinessProfilestoExtract");
        public readonly static string BusinessInstruction_1 = GetValue("BusinessInstruction_1");
        public readonly static string BusinessName = GetValue("BusinessName");
        public readonly static string Description = GetValue("Description");
        public readonly static string PrimaryInstruction = GetValue("PrimaryInstruction");
        public readonly static string SocialMediaDataExtractor = GetValue("SocialMediaDataExtractor");
        public readonly static string KeyWords = GetValue("KeyWords");
        public readonly static string Mobile = GetValue("Mobile");
        public readonly static string LocationOptional = GetValue("LocationOptional");
        public readonly static string SocialMedicaWebsite = GetValue("SocialMedicaWebsite");
        public readonly static string DelayInSecondsAfterEachPage = GetValue("DelayInSecondsAfterEachPage");
        public readonly static string Title = GetValue("Title");
        public readonly static string WhatareyouLookingFor = GetValue("WhatareyouLookingFor");
        public readonly static string ieWebDevelopersWebDesignersect = GetValue("ieWebDevelopersWebDesignersect");
        public readonly static string Youcanaddmultiplekeywords = GetValue("Youcanaddmultiplekeywords");
        public readonly static string KeepeachKeywordinonline = GetValue("KeepeachKeywordinonline");
        public readonly static string TograbMobileNumbers = GetValue("TograbMobileNumbers");
        public readonly static string AddYourCountryCodeonly = GetValue("AddYourCountryCodeonly");
        public readonly static string KeepeachCountrycodeinonline = GetValue("KeepeachCountrycodeinonline");
        public readonly static string TograbEmailIds = GetValue("TograbEmailIds");
        public readonly static string Addemailidsourcetotargetlike = GetValue("Addemailidsourcetotargetlike");
        public readonly static string Keepeachemailinonline = GetValue("Keepeachemailinonline");
        public readonly static string Pleasefillthecaptcha = GetValue("Pleasefillthecaptcha");
        public readonly static string MSEdgeDriversarenotDownloadedProperly = GetValue("MSEdgeDriversarenotDownloadedProperly");
        public readonly static string ChromeDriversarenotDownloadedProperly = GetValue("ChromeDriversarenotDownloadedProperly");
        public readonly static string YourComputerdonthaveCompatiblewebviewinstallation = GetValue("YourComputerdonthaveCompatiblewebviewinstallation");
        public readonly static string AboutLicence = GetValue("AboutLicence");
        public readonly static string Paused = GetValue("Paused");
        public readonly static string NotInitialised = GetValue("NotInitialised");
        public readonly static string Initialising = GetValue("Initialising");
        public readonly static string initialised = GetValue("initialised");

        public readonly static string Warning = GetValue("Warning");
        public readonly static string YouraretryingtosendLargefile = GetValue("YouraretryingtosendLargefile");
        public readonly static string GetCommunityMembers = GetValue("GetCommunityMembers");
        public readonly static string IsDefault = GetValue("IsDefault");
        public readonly static string WaitingforyourWhatsappAccountReadiness = GetValue("WaitingforyourWhatsappAccountReadiness");
        public readonly static string fetchingdata = GetValue("fetchingdata");
        public readonly static string Schedule = GetValue("Schedule");
        public readonly static string ScheduleCampaign = GetValue("ScheduleCampaign");
        public readonly static string Pending = GetValue("Pending");
        public readonly static string ScheduleChecker = GetValue("ScheduleChecker");
        public readonly static string Pleasegiveanyname = GetValue("Pleasegiveanyname");
        public readonly static string Dateandtimeisnotvalid = GetValue("Dateandtimeisnotvalid");
        public readonly static string ScheduleUpdatedSuccessfully = GetValue("ScheduleUpdatedSuccessfully");
        public readonly static string CampaignScheduledSuccessfully = GetValue("CampaignScheduledSuccessfully");
        public readonly static string Success = GetValue("Success");
        public readonly static string MakesureyourPCwillbeturnONattheScheduleDateandtime = GetValue("MakesureyourPCwillbeturnONattheScheduleDateandtime");
        public readonly static string ScheduleName = GetValue("ScheduleName");
        public readonly static string ScheduleDateandTime = GetValue("ScheduleDateandTime");
        public readonly static string timein24Hoursformat = GetValue("timein24Hoursformat");
        public readonly static string ScheduleNow = GetValue("ScheduleNow");
        public readonly static string PleaseNote = GetValue("PleaseNote");
        public readonly static string Makesure = GetValue("Makesure");
        public readonly static string isnotRunningonScheduletime = GetValue("isnotRunningonScheduletime");
        public readonly static string PleasemaintainTenminutesgapbetweenschedules = GetValue("PleasemaintainTenminutesgapbetweenschedules");
        public readonly static string TryingtoruntheScheduleBut = GetValue("TryingtoruntheScheduleBut");
        public readonly static string alreadyrunning = GetValue("alreadyrunning");
        public readonly static string IwillstayhereutillyourallSchedulesarecompleted = GetValue("IwillstayhereutillyourallSchedulesarecompleted");
        public readonly static string Days = GetValue("Days");
        public readonly static string Hours = GetValue("Hours");
        public readonly static string Minutes = GetValue("Minutes");
        public readonly static string NextSchedulein = GetValue("NextSchedulein");
        public readonly static string AllSchedules = GetValue("AllSchedules");
        public readonly static string ScheduleDate = GetValue("ScheduleDate");
        public readonly static string Edit = GetValue("Edit");
        public readonly static string Report = GetValue("Report");
        public readonly static string Clone = GetValue("Clone");
        public readonly static string SendNow = GetValue("SendNow");
        public readonly static string PENDING = GetValue("PENDING");
        public readonly static string COMPLETED = GetValue("COMPLETED");
        public readonly static string MISSED = GetValue("MISSED");
        public readonly static string Group = GetValue("Group");
        public readonly static string individual = GetValue("individual");
        public readonly static string AreyousuretodeletethisSchedule = GetValue("AreyousuretodeletethisSchedule");
        public readonly static string TocreatenewSchedulePrepareyourCampainwithyourmessageandnumbers = GetValue("TocreatenewSchedulePrepareyourCampainwithyourmessageandnumbers");
        public readonly static string Clickon = GetValue("Clickon");
        public readonly static string Button = GetValue("Button");
        public readonly static string OptionwillbeavailableonLauncherwindow = GetValue("OptionwillbeavailableonLauncherwindow");
        public readonly static string Takinganinternalupdates = GetValue("Takinganinternalupdates");
        public readonly static string EnableFriendlyNumbers = GetValue("EnableFriendlyNumbers");
        public readonly static string FriendlyNumbers = GetValue("FriendlyNumbers");
        public readonly static string FriendlyNumber = GetValue("FriendlyNumber");
        public readonly static string WhatIs = GetValue("WhatIs");
        public readonly static string SendmessagetothisNumbersaftermessages = GetValue("SendmessagetothisNumbersaftermessages");
        public readonly static string AspartoftheAntibanSetting = GetValue("AspartoftheAntibanSetting");
        public readonly static string Recommendedtotalis3to7numbers = GetValue("Recommendedtotalis3to7numbers");
        public readonly static string Ifyouareusingmultipleaccountstosendmessages = GetValue("Ifyouareusingmultipleaccountstosendmessages");
        public readonly static string Entereachnumberinoneline = GetValue("Entereachnumberinoneline");
        public readonly static string Pleasecheckinputs = GetValue("Pleasecheckinputs");
        public readonly static string Failed = GetValue("Failed");
        public readonly static string Dontlinkpreviewwhilesendingmessage = GetValue("Dontlinkpreviewwhilesendingmessage");
        public readonly static string Fallbackmessagemeansifanyincommingmessage = GetValue("Fallbackmessagemeansifanyincommingmessage");
        public readonly static string forexampleIdontunderstandthat = GetValue("forexampleIdontunderstandthat");
        public readonly static string CheckSubLinks = GetValue("CheckSubLinks");
        public readonly static string LimitofSublinksunderanywebsite = GetValue("LimitofSublinksunderanywebsite");
        public readonly static string Put0asunlimited = GetValue("Put0asunlimited");
        public readonly static string IfincomingmessagematchEXACTwithyourrulemessage = GetValue("IfincomingmessagematchEXACTwithyourrulemessage");
        public readonly static string RuleMessagehi = GetValue("RuleMessagehi");
        public readonly static string Usersnedhi = GetValue("Usersnedhi");
        public readonly static string IfincomingmessageCONTAINSwithanyspecificword = GetValue("IfincomingmessageCONTAINSwithanyspecificword");
        public readonly static string ieRuleMessagenotworking = GetValue("ieRuleMessagenotworking");
        public readonly static string UserSendheythereproductisnotworking = GetValue("UserSendheythereproductisnotworking");
        public readonly static string IfincomingmessageStartwithanyspecificword = GetValue("IfincomingmessageStartwithanyspecificword");
        public readonly static string ieRuleMessageSameError = GetValue("ieRuleMessageSameError");
        public readonly static string UserSendSameerrorafterthesameprocess = GetValue("UserSendSameerrorafterthesameprocess");
        public readonly static string IfincomingmessageEndswithanyspecificword = GetValue("IfincomingmessageEndswithanyspecificword");
        public readonly static string ieRuleMessageThankYou = GetValue("ieRuleMessageThankYou");
        public readonly static string UserSendHaveagoodaybrothankyou = GetValue("UserSendHaveagoodaybrothankyou");
        public readonly static string Summery = GetValue("Summery");
        public readonly static string TotalSuccess = GetValue("TotalSuccess");
        public readonly static string TotalFail = GetValue("TotalFail");
        public readonly static string DownloadReport = GetValue("DownloadReport");
        public readonly static string DontuseyourAccountforbulkSendingifitsbrandnew = GetValue("DontuseyourAccountforbulkSendingifitsbrandnew");
        public readonly static string PleaseConfirm = GetValue("PleaseConfirm");
        public readonly static string Afterobtainingthelistofcommunitymembers = GetValue("Afterobtainingthelistofcommunitymembers");
        public readonly static string Toeditaruledoubleclickonit = GetValue("Toeditaruledoubleclickonit");
        public readonly static string ToeditaMessagedoubleclickonit = GetValue("ToeditaMessagedoubleclickonit");
        public readonly static string WarmingMethod = GetValue("WarmingMethod");
        public readonly static string OnetoOne = GetValue("OnetoOne");
        public readonly static string OnetoMany = GetValue("OnetoMany");
        public readonly static string Onetoonemeans = GetValue("Onetoonemeans");
        public readonly static string RecommendedIfyouhavenewwhatsappaccounts = GetValue("RecommendedIfyouhavenewwhatsappaccounts");
        public readonly static string OnetoManymeans = GetValue("OnetoManymeans");
        public readonly static string RecommendedifyouhaveOldWhatsAppAccounts = GetValue("RecommendedifyouhaveOldWhatsAppAccounts");
        public readonly static string Browser = GetValue("Browser");


        public readonly static string ImportNumberinSender = GetValue("ImportNumberinSender");
        public readonly static string QuickFilter = GetValue("QuickFilter");
        public readonly static string Unknown = GetValue("Unknown");
        public readonly static string SkipFiltaringandsendMessagewithoutcheckingnumber = GetValue("SkipFiltaringandsendMessagewithoutcheckingnumber");
        public readonly static string Thissoftwaredoesnothaveitsowngroupdatabase = GetValue("Thissoftwaredoesnothaveitsowngroupdatabase");
        public readonly static string ItisrecommendedtotrygenerickeywordsinEnglishsuchas = GetValue("ItisrecommendedtotrygenerickeywordsinEnglishsuchas");
        public readonly static string TagAllMemberswithmessage = GetValue("TagAllMemberswithmessage");
        public readonly static string BuiltInVariable = GetValue("BuiltInVariable");
        public readonly static string Spoiler = GetValue("Spoiler");
        public readonly static string Info = GetValue("Info");
        public readonly static string Close = GetValue("Close");
        public readonly static string NAMEvariableinsertsausersnameintoamessageautomaticallywhilesendingmessages = GetValue("NAMEvariableinsertsausersnameintoamessageautomaticallywhilesendingmessages");
        public readonly static string Donottranslatethisvariableintoanyotherlanguage = GetValue("Donottranslatethisvariableintoanyotherlanguage");
        public readonly static string ImportantNotethisvariableonlyworksforthosenumberswho = GetValue("ImportantNotethisvariableonlyworksforthosenumberswho");
        public readonly static string SPOILERthisvariableSendWhatsAppmessageswithspoilertags = GetValue("SPOILERthisvariableSendWhatsAppmessageswithspoilertags");
        public readonly static string NoteSpoilerswillnotworkforiOS = GetValue("NoteSpoilerswillnotworkforiOS");
        public readonly static string CountryShortCode = GetValue("CountryShortCode");
        public readonly static string Starting = GetValue("Starting");
        public readonly static string FilterNumbersBeforeSendingMessages = GetValue("FilterNumbersBeforeSendingMessages");
        public readonly static string ExportAll = GetValue("ExportAll");
        public readonly static string ExportAvailable = GetValue("ExportAvailable");
        public readonly static string DeleteAllRows = GetValue("DeleteAllRows");


        public readonly static string NoteRightClicktoanyaccounttosentitasprimary = GetValue("NoteRightClicktoanyaccounttosentitasprimary");
        public readonly static string CalltoActionButton = GetValue("CalltoActionButton");
        public readonly static string ReplyButtons = GetValue("ReplyButtons");
        public readonly static string title = GetValue("title");
        public readonly static string FooterText = GetValue("FooterText");
        public readonly static string PleaseSelectButtonType = GetValue("PleaseSelectButtonType");
        public readonly static string PleaseaddOneorMoreButtons = GetValue("PleaseaddOneorMoreButtons");
        public readonly static string PleaseEnterTitle = GetValue("PleaseEnterTitle");
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //public readonly static string TotalFail = "TotalFail";
        //------------END  Do not Trancelate ----------------------
        public static string PurchaseCode = Config.PurchaseCode;
        //------------END  Do not Trancelate ----------------------
        

        public static List<WASender.Models.LanguageModel> dict;


        #region donnotchange
        public readonly static string SoftwareVersion = "3.5.0"; /// Dont change this
        public readonly static int CurrentInternalUpdateVersion = 2; //It Should be same as listed in onlie file 
        #endregion

        public static string getLanguage(LanguagesEnum enumLang)
        {
            if (enumLang == LanguagesEnum.Arabic)
            {
                return "Arabic";    
            }
            else if (enumLang == LanguagesEnum.Chinese)
            {
                return "Chinese";
            }
            else if (enumLang == LanguagesEnum.Dutch)
            {
                return "Dutch";
            }
            else if (enumLang == LanguagesEnum.English)
            {
                return "English";
            }
            else if (enumLang == LanguagesEnum.French)
            {
                return "French";
            }
            else if (enumLang == LanguagesEnum.German)
            {
                return "German";
            }
            else if (enumLang == LanguagesEnum.Greek)
            {
                return "Greek";
            }
            else if (enumLang == LanguagesEnum.Gujarati)
            {
                return "Gujarati";
            }
            else if (enumLang == LanguagesEnum.Hebrew)
            {
                return "Hebrew";
            }
            else if (enumLang == LanguagesEnum.Hindi)
            {
                return "Hindi";
            }
            else if (enumLang == LanguagesEnum.Indonesian)
            {
                return "Indonesian";
            }
            else if (enumLang == LanguagesEnum.Italian)
            {
                return "Italian";
            }
            else if (enumLang == LanguagesEnum.Japanese)
            {
                return "Japanese";
            }
            else if (enumLang == LanguagesEnum.Laos)
            {
                return "Laos";
            }
            else if (enumLang == LanguagesEnum.Portuguese)
            {
                return "Portuguese";
            }
            else if (enumLang == LanguagesEnum.Russian)
            {
                return "Russian";
            }
            else if (enumLang == LanguagesEnum.Spanish)
            {
                return "Spanish";
            }
            else if (enumLang == LanguagesEnum.Turkish)
            {
                return "Turkish";
            }
            else if (enumLang == LanguagesEnum.Urdu)
            {
                return "Urdu";
            }
            else if (enumLang == LanguagesEnum.Ukrainian)
            {
                return "Ukrainian";
            }
            else if (enumLang == LanguagesEnum.Tamil)
            {
                return "Tamil";
            }
            else if (enumLang == LanguagesEnum.Bengali)
            {
                return "Bengali";
            }
            else if (enumLang == LanguagesEnum.Telugu)
            {
                return "Telugu";
            }
            else if (enumLang == LanguagesEnum.Malaya)
            {
                return "Malaya";
            }
            return "English";
        }
        public static string GetValue(string name)
        {
            if (languageJson == "")
            {
                LoadLanguageJson();
            }
            if (dict == null)
            {
                dict = JsonConvert.DeserializeObject<List<WASender.Models.LanguageModel>>(languageJson);
            }

            

            try
            {
                return dict.Where(x => x.name.Trim() == name.Trim()).FirstOrDefault().value;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private static void LoadLanguageJson()
        {
            string fileName = "English.json";

            if (selectedLanguage != "" && selectedLanguage != null)
            {
                fileName = selectedLanguage;
            }
            try
            {
                string settingPath = Config.GetGeneralSettingsFilePath();

                if (!File.Exists(settingPath))
                {
                    File.Create(settingPath).Close();
                }
                GeneralSettingsModel generalSettingsModel = new GeneralSettingsModel();
                generalSettingsModel.selectedLanguage = selectedLanguage;
                string GeneralSettingJson = "";
                using (StreamReader r = new StreamReader(settingPath))
                {
                    GeneralSettingJson = r.ReadToEnd();
                }
                var dict = JsonConvert.DeserializeObject<GeneralSettingsModel>(GeneralSettingJson);
                if (dict != null)
                {
                    generalSettingsModel = dict;
                }
                if (generalSettingsModel.selectedLanguage == null || generalSettingsModel.selectedLanguage == "")
                {
                    generalSettingsModel.selectedLanguage = selectedLanguage;
                }
                selectedLanguage = generalSettingsModel.selectedLanguage;
            }
            catch (Exception ex)
            {

            }

            if (selectedLanguage != "")
            {
                fileName = selectedLanguage + ".json";
            }

            using (StreamReader r = new StreamReader("languages\\" + fileName))
            {
                languageJson = r.ReadToEnd();
            }
        }




    }
}


