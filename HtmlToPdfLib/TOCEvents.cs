using System;
using System.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Collections.Generic;

namespace HtmlToPdfLib
{
    public class TOCEvents : PdfPageEventHelper
    {
        PdfContentByte cb;
        BaseFont bf = null;
        iTextSharp.text.Font font;
        protected int counter = 0;
        const float topOffset = 0f;

        public bool DrawFooter { get; set; }
//        public bool newPageFlag { get; set; }

        LocalizationXmlParser locXmlParser = new LocalizationXmlParser();

        public TOCEvents(string Lang)
        {
            locXmlParser.GetNewLanguage(/*inputTextXmlPath, localizationXmlPath,*/ Lang);
        }


        protected Dictionary<string, KeyValuePair<string, int>> toc = new Dictionary<string, KeyValuePair<string, int>>(5);
        public Dictionary<string, KeyValuePair<string, int>> GetTOC()
        {
            return toc;
        }

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            base.OnOpenDocument(writer, document);

            try
            {
                bf = BaseFont.CreateFont(@"SystemData\font\font.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                font = new iTextSharp.text.Font(bf, 14, iTextSharp.text.Font.NORMAL);
            }
            catch (Exception e)
            {

            }
        }

        public override void OnGenericTag(PdfWriter writer, Document document, Rectangle rect, String text)
        {
            string name = "dest" + (counter++);
            int page = writer.PageNumber;

            if (!toc.Keys.Contains<string>(text))
                toc.Add(text, new KeyValuePair<string, int>(name, page));

            writer.AddNamedDestination(name, page, new PdfDestination(PdfDestination.FITH, rect.GetTop(topOffset)));
        }

        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnEndPage(writer, document);

            int pageN = writer.PageNumber;
            Rectangle pageSize = document.PageSize;
            if (DrawFooter)
            {
                if ((pageN % 2) == 1)
                {
                    cb.BeginText();
                    //cb.SetFontAndSize(bf, 14);
                    //cb.SetTextMatrix(pageSize.GetRight(80), pageSize.GetBottom(15));
                    //cb.ShowText(/*"Стр. "*/locXmlParser.GetLocalization("numberOfPage") + " " + pageN.ToString());


                    ColumnText ct = new ColumnText(cb);
                    ct.SetSimpleColumn(
                        new Phrase(
                        (new Chunk(locXmlParser.GetLocalization("numberOfPage") + " " + pageN.ToString(), font))),
                        writer.PageSize.GetRight(36f), 20, writer.PageSize.GetLeft(36f), 0, 0, Element.ALIGN_RIGHT);
                    ct.Go(); 



                    cb.EndText();
                }
                else
                {
                    cb.BeginText();
//                    cb.SetFontAndSize(bf, 14);
//                    cb.SetTextMatrix(pageSize.GetLeft(20), pageSize.GetBottom(15));
//                    cb.ShowText(/*"Стр. "*/locXmlParser.GetLocalization("numberOfPage") + " " + pageN.ToString());

                    ColumnText ct = new ColumnText(cb);
                    ct.SetSimpleColumn(
                        new Phrase(
                        (new Chunk(locXmlParser.GetLocalization("numberOfPage") + " " + pageN.ToString(), font))),
                        writer.PageSize.GetRight(36f), 20, writer.PageSize.GetLeft(36f), 0, 0, Element.ALIGN_LEFT);
                    ct.Go(); 

                    cb.EndText();
                }
            }
        }



    }
}
