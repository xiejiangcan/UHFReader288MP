using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace UHFReader288MP
{
    public partial class Setting : UserControl
    {
        const string windowName = "UHFReader288MP";

        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        private static extern int PostMessage(
        IntPtr hWnd, // handle to destination window 
        uint Msg, // message 
        uint wParam, // first message parameter 
        uint lParam // second message parameter 
        );

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, string lParam);

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public const int USER = 0x0400;
        public const int WM_SENDTAG = USER + 101;
        public const int WM_SENDTAGSTAT = USER + 102;
        public const int WM_SENDSTATU = USER + 103;
        public const int WM_SENDBUFF = USER + 104;
        public const int WM_MIXTAG = USER + 105;
        public const int WM_SHOWNUM = USER + 106;
        public const int WM_FASTID = USER + 107;
        public const int WM_START = USER + 108;
        public const int WM_STOP = USER + 109;
        public const int WM_SETPOWER = USER + 110;
        public const int WM_SETREADERADDR = USER + 111;
        public const int WM_SETREADERID = USER + 112;
        public const int WM_SETWORKMODEL = USER + 113;
        public const int WM_SETSOUNDSWITCH = USER + 114;
        public const int WM_SETMAXTIME = USER + 115;

        public Setting()
        {
            InitializeComponent();
            InitControl();
            InitProperty();
        }

        void InitControl()
        {
            for (int i = 0; i <= 30; i++)
            {
                this.CB_Power.Items.Add(Convert.ToString(i));
            }

            for (int i = 0x00; i <= 0xFF; i++)
            {
                this.CB_ScanTime.Items.Add(Convert.ToString(i) + "*100ms");
            }
        }

        void InitProperty()
        {
            this.Power = 30;
            this.ReaderAddr = "00";
            this.ReaderID = "";
            this.ScanTime = 20;
            this.IsSoundOpen = true;
            this.SqlAddr = "81.69.20.82";
            this.SqlPort = "3306";
            this.SqlAccount = "rfidepc";
            this.SqlPwd = "e8pNW2NZsxk5KeGD";
        }

        private int power;

        public int Power
        {
            get { return power; }
            set
            {
                power = value;
                this.CB_Power.SelectedIndex = power;
            }
        }

        private string readerAddr;

        public string ReaderAddr
        {
            get { return readerAddr; }
            set
            {
                readerAddr = value;
                this.TB_ReaderAddr.Text = readerAddr;
            }
        }

        private string readerID;

        public string ReaderID
        {
            get { return readerID; }
            set
            {
                readerID = value;
                this.TB_ReaderID.Text = readerID;
            }
        }

        private int scanTime;

        public int ScanTime
        {
            get { return scanTime; }
            set
            {
                scanTime = value;
                this.CB_ScanTime.SelectedIndex = scanTime;
            }
        }

        private bool isSoundOpen;

        public bool IsSoundOpen
        {
            get { return isSoundOpen; }
            set
            {
                isSoundOpen = value;
                if (isSoundOpen)
                {
                    this.RB_Open.Checked = true;
                }
                else
                {
                    this.RB_Close.Checked = true;
                }
            }
        }

        private string sqlAddr;

        public string SqlAddr
        {
            get { return sqlAddr; }
            set
            {
                sqlAddr = value;
                this.TB_SqlAddr.Text = sqlAddr;
                //SqlOperation.Instance.Addr = sqlAddr;
            }
        }

        private string sqlPort;

        public string SqlPort
        {
            get { return sqlPort; }
            set
            {
                sqlPort = value;
                this.TB_SqlPort.Text = sqlPort;
                //SqlOperation.Instance.Port = int.Parse(sqlPort);
            }
        }

        private string sqlAccount;

        public string SqlAccount
        {
            get { return sqlAccount; }
            set
            {
                sqlAccount = value;
                this.TB_SqlAccount.Text = sqlAccount;
                //SqlOperation.Instance.Account = sqlAccount;
            }
        }

        private string sqlPwd;

        public string SqlPwd
        {
            get { return sqlPwd; }
            set
            {
                sqlPwd = value;
                this.TB_SqlPwd.Text = sqlPwd;
                //SqlOperation.Instance.PassWd = sqlPwd;
            }
        }

        private void Btn_Power_Click(object sender, EventArgs e)
        {
            IntPtr ptrWnd = IntPtr.Zero;
            ptrWnd = FindWindow(null, windowName);
            if (ptrWnd != IntPtr.Zero)         // 检查当前统计窗口是否打开
            {
                string para = this.CB_Power.Text;
                SendMessage(ptrWnd, WM_SETPOWER, IntPtr.Zero, para);
            }
            ptrWnd = IntPtr.Zero;
        }

        private void Btn_ReaderAddr_Click(object sender, EventArgs e)
        {
            IntPtr ptrWnd = IntPtr.Zero;
            ptrWnd = FindWindow(null, windowName);
            if (ptrWnd != IntPtr.Zero)         // 检查当前统计窗口是否打开
            {
                string para = TB_ReaderAddr.Text;
                SendMessage(ptrWnd, WM_SETREADERADDR, IntPtr.Zero, para);
            }
            ptrWnd = IntPtr.Zero;
        }

        public void SetReaderID(string id)
        {
            this.TB_ReaderID.Text = id;
        }

        private void Btn_ReaderID_Click(object sender, EventArgs e)
        {
            IntPtr ptrWnd = IntPtr.Zero;
            ptrWnd = FindWindow(null, windowName);
            if (ptrWnd != IntPtr.Zero)         // 检查当前统计窗口是否打开
            {
                string para = TB_ReaderID.Text;
                TB_ReaderID.Text = "";
                SendMessage(ptrWnd, WM_SETREADERID, IntPtr.Zero, para);
            }
            ptrWnd = IntPtr.Zero;
        }

        private void Btn_SoundSwith_Click(object sender, EventArgs e)
        {
            IntPtr ptrWnd = IntPtr.Zero;
            ptrWnd = FindWindow(null, windowName);
            if (ptrWnd != IntPtr.Zero)         // 检查当前统计窗口是否打开
            {
                string para = RB_Open.Checked ? "open" : "close";
                SendMessage(ptrWnd, WM_SETSOUNDSWITCH, IntPtr.Zero, para);
            }
            ptrWnd = IntPtr.Zero;
        }

        private void Btn_ScanTime_Click(object sender, EventArgs e)
        {
            IntPtr ptrWnd = IntPtr.Zero;
            ptrWnd = FindWindow(null, windowName);
            if (ptrWnd != IntPtr.Zero)         // 检查当前统计窗口是否打开
            {
                string para = CB_ScanTime.SelectedIndex.ToString();
                SendMessage(ptrWnd, WM_SETMAXTIME, IntPtr.Zero, para);
            }
            ptrWnd = IntPtr.Zero;
        }
    }
}
