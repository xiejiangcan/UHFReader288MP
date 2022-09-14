using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CustomControl;

namespace UHFReader288MP
{
    public partial class LogView : UserControl
    {
        LogRichTextBox mLogRichTextBox;

        public LogView()
        {
            InitializeComponent();
            mLogRichTextBox = new LogRichTextBox();
            this.panel1.Controls.Add(mLogRichTextBox);
            mLogRichTextBox.Dock = DockStyle.Fill;
        }

        public LogRichTextBox RtbLog
        {
            get { return this.mLogRichTextBox;  }
        }

    }
}
