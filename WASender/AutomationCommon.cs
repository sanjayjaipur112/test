using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WASender.enums;
using WASender.Models;

namespace WASender
{
    public class AutomationCommon
    {


        public static IWebElement WaitUntilElementVisible(IWebDriver driver, By by, int attempts = 10)
        {
            IWebElement element = null;
            int attempt = 0;
            while (element == null)
            {
                if (attempt >= attempts)
                {
                    break;
                }
                element = check(driver, by);
                attempt++;
            }
            return element;
        }

        public static IWebElement WaitUntilElementDispose(IWebDriver driver, By by, int attempts = 10, bool doClick = false, string matchText = "")
        {
            IWebElement element = check(driver, by);
            int attempt = 0;
            while (element != null)
            {
                if (attempt >= attempts)
                {
                    break;
                }
                element = check(driver, by, doClick, matchText);
                attempt++;
            }
            return element;
        }

        private static IWebElement check(IWebDriver driver, By by, bool doClick = false, string matchText = "")
        {
            Thread.Sleep(1000);
            if (IsElementPresent(by, driver))
            {
                IWebElement el = driver.FindElement(by);
                if (doClick == true)
                {
                    if (matchText != "")
                    {
                        if (matchText == el.Text)
                        {
                            el.Click();
                        }
                    }
                    else
                    {
                        el.Click();
                    }

                }
                return el;
            }
            return null;
        }


        private static string successImage = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA8AAAALCAYAAACgR9dcAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyZpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuNi1jMTM4IDc5LjE1OTgyNCwgMjAxNi8wOS8xNC0wMTowOTowMSAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENDIDIwMTcgKFdpbmRvd3MpIiB4bXBNTTpJbnN0YW5jZUlEPSJ4bXAuaWlkOkY1ODVFMzhBMEI3QjExRUZCOUJBRURFRjQ4QjUxQzY4IiB4bXBNTTpEb2N1bWVudElEPSJ4bXAuZGlkOkY1ODVFMzhCMEI3QjExRUZCOUJBRURFRjQ4QjUxQzY4Ij4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6RjU4NUUzODgwQjdCMTFFRkI5QkFFREVGNDhCNTFDNjgiIHN0UmVmOmRvY3VtZW50SUQ9InhtcC5kaWQ6RjU4NUUzODkwQjdCMTFFRkI5QkFFREVGNDhCNTFDNjgiLz4gPC9yZGY6RGVzY3JpcHRpb24+IDwvcmRmOlJERj4gPC94OnhtcG1ldGE+IDw/eHBhY2tldCBlbmQ9InIiPz6hVX3cAAACE0lEQVR42oRSzWsTQRT/zcx+JFmaJlubkEZREVtpU6qlEaQ3EU9SlAga7cGDf4Fnbx49eJRePVSpSEDTEFQoFIoUC4IUSrVCTciHNa6tSfOx2dlxtkFFEHzw4PEev495b8i97YewucDq900kzZOweA0RZoIKFSXXgss5fFSHwjCxUM4+tjo/QjPR87fCurmm4D+hMR0QYuxJ4elitVOLg6hYsd4+OqQP5hUKAo0qoIQd1plWYy5r92ACjDAQQRLZ8ovn1bYE+uOy7aDU2YmW2pURajAD7+vr9/O1bPFZJfdSp2pMoypUmZIsvljNZbZbxePwxwDXBlplJIOJ5WtDMwvMnjUf5Hde3xFE4Mv+56Ml27pwcWB6LkANMleYXy40tkbhi0hFAjh7MLSBrXPm2UtHfLFXysb+5jCnDGBBQAUK9U9yMbl5SMPF+seJA0V4QAsxPfohGjiWanO73OBN0NtD6buDzCjA/tp7qi+OJevN9aVvK+keUIachZlRGQ2OX+bCWff2IelA+5jv3ZnwVEquvomu1VNRPBf93lgCLUTUUCtpTl9pu/YGF/z3JWiTtzyetRPGcCrMAh10d2VXk6kfAEPM3xkJnr4qSVcdtyupyR+wV3pslND8ZDiZDhDShdOAl4asJ8NTaUJITtoFIX//AfqrcORQEJYZ60vckKfaZZTuneofvymIluFS8V/xU4ABAAREzr5Wl0pAAAAAAElFTkSuQmCC";
        private static string failImage = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyZpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuNi1jMTM4IDc5LjE1OTgyNCwgMjAxNi8wOS8xNC0wMTowOTowMSAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENDIDIwMTcgKFdpbmRvd3MpIiB4bXBNTTpJbnN0YW5jZUlEPSJ4bXAuaWlkOjE1MjdEMUFGMEI3QzExRUZCMEUwRUY0OTlBN0U4MDZEIiB4bXBNTTpEb2N1bWVudElEPSJ4bXAuZGlkOjE1MjdEMUIwMEI3QzExRUZCMEUwRUY0OTlBN0U4MDZEIj4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6MTUyN0QxQUQwQjdDMTFFRkIwRTBFRjQ5OUE3RTgwNkQiIHN0UmVmOmRvY3VtZW50SUQ9InhtcC5kaWQ6MTUyN0QxQUUwQjdDMTFFRkIwRTBFRjQ5OUE3RTgwNkQiLz4gPC9yZGY6RGVzY3JpcHRpb24+IDwvcmRmOlJERj4gPC94OnhtcG1ldGE+IDw/eHBhY2tldCBlbmQ9InIiPz4UlVbkAAACpElEQVR42nRSW0tUURRea+9zRmccR7sg6UN0e8i30JBIoocyER284C0jZTTRyuhFKCoi8EIS3aCILGXywpQElUh2fYi0MOoPVJTKdFEsFZ3OzOx99m6fkSbL+h72y17fWt/6voUTY2/B1eWr1F+9KWUamcXsHZf0nKyXxDAAkILhsgPadaC37qfis+FGMTmVRNI3PdYqSy9rrjsD1bE9vg7QKFAhwRz350tnfAVs39oPRgjAGQc4PLKZerv7ycxMslUn747lMc6WI891v6ajH9LBZoMIOAdBKMNaTz2UFHSyoRc7tTMXe8ncfBLY9IUaKYEDTmtgj50DNTEKTQNimrrovHGNfxxL1Z4P7SPGj8Qo0YIQIB2OIMHsrLOCogRh/v6kFIgQxDb4oJGEQomgLyJKASIcBlJW2kQwJ2fAbKirlYyrhRc1QASIiQEgRBEgKlcGlQ8VZSdpvvuKhspVvruwQ1LK9HbvBWRsWYS4BEqckEFetbdFVFU0Y1iAFnDEArUaVJd3kc9Tq6jP14Z2+1Iu42Cu3eCdaDjYHHQmAIIAMu90KHmqQf/DXeTJ0/1oSf0XlJHUP16y0nfTk/ztC6T4/YBfmQErbvfl0tPn+tAUDtA1+C8sl7nypa6mFt3u65pj8FEmbTvfh+IvojIHlKuR/H95oMxDqna/2tEu4l3TJM7nOwrBkMOStXiCUGSWnnZPImF/pKBiVL1Q9vS2EDEztxrVyUWhCkVY3U9x4Qmt9VQBr6/xSEIN6/KiKSqFODmVQiAjrRus7NQ0YCwyUR6orcfK8hYMhYDvKeplRw4XS10PQCi8kHXAAHNLRhc91to0wnUaxtFPa1hiwntRVnSIFub1YDAYOQ5O1LNx/TuQdEh+n11nBgwTtmV6tTrP8Z8CDACIVCz0o1LergAAAABJRU5ErkJggg==";

        public static string GenerateReport(WASenderGroupTransModel wASenderSingleTransModel)
        {

            string text = System.IO.File.ReadAllText(@"bootstrap.min.css");


            string strim = "<head> <meta charset=\"UTF-8\"></head> <style>";
            strim += text;
            strim += "</style>";

            strim += "<body><div class=\"container\"><center><h1>";
            strim += wASenderSingleTransModel.CampaignName;
            strim += " </h1></center>";

            int MesageCounter = 0;
            if (wASenderSingleTransModel.messages != null)
            {
                foreach (var message in wASenderSingleTransModel.messages)
                {
                    if (message != null)
                    {
                        MesageCounter++;
                        strim += " <div class='panel panel-default'>";
                        strim += "  <div class='panel-heading'>";
                        strim += "      "+Strings.Message+" " + MesageCounter;
                        strim += "  </div>";
                        strim += "  <div class='panel-body'>";
                        strim += message.longMessage;
                        strim += "  </div>";
                        strim += "";
                        strim += "";
                        strim += "</div>";
                        strim += "<br/>";
                    }
                }
            }

            int SuccessCount = wASenderSingleTransModel.groupList.Where(x => x.sendStatusModel.sendStatusEnum == SendStatusEnum.Success).Count();
            int FailedCount = wASenderSingleTransModel.groupList.Where(x => x.sendStatusModel.sendStatusEnum != SendStatusEnum.Success).Count();

            strim += " <div class='panel panel-default'>";
            strim += "  <div class='panel-heading'>";
            strim += "       "+ Strings.Summery;
            strim += "  </div>";
            strim += "  <div class='panel-body'>";
            strim += Strings.TotalSuccess +"  = " + SuccessCount + " / ";
            strim += Strings.TotalFail + "  = " + FailedCount;
            strim += "  </div>";
            strim += "";
            strim += "";
            strim += "</div>";
            strim += "<br/>";


            strim += "<table class=\"table table-bordered\"><thead><tr><th>" + Strings.ChatName + "</th><th>" + Strings.Result + "</th></tr></thead><tbody>";

            

            foreach (GroupModel contact in wASenderSingleTransModel.groupList)
            {
                strim += "<tr>";
                strim += "<td>";
                strim += contact.Name;
                strim += "</td>";
                strim += "<td>";
                if (contact.sendStatusModel.sendStatusEnum == SendStatusEnum.Success)
                {
                    strim += "<img src=\"" + successImage + "\"> " + Strings.Success;
                }
                else
                {
                    strim += "<img src=\"" + failImage + "\">" + " " + Strings.Failed + " - " + contact.sendStatusModel.sendStatusEnum.ToString();
                }
                strim += "</td>";
                strim += "</tr>";
            }


            string script = "";
            script += "<script>";
            script += "function downloadDiv(filename, elementId, mimeType) {";
            script += "     var elementHtml = elementId;";
            script += "     var link = document.createElement('a');";
            script += "     mimeType = mimeType || 'text/plain';";
            script += "     link.setAttribute('download', filename);";
            script += "     link.setAttribute('href', 'data:' + mimeType  +  ';charset=utf-8,' + encodeURIComponent(elementHtml));";
            script += "     link.click(); ";
            script += "}";
            script += "</script>";


            strim += "</tbody></table><button type=\"button\" onClick='downloadDiv( document.getElementsByTagName(\"h1\")[0].innerText+\".html\",document.body.innerHTML,\"text/html\");'> " + Strings.DownloadReport + "</a></div></div></body>" + script;

            return strim;
        }

        public static string GenerateReport(WASenderSingleTransModel wASenderSingleTransModel)
        {
            string text = System.IO.File.ReadAllText(@"bootstrap.min.css");


            string strim = "<style>";
            strim += text;
            strim += "</style>";
            strim += "<body><div class=\"container\"><center><h1>";
            strim += wASenderSingleTransModel.CampaignName;
            strim += " </h1></center>";

            int MesageCounter = 0;
            foreach (var message in wASenderSingleTransModel.messages)
            {
                if (message != null)
                {
                    MesageCounter++;
                    strim += " <div class='panel panel-default'>";
                    strim += "  <div class='panel-heading'>";
                    strim += Strings.Message+ "       " + MesageCounter;
                    strim += "  </div>";
                    strim += "  <div class='panel-body'>";
                    strim += message.longMessage;
                    strim += "  </div>";
                    strim += "";
                    strim += "";
                    strim += "</div>";
                    strim += "<br/>";
                }
            }

            int SuccessCount = wASenderSingleTransModel.contactList.Where(x => x.sendStatusModel.sendStatusEnum == SendStatusEnum.Success).Count();
            int FailedCount = wASenderSingleTransModel.contactList.Where(x => x.sendStatusModel.sendStatusEnum != SendStatusEnum.Success).Count();

            strim += " <div class='panel panel-default'>";
            strim += "  <div class='panel-heading'>";
            strim += "       "+ Strings.Summery;
            strim += "  </div>";
            strim += "  <div class='panel-body'>";
            strim += Strings.TotalSuccess +"  = " + SuccessCount + " / ";
            strim += Strings.TotalFail+ "  = " + FailedCount;
            strim += "  </div>";
            strim += "";
            strim += "";
            strim += "</div>";
            strim += "<br/>";


            strim += "<table class=\"table table-bordered\"><thead><tr><th>"+ Strings.ChatName +"</th><th>"+ Strings.Result +"</th></tr></thead><tbody>";


            foreach (ContactModel contact in wASenderSingleTransModel.contactList)
            {
                strim += "<tr>";
                strim += "<td>";
                strim += contact.number;
                strim += "</td>";
                strim += "<td>";
                if (contact.sendStatusModel.sendStatusEnum == SendStatusEnum.Success)
                {
                    strim += "<img src=\""+ successImage +"\"> " + Strings.Success + "" + (contact.isFriendly ? " - " + Strings.FriendlyNumber : "");
                }
                else
                {
                    strim += "<img src=\""+ failImage +"\">" + " " + Strings.Failed + " - " + contact.sendStatusModel.sendStatusEnum.ToString();
                }
                strim += "</td>";
                strim += "</tr>";
            }

            string script = "";
            script += "<script>";
            script += "function downloadDiv(filename, elementId, mimeType){";
            script += "var elementHtml = elementId;";
            script += "var link = document.createElement('a');";
            script += "mimeType = mimeType || 'text/plain';";
            script += "link.setAttribute('download', filename);";
            script += "link.setAttribute('href', 'data:' + mimeType  +  ';charset=utf-8,' + encodeURIComponent(elementHtml));link.click();}";
            script += "</script>";


            strim += "</tbody></table><button type=\"button\" onClick='downloadDiv( document.getElementsByTagName(\"h1\")[0].innerText+\".html\",document.body.innerHTML,\"text/html\");'> " + Strings.DownloadReport + "</a></div></div></body>" + script;

            return strim;
        }


        public static void isLoadingWeb(IWebDriver driver)
        {
            By loadingBy = By.XPath("//div[contains(@class, '_1INL_') and contains(@class, '_1iyey') and contains(@class, '_1UG2S') ]");
            bool IsLoading = AutomationCommon.IsElementPresent(loadingBy, driver);
            if (IsLoading == true)
            {
                AutomationCommon.WaitUntilElementDispose(driver, loadingBy, 500);
            }
        }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }


        public static void checkConnection(IWebDriver driver)
        {

            By alertPhone = By.CssSelector("[data-testid='alert-phone']");
            if (IsElementPresent(alertPhone, driver))
            {
                AutomationCommon.WaitUntilElementDispose(driver, alertPhone, 5000, true);
            }

            By alertComputer = By.CssSelector("[data-testid='alert-computer']");
            if (IsElementPresent(alertComputer, driver))
            {
                AutomationCommon.WaitUntilElementDispose(driver, alertComputer, 5000, true);
            }



            string ss = "";

            By retryingBy = By.XPath("//div[contains(@class, 'tvf2evcx') and contains(@class, 'm0h2a7mj') and contains(@class, 'lb5m6g5c') and contains(@class, 'j7l1k36l') and contains(@class, 'ktfrpxia') and contains(@class, 'nu7pwgvd') and contains(@class, 'gjuq5ydh') ]");
            bool IsRetrying = AutomationCommon.IsElementPresent(retryingBy, driver);

            if (IsRetrying == true)
            {
                var retryEltxt = driver.FindElements(retryingBy);
                foreach (var retryEl in retryEltxt)
                {
                    if (retryEl.Text == "RETRY NOW")
                    {
                        AutomationCommon.WaitUntilElementDispose(driver, retryingBy, 500, true, "Retry Now");
                    }
                }
            }
        }


        public static bool IsElementPresent(By by, IWebElement element)
        {
            try
            {
                element.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }


        public static bool IsElementPresent(By by, IWebDriver driver)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public static void ChangeCampStatus(CampaignStatusEnum _campaignStatus, Label lblRunStatus)
        {
            if (_campaignStatus == CampaignStatusEnum.NotStarted)
            {
                lblRunStatus.ForeColor = Color.Orange;
                lblRunStatus.Text = Strings.NotStarted;
            }
            else if (_campaignStatus == CampaignStatusEnum.Starting)
            {
                lblRunStatus.ForeColor = Color.Gray;
                lblRunStatus.Text = Strings.Starting;
            }
            else if (_campaignStatus == CampaignStatusEnum.Running)
            {
                lblRunStatus.ForeColor = Color.Green;
                lblRunStatus.Text = Strings.Running;
            }
            else if (_campaignStatus == CampaignStatusEnum.Paused)
            {
                lblRunStatus.ForeColor = Color.Blue;
                lblRunStatus.Text = Strings.Paused;
            }
            else if (_campaignStatus == CampaignStatusEnum.Stopped)
            {
                lblRunStatus.ForeColor = Color.Red;
                lblRunStatus.Text = Strings.Stopped;
            }
            else if (_campaignStatus == CampaignStatusEnum.Error)
            {
                lblRunStatus.ForeColor = Color.DarkRed;
                lblRunStatus.Text = Strings.Error;
            }
            else if (_campaignStatus == CampaignStatusEnum.Finish)
            {
                lblRunStatus.ForeColor = Color.DarkGray;
                lblRunStatus.Text = Strings.Completed;
            }
        }

        public static void ChangeInitStatus(InitStatusEnum _initStatus, Label lblInitStatus)
        {
            if (_initStatus == InitStatusEnum.NotInitialised)
            {
                lblInitStatus.ForeColor = Color.Orange;
                lblInitStatus.Text = Strings.NotInitialised;
            }
            else if (_initStatus == InitStatusEnum.Initialising)
            {
                lblInitStatus.ForeColor = Color.Gray;
                lblInitStatus.Text = Strings.Initialising;
            }
            else if (_initStatus == InitStatusEnum.Initialised)
            {
                lblInitStatus.ForeColor = Color.Green;
                lblInitStatus.Text = Strings.initialised;
            }
            else if (_initStatus == InitStatusEnum.Unable)
            {
                lblInitStatus.ForeColor = Color.DarkRed;
                lblInitStatus.Text = Strings.Error;
            }
            else if (_initStatus == InitStatusEnum.Stopped)
            {
                lblInitStatus.ForeColor = Color.Red;
                lblInitStatus.Text = Strings.Stopped;
            }


        }


        public static List<string> GetNumbers(string List)
        {
            List = List.Replace(" ", "");
            List = List.Replace("+", "");
            List = List.Replace("-", "");
            List = List.Replace("/", "");
            List = List.Replace("\\", "");
            List = List.Replace("(", "");
            List = List.Replace(")", "");
            List = List.Replace("，", ",");
            List<string> list = new List<string>();
            string[] array = List.Split(',');
            foreach (string text in array)
            {
                // if (text.StartsWith("+"))
                // {
                list.Add(text);
                //}
            }
            return list;
        }

    }

}
