using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using DeviceHealth;

namespace USB_Test_Infrastructure
{
	// Token: 0x0200001F RID: 31
	public class Tests
	{
		// Token: 0x06000074 RID: 116 RVA: 0x00002FB7 File Offset: 0x000011B7
		public Tests()
		{
			this.TestRunning = 0;
			this.TestCancel = new AutoResetEvent(false);
			this.resetGlobalResults();
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00002FDF File Offset: 0x000011DF
		public void resetGlobalResults()
		{
			this.g_iPass = 0;
			this.g_iSkip = 0;
			this.g_iFail = 0;
			this.g_iAbort = 0;
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00003000 File Offset: 0x00001200
		private static bool ValidateDataEquality(byte[] buffer1, int size1, byte[] buffer2, int size2)
		{
			int num = Math.Min(size1, size2);
			for (int i = 0; i < num; i++)
			{
				if (buffer1[i] != buffer2[i])
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x0000302C File Offset: 0x0000122C
		public bool TestCaseMaxMTU(DTSFUsbStream usb, Tests.TestCompleteCallback completionCallback, object obj)
		{
			if (this != null)
			{
				Tests.TestData testData = new Tests.TestData();
				testData.usb = usb;
				testData.completionCallback = completionCallback;
				testData.obj = obj;
				new Thread(new ParameterizedThreadStart(this.TestCaseMaxMTU)).Start(testData);
			}
			return true;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00003070 File Offset: 0x00001270
		private void TestCaseMaxMTU(object usbObj)
		{
			Tests.TestData testData = (Tests.TestData)usbObj;
			DTSFUsbStream usb = testData.usb;
			testData.completionCallback(this.TestCaseMaxMTU(usb), testData.obj);
		}

		// Token: 0x06000079 RID: 121 RVA: 0x000030A4 File Offset: 0x000012A4
		public bool TestCaseMaxPacket(DTSFUsbStream usb, Tests.TestCompleteCallback completionCallback, object obj)
		{
			if (this != null)
			{
				Tests.TestData testData = new Tests.TestData();
				testData.usb = usb;
				testData.completionCallback = completionCallback;
				testData.obj = obj;
				new Thread(new ParameterizedThreadStart(this.TestCaseMaxPacket)).Start(testData);
			}
			return true;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x000030E8 File Offset: 0x000012E8
		private void TestCaseMaxPacket(object usbObj)
		{
			Tests.TestData testData = (Tests.TestData)usbObj;
			DTSFUsbStream usb = testData.usb;
			testData.completionCallback(this.TestCaseMaxPacket(usb), testData.obj);
		}

		// Token: 0x0600007B RID: 123 RVA: 0x0000311C File Offset: 0x0000131C
		public bool TestCaseMaxPacketPerf(DTSFUsbStream usb, Tests.TestCompleteCallback completionCallback, object obj)
		{
			if (this != null)
			{
				Tests.TestData testData = new Tests.TestData();
				testData.usb = usb;
				testData.completionCallback = completionCallback;
				testData.obj = obj;
				new Thread(new ParameterizedThreadStart(this.TestCaseMaxPacketPerf)).Start(testData);
			}
			return true;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00003160 File Offset: 0x00001360
		private void TestCaseMaxPacketPerf(object usbObj)
		{
			Tests.TestData testData = (Tests.TestData)usbObj;
			DTSFUsbStream usb = testData.usb;
			testData.completionCallback(this.TestCaseMaxPacketPerf(usb), testData.obj);
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00003194 File Offset: 0x00001394
		public bool TestCaseMaxTransferSweep(DTSFUsbStream usb, Tests.TestCompleteCallback completionCallback, object obj)
		{
			if (this != null)
			{
				Tests.TestData testData = new Tests.TestData();
				testData.usb = usb;
				testData.completionCallback = completionCallback;
				testData.obj = obj;
				new Thread(new ParameterizedThreadStart(this.TestCaseMaxTransferSweep)).Start(testData);
			}
			return true;
		}

		// Token: 0x0600007E RID: 126 RVA: 0x000031D8 File Offset: 0x000013D8
		private void TestCaseMaxTransferSweep(object usbObj)
		{
			Tests.TestData testData = (Tests.TestData)usbObj;
			DTSFUsbStream usb = testData.usb;
			testData.completionCallback(this.TestCaseMaxTransferSweep(usb), testData.obj);
		}

		// Token: 0x0600007F RID: 127 RVA: 0x0000320C File Offset: 0x0000140C
		public bool TestCase512Sweep(DTSFUsbStream usb, Tests.TestCompleteCallback completionCallback, object obj)
		{
			if (this != null)
			{
				Tests.TestData testData = new Tests.TestData();
				testData.usb = usb;
				testData.completionCallback = completionCallback;
				testData.obj = obj;
				new Thread(new ParameterizedThreadStart(this.TestCase512Sweep)).Start(testData);
			}
			return true;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00003250 File Offset: 0x00001450
		private void TestCase512Sweep(object usbObj)
		{
			Tests.TestData testData = (Tests.TestData)usbObj;
			DTSFUsbStream usb = testData.usb;
			testData.completionCallback(this.TestCase512Sweep(usb), testData.obj);
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00003284 File Offset: 0x00001484
		public bool TestCase1024Sweep(DTSFUsbStream usb, Tests.TestCompleteCallback completionCallback, object obj)
		{
			if (this != null)
			{
				Tests.TestData testData = new Tests.TestData();
				testData.usb = usb;
				testData.completionCallback = completionCallback;
				testData.obj = obj;
				new Thread(new ParameterizedThreadStart(this.TestCase1024Sweep)).Start(testData);
			}
			return true;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x000032C8 File Offset: 0x000014C8
		private void TestCase1024Sweep(object usbObj)
		{
			Tests.TestData testData = (Tests.TestData)usbObj;
			DTSFUsbStream usb = testData.usb;
			testData.completionCallback(this.TestCase1024Sweep(usb), testData.obj);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x000032FC File Offset: 0x000014FC
		public bool TestCaseMaxTransferLongRun(DTSFUsbStream usb, Tests.TestCompleteCallback completionCallback, object obj)
		{
			if (this != null)
			{
				Tests.TestData testData = new Tests.TestData();
				testData.usb = usb;
				testData.completionCallback = completionCallback;
				testData.obj = obj;
				new Thread(new ParameterizedThreadStart(this.TestCaseMaxTransferLongRun)).Start(testData);
			}
			return true;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00003340 File Offset: 0x00001540
		private void TestCaseMaxTransferLongRun(object usbObj)
		{
			Tests.TestData testData = (Tests.TestData)usbObj;
			DTSFUsbStream usb = testData.usb;
			testData.completionCallback(this.TestCaseMaxTransferLongRun(usb), testData.obj);
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00003374 File Offset: 0x00001574
		public bool TestCaseMaxPacket1G(DTSFUsbStream usb, Tests.TestCompleteCallback completionCallback, object obj)
		{
			if (this != null)
			{
				Tests.TestData testData = new Tests.TestData();
				testData.usb = usb;
				testData.completionCallback = completionCallback;
				testData.obj = obj;
				new Thread(new ParameterizedThreadStart(this.TestCaseMaxPacket1G)).Start(testData);
			}
			return true;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000033B8 File Offset: 0x000015B8
		private void TestCaseMaxPacket1G(object usbObj)
		{
			Tests.TestData testData = (Tests.TestData)usbObj;
			DTSFUsbStream usb = testData.usb;
			testData.completionCallback(this.TestCaseMaxPacket1G(usb), testData.obj);
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000033EC File Offset: 0x000015EC
		public bool TestCaseMaxPacket1GOutOnly(DTSFUsbStream usb, Tests.TestCompleteCallback completionCallback, object obj)
		{
			if (this != null)
			{
				Tests.TestData testData = new Tests.TestData();
				testData.usb = usb;
				testData.completionCallback = completionCallback;
				testData.obj = obj;
				new Thread(new ParameterizedThreadStart(this.TestCaseMaxPacket1GOutOnly)).Start(testData);
			}
			return true;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00003430 File Offset: 0x00001630
		private void TestCaseMaxPacket1GOutOnly(object usbObj)
		{
			Tests.TestData testData = (Tests.TestData)usbObj;
			DTSFUsbStream usb = testData.usb;
			testData.completionCallback(this.TestCaseMaxPacket1GOutOnly(usb), testData.obj);
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00003464 File Offset: 0x00001664
		public void PrintLogHeader(int numOfTests)
		{
			Log.Info(new object[]
			{
				"Tux: Generating WTTLog filename (use -t option to specify)"
			});
			Log.Info(new object[]
			{
				"Tux: Attempting Tux's Directory"
			});
			Log.Info(new object[]
			{
				"<TESTGROUP>"
			});
			Log.Info(new object[]
			{
				"*** =================================================================="
			});
			Log.Info(new object[]
			{
				"*** SUITE INFORMATION"
			});
			Log.Info(new object[]
			{
				"***"
			});
			Log.Info(new object[]
			{
				"*** Suite Name:        N/A (built on the fly)"
			});
			Log.Info(new object[]
			{
				"*** Suite Description: N/A"
			});
			Log.Info(new object[]
			{
				"*** Number of Tests:   {0}",
				numOfTests
			});
			Log.Info(new object[]
			{
				"*** =================================================================="
			});
			Log.Info(new object[]
			{
				""
			});
			Log.Info(new object[]
			{
				"*** =================================================================="
			});
			Log.Info(new object[]
			{
				"*** SYSTEM INFORMATION"
			});
			Log.Info(new object[]
			{
				"***"
			});
			Log.Info(new object[]
			{
				"*** Date and Time:          11/02/2011  7:58 PM (Wednesday)"
			});
			Log.Info(new object[]
			{
				"***"
			});
			Log.Info(new object[]
			{
				"*** Computer Name:          \"mobilecore-258\""
			});
			Log.Info(new object[]
			{
				"*** OS Version:             6.02"
			});
			Log.Info(new object[]
			{
				"*** Build Number:           8141"
			});
			Log.Info(new object[]
			{
				"*** Platform ID:            2 \"Windows NT\""
			});
			Log.Info(new object[]
			{
				"*** Version String:         \"\""
			});
			Log.Info(new object[]
			{
				"***"
			});
			Log.Info(new object[]
			{
				"*** Processor Type:         0x00000000 (0) \"Unknown\""
			});
			Log.Info(new object[]
			{
				"*** Processor Architecture: 0x0005     (5) \"ARM\""
			});
			Log.Info(new object[]
			{
				"*** Page Size:              0x00001000 (4,096)"
			});
			Log.Info(new object[]
			{
				"*** Minimum App Address:    0x00010000 (65,536)"
			});
			Log.Info(new object[]
			{
				"*** Maximum App Address:    0xBFFEFFFF (3,221,159,935)"
			});
			Log.Info(new object[]
			{
				"*** Active Processor Mask:  0x00000003"
			});
			Log.Info(new object[]
			{
				"*** Number Of Processors:   2"
			});
			Log.Info(new object[]
			{
				"*** Allocation Granularity: 0x00010000 (65,536)"
			});
			Log.Info(new object[]
			{
				"*** Processor Level:        0x002D     (45)"
			});
			Log.Info(new object[]
			{
				"*** Processor Revision:     0x0002     (2)"
			});
			Log.Info(new object[]
			{
				"*** =================================================================="
			});
			Log.Info(new object[]
			{
				""
			});
			Log.Info(new object[]
			{
				"*** =================================================================="
			});
			Log.Info(new object[]
			{
				"*** MEMORY INFO"
			});
			Log.Info(new object[]
			{
				"***"
			});
			Log.Info(new object[]
			{
				"*** Memory Total:   423,452,672 bytes"
			});
			Log.Info(new object[]
			{
				"*** Memory Used:    266,821,632 bytes"
			});
			Log.Info(new object[]
			{
				"*** Memory Free:    156,631,040 bytes"
			});
			Log.Info(new object[]
			{
				"*** =================================================================="
			});
			Log.Info(new object[]
			{
				""
			});
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00003798 File Offset: 0x00001998
		public void PrintLogFooter()
		{
			this.PrintLogFooter(this.g_iPass, this.g_iFail, this.g_iSkip, this.g_iAbort);
		}

		// Token: 0x0600008B RID: 139 RVA: 0x000037B8 File Offset: 0x000019B8
		public void PrintLogFooter(int iPass, int iFail, int iSkip, int iAbort)
		{
			Log.Info(new object[]
			{
				"*** =================================================================="
			});
			Log.Info(new object[]
			{
				"*** MEMORY INFO"
			});
			Log.Info(new object[]
			{
				"***"
			});
			Log.Info(new object[]
			{
				"*** Memory Total:   423,452,672 bytes"
			});
			Log.Info(new object[]
			{
				"*** Memory Used:    275,075,072 bytes"
			});
			Log.Info(new object[]
			{
				"*** Memory Free:    148,377,600 bytes"
			});
			Log.Info(new object[]
			{
				"*** =================================================================="
			});
			Log.Info(new object[]
			{
				""
			});
			Log.Info(new object[]
			{
				"*** =================================================================="
			});
			Log.Info(new object[]
			{
				"*** SUITE SUMMARY"
			});
			Log.Info(new object[]
			{
				"***"
			});
			Log.Info(new object[]
			{
				"*** Passed:          {0}",
				iPass
			});
			Log.Info(new object[]
			{
				"*** Failed:          {0}",
				iFail
			});
			Log.Info(new object[]
			{
				"*** Skipped:         {0}",
				iSkip
			});
			Log.Info(new object[]
			{
				"*** Aborted:         {0}",
				iAbort
			});
			Log.Info(new object[]
			{
				"*** -------- ---------"
			});
			Log.Info(new object[]
			{
				"*** Total:           {0}",
				iPass + iFail + iSkip + iAbort
			});
			Log.Info(new object[]
			{
				"***"
			});
			Log.Info(new object[]
			{
				"*** Cumulative Test Execution Time: 0:01:00.000"
			});
			Log.Info(new object[]
			{
				"*** Total Tux Suite Execution Time: 0:01:00.001"
			});
			Log.Info(new object[]
			{
				"*** =================================================================="
			});
			Log.Info(new object[]
			{
				"</TESTGROUP>"
			});
			Log.Info(new object[]
			{
				"@@@@@@{0}",
				iFail + iAbort
			});
		}

		// Token: 0x0600008C RID: 140 RVA: 0x000039BC File Offset: 0x00001BBC
		private void PrintTestHeader(string sTestName, int iTestID)
		{
			Log.Info(new object[]
			{
				"<TESTCASE ID={0}>",
				iTestID
			});
			Log.Info(new object[]
			{
				"*** vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv"
			});
			Log.Info(new object[]
			{
				"*** TEST STARTING"
			});
			Log.Info(new object[]
			{
				"***"
			});
			Log.Info(new object[]
			{
				"*** Test Name:      {0}",
				sTestName
			});
			Log.Info(new object[]
			{
				"*** Test ID:        {0}",
				iTestID
			});
			Log.Info(new object[]
			{
				"*** Library Path:   siostress.dll"
			});
			Log.Info(new object[]
			{
				"*** Command Line:"
			});
			Log.Info(new object[]
			{
				"*** Random Seed:    0"
			});
			Log.Info(new object[]
			{
				"*** Thread Count:   0"
			});
			Log.Info(new object[]
			{
				"*** vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv"
			});
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00003AB0 File Offset: 0x00001CB0
		private void PrintTestFooter(string sTestName, int iTestID, Tests.Results result)
		{
			Log.Info(new object[]
			{
				"*** ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^"
			});
			Log.Info(new object[]
			{
				"*** TEST COMPLETED"
			});
			Log.Info(new object[]
			{
				"***"
			});
			Log.Info(new object[]
			{
				"*** Test Name:      {0}",
				sTestName
			});
			Log.Info(new object[]
			{
				"*** Test ID:        {0}",
				iTestID
			});
			Log.Info(new object[]
			{
				"*** Library Path:   siostress.dll"
			});
			Log.Info(new object[]
			{
				"*** Command Line:"
			});
			switch (result)
			{
			case Tests.Results.Pass:
				Log.Info(new object[]
				{
					"*** Result:         Passed"
				});
				break;
			case Tests.Results.Fail:
				Log.Info(new object[]
				{
					"*** Result:         Failed"
				});
				break;
			case Tests.Results.Skip:
				Log.Info(new object[]
				{
					"*** Result:         Skipped"
				});
				break;
			case Tests.Results.Abort:
				Log.Info(new object[]
				{
					"*** Result:         Aborted"
				});
				break;
			}
			Log.Info(new object[]
			{
				"*** Random Seed:    0"
			});
			Log.Info(new object[]
			{
				"*** Thread Count:   1"
			});
			Log.Info(new object[]
			{
				"*** Execution Time: 0:00:01.000"
			});
			Log.Info(new object[]
			{
				"*** ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^"
			});
			Log.Info(new object[]
			{
				""
			});
			switch (result)
			{
			case Tests.Results.Pass:
				Log.Info(new object[]
				{
					"</TESTCASE RESULT=\"PASSED\">"
				});
				return;
			case Tests.Results.Fail:
				Log.Info(new object[]
				{
					"</TESTCASE RESULT=\"FAILED\">"
				});
				return;
			case Tests.Results.Skip:
				Log.Info(new object[]
				{
					"</TESTCASE RESULT=\"SKIPPED\">"
				});
				return;
			case Tests.Results.Abort:
				Log.Info(new object[]
				{
					"</TESTCASE RESULT=\"ABORTED\">"
				});
				return;
			default:
				return;
			}
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00003C80 File Offset: 0x00001E80
		public bool TestCaseMaxMTU(DTSFUsbStream usb)
		{
			byte[] array = new byte[16376];
			byte[] array2 = new byte[16376];
			int num = 0;
			int num2 = 0;
			long num3 = 0L;
			long num4 = 0L;
			TimeSpan timeSpan = default(TimeSpan);
			TimeSpan timeSpan2 = default(TimeSpan);
			Stopwatch stopwatch = new Stopwatch();
			Stopwatch stopwatch2 = new Stopwatch();
			bool flag = true;
			int num5 = 0;
			this.TestRunning = 1;
			if (this.EnableLogHeaderFooter)
			{
				this.PrintLogHeader(1);
			}
			this.PrintTestHeader("Single transmission max MTU", this.TestRunning);
			int num6 = 16376;
			this.FillBuffer(array2, num);
			try
			{
				stopwatch.Start();
				usb.Write(array2, 0, num6);
				stopwatch.Stop();
				Log.Verbose(new object[]
				{
					"INFO: [{0}]: TX {1} bytes",
					num,
					num6
				});
				if (num6 % 512 == 0)
				{
					stopwatch.Start();
					usb.Write(array2, 0, 0);
					stopwatch.Stop();
					Log.Verbose(new object[]
					{
						"INFO: [{0}]: TX ZLP",
						num,
						num6
					});
				}
				Array.Clear(array, 0, array.Length);
				stopwatch2.Start();
				int num7 = usb.Read(array, 0, array.Length);
				stopwatch2.Stop();
				timeSpan2 = timeSpan2.Add(stopwatch2.Elapsed);
				num4 += (long)num7;
				stopwatch2.Reset();
				num3 += (long)num6;
				timeSpan = timeSpan.Add(stopwatch.Elapsed);
				stopwatch.Reset();
				num++;
				if (num7 == num6 && Tests.ValidateDataEquality(array, num7, array2, num6))
				{
					num2++;
					Log.Info(new object[]
					{
						"*PASS*: [{0}]: Received {1} bytes as expected",
						num,
						num7
					});
				}
				else
				{
					Log.Error(new object[]
					{
						"#FAIL#: [{0}]: Received {1} bytes, expected {2} bytes",
						num,
						num7,
						num6
					});
					Console.WriteLine("#FAIL#: [{0}]: Received {1} bytes, expected {2} bytes", num, num7, num6);
					string @string = Encoding.ASCII.GetString(array2);
					int startIndex = 0;
					int num8 = num6;
					StringBuilder stringBuilder = new StringBuilder(@string, startIndex, num8, num8);
					string string2 = Encoding.ASCII.GetString(array);
					int startIndex2 = 0;
					int num9 = num7;
					StringBuilder stringBuilder2 = new StringBuilder(string2, startIndex2, num9, num9);
					Log.Error(new object[]
					{
						"#FAIL#: [{0}]: TX: {1})",
						num,
						stringBuilder.ToString()
					});
					Log.Error(new object[]
					{
						"#FAIL#: [{0}]: RX: {1})",
						num,
						stringBuilder2.ToString()
					});
					flag = false;
				}
			}
			catch (Exception e)
			{
				this.ExceptionSpew(e);
				flag = false;
				num5 = 1;
				this.g_iAbort++;
			}
			this.PerfSpew(num3, timeSpan.TotalSeconds, num4, timeSpan2.TotalSeconds);
			if (num5 > 0)
			{
				this.PrintTestFooter("Single transmission max MTU", this.TestRunning, Tests.Results.Abort);
			}
			else if (!flag)
			{
				this.PrintTestFooter("Single transmission max MTU", this.TestRunning, Tests.Results.Fail);
				this.g_iFail++;
			}
			else if (num2 > 0 && num - num2 == 0)
			{
				this.PrintTestFooter("Single transmission max MTU", this.TestRunning, Tests.Results.Pass);
				this.g_iPass++;
			}
			else if (num2 == 0 && num == 0)
			{
				this.PrintTestFooter("Single transmission max MTU", this.TestRunning, Tests.Results.Skip);
				this.g_iSkip++;
			}
			else
			{
				this.PrintTestFooter("Single transmission max MTU", this.TestRunning, Tests.Results.Fail);
				this.g_iFail++;
			}
			if (this.EnableLogHeaderFooter)
			{
				this.PrintLogFooter();
			}
			this.TestRunning = 0;
			return flag;
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00004024 File Offset: 0x00002224
		public bool TestCaseMaxPacket(DTSFUsbStream usb)
		{
			byte[] array = new byte[16376];
			byte[] array2 = new byte[16376];
			int num = 0;
			int num2 = 0;
			long num3 = 0L;
			long num4 = 0L;
			TimeSpan timeSpan = default(TimeSpan);
			TimeSpan timeSpan2 = default(TimeSpan);
			Stopwatch stopwatch = new Stopwatch();
			Stopwatch stopwatch2 = new Stopwatch();
			int num5 = 0;
			bool flag = true;
			this.TestRunning = 2;
			if (this.EnableLogHeaderFooter)
			{
				this.PrintLogHeader(1);
			}
			this.PrintTestHeader("Max Packet 512 transmission", this.TestRunning);
			int num6 = 512;
			this.FillBuffer(array2, num);
			try
			{
				stopwatch.Start();
				usb.Write(array2, 0, num6);
				stopwatch.Stop();
				Log.Verbose(new object[]
				{
					"INFO: [{0}]: TX {1} bytes",
					num,
					num6
				});
				if (num6 % 512 == 0)
				{
					stopwatch.Start();
					usb.Write(array2, 0, 0);
					stopwatch.Stop();
					Log.Verbose(new object[]
					{
						"INFO: [{0}]: TX ZLP",
						num,
						num6
					});
				}
				Array.Clear(array, 0, array.Length);
				stopwatch2.Start();
				int num7 = usb.Read(array, 0, array.Length);
				stopwatch2.Stop();
				timeSpan2 = timeSpan2.Add(stopwatch2.Elapsed);
				num4 += (long)num7;
				stopwatch2.Reset();
				num3 += (long)num6;
				timeSpan = timeSpan.Add(stopwatch.Elapsed);
				stopwatch.Reset();
				num++;
				Log.Trace(new object[]
				{
					"INFO: [{0}]: Received {1} bytes, expected {2} bytes",
					num,
					num7,
					num6
				});
				if (num7 == num6 && Tests.ValidateDataEquality(array, num7, array2, num6))
				{
					num2++;
					Log.Trace(new object[]
					{
						"*PASS*: [{0}]: Received {1} bytes as expected",
						num,
						num7
					});
					Console.WriteLine("*PASS*: [{0}]: Received {1} bytes as expected", num, num7);
				}
				else
				{
					Log.Error(new object[]
					{
						"#FAIL#: [{0}]: Received {1} bytes, expected {2} bytes",
						num,
						num7,
						num6
					});
					Console.WriteLine("#FAIL#: [{0}]: Received {1} bytes, expected {2} bytes", num, num7, num6);
					string @string = Encoding.ASCII.GetString(array2);
					int startIndex = 0;
					int num8 = num6;
					StringBuilder stringBuilder = new StringBuilder(@string, startIndex, num8, num8);
					string string2 = Encoding.ASCII.GetString(array);
					int startIndex2 = 0;
					int num9 = num7;
					StringBuilder stringBuilder2 = new StringBuilder(string2, startIndex2, num9, num9);
					Log.Error(new object[]
					{
						"#FAIL#: [{0}]: TX: {1})",
						num,
						stringBuilder.ToString()
					});
					Log.Error(new object[]
					{
						"#FAIL#: [{0}]: RX: {1})",
						num,
						stringBuilder2.ToString()
					});
					flag = false;
				}
			}
			catch (Exception e)
			{
				this.ExceptionSpew(e);
				flag = false;
				num5 = 1;
				this.g_iAbort++;
			}
			Log.Info(new object[]
			{
				"*Pass*: {0}, Fail: {1}, Rate: {2}%",
				num2,
				num - num2,
				(double)num2 / (double)num * 100.0
			});
			this.PerfSpew(num3, timeSpan.TotalSeconds, num4, timeSpan2.TotalSeconds);
			if (num5 > 0)
			{
				this.PrintTestFooter("Max Packet 512 transmission", this.TestRunning, Tests.Results.Abort);
			}
			else if (!flag)
			{
				this.PrintTestFooter("Max Packet 512 transmission", this.TestRunning, Tests.Results.Fail);
				this.g_iFail++;
			}
			else if (num2 > 0 && num - num2 == 0)
			{
				this.PrintTestFooter("Max Packet 512 transmission", this.TestRunning, Tests.Results.Pass);
				this.g_iPass++;
			}
			else if (num2 == 0 && num == 0)
			{
				this.PrintTestFooter("Max Packet 512 transmission", this.TestRunning, Tests.Results.Skip);
				this.g_iSkip++;
			}
			else
			{
				this.PrintTestFooter("Max Packet 512 transmission", this.TestRunning, Tests.Results.Fail);
				this.g_iFail++;
			}
			if (this.EnableLogHeaderFooter)
			{
				this.PrintLogFooter();
			}
			this.TestRunning = 0;
			return flag;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x0000444C File Offset: 0x0000264C
		public bool TestCaseMaxPacketPerf(DTSFUsbStream usb)
		{
			byte[] array = new byte[16376];
			byte[] array2 = new byte[16376];
			int num = 0;
			int num2 = 0;
			long num3 = 0L;
			long num4 = 0L;
			TimeSpan timeSpan = default(TimeSpan);
			TimeSpan timeSpan2 = default(TimeSpan);
			Stopwatch stopwatch = new Stopwatch();
			Stopwatch stopwatch2 = new Stopwatch();
			int num5 = 0;
			bool flag = true;
			this.TestRunning = 3;
			if (this.EnableLogHeaderFooter)
			{
				this.PrintLogHeader(1);
			}
			this.PrintTestHeader("Max Packet perf", this.TestRunning);
			this.FillBuffer(array2, num);
			while (1000 > num)
			{
				int num6 = 16376;
				try
				{
					stopwatch.Start();
					usb.Write(array2, 0, num6);
					stopwatch.Stop();
					if (num6 % 512 == 0)
					{
						stopwatch.Start();
						usb.Write(array2, 0, 0);
						stopwatch.Stop();
					}
					Array.Clear(array, 0, array.Length);
					stopwatch2.Start();
					int num7 = usb.Read(array, 0, array.Length);
					stopwatch2.Stop();
					timeSpan2 = timeSpan2.Add(stopwatch2.Elapsed);
					num4 += (long)num7;
					stopwatch2.Reset();
					num3 += (long)num6;
					timeSpan = timeSpan.Add(stopwatch.Elapsed);
					stopwatch.Reset();
					num++;
					if (num7 == num6 && Tests.ValidateDataEquality(array, num7, array2, num6))
					{
						num2++;
					}
					else
					{
						Log.Error(new object[]
						{
							"#FAIL#: [{0}]: Received {1} bytes, expected {2} bytes",
							num,
							num7,
							num6
						});
						Console.WriteLine("#FAIL#: [{0}]: Received {1} bytes, expected {2} bytes", num, num7, num6);
						string @string = Encoding.ASCII.GetString(array2);
						int startIndex = 0;
						int num8 = num6;
						StringBuilder stringBuilder = new StringBuilder(@string, startIndex, num8, num8);
						string string2 = Encoding.ASCII.GetString(array);
						int startIndex2 = 0;
						int num9 = num7;
						StringBuilder stringBuilder2 = new StringBuilder(string2, startIndex2, num9, num9);
						Log.Error(new object[]
						{
							"#FAIL#: [{0}]: TX: {1})",
							num,
							stringBuilder.ToString()
						});
						Log.Error(new object[]
						{
							"#FAIL#: [{0}]: RX: {1})",
							num,
							stringBuilder2.ToString()
						});
						flag = false;
					}
				}
				catch (Exception e)
				{
					this.ExceptionSpew(e);
					flag = false;
					num5 = 1;
					this.g_iAbort++;
					break;
				}
				if (this.TestCancel.WaitOne(0))
				{
					flag = false;
					num5 = 1;
					this.g_iAbort++;
					break;
				}
			}
			Log.Info(new object[]
			{
				"*Pass*: {0}, Fail: {1}, Rate: {2}%",
				num2,
				num - num2,
				(double)num2 / (double)num * 100.0
			});
			this.PerfSpew(num3, timeSpan.TotalSeconds, num4, timeSpan2.TotalSeconds);
			double num10 = (double)num3 / timeSpan.TotalSeconds / 1000.0;
			double num11 = (double)num4 / timeSpan2.TotalSeconds / 1000.0;
			if (3000.0 > num10 || 3000.0 > num11)
			{
				Log.Error(new object[]
				{
					"#FAIL#: Total throughput too low. Transfer rates must be OVER 3000.0 KBps."
				});
				flag = false;
			}
			if (num5 > 0)
			{
				this.PrintTestFooter("Max Packet perf", this.TestRunning, Tests.Results.Abort);
			}
			else if (!flag)
			{
				this.PrintTestFooter("Max Packet perf", this.TestRunning, Tests.Results.Fail);
				this.g_iFail++;
			}
			else if (num2 > 0 && num - num2 == 0)
			{
				this.PrintTestFooter("Max Packet perf", this.TestRunning, Tests.Results.Pass);
				this.g_iPass++;
			}
			else if (num2 == 0 && num == 0)
			{
				this.PrintTestFooter("Max Packet perf", this.TestRunning, Tests.Results.Skip);
				this.g_iSkip++;
			}
			else
			{
				this.PrintTestFooter("Max Packet perf", this.TestRunning, Tests.Results.Fail);
				this.g_iFail++;
			}
			if (this.EnableLogHeaderFooter)
			{
				this.PrintLogFooter();
			}
			this.TestRunning = 0;
			return flag;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00004874 File Offset: 0x00002A74
		public bool TestCase512Sweep(DTSFUsbStream usb)
		{
			byte[] array = new byte[16376];
			byte[] array2 = new byte[16376];
			int num = 0;
			int num2 = 0;
			long num3 = 0L;
			long num4 = 0L;
			TimeSpan timeSpan = default(TimeSpan);
			TimeSpan timeSpan2 = default(TimeSpan);
			Stopwatch stopwatch = new Stopwatch();
			Stopwatch stopwatch2 = new Stopwatch();
			int num5 = 0;
			bool flag = true;
			this.TestRunning = 4;
			if (this.EnableLogHeaderFooter)
			{
				this.PrintLogHeader(1);
			}
			this.PrintTestHeader("1-512 sweep transmission", this.TestRunning);
			while (512 > num)
			{
				int num6 = num + 1;
				this.FillBuffer(array2, num);
				try
				{
					stopwatch.Start();
					usb.Write(array2, 0, num6);
					stopwatch.Stop();
					Log.Verbose(new object[]
					{
						"INFO: [{0}]: TX {1} bytes",
						num,
						num6
					});
					if (num6 % 512 == 0)
					{
						stopwatch.Start();
						usb.Write(array2, 0, 0);
						stopwatch.Stop();
						Log.Verbose(new object[]
						{
							"INFO: [{0}]: TX ZLP",
							num,
							num6
						});
					}
					Array.Clear(array, 0, array.Length);
					stopwatch2.Start();
					int num7 = usb.Read(array, 0, array.Length);
					stopwatch2.Stop();
					timeSpan2 = timeSpan2.Add(stopwatch2.Elapsed);
					num4 += (long)num7;
					stopwatch2.Reset();
					num3 += (long)num6;
					timeSpan = timeSpan.Add(stopwatch.Elapsed);
					stopwatch.Reset();
					num++;
					Log.Trace(new object[]
					{
						"INFO: [{0}]: Received {1} bytes, expected {2} bytes",
						num,
						num7,
						num6
					});
					if (num7 == num6 && Tests.ValidateDataEquality(array, num7, array2, num6))
					{
						num2++;
						Log.Trace(new object[]
						{
							"*PASS*: [{0}]: Received {1} bytes as expected",
							num,
							num7
						});
						Console.WriteLine("*PASS*: [{0}]: Received {1} bytes as expected", num, num7);
					}
					else
					{
						Log.Error(new object[]
						{
							"#FAIL#: [{0}]: Received {1} bytes, expected {2} bytes",
							num,
							num7,
							num6
						});
						Console.WriteLine("#FAIL#: [{0}]: Received {1} bytes, expected {2} bytes", num, num7, num6);
						string @string = Encoding.ASCII.GetString(array2);
						int startIndex = 0;
						int num8 = num6;
						StringBuilder stringBuilder = new StringBuilder(@string, startIndex, num8, num8);
						string string2 = Encoding.ASCII.GetString(array);
						int startIndex2 = 0;
						int num9 = num7;
						StringBuilder stringBuilder2 = new StringBuilder(string2, startIndex2, num9, num9);
						Log.Error(new object[]
						{
							"#FAIL#: [{0}]: TX: {1})",
							num,
							stringBuilder.ToString()
						});
						Log.Error(new object[]
						{
							"#FAIL#: [{0}]: RX: {1})",
							num,
							stringBuilder2.ToString()
						});
						flag = false;
					}
				}
				catch (Exception e)
				{
					this.ExceptionSpew(e);
					flag = false;
					num5 = 1;
					this.g_iAbort++;
					break;
				}
				if (this.TestCancel.WaitOne(0))
				{
					flag = false;
					num5 = 1;
					this.g_iAbort++;
					break;
				}
			}
			Log.Info(new object[]
			{
				"*Pass*: {0}, Fail: {1}, Rate: {2}%",
				num2,
				num - num2,
				(double)num2 / (double)num * 100.0
			});
			this.PerfSpew(num3, timeSpan.TotalSeconds, num4, timeSpan2.TotalSeconds);
			if (num5 > 0)
			{
				this.PrintTestFooter("1-512 sweep transmission", this.TestRunning, Tests.Results.Abort);
			}
			else if (!flag)
			{
				this.PrintTestFooter("1-512 sweep transmission", this.TestRunning, Tests.Results.Fail);
				this.g_iFail++;
			}
			else if (num2 > 0 && num - num2 == 0)
			{
				this.PrintTestFooter("1-512 sweep transmission", this.TestRunning, Tests.Results.Pass);
				this.g_iPass++;
			}
			else if (num2 == 0 && num == 0)
			{
				this.PrintTestFooter("1-512 sweep transmission", this.TestRunning, Tests.Results.Skip);
				this.g_iSkip++;
			}
			else
			{
				this.PrintTestFooter("1-512 sweep transmission", this.TestRunning, Tests.Results.Fail);
				this.g_iFail++;
			}
			if (this.EnableLogHeaderFooter)
			{
				this.PrintLogFooter();
			}
			this.TestRunning = 0;
			return flag;
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00004CD0 File Offset: 0x00002ED0
		public bool TestCase1024Sweep(DTSFUsbStream usb)
		{
			byte[] array = new byte[16376];
			byte[] array2 = new byte[16376];
			int num = 0;
			int num2 = 0;
			long num3 = 0L;
			long num4 = 0L;
			TimeSpan timeSpan = default(TimeSpan);
			TimeSpan timeSpan2 = default(TimeSpan);
			Stopwatch stopwatch = new Stopwatch();
			Stopwatch stopwatch2 = new Stopwatch();
			int num5 = 0;
			bool flag = true;
			this.TestRunning = 5;
			if (this.EnableLogHeaderFooter)
			{
				this.PrintLogHeader(1);
			}
			this.PrintTestHeader("513-1024 sweep transmission", this.TestRunning);
			while (512 > num)
			{
				int num6 = 512 + num + 1;
				this.FillBuffer(array2, num);
				try
				{
					stopwatch.Start();
					usb.Write(array2, 0, num6);
					stopwatch.Stop();
					Log.Verbose(new object[]
					{
						"INFO: [{0}]: TX {1} bytes",
						num,
						num6
					});
					if (num6 % 512 == 0)
					{
						stopwatch.Start();
						usb.Write(array2, 0, 0);
						stopwatch.Stop();
						Log.Verbose(new object[]
						{
							"INFO: [{0}]: TX ZLP",
							num,
							num6
						});
					}
					Array.Clear(array, 0, array.Length);
					stopwatch2.Start();
					int num7 = usb.Read(array, 0, array.Length);
					stopwatch2.Stop();
					timeSpan2 = timeSpan2.Add(stopwatch2.Elapsed);
					num4 += (long)num7;
					stopwatch2.Reset();
					num3 += (long)num6;
					timeSpan = timeSpan.Add(stopwatch.Elapsed);
					stopwatch.Reset();
					num++;
					Log.Trace(new object[]
					{
						"INFO: [{0}]: Received {1} bytes, expected {2} bytes",
						num,
						num7,
						num6
					});
					if (num7 == num6 && Tests.ValidateDataEquality(array, num7, array2, num6))
					{
						num2++;
						Log.Info(new object[]
						{
							"*PASS*: [{0}]: Received {1} bytes as expected",
							num,
							num7
						});
					}
					else
					{
						Log.Error(new object[]
						{
							"#FAIL#: [{0}]: Received {1} bytes, expected {2} bytes",
							num,
							num7,
							num6
						});
						Console.WriteLine("#FAIL#: [{0}]: Received {1} bytes, expected {2} bytes", num, num7, num6);
						string @string = Encoding.ASCII.GetString(array2);
						int startIndex = 0;
						int num8 = num6;
						StringBuilder stringBuilder = new StringBuilder(@string, startIndex, num8, num8);
						string string2 = Encoding.ASCII.GetString(array);
						int startIndex2 = 0;
						int num9 = num7;
						StringBuilder stringBuilder2 = new StringBuilder(string2, startIndex2, num9, num9);
						Log.Error(new object[]
						{
							"#FAIL#: [{0}]: TX: {1})",
							num,
							stringBuilder.ToString()
						});
						Log.Error(new object[]
						{
							"#FAIL#: [{0}]: RX: {1})",
							num,
							stringBuilder2.ToString()
						});
						flag = false;
					}
				}
				catch (Exception e)
				{
					this.ExceptionSpew(e);
					flag = false;
					num5 = 1;
					this.g_iAbort++;
					break;
				}
				if (this.TestCancel.WaitOne(0))
				{
					flag = false;
					num5 = 1;
					this.g_iAbort++;
					break;
				}
			}
			Log.Info(new object[]
			{
				"*Pass*: {0}, Fail: {1}, Rate: {2}%",
				num2,
				num - num2,
				(double)num2 / (double)num * 100.0
			});
			this.PerfSpew(num3, timeSpan.TotalSeconds, num4, timeSpan2.TotalSeconds);
			if (num5 > 0)
			{
				this.PrintTestFooter("513-1024 sweep transmission", this.TestRunning, Tests.Results.Abort);
			}
			else if (!flag)
			{
				this.PrintTestFooter("513-1024 sweep transmission", this.TestRunning, Tests.Results.Fail);
				this.g_iFail++;
			}
			else if (num2 > 0 && num - num2 == 0)
			{
				this.PrintTestFooter("513-1024 sweep transmission", this.TestRunning, Tests.Results.Pass);
				this.g_iPass++;
			}
			else if (num2 == 0 && num == 0)
			{
				this.PrintTestFooter("513-1024 sweep transmission", this.TestRunning, Tests.Results.Skip);
				this.g_iSkip++;
			}
			else
			{
				this.PrintTestFooter("513-1024 sweep transmission", this.TestRunning, Tests.Results.Fail);
				this.g_iFail++;
			}
			if (this.EnableLogHeaderFooter)
			{
				this.PrintLogFooter();
			}
			this.TestRunning = 0;
			return flag;
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00005118 File Offset: 0x00003318
		public bool TestCaseMaxTransferSweep(DTSFUsbStream usb)
		{
			byte[] array = new byte[16376];
			byte[] array2 = new byte[16376];
			int num = 0;
			int num2 = 0;
			long num3 = 0L;
			long num4 = 0L;
			TimeSpan timeSpan = default(TimeSpan);
			TimeSpan timeSpan2 = default(TimeSpan);
			Stopwatch stopwatch = new Stopwatch();
			Stopwatch stopwatch2 = new Stopwatch();
			int num5 = 0;
			bool flag = true;
			this.TestRunning = 9;
			if (this.EnableLogHeaderFooter)
			{
				this.PrintLogHeader(1);
			}
			this.PrintTestHeader("1-MTU sweep transmission", this.TestRunning);
			while (16376 > num)
			{
				int num6 = num + 1;
				this.FillBuffer(array2, num);
				try
				{
					stopwatch.Start();
					usb.Write(array2, 0, num6);
					stopwatch.Stop();
					Log.Verbose(new object[]
					{
						"INFO: [{0}]: TX {1} bytes",
						num,
						num6
					});
					if (num6 % 512 == 0)
					{
						stopwatch.Start();
						usb.Write(array2, 0, 0);
						stopwatch.Stop();
						Log.Verbose(new object[]
						{
							"INFO: [{0}]: TX ZLP",
							num,
							num6
						});
					}
					Array.Clear(array, 0, array.Length);
					stopwatch2.Start();
					int num7 = usb.Read(array, 0, array.Length);
					stopwatch2.Stop();
					timeSpan2 = timeSpan2.Add(stopwatch2.Elapsed);
					num4 += (long)num7;
					stopwatch2.Reset();
					num3 += (long)num6;
					timeSpan = timeSpan.Add(stopwatch.Elapsed);
					stopwatch.Reset();
					num++;
					Log.Trace(new object[]
					{
						"INFO: [{0}]: Received {1} bytes, expected {2} bytes",
						num,
						num7,
						num6
					});
					if (num7 == num6 && Tests.ValidateDataEquality(array, num7, array2, num6))
					{
						num2++;
						Log.Info(new object[]
						{
							"*PASS*: [{0}]: Received {1} bytes as expected.  RX:{2} == TX:{3}",
							num,
							num7,
							array[0],
							array2[0]
						});
					}
					else
					{
						Log.Error(new object[]
						{
							"#FAIL#: [{0}]: Received {1} bytes, expected {2} bytes",
							num,
							num7,
							num6
						});
						Console.WriteLine("#FAIL#: [{0}]: Received {1} bytes, expected {2} bytes", num, num7, num6);
						string @string = Encoding.ASCII.GetString(array2);
						int startIndex = 0;
						int num8 = num6;
						StringBuilder stringBuilder = new StringBuilder(@string, startIndex, num8, num8);
						string string2 = Encoding.ASCII.GetString(array);
						int startIndex2 = 0;
						int num9 = num7;
						StringBuilder stringBuilder2 = new StringBuilder(string2, startIndex2, num9, num9);
						Log.Error(new object[]
						{
							"#FAIL#: [{0}]: TX: {1})",
							num,
							stringBuilder.ToString()
						});
						Log.Error(new object[]
						{
							"#FAIL#: [{0}]: RX: {1})",
							num,
							stringBuilder2.ToString()
						});
						flag = false;
					}
				}
				catch (Exception e)
				{
					this.ExceptionSpew(e);
					flag = false;
					num5 = 1;
					this.g_iAbort++;
					break;
				}
				if (this.TestCancel.WaitOne(0))
				{
					flag = false;
					num5 = 1;
					this.g_iAbort++;
					break;
				}
			}
			Log.Info(new object[]
			{
				"*Pass*: {0}, Fail: {1}, Rate: {2}%",
				num2,
				num - num2,
				(double)num2 / (double)num * 100.0
			});
			this.PerfSpew(num3, timeSpan.TotalSeconds, num4, timeSpan2.TotalSeconds);
			if (num5 > 0)
			{
				this.PrintTestFooter("1-MTU sweep transmission", this.TestRunning, Tests.Results.Abort);
			}
			else if (!flag)
			{
				this.PrintTestFooter("1-MTU sweep transmission", this.TestRunning, Tests.Results.Fail);
				this.g_iFail++;
			}
			else if (num2 > 0 && num - num2 == 0)
			{
				this.PrintTestFooter("1-MTU sweep transmission", this.TestRunning, Tests.Results.Pass);
				this.g_iPass++;
			}
			else if (num2 == 0 && num == 0)
			{
				this.PrintTestFooter("1-MTU sweep transmission", this.TestRunning, Tests.Results.Skip);
				this.g_iSkip++;
			}
			else
			{
				this.PrintTestFooter("1-MTU sweep transmission", this.TestRunning, Tests.Results.Fail);
				this.g_iFail++;
			}
			if (this.EnableLogHeaderFooter)
			{
				this.PrintLogFooter();
			}
			this.TestRunning = 0;
			return flag;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00005574 File Offset: 0x00003774
		public bool TestCaseMaxTransferLongRun(DTSFUsbStream usb)
		{
			byte[] array = new byte[16376];
			byte[] array2 = new byte[16376];
			int num = 0;
			int num2 = 0;
			long num3 = 0L;
			long num4 = 0L;
			TimeSpan timeSpan = default(TimeSpan);
			TimeSpan timeSpan2 = default(TimeSpan);
			Stopwatch stopwatch = new Stopwatch();
			Stopwatch stopwatch2 = new Stopwatch();
			int num5 = 0;
			bool flag = true;
			this.TestRunning = 6;
			if (this.EnableLogHeaderFooter)
			{
				this.PrintLogHeader(1);
			}
			this.PrintTestHeader("Max Transfer Long Run", this.TestRunning);
			this.FillBuffer(array2, num);
			do
			{
				int num6 = 16376;
				try
				{
					stopwatch.Start();
					usb.Write(array2, 0, num6);
					stopwatch.Stop();
					if (num6 % 512 == 0)
					{
						stopwatch.Start();
						usb.Write(array2, 0, 0);
						stopwatch.Stop();
					}
					Array.Clear(array, 0, array.Length);
					stopwatch2.Start();
					int num7 = usb.Read(array, 0, array.Length);
					stopwatch2.Stop();
					timeSpan2 = timeSpan2.Add(stopwatch2.Elapsed);
					num4 += (long)num7;
					num3 += (long)num6;
					timeSpan = timeSpan.Add(stopwatch.Elapsed);
					num++;
					if (num7 == num6 && Tests.ValidateDataEquality(array, num7, array2, num6))
					{
						num2++;
						Console.WriteLine("\f");
						Log.Info(new object[]
						{
							"*PASS*: [{0}]: Received {1} bytes as expected",
							num,
							num7
						});
					}
					else
					{
						Log.Error(new object[]
						{
							"#FAIL#: [{0}]: Received {1} bytes, expected {2} bytes",
							num,
							num7,
							num6
						});
						Console.WriteLine("#FAIL#: [{0}]: Received {1} bytes, expected {2} bytes", num, num7, num6);
						string @string = Encoding.ASCII.GetString(array2);
						int startIndex = 0;
						int num8 = num6;
						StringBuilder stringBuilder = new StringBuilder(@string, startIndex, num8, num8);
						string string2 = Encoding.ASCII.GetString(array);
						int startIndex2 = 0;
						int num9 = num7;
						StringBuilder stringBuilder2 = new StringBuilder(string2, startIndex2, num9, num9);
						Log.Error(new object[]
						{
							"#FAIL#: [{0}]: TX: {1})",
							num,
							stringBuilder.ToString()
						});
						Log.Error(new object[]
						{
							"#FAIL#: [{0}]: RX: {1})",
							num,
							stringBuilder2.ToString()
						});
						flag = false;
					}
					this.PerfSpew((long)num6, stopwatch.Elapsed.TotalSeconds, (long)num7, stopwatch2.Elapsed.TotalSeconds);
					stopwatch2.Reset();
					stopwatch.Reset();
				}
				catch (Exception e)
				{
					this.ExceptionSpew(e);
					flag = false;
					num5 = 1;
					this.g_iAbort++;
					goto IL_2A4;
				}
			}
			while (!this.TestCancel.WaitOne(0));
			flag = true;
			num5 = 1;
			this.g_iAbort++;
			IL_2A4:
			Log.Info(new object[]
			{
				"*Pass*: {0}, Fail: {1}, Rate: {2}%",
				num2,
				num - num2,
				(double)num2 / (double)num * 100.0
			});
			this.PerfSpew(num3, timeSpan.TotalSeconds, num4, timeSpan2.TotalSeconds);
			if (num5 > 0)
			{
				this.PrintTestFooter("Max Transfer Long Run", this.TestRunning, Tests.Results.Abort);
			}
			else if (!flag)
			{
				this.PrintTestFooter("Max Transfer Long Run", this.TestRunning, Tests.Results.Fail);
				this.g_iFail++;
			}
			else if (num2 > 0 && num - num2 == 0)
			{
				this.PrintTestFooter("Max Transfer Long Run", this.TestRunning, Tests.Results.Pass);
				this.g_iPass++;
			}
			else if (num2 == 0 && num == 0)
			{
				this.PrintTestFooter("Max Transfer Long Run", this.TestRunning, Tests.Results.Skip);
				this.g_iSkip++;
			}
			else
			{
				this.PrintTestFooter("Max Transfer Long Run", this.TestRunning, Tests.Results.Fail);
				this.g_iFail++;
			}
			if (this.EnableLogHeaderFooter)
			{
				this.PrintLogFooter();
			}
			this.TestRunning = 0;
			return flag;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00005964 File Offset: 0x00003B64
		public bool TestCaseMaxPacket1GOutOnly(DTSFUsbStream usb)
		{
			new byte[16376];
			byte[] buffer = new byte[16376];
			int num = 0;
			int num2 = 0;
			long num3 = 0L;
			long num4 = 0L;
			TimeSpan timeSpan = default(TimeSpan);
			TimeSpan timeSpan2 = default(TimeSpan);
			Stopwatch stopwatch = new Stopwatch();
			new Stopwatch();
			int num5 = 0;
			bool flag = true;
			this.TestRunning = 7;
			if (this.EnableLogHeaderFooter)
			{
				this.PrintLogHeader(1);
			}
			this.PrintTestHeader("1GB perf", this.TestRunning);
			this.FillBuffer(buffer, num);
			while (65569 > num)
			{
				int num6 = 16376;
				try
				{
					stopwatch.Start();
					usb.Write(buffer, 0, num6);
					stopwatch.Stop();
					if (num6 % 512 == 0)
					{
						stopwatch.Start();
						usb.Write(buffer, 0, 0);
						stopwatch.Stop();
					}
					num3 += (long)num6;
					timeSpan = timeSpan.Add(stopwatch.Elapsed);
					stopwatch.Reset();
					num++;
					if (num % 65 == 0)
					{
						Console.Write(".");
					}
					if (num % 6556 == 0)
					{
						Console.WriteLine("");
						this.PerfSpew(num3, timeSpan.TotalSeconds, num4, timeSpan2.TotalSeconds);
					}
				}
				catch (Exception e)
				{
					this.ExceptionSpew(e);
					flag = false;
					num5 = 1;
					this.g_iAbort++;
					break;
				}
				if (this.TestCancel.WaitOne(0))
				{
					flag = false;
					num5 = 1;
					this.g_iAbort++;
					break;
				}
			}
			Log.Info(new object[]
			{
				"*Pass*: {0}, Fail: {1}, Rate: {2}%",
				num2,
				num - num2,
				(double)num2 / (double)num * 100.0
			});
			this.PerfSpew(num3, timeSpan.TotalSeconds, 0L, 0.0);
			double num7 = (double)num3 / timeSpan.TotalSeconds / 1000.0;
			double num8 = (double)num4 / timeSpan2.TotalSeconds / 1000.0;
			if (3000.0 > num7 || 3000.0 > num8)
			{
				Log.Error(new object[]
				{
					"#WARNING#: Low throughput. Transfer rates should be OVER 3000.0 KBps."
				});
			}
			if (num5 > 0)
			{
				this.PrintTestFooter("1GB perf", this.TestRunning, Tests.Results.Abort);
			}
			else if (!flag)
			{
				this.PrintTestFooter("1GB perf", this.TestRunning, Tests.Results.Fail);
				this.g_iFail++;
			}
			else if (num2 > 0 && num - num2 == 0)
			{
				this.PrintTestFooter("1GB perf", this.TestRunning, Tests.Results.Pass);
				this.g_iPass++;
			}
			else if (num2 == 0 && num == 0)
			{
				this.PrintTestFooter("1GB perf", this.TestRunning, Tests.Results.Skip);
				this.g_iSkip++;
			}
			else
			{
				this.PrintTestFooter("1GB perf", this.TestRunning, Tests.Results.Fail);
				this.g_iFail++;
			}
			if (this.EnableLogHeaderFooter)
			{
				this.PrintLogFooter();
			}
			this.TestRunning = 0;
			return flag;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00005C84 File Offset: 0x00003E84
		public bool TestCaseMaxPacket1G(DTSFUsbStream usb)
		{
			byte[] array = new byte[16376];
			byte[] array2 = new byte[16376];
			int num = 0;
			int num2 = 0;
			long num3 = 0L;
			long num4 = 0L;
			TimeSpan timeSpan = default(TimeSpan);
			TimeSpan timeSpan2 = default(TimeSpan);
			Stopwatch stopwatch = new Stopwatch();
			Stopwatch stopwatch2 = new Stopwatch();
			int num5 = 0;
			bool flag = true;
			this.TestRunning = 7;
			if (this.EnableLogHeaderFooter)
			{
				this.PrintLogHeader(1);
			}
			this.PrintTestHeader("1GB perf", this.TestRunning);
			this.FillBuffer(array2, num);
			while (65569 > num)
			{
				int num6 = 16376;
				try
				{
					stopwatch.Start();
					usb.Write(array2, 0, num6);
					stopwatch.Stop();
					if (num6 % 512 == 0)
					{
						stopwatch.Start();
						usb.Write(array2, 0, 0);
						stopwatch.Stop();
					}
					Array.Clear(array, 0, array.Length);
					stopwatch2.Start();
					int num7 = usb.Read(array, 0, array.Length);
					stopwatch2.Stop();
					timeSpan2 = timeSpan2.Add(stopwatch2.Elapsed);
					num4 += (long)num7;
					stopwatch2.Reset();
					num3 += (long)num6;
					timeSpan = timeSpan.Add(stopwatch.Elapsed);
					stopwatch.Reset();
					num++;
					if (num % 65 == 0)
					{
						Console.Write(".");
					}
					if (num % 6556 == 0)
					{
						Console.WriteLine("");
						this.PerfSpew(num3, timeSpan.TotalSeconds, num4, timeSpan2.TotalSeconds);
					}
					if (num7 == num6 && Tests.ValidateDataEquality(array, num7, array2, num6))
					{
						num2++;
					}
					else
					{
						Log.Error(new object[]
						{
							"#FAIL#: [{0}]: Received {1} bytes, expected {2} bytes",
							num,
							num7,
							num6
						});
						Console.WriteLine("#FAIL#: [{0}]: Received {1} bytes, expected {2} bytes", num, num7, num6);
						string @string = Encoding.ASCII.GetString(array2);
						int startIndex = 0;
						int num8 = num6;
						StringBuilder stringBuilder = new StringBuilder(@string, startIndex, num8, num8);
						string string2 = Encoding.ASCII.GetString(array);
						int startIndex2 = 0;
						int num9 = num7;
						StringBuilder stringBuilder2 = new StringBuilder(string2, startIndex2, num9, num9);
						Log.Error(new object[]
						{
							"#FAIL#: [{0}]: TX: {1})",
							num,
							stringBuilder.ToString()
						});
						Log.Error(new object[]
						{
							"#FAIL#: [{0}]: RX: {1})",
							num,
							stringBuilder2.ToString()
						});
						flag = false;
					}
				}
				catch (Exception e)
				{
					this.ExceptionSpew(e);
					flag = false;
					num5 = 1;
					this.g_iAbort++;
					break;
				}
				if (this.TestCancel.WaitOne(0))
				{
					flag = false;
					num5 = 1;
					this.g_iAbort++;
					break;
				}
			}
			Log.Info(new object[]
			{
				"*Pass*: {0}, Fail: {1}, Rate: {2}%",
				num2,
				num - num2,
				(double)num2 / (double)num * 100.0
			});
			this.PerfSpew(num3, timeSpan.TotalSeconds, num4, timeSpan2.TotalSeconds);
			double num10 = (double)num3 / timeSpan.TotalSeconds / 1000.0;
			double num11 = (double)num4 / timeSpan2.TotalSeconds / 1000.0;
			if (3000.0 > num10 || 3000.0 > num11)
			{
				Log.Error(new object[]
				{
					"#WARNING#: Low throughput. Transfer rates should be OVER 3000.0 KBps."
				});
			}
			if (num5 > 0)
			{
				this.PrintTestFooter("1GB perf", this.TestRunning, Tests.Results.Abort);
			}
			else if (!flag)
			{
				this.PrintTestFooter("1GB perf", this.TestRunning, Tests.Results.Fail);
				this.g_iFail++;
			}
			else if (num2 > 0 && num - num2 == 0)
			{
				this.PrintTestFooter("1GB perf", this.TestRunning, Tests.Results.Pass);
				this.g_iPass++;
			}
			else if (num2 == 0 && num == 0)
			{
				this.PrintTestFooter("1GB perf", this.TestRunning, Tests.Results.Skip);
				this.g_iSkip++;
			}
			else
			{
				this.PrintTestFooter("1GB perf", this.TestRunning, Tests.Results.Fail);
				this.g_iFail++;
			}
			if (this.EnableLogHeaderFooter)
			{
				this.PrintLogFooter();
			}
			this.TestRunning = 0;
			return flag;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x000060E8 File Offset: 0x000042E8
		private void PerfSpew(long txSize, double TxTotalSeconds, long rxSize, double RxTotalSeconds)
		{
			double num = (double)txSize / TxTotalSeconds / 1000.0;
			double num2 = (double)rxSize / RxTotalSeconds / 1000.0;
			Log.Info(new object[]
			{
				"==============================================================================="
			});
			Log.Info(new object[]
			{
				"TX: {0} bytes in {1} Seconds ({2} KBps)",
				txSize,
				TxTotalSeconds,
				num
			});
			Log.Info(new object[]
			{
				"RX: {0} bytes in {1} Seconds ({2} KBps)",
				rxSize,
				RxTotalSeconds,
				num2
			});
			Log.Info(new object[]
			{
				"==============================================================================="
			});
		}

		// Token: 0x06000098 RID: 152 RVA: 0x0000619C File Offset: 0x0000439C
		private void FillBuffer(byte[] Buffer, int seed)
		{
			for (int i = 0; i < Buffer.Length; i++)
			{
				Buffer[i] = Convert.ToByte(seed % 127 + 49);
			}
		}

		// Token: 0x06000099 RID: 153 RVA: 0x000061C8 File Offset: 0x000043C8
		private void ExceptionSpew(Exception e)
		{
			Log.Info(new object[]
			{
				"==============================================================================="
			});
			Log.Info(new object[]
			{
				"==============================Exception!======================================="
			});
			Log.Info(new object[]
			{
				"{0}",
				e
			});
			Log.Info(new object[]
			{
				"==============================================================================="
			});
		}

		// Token: 0x0400009D RID: 157
		private static Random random = new Random();

		// Token: 0x0400009E RID: 158
		private const int MTU = 16376;

		// Token: 0x0400009F RID: 159
		private const int USB_FS_MAX_PACKET_SIZE = 64;

		// Token: 0x040000A0 RID: 160
		private const int USB_HS_MAX_PACKET_SIZE = 512;

		// Token: 0x040000A1 RID: 161
		private const int ONEGIG = 1073741824;

		// Token: 0x040000A2 RID: 162
		public AutoResetEvent TestCancel;

		// Token: 0x040000A3 RID: 163
		public int TestRunning;

		// Token: 0x040000A4 RID: 164
		public bool EnableLogHeaderFooter = true;

		// Token: 0x040000A5 RID: 165
		private int g_iPass;

		// Token: 0x040000A6 RID: 166
		private int g_iSkip;

		// Token: 0x040000A7 RID: 167
		private int g_iFail;

		// Token: 0x040000A8 RID: 168
		private int g_iAbort;

		// Token: 0x02000020 RID: 32
		private enum Results
		{
			// Token: 0x040000AA RID: 170
			Pass,
			// Token: 0x040000AB RID: 171
			Fail,
			// Token: 0x040000AC RID: 172
			Skip,
			// Token: 0x040000AD RID: 173
			Abort
		}

		// Token: 0x02000021 RID: 33
		// (Invoke) Token: 0x0600009C RID: 156
		public delegate void TestCompleteCallback(bool result, object obj);

		// Token: 0x02000022 RID: 34
		public class TestData
		{
			// Token: 0x040000AE RID: 174
			public DTSFUsbStream usb;

			// Token: 0x040000AF RID: 175
			public Tests.TestCompleteCallback completionCallback;

			// Token: 0x040000B0 RID: 176
			public object obj;
		}
	}
}
