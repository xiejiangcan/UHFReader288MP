using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UHF;
using UHFReader288MPDemo;

namespace UHFReader288MP
{
    public partial class MainForm : Form
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

        Setting mSetting;
        ReadLabel mReadInput;
        ReadLabel mReadOutput;
        LogView mLogView;
        private int ecpCounter;
        private int flagRecord = 0; // 0:默认值， 1：OutPut； 2：InPut

        private byte fComAdr = 0xff; //当前操作的ComAdr
        private int ferrorcode;
        private byte fBaud;
        private double fdminfre;
        private double fdmaxfre;
        private int fCmdRet = 30; //所有执行指令的返回值
        private bool fisinventoryscan_6B;
        private byte[] fOperEPC = new byte[100];
        private byte[] fPassWord = new byte[4];
        private byte[] fOperID_6B = new byte[10];
        ArrayList list = new ArrayList();
        private List<string> epclist = new List<string>();
        private List<string> tidlist = new List<string>();
        private int CardNum1 = 0;
        private string fInventory_EPC_List; //存贮询查列表（如果读取的数据没有变化，则不进行刷新）
        private int frmcomportindex;
        private bool SeriaATflag = false;
        private byte Target = 0;
        private byte InAnt = 128;
        private byte Scantime = 20;
        private byte FastFlag = 1;
        private byte Qvalue = 4;
        private byte Session = 0;
        private int total_turns = 0;//轮数
        private int total_tagnum = 0;//标签数量
        private int CardNum = 0;
        private int total_time = 0;//总时间
        private int targettimes = 0;
        private byte TIDFlag = 0;
        private byte tidLen = 0;
        private byte tidAddr = 0;
        public static byte antinfo = 0;
        private int AA_times = 0;
        private int CommunicationTime = 0;
        public DeviceClass SelectedDevice;
        private static List<DeviceClass> DevList;
        private static SearchCallBack searchCallBack = new SearchCallBack(searchCB);
        private string ReadTypes = "";

        private volatile bool fIsInventoryScan = false;
        private volatile bool toStopThread = false;
        private Thread mythread = null;

        private static void searchCB(IntPtr dev, IntPtr data)
        {
            uint ipAddr = 0;
            StringBuilder devname = new StringBuilder(100);
            StringBuilder macAdd = new StringBuilder(100);
            //获取搜索到的设备信息；
            DevControl.tagErrorCode eCode = DevControl.DM_GetDeviceInfo(dev, ref ipAddr, macAdd, devname);
            if (eCode == DevControl.tagErrorCode.DM_ERR_OK)
            {
                //将搜索到的设备加入设备列表；
                DeviceClass device = new DeviceClass(dev, ipAddr, macAdd.ToString(), devname.ToString());
                DevList.Add(device);
            }
            else
            {
                //异常处理；
                string errMsg = ErrorHandling.GetErrorMsg(eCode);
                Log.WriteError(errMsg);
            }
        }

        public int EpcCounter
        {
            get { return ecpCounter; }
            set
            {
                ecpCounter = value;
                this.LB_EcpCounter.Text = string.Format("{0}", ecpCounter);
            }
        }

        RFIDCallBack elegateRFIDCallBack;
        public MainForm()
        {
            InitializeComponent();
            InitForm();
            InitProperty();

            //初始化设备控制模块；
            DevControl.tagErrorCode eCode = DevControl.DM_Init(searchCallBack, IntPtr.Zero);
            if (eCode != DevControl.tagErrorCode.DM_ERR_OK)
            {
                //如果初始化失败则关闭程序，并进行异常处理；
                string errMsg = ErrorHandling.HandleError(eCode);
                throw new Exception(errMsg);
            }
            elegateRFIDCallBack = new RFIDCallBack(GetUid);
        }

        ~MainForm()
        {
            if (frmcomportindex > 0)
                fCmdRet = RWDev.CloseSpecComPort(frmcomportindex);
            if (fCmdRet != 0)
            {
                string strLog = "Serialport Stop Failed: " + GetReturnCodeDesc(fCmdRet);
                WriteLog(mLogView.RtbLog, strLog, 1);
                return;
            }
            else
            {
                string strLog = "Serialport Stop Succeed!";
                WriteLog(mLogView.RtbLog, strLog, 0);
            }
        }

        string epcandtid = "";//标记整合数据
        int lastnum = 0;
        private void GetUid(IntPtr p, int nEvt)
        {
            RFIDTag ce = (RFIDTag)Marshal.PtrToStructure(p, typeof(RFIDTag));
            this.Invoke((EventHandler)delegate
            {
                IntPtr ptrWnd = IntPtr.Zero;
                ptrWnd = FindWindow(null, windowName);
                if (ptrWnd != IntPtr.Zero)         // 检查当前统计窗口是否打开
                {
                    {
                        int Antnum = ce.ANT;
                        string str_ant = Convert.ToString(Antnum, 2).PadLeft(4, '0');
                        string epclen = Convert.ToString(ce.LEN, 16);
                        if (epclen.Length == 1) epclen = "0" + epclen;
                        string para = str_ant + "," + epclen + ce.UID + "," + ce.RSSI.ToString() + " ";
                        SendMessage(ptrWnd, WM_SENDTAG, IntPtr.Zero, para);
                    }
                }
                total_tagnum++;
                CardNum++;
            });
        }

        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == WM_SENDTAG)
            {

                string tagInfo = Marshal.PtrToStringAnsi(m.LParam);
                string sEPC;
                string str_ant = tagInfo.Substring(0, 4);
                tagInfo = tagInfo.Substring(5);
                int index = tagInfo.IndexOf(',');
                sEPC = tagInfo.Substring(0, index);
                string str_epclen = sEPC.Substring(0, 2);
                byte epclen = Convert.ToByte(str_epclen, 16);
                sEPC = sEPC.Substring(2);
                index++;
                string RSSI = tagInfo.Substring(index);

                try
                {
                    SqlOperation.Instance.EpcRecordInsert(sEPC);
                    SqlOperation.Instance.EpcLabelInsert(sEPC, flagRecord);
                }
                catch (Exception ex)
                {
                    WriteLog(mLogView.RtbLog, ex.Message, 1);
                }

                DataTable dt = dataGridView1.DataSource as DataTable;

                if (dt == null)
                {
                    dt = new DataTable();
                    dt.Columns.Add("Column1", Type.GetType("System.String"));
                    dt.Columns.Add("Column2", Type.GetType("System.String"));
                    dt.Columns.Add("Column3", Type.GetType("System.String"));
                    dt.Columns.Add("Column4", Type.GetType("System.String"));
                    dt.Columns.Add("Column5", Type.GetType("System.String"));
                    DataRow dr = dt.NewRow();
                    dr["Column1"] = (dt.Rows.Count + 1).ToString();
                    dr["Column2"] = sEPC;
                    dr["Column3"] = "1";
                    dr["Column4"] = RSSI;
                    dr["Column5"] = str_ant;
                    dt.Rows.Add(dr);
                    this.EpcCounter = dt.Rows.Count;
                }
                else
                {
                    DataRow[] dr;
                    dr = dt.Select("Column2='" + sEPC + "'");
                    if (dr.Length == 0)     // epc号不存在
                    {
                        DataRow dr2 = dt.NewRow();
                        dr2["Column1"] = (dt.Rows.Count + 1).ToString();
                        dr2["Column2"] = sEPC;
                        dr2["Column3"] = "1";
                        dr2["Column4"] = RSSI;
                        dr2["Column5"] = str_ant;
                        dt.Rows.Add(dr2);
                        this.EpcCounter = dt.Rows.Count;
                    }
                    else     // epc号已存在
                    {
                        int cnt = int.Parse(dr[0]["Column3"].ToString());
                        cnt++;
                        dt.Rows[dt.Rows.IndexOf(dr[0])]["Column3"] = cnt.ToString();
                        dt.Rows[dt.Rows.IndexOf(dr[0])]["Column4"] = RSSI;
                        int ant1 = Convert.ToInt32(dr[0]["Column5"].ToString(), 2);
                        int ant2 = Convert.ToInt32(str_ant, 2);
                        dt.Rows[dt.Rows.IndexOf(dr[0])]["Column5"] = Convert.ToString((ant1 | ant2), 2).PadLeft(4, '0');
                    }
                }
                bool flagset = false;
                flagset = (dataGridView1.DataSource == null) ? true : false;
                dataGridView1.DataSource = dt;

                if (flagset)
                {
                    dataGridView1.Columns["Column1"].HeaderText = "Index";
                    dataGridView1.Columns["Column1"].Width = 80;
                    dataGridView1.Columns["Column2"].HeaderText = "EPC";
                    dataGridView1.Columns["Column2"].Width = 300;
                    dataGridView1.Columns["Column3"].HeaderText = "Times";
                    dataGridView1.Columns["Column3"].Width = 80;
                    dataGridView1.Columns["Column4"].HeaderText = "RSSI";
                    dataGridView1.Columns["Column4"].Width = 80;
                    dataGridView1.Columns["Column5"].HeaderText = "Ante(4-1)";
                    dataGridView1.Columns["Column5"].Width = 100;
                }
            }
            else if (m.Msg == WM_MIXTAG)
            {
                string tagInfo = Marshal.PtrToStringAnsi(m.LParam);
                string sEPC, sData;
                string str_ant = tagInfo.Substring(0, 4);
                tagInfo = tagInfo.Substring(5);

                int index = tagInfo.IndexOf(',');
                sEPC = tagInfo.Substring(0, index);
                int n = sEPC.IndexOf("-");
                sData = sEPC.Substring(n + 1);
                sEPC = sEPC.Substring(0, n);
                index++;
                string RSSI = tagInfo.Substring(index);

                DataTable dt = dataGridView1.DataSource as DataTable;

                if (dt == null)
                {
                    dt = new DataTable();
                    dt.Columns.Add("Column1", Type.GetType("System.String"));
                    dt.Columns.Add("Column2", Type.GetType("System.String"));
                    dt.Columns.Add("Column3", Type.GetType("System.String"));
                    dt.Columns.Add("Column4", Type.GetType("System.String"));
                    dt.Columns.Add("Column5", Type.GetType("System.String"));
                    dt.Columns.Add("Column6", Type.GetType("System.String"));
                    DataRow dr = dt.NewRow();
                    dr["Column1"] = (dt.Rows.Count + 1).ToString();
                    dr["Column2"] = sEPC;
                    dr["Column3"] = sData;
                    dr["Column4"] = "1";
                    dr["Column5"] = RSSI;
                    dr["Column6"] = str_ant;
                    dt.Rows.Add(dr);
                    this.EpcCounter = dt.Rows.Count;
                }
                else
                {
                    DataRow[] dr;
                    dr = dt.Select("Column2='" + sEPC + "'");
                    if (dr.Length == 0)     // epc号不存在
                    {
                        DataRow dr2 = dt.NewRow();
                        dr2["Column1"] = (dt.Rows.Count + 1).ToString();
                        dr2["Column2"] = sEPC;
                        dr2["Column3"] = sData;
                        dr2["Column4"] = "1";
                        dr2["Column5"] = RSSI;
                        dr2["Column6"] = str_ant;
                        dt.Rows.Add(dr2);
                        this.EpcCounter = dt.Rows.Count;
                    }
                    else     // epc号已存在
                    {
                        int cnt = int.Parse(dr[0]["Column4"].ToString());
                        cnt++;
                        dt.Rows[dt.Rows.IndexOf(dr[0])]["Column4"] = cnt.ToString();
                        dt.Rows[dt.Rows.IndexOf(dr[0])]["Column5"] = RSSI;
                        int ant1 = Convert.ToInt32(dr[0]["Column6"].ToString(), 2);
                        int ant2 = Convert.ToInt32(str_ant, 2);
                        dt.Rows[dt.Rows.IndexOf(dr[0])]["Column6"] = Convert.ToString((ant1 | ant2), 2).PadLeft(4, '0');
                    }
                }
                bool flagset = false;
                flagset = (dataGridView1.DataSource == null) ? true : false;
                dataGridView1.DataSource = dt;

                if (flagset)
                {
                    dataGridView1.Columns["Column1"].HeaderText = "Index";
                    dataGridView1.Columns["Column1"].Width = 60;

                    dataGridView1.Columns["Column2"].HeaderText = "EPC";
                    dataGridView1.Columns["Column2"].Width = 280;

                    dataGridView1.Columns["Column3"].HeaderText = "Data";
                    dataGridView1.Columns["Column3"].Width = 150;

                    dataGridView1.Columns["Column4"].HeaderText = "Times";
                    dataGridView1.Columns["Column4"].Width = 60;

                    dataGridView1.Columns["Column5"].HeaderText = "RSSI";
                    dataGridView1.Columns["Column5"].Width = 60;

                    dataGridView1.Columns["Column6"].HeaderText = "Ante(4-1)";
                    dataGridView1.Columns["Column6"].Width = 60;
                }
            }
            else if (m.Msg == WM_FASTID)
            {
                string tagInfo = Marshal.PtrToStringAnsi(m.LParam);
                string sEPC = "";
                string sTID = "";
                string str_ant = tagInfo.Substring(0, 4);
                tagInfo = tagInfo.Substring(5);
                int index = tagInfo.IndexOf(',');
                sEPC = tagInfo.Substring(0, index);
                string str_epclen = sEPC.Substring(0, 2);
                byte nlen = Convert.ToByte(str_epclen, 16);
                if ((nlen & 0x80) == 0)
                {
                    sEPC = sEPC.Substring(2);//只有EPC
                    if (epclist.IndexOf(sEPC) == -1)
                    {
                        epclist.Add(sEPC);
                    }
                    this.EpcCounter = epclist.Count;
                }
                else
                {
                    int epclen = (nlen & 0x7F) - 12;
                    sTID = sEPC.Substring(2 + epclen * 2, 24);
                    sEPC = sEPC.Substring(2, epclen * 2);
                    if (epclist.IndexOf(sEPC) == -1)
                    {
                        epclist.Add(sEPC);
                    }
                    if (tidlist.IndexOf(sTID) == -1)
                    {
                        tidlist.Add(sTID);
                    }
                    this.EpcCounter = epclist.Count;

                }
                index++;
                string RSSI = tagInfo.Substring(index);

                DataTable dt = dataGridView1.DataSource as DataTable;

                if (dt == null)
                {
                    dt = new DataTable();
                    dt.Columns.Add("Column1", Type.GetType("System.String"));
                    dt.Columns.Add("Column2", Type.GetType("System.String"));
                    dt.Columns.Add("Column3", Type.GetType("System.String"));
                    dt.Columns.Add("Column4", Type.GetType("System.String"));
                    dt.Columns.Add("Column5", Type.GetType("System.String"));
                    dt.Columns.Add("Column6", Type.GetType("System.String"));
                    DataRow dr = dt.NewRow();
                    dr["Column1"] = (dt.Rows.Count + 1).ToString();
                    dr["Column2"] = sEPC;
                    dr["Column3"] = sTID;
                    dr["Column4"] = "1";
                    dr["Column5"] = RSSI;
                    dr["Column6"] = str_ant;
                    dt.Rows.Add(dr);
                }
                else
                {
                    DataRow[] dr;
                    dr = dt.Select("Column2='" + sEPC + "' and Column3='" + sTID + "'");
                    if (dr.Length == 0)     // epc号不存在
                    {
                        DataRow dr2 = dt.NewRow();
                        dr2["Column1"] = (dt.Rows.Count + 1).ToString();
                        dr2["Column2"] = sEPC;
                        dr2["Column3"] = sTID;
                        dr2["Column4"] = "1";
                        dr2["Column5"] = RSSI;
                        dr2["Column6"] = str_ant;
                        dt.Rows.Add(dr2);
                    }
                    else     // epc号已存在
                    {
                        int cnt = int.Parse(dr[0]["Column4"].ToString());
                        cnt++;
                        dt.Rows[dt.Rows.IndexOf(dr[0])]["Column4"] = cnt.ToString();
                        dt.Rows[dt.Rows.IndexOf(dr[0])]["Column5"] = RSSI;
                        int ant1 = Convert.ToInt32(dr[0]["Column6"].ToString(), 2);
                        int ant2 = Convert.ToInt32(str_ant, 2);
                        dt.Rows[dt.Rows.IndexOf(dr[0])]["Column6"] = Convert.ToString((ant1 | ant2), 2).PadLeft(4, '0');
                    }
                }
                bool flagset = false;
                flagset = (dataGridView1.DataSource == null) ? true : false;
                dataGridView1.DataSource = dt;

                if (flagset)
                {
                    dataGridView1.Columns["Column1"].HeaderText = "Index";
                    dataGridView1.Columns["Column1"].Width = 60;

                    dataGridView1.Columns["Column2"].HeaderText = "EPC";
                    dataGridView1.Columns["Column2"].Width = 280;

                    dataGridView1.Columns["Column3"].HeaderText = "TID";
                    dataGridView1.Columns["Column3"].Width = 150;

                    dataGridView1.Columns["Column4"].HeaderText = "Times";
                    dataGridView1.Columns["Column4"].Width = 60;

                    dataGridView1.Columns["Column5"].HeaderText = "RSSI";
                    dataGridView1.Columns["Column5"].Width = 60;

                    dataGridView1.Columns["Column6"].HeaderText = "Ante(4-1)";
                    dataGridView1.Columns["Column6"].Width = 60;
                }
            }
            else if (m.Msg == WM_SENDTAGSTAT)
            {
                string tagInfo = Marshal.PtrToStringAnsi(m.LParam);
                int index = tagInfo.IndexOf(',');
                string tagRate = tagInfo.Substring(0, index);
                index++;
                string str = tagInfo.Substring(index);
                index = str.IndexOf(',');
                string tagNum = str.Substring(0, index);
                index++;
                string cmdTime = str.Substring(index);
            }
            else if (m.Msg == WM_SENDSTATU)
            {
                string Info = Marshal.PtrToStringAnsi(m.LParam);
                fCmdRet = Convert.ToInt32(Info);
                string strLog = "Check Label: " + GetReturnCodeDesc(fCmdRet);
                WriteLog(mLogView.RtbLog, strLog, 1);
            }
            else if (m.Msg == WM_SENDBUFF)
            {
                string tagInfo = Marshal.PtrToStringAnsi(m.LParam);
                int index = tagInfo.IndexOf(',');
                string tagNum = tagInfo.Substring(0, index);
                index++;

                string str = tagInfo.Substring(index);
                index = str.IndexOf(',');
                string cmdTime = str.Substring(0, index);
                index++;

                str = str.Substring(index);
                index = str.IndexOf(',');
                string tagRate = str.Substring(0, index);
                index++;

                str = str.Substring(index);
                string total_tagnum = str;

                WriteLog(mLogView.RtbLog, "Check Buff: Succeed ", 1);
            }
            else if (m.Msg == WM_START)
            {
                string tagInfo = Marshal.PtrToStringAnsi(m.LParam);
                if (tagInfo.Contains("Input"))
                {
                    flagRecord = 2;
                    this.mReadOutput.SetClickButtonEnable(false);
                }
                else
                {
                    flagRecord = 1;
                    this.mReadInput.SetClickButtonEnable(false);
                }
                toStopThread = false;
                mythread = new Thread(new ThreadStart(Inventory));
                mythread.IsBackground = true;
                mythread.Start();
                
            }
            else if (m.Msg == WM_STOP)
            {
                string tagInfo = Marshal.PtrToStringAnsi(m.LParam);
                flagRecord = 0;
                if (tagInfo.Contains("Input"))
                {
                    this.mReadInput.SetClickButtonEnable(false);
                    this.mReadInput.SetButtonText("Stopping");
                }
                else
                {
                    this.mReadOutput.SetClickButtonEnable(false);
                    this.mReadOutput.SetButtonText("Stopping");
                }
                toStopThread = true;
            }
            else if (m.Msg == WM_SETPOWER)
            {
                if (toStopThread == true && !Connect232())
                {
                    toStopThread = true;
                }

                string tagInfo = Marshal.PtrToStringAnsi(m.LParam);
                byte powerDbm = byte.Parse(tagInfo);
                fCmdRet = RWDev.SetRfPower(ref fComAdr, powerDbm, frmcomportindex);
                if (fCmdRet != 0)
                {
                    string strLog = "Set RF power failed, the reason is: " + GetReturnCodeDesc(fCmdRet);
                    WriteLog(mLogView.RtbLog, strLog, 1);
                }
                else
                {
                    string strLog = "Set RF power succeed ";
                    WriteLog(mLogView.RtbLog, strLog, 0);
                }

                if (toStopThread == true)
                    RWDev.CloseComPort();

            }
            else if (m.Msg == WM_SETREADERADDR)
            {
                if (toStopThread == true && !Connect232())
                {
                    toStopThread = true;
                }

                string tagInfo = Marshal.PtrToStringAnsi(m.LParam);
                byte aNewComAdr = Convert.ToByte(tagInfo, 16);
                fCmdRet = RWDev.SetAddress(ref fComAdr, aNewComAdr, frmcomportindex);
                if (fCmdRet != 0)
                {
                    string strLog = "Set reader address failed, the reason is: " + GetReturnCodeDesc(fCmdRet);
                    WriteLog(mLogView.RtbLog, strLog, 1);
                }
                else
                {
                    string strLog = "Set reader address succeed ";
                    WriteLog(mLogView.RtbLog, strLog, 0);
                }

                if (toStopThread == true)
                    RWDev.CloseComPort();
            }
            else if (m.Msg == WM_SETREADERID)
            {
                if (toStopThread == true && !Connect232())
                {
                    toStopThread = true;
                }

                string tagInfo = Marshal.PtrToStringAnsi(m.LParam);
                byte[] SeriaNo = new byte[4];
                fCmdRet = RWDev.GetSeriaNo(ref fComAdr, SeriaNo, frmcomportindex);
                if (fCmdRet != 0)
                {
                    string strLog = "Get reader ID failed, the reason is: " + GetReturnCodeDesc(fCmdRet);
                    WriteLog(mLogView.RtbLog, strLog, 1);
                }
                else
                {
                    string id = ByteArrayToHexString(SeriaNo);
                    mSetting.SetReaderID(id);
                    string strLog = "Get reader ID succeed ";
                    WriteLog(mLogView.RtbLog, strLog, 0);
                }

                if (toStopThread == true)
                    RWDev.CloseComPort();
            }
            else if (m.Msg == WM_SETSOUNDSWITCH)
            {
                if (toStopThread == true && !Connect232())
                {
                    toStopThread = true;
                }

                string tagInfo = Marshal.PtrToStringAnsi(m.LParam);
                byte BeepEn = 0;
                if (tagInfo.Contains("open"))
                    BeepEn = 1;
                else
                    BeepEn = 0;
                fCmdRet = RWDev.SetBeepNotification(ref fComAdr, BeepEn, frmcomportindex);
                if (fCmdRet != 0)
                {
                    string strLog = "Set buzzer switch failed, the reason is: " + GetReturnCodeDesc(fCmdRet);
                    WriteLog(mLogView.RtbLog, strLog, 1);
                }
                else
                {
                    string strLog = "Set buzzer switch succeed ";
                    WriteLog(mLogView.RtbLog, strLog, 0);
                }

                if (toStopThread == true)
                    RWDev.CloseComPort();
            }
            else if (m.Msg == WM_SETMAXTIME)
            {
                if (toStopThread == true && !Connect232())
                {
                    toStopThread = true;
                }

                string tagInfo = Marshal.PtrToStringAnsi(m.LParam);
                byte Scantime = 0;
                Scantime = Convert.ToByte(tagInfo);
                fCmdRet = RWDev.SetInventoryScanTime(ref fComAdr, Scantime, frmcomportindex);
                if (fCmdRet != 0)
                {
                    string strLog = "Set maximum instruction response time failed，the reason is: " + GetReturnCodeDesc(fCmdRet);
                    WriteLog(mLogView.RtbLog, strLog, 1);
                }
                else
                {
                    string strLog = "Set maximum instruction response time succeed ";
                    WriteLog(mLogView.RtbLog, strLog, 0);
                }
                if(toStopThread == true)
                    RWDev.CloseComPort();
            }
            else
                base.DefWndProc(ref m);
        }

        private void Inventory()
        {
            if (!Connect232())
            {
                toStopThread = true;
            }

            fIsInventoryScan = true;
            while (!toStopThread)
            {
                Flash_G2();
                Thread.Sleep(5);
            }

            this.Invoke((EventHandler)delegate
            {
                if (fIsInventoryScan)
                {
                    toStopThread = true;//标志，接收数据线程判断stop为true，正常情况下会自动退出线程
                    mythread.Abort();//若线程无法退出，强制结束
                    fIsInventoryScan = false;
                    mReadInput.IsRuning = false;
                    mReadInput.SetClickButtonEnable(true);
                    mReadInput.SetButtonText("Start Into");
                    mReadOutput.IsRuning = false;
                    mReadOutput.SetClickButtonEnable(true);
                    mReadOutput.SetButtonText("Start Out");
                    RWDev.CloseComPort();
                }
                fIsInventoryScan = false;
            });
        }

        private void Flash_G2()
        {
            switch (flagRecord)
            {
                case 2:
                    InAnt = mReadInput.GetCurAntCode();
                    break;
                case 1:
                    InAnt = mReadOutput.GetCurAntCode();
                    break;
                default:
                    return;
            }
            byte Ant = 0;
            int TagNum = 0;
            int Totallen = 0;
            int EPClen, m;
            byte[] EPC = new byte[50000];
            int CardIndex;
            string temps, temp;
            temp = "";
            string sEPC;
            byte MaskMem = 0;
            byte[] MaskAdr = new byte[2];
            byte MaskLen = 0;
            byte[] MaskData = new byte[100];
            byte MaskFlag = 0;
            MaskFlag = 0;
            int cbtime = System.Environment.TickCount;
            CardNum = 0;
            fCmdRet = RWDev.Inventory_G2(ref fComAdr, Qvalue, Session, MaskMem, MaskAdr, MaskLen, MaskData, MaskFlag, tidAddr, tidLen, TIDFlag, Target, InAnt, Scantime, FastFlag, EPC, ref Ant, ref Totallen, ref TagNum, frmcomportindex);
            int cmdTime = System.Environment.TickCount - cbtime;//命令时间
            
            if (fCmdRet == 0x30)
            {
                CardNum = 0;
            }
            if (CardNum == 0)
            {
                if (Session > 1)
                    AA_times = AA_times + 1;//没有得到标签只更新状态栏
            }
            else
                AA_times = 0;
            if ((fCmdRet == 1) || (fCmdRet == 2) || (fCmdRet == 0xFB) || (fCmdRet == 0x26))
            {
                if (cmdTime > CommunicationTime)
                    cmdTime = cmdTime - CommunicationTime;//减去通讯时间等于标签的实际时间
                if (cmdTime > 0)
                {
                    int tagrate = (CardNum * 1000) / cmdTime;//速度等于张数/时间
                    IntPtr ptrWnd = IntPtr.Zero;
                    ptrWnd = FindWindow(null, windowName);
                    if (ptrWnd != IntPtr.Zero)         // 检查当前统计窗口是否打开
                    {
                        string para = tagrate.ToString() + "," + total_tagnum.ToString() + "," + cmdTime.ToString();
                        SendMessage(ptrWnd, WM_SENDTAGSTAT, IntPtr.Zero, para);
                    }
                }

            }
        }

        private delegate void WriteLogUnSafe(CustomControl.LogRichTextBox logRichTxt, string strLog, int nType);
        private void WriteLog(CustomControl.LogRichTextBox logRichTxt, string strLog, int nType)
        {
            if (this.InvokeRequired)
            {
                WriteLogUnSafe InvokeWriteLog = new WriteLogUnSafe(WriteLog);
                this.Invoke(InvokeWriteLog, new object[] { logRichTxt, strLog, nType });
            }
            else
            {
                if ((nType == 0) || (nType == 0x26) || (nType == 0x01) || (nType == 0x02) || (nType == 0xFB))
                {
                    logRichTxt.AppendTextEx(strLog, Color.Indigo);
                }
                else
                {
                    logRichTxt.AppendTextEx(strLog, Color.Red);
                }

                logRichTxt.Select(logRichTxt.TextLength, 0);
                logRichTxt.ScrollToCaret();
            }
        }

        /// <summary>
        /// 错误代码
        /// </summary>
        /// <param name="cmdRet"></param>
        /// <returns></returns>
        #region 
        private string GetReturnCodeDesc(int cmdRet)
        {
            switch (cmdRet)
            {
                case 0x00:
                case 0x26:
                    return "操作成功";
                case 0x01:
                    return "询查时间结束前返回";
                case 0x02:
                    return "指定的询查时间溢出";
                case 0x03:
                    return "本条消息之后，还有消息";
                case 0x04:
                    return "读写模块存储空间已满";
                case 0x05:
                    return "访问密码错误";
                case 0x09:
                    return "销毁密码错误";
                case 0x0a:
                    return "销毁密码不能为全0";
                case 0x0b:
                    return "电子标签不支持该命令";
                case 0x0c:
                    return "对该命令，访问密码不能为全0";
                case 0x0d:
                    return "电子标签已经被设置了读保护，不能再次设置";
                case 0x0e:
                    return "电子标签没有被设置读保护，不需要解锁";
                case 0x10:
                    return "有字节空间被锁定，写入失败";
                case 0x11:
                    return "不能锁定";
                case 0x12:
                    return "已经锁定，不能再次锁定";
                case 0x13:
                    return "参数保存失败,但设置的值在读写模块断电前有效";
                case 0x14:
                    return "无法调整";
                case 0x15:
                    return "询查时间结束前返回";
                case 0x16:
                    return "指定的询查时间溢出";
                case 0x17:
                    return "本条消息之后，还有消息";
                case 0x18:
                    return "读写模块存储空间已满";
                case 0x19:
                    return "电子不支持该命令或者访问密码不能为0";
                case 0x1A:
                    return "标签自定义功能执行错误";
                case 0xF8:
                    return "检测天线错误";
                case 0xF9:
                    return "命令执行出错";
                case 0xFA:
                    return "有电子标签，但通信不畅，无法操作";
                case 0xFB:
                    return "无电子标签可操作";
                case 0xFC:
                    return "电子标签返回错误代码";
                case 0xFD:
                    return "命令长度错误";
                case 0xFE:
                    return "不合法的命令";
                case 0xFF:
                    return "参数错误";
                case 0x30:
                    return "通讯错误";
                case 0x31:
                    return "CRC校验错误";
                case 0x32:
                    return "返回数据长度有错误";
                case 0x33:
                    return "通讯繁忙，设备正在执行其他指令";
                case 0x34:
                    return "繁忙，指令正在执行";
                case 0x35:
                    return "端口已打开";
                case 0x36:
                    return "端口已关闭";
                case 0x37:
                    return "无效句柄";
                case 0x38:
                    return "无效端口";
                case 0xEE:
                    return "命令代码错误";
                default:
                    return Convert.ToString(cmdRet, 16);
            }
        }
        private string GetErrorCodeDesc(int cmdRet)
        {
            switch (cmdRet)
            {
                case 0x00:
                    return "其它错误";
                case 0x03:
                    return "存储器超限或不被支持的PC值";
                case 0x04:
                    return "存储器锁定";
                case 0x0b:
                    return "电源不足";
                case 0x0f:
                    return "非特定错误";
                default:
                    return "";
            }
        }
        #endregion

        void InitForm()
        {
            mSetting = new Setting();
            mReadInput = new ReadLabel(true);
            mReadOutput = new ReadLabel(false);
            mLogView = new LogView();
            mSetting.Dock = DockStyle.Fill;
            mReadInput.Dock = DockStyle.Fill;
            mReadOutput.Dock = DockStyle.Fill;
            mLogView.Dock = DockStyle.Fill;

            this.panelCenter.Controls.Clear();
            this.panelCenter.Controls.Add(mSetting);

            this.btnSetting.Select();
        }

        void InitProperty()
        {
            this.EpcCounter = 0;
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            this.panelCenter.Controls.Clear();
            this.panelCenter.Controls.Add(mSetting);
        }

        private void btnInput_Click(object sender, EventArgs e)
        {
            this.panelCenter.Controls.Clear();
            this.panelCenter.Controls.Add(mReadInput);
        }

        private void btnOutput_Click(object sender, EventArgs e)
        {
            this.panelCenter.Controls.Clear();
            this.panelCenter.Controls.Add(mReadOutput);
        }

        private void btn_Log_Click(object sender, EventArgs e)
        {
            this.panelCenter.Controls.Clear();
            this.panelCenter.Controls.Add(mLogView);
        }

        private void btnRecount_Click(object sender, EventArgs e)
        {
            this.EpcCounter = 0;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            DataTable dt = dataGridView1.DataSource as DataTable;
            dt.Clear();
        }

        private bool Connect232()
        {
            int portNum = 0;
            int FrmPortIndex = 0;
            string strException = string.Empty;
            fBaud = 5;
            fComAdr = 0xFF; //广播地址打开设备
            fCmdRet = RWDev.AutoOpenComPort(ref portNum, ref fComAdr, fBaud, ref FrmPortIndex);
            if (fCmdRet != 0)
            {
                string strLog = "Connect reader failed, the reason is:  " + GetReturnCodeDesc(fCmdRet);
                WriteLog(mLogView.RtbLog, strLog, 1);
                return false;
            }
            else
            {
                frmcomportindex = FrmPortIndex;
                string strLog = "Connect the reader " + portNum.ToString() + "@" + fBaud.ToString();
                WriteLog(mLogView.RtbLog, strLog, 0);
            }

            if (FrmPortIndex > 0)
                RWDev.InitRFIDCallBack(elegateRFIDCallBack, true, FrmPortIndex);

            return true;
        }

        public static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        public static string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            return sb.ToString().ToUpper();

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string epcId = "";
            ResultStruct result;
            if (e.RowIndex > -1)
            {
                epcId = this.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                try
                {
                    if (HttpHelper.GetLabelProperty(epcId, out result))
                    {
                        switch (flagRecord)
                        {
                            case 2:
                                this.mReadInput.SetProperty(result);
                                break;
                            case 1:
                                this.mReadOutput.SetProperty(result);
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
