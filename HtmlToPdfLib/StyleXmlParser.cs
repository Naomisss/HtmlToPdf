using System;
using System.Collections.Generic;
using System.Xml;
using HtmlAgilityPack;

namespace HtmlToPdfLib
{
    public class StyleXmlParser
    {
        private struct AttributesOfTags
        {
            public string attributeName;
            public string valueOfAttrribute;
        }

        private struct AttributesOfInsertTag
        {
            public string tagName;
            public List<AttributesOfTags> attributes;
        }

        private const string styleXmlPath = @"SystemData\xml\StyleXML.xml";

        private Dictionary<string, List<AttributesOfTags>> styleOfTagsReplase;
        private List<AttributesOfTags> listOfAttr;
        private AttributesOfTags tmpNodeAttribute;
        private AttributesOfInsertTag AttrForInsert;



        public StyleXmlParser(/*string xmlPath*/)
        {
            styleOfTagsReplase = new Dictionary<string, List<AttributesOfTags>>();
            tmpNodeAttribute = new AttributesOfTags();
            AttrForInsert = new AttributesOfInsertTag();

            var tagName = "";
            var tagAttrributeName = "";
            var tagAttributeValue = "";

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(styleXmlPath);
                XmlElement xRoot = xmlDoc.DocumentElement;

                if (xRoot != null)
                {
                    if (xRoot.SelectNodes("tag") != null)
                    {
                        XmlNodeList childenodes = xRoot.SelectNodes("tag");

                        if (childenodes != null)
                        {
                            int capacity = childenodes.Count;
                            for (int i = 0; i < capacity; i++)
                            {
                                XmlNode tmpNode = childenodes[i];
                                listOfAttr = new List<AttributesOfTags>();

                                if (tmpNode.Attributes != null)
                                {
                                    tagName = tmpNode.Attributes["name"].Value;

                                    if (tmpNode.Attributes["class"] == null || tmpNode.Attributes["class"].Value != "insert")
                                    {
                                        for (int j = 0; j < tmpNode.ChildNodes.Count; j++)
                                        {
                                            if (tmpNode.ChildNodes[j].Attributes != null)
                                            {
                                                tagAttrributeName = tmpNode.ChildNodes[j].Attributes["name"].Value;
                                                tagAttributeValue = tmpNode.ChildNodes[j].Attributes["value"].Value;

                                                if (!String.IsNullOrEmpty(tagAttrributeName) && !String.IsNullOrEmpty(tagAttributeValue))
                                                {
                                                    tmpNodeAttribute.attributeName = tagAttrributeName;
                                                    tmpNodeAttribute.valueOfAttrribute = tagAttributeValue;

                                                    listOfAttr.Add(tmpNodeAttribute);
                                                }
                                            }
                                        }
                                        styleOfTagsReplase.Add(tagName, listOfAttr);
                                    }
                                    else
                                    {
                                        for (int j = 0; j < tmpNode.ChildNodes.Count; j++)
                                        {
                                            if (tmpNode.ChildNodes[j].Attributes != null)
                                            {
                                                tagAttrributeName = tmpNode.ChildNodes[j].Attributes["name"].Value;
                                                tagAttributeValue = tmpNode.ChildNodes[j].Attributes["value"].Value;

                                                if (!String.IsNullOrEmpty(tagAttrributeName) && !String.IsNullOrEmpty(tagAttributeValue))
                                                {
                                                    tmpNodeAttribute.attributeName = tagAttrributeName;
                                                    tmpNodeAttribute.valueOfAttrribute = tagAttributeValue;

                                                    listOfAttr.Add(tmpNodeAttribute);
                                                }
                                            }
                                        }
                                        AttrForInsert.tagName = tagName;
                                        AttrForInsert.attributes = listOfAttr;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public void FixCollectionsOfAttributes(HtmlDocument doc)
        {
            var styleReplaseDictionary = GetStyleForReplase();

            foreach (var nodeStyle in styleReplaseDictionary)
            {
                var str = nodeStyle.Key;
                var nodeCollection = doc.DocumentNode.SelectNodes(@"//" + str);

                if (nodeCollection != null)
                {
                    var key = "";

                    foreach (var node in nodeCollection)
                    {
                        key = node.Name;
                        if (styleReplaseDictionary.ContainsKey(key))
                        {
                            var tmpList = styleReplaseDictionary[key];
                            if (tmpList.Capacity != 0)
                            {
                                node.Attributes.RemoveAll();
                                foreach (var attribute in tmpList)
                                {
                                    node.SetAttributeValue(attribute.attributeName, attribute.valueOfAttrribute);
                                }
                            }
                            else
                            {
                                node.Attributes.RemoveAll();
                            }
                        }
                    }
                }
            }
        }

        public void SetAttributesOfTag(HtmlNode nodeHtml)
        {
            foreach (var attribute in AttrForInsert.attributes)
            {
                nodeHtml.SetAttributeValue(attribute.attributeName, attribute.valueOfAttrribute);
            }
        }

        private Dictionary<string, List<AttributesOfTags>> GetStyleForReplase()
        {
            return styleOfTagsReplase;
        }
    }
}
