using System;
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
using UHFReader288MPDemo;

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
                this.btnSwitch.Text = "Start Into";
            }
            else
            {
                this.btnSwitch.Text = "Start Out";
            }
        }

        public void SetProperty(ResultStruct res)
        {
            this.TagId = res.tag;
            this.TagSize = res.size;
            this.TagType = res.type;
            this.TagColor = res.color;
            this.Pic = res.pic;
            this.InMoney = res.in_money;
            this.OutMoney = res.out_money;
            this.SupplierId = res.supplier_id;
            this.WarehouseId = res.warehouse_id;
            this.TypeName = res.type_name;
            this.ColorName = res.color_name;
            this.SupplierName = res.supplier_name;
            this.WarehouseName = res.warehouse_name;
        }

        void InitProperty()
        {
            this.IsRuning = false;
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

        private string tagId;

        public string TagId
        {
            get { return tagId; }
            set
            {
                tagId = value;
                this.LB_TagValue.Text = tagId;
            }
        }

        private string tagSize;

        public string TagSize
        {
            get { return tagSize; }
            set
            {
                tagSize = value;
                this.LB_SizeValue.Text = tagSize;
            }
        }

        private string tagType;

        public string TagType
        {
            get { return tagType; }
            set
            {
                tagType = value;
                this.LB_TypeValue.Text = tagType;
            }
        }

        private string tagColor;

        public string TagColor
        {
            get { return tagColor; }
            set
            {
                tagColor = value;
                this.LB_ColorValue.Text = tagColor;
            }
        }

        private string pic;

        public string Pic
        {
            get { return pic; }
            set
            {
                pic = value;
                try
                {
                    System.Net.WebRequest webreq = System.Net.WebRequest.Create(pic);
                    System.Net.WebResponse webres = webreq.GetResponse();
                    using (System.IO.Stream stream = webres.GetResponseStream())
                    {
                        pictureBox.Image = Image.FromStream(stream);
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteError(ex.Message);
                }
            }
        }

        private string inMoney;

        public string InMoney
        {
            get { return inMoney; }
            set
            {
                inMoney = value;
                this.LB_InMoneyValue.Text = inMoney;
            }
        }

        private string outMoney;

        public string OutMoney
        {
            get { return outMoney; }
            set
            {
                outMoney = value;
                this.LB_OutMoneyValue.Text = outMoney;
            }
        }

        private string supplierId;

        public string SupplierId
        {
            get { return supplierId; }
            set
            {
                supplierId = value;
                this.LB_SuypplierIDValue.Text = supplierId;
            }
        }

        private string warehouseId;

        public string WarehouseId
        {
            get { return warehouseId; }
            set
            {
                warehouseId = value;
                this.LB_WarehouseIDValue.Text = warehouseId;
            }
        }

        private string typeName;

        public string TypeName
        {
            get { return typeName; }
            set
            {
                typeName = value;
                this.LB_TypeNameValue.Text = typeName;
            }
        }

        private string colorName;

        public string ColorName
        {
            get { return colorName; }
            set
            {
                colorName = value;
                this.LB_ColorNameValue.Text = colorName;
            }
        }

        private string supplierName;

        public string SupplierName
        {
            get { return supplierName; }
            set
            {
                supplierName = value;
                this.LB_SupplierNameValue.Text = supplierName;
            }
        }

        private string warehouseName;

        public string WarehouseName
        {
            get { return warehouseName; }
            set
            {
                warehouseName = value;
                this.LB_WarehouseNameValue.Text = warehouseId;
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
                MessageBox.Show("Please check out the MySql Server!");
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
                MessageBox.Show("Please Select the antenna!");
                return;
            }

            if (IsRuning)
            {
                IsRuning = false;
                btnSwitch.Enabled = false;
                btnSwitch.BackColor = Color.Transparent;
                btnSwitch.Text = "Stopping";

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
                btnSwitch.Text = "Stop";

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
        
    }
}
