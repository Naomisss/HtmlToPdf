using System;
using System.Collections.Generic;
using System.Xml;

namespace HtmlToPdfLib
{
    public class LocalizationXmlParser
    {
        private Dictionary<string, string> inputNodes;
        private const string localizationXmlPath = @"SystemData\xml\Localization.xml";
        private const string inputTextXmlPath = @"SystemData\xml\InputText.xml";


        public string DefautLanguage(/*string xmlPath*/)
        {
            XmlDocument xmlDoc = new XmlDocument();
            string defaultLang = "";

            xmlDoc.Load(localizationXmlPath);
            XmlElement xRoot = xmlDoc.DocumentElement;

            XmlNode nodeDefaultLanguage = xRoot.SelectSingleNode("settings/defaultLang");
            XmlNode nodeSomeLanguage = xRoot.SelectSingleNode("settings/languages/lang");
            XmlNodeList childNodes = xRoot.SelectNodes("settings/languages/lang");

            if (String.IsNullOrEmpty(nodeDefaultLanguage.InnerXml))
            {
                defaultLang = nodeSomeLanguage.InnerText;
            }
            else
            {
                foreach (XmlNode n in childNodes)
                {
                    if (nodeDefaultLanguage.InnerText == n.InnerText)
                    {
                        defaultLang = nodeDefaultLanguage.InnerText;
                        return defaultLang;
                    }
                    else
                    {
                        defaultLang = nodeSomeLanguage.InnerText;
                    }
                }
            }


            return defaultLang;
        }

        public string[] SearchLanguages(string xmlPath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
            XmlElement xRoot = xmlDoc.DocumentElement;
            XmlNodeList childNodes = xRoot.SelectNodes("settings/languages/lang");

            int capacity = childNodes.Count;

            string[] strArr = new string[capacity];

            if(strArr.Length != null)
            {
                for (int i = 0; i < capacity; i++)
                {
                    strArr[i] = childNodes[i].InnerXml;
                }
                return strArr;
            }
            else
            {
                return new string[] { "ru", "en" };
            }
        }

        public string GetNewLanguage(/*string xmlPathInputText, string xmlPathLocalization,*/ string lang)
        {
            inputNodes = new Dictionary<string, string>();
            var key = "";
            var value = "";

            var defLang = DefautLanguage(/*xmlPathLocalization*/);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(/*xmlPathInputText*/inputTextXmlPath);
            XmlElement xRoot = xmlDoc.DocumentElement;
            XmlNodeList localizationChildNodes = xRoot.SelectNodes("localization/node");

            if (localizationChildNodes != null)
            {
                foreach (XmlNode node in localizationChildNodes)
                {
                    try
                    {
                        key = node.SelectSingleNode("@id").Value;
                        value = node.SelectSingleNode("textItem[@lang='" + lang + "']").InnerText;
                        inputNodes.Add(key, value);
                    }
                    catch (Exception e)
                    {
                        GetNewLanguage(/*xmlPathInputText, xmlPathLocalization,*/ defLang);
                    }
                }

                return lang;
            }
            else
            {
                return "no xml nodes";
            }
            
        }

        public string GetLocalization(string key)
        {
            return inputNodes[key];
        }
    }
}
