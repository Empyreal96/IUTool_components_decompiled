using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using DeviceHealth;
using USB_Test_Infrastructure;

namespace UEFIUSBFnDevTester
{
	// Token: 0x02000002 RID: 2
	public partial class Form1 : Form
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		private static void DisconnectStatusDM(Button b)
		{
			b.BackColor = Color.Red;
			b.Text = "No connection";
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002068 File Offset: 0x00000268
		private static void ConnectStatusDM(Button b)
		{
			b.BackColor = Color.Green;
			b.Text = "Connected";
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002080 File Offset: 0x00000280
		public Form1()
		{
			this.InitializeComponent();
			this.inReset = new ManualResetEvent(true);
			this.cm = new UsbConnectionManager(delegate(string connectDeviceId)
			{
				Log.Info(new object[]
				{
					"ConnectDeviceId = {0}",
					connectDeviceId
				});
				try
				{
					this.usb = new DTSFUsbStream(connectDeviceId);
					this.test = new Tests();
					Form1.DS method = new Form1.DS(Form1.ConnectStatusDM);
					this.CurrentDevice = connectDeviceId;
					this.ConnectionStatusButton.Invoke(method, new object[]
					{
						this.ConnectionStatusButton
					});
					this.inReset.Set();
				}
				catch (Exception)
				{
					Log.Info(new object[]
					{
						"Connection Failed: {0}",
						connectDeviceId
					});
					if (this.usb != null)
					{
						this.usb.Dispose();
					}
					if (this.test != null)
					{
						this.test = null;
					}
					Form1.DS method2 = new Form1.DS(Form1.DisconnectStatusDM);
					this.ConnectionStatusButton.Invoke(method2, new object[]
					{
						this.ConnectionStatusButton
					});
				}
			}, delegate(string disconnectDeviceId)
			{
				Log.Info(new object[]
				{
					"DisonnectDeviceId = {0}",
					disconnectDeviceId
				});
				if (this.usb != null)
				{
					this.usb.Dispose();
					this.usb = null;
				}
				this.test = null;
				this.ConnectionStatusButton.BackColor = Color.Red;
				this.ConnectionStatusButton.Text = "No connection";
			});
			this.rtbredirect = new Form1.StringRedir(ref this.richTextBox1);
			Console.SetOut(this.rtbredirect);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020F0 File Offset: 0x000002F0
		public void ButtonCallback(bool result, object obj)
		{
			if (obj == null)
			{
				return;
			}
			if (result)
			{
				((Button)obj).BackColor = Color.Green;
			}
			else
			{
				((Button)obj).BackColor = Color.Red;
			}
			if (this.jobQueue.Count > 0)
			{
				this.jobQueue.Dequeue().PerformClick();
				return;
			}
			if (!this.test.EnableLogHeaderFooter)
			{
				this.test.PrintLogFooter();
				this.test.EnableLogHeaderFooter = true;
				this.btnStartTest.Enabled = true;
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002175 File Offset: 0x00000375
		private void Form1_Load(object sender, EventArgs e)
		{
			this.Text = "UEFI SimpleIO USB test " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
			this.cm.Start();
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000021A8 File Offset: 0x000003A8
		private void MTUPerfButton_Click(object sender, EventArgs e)
		{
			if (this.test.EnableLogHeaderFooter)
			{
				this.test.resetGlobalResults();
			}
			if (this.test != null && this.test.TestRunning == 0)
			{
				this.test.TestCaseMaxPacketPerf(this.usb, new Tests.TestCompleteCallback(this.ButtonCallback), this.MTUPerfButton);
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002208 File Offset: 0x00000408
		private void MaxMTU_Click(object sender, EventArgs e)
		{
			if (this.test.EnableLogHeaderFooter)
			{
				this.test.resetGlobalResults();
			}
			if (this.test != null && this.test.TestRunning == 0)
			{
				this.test.TestCaseMaxMTU(this.usb, new Tests.TestCompleteCallback(this.ButtonCallback), this.MaxMTU);
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002268 File Offset: 0x00000468
		private void Sweep512Button_Click(object sender, EventArgs e)
		{
			if (this.test.EnableLogHeaderFooter)
			{
				this.test.resetGlobalResults();
			}
			if (this.test != null && this.test.TestRunning == 0)
			{
				this.test.TestCase512Sweep(this.usb, new Tests.TestCompleteCallback(this.ButtonCallback), this.Sweep512Button);
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000022C8 File Offset: 0x000004C8
		private void MaxPacketButton_Click(object sender, EventArgs e)
		{
			if (this.test.EnableLogHeaderFooter)
			{
				this.test.resetGlobalResults();
			}
			if (this.test != null && this.test.TestRunning == 0)
			{
				this.test.TestCaseMaxPacket(this.usb, new Tests.TestCompleteCallback(this.ButtonCallback), this.MaxPacketButton);
			}
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002328 File Offset: 0x00000528
		private void ResetButton_Click_Thread()
		{
			if (this.test != null)
			{
				this.test.TestCancel.Set();
			}
			Thread.Sleep(10);
			if (this.usb != null)
			{
				this.usb.Dispose();
				this.usb = null;
			}
			if (this.cm != null)
			{
				this.cm.Stop();
				this.cm.Start();
			}
			this.inReset.Set();
		}

		// Token: 0x0600000B RID: 11 RVA: 0x0000239C File Offset: 0x0000059C
		private void ResetButton_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.inReset.WaitOne(0))
				{
					this.inReset.Reset();
					new Thread(new ThreadStart(this.ResetButton_Click_Thread)).Start();
				}
				this.btnStartTest.Enabled = true;
			}
			catch
			{
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000023FC File Offset: 0x000005FC
		private void MTULongRunButton_Click(object sender, EventArgs e)
		{
			if (this.test.EnableLogHeaderFooter)
			{
				this.test.resetGlobalResults();
			}
			if (this.test != null && this.test.TestRunning == 0)
			{
				this.test.TestCaseMaxTransferLongRun(this.usb, new Tests.TestCompleteCallback(this.ButtonCallback), this.MTULongRunButton);
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x0000245C File Offset: 0x0000065C
		private void Sweep1024Button_Click(object sender, EventArgs e)
		{
			if (this.test.EnableLogHeaderFooter)
			{
				this.test.resetGlobalResults();
			}
			if (this.test != null && this.test.TestRunning == 0)
			{
				this.test.TestCase1024Sweep(this.usb, new Tests.TestCompleteCallback(this.ButtonCallback), this.Sweep1024Button);
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000024BA File Offset: 0x000006BA
		private void CmdCancelButton_Click(object sender, EventArgs e)
		{
			if (this.test != null)
			{
				this.test.TestCancel.Set();
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000024D8 File Offset: 0x000006D8
		private void OneGBTest_Click(object sender, EventArgs e)
		{
			if (this.test.EnableLogHeaderFooter)
			{
				this.test.resetGlobalResults();
			}
			if (this.test != null && this.test.TestRunning == 0)
			{
				this.test.TestCaseMaxPacket1G(this.usb, new Tests.TestCompleteCallback(this.ButtonCallback), this.OneGBTest);
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002538 File Offset: 0x00000738
		private void ReconnectTest_Click_Thread()
		{
			while (!this.test.TestCancel.WaitOne(0))
			{
				this.inReset.WaitOne();
				this.inReset.Reset();
				Log.Info(new object[]
				{
					"DisonnectDeviceId = {0}",
					this.CurrentDevice
				});
				if (this.usb != null)
				{
					this.usb.Dispose();
					this.usb = null;
				}
				if (this.cm != null)
				{
					this.cm.Stop();
					this.cm.Start();
				}
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000025CC File Offset: 0x000007CC
		private void ReconnectTest_Click(object sender, EventArgs e)
		{
			if (this.test.EnableLogHeaderFooter)
			{
				this.test.resetGlobalResults();
			}
			try
			{
				if (this.inReset.WaitOne(0))
				{
					new Thread(new ThreadStart(this.ReconnectTest_Click_Thread)).Start();
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000262C File Offset: 0x0000082C
		private void MaxTransferSweep_Click(object sender, EventArgs e)
		{
			if (this.test.EnableLogHeaderFooter)
			{
				this.test.resetGlobalResults();
			}
			if (this.test != null && this.test.TestRunning == 0)
			{
				this.test.TestCaseMaxTransferSweep(this.usb, new Tests.TestCompleteCallback(this.ButtonCallback), this.MaxTransferSweep);
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x0000268C File Offset: 0x0000088C
		private void OneGBOutOnly_Click(object sender, EventArgs e)
		{
			if (this.test.EnableLogHeaderFooter)
			{
				this.test.resetGlobalResults();
			}
			if (this.test != null && this.test.TestRunning == 0)
			{
				this.test.TestCaseMaxPacket1GOutOnly(this.usb, new Tests.TestCompleteCallback(this.ButtonCallback), this.OneGBOutOnly);
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000026EA File Offset: 0x000008EA
		private void richTextBox1_TextChanged(object sender, EventArgs e)
		{
			this.richTextBox1.SelectionStart = this.richTextBox1.Text.Length;
			this.richTextBox1.ScrollToCaret();
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002712 File Offset: 0x00000912
		private void groupBox1_Enter(object sender, EventArgs e)
		{
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002714 File Offset: 0x00000914
		private void btnStartTest_Click(object sender, EventArgs e)
		{
			if (!this.checkBox1.Checked && !this.checkBox2.Checked && !this.checkBox3.Checked && !this.checkBox4.Checked && !this.checkBox5.Checked && !this.checkBox6.Checked && !this.checkBox7.Checked && !this.checkBox8.Checked && !this.checkBox9.Checked && !this.checkBox10.Checked)
			{
				Log.Info(new object[]
				{
					"No tests were added to this run."
				});
				return;
			}
			this.btnStartTest.Enabled = false;
			this.test.resetGlobalResults();
			if (this.checkBox1.Checked)
			{
				this.jobQueue.Enqueue(this.MaxMTU);
			}
			if (this.checkBox2.Checked)
			{
				this.jobQueue.Enqueue(this.MaxPacketButton);
			}
			if (this.checkBox3.Checked)
			{
				this.jobQueue.Enqueue(this.Sweep512Button);
			}
			if (this.checkBox4.Checked)
			{
				this.jobQueue.Enqueue(this.Sweep1024Button);
			}
			if (this.checkBox5.Checked)
			{
				this.jobQueue.Enqueue(this.MTUPerfButton);
			}
			if (this.checkBox6.Checked)
			{
				this.jobQueue.Enqueue(this.MTULongRunButton);
			}
			if (this.checkBox7.Checked)
			{
				this.jobQueue.Enqueue(this.OneGBTest);
			}
			if (this.checkBox8.Checked)
			{
				this.jobQueue.Enqueue(this.ReconnectTest);
			}
			if (this.checkBox9.Checked)
			{
				this.jobQueue.Enqueue(this.MaxTransferSweep);
			}
			if (this.checkBox10.Checked)
			{
				this.jobQueue.Enqueue(this.OneGBOutOnly);
			}
			this.test.EnableLogHeaderFooter = false;
			this.test.PrintLogHeader(this.jobQueue.Count);
			this.jobQueue.Dequeue().PerformClick();
		}

		// Token: 0x06000017 RID: 23 RVA: 0x0000292F File Offset: 0x00000B2F
		private void btnClearLogFrame_Click(object sender, EventArgs e)
		{
			this.richTextBox1.Text = "";
		}

		// Token: 0x04000001 RID: 1
		public DTSFUsbStream usb;

		// Token: 0x04000002 RID: 2
		public Tests test;

		// Token: 0x04000003 RID: 3
		public UsbConnectionManager cm;

		// Token: 0x04000004 RID: 4
		private string CurrentDevice;

		// Token: 0x04000005 RID: 5
		private ManualResetEvent inReset;

		// Token: 0x04000006 RID: 6
		private Queue<Button> jobQueue = new Queue<Button>();

		// Token: 0x04000007 RID: 7
		private Form1.StringRedir rtbredirect;

		// Token: 0x02000006 RID: 6
		public class StringRedir : StringWriter
		{
			// Token: 0x06000025 RID: 37 RVA: 0x00003C28 File Offset: 0x00001E28
			public StringRedir(ref RichTextBox textBox)
			{
				this.outBox = textBox;
			}

			// Token: 0x06000026 RID: 38 RVA: 0x00003C38 File Offset: 0x00001E38
			public override void WriteLine(string x)
			{
				if (this.outBox.InvokeRequired)
				{
					Form1.StringRedir.SetTextCallback method = new Form1.StringRedir.SetTextCallback(this.WriteLine);
					this.outBox.Invoke(method, new object[]
					{
						x
					});
					return;
				}
				if ("\f" == x)
				{
					this.outBox.Clear();
				}
				RichTextBox richTextBox = this.outBox;
				richTextBox.Text = richTextBox.Text + x + "\n";
				this.outBox.Refresh();
			}

			// Token: 0x06000027 RID: 39 RVA: 0x00003CBC File Offset: 0x00001EBC
			public override void Write(string x)
			{
				if (this.outBox.InvokeRequired)
				{
					Form1.StringRedir.SetTextCallback method = new Form1.StringRedir.SetTextCallback(this.Write);
					this.outBox.Invoke(method, new object[]
					{
						x
					});
					return;
				}
				if ("\f" == x)
				{
					this.outBox.Clear();
				}
				RichTextBox richTextBox = this.outBox;
				richTextBox.Text += x;
				this.outBox.Refresh();
			}

			// Token: 0x0400002B RID: 43
			public RichTextBox outBox;

			// Token: 0x02000008 RID: 8
			// (Invoke) Token: 0x0600002D RID: 45
			private delegate void SetTextCallback(string text);
		}

		// Token: 0x02000007 RID: 7
		// (Invoke) Token: 0x06000029 RID: 41
		private delegate void DS(Button b);
	}
}
