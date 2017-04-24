﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using HtmlAgilityPack;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using TidyManaged;

using Document = iTextSharp.text.Document;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using System.Text.RegularExpressions;
using Image = iTextSharp.tool.xml.html.Image;

namespace HtmlToPdfLib
{
    public class CreatePdf
    {
        private struct ParentOfNodes
        {
            public HtmlNode parent;
            public HtmlNode child;
        }

//        private const string localizationXmlPath = @"SystemData\xml\Localization.xml";
//        private const string inputTextXmlPath = @"SystemData\xml\InputText.xml";
//        private const string styleXmlPath = @"SystemData\xml\StyleXML.xml";

        private const string inputFont = @"SystemData\font\font.ttf";/*@"SystemData\font\arial.ttf";*/
        private string fileNameCss = @"SystemData\style\style.css";

        private string curentDirectory = "";
        private string fileNamePDF = "";

        private string fileNameXML = "";
        private string tmpNameXML = @"Tree.xml";//вспомогательное значение


        public bool showDebugImgFlag = false;
        public bool duplexPrintingFlag = false;

        //private string defaultLang = "";
        public string selectedLang = "";

        static public BaseFont baseFont;
        Font font_14;
        Font fontBold_14;
        Font fontBold_16;
        Font fontBold_18;

        private TOCEvents ev;
        LocalizationXmlParser locXmlParser = new LocalizationXmlParser();
        StyleXmlParser styleXmlParser = new StyleXmlParser(/*styleXmlPath*/);

        XmlDocument readerXml = new XmlDocument();
        private XmlNode rootXmlNode;

        private Process proc;

        public CreatePdf()
        {
            baseFont = BaseFont.CreateFont(inputFont, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            font_14 = new Font(baseFont, 14, iTextSharp.text.Font.NORMAL);
            fontBold_14 = new Font(baseFont, 14, iTextSharp.text.Font.BOLD);
            fontBold_16 = new Font(baseFont, 16, iTextSharp.text.Font.BOLD);
            fontBold_18 = new Font(baseFont, 18, iTextSharp.text.Font.BOLD);



            //defaultLang = locXmlParser.DefautLanguage(/*localizationXmlPath*/);
//            locXmlParser.GetNewLanguage(/*inputTextXmlPath, localizationXmlPath,*/ selectedLang);
        }

        public void CreateNewPdf(string pathToPdf, string DirectoryPath)
        {
            locXmlParser.GetNewLanguage(selectedLang);
            ev = new TOCEvents(selectedLang);


            fileNamePDF = pathToPdf;
            curentDirectory = DirectoryPath;
            fileNameXML = curentDirectory + tmpNameXML;

            #region work with XML
            try
            {
                readerXml.Load(fileNameXML);
                rootXmlNode = readerXml.ChildNodes[0];
            }
            catch(Exception)
            {
                MessageBox.Show("Нет XML файла для HTML старниц ");
                return;
            }

            #endregion

            #region add pdf document
            FileStream stream;

            try
            {
                stream = new FileStream(fileNamePDF, FileMode.Create);
            }
            catch (Exception)
            {
                try
                {
                    stream = new FileStream(fileNamePDF, FileMode.Create);
                }
                catch (Exception)
                {
                    Console.WriteLine("Файл занят, закройте его ");
                    MessageBox.Show("Файл занят, закройте его ");
                    return;
                }
            }
            #endregion

            #region create PDF
            using (Document document = new Document(PageSize.A4, 72f, 72f, 36f,36 ))
            {
                PdfWriter writer = PdfWriter.GetInstance(document, stream);

                //Crete event
                writer.SetLinearPageMode();
                writer.PageEvent = ev;


                document.Open();
                CreateContent(rootXmlNode, document, writer, writer.RootOutline, "", 0);
                document.NewPage();


                //Create Table of Contents
                document.Add(new Paragraph(locXmlParser.GetLocalization("tableOfContent"), fontBold_18));
                ev.DrawFooter = false;


                //Reorgering page
                int pageStartToc = writer.PageNumber;
                int pageEndToc = CreateTOC(rootXmlNode, document, writer, "", 0, writer.RootOutline);
                if (duplexPrintingFlag)
                {
                    if (((pageEndToc - pageStartToc) + 1) % 2 == 1)//document.PageNumber % 2 == 1)
                    {
                        ev.DrawFooter = false;

                        document.NewPage();
                        pageEndToc = pageEndToc + 1;
                        document.Add(new Paragraph(" "));
                    }
                }
                ReorderPagesWithTableOfContent(document, writer, pageStartToc, pageEndToc);//////

                //ReorderPage(document, writer, pageStartToc, tocPages);//поправить (from page to page)

                document.Close();
            }
            #endregion
        }

        private void CreateContent(XmlNode xmlNode, Document doc, PdfWriter writer, PdfOutline pdfBookmark, string str, int counterOfChapter)
        {
/*            #region Font
            FontFactory.Register(inputFont, "TrebucheteFont");

            StyleSheet ST = new StyleSheet();

            ST.LoadTagStyle(HtmlTags.BODY, HtmlTags.FACE, "TrebucheteFont");
            ST.LoadTagStyle(HtmlTags.BODY, HtmlTags.ENCODING, BaseFont.IDENTITY_H);

            HTMLWorker worker = new HTMLWorker(doc);
            worker.SetStyleSheet(ST);

            #endregion*/

            if (xmlNode != null)
            {
                Chunk chunk;
                Paragraph paragraph;
                string nodeText = "";
                string nodeType = "";
                string nodePath = "";

                string numOfChupter = "";
                string textOfChapter = "";

                bool flagPath = false;

                PdfOutline bookmark;

                PdfContentByte cb = writer.DirectContent;

                PdfPTable table;
                PdfPCell cell;


                for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
                {
                    nodeText = xmlNode.ChildNodes[i].Attributes["text"].Value;
                    nodeType = xmlNode.ChildNodes[i].Attributes["type"].Value;
                    nodePath = /*@"Data\"*/curentDirectory + xmlNode.ChildNodes[i].Attributes["path"].Value;

                    numOfChupter = str + (i + 1) + ".";
                    textOfChapter = numOfChupter + " " + nodeText;

                    if (!String.IsNullOrEmpty(nodePath))
                        flagPath = true;
                    else
                        flagPath = false;

                    float tmpPositionY = 0f;
                    float tmpLimit = 560f;

                    if (!String.IsNullOrEmpty(nodeText))
                    {
                        switch (nodeType)
                        {
                            case "book":
//                                ev.DrawHeader = false;
//                                cb.BeginText();
//                                cb.SetFontAndSize(baseFont, 16);
//                                cb.SetTextMatrix(doc.PageSize.Width / 2f, doc.PageSize.Height / 2f);
//                                cb.ShowText(nodeText);
//                                cb.EndText();

//                                doc.NewPage();
                                doc.NewPage();
                                ev.DrawFooter = false;

                                table = new PdfPTable(1);
                                table.WidthPercentage = 100;

                                cell = new PdfPCell();
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
                                cell.MinimumHeight = doc.PageSize.Height - (doc.BottomMargin + doc.TopMargin);
                                cell.Border = 0;

                                chunk = new Chunk(nodeText, fontBold_18);

                                paragraph = new Paragraph(chunk);
                                paragraph.Alignment = Element.ALIGN_CENTER;

                                cell.AddElement(paragraph);
                                table.AddCell(cell);
                                doc.Add(table);

                                doc.NewPage();

                                if(duplexPrintingFlag)
                                {
                                    if(writer.PageNumber % 2 == 0)
                                    {
                                        
                                        doc.Add(new Paragraph(" "));
                                        doc.NewPage();
                                    }
                                }


                                break;

                            case "part":
                                doc.NewPage();
                                ev.DrawFooter = false;

                                if (duplexPrintingFlag)/////////////////////////
                                {
                                    if ((writer.PageNumber) % 2 == 0)
                                    {
                                        doc.Add(new Paragraph(" "));
                                        doc.NewPage();
                                    }
                                }

//                                chunk = new Chunk(nodeText, fontBold_18);
//                                chunk.SetGenericTag(textOfChapter);
//                                paragraph = new Paragraph(chunk);
//                                doc.Add(paragraph);

                                table = new PdfPTable(1);
                                table.WidthPercentage = 100;

                                cell = new PdfPCell();
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
                                cell.MinimumHeight = doc.PageSize.Height - (doc.BottomMargin + doc.TopMargin);
                                cell.Border = 0;


                                chunk = new Chunk(nodeText, fontBold_18);
                                chunk.SetGenericTag(textOfChapter);

                                paragraph = new Paragraph(chunk);
                                paragraph.Alignment = Element.ALIGN_CENTER;


//                                cell.AddElement(chunk);
                                cell.AddElement(paragraph);
                                table.AddCell(cell);
                                doc.Add(table);

                                doc.NewPage();

                                if (duplexPrintingFlag)
                                {
                                    if ((writer.PageNumber) % 2 == 0)
                                    {
                                        
                                        doc.Add(new Paragraph(" "));
                                        doc.NewPage();
                                    }
                                }

                                break;

                            case "chapter":
                                ev.DrawFooter = true;

                                tmpPositionY = writer.GetVerticalPosition(false);

                                if (/*(writer.PageSize.Height - tmpPositionY)*/tmpPositionY < tmpLimit)
                                {
                                    doc.NewPage();
                                }

                                chunk = new Chunk(textOfChapter, fontBold_16);
                                chunk.SetGenericTag(textOfChapter);
                                paragraph = new Paragraph(chunk);
                                doc.Add(paragraph);

                                if (flagPath)
                                {
                                    addHtmlPage(doc, writer, nodePath, i + 1, str);
                                }

                                break;
                            case "subject":
                            case "frame":
                                ev.DrawFooter = true;

                                tmpPositionY = writer.GetVerticalPosition(false);

                                if (/*(writer.PageSize.Height - tmpPositionY)*/tmpPositionY < tmpLimit)
                                {
                                    doc.NewPage();
                                }

                                chunk = new Chunk(textOfChapter, fontBold_14);
                                chunk.SetGenericTag(textOfChapter);
                                paragraph = new Paragraph(chunk);
                                doc.Add(paragraph);

                                if (flagPath)
                                {
                                    addHtmlPage(doc, writer, nodePath, i + 1, str);
                                }

                                break;

//                            case "frame":
//                                ev.DrawFooter = true;
//
//                                chunk = new Chunk(textOfChapter, fontBold_14);
//                                chunk.SetGenericTag(textOfChapter);
//                                paragraph = new Paragraph(chunk);
//                                doc.Add(paragraph);
//
//                                addHtmlPage(doc, writer, nodePath, i + 1, str);
//                                break;
                        }
                    }

                    if (nodeType != "book")
                    {
                        CreateContent(xmlNode.ChildNodes[i], doc, writer, pdfBookmark, numOfChupter, counterOfChapter + 1);
                    }
                    else
                    {
                        CreateContent(xmlNode.ChildNodes[i], doc, writer, pdfBookmark, str,
                                      counterOfChapter + 1);
                    }
                }
            }
            else
            {
                Console.WriteLine(locXmlParser.GetLocalization("noXML"));
            }
        }
        private int CreateTOC(XmlNode xmlNode, Document doc, PdfWriter writer, string str, int level, PdfOutline pdfBookmark)
        {
            PdfOutline bookmark;
            string nodeText = "";
            string nodeType = "";
            //string nodePath = "";
            var numOfChupter = "";


            var toc = ev.GetTOC();
            KeyValuePair<string, int> value;

            Chunk dottedLine = new Chunk(new iTextSharp.text.pdf.draw.DottedLineSeparator());

            for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
            {
                nodeText = xmlNode.ChildNodes[i].Attributes["text"].Value;
                nodeType = xmlNode.ChildNodes[i].Attributes["type"].Value;
                numOfChupter = str + (i + 1) + ". ";

                if (nodeType != "book")
                {
                    var key = numOfChupter + nodeText;
                    value = toc[key];

                    var dest = value.Key;
                    var page = value.Value;

                    //number of chapter
                    var cN = new Chunk(numOfChupter, font_14);
                    cN.SetAction(PdfAction.GotoLocalPage(dest, false));
                    var p = new Paragraph(cN);
                    //text
                    var c = new Chunk(nodeText, font_14);
                    c.SetAction(PdfAction.GotoLocalPage(dest, false));
                    p.Add(c);
                    //indent and dottedLine
                    p.IndentationLeft = 10 * level;
                    p.Add(dottedLine);
                    //page
                    c = new Chunk(page.ToString(), font_14);
                    c.SetAction(PdfAction.GotoLocalPage(dest, false));
                    p.Add(c);

                    doc.Add(p);

                    bookmark = createBookmark(pdfBookmark, xmlNode.ChildNodes[i], writer,
                                     numOfChupter, ev.GetTOC());

                    CreateTOC(xmlNode.ChildNodes[i], doc, writer, str + (i + 1) + ".", level + 1, bookmark);
                }
                else
                {
                    CreateTOC(xmlNode.ChildNodes[i], doc, writer, "", level, pdfBookmark);
                }
            }
            return writer.PageNumber;
        }
        private PdfOutline createBookmark(PdfOutline pdfBookmark, XmlNode xmlNode, PdfWriter writer,
                                  string str, Dictionary<string, KeyValuePair<string, int>> toc)
        {
            var text = xmlNode.Attributes["text"].Value;
            KeyValuePair<string, int> value;

            var key = "";
            key = str + text;

            if (toc.ContainsKey(key))
            {
                value = toc[key];
                var page = value.Value;
                var bookmark = new PdfOutline(pdfBookmark,
                                              PdfAction.GotoLocalPage(page,
                                              new PdfDestination(PdfDestination.FITH),
                                              writer), str + " " + text, true);

                return bookmark;
            }
            else
            {
                var bookmark = new PdfOutline(pdfBookmark,
                                              PdfAction.GotoLocalPage(1,
                                              new PdfDestination(PdfDestination.FITH),
                                              writer), "no bookmark*** " + str + " " + text, true);
                return bookmark;
            }
        }

        private void ReorderPagesWithTableOfContent(Document doc, PdfWriter writer, int tocPage, int endTocPage)
        {
            doc.NewPage();
            // get the total number of pages that needs to be reordered
            int total = writer.ReorderPages(null);

            // change the order
            int[] order = new int[total];

            for (int i = 0; i < total; i++)
            {
                if (duplexPrintingFlag)
                {
                    if (i == 0)
                    {
                        order[i] = 1;
                    }

                    if (i == 1)
                    {
                        order[i] = 2;
                    }

                    if (i == 2)
                    {
                        order[i] = tocPage;
                    }

                    if (i > 2)
                    {
                        order[i] = (i - 2) + tocPage;

                        if (order[i] > total)
                        {
                            order[i] -= total - 2;
                        }

                    }

                    /*if (i > endTocPage - tocPage)
                    {

                    }

                    if (i == total - 1)
                    {
                        Console.WriteLine("");
                    }*/
                }
                else
                {
                    if (i == 0)
                    {
                        order[i] = 1;
                    }

                    if (i == 1)
                    {
                        order[i] = tocPage;
                    }

                    if (i > 1)
                    {
                        order[i] = (i - 1) + tocPage;

                        if (order[i] > total)
                        {
                            order[i] -= total - 1;
                        }

                    }

                    if (i > endTocPage - tocPage)
                    {

                    }

                    /*if (i == total - 1)
                    {
                        Console.WriteLine("");
                    }*/
                }
                
            }
            // apply the new order
            writer.ReorderPages(order);
        }

        private void addHtmlPage(Document doc,/* HTMLWorker worker,*/ PdfWriter writer, string path, int counter, string str)
        {
            var extension = System.IO.Path.GetExtension(path);

            if (!String.IsNullOrEmpty(path) && (extension == ".htm" | extension == ".html"))
            {


//                try
//                {



                    ////////
//                    string dirtyHtml = FixHtmlFile(path, counter, str);
//                    string cleanHtml = "";

                

                //cleanHtml = PrepareTidyHtml(dirtyHtml);
    /*                    using (TidyManaged.Document docHtmlQWERTY = TidyManaged.Document.FromString(dirtyHtml))
                {
    //                        docHtml.OutputBodyOnly = AutoBool.Yes;
    //                        docHtml.Quiet = true;
    //                        docHtml.CleanAndRepair();
    //                        cleanHtml = docHtml.Save();

                    //Encoding srcEncoding = Encoding.GetEncoding(dirtyHtml);
    //                        byte[] srcEncodingBytes = srcEncoding.GetBytes(dirtyHtml);
    //                        Encoding destEncoding = Encoding.UTF8;
    //                        byte[] destEncodingBytes = Encoding.Convert(srcEncoding, destEncoding, srcEncodingBytes);
    //                        var strStream = new MemoryStream(destEncodingBytes);

                    byte[] bytes = Encoding.Default.GetBytes(dirtyHtml);
                    byte[] destEncodingBytes = Encoding.Convert(Encoding.Default, Encoding.UTF8, bytes);
                    dirtyHtml = System.Text.Encoding.UTF8.GetString(destEncodingBytes);
                    var strStream = new MemoryStream(destEncodingBytes);

                    //do the parsing
                    var docHtml = TidyManaged.Document.FromStream(strStream);
                    docHtml.InputCharacterEncoding = TidyManaged.EncodingType.Utf8;
                    docHtml.OutputCharacterEncoding = TidyManaged.EncodingType.Utf8;
                    docHtml.CharacterEncoding = TidyManaged.EncodingType.Utf8;
                    docHtml.ShowWarnings = false;
                    docHtml.Quiet = true;
                    docHtml.OutputXhtml = true;
                    docHtml.CleanAndRepair();
                    cleanHtml = docHtml.Save();
                }*/

                string cleanHtml = FixHtmlFile(path, counter, str);
                string css = File.ReadAllText(fileNameCss);


                var msCss = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(css));
                var msHtml = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(cleanHtml));

                FontFactory.Register(inputFont);

                XMLWorkerFontProvider fontProvider = new XMLWorkerFontProvider(XMLWorkerFontProvider.DONTLOOKFORFONTS);
                fontProvider.Register(inputFont);
                FontFactory.FontImp = fontProvider;

                XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msHtml, msCss, Encoding.UTF8, fontProvider);


/*                var cssRsolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(true);
                HtmlPipelineContext htmlContext = new HtmlPipelineContext(null);
                htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());
                htmlContext.SetImageProvider( );*/


                /////////////

                /*XMLWorkerFontProvider fontProvider = new XMLWorkerFontProvider(XMLWorkerFontProvider.DONTLOOKFORFONTS);
                    fontProvider.Register(inputFont);
                    FontFactory.FontImp = fontProvider;

                    CssAppliers cssAppliers = new CssAppliersImpl(fontProvider);

                    HtmlPipelineContext htmlContext = new HtmlPipelineContext(cssAppliers);

                    htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());

                    ICSSResolver cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(true);

                    IPipeline pipeline = new CssResolverPipeline(cssResolver, new HtmlPipeline(htmlContext, new PdfWriterPipeline(doc, writer)));

                    XMLWorker worker = new XMLWorker(pipeline, true);

                    XMLParser p = new XMLParser(true, worker, Encoding.UTF8);

                    TextReader reader = new StringReader(cleanHtml);

                    p.Parse(reader);

                    p.Flush();*/






                /////////
//                    worker.StartDocument();
//                    worker.Parse(new StringReader(FixHtmlFile(path, counter, str)));





//                }
//                catch (Exception ex)
//                {
//                    Chunk dl = new Chunk(new iTextSharp.text.pdf.draw.DottedLineSeparator());
//                    doc.Add(dl);
//                    doc.Add(new Paragraph(ex.Message, font_14));
//                    doc.Add(dl);
//                }






//                finally
//                {
//                    worker.EndDocument();
//                    worker.Close();
//                    doc.NewPage();
//                }
            }
            else if (extension == ".jpg")
            {
                iTextSharp.text.Image img =
                    iTextSharp.text.Image.GetInstance(path);

                if (img.Height > img.Width)
                {
                    //Maximum height is 800 pixels.
                    float percentage = 0.0f;
                    percentage = 700 / img.Height;
                    img.ScalePercent(percentage * 100);
                }
                else
                {
                    //Maximum width is 600 pixels.
                    float percentage = 0.0f;
                    percentage = 540 / img.Width;
                    img.ScalePercent(percentage * 100);
                }
                img.Border = iTextSharp.text.Rectangle.BOX;
                img.BorderColor = iTextSharp.text.BaseColor.BLACK;
                img.BorderWidth = 3f;
                doc.Add(img);
            }
        }
        private string FixHtmlFile(string htmlPath, int counterOfChapter, string str)
        {
            HtmlDocument doc = new HtmlDocument();
            string content = "";


//            if ((htmlPath == ("Data\\" + "Frames/Ru/MIG-000085544.htm"))) //|| (htmlPath == ("Data\\" + "Frames/Ru/MIG-000085662.htm")))//debug if
//            {

            //read html
            try
            {
                //debug


                content = File.ReadAllText(htmlPath);//без изменений
                doc.LoadHtml(content);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            List<ParentOfNodes> listImg = new List<ParentOfNodes>();

            RemoveStyleAttributes(doc);

            //чистим и добавляем нужные атрибуты к тегам в html
            styleXmlParser.FixCollectionsOfAttributes(doc);

            content = doc.DocumentNode.OuterHtml;


            var patternST1 = @"(</?st1\b?.*?>)";
            var patternO_P = @"(<)(/?)(o:p)\b?.*?(>)";
//            var patternLI = @"(<li>)(<div>)(.*?)(</div>)(</li>)";

            var replacementST1 = "";
            var replacementO_P = "$1p$4";
//            var replacementLi = "$1$3$5";
            //Regex regexST = new Regex(patternST1);
            //Match match = regex.Match(doc.DocumentNode.OuterHtml);

            content = Regex.Replace(content, patternST1, replacementST1);
            content = Regex.Replace(content, patternO_P, replacementO_P);
//            content = Regex.Replace(content, patternLI, replacementLi);////////
            //            content = Regex.Replace(content, pattern1, replacement);
            //            content = Regex.Replace(content, pattern2, replacement);
            //                }

            var cleanupContent = PrepareTidyHtml(content);
            content = cleanupContent;

            doc.LoadHtml(content);

            //            }//////////?


            HtmlNode nodeBody = doc.DocumentNode.SelectSingleNode("//body");
            HtmlNodeCollection nodeCollectionA = doc.DocumentNode.SelectNodes("//a[@class='abc-picture']");

            HtmlNodeCollection nodeCollectionImg = doc.DocumentNode.SelectNodes("//img");

            HtmlNodeCollection nodeCollectionH1 = doc.DocumentNode.SelectNodes("//h1");
            HtmlNodeCollection nodeCollectionH2 = doc.DocumentNode.SelectNodes("//h2");
            HtmlNodeCollection nodeCollectionH3 = doc.DocumentNode.SelectNodes("//h3");
            HtmlNodeCollection nodeCollectionH4 = doc.DocumentNode.SelectNodes("//h4");
            HtmlNodeCollection nodeCollectionH5 = doc.DocumentNode.SelectNodes("//h5");

            #region Colection h1,2,3,4,5
            if (nodeCollectionH1 != null)
            {
                prepareHNodeCollection(nodeCollectionH1, "divH1", doc);
            }
            if (nodeCollectionH2 != null)
            {
                prepareHNodeCollection(nodeCollectionH2, "divH2", doc);
            }
            if (nodeCollectionH3 != null)
            {
                prepareHNodeCollection(nodeCollectionH3, "divH3", doc);
            }
            if (nodeCollectionH4 != null)
            {
                prepareHNodeCollection(nodeCollectionH4, "divH4", doc);
            }
            if (nodeCollectionH5 != null)
            {
                prepareHNodeCollection(nodeCollectionH5, "divH5", doc);
            }
            #endregion


            HtmlNodeCollection nodeCollectionDivBlockAlert = doc.DocumentNode.SelectNodes("//div[@class='blockAlert']");
            HtmlNodeCollection nodeCollectionDivBlockAttention = doc.DocumentNode.SelectNodes("//div[@class='blockAttention']");
            HtmlNodeCollection nodeCollectionDivBlockQuote = doc.DocumentNode.SelectNodes("//div[@class='blockQuote']");
            HtmlNodeCollection nodeCollectionDivBlockInfo = doc.DocumentNode.SelectNodes("//div[@class='blockInfo']");
            HtmlNodeCollection nodeCollectionDivBlockNote = doc.DocumentNode.SelectNodes("//div[@class='blockNote']");

            #region Colection DivBlock
            if (nodeCollectionDivBlockAlert != null)
            {
                prepareDivBloc(nodeCollectionDivBlockAlert, doc);
            }
            if (nodeCollectionDivBlockAttention != null)
            {
                prepareDivBloc(nodeCollectionDivBlockAttention, doc);
            }
            if (nodeCollectionDivBlockQuote != null)
            {
                prepareDivBloc(nodeCollectionDivBlockQuote, doc);
            }
            if (nodeCollectionDivBlockInfo != null)
            {
                prepareDivBloc(nodeCollectionDivBlockInfo, doc);
            }
            if (nodeCollectionDivBlockNote != null)
            {
                prepareDivBloc(nodeCollectionDivBlockNote, doc);
            }
            #endregion

            if (nodeBody != null)
            {
                string textOfImg = locXmlParser.GetLocalization("img") + " " + str + counterOfChapter;
                var tmpNode = AddPicture(nodeBody, doc, textOfImg);


                //nodeBody.Attributes.RemoveAll();
            }
            if (nodeCollectionImg != null)
            {
                foreach (var htmlNode in nodeCollectionImg)
                {
                    var tmpPath = /*@"Data/"*/curentDirectory + htmlNode.Attributes["path"].Value;

                    tmpPath = tmpPath.Replace(@"/", @"\");

                    if (!String.IsNullOrEmpty(tmpPath) && File.Exists(tmpPath))
                    {
                        var uri = new System.Uri(tmpPath);
                        var convertedURI = uri.AbsoluteUri;

                        //htmlNode.Attributes["path"].Value = Path.GetFullPath(tmpPath);
                        htmlNode.SetAttributeValue("src", convertedURI);
                        htmlNode.Attributes["path"].Remove();

                        int heightImg;
                        int widthImg;
                        int minLimit = 64;

                        try
                        {

                            using (System.Drawing.Image img = System.Drawing.Image.FromFile(tmpPath))//.Drawing.Image objImage = System.Drawing.Image.FromFile(FileName))
                            {
                                widthImg = img.Width;
                                heightImg = img.Height;
                            }

                            if ((widthImg < minLimit) || (heightImg < minLimit))
                            {
                                if (widthImg == heightImg)
                                {
                                    htmlNode.SetAttributeValue("class", "icon");
                                }
                                else
                                {
                                    htmlNode.SetAttributeValue("class", "picture-text");
                                }
                            }
                            else
                            {
                                htmlNode.SetAttributeValue("class", "ui-chapter-picture_abc-standart");
                            }
                        }
                        catch(Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }

                        

                    }
                    else
                    {
                        HtmlNode pNode = doc.CreateElement("p");
                        pNode.InnerHtml = "no Img";
                        htmlNode.ParentNode.ReplaceChild(pNode, htmlNode);
                    }

                    
                }
            }
            if (nodeCollectionA != null)
            {
                int counterOfImg = 1;
                string textOfImg = "";

                foreach (var node in nodeCollectionA)
                {
                    textOfImg = locXmlParser.GetLocalization("img") + " " + str + counterOfChapter + "." + counterOfImg;
                    listImg.Add(AddPicture(node, doc, textOfImg));
                    counterOfImg++;
                }

                for (int i = listImg.Count - 1; i >= 0; i--)
                {
                    var parentNode = listImg[i].parent;
                    if (parentNode != null)
                    {
                        if (parentNode.ParentNode != null)
                        {

                            parentNode.ParentNode.InsertAfter(listImg[i].child, parentNode);

                            

                        }
                        else
                        {
                            Console.WriteLine(htmlPath + ": " + listImg[i].parent.ToString() + " " +
                                listImg[i].child.ToString());

                            Console.WriteLine(doc.DocumentNode.OuterHtml);
                        }

                        //Console.WriteLine(doc.DocumentNode.OuterHtml);
                    }
                }
            }

            HtmlNodeCollection nodeCollectionIcon = doc.DocumentNode.SelectNodes("//img[@class='icon']");
            HtmlNodeCollection nodeCollectionPictext = doc.DocumentNode.SelectNodes("//img[@class='picture-text']");
            if(nodeCollectionIcon != null)
            {
                foreach (var node in nodeCollectionIcon)
                {
                    prepareIconColumn(node);
                }
            }
            if (nodeCollectionPictext != null)
            {
                foreach (var node in nodeCollectionPictext)
                {
                    prepareIconColumn(node);
                }
            }


//            var nodeCollectiondivH3 = doc.DocumentNode.SelectNodes("//div[@class='divH3']");
//            if (nodeCollectiondivH3 != null)
//            {
//                foreach (var htmlNode in nodeCollectiondivH3)
//                {
//                    var parent = htmlNode.ParentNode;
//
//                    for (int i = 0; i < parent.ChildNodes.Count; i++)
//                    {
//
//                        var child = parent.ChildNodes[i];
//                        if (child.Name == htmlNode.Name && child.Attributes["divH3"] != null)
//                        {
//
//                            var tmpChild =  parent.ChildNodes[i + 1];
//                            if (tmpChild.Name == "ul")
//                            {
//                                var childUl = tmpChild;
//
//                                var divNode = doc.CreateElement("div");
//                                divNode.SetAttributeValue("class", "list-block");
//
//                                divNode.AppendChild(htmlNode);
//                                divNode.AppendChild(childUl);
//
//                                parent.ReplaceChild(divNode, htmlNode);
//                                childUl.Remove();
//
//
//
//                            }
//                        }
//                    }
//
//                }
//            }

//            var nodeCollectionUl = doc.DocumentNode.SelectSingleNode("//ul");

            //HtmlNodeCollection nodeCollectionUl = doc.DocumentNode.SelectNodes("//ul");

            //if (nodeCollectionUl != null)
            //{
            //    foreach (var htmlNode in nodeCollectionUl)
            //    {
            //        HtmlNode divNode = doc.CreateElement("div");
            //        divNode.SetAttributeValue("class", "list-block");



            //        //HtmlNode tmpList = doc.CreateElement("ul");

            //        //tmpList.InnerHtml = htmlNode.InnerHtml;

            //        //divNode.AppendChild(tmpList);//htmlNode.Clone());

            //        //node.ParentNode.ReplaceChild(divNode, htmlNode);
            //    }
            //}

//            }//debug if

            SaveHTML(doc, Path.GetFileName(htmlPath));//debug save fixed html

            var tmpHtml =  PrepareTidyHtml(doc.DocumentNode.OuterHtml);

            return tmpHtml;
        }


        public static void RemoveStyleAttributes(HtmlDocument html)
        {
            var elementsWithStyleAttribute = html.DocumentNode.SelectNodes("//@style");

            if (elementsWithStyleAttribute != null)
            {
                foreach (var element in elementsWithStyleAttribute)
                {
                    element.Attributes["style"].Remove();
                }
            }
        }
        private void prepareHNodeCollection(HtmlNodeCollection HNodeCollection, string className ,HtmlAgilityPack.HtmlDocument doc)
        {
            foreach (var htmlNode in HNodeCollection)
            {
                HtmlNode divHNode = doc.CreateElement("div");
                divHNode.SetAttributeValue("class", className);

                divHNode.InnerHtml = htmlNode.InnerText;

                htmlNode.ParentNode.ReplaceChild(divHNode, htmlNode);
            }
        }
        private void prepareDivBloc(HtmlNodeCollection divBlocNodeColection, HtmlAgilityPack.HtmlDocument doc)
        {
            foreach (var node in divBlocNodeColection)
            {
                string className = "";

                className = node.Attributes["class"].Value;
                var tmpNode = node.Clone();
                

                HtmlNode tableNode = doc.CreateElement("table");
                tableNode.Attributes.Add("class", className);

                HtmlNode trHeaderNode = doc.CreateElement("tr");
                HtmlNode thHeaderNode = doc.CreateElement("th");

                HtmlNode trTextNode = doc.CreateElement("tr");
                HtmlNode tdTextNode = doc.CreateElement("td");


                //thHeaderNode.InnerHtml = tmpNode.SelectSingleNode("//div[@class='divH1']").InnerHtml;
                var tmpDiv = tmpNode.SelectSingleNode(".//div[@class='divH1']");
                thHeaderNode.InnerHtml = tmpDiv.InnerHtml;//ChildNodes["div"].InnerText;//SelectSingleNode("./div[@class='divH1'").InnerHtml;

                tmpDiv.ParentNode.RemoveChild(tmpDiv, false);


                //tmpNode.SelectSingleNode(".//div[@class='divH1']").Remove();//SelectSingleNode("./div[@class='divH1']").Remove();
                
                tdTextNode.InnerHtml = tmpNode.InnerText;

                tableNode.AppendChild(trHeaderNode);
                tableNode.AppendChild(trTextNode);

                trHeaderNode.AppendChild(thHeaderNode);
                trTextNode.AppendChild(tdTextNode);

                node.ParentNode.ReplaceChild(tableNode, node);

            }
        }

        private ParentOfNodes AddPicture(HtmlNode htmlNode, HtmlAgilityPack.HtmlDocument doc, string textImg)
        {
            #region create tags
            ParentOfNodes tmpNode = new ParentOfNodes();
            //<span>
            HtmlNode spanNode = doc.CreateElement("span");
            //<div>
            HtmlNode divNode = doc.CreateElement("div");
            divNode.SetAttributeValue("align", "center");
            divNode.SetAttributeValue("class", "abc-picture");
            //<br>
            HtmlNode brNode = doc.CreateElement("br");
            //<img>
            HtmlNode imgNode = doc.CreateElement("img");
            //<p> --- <div> - text
            HtmlNode divTextNode = doc.CreateElement("div");
            divTextNode.Attributes.Add("class", "imgText");//pNode.Attributes.Add("class", "center-text");

            #endregion

            string tmpPath = "";

            if (htmlNode.Attributes["path"] != null)
            {
                tmpPath = /*@"Data/"*/curentDirectory + htmlNode.Attributes["path"].Value;

                tmpPath = tmpPath.Replace(@"/", @"\");

                //tmpPath = System.Web.HttpContext.Current.Server.MapPath(tmpPath);
//                System.Web.Hosting.HostingEnvironment.MapPath(tmpPath);

               


            }

            string tmpTextTag = htmlNode.InnerText + " (" + textImg + ")";

            if (!String.IsNullOrEmpty(tmpPath) && File.Exists(tmpPath))
            {
                //tmpPath = Path.GetFullPath(tmpPath);
                //tmpPath = tmpPath.Replace(" ", "%20");

                //tmpPath = System.Web.Hosting.HostingEnvironment.MapPath(tmpPath);

                var uri = new System.Uri(tmpPath);
                var convertedURI = uri.AbsoluteUri;



                imgNode.SetAttributeValue("src", convertedURI); //tmpPath);
                //styleXmlParser.SetAttributesOfTag(imgNode);

                try
                {
                    int heightImg;
                    int widthImg;
                    int minLimit = 600;
                    using (System.Drawing.Image img = System.Drawing.Image.FromFile(tmpPath))//.Drawing.Image objImage = System.Drawing.Image.FromFile(FileName))
                    {
                        widthImg = img.Width;
                        heightImg = img.Height;
                    }

                    if ((widthImg < minLimit) && (heightImg < minLimit))
                    {
                        imgNode.SetAttributeValue("class", "ui-chapter-picture_abc-standart");
                    }
                    else
                    {
                        if (widthImg > heightImg)
                        {
                            imgNode.SetAttributeValue("class", "ui-chapter-picture_abc-landscape");
                        }
                        else
                        {
                            imgNode.SetAttributeValue("class", "ui-chapter-picture_abc-portrait");
                        }
                    }
                }
                catch(Exception e)
                {

                }


                divNode.AppendChild(brNode);
                divNode.AppendChild(imgNode);
                divNode.AppendChild(divTextNode);

                switch (htmlNode.Name)
                {
                    case "body":
                        var textTagBody = doc.CreateTextNode(textImg + " " + locXmlParser.GetLocalization("generalView"));
                        divTextNode.AppendChild(textTagBody);

                        //htmlNode.InsertBefore(divNode, htmlNode.FirstChild);
                        htmlNode.PrependChild(divNode);
                        break;
                    case "a":
                        var textTagA = doc.CreateTextNode(textImg);

                        divTextNode.AppendChild(textTagA);

                        tmpNode.parent = GetAcceptableParentNode(htmlNode);
                        tmpNode.child = divNode;
                        htmlNode.ParentNode.ReplaceChild(HtmlTextNode.CreateNode(spanNode.InnerText + tmpTextTag), htmlNode);
                        break;
                }
                return tmpNode;
            }
            else
            {
                if (showDebugImgFlag)/////////////
                {
                    switch (htmlNode.Name)
                    {
                        case "body":

                            var textTagBody =
                                doc.CreateTextNode(textImg + " " + locXmlParser.GetLocalization("generalView") +
                                                   " " + locXmlParser.GetLocalization("noPicture"));

                            divTextNode.AppendChild(textTagBody);
                            htmlNode.InsertBefore(divTextNode, htmlNode.FirstChild);
                            break;
                        case "a":

                            htmlNode.ParentNode.ReplaceChild(HtmlTextNode.CreateNode(spanNode.InnerText +
                                                                                     tmpTextTag +
                                                                                     locXmlParser.GetLocalization(
                                                                                         "noPicture")), htmlNode);
                            break;
                    }
                }/////////////////
                return tmpNode;
                
            }
        }
        private HtmlNode GetAcceptableParentNode(HtmlNode node)
        {
            var parent = node.ParentNode;
            var resultNode = node;

            while (parent != null)
            {
                switch (resultNode.Name)
                {
                    case "a":
                        if (parent.Name == "p" || parent.Name == "div" || parent.Name == "table" || parent.Name == "li")
                            resultNode = parent;
                        break;

                    case "p":
                    case "div":
                        if (parent.Name == "table")
                            resultNode = parent;
                        break;
                }

                parent = parent.ParentNode;
            }
            return resultNode;
        }

        private void prepareIconColumn(HtmlNode node)
        {
            var parent = node.ParentNode;

            while (parent != null)
            {
                if(parent.Name == "td")
                {
                    parent.Attributes.Add("class", "column_icon");

                    break;
                    //parent = null;
                }
                else
                {
                    parent = parent.ParentNode;
                }
            }
        }


        private string PrepareTidyHtml(string htmlText)
        {
            string dirtyHtml = htmlText;
            string cleanHtml = "";

            using (TidyManaged.Document docHtmlQWERTY = TidyManaged.Document.FromString(dirtyHtml))
            {
                byte[] bytes = Encoding.Default.GetBytes(dirtyHtml);
                byte[] destEncodingBytes = Encoding.Convert(Encoding.Default, Encoding.UTF8, bytes);
                dirtyHtml = System.Text.Encoding.UTF8.GetString(destEncodingBytes);//
                var strStream = new MemoryStream(destEncodingBytes);

                //do the parsing
                var docHtml = TidyManaged.Document.FromStream(strStream);
                docHtml.InputCharacterEncoding = TidyManaged.EncodingType.Utf8;
                docHtml.OutputCharacterEncoding = TidyManaged.EncodingType.Utf8;
                docHtml.CharacterEncoding = TidyManaged.EncodingType.Utf8;
                docHtml.ShowWarnings = false;
                docHtml.Quiet = true;
                docHtml.OutputXhtml = true;
                docHtml.CleanAndRepair();

                cleanHtml = docHtml.Save();
            }
            return cleanHtml;
        }
        private void SaveHTML(HtmlDocument doc, string fileName)
        {
            var directoryName = @"Debug_SavedHTML";

            Directory.CreateDirectory(directoryName);


            doc.Save(directoryName + @"\" + "Debug_" +fileName);

        }
    }



//    public class MyImageFactory : iTextSharp.text.html.simpleparser.IImageProvider
//    {
//        iTextSharp.text.Image iTextSharp.text.html.simpleparser.IImageProvider.GetImage(string src, IDictionary<string, string> attrs, ChainedProperties chain, IDocListener doc)
//        {
//            return iTextSharp.text.Image.GetInstance(src);
//        }
//    }

}
