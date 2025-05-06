using MaterialSkin;
using MaterialSkin.Controls;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WASender;

namespace ScheduleChecker
{
    public class Utils
    {

       
        public static MaterialSkin.MaterialSkinManager SetColorScheme(MaterialSkin.MaterialSkinManager materialSkinManager, MaterialForm materialForm, string colorSchemeenum)
        {
            //currentColorscheme = colorSchemeenum;

            materialSkinManager = MaterialSkin.MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(materialForm);
            materialSkinManager.Theme = MaterialSkin.MaterialSkinManager.Themes.LIGHT;
            if (colorSchemeenum == null || colorSchemeenum =="")
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Green700, Primary.Green400, Primary.Green900, Accent.Green700, TextShade.WHITE);
                return materialSkinManager;
            }
            if (colorSchemeenum == "Green")
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Green700, Primary.Green400, Primary.Green900, Accent.Green700, TextShade.WHITE);
            }
            else if (colorSchemeenum == "Red")
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Red700, Primary.Red400, Primary.Red900, Accent.Red700, TextShade.WHITE);
            }
            else if (colorSchemeenum == "Amber")
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Amber700, Primary.Amber400, Primary.Amber900, Accent.Amber700, TextShade.WHITE);
            }
            else if (colorSchemeenum == "Blue")
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Blue700, Primary.Blue400, Primary.Blue900, Accent.Blue700, TextShade.WHITE);
            }
            else if (colorSchemeenum == "BlueGrey")
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey700, Primary.BlueGrey400, Primary.BlueGrey900, Accent.Blue700, TextShade.WHITE);
            }
            else if (colorSchemeenum == "Brown")
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Brown700, Primary.Brown400, Primary.Brown900, Accent.Blue700, TextShade.WHITE);
            }
            else if (colorSchemeenum == "Cyan")
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Cyan700, Primary.Cyan400, Primary.Cyan900, Accent.Cyan700, TextShade.WHITE);
            }
            else if (colorSchemeenum == "DeepOrange")
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.DeepOrange700, Primary.DeepOrange400, Primary.DeepOrange900, Accent.DeepOrange700, TextShade.WHITE);
            }
            else if (colorSchemeenum == "DeepPurple")
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.DeepPurple700, Primary.DeepPurple400, Primary.DeepPurple900, Accent.DeepPurple700, TextShade.WHITE);
            }
            else if (colorSchemeenum == "Grey")
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Grey700, Primary.Grey400, Primary.Grey900, Accent.Blue700, TextShade.WHITE);
            }
            else if (colorSchemeenum == "Indigo")
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo700, Primary.Indigo400, Primary.Indigo900, Accent.Indigo700, TextShade.WHITE);
            }
            else if (colorSchemeenum == "LightBlue")
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.LightBlue700, Primary.LightBlue400, Primary.LightBlue900, Accent.LightBlue700, TextShade.WHITE);
            }
            else if (colorSchemeenum == "Lime")
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Lime700, Primary.Lime400, Primary.Lime900, Accent.Lime700, TextShade.WHITE);
            }
            else if (colorSchemeenum == "Orange")
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Orange700, Primary.Orange400, Primary.Orange900, Accent.Orange700, TextShade.WHITE);
            }
            else if (colorSchemeenum == "Pink")
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Pink700, Primary.Pink400, Primary.Pink900, Accent.Pink700, TextShade.WHITE);
            }
            else if (colorSchemeenum == "Purple")
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Purple700, Primary.Purple400, Primary.Purple900, Accent.Purple700, TextShade.WHITE);
            }
            else if (colorSchemeenum == "Teal")
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Teal700, Primary.Teal400, Primary.Teal900, Accent.Teal700, TextShade.WHITE);
            }
            else if (colorSchemeenum == "Yellow")
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Yellow700, Primary.Yellow400, Primary.Yellow900, Accent.Yellow700, TextShade.WHITE);
            }

            return materialSkinManager;
        }
    }
}
