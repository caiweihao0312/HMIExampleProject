namespace SerialPortTool
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnAddPort;
        private System.Windows.Forms.TabControl tabControl;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnAddPort = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // btnAddPort
            // 
            this.btnAddPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddPort.Location = new System.Drawing.Point(700, 12);
            this.btnAddPort.Name = "btnAddPort";
            this.btnAddPort.Size = new System.Drawing.Size(88, 32);
            this.btnAddPort.TabIndex = 0;
            this.btnAddPort.Text = "添加串口";
            this.btnAddPort.UseVisualStyleBackColor = true;
            this.btnAddPort.Click += new System.EventHandler(this.btnAddPort_Click);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Location = new System.Drawing.Point(12, 60);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(776, 480);
            this.tabControl.TabIndex = 1;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(800, 552);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.btnAddPort);
            this.MinimumSize = new System.Drawing.Size(650, 450);
            this.Name = "MainForm";
            this.Text = "SerialPortTool 高级串口调试";
            this.ResumeLayout(false);
        }
    }
}