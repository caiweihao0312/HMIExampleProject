using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace SerialPortTool
{
    public partial class MainForm : Form
    {
        private int _tabCount = 0;
        private readonly Dictionary<TabPage, SerialPort> _portMap = new Dictionary<TabPage, SerialPort>();

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnAddPort_Click(object sender, EventArgs e)
        {
            var tab = new TabPage($"Port{++_tabCount}");
            CreatePortPanel(tab);
            tabControl.TabPages.Add(tab);
            tabControl.SelectedTab = tab;
        }

        private void CreatePortPanel(TabPage tab)
        {
            // 用 TableLayoutPanel 布局
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 7,
                Padding = new Padding(8),
                RowStyles =
                {
                    new RowStyle(SizeType.Absolute, 40), // 串口设置
                    new RowStyle(SizeType.Absolute, 30), // 进阶设置
                    new RowStyle(SizeType.Absolute, 30), // 接收选项
                    new RowStyle(SizeType.Percent, 100), // 接收区
                    new RowStyle(SizeType.Absolute, 40), // 发送设置
                    new RowStyle(SizeType.Absolute, 30), // 发送选项
                    new RowStyle(SizeType.Absolute, 24)  // 状态栏
                }
            };

            // Row 0: 串口基础设置
            var panel0 = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = false };
            panel0.Controls.AddRange(new Control[]
            {
                new Label { Text = "Port:", AutoSize = true, TextAlign = ContentAlignment.MiddleCenter },
                CreateComboBox(SerialPort.GetPortNames(), 80),
                new Label { Text = "Baud:", AutoSize = true, TextAlign = ContentAlignment.MiddleCenter },
                CreateComboBox(new[] { "9600","19200","38400","57600","115200" }, 80),
                new Button { Text = "打开", Width = 60 },
                new Button { Text = "关闭", Width = 60, Enabled = false }
            });
            layout.Controls.Add(panel0, 0, 0);

            // Row 1: 串口进阶设置
            var panel1 = new FlowLayoutPanel { Dock = DockStyle.Fill };
            panel1.Controls.AddRange(new Control[]
            {
                new Label { Text="DataBits:",AutoSize=true },
                CreateComboBox(new[] { "5","6","7","8" }, 60, "8"),
                new Label { Text="Parity:",AutoSize=true },
                CreateComboBox(Enum.GetNames(typeof(Parity)), 70, "None"),
                new Label { Text="StopBits:",AutoSize=true },
                CreateComboBox(Enum.GetNames(typeof(StopBits)), 70, "One"),
                new Label { Text="Flow:",AutoSize=true },
                CreateComboBox(Enum.GetNames(typeof(Handshake)), 80, "None")
            });
            layout.Controls.Add(panel1, 0, 1);

            // Row 2: 接收选项
            var panel2 = new FlowLayoutPanel { Dock = DockStyle.Fill };
            var chkHexRecv = new CheckBox { Text = "HEX接收", AutoSize = true };
            var chkTs = new CheckBox { Text = "带时间戳", AutoSize = true };
            var btnClearRx = new Button { Text = "清空接收", Width = 80 };
            panel2.Controls.AddRange(new Control[] { chkHexRecv, chkTs, btnClearRx });
            layout.Controls.Add(panel2, 0, 2);

            // Row 3: 接收区
            var rtbRecv = new RichTextBox { Dock = DockStyle.Fill, ReadOnly = true, BackColor = Color.White };
            layout.Controls.Add(rtbRecv, 0, 3);

            // Row 4: 发送设置
            var panel4 = new FlowLayoutPanel { Dock = DockStyle.Fill };
            var tbSend = new TextBox { Width = 250 };
            var btnSend = new Button { Text = "发送", Width = 60 };
            var chkAuto = new CheckBox { Text = "自动发送", AutoSize = true };
            var nudInterval = new NumericUpDown { Minimum = 100, Maximum = 10000, Value = 1000, Width = 60 };
            var btnClearTx = new Button { Text = "清空发送", Width = 80 };
            var btnExport = new Button { Text = "导出日志", Width = 80 };
            panel4.Controls.AddRange(new Control[] { tbSend, btnSend, chkAuto, nudInterval, btnClearTx, btnExport });
            layout.Controls.Add(panel4, 0, 4);

            // Row 5: 发送选项
            var panel5 = new FlowLayoutPanel { Dock = DockStyle.Fill };
            var chkHexSend = new CheckBox { Text = "HEX发送", AutoSize = true };
            panel5.Controls.Add(chkHexSend);
            layout.Controls.Add(panel5, 0, 5);

            // Row 6: 状态栏
            var lblStatus = new Label { Dock = DockStyle.Fill, Text = "已接收: 0 字节  已发送: 0 字节", TextAlign = ContentAlignment.MiddleLeft };
            layout.Controls.Add(lblStatus, 0, 6);

            tab.Controls.Add(layout);

            // 提取控件引用
            var cbPort = (ComboBox)panel0.Controls[1];
            var cbBaud = (ComboBox)panel0.Controls[3];
            var btnOpen = (Button)panel0.Controls[4];
            var btnClose = (Button)panel0.Controls[5];
            var cbDataBits = (ComboBox)panel1.Controls[1];
            var cbParity = (ComboBox)panel1.Controls[3];
            var cbStopBits = (ComboBox)panel1.Controls[5];
            var cbHandshake = (ComboBox)panel1.Controls[7];
            var timer = new Timer { Interval = (int)nudInterval.Value };
            int recvCount = 0, sendCount = 0;

            // 打开串口
            btnOpen.Click += (s, e) =>
            {
                if (_portMap.ContainsKey(tab)) return;
                var sp = new SerialPort(
                    cbPort.Text,
                    int.Parse(cbBaud.Text),
                    (Parity)Enum.Parse(typeof(Parity), cbParity.Text),
                    int.Parse(cbDataBits.Text),
                    (StopBits)Enum.Parse(typeof(StopBits), cbStopBits.Text))
                {
                    Handshake = (Handshake)Enum.Parse(typeof(Handshake), cbHandshake.Text),
                    Encoding = Encoding.ASCII,
                    ReadTimeout = 500
                };
                sp.DataReceived += (s2, e2) =>
                {
                    try
                    {
                        var buf = new List<byte>();
                        while (sp.BytesToRead > 0)
                            buf.Add((byte)sp.ReadByte());
                        Invoke(new Action(() =>
                        {
                            var text = chkHexRecv.Checked
                                ? BitConverter.ToString(buf.ToArray())
                                : sp.Encoding.GetString(buf.ToArray());
                            if (chkTs.Checked)
                                text = $"[{DateTime.Now:HH:mm:ss.fff}] {text}";
                            rtbRecv.AppendText(text + Environment.NewLine);
                            recvCount += buf.Count;
                            lblStatus.Text = $"已接收: {recvCount} 字节  已发送: {sendCount} 字节";
                            rtbRecv.ScrollToCaret();
                        }));
                    }
                    catch { }
                };
                try
                {
                    sp.Open();
                    _portMap[tab] = sp;
                    btnOpen.Enabled = false;
                    btnClose.Enabled = true;
                    timer.Tick += (ts, te) => btnSend.PerformClick();
                    timer.Interval = (int)nudInterval.Value;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("打开失败: " + ex.Message);
                }
            };

            // 关闭串口
            btnClose.Click += (s, e) =>
            {
                if (!_portMap.ContainsKey(tab)) return;
                timer.Stop();
                var sp = _portMap[tab];
                try { sp.Close(); sp.Dispose(); }
                catch { }
                _portMap.Remove(tab);
                btnOpen.Enabled = true;
                btnClose.Enabled = false;
            };

            // 发送数据
            btnSend.Click += (s, e) =>
            {
                if (!_portMap.ContainsKey(tab)) return;
                var sp = _portMap[tab];
                if(!sp.IsOpen)
                {
                    MessageBox.Show("串口未打开！");
                    return;
                }
                sp.ReadTimeout = 500;  // 读超时时间，单位毫秒
                sp.WriteTimeout = 500; // 写超时时间，单位毫秒
                var txt = tbSend.Text.Trim();
                if (string.IsNullOrEmpty(txt)) return;
                try
                {
                    byte[] data;
                    // 尝试解析为整数
                    if (int.TryParse(txt, out int intValue))
                    {
                        data = BitConverter.GetBytes(intValue);
                        Array.Reverse(data); // 大端序
                    }
                    // 尝试解析为浮点数
                    else if (float.TryParse(txt, out float floatValue))
                    {
                        data = BitConverter.GetBytes(floatValue);
                        Array.Reverse(data); // 大端序
                    }
                    // 默认按字符串发送（按ASCII编码）
                    else
                    {
                        data = System.Text.Encoding.ASCII.GetBytes(txt);
                    }
                    if (chkHexSend.Checked)
                    {
                        var parts = txt.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                        data = new byte[parts.Length];
                        for (int i = 0; i < parts.Length; i++)
                            data[i] = Convert.ToByte(parts[i], 16);
                    }
                    else
                    {
                        data = sp.Encoding.GetBytes(txt);
                    }
                    sp.Write(data, 0, data.Length);
                    sendCount += data.Length;
                    lblStatus.Text = $"已接收: {recvCount} 字节  已发送: {sendCount} 字节";
                }
                catch (Exception ex)
                {
                    timer.Stop(); // 发送失败时停止自动发送
                    chkAuto.Checked = false; // 发送失败时自动取消自动发送
                    MessageBox.Show("发送失败: " + ex.Message);
                }
            };

            // 自动发送开关 & 发送间隔
            chkAuto.CheckedChanged += (s, e) =>
            {
                if (chkAuto.Checked) timer.Start();
                else timer.Stop();
            };
            nudInterval.ValueChanged += (s, e) => timer.Interval = (int)nudInterval.Value;

            // 清空接收/发送
            btnClearRx.Click += (s, e) => { rtbRecv.Clear(); recvCount = 0; lblStatus.Text = $"已接收: {recvCount} 字节  已发送: {sendCount} 字节"; };
            btnClearTx.Click += (s, e) => tbSend.Clear();

            // 导出日志
            btnExport.Click += (s, e) =>
            {
                var dlg = new SaveFileDialog()
                {
                    Filter = "文本文件 (*.txt)|*.txt",
                    FileName = $"SerialLog_Port{_tabCount}_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
                };
                if (dlg.ShowDialog() == DialogResult.OK)
                    File.WriteAllText(dlg.FileName, rtbRecv.Text, Encoding.UTF8);
            };
        }

        private ComboBox CreateComboBox(IEnumerable<string> items, int width, string defaultText = null)
        {
            var cb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = width };
            cb.Items.AddRange(new List<string>(items).ToArray());
            if (!string.IsNullOrEmpty(defaultText) && cb.Items.Contains(defaultText))
                cb.SelectedItem = defaultText;
            else if (cb.Items.Count > 0)
                cb.SelectedIndex = 0;
            return cb;
        }
    }
}