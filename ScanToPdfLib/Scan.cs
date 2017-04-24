using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Dosadi.EZTwain;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Image = iTextSharp.text.Image;
using Rectangle = iTextSharp.text.Rectangle;


namespace ScanToPdfLib
{
    public class Scan
    {
        #region imports

        [DllImport("gdiplus.dll", ExactSpelling = true)]
        private static extern int GdipCreateBitmapFromGdiDib(IntPtr bminfo, IntPtr pixdat, ref IntPtr image);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GlobalLock(IntPtr handle);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GlobalFree(IntPtr handle);

        [StructLayout(LayoutKind.Sequential, Pack = 2)]

        #endregion

        private class BitMapInfo
        {
            public int biSize = 0;
            public int biWidth = 0;
            public int biHeight = 0;
            public short biPlanes = 0;
            public short biBitCount = 0;
            public int biCompression = 0;
            public int biSizeImage = 0;
            public int biXPelsPerMeter = 0;
            public int biYPelsPerMeter = 0;
            public int biClrUsed = 0;
            public int biClrImportant = 0;
        }

        public enum PixTypes
        {
            TWAIN_BW, 
            WAIN_GRAY, 
            TWAIN_RGB, 
            TWAIN_PALETTE,
            TWAIN_ANYTYPE
        }

        private int[] arrayPixTypes = new int[] 
                                        { EZTwain.TWAIN_BW, 
                                          EZTwain.TWAIN_GRAY, 
                                          EZTwain.TWAIN_RGB, 
                                          EZTwain.TWAIN_PALETTE,
                                          EZTwain.TWAIN_ANYTYPE
                                        };

        public Bitmap ScanImage(IntPtr Handle, int nBits = 24, int resolution = 300, 
                                bool hideDialog = true, PixTypes px = PixTypes.TWAIN_RGB )
        {
            IntPtr hdib = IntPtr.Zero;
            int flagHideDialog;

            if (hideDialog) 
                flagHideDialog = 1;
            else 
                flagHideDialog = 0;

            if (EZTwain.IsAvailable() == 1)
            {
                try
                {

                    EZTwain.OpenDefaultSource();//выбор стандартного сканера

                    EZTwain.SetHideUI(flagHideDialog); // показывать\не показывать диалог сканера

                    EZTwain.SetBitDepth(24); //установка глубины цвета 24бит/пиксел
                    EZTwain.SetCurrentResolution(resolution);//разрешение

                    hdib = EZTwain.AcquireNative(Handle, arrayPixTypes[(int)px]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return null;
                }
                if (hdib == IntPtr.Zero)
                {
                    MessageBox.Show("Изображение не загружено.");
                    return null;
                }


                return new Bitmap(bitmapFromDIB(hdib));
            }
            else
            {
                MessageBox.Show("Нет библиотеки TWAIN");
                return null;
            }
        }

        private Bitmap bitmapFromDIB(IntPtr dibhand)
        {

            IntPtr bmpptr = GlobalLock(dibhand);
            IntPtr pixptr = GetPixelInfo(bmpptr);
            IntPtr pBmp = IntPtr.Zero;
            int status = GdipCreateBitmapFromGdiDib(bmpptr, pixptr, ref pBmp);
            if ((status == 0) && (pBmp != IntPtr.Zero))
            {

                MethodInfo mi = typeof(Bitmap).GetMethod("FromGDIplus", BindingFlags.Static | BindingFlags.NonPublic);
                if (mi == null) return null;
                Bitmap result = new Bitmap(mi.Invoke(null, new object[] { pBmp }) as Bitmap);
                GlobalFree(dibhand);
                dibhand = IntPtr.Zero;
                return result;

            }
            else
            {
                return null;
            }
        }
        private IntPtr GetPixelInfo(IntPtr bmpptr)
        {

            BitMapInfo bmi = new BitMapInfo();
            Marshal.PtrToStructure(bmpptr, bmi);
            Rectangle bmprect = new Rectangle(0, 0, bmi.biWidth, bmi.biHeight);
            if (bmi.biSizeImage == 0) bmi.biSizeImage = ((((bmi.biWidth * bmi.biBitCount) + 31) & ~31) >> 3) * bmi.biHeight;
            int p = bmi.biClrUsed;
            if ((p == 0) && (bmi.biBitCount <= 8)) p = 1 << bmi.biBitCount;
            p = (p * 4) + bmi.biSize + (int)bmpptr;
            return (IntPtr)p;

        }

        public void AppendToExistingDocument(string sourcePdfPathMain, string sourcePdfPathSub, string outputPdfPath)
        {
            using (var sourceDocumentStreamMain = new FileStream(sourcePdfPathMain, FileMode.Open))
            {
                using (var sourceDocumentStreamSub = new FileStream(sourcePdfPathSub, FileMode.Open))
                {
                    using (var destinationDocumentStream = new FileStream(outputPdfPath, FileMode.Create))
                    {
                        var pdfConcat = new PdfConcatenate(destinationDocumentStream);
                        var pdfReader = new PdfReader(sourceDocumentStreamMain);

                        var pages = new List<int>();
                        for (int i = 0; i < pdfReader.NumberOfPages; i++)
                        {
                            pages.Add(i);
                        }

                        pdfReader.SelectPages(pages);
                        pdfConcat.AddPages(pdfReader);

                        pdfReader = new PdfReader(sourceDocumentStreamSub);

                        pages = new List<int>();
                        for (int i = 0; i <= pdfReader.NumberOfPages; i++)
                        {
                            pages.Add(i);
                        }

                        pdfReader.SelectPages(pages);
                        pdfConcat.AddPages(pdfReader);

                        pdfReader.Close();
                        pdfConcat.Close();
                    }
                }
            }
        }

        public void CreatePdfWithImg(string fileNamePdf, List<System.Drawing.Image> listImg)
        {
            FileStream stream = new FileStream(fileNamePdf, FileMode.Create);
            Document document = new Document();

            PdfWriter writer = PdfWriter.GetInstance(document, stream);

            Image img;

            document.Open();
            foreach (var image in listImg)
            {
                document.Add(ScaleImg(Image.GetInstance(image, new BaseColor(255, 255, 255))));
            }
            document.Close();
        }

        public void CreatePdfWithImg(string fileNamePdf, System.Drawing.Image Img)
        {
            FileStream stream = new FileStream(fileNamePdf, FileMode.Create);
            Document document = new Document();

            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            document.Open();

            document.Add(ScaleImg(Image.GetInstance(Img, new BaseColor(255, 255, 255))));

            document.Close();
        }

        private Image ScaleImg(Image img)
        {
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

            return img;
        }
    }
}
