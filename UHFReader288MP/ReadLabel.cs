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
using System.Net.Http;
using System.IO;
using GroupDocs.Conversion;
using GroupDocs.Conversion.FileTypes;
using GroupDocs.Conversion.Options.Convert;

namespace UHFReader288MP
{
    public partial class ReadLabel : UserControl
    {
        const string windowName = "Dashboard";

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
            this.Pic = res.pic;
            this.InMoney = res.in_money;
            this.OutMoney = res.out_money;
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

        private string pic;

        public string Pic
        {
            get { return pic; }
            set
            {
                pic = value;
                try
                {
                    this.pictureBox.Image = null;
                    this.pictureBox.WaitOnLoad = false; //设置为异步加载图片
                    this.pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    //this.pictureBox.Load(pic); //"http://image.photophoto.cn/nm-7/003/028/0030280465.jpg"
                    string webpUrl = "./tempWebp.webp";
                    string jpgUrl = "./tempJpg.jpg";

                    if (HttpHelper.GetImgJpg(webpUrl, jpgUrl, pic))
                    {
                        this.pictureBox.Image = GetFile(jpgUrl);
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteError(ex.Message);
                }
            }
        }

        ///
        /// 将文件转为内存流
        ///
        /// 
        /// 
        private MemoryStream ReadFile(string path)
        {
            if (!File.Exists(path))
                return null;

            using (FileStream file = new FileStream(path, FileMode.Open))
            {
                byte[] b = new byte[file.Length];
                file.Read(b, 0, b.Length);

                MemoryStream stream = new MemoryStream(b);
                return stream;
            }
        }

        ///
        /// 将内存流转为图片
        ///
        /// 
        /// 
        private Image GetFile(string path)
        {
            MemoryStream stream = ReadFile(path);
            return stream == null ? null : Image.FromStream(stream);
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
                this.LB_WarehouseNameValue.Text = warehouseName;
            }
        }

        public Image GetHttpImage(string url)
        {

            var client = new HttpClient();

            var uri = new Uri(Uri.EscapeUriString(url));
            byte[] urlContents = client.GetByteArrayAsync(uri).Result;

            using (var ms = new System.IO.MemoryStream(urlContents))
            {
                return Image.FromStream(ms);
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
