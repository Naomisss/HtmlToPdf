using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HTML_to_PDF_book
{
    public partial class TimerForm : Form
    {
        int sec, min, hours;

        public TimerForm()
        {
            InitializeComponent();

            timer1.Interval = 1000;
            timer1.Tick +=new EventHandler(timer1_Tick);
            timer1.Start();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            sec++;
            label1.Text = String.Format("{0}:{1:00}:{2:00}", sec / 3600, sec / 60 % 60, sec % 60);
        }

        public void TimerStop()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(TimerStop));
            }
            else
            {
                timer1.Stop();
                label1.Text = "The End" + Environment.NewLine + String.Format("{0}:{1:00}:{2:00}", sec / 3600, sec / 60 % 60, sec % 60);
            }
        }

        public void TimerFormClose()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(TimerFormClose));
            }
            else
            {
                this.Close();
            }
        }

    }
}
