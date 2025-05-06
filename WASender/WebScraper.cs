using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;


public static class WebScraper
{
	[ComImport]
	[InterfaceType(1)]
	[Guid("3050F669-98B5-11CF-BB82-00AA00BDCE0B")]
	private interface IHTMLElementRenderFixed
	{
		void DrawToDC(IntPtr hdc);

		void SetDocumentPrinter(string bstrPrinterName, IntPtr hdc);
	}

	public static void SetBrowserFeatureControlKey(string feature, string appName, uint value)
	{
		RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\" + feature, RegistryKeyPermissionCheck.ReadWriteSubTree);
		registryKey.SetValue(appName, value, RegistryValueKind.DWord);
	}

	public static void SetBrowserFeatureControl()
	{
		string fileName = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
		if (string.Compare(fileName, "devenv.exe", ignoreCase: true) != 0 && string.Compare(fileName, "XDesProc.exe", ignoreCase: true) != 0)
		{
			SetBrowserFeatureControlKey("FEATURE_BROWSER_EMULATION", fileName, GetBrowserEmulationMode());
			SetBrowserFeatureControlKey("FEATURE_AJAX_CONNECTIONEVENTS", fileName, 1u);
			SetBrowserFeatureControlKey("FEATURE_ENABLE_CLIPCHILDREN_OPTIMIZATION", fileName, 1u);
			SetBrowserFeatureControlKey("FEATURE_MANAGE_SCRIPT_CIRCULAR_REFS", fileName, 1u);
			SetBrowserFeatureControlKey("FEATURE_DOMSTORAGE ", fileName, 1u);
			SetBrowserFeatureControlKey("FEATURE_GPU_RENDERING ", fileName, 1u);
			SetBrowserFeatureControlKey("FEATURE_IVIEWOBJECTDRAW_DMLT9_WITH_GDI  ", fileName, 0u);
			SetBrowserFeatureControlKey("FEATURE_DISABLE_LEGACY_COMPRESSION", fileName, 1u);
			SetBrowserFeatureControlKey("FEATURE_LOCALMACHINE_LOCKDOWN", fileName, 0u);
			SetBrowserFeatureControlKey("FEATURE_BLOCK_LMZ_OBJECT", fileName, 0u);
			SetBrowserFeatureControlKey("FEATURE_BLOCK_LMZ_SCRIPT", fileName, 0u);
			SetBrowserFeatureControlKey("FEATURE_DISABLE_NAVIGATION_SOUNDS", fileName, 1u);
			SetBrowserFeatureControlKey("FEATURE_SCRIPTURL_MITIGATION", fileName, 1u);
			SetBrowserFeatureControlKey("FEATURE_SPELLCHECKING", fileName, 0u);
			SetBrowserFeatureControlKey("FEATURE_STATUS_BAR_THROTTLING", fileName, 1u);
			SetBrowserFeatureControlKey("FEATURE_TABBED_BROWSING", fileName, 1u);
			SetBrowserFeatureControlKey("FEATURE_VALIDATE_NAVIGATE_URL", fileName, 1u);
			SetBrowserFeatureControlKey("FEATURE_WEBOC_DOCUMENT_ZOOM", fileName, 1u);
			SetBrowserFeatureControlKey("FEATURE_WEBOC_POPUPMANAGEMENT", fileName, 0u);
			SetBrowserFeatureControlKey("FEATURE_WEBOC_MOVESIZECHILD", fileName, 1u);
			SetBrowserFeatureControlKey("FEATURE_ADDON_MANAGEMENT", fileName, 0u);
			SetBrowserFeatureControlKey("FEATURE_WEBSOCKET", fileName, 1u);
			SetBrowserFeatureControlKey("FEATURE_WINDOW_RESTRICTIONS ", fileName, 0u);
			SetBrowserFeatureControlKey("FEATURE_XMLHTTP", fileName, 1u);
		}
	}

	private static uint GetBrowserEmulationMode()
	{
		int result = 11000;
		using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Internet Explorer", RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.QueryValues))
		{
			object value = registryKey.GetValue("svcVersion");
			if (value == null)
			{
				value = registryKey.GetValue("Version");
				if (value == null)
				{
					throw new ApplicationException("Microsoft Internet Explorer is required!");
				}
			}
			int.TryParse(value.ToString().Split('.')[0], out result);
		}
		uint result2 = 11000u;
		switch (result)
		{
		case 7:
			result2 = 7000u;
			break;
		case 8:
			result2 = 8000u;
			break;
		case 9:
			result2 = 9000u;
			break;
		case 10:
			result2 = 10000u;
			break;
		}
		return result2;
	}

	public static List<HtmlElement> GetHTMLElementsCollection(WebBrowser webBrowser, HtmlDocument Frame, string Tag, string ClassName)
	{
		List<HtmlElement> list = new List<HtmlElement>();
		foreach (HtmlElement item in Frame.GetElementsByTagName(Tag))
		{
			if (item.GetAttribute("className") == ClassName)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static List<HtmlElement> GetHTMLElementsCollection(WebBrowser webBrowser, HtmlDocument Frame, string Tag)
	{
		List<HtmlElement> list = new List<HtmlElement>();
		foreach (HtmlElement item in Frame.GetElementsByTagName(Tag))
		{
			list.Add(item);
		}
		return list;
	}

	public static List<HtmlElement> GetHTMLElementsCollection(WebBrowser webBrowser, string Tag, string ClassName)
	{
		List<HtmlElement> list = new List<HtmlElement>();
		foreach (HtmlElement item in webBrowser.Document.GetElementsByTagName(Tag))
		{
			if (item.GetAttribute("className") == ClassName)
			{
				list.Add(item);
			}
		}
		return list;
	}

    //public static List<IHTMLElement> GetHTMLElementsCollectionDOM3(WebBrowser webBrowser, string Tag, string ClassName)
    //{
    //    List<IHTMLElement> result = new List<IHTMLElement>();
    //    var _ = (IHTMLElementCollection3)((IHTMLDocument2)webBrowser.Document.DomDocument).links;
    //    return result;
    //}

	public static List<HtmlElement> GetHTMLElementsCollection(HtmlElement Element, string Tag, string ClassName)
	{
		List<HtmlElement> list = new List<HtmlElement>();
		foreach (HtmlElement item in Element.GetElementsByTagName(Tag))
		{
			if (item.GetAttribute("className") == ClassName)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static List<HtmlElement> GetHTMLElementsCollection(HtmlElement Element, string Tag, string[] ClassNames)
	{
		List<HtmlElement> list = new List<HtmlElement>();
		foreach (HtmlElement item in Element.GetElementsByTagName(Tag))
		{
			string attribute = item.GetAttribute("className");
			foreach (string text in ClassNames)
			{
				if (attribute == text)
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	public static List<HtmlElement> GetHTMLElementsCollection(HtmlElement Element, string Tag)
	{
		List<HtmlElement> list = new List<HtmlElement>();
		foreach (HtmlElement item in Element.GetElementsByTagName(Tag))
		{
			list.Add(item);
		}
		return list;
	}

	public static List<HtmlElement> GetHTMLElementsCollectionByClass(WebBrowser webBrowser, string Tag, string ClassName)
	{
		List<HtmlElement> list = new List<HtmlElement>();
		foreach (HtmlElement item in webBrowser.Document.GetElementsByTagName(Tag))
		{
			if (item.GetAttribute("className").IndexOf(ClassName) > -1)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static List<HtmlElement> GetHTMLElementsCollectionByTag(HtmlElement Element, string Tag)
	{
		List<HtmlElement> list = new List<HtmlElement>();
		foreach (HtmlElement item in Element.GetElementsByTagName(Tag))
		{
			list.Add(item);
		}
		return list;
	}

	public static List<HtmlElement> GetHTMLElementsCollectionByClassExactly(WebBrowser webBrowser, string Tag, string ClassName)
	{
		List<HtmlElement> list = new List<HtmlElement>();
		foreach (HtmlElement item in webBrowser.Document.GetElementsByTagName(Tag))
		{
			if (item.GetAttribute("className") == ClassName)
			{
				list.Add(item);
			}
		}
		return list;
	}


    
    public static List<HtmlElement> GetHTMLElementsCollectionBySimilarId(WebBrowser webBrowser, string Tag, string ClassName)
    {
        List<HtmlElement> list = new List<HtmlElement>();
        foreach (HtmlElement item in webBrowser.Document.GetElementsByTagName(Tag))
        {
            if (item.Id == ClassName)
            {
                list.Add(item);
            }
        }
        return list;
    }

	public static List<HtmlElement> GetHTMLElementsCollectionBySimilarClass(WebBrowser webBrowser, string Tag, string ClassName)
	{
		List<HtmlElement> list = new List<HtmlElement>();
		foreach (HtmlElement item in webBrowser.Document.GetElementsByTagName(Tag))
		{
			if (item.GetAttribute("className").IndexOf(ClassName) > -1)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static List<HtmlElement> GetHTMLElementsCollectionByClass(HtmlElement Element, string Tag, string ClassName)
	{
		List<HtmlElement> list = new List<HtmlElement>();
		foreach (HtmlElement item in Element.GetElementsByTagName(Tag))
		{
			string attribute = item.GetAttribute("className");
			if (ClassName == attribute)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static List<HtmlElement> GetHTMLElementsCollectionByClass(HtmlElement Element, string Tag, string[] ClassNames)
	{
		List<HtmlElement> list = new List<HtmlElement>();
		int num = 0;
		do
		{
			foreach (HtmlElement item in Element.GetElementsByTagName(Tag))
			{
				string attribute = item.GetAttribute("className");
				if (ClassNames[num] == attribute)
				{
					list.Add(item);
				}
			}
			num++;
		}
		while (list.Count == 0 && num < ClassNames.Length);
		return list;
	}

	public static List<HtmlElement> GetHTMLElementsCollectionByClass(HtmlElement Element, string[] Tags, string ClassName)
	{
		List<HtmlElement> list = new List<HtmlElement>();
		int num = 0;
		do
		{
			foreach (HtmlElement item in Element.GetElementsByTagName(Tags[num]))
			{
				string attribute = item.GetAttribute("className");
				if (ClassName == attribute)
				{
					list.Add(item);
				}
			}
			num++;
		}
		while (list.Count == 0 && num < Tags.Length);
		return list;
	}

	public static List<HtmlElement> GetHTMLElementsCollectionBySimilarClass(HtmlElement Element, string Tag, string ClassName)
	{
		List<HtmlElement> list = new List<HtmlElement>();
		foreach (HtmlElement item in Element.GetElementsByTagName(Tag))
		{
			if (item.GetAttribute("className").IndexOf(ClassName) > -1)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static List<HtmlElement> GetHTMLElementsCollection(WebBrowser webBrowser, string Tag)
	{
		List<HtmlElement> list = new List<HtmlElement>();
		foreach (HtmlElement item in webBrowser.Document.GetElementsByTagName(Tag))
		{
			list.Add(item);
		}
		return list;
	}

	public static List<HtmlElement> GetHTMLElementsByName(WebBrowser webBrowser, string Tag, string Name)
	{
		List<HtmlElement> list = new List<HtmlElement>();
		foreach (HtmlElement item in webBrowser.Document.GetElementsByTagName(Tag))
		{
			if (item.GetAttribute("name") == Name)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static List<HtmlElement> GetHTMLElementsByName(HtmlElement Element, string Tag, string Name)
	{
		List<HtmlElement> list = new List<HtmlElement>();
		foreach (HtmlElement item in Element.GetElementsByTagName(Tag))
		{
			if (item.GetAttribute("name") == Name)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static List<HtmlElement> GetHTMLElementsByAttribute(HtmlElement Element, string Tag, string Attribute, string Name)
	{
		List<HtmlElement> list = new List<HtmlElement>();
		foreach (HtmlElement item in Element.GetElementsByTagName(Tag))
		{
			if (item.GetAttribute(Attribute) == Name)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static List<HtmlElement> GetHTMLElementsBySimilarAttribute(HtmlElement Element, string Tag, string Attribute, string Name)
	{
		List<HtmlElement> list = new List<HtmlElement>();
		foreach (HtmlElement item in Element.GetElementsByTagName(Tag))
		{
			if (item.GetAttribute(Attribute).IndexOf(Name) > -1)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static List<HtmlElement> GetHTMLElementsByNameLike(WebBrowser webBrowser, string Tag, string Name)
	{
		List<HtmlElement> list = new List<HtmlElement>();
		foreach (HtmlElement item in webBrowser.Document.GetElementsByTagName(Tag))
		{
			if (item.GetAttribute("name").IndexOf(Name) > -1)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static HtmlElement GetHTMLElementById(WebBrowser webBrowser, string Tag, string Id)
	{
		foreach (HtmlElement item in webBrowser.Document.GetElementsByTagName(Tag))
		{
			if (item.GetAttribute("id") == Id)
			{
				return item;
			}
		}
		return null;
	}

	public static List<HtmlElement> GetHTMLElementsByType(WebBrowser webBrowser, string Tag, string Type)
	{
		List<HtmlElement> list = new List<HtmlElement>();
		foreach (HtmlElement item in webBrowser.Document.GetElementsByTagName(Tag))
		{
			if (item.GetAttribute("type") == Type)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static List<string[]> ParseHTML(string HTML, string Template)
	{
		List<string[]> list = new List<string[]>();
		Match match = Regex.Match(HTML, Template);
		while (match.Success)
		{
			string[] array = new string[match.Groups.Count];
			for (int i = 0; i < match.Groups.Count; i++)
			{
				array[i] = match.Groups[i].Value;
			}
			list.Add(array);
			match = match.NextMatch();
		}
		return list;
	}

	public static string ClearString(string Source)
	{
		char[] array = Source.ToCharArray();
		char[] array2 = new char[3] { '\n', '\r', '\t' };
		for (int i = 0; i < Source.Length - 1; i++)
		{
			if (Source[i] == ' ' && Source[i + 1] == ' ')
			{
				array[i] = '*';
				array[i + 1] = '*';
			}
			for (int j = 0; j < array2.Length; j++)
			{
				if (array[i] == array2[j])
				{
					array[i] = '*';
				}
			}
		}
		return new string(array).Replace("*", "");
	}

    //public static Bitmap GetImage(WebBrowser wb, string ImageId)
    //{
    //    IHTMLImgElement iHTMLImgElement = (IHTMLImgElement)wb.Document.GetElementById(ImageId).DomElement;
    //    IHTMLElementRenderFixed iHTMLElementRenderFixed = (IHTMLElementRenderFixed)iHTMLImgElement;
    //    Bitmap bitmap = new Bitmap(iHTMLImgElement.width, iHTMLImgElement.height);
    //    Graphics graphics = Graphics.FromImage(bitmap);
    //    IntPtr hdc = graphics.GetHdc();
    //    iHTMLElementRenderFixed.DrawToDC(hdc);
    //    graphics.ReleaseHdc(hdc);
    //    return bitmap;
    //}
}
