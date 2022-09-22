namespace UHFReader288MP
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.btn_Log = new System.Windows.Forms.Button();
            this.btnOutput = new System.Windows.Forms.Button();
            this.btnInput = new System.Windows.Forms.Button();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.LB_EcpCounter = new System.Windows.Forms.Label();
            this.btnRecount = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.panelCenter = new System.Windows.Forms.Panel();
            this.btnSetting = new UHFReader288MP.RoundButton();
            this.roundButton1 = new UHFReader288MP.RoundButton();
            this.roundButton2 = new UHFReader288MP.RoundButton();
            this.roundButton3 = new UHFReader288MP.RoundButton();
            this.panel1.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel7.SuspendLayout();
            this.panel8.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(57)))), ((int)(((byte)(93)))));
            this.panel1.Controls.Add(this.panel6);
            this.panel1.Controls.Add(this.panel5);
            this.panel1.Controls.Add(this.panel4);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.roundButton3);
            this.panel6.Controls.Add(this.roundButton2);
            this.panel6.Controls.Add(this.roundButton1);
            this.panel6.Controls.Add(this.btnSetting);
            this.panel6.Controls.Add(this.btn_Log);
            this.panel6.Controls.Add(this.btnOutput);
            this.panel6.Controls.Add(this.btnInput);
            resources.ApplyResources(this.panel6, "panel6");
            this.panel6.Name = "panel6";
            // 
            // btn_Log
            // 
            resources.ApplyResources(this.btn_Log, "btn_Log");
            this.btn_Log.Name = "btn_Log";
            this.btn_Log.UseVisualStyleBackColor = true;
            this.btn_Log.Click += new System.EventHandler(this.btn_Log_Click);
            // 
            // btnOutput
            // 
            resources.ApplyResources(this.btnOutput, "btnOutput");
            this.btnOutput.Name = "btnOutput";
            this.btnOutput.UseVisualStyleBackColor = true;
            this.btnOutput.Click += new System.EventHandler(this.btnOutput_Click);
            // 
            // btnInput
            // 
            resources.ApplyResources(this.btnInput, "btnInput");
            this.btnInput.Name = "btnInput";
            this.btnInput.UseVisualStyleBackColor = true;
            this.btnInput.Click += new System.EventHandler(this.btnInput_Click);
            // 
            // panel5
            // 
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.Name = "panel5";
            // 
            // panel4
            // 
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Controls.Add(this.label2);
            this.panel4.Name = "panel4";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.groupBox1);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dataGridView1);
            this.groupBox1.Controls.Add(this.panel7);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.panel8);
            this.panel7.Controls.Add(this.btnRecount);
            this.panel7.Controls.Add(this.btnClear);
            resources.ApplyResources(this.panel7, "panel7");
            this.panel7.Name = "panel7";
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.label3);
            this.panel8.Controls.Add(this.LB_EcpCounter);
            resources.ApplyResources(this.panel8, "panel8");
            this.panel8.Name = "panel8";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // LB_EcpCounter
            // 
            resources.ApplyResources(this.LB_EcpCounter, "LB_EcpCounter");
            this.LB_EcpCounter.Name = "LB_EcpCounter";
            // 
            // btnRecount
            // 
            resources.ApplyResources(this.btnRecount, "btnRecount");
            this.btnRecount.Name = "btnRecount";
            this.btnRecount.UseVisualStyleBackColor = true;
            this.btnRecount.Click += new System.EventHandler(this.btnRecount_Click);
            // 
            // btnClear
            // 
            resources.ApplyResources(this.btnClear, "btnClear");
            this.btnClear.Name = "btnClear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // panelCenter
            // 
            resources.ApplyResources(this.panelCenter, "panelCenter");
            this.panelCenter.Name = "panelCenter";
            // 
            // btnSetting
            // 
            this.btnSetting.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(57)))), ((int)(((byte)(93)))));
            this.btnSetting.ControlState = UHFReader288MP.ControlState.Normal;
            resources.ApplyResources(this.btnSetting, "btnSetting");
            this.btnSetting.FlatAppearance.BorderSize = 0;
            this.btnSetting.HoverColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSetting.IsChecked = false;
            this.btnSetting.Name = "btnSetting";
            this.btnSetting.NormalColor = System.Drawing.SystemColors.ButtonFace;
            this.btnSetting.PressedColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnSetting.Radius = 5;
            this.btnSetting.UseVisualStyleBackColor = false;
            // 
            // roundButton1
            // 
            this.roundButton1.ControlState = UHFReader288MP.ControlState.Normal;
            resources.ApplyResources(this.roundButton1, "roundButton1");
            this.roundButton1.FlatAppearance.BorderSize = 0;
            this.roundButton1.HoverColor = System.Drawing.SystemColors.ButtonHighlight;
            this.roundButton1.IsChecked = false;
            this.roundButton1.Name = "roundButton1";
            this.roundButton1.NormalColor = System.Drawing.SystemColors.ButtonFace;
            this.roundButton1.PressedColor = System.Drawing.SystemColors.ButtonShadow;
            this.roundButton1.Radius = 5;
            this.roundButton1.UseVisualStyleBackColor = true;
            // 
            // roundButton2
            // 
            this.roundButton2.ControlState = UHFReader288MP.ControlState.Normal;
            resources.ApplyResources(this.roundButton2, "roundButton2");
            this.roundButton2.FlatAppearance.BorderSize = 0;
            this.roundButton2.HoverColor = System.Drawing.SystemColors.ButtonHighlight;
            this.roundButton2.IsChecked = false;
            this.roundButton2.Name = "roundButton2";
            this.roundButton2.NormalColor = System.Drawing.SystemColors.ButtonFace;
            this.roundButton2.PressedColor = System.Drawing.SystemColors.ButtonShadow;
            this.roundButton2.Radius = 5;
            this.roundButton2.UseVisualStyleBackColor = true;
            // 
            // roundButton3
            // 
            this.roundButton3.ControlState = UHFReader288MP.ControlState.Normal;
            resources.ApplyResources(this.roundButton3, "roundButton3");
            this.roundButton3.FlatAppearance.BorderSize = 0;
            this.roundButton3.HoverColor = System.Drawing.SystemColors.ButtonHighlight;
            this.roundButton3.IsChecked = false;
            this.roundButton3.Name = "roundButton3";
            this.roundButton3.NormalColor = System.Drawing.SystemColors.ButtonFace;
            this.roundButton3.PressedColor = System.Drawing.SystemColors.ButtonShadow;
            this.roundButton3.Radius = 5;
            this.roundButton3.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelCenter);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Name = "MainForm";
            this.panel1.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel7.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Button btnOutput;
        private System.Windows.Forms.Button btnInput;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label LB_EcpCounter;
        private System.Windows.Forms.Button btnRecount;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Panel panelCenter;
        private System.Windows.Forms.Button btn_Log;
        private RoundButton btnSetting;
        private RoundButton roundButton3;
        private RoundButton roundButton2;
        private RoundButton roundButton1;
    }
}

