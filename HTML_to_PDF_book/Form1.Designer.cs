namespace HTML_to_PDF_book
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBoxDefaultDirectory = new System.Windows.Forms.TextBox();
            this.buttonOpenFolder = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.textBoxFileName = new System.Windows.Forms.TextBox();
            this.buttonOpenFolderForButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.buttonGoCreatePdf = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.checkBoxDuplexPrinting = new System.Windows.Forms.CheckBox();
            this.checkBoxImgDebug = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Тестовая иконка в трее";
            this.notifyIcon1.Visible = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemExit,
            this.ToolStripMenuItemOpen});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(122, 48);
            // 
            // ToolStripMenuItemExit
            // 
            this.ToolStripMenuItemExit.Name = "ToolStripMenuItemExit";
            this.ToolStripMenuItemExit.Size = new System.Drawing.Size(121, 22);
            this.ToolStripMenuItemExit.Text = "Выйти";
            this.ToolStripMenuItemExit.Click += new System.EventHandler(this.ToolStripMenuItemExit_Click);
            // 
            // ToolStripMenuItemOpen
            // 
            this.ToolStripMenuItemOpen.Name = "ToolStripMenuItemOpen";
            this.ToolStripMenuItemOpen.Size = new System.Drawing.Size(121, 22);
            this.ToolStripMenuItemOpen.Text = "Открыть";
            this.ToolStripMenuItemOpen.Click += new System.EventHandler(this.ToolStripMenuItemOpen_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Путь до директории";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBoxDefaultDirectory);
            this.panel1.Controls.Add(this.buttonOpenFolder);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(10, 10, 10, 0);
            this.panel1.Size = new System.Drawing.Size(559, 47);
            this.panel1.TabIndex = 12;
            // 
            // textBoxDefaultDirectory
            // 
            this.textBoxDefaultDirectory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDefaultDirectory.Location = new System.Drawing.Point(10, 23);
            this.textBoxDefaultDirectory.Name = "textBoxDefaultDirectory";
            this.textBoxDefaultDirectory.Size = new System.Drawing.Size(453, 20);
            this.textBoxDefaultDirectory.TabIndex = 8;
            this.textBoxDefaultDirectory.TextChanged += new System.EventHandler(this.textBoxDefaultDirectory_TextChanged);
            // 
            // buttonOpenFolder
            // 
            this.buttonOpenFolder.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonOpenFolder.Location = new System.Drawing.Point(463, 23);
            this.buttonOpenFolder.Name = "buttonOpenFolder";
            this.buttonOpenFolder.Size = new System.Drawing.Size(86, 24);
            this.buttonOpenFolder.TabIndex = 9;
            this.buttonOpenFolder.Text = "Выбрать...";
            this.buttonOpenFolder.UseVisualStyleBackColor = true;
            this.buttonOpenFolder.Click += new System.EventHandler(this.buttonOpenFolder_Click_1);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.Location = new System.Drawing.Point(10, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Путь к дериктории";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(10, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Имя файла";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.textBoxFileName);
            this.panel3.Controls.Add(this.buttonOpenFolderForButton);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 47);
            this.panel3.Margin = new System.Windows.Forms.Padding(3, 3, 3, 50);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.panel3.Size = new System.Drawing.Size(559, 37);
            this.panel3.TabIndex = 14;
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxFileName.Location = new System.Drawing.Point(10, 13);
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.Size = new System.Drawing.Size(453, 20);
            this.textBoxFileName.TabIndex = 22;
            this.textBoxFileName.TextChanged += new System.EventHandler(this.textBoxFileName_TextChanged);
            // 
            // buttonOpenFolderForButton
            // 
            this.buttonOpenFolderForButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonOpenFolderForButton.Location = new System.Drawing.Point(463, 13);
            this.buttonOpenFolderForButton.Name = "buttonOpenFolderForButton";
            this.buttonOpenFolderForButton.Size = new System.Drawing.Size(86, 24);
            this.buttonOpenFolderForButton.TabIndex = 21;
            this.buttonOpenFolderForButton.Text = "Выбрать...";
            this.buttonOpenFolderForButton.UseVisualStyleBackColor = true;
            this.buttonOpenFolderForButton.Click += new System.EventHandler(this.buttonOpenFolderForButton_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.textBox2);
            this.panel2.Controls.Add(this.buttonGoCreatePdf);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(421, 84);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(5);
            this.panel2.Size = new System.Drawing.Size(138, 321);
            this.panel2.TabIndex = 16;
            // 
            // textBox2
            // 
            this.textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox2.Location = new System.Drawing.Point(5, 37);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(128, 279);
            this.textBox2.TabIndex = 4;
            // 
            // buttonGoCreatePdf
            // 
            this.buttonGoCreatePdf.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonGoCreatePdf.Location = new System.Drawing.Point(5, 5);
            this.buttonGoCreatePdf.Name = "buttonGoCreatePdf";
            this.buttonGoCreatePdf.Size = new System.Drawing.Size(128, 32);
            this.buttonGoCreatePdf.TabIndex = 0;
            this.buttonGoCreatePdf.Text = "Создать PDF";
            this.buttonGoCreatePdf.UseVisualStyleBackColor = true;
            this.buttonGoCreatePdf.Click += new System.EventHandler(this.buttonGoCreatePdf_Click);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.comboBox1);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 84);
            this.panel4.Name = "panel4";
            this.panel4.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.panel4.Size = new System.Drawing.Size(421, 53);
            this.panel4.TabIndex = 17;
            // 
            // comboBox1
            // 
            this.comboBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(10, 13);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(189, 21);
            this.comboBox1.TabIndex = 13;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(10, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Язык";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.checkBoxDuplexPrinting);
            this.panel5.Controls.Add(this.checkBoxImgDebug);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 137);
            this.panel5.Name = "panel5";
            this.panel5.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.panel5.Size = new System.Drawing.Size(421, 100);
            this.panel5.TabIndex = 18;
            // 
            // checkBoxDuplexPrinting
            // 
            this.checkBoxDuplexPrinting.AutoSize = true;
            this.checkBoxDuplexPrinting.Checked = true;
            this.checkBoxDuplexPrinting.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxDuplexPrinting.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkBoxDuplexPrinting.Location = new System.Drawing.Point(10, 17);
            this.checkBoxDuplexPrinting.Name = "checkBoxDuplexPrinting";
            this.checkBoxDuplexPrinting.Size = new System.Drawing.Size(401, 17);
            this.checkBoxDuplexPrinting.TabIndex = 1;
            this.checkBoxDuplexPrinting.Text = "Двухсторонняя печать";
            this.checkBoxDuplexPrinting.UseVisualStyleBackColor = true;
            this.checkBoxDuplexPrinting.CheckedChanged += new System.EventHandler(this.checkBoxDuplexPrinting_CheckedChanged);
            // 
            // checkBoxImgDebug
            // 
            this.checkBoxImgDebug.AutoSize = true;
            this.checkBoxImgDebug.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkBoxImgDebug.Location = new System.Drawing.Point(10, 0);
            this.checkBoxImgDebug.Name = "checkBoxImgDebug";
            this.checkBoxImgDebug.Size = new System.Drawing.Size(401, 17);
            this.checkBoxImgDebug.TabIndex = 0;
            this.checkBoxImgDebug.Text = "Показывать записи о ненайденных картинках";
            this.checkBoxImgDebug.UseVisualStyleBackColor = true;
            this.checkBoxImgDebug.CheckedChanged += new System.EventHandler(this.checkBoxImgDebug_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(559, 405);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemExit;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemOpen;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button buttonGoCreatePdf;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxDefaultDirectory;
        private System.Windows.Forms.Button buttonOpenFolder;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonOpenFolderForButton;
        private System.Windows.Forms.TextBox textBoxFileName;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.CheckBox checkBoxImgDebug;
        private System.Windows.Forms.CheckBox checkBoxDuplexPrinting;
    }
}

