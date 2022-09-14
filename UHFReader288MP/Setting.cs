using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UHFReader288MP
{
    public partial class Setting : UserControl
    {
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

            this.CB_WorkModel.Items.AddRange(new object[] {
                "应答模式",
                "实时模式",
                "触发模式"
            });

            this.CB_WorkModel.SelectedIndex = 0;
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
                SqlOperation.Instance.Addr = sqlAddr;
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
                SqlOperation.Instance.Port = int.Parse(sqlPort);
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
                SqlOperation.Instance.Account = sqlAccount;
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
                SqlOperation.Instance.PassWd = sqlPwd;
            }
        }



    }
}
