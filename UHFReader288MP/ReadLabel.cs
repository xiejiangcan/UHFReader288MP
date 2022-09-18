﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using UHF;

namespace UHFReader288MP
{
    public partial class ReadLabel : UserControl
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

        private bool isInput;
        private List<CheckBox> checkBoxList = new List<CheckBox>();
        
        public ReadLabel(bool isInput)
        {
            InitializeComponent();
            InitProperty();
            checkBoxList.Add(this.checkBox1);
            checkBoxList.Add(this.checkBox2);
            checkBoxList.Add(this.checkBox3);
            checkBoxList.Add(this.checkBox4);

            this.isInput = isInput;

            if (isInput)
            {
                this.btnSwitch.Text = "开始入库";
            }
            else
            {
                this.btnSwitch.Text = "开始出库";
            }
        }

        void InitProperty()
        {
            this.IsRuning = false;
            this.LabelTableName = "ali_base_epc";
            this.IOTableName = "ali_base_epc_log";
            this.HttpAddr = "未知";
            this.LabelWidth = 0;
            this.LabelLength = 0;
        }

        public void SetClickButtonEnable(bool state)
        {
            this.btnSwitch.Enabled = state;
        }

        public void SetButtonText(string str)
        {
            this.btnSwitch.Text = str;
        }

        public bool IsRuning { get; set; }

        private string labelTableName;

        public string LabelTableName
        {
            get { return labelTableName; }
            set
            {
                labelTableName = value;
                this.TB_LabelTabName.Text = labelTableName;
                SqlOperation.Instance.BaseTableName = labelTableName;
            }
        }

        private string ioTableName;

        public string IOTableName
        {
            get { return ioTableName; }
            set
            {
                ioTableName = value;
                this.TB_IOTabName.Text = ioTableName;
                SqlOperation.Instance.LogTableName = ioTableName;
            }
        }

        private string httpAddr;

        public string HttpAddr
        {
            get { return httpAddr; }
            set
            {
                httpAddr = value;
                this.TB_HttpAddr.Text = httpAddr;
            }
        }

        private int labelWidth;

        public int LabelWidth
        {
            get { return labelWidth; }
            set
            {
                labelWidth = value;
                this.TB_LabelWidth.Text = string.Format("{0}",labelWidth);
            }
        }

        private int labelLength;

        public int LabelLength
        {
            get { return labelLength; }
            set
            {
                labelLength = value;
                this.TB_LabelLength.Text = string.Format("{0}", labelLength);
            }
        }

        private int curIndex = 0;
        private List<byte> antList;
        public byte GetCurAntCode()
        {
            if (antList.Count == 0)
                return 0x80;
            curIndex = (curIndex + 1) % antList.Count;
            return antList[curIndex];
        }

        private void btnSwitch_Click(object sender, EventArgs e)
        {
            // 检查远程数据库服务器
            if (!SqlOperation.Instance.ConnectSql())
            {
                MessageBox.Show("请检查远程数据库!");
                return;
            }

            antList = new List<byte>();
            for (int i = 0; i < checkBoxList.Count; i++)
            {
                if (checkBoxList[i].Checked)
                {
                    switch (i)
                    {
                        case 0:
                            antList.Add(0x80);
                            break;
                        case 1:
                            antList.Add(0x81);
                            break;
                        case 2:
                            antList.Add(0x82);
                            break;
                        case 3:
                            antList.Add(0x83);
                            break;
                        default:
                            antList.Add(0x80);
                            break;
                    }
                }
            }
            if (antList.Count == 0)
            {
                MessageBox.Show("请选择天线通道!");
                return;
            }

            if (IsRuning)
            {
                IsRuning = false;
                btnSwitch.Enabled = false;
                btnSwitch.BackColor = Color.Transparent;
                btnSwitch.Text = "停止中";

                IntPtr ptrWnd = IntPtr.Zero;
                ptrWnd = FindWindow(null, windowName);
                if (ptrWnd != IntPtr.Zero)         // 检查当前统计窗口是否打开
                {
                    string para = "";
                    if (isInput)
                    {
                        para = "Input Stop";
                    }
                    else
                    {
                        para = "Output Stop";
                    }
                    SendMessage(ptrWnd, WM_STOP, IntPtr.Zero, para);
                }
                ptrWnd = IntPtr.Zero;
            }
            else
            {
                IsRuning = true;
                btnSwitch.BackColor = Color.Indigo;
                btnSwitch.Text = "停止";

                IntPtr ptrWnd = IntPtr.Zero;
                ptrWnd = FindWindow(null, windowName);
                if (ptrWnd != IntPtr.Zero)         // 检查当前统计窗口是否打开
                {
                    string para = "";
                    if (isInput)
                    {
                        para = "Input Start";
                    }
                    else
                    {
                        para = "Output Start";
                    }
                    SendMessage(ptrWnd, WM_START, IntPtr.Zero, para);
                }
                ptrWnd = IntPtr.Zero;
            }
        }

        public void SetFixedEnable(bool enable)
        {

        }

        private void TB_LabelTabName_TextChanged(object sender, EventArgs e)
        {
            this.LabelTableName = this.TB_LabelTabName.Text;
        }

        private void TB_IOTabName_TextChanged(object sender, EventArgs e)
        {
            this.IOTableName = this.TB_IOTabName.Text;
        }
    }
}
