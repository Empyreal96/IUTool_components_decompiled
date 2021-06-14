namespace UEFIUSBFnDevTester
{
	// Token: 0x02000002 RID: 2
	public partial class Form1 : global::System.Windows.Forms.Form
	{
		// Token: 0x06000018 RID: 24 RVA: 0x00002941 File Offset: 0x00000B41
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002960 File Offset: 0x00000B60
		private void InitializeComponent()
		{
			this.MTUPerfButton = new global::System.Windows.Forms.Button();
			this.MaxMTU = new global::System.Windows.Forms.Button();
			this.Sweep512Button = new global::System.Windows.Forms.Button();
			this.MaxPacketButton = new global::System.Windows.Forms.Button();
			this.groupBox1 = new global::System.Windows.Forms.GroupBox();
			this.btnClearLogFrame = new global::System.Windows.Forms.Button();
			this.btnStartTest = new global::System.Windows.Forms.Button();
			this.checkBox10 = new global::System.Windows.Forms.CheckBox();
			this.checkBox9 = new global::System.Windows.Forms.CheckBox();
			this.checkBox8 = new global::System.Windows.Forms.CheckBox();
			this.checkBox7 = new global::System.Windows.Forms.CheckBox();
			this.checkBox6 = new global::System.Windows.Forms.CheckBox();
			this.checkBox5 = new global::System.Windows.Forms.CheckBox();
			this.checkBox4 = new global::System.Windows.Forms.CheckBox();
			this.checkBox3 = new global::System.Windows.Forms.CheckBox();
			this.checkBox2 = new global::System.Windows.Forms.CheckBox();
			this.checkBox1 = new global::System.Windows.Forms.CheckBox();
			this.OneGBOutOnly = new global::System.Windows.Forms.Button();
			this.MaxTransferSweep = new global::System.Windows.Forms.Button();
			this.ReconnectTest = new global::System.Windows.Forms.Button();
			this.OneGBTest = new global::System.Windows.Forms.Button();
			this.CmdCancelButton = new global::System.Windows.Forms.Button();
			this.Sweep1024Button = new global::System.Windows.Forms.Button();
			this.MTULongRunButton = new global::System.Windows.Forms.Button();
			this.ConnectionStatusButton = new global::System.Windows.Forms.Button();
			this.ResetButton = new global::System.Windows.Forms.Button();
			this.richTextBox1 = new global::System.Windows.Forms.RichTextBox();
			this.groupBox1.SuspendLayout();
			base.SuspendLayout();
			this.MTUPerfButton.Location = new global::System.Drawing.Point(19, 153);
			this.MTUPerfButton.Name = "MTUPerfButton";
			this.MTUPerfButton.Size = new global::System.Drawing.Size(112, 23);
			this.MTUPerfButton.TabIndex = 0;
			this.MTUPerfButton.Text = "MTU Perf test";
			this.MTUPerfButton.UseVisualStyleBackColor = true;
			this.MTUPerfButton.Click += new global::System.EventHandler(this.MTUPerfButton_Click);
			this.MaxMTU.Location = new global::System.Drawing.Point(18, 37);
			this.MaxMTU.Name = "MaxMTU";
			this.MaxMTU.Size = new global::System.Drawing.Size(112, 23);
			this.MaxMTU.TabIndex = 1;
			this.MaxMTU.Text = "Single MTU test";
			this.MaxMTU.UseVisualStyleBackColor = true;
			this.MaxMTU.Click += new global::System.EventHandler(this.MaxMTU_Click);
			this.Sweep512Button.Location = new global::System.Drawing.Point(18, 95);
			this.Sweep512Button.Name = "Sweep512Button";
			this.Sweep512Button.Size = new global::System.Drawing.Size(112, 23);
			this.Sweep512Button.TabIndex = 2;
			this.Sweep512Button.Text = "512 byte sweep test";
			this.Sweep512Button.UseVisualStyleBackColor = true;
			this.Sweep512Button.Click += new global::System.EventHandler(this.Sweep512Button_Click);
			this.MaxPacketButton.Location = new global::System.Drawing.Point(18, 66);
			this.MaxPacketButton.Name = "MaxPacketButton";
			this.MaxPacketButton.Size = new global::System.Drawing.Size(112, 23);
			this.MaxPacketButton.TabIndex = 3;
			this.MaxPacketButton.Text = "Max Packet test";
			this.MaxPacketButton.UseVisualStyleBackColor = true;
			this.MaxPacketButton.Click += new global::System.EventHandler(this.MaxPacketButton_Click);
			this.groupBox1.Controls.Add(this.btnClearLogFrame);
			this.groupBox1.Controls.Add(this.btnStartTest);
			this.groupBox1.Controls.Add(this.ConnectionStatusButton);
			this.groupBox1.Controls.Add(this.checkBox10);
			this.groupBox1.Controls.Add(this.checkBox9);
			this.groupBox1.Controls.Add(this.checkBox8);
			this.groupBox1.Controls.Add(this.checkBox7);
			this.groupBox1.Controls.Add(this.checkBox6);
			this.groupBox1.Controls.Add(this.checkBox5);
			this.groupBox1.Controls.Add(this.checkBox4);
			this.groupBox1.Controls.Add(this.checkBox3);
			this.groupBox1.Controls.Add(this.checkBox2);
			this.groupBox1.Controls.Add(this.checkBox1);
			this.groupBox1.Controls.Add(this.OneGBOutOnly);
			this.groupBox1.Controls.Add(this.MaxTransferSweep);
			this.groupBox1.Controls.Add(this.ReconnectTest);
			this.groupBox1.Controls.Add(this.OneGBTest);
			this.groupBox1.Controls.Add(this.CmdCancelButton);
			this.groupBox1.Controls.Add(this.Sweep1024Button);
			this.groupBox1.Controls.Add(this.MTULongRunButton);
			this.groupBox1.Controls.Add(this.ResetButton);
			this.groupBox1.Controls.Add(this.MaxMTU);
			this.groupBox1.Controls.Add(this.Sweep512Button);
			this.groupBox1.Controls.Add(this.MaxPacketButton);
			this.groupBox1.Controls.Add(this.MTUPerfButton);
			this.groupBox1.Location = new global::System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new global::System.Drawing.Size(200, 475);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "UEFI USBFn Loopback tests";
			this.groupBox1.Enter += new global::System.EventHandler(this.groupBox1_Enter);
			this.btnClearLogFrame.Location = new global::System.Drawing.Point(19, 357);
			this.btnClearLogFrame.Name = "btnClearLogFrame";
			this.btnClearLogFrame.Size = new global::System.Drawing.Size(80, 25);
			this.btnClearLogFrame.TabIndex = 24;
			this.btnClearLogFrame.Text = "Clear";
			this.btnClearLogFrame.UseVisualStyleBackColor = true;
			this.btnClearLogFrame.Click += new global::System.EventHandler(this.btnClearLogFrame_Click);
			this.btnStartTest.Location = new global::System.Drawing.Point(19, 328);
			this.btnStartTest.Name = "btnStartTest";
			this.btnStartTest.Size = new global::System.Drawing.Size(80, 25);
			this.btnStartTest.TabIndex = 23;
			this.btnStartTest.Text = "Start Test";
			this.btnStartTest.UseVisualStyleBackColor = true;
			this.btnStartTest.Click += new global::System.EventHandler(this.btnStartTest_Click);
			this.checkBox10.AutoSize = true;
			this.checkBox10.Location = new global::System.Drawing.Point(136, 303);
			this.checkBox10.Name = "checkBox10";
			this.checkBox10.Size = new global::System.Drawing.Size(45, 17);
			this.checkBox10.TabIndex = 22;
			this.checkBox10.Text = "Add";
			this.checkBox10.UseVisualStyleBackColor = true;
			this.checkBox10.Visible = false;
			this.checkBox9.AutoSize = true;
			this.checkBox9.Location = new global::System.Drawing.Point(136, 273);
			this.checkBox9.Name = "checkBox9";
			this.checkBox9.Size = new global::System.Drawing.Size(45, 17);
			this.checkBox9.TabIndex = 21;
			this.checkBox9.Text = "Add";
			this.checkBox9.UseVisualStyleBackColor = true;
			this.checkBox8.AutoSize = true;
			this.checkBox8.Location = new global::System.Drawing.Point(136, 244);
			this.checkBox8.Name = "checkBox8";
			this.checkBox8.Size = new global::System.Drawing.Size(45, 17);
			this.checkBox8.TabIndex = 20;
			this.checkBox8.Text = "Add";
			this.checkBox8.UseVisualStyleBackColor = true;
			this.checkBox7.AutoSize = true;
			this.checkBox7.Location = new global::System.Drawing.Point(136, 215);
			this.checkBox7.Name = "checkBox7";
			this.checkBox7.Size = new global::System.Drawing.Size(45, 17);
			this.checkBox7.TabIndex = 19;
			this.checkBox7.Text = "Add";
			this.checkBox7.UseVisualStyleBackColor = true;
			this.checkBox6.AutoSize = true;
			this.checkBox6.Location = new global::System.Drawing.Point(136, 186);
			this.checkBox6.Name = "checkBox6";
			this.checkBox6.Size = new global::System.Drawing.Size(45, 17);
			this.checkBox6.TabIndex = 18;
			this.checkBox6.Text = "Add";
			this.checkBox6.UseVisualStyleBackColor = true;
			this.checkBox5.AutoSize = true;
			this.checkBox5.Location = new global::System.Drawing.Point(136, 157);
			this.checkBox5.Name = "checkBox5";
			this.checkBox5.Size = new global::System.Drawing.Size(45, 17);
			this.checkBox5.TabIndex = 17;
			this.checkBox5.Text = "Add";
			this.checkBox5.UseVisualStyleBackColor = true;
			this.checkBox4.AutoSize = true;
			this.checkBox4.Location = new global::System.Drawing.Point(136, 128);
			this.checkBox4.Name = "checkBox4";
			this.checkBox4.Size = new global::System.Drawing.Size(45, 17);
			this.checkBox4.TabIndex = 16;
			this.checkBox4.Text = "Add";
			this.checkBox4.UseVisualStyleBackColor = true;
			this.checkBox3.AutoSize = true;
			this.checkBox3.Location = new global::System.Drawing.Point(136, 99);
			this.checkBox3.Name = "checkBox3";
			this.checkBox3.Size = new global::System.Drawing.Size(45, 17);
			this.checkBox3.TabIndex = 15;
			this.checkBox3.Text = "Add";
			this.checkBox3.UseVisualStyleBackColor = true;
			this.checkBox2.AutoSize = true;
			this.checkBox2.Location = new global::System.Drawing.Point(136, 70);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new global::System.Drawing.Size(45, 17);
			this.checkBox2.TabIndex = 14;
			this.checkBox2.Text = "Add";
			this.checkBox2.UseVisualStyleBackColor = true;
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new global::System.Drawing.Point(136, 41);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new global::System.Drawing.Size(45, 17);
			this.checkBox1.TabIndex = 13;
			this.checkBox1.Text = "Add";
			this.checkBox1.UseVisualStyleBackColor = true;
			this.OneGBOutOnly.Location = new global::System.Drawing.Point(19, 299);
			this.OneGBOutOnly.Name = "OneGBOutOnly";
			this.OneGBOutOnly.Size = new global::System.Drawing.Size(110, 23);
			this.OneGBOutOnly.TabIndex = 12;
			this.OneGBOutOnly.Text = "1GB Out Only";
			this.OneGBOutOnly.UseVisualStyleBackColor = true;
			this.OneGBOutOnly.Visible = false;
			this.OneGBOutOnly.Click += new global::System.EventHandler(this.OneGBOutOnly_Click);
			this.MaxTransferSweep.Location = new global::System.Drawing.Point(19, 269);
			this.MaxTransferSweep.Name = "MaxTransferSweep";
			this.MaxTransferSweep.Size = new global::System.Drawing.Size(110, 23);
			this.MaxTransferSweep.TabIndex = 11;
			this.MaxTransferSweep.Text = "MTU Sweep";
			this.MaxTransferSweep.UseVisualStyleBackColor = true;
			this.MaxTransferSweep.Click += new global::System.EventHandler(this.MaxTransferSweep_Click);
			this.ReconnectTest.Location = new global::System.Drawing.Point(18, 240);
			this.ReconnectTest.Name = "ReconnectTest";
			this.ReconnectTest.Size = new global::System.Drawing.Size(111, 23);
			this.ReconnectTest.TabIndex = 10;
			this.ReconnectTest.Text = "Connection test";
			this.ReconnectTest.UseVisualStyleBackColor = true;
			this.ReconnectTest.Click += new global::System.EventHandler(this.ReconnectTest_Click);
			this.OneGBTest.Location = new global::System.Drawing.Point(18, 211);
			this.OneGBTest.Name = "OneGBTest";
			this.OneGBTest.Size = new global::System.Drawing.Size(111, 23);
			this.OneGBTest.TabIndex = 9;
			this.OneGBTest.Text = "1GB Throughput Test";
			this.OneGBTest.UseVisualStyleBackColor = true;
			this.OneGBTest.Click += new global::System.EventHandler(this.OneGBTest_Click);
			this.CmdCancelButton.Location = new global::System.Drawing.Point(101, 328);
			this.CmdCancelButton.Name = "CmdCancelButton";
			this.CmdCancelButton.Size = new global::System.Drawing.Size(80, 25);
			this.CmdCancelButton.TabIndex = 8;
			this.CmdCancelButton.Text = "Stop Test";
			this.CmdCancelButton.UseVisualStyleBackColor = true;
			this.CmdCancelButton.Click += new global::System.EventHandler(this.CmdCancelButton_Click);
			this.Sweep1024Button.Location = new global::System.Drawing.Point(19, 124);
			this.Sweep1024Button.Name = "Sweep1024Button";
			this.Sweep1024Button.Size = new global::System.Drawing.Size(111, 23);
			this.Sweep1024Button.TabIndex = 7;
			this.Sweep1024Button.Text = "1024 byte sweep test";
			this.Sweep1024Button.UseVisualStyleBackColor = true;
			this.Sweep1024Button.Click += new global::System.EventHandler(this.Sweep1024Button_Click);
			this.MTULongRunButton.Location = new global::System.Drawing.Point(18, 182);
			this.MTULongRunButton.Name = "MTULongRunButton";
			this.MTULongRunButton.Size = new global::System.Drawing.Size(111, 23);
			this.MTULongRunButton.TabIndex = 6;
			this.MTULongRunButton.Text = "MTU Long Run";
			this.MTULongRunButton.UseVisualStyleBackColor = true;
			this.MTULongRunButton.Click += new global::System.EventHandler(this.MTULongRunButton_Click);
			this.ConnectionStatusButton.BackColor = global::System.Drawing.Color.Red;
			this.ConnectionStatusButton.Location = new global::System.Drawing.Point(19, 388);
			this.ConnectionStatusButton.Name = "ConnectionStatusButton";
			this.ConnectionStatusButton.Size = new global::System.Drawing.Size(162, 81);
			this.ConnectionStatusButton.TabIndex = 5;
			this.ConnectionStatusButton.Text = "No Connection";
			this.ConnectionStatusButton.UseVisualStyleBackColor = false;
			this.ResetButton.Location = new global::System.Drawing.Point(101, 357);
			this.ResetButton.Name = "ResetButton";
			this.ResetButton.Size = new global::System.Drawing.Size(80, 25);
			this.ResetButton.TabIndex = 4;
			this.ResetButton.Text = "Reset";
			this.ResetButton.UseVisualStyleBackColor = true;
			this.ResetButton.Click += new global::System.EventHandler(this.ResetButton_Click);
			this.richTextBox1.Location = new global::System.Drawing.Point(218, 12);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.ScrollBars = global::System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
			this.richTextBox1.ShowSelectionMargin = true;
			this.richTextBox1.Size = new global::System.Drawing.Size(584, 475);
			this.richTextBox1.TabIndex = 5;
			this.richTextBox1.Text = "";
			this.richTextBox1.TextChanged += new global::System.EventHandler(this.richTextBox1_TextChanged);
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(810, 495);
			base.Controls.Add(this.richTextBox1);
			base.Controls.Add(this.groupBox1);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.Fixed3D;
			base.MaximizeBox = false;
			base.Name = "Form1";
			this.Text = "UEFI SimpleIO USB test";
			base.Load += new global::System.EventHandler(this.Form1_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			base.ResumeLayout(false);
		}

		// Token: 0x04000008 RID: 8
		private global::System.ComponentModel.IContainer components;

		// Token: 0x04000009 RID: 9
		private global::System.Windows.Forms.Button MTUPerfButton;

		// Token: 0x0400000A RID: 10
		private global::System.Windows.Forms.Button MaxMTU;

		// Token: 0x0400000B RID: 11
		private global::System.Windows.Forms.Button Sweep512Button;

		// Token: 0x0400000C RID: 12
		private global::System.Windows.Forms.Button MaxPacketButton;

		// Token: 0x0400000D RID: 13
		private global::System.Windows.Forms.GroupBox groupBox1;

		// Token: 0x0400000E RID: 14
		private global::System.Windows.Forms.RichTextBox richTextBox1;

		// Token: 0x0400000F RID: 15
		private global::System.Windows.Forms.Button ResetButton;

		// Token: 0x04000010 RID: 16
		private global::System.Windows.Forms.Button ConnectionStatusButton;

		// Token: 0x04000011 RID: 17
		private global::System.Windows.Forms.Button Sweep1024Button;

		// Token: 0x04000012 RID: 18
		private global::System.Windows.Forms.Button MTULongRunButton;

		// Token: 0x04000013 RID: 19
		private global::System.Windows.Forms.Button CmdCancelButton;

		// Token: 0x04000014 RID: 20
		private global::System.Windows.Forms.Button OneGBTest;

		// Token: 0x04000015 RID: 21
		private global::System.Windows.Forms.Button ReconnectTest;

		// Token: 0x04000016 RID: 22
		private global::System.Windows.Forms.Button MaxTransferSweep;

		// Token: 0x04000017 RID: 23
		private global::System.Windows.Forms.Button OneGBOutOnly;

		// Token: 0x04000018 RID: 24
		private global::System.Windows.Forms.CheckBox checkBox1;

		// Token: 0x04000019 RID: 25
		private global::System.Windows.Forms.Button btnStartTest;

		// Token: 0x0400001A RID: 26
		private global::System.Windows.Forms.CheckBox checkBox10;

		// Token: 0x0400001B RID: 27
		private global::System.Windows.Forms.CheckBox checkBox9;

		// Token: 0x0400001C RID: 28
		private global::System.Windows.Forms.CheckBox checkBox8;

		// Token: 0x0400001D RID: 29
		private global::System.Windows.Forms.CheckBox checkBox7;

		// Token: 0x0400001E RID: 30
		private global::System.Windows.Forms.CheckBox checkBox6;

		// Token: 0x0400001F RID: 31
		private global::System.Windows.Forms.CheckBox checkBox5;

		// Token: 0x04000020 RID: 32
		private global::System.Windows.Forms.CheckBox checkBox4;

		// Token: 0x04000021 RID: 33
		private global::System.Windows.Forms.CheckBox checkBox3;

		// Token: 0x04000022 RID: 34
		private global::System.Windows.Forms.CheckBox checkBox2;

		// Token: 0x04000023 RID: 35
		private global::System.Windows.Forms.Button btnClearLogFrame;
	}
}
