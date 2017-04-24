using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using HtmlToPdfLib;
using System.Diagnostics;
using System.Threading;
using ScanToPdfLib;

namespace HTML_to_PDF_book
{
    public partial class Form1 : Form
    {
        #region strings
        //private const string inputFont = @"SystemData\xml\font.ttf";
        private const string localizationXmlPath = @"SystemData\xml\Localization.xml";
        private const string inputTextXmlPath = @"SystemData\xml\InputText.xml";
        private const string styleXmlPath = @"SystemData\xml\StyleXML.xml";

        private string defaultDirectory = "";

        private string fileNamePDF = "";
        //private string fileNameXML = "";

        private string defaultLang = "";
        private string selectedLocalization = "";
        #endregion


        private TimerForm timerForm;
        private Thread _splashThread;

        private List<System.Drawing.Image> listImg = new List<System.Drawing.Image>();
        private System.Drawing.Image img = null;

        private Scan scan;
        private LocalizationXmlParser locXmlParser = new LocalizationXmlParser();
        private CreatePdf cPdf;

        public Form1()
        {
            InitializeComponent();

            #region icon
            // делаем невидимой нашу иконку в трее
            notifyIcon1.Visible = false;
            // добавляем Эвент или событие по 2му клику мышки, 
            //вызывая функцию  notifyIcon1_MouseDoubleClick
            this.notifyIcon1.MouseDoubleClick += new MouseEventHandler(notifyIcon1_MouseDoubleClick);

            // добавляем событие на изменение окна
            this.Resize += new System.EventHandler(this.Form1_Resize);


            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            #endregion


            defaultLang = locXmlParser.DefautLanguage(/*localizationXmlPath*/);
            selectedLocalization = defaultLang;
            locXmlParser.GetNewLanguage(/*inputTextXmlPath, localizationXmlPath, */defaultLang);

            comboBox1.Items.AddRange(locXmlParser.SearchLanguages(localizationXmlPath));
            comboBox1.SelectedIndex = comboBox1.Items.IndexOf(defaultLang);
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;

            scan = new Scan();

            var tmpDitectory = Application.StartupPath + "\\";
            defaultDirectory = tmpDitectory + @"Data\";

            fileNamePDF = tmpDitectory + "result.pdf";
            //fileNameXML = defaultDirectory + @"Data\" + "Tree.xml";


            textBoxDefaultDirectory.Text = defaultDirectory;
            textBoxFileName.Text = fileNamePDF;


            cPdf = new CreatePdf();
            cPdf.duplexPrintingFlag = true;
            cPdf.showDebugImgFlag = false;
            cPdf.selectedLang = defaultLang;



        }

        #region Windows Form
        //Форма для таймера в отдельном потоке.
        public void ShowTimerForm()
        {
            Action showForm = delegate
                                  {
                                      timerForm = new TimerForm();
                                      timerForm.TopLevel = true;
                                      timerForm.ShowDialog();
                                  };
            var treadStart = new ThreadStart(showForm);
            _splashThread = new Thread(treadStart) { IsBackground = true, ApartmentState = ApartmentState.MTA };
            _splashThread.Start();

            //_splashThread.Abort();
        }
        private void buttonGoCreatePdf_Click(object sender, EventArgs e)
        {
            var timer = Stopwatch.StartNew();
            textBox2.AppendText(Environment.NewLine + "Создание файла...");// + Environment.NewLine + timer.ElapsedMilliseconds + " Milliseconds");

            ShowTimerForm();



            cPdf.selectedLang = selectedLocalization;
            cPdf.CreateNewPdf(fileNamePDF , defaultDirectory);

            timerForm.TimerStop();
            timer.Stop();
            textBox2.AppendText(Environment.NewLine + "Файл создан за " + timer.ElapsedMilliseconds + " миллисекунд" + Environment.NewLine);

            try
            {
                Process.Start(fileNamePDF);
            }
            catch(Exception ex)
            {

            }
        }

        /*
        private void buttonScan_Click(object sender, EventArgs e)
        {
//            img = scan.ScanImage(this.Handle);

            
            img = scan.ScanImage(Handle, 24, 300, true, Scan.PixTypes.TWAIN_PALETTE);

            if(img != null)
                listImg.Add(img);
        }
        private void buttonAddImagesToNewPdf_Click(object sender, EventArgs e)
        {
            scan.CreatePdfWithImg(fileNamePdftmp, listImg);

            Process.Start(fileNamePdftmp);
        }
        private void buttonAddImagesToExistingPdf_Click(object sender, EventArgs e)
        {
            scan.AppendToExistingDocument(fileNamePDF, fileNamePdftmp, fileNamePdfOutput);
        }*/
        #endregion

        #region NotifyIcon
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // делаем нашу иконку скрытой
            notifyIcon1.Visible = false;
            // возвращаем отображение окна в панели
            this.ShowInTaskbar = true;
            //разворачиваем окно
            WindowState = FormWindowState.Normal;
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            // проверяем наше окно, и если оно было свернуто, делаем событие        
            if (WindowState == FormWindowState.Minimized)
            {
                // прячем наше окно из панели
                this.ShowInTaskbar = false;
                // делаем нашу иконку в трее активной
                notifyIcon1.Visible = true;

//                Hide();
//                notifyIcon1.Visible = true;


                notifyIcon1.ShowBalloonTip(500, "Сообщение", "Приложение было свернуто", ToolTipIcon.None);
            }
        }
        private void ToolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void ToolStripMenuItemOpen_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            this.ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;
        }
        #endregion


        private void textBoxDefaultDirectory_TextChanged(object sender, EventArgs e)
        {
            defaultDirectory = (sender as TextBox).Text;
        }
        private void textBoxFileName_TextChanged(object sender, EventArgs e)
        {
            fileNamePDF = (sender as TextBox).Text;
        }



        private void buttonOpenFolder_Click_1(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = defaultDirectory + "\\";
            folderBrowserDialog1.ShowNewFolderButton = false;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxDefaultDirectory.Text = folderBrowserDialog1.SelectedPath + "\\";
            }
        }

        private void buttonOpenFolderForButton_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = defaultDirectory;
            folderBrowserDialog1.ShowNewFolderButton = true;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxFileName.Text = folderBrowserDialog1.SelectedPath + "\\" + Path.GetFileName(fileNamePDF);
            }
        }

        private void checkBoxImgDebug_CheckedChanged(object sender, EventArgs e)
        {
            cPdf.showDebugImgFlag = checkBoxImgDebug.Checked;
        }
        private void checkBoxDuplexPrinting_CheckedChanged(object sender, EventArgs e)
        {
            cPdf.duplexPrintingFlag = checkBoxDuplexPrinting.Checked;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //selectedLocalization = comboBox1.SelectedItem.ToString();
            selectedLocalization = locXmlParser.GetNewLanguage(/*inputTextXmlPath, localizationXmlPath,*/ comboBox1.SelectedItem.ToString());

            buttonGoCreatePdf.Text = locXmlParser.GetLocalization("buttonGo");
        }




    }
}
