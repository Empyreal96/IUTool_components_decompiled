using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using FFUComponents;

namespace Microsoft.Windows.ImageTools
{
	// Token: 0x0200000E RID: 14
	public static class FFUTool
	{
		// Token: 0x0600001B RID: 27 RVA: 0x00002B3C File Offset: 0x00000D3C
		public static void Main(string[] args)
		{
			if (args.Length > 2 && FFUTool.forceParam.IsMatch(args[2]))
			{
				Console.WriteLine(Resources.FORCE_OPTION_DEPRECATED);
			}
			bool flag = false;
			string text = null;
			uint bootMode = 0U;
			string profileName = null;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			if (args.Length < 1 || (!FFUTool.flashParam.IsMatch(args[0]) && !FFUTool.uefiFlashParam.IsMatch(args[0]) && !FFUTool.fastFlashParam.IsMatch(args[0]) && !FFUTool.wimParam.IsMatch(args[0]) && !FFUTool.skipParam.IsMatch(args[0]) && !FFUTool.listParam.IsMatch(args[0]) && !FFUTool.massParam.IsMatch(args[0]) && !FFUTool.serParam.IsMatch(args[0]) && !FFUTool.clearIdParam.IsMatch(args[0]) && !FFUTool.setBootModeParam.IsMatch(args[0]) && !FFUTool.servicingLogsParam.IsMatch(args[0]) && !FFUTool.flashLogsParam.IsMatch(args[0])))
			{
				flag = true;
			}
			if (!flag && (FFUTool.flashParam.IsMatch(args[0]) || FFUTool.uefiFlashParam.IsMatch(args[0]) || FFUTool.fastFlashParam.IsMatch(args[0]) || FFUTool.wimParam.IsMatch(args[0])))
			{
				if (args.Length <= 1)
				{
					flag = true;
				}
				else
				{
					text = args[1];
					if (!File.Exists(text))
					{
						Console.WriteLine(Resources.ERROR_FILE_NOT_FOUND, text);
						Environment.ExitCode = -1;
						return;
					}
					if (FFUTool.flashParam.IsMatch(args[0]) && args.Length >= 3)
					{
						Console.WriteLine(Resources.WIM_FLASH_OPTION_DEPRECATED);
					}
				}
			}
			if (!flag && FFUTool.servicingLogsParam.IsMatch(args[0]))
			{
				if (args.Length <= 1)
				{
					flag = true;
				}
				else
				{
					text = args[1];
				}
			}
			if (!flag && FFUTool.flashLogsParam.IsMatch(args[0]))
			{
				if (args.Length <= 1)
				{
					flag = true;
				}
				else
				{
					text = args[1];
				}
			}
			if (!flag && FFUTool.setBootModeParam.IsMatch(args[0]))
			{
				if (args.Length <= 1)
				{
					flag = true;
				}
				else
				{
					try
					{
						bootMode = Convert.ToUInt32(args[1], CultureInfo.InvariantCulture);
						if (args.Length >= 3)
						{
							profileName = args[2];
						}
						else
						{
							profileName = "";
						}
					}
					catch (Exception)
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				Console.WriteLine(Resources.USAGE);
				Environment.ExitCode = -1;
				return;
			}
			if (args.Any((string s) => FFUTool.noLogParam.IsMatch(s)))
			{
				flag2 = true;
				Environment.SetEnvironmentVariable("FFUComponents_NoLog", "Yes");
			}
			try
			{
				FFUManager.Start();
				ICollection<IFFUDevice> collection = new List<IFFUDevice>();
				FFUManager.GetFlashableDevices(ref collection);
				if (collection.Count == 0)
				{
					if (!flag2)
					{
						Console.WriteLine(Resources.LOGGING_UFP_TO_LOG, FFUManager.GetUFPLogPath());
					}
					Console.WriteLine(Resources.NO_CONNECTED_DEVICES);
					Environment.ExitCode = 0;
				}
				else if (FFUTool.listParam.IsMatch(args[0]))
				{
					Console.WriteLine(Resources.DEVICES_FOUND, collection.Count);
					int num = 0;
					foreach (IFFUDevice iffudevice in collection)
					{
						Console.WriteLine(Resources.DEVICE_NO, num);
						Console.WriteLine(Resources.NAME, iffudevice.DeviceFriendlyName);
						Console.WriteLine(Resources.ID, iffudevice.DeviceUniqueID);
						Console.WriteLine(Resources.DEVICE_TYPE, iffudevice.DeviceType);
						Console.WriteLine();
						num++;
					}
					Environment.ExitCode = 0;
				}
				else
				{
					FlashParam[] array = new FlashParam[collection.Count];
					using (EtwSession session = new EtwSession(!flag2))
					{
						Console.CancelKeyPress += delegate(object A_1, ConsoleCancelEventArgs A_2)
						{
							session.Dispose();
						};
						if (!flag2)
						{
							foreach (IFFUDevice iffudevice2 in collection)
							{
								if (string.Compare(iffudevice2.DeviceType, "UFPDevice") == 0)
								{
									flag4 = true;
								}
								else if (string.Compare(iffudevice2.DeviceType, "SimpleIODevice") == 0)
								{
									flag3 = true;
								}
							}
							if (flag3)
							{
								Console.WriteLine(Resources.LOGGING_SIMPLEIO_TO_ETL, session.EtlPath);
							}
							if (flag4)
							{
								Console.WriteLine(Resources.LOGGING_UFP_TO_LOG, FFUManager.GetUFPLogPath());
							}
						}
						Console.WriteLine();
						ConsoleEx.Instance.Initialize(collection);
						int num2 = 0;
						try
						{
							foreach (IFFUDevice device in collection)
							{
								AutoResetEvent waitHandle = new AutoResetEvent(false);
								if (FFUTool.uefiFlashParam.IsMatch(args[0]))
								{
									FlashParam[] array2 = array;
									int num3 = num2;
									FlashParam flashParam = new FlashParam();
									flashParam.Device = device;
									flashParam.FfuFilePath = text;
									flashParam.WaitHandle = waitHandle;
									flashParam.FastFlash = false;
									FlashParam param2 = flashParam;
									array2[num3] = flashParam;
									FlashParam param = param2;
									ThreadPool.QueueUserWorkItem(delegate(object s)
									{
										FFUTool.DoFlash(param);
									});
								}
								else if (FFUTool.fastFlashParam.IsMatch(args[0]))
								{
									FlashParam[] array3 = array;
									int num4 = num2;
									FlashParam flashParam2 = new FlashParam();
									flashParam2.Device = device;
									flashParam2.FfuFilePath = text;
									flashParam2.WaitHandle = waitHandle;
									flashParam2.FastFlash = true;
									FlashParam param2 = flashParam2;
									array3[num4] = flashParam2;
									FlashParam param = param2;
									ThreadPool.QueueUserWorkItem(delegate(object s)
									{
										FFUTool.DoFlash(param);
									});
								}
								else if (FFUTool.flashParam.IsMatch(args[0]))
								{
									FlashParam[] array4 = array;
									int num5 = num2;
									FlashParam flashParam3 = new FlashParam();
									flashParam3.Device = device;
									flashParam3.FfuFilePath = text;
									flashParam3.WaitHandle = waitHandle;
									flashParam3.FastFlash = true;
									FlashParam param2 = flashParam3;
									array4[num5] = flashParam3;
									FlashParam param = param2;
									ThreadPool.QueueUserWorkItem(delegate(object s)
									{
										FFUTool.DoFlash(param);
									});
								}
								else if (FFUTool.skipParam.IsMatch(args[0]))
								{
									FlashParam[] array5 = array;
									int num6 = num2;
									FlashParam flashParam4 = new FlashParam();
									flashParam4.Device = device;
									flashParam4.WaitHandle = waitHandle;
									FlashParam param2 = flashParam4;
									array5[num6] = flashParam4;
									FlashParam param = param2;
									ThreadPool.QueueUserWorkItem(delegate(object s)
									{
										FFUTool.DoSkip(param);
									});
								}
								else if (FFUTool.massParam.IsMatch(args[0]))
								{
									FlashParam[] array6 = array;
									int num7 = num2;
									FlashParam flashParam5 = new FlashParam();
									flashParam5.Device = device;
									flashParam5.WaitHandle = waitHandle;
									FlashParam param2 = flashParam5;
									array6[num7] = flashParam5;
									FlashParam param = param2;
									ThreadPool.QueueUserWorkItem(delegate(object s)
									{
										FFUTool.DoMassStorage(param);
									});
								}
								else if (FFUTool.clearIdParam.IsMatch(args[0]))
								{
									FlashParam[] array7 = array;
									int num8 = num2;
									FlashParam flashParam6 = new FlashParam();
									flashParam6.Device = device;
									flashParam6.WaitHandle = waitHandle;
									FlashParam param2 = flashParam6;
									array7[num8] = flashParam6;
									FlashParam param = param2;
									ThreadPool.QueueUserWorkItem(delegate(object s)
									{
										FFUTool.DoClearId(param);
									});
								}
								else if (FFUTool.serParam.IsMatch(args[0]))
								{
									FlashParam[] array8 = array;
									int num9 = num2;
									FlashParam flashParam7 = new FlashParam();
									flashParam7.Device = device;
									flashParam7.WaitHandle = waitHandle;
									FlashParam param2 = flashParam7;
									array8[num9] = flashParam7;
									FlashParam param = param2;
									ThreadPool.QueueUserWorkItem(delegate(object s)
									{
										FFUTool.DoSerialNumber(param);
									});
								}
								else if (FFUTool.wimParam.IsMatch(args[0]))
								{
									FlashParam[] array9 = array;
									int num10 = num2;
									FlashParam flashParam8 = new FlashParam();
									flashParam8.Device = device;
									flashParam8.FfuFilePath = text;
									flashParam8.WaitHandle = waitHandle;
									FlashParam param2 = flashParam8;
									array9[num10] = flashParam8;
									FlashParam param = param2;
									ThreadPool.QueueUserWorkItem(delegate(object s)
									{
										FFUTool.DoWim(param);
									});
								}
								else if (FFUTool.setBootModeParam.IsMatch(args[0]))
								{
									FlashParam[] array10 = array;
									int num11 = num2;
									SetBootModeParam setBootModeParam = new SetBootModeParam();
									setBootModeParam.Device = device;
									setBootModeParam.BootMode = bootMode;
									setBootModeParam.ProfileName = profileName;
									setBootModeParam.WaitHandle = waitHandle;
									FlashParam param2 = setBootModeParam;
									array10[num11] = setBootModeParam;
									FlashParam param = param2;
									ThreadPool.QueueUserWorkItem(delegate(object s)
									{
										FFUTool.DoSetBootMode(param as SetBootModeParam);
									});
								}
								else if (FFUTool.servicingLogsParam.IsMatch(args[0]))
								{
									FlashParam[] array11 = array;
									int num12 = num2;
									FlashParam flashParam9 = new FlashParam();
									flashParam9.Device = device;
									flashParam9.LogFolderPath = text;
									flashParam9.WaitHandle = waitHandle;
									FlashParam param2 = flashParam9;
									array11[num12] = flashParam9;
									FlashParam param = param2;
									ThreadPool.QueueUserWorkItem(delegate(object s)
									{
										FFUTool.DoServicingLogs(param);
									});
								}
								else if (FFUTool.flashLogsParam.IsMatch(args[0]))
								{
									FlashParam[] array12 = array;
									int num13 = num2;
									FlashParam flashParam10 = new FlashParam();
									flashParam10.Device = device;
									flashParam10.LogFolderPath = text;
									flashParam10.WaitHandle = waitHandle;
									FlashParam param2 = flashParam10;
									array12[num13] = flashParam10;
									FlashParam param = param2;
									ThreadPool.QueueUserWorkItem(delegate(object s)
									{
										FFUTool.DoFlashLogs(param);
									});
								}
								num2++;
							}
							WaitHandle.WaitAll((from p in array
							select p.WaitHandle).ToArray<WaitHandle>());
							if (array.Any((FlashParam p) => p.Result == -1))
							{
								Console.WriteLine(Resources.ERROR_AT_LEAST_ONE_DEVICE_FAILED);
								Environment.ExitCode = -1;
							}
						}
						finally
						{
							Console.CancelKeyPress -= delegate(object A_1, ConsoleCancelEventArgs A_2)
							{
								session.Dispose();
							};
						}
					}
				}
			}
			catch (FFUException ex)
			{
				Console.WriteLine();
				Console.WriteLine(Resources.ERROR_FFU + ex.Message);
				Environment.ExitCode = -1;
			}
			catch (TimeoutException ex2)
			{
				Console.WriteLine();
				Console.WriteLine(Resources.ERROR_TIMED_OUT + ex2.Message);
				Environment.ExitCode = -1;
			}
			finally
			{
				FFUManager.Stop();
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x0000351C File Offset: 0x0000171C
		private static void DoSerialNumber(FlashParam param)
		{
			if (param == null)
			{
				throw new ArgumentNullException("param");
			}
			try
			{
				byte[] value = param.Device.SerialNumber.ToByteArray();
				ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.MESSAGE, string.Format(CultureInfo.InvariantCulture, Resources.SERIAL_NO_FORMAT, new object[]
				{
					Resources.SERIAL_NO,
					BitConverter.ToString(value).Replace("-", string.Empty)
				}));
			}
			catch (Exception data)
			{
				param.Result = -1;
				ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.EXCEPTION, data);
			}
			finally
			{
				param.WaitHandle.Set();
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000035DC File Offset: 0x000017DC
		private static void DoSkip(FlashParam param)
		{
			if (param == null)
			{
				throw new ArgumentNullException("param");
			}
			try
			{
				ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.MESSAGE, Resources.STATUS_SKIPPING);
				if (param.Device.SkipTransfer())
				{
					ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.MESSAGE, Resources.STATUS_SKIPPED);
				}
				else
				{
					param.Result = -1;
					ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.ERROR, Resources.ERROR_SKIP_TRANSFER);
				}
			}
			catch (Exception data)
			{
				param.Result = -1;
				ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.EXCEPTION, data);
			}
			finally
			{
				param.WaitHandle.Set();
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00003698 File Offset: 0x00001898
		private static void DoMassStorage(FlashParam param)
		{
			if (param == null)
			{
				throw new ArgumentNullException("param");
			}
			try
			{
				if (param.Device.EnterMassStorage())
				{
					ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.MESSAGE, Resources.RESET_MASS_STORAGE_MODE);
				}
				else
				{
					param.Result = -1;
					ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.ERROR, Resources.ERROR_RESET_MASS_STORAGE_MODE);
				}
			}
			catch (Exception data)
			{
				param.Result = -1;
				ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.EXCEPTION, data);
			}
			finally
			{
				param.WaitHandle.Set();
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00003740 File Offset: 0x00001940
		private static void DoClearId(FlashParam param)
		{
			if (param == null)
			{
				throw new ArgumentNullException("param");
			}
			try
			{
				Console.WriteLine(Resources.DEVICE_ID, param.Device.DeviceFriendlyName);
				if (param.Device.ClearIdOverride())
				{
					param.Result = 0;
					ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.MESSAGE, string.Format(CultureInfo.CurrentUICulture, Resources.REMOVE_PLATFORM_ID, new object[]
					{
						param.Device.DeviceFriendlyName
					}));
				}
				else
				{
					param.Result = -1;
					ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.ERROR, Resources.ERROR_NO_PLATFORM_ID);
				}
			}
			catch (Exception data)
			{
				param.Result = -1;
				ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.EXCEPTION, data);
			}
			finally
			{
				param.WaitHandle.Set();
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00003820 File Offset: 0x00001A20
		private static void DoWim(FlashParam param)
		{
			if (param == null)
			{
				throw new ArgumentNullException("param");
			}
			try
			{
				ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.MESSAGE, string.Format(CultureInfo.CurrentUICulture, Resources.BOOTING_WIM, new object[]
				{
					Path.GetFileName(param.FfuFilePath)
				}));
				Stopwatch stopwatch = Stopwatch.StartNew();
				param.Device.EndTransfer();
				bool flag = param.Device.WriteWim(param.FfuFilePath);
				stopwatch.Stop();
				if (flag)
				{
					ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.MESSAGE, string.Format(CultureInfo.CurrentUICulture, Resources.WIM_TRANSFER_RATE, new object[]
					{
						stopwatch.Elapsed.TotalSeconds
					}));
				}
				else
				{
					ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.MESSAGE, Resources.ERROR_BOOT_WIM);
				}
			}
			catch (Exception data)
			{
				param.Result = -1;
				ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.EXCEPTION, data);
			}
			finally
			{
				param.WaitHandle.Set();
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00003938 File Offset: 0x00001B38
		private static void DoServicingLogs(FlashParam param)
		{
			if (param == null)
			{
				throw new ArgumentNullException("param");
			}
			try
			{
				ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.MESSAGE, Resources.STATUS_LOGS);
				string servicingLogs = param.Device.GetServicingLogs(param.LogFolderPath);
				ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.MESSAGE, string.Format(CultureInfo.CurrentUICulture, Resources.LOGS_PATH, new object[]
				{
					servicingLogs
				}));
			}
			catch (Exception data)
			{
				param.Result = -1;
				ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.EXCEPTION, data);
			}
			finally
			{
				param.WaitHandle.Set();
			}
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000039F0 File Offset: 0x00001BF0
		private static void DoFlashLogs(FlashParam param)
		{
			if (param == null)
			{
				throw new ArgumentNullException("param");
			}
			try
			{
				ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.MESSAGE, Resources.STATUS_LOGS);
				string flashingLogs = param.Device.GetFlashingLogs(param.LogFolderPath);
				ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.MESSAGE, string.Format(CultureInfo.CurrentUICulture, Resources.LOGS_PATH, new object[]
				{
					flashingLogs
				}));
			}
			catch (Exception data)
			{
				param.Result = -1;
				ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.EXCEPTION, data);
			}
			finally
			{
				param.WaitHandle.Set();
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00003AA8 File Offset: 0x00001CA8
		private static void PrepareFlash(IFFUDevice device)
		{
			device.EndTransfer();
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00003AB4 File Offset: 0x00001CB4
		private static void TransferWimIfPresent(ref IFFUDevice device, string ffuFilePath, string wimFilePath)
		{
			IFFUDevice wimDevice = null;
			Guid id = device.DeviceUniqueID;
			ManualResetEvent deviceConnected = new ManualResetEvent(false);
			EventHandler<ConnectEventArgs> value = delegate(object sender, ConnectEventArgs e)
			{
				if (e.Device.DeviceUniqueID == id)
				{
					wimDevice = e.Device;
					deviceConnected.Set();
				}
			};
			if (string.IsNullOrEmpty(wimFilePath))
			{
				wimFilePath = Path.Combine(Path.GetDirectoryName(ffuFilePath), "flashwim.wim");
			}
			if (File.Exists(wimFilePath))
			{
				FFUManager.DeviceConnectEvent += value;
				ConsoleEx.Instance.UpdateStatus(device, DeviceStatus.TRANSFER_WIM, wimFilePath);
				bool flag = false;
				try
				{
					flag = device.WriteWim(wimFilePath);
				}
				catch (FFUException)
				{
				}
				if (flag)
				{
					ConsoleEx.Instance.UpdateStatus(device, DeviceStatus.BOOTING_WIM, wimFilePath);
					bool flag2 = deviceConnected.WaitOne(TimeSpan.FromSeconds(30.0));
					FFUManager.DeviceConnectEvent -= value;
					if (!flag2)
					{
						throw new FFUException(device.DeviceFriendlyName, device.DeviceUniqueID, Resources.ERROR_WIM_BOOT);
					}
					device = wimDevice;
				}
			}
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00003BA0 File Offset: 0x00001DA0
		private static void FlashFile(IFFUDevice device, string ffuFilePath, bool optimize)
		{
			ConsoleEx.Instance.UpdateStatus(device, DeviceStatus.FLASHING, null);
			device.ProgressEvent += FFUTool.Device_ProgressEvent;
			device.EndTransfer();
			device.FlashFFUFile(ffuFilePath, optimize);
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00003BD0 File Offset: 0x00001DD0
		private static void DoFlash(FlashParam param)
		{
			if (param == null)
			{
				throw new ArgumentNullException("param");
			}
			try
			{
				FFUTool.PrepareFlash(param.Device);
				FFUTool.FlashFile(param.Device, param.FfuFilePath, param.FastFlash);
				ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.DONE, null);
			}
			catch (Exception data)
			{
				param.Result = -1;
				ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.EXCEPTION, data);
			}
			finally
			{
				param.WaitHandle.Set();
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00003C68 File Offset: 0x00001E68
		private static void DoSetBootMode(SetBootModeParam param)
		{
			if (param == null)
			{
				throw new ArgumentNullException("param");
			}
			try
			{
				uint num = param.Device.SetBootMode(param.BootMode, param.ProfileName);
				if (num == 0U)
				{
					ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.MESSAGE, Resources.RESET_BOOT_MODE);
				}
				else
				{
					param.Result = -1;
					ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.ERROR, string.Format(CultureInfo.CurrentUICulture, Resources.ERROR_RESET_BOOT_MODE, new object[]
					{
						num
					}));
				}
			}
			catch (Exception data)
			{
				param.Result = -1;
				ConsoleEx.Instance.UpdateStatus(param.Device, DeviceStatus.EXCEPTION, data);
			}
			finally
			{
				param.WaitHandle.Set();
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00003D34 File Offset: 0x00001F34
		private static void Device_ProgressEvent(object sender, ProgressEventArgs e)
		{
			ConsoleEx.Instance.UpdateProgress(e);
		}

		// Token: 0x0400003C RID: 60
		private static Regex flashParam = new Regex("[-/]flash$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// Token: 0x0400003D RID: 61
		private static Regex uefiFlashParam = new Regex("[-/]uefiflash$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// Token: 0x0400003E RID: 62
		private static Regex fastFlashParam = new Regex("[-/]fastflash$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// Token: 0x0400003F RID: 63
		private static Regex skipParam = new Regex("[-/]skip$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// Token: 0x04000040 RID: 64
		private static Regex listParam = new Regex("[-/]list$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// Token: 0x04000041 RID: 65
		private static Regex forceParam = new Regex("[-/]force$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// Token: 0x04000042 RID: 66
		private static Regex massParam = new Regex("[-/]massStorage$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// Token: 0x04000043 RID: 67
		private static Regex clearIdParam = new Regex("[-/]clearId$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// Token: 0x04000044 RID: 68
		private static Regex serParam = new Regex("[-/]serial$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// Token: 0x04000045 RID: 69
		private static Regex wimParam = new Regex("[-/]wim$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// Token: 0x04000046 RID: 70
		private static Regex setBootModeParam = new Regex("[-/]setBootMode$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// Token: 0x04000047 RID: 71
		private static Regex servicingLogsParam = new Regex("[-/]getServicingLogs", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// Token: 0x04000048 RID: 72
		private static Regex flashLogsParam = new Regex("[-/]getFlashingLogs", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// Token: 0x04000049 RID: 73
		private static Regex noLogParam = new Regex("[-/]noLog", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	}
}
