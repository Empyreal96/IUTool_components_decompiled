using System;
using System.Collections.Generic;
using FFUComponents;

namespace Microsoft.Windows.ImageTools
{
	// Token: 0x02000004 RID: 4
	internal class ConsoleEx
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static ConsoleEx Instance
		{
			get
			{
				if (ConsoleEx.instance == null)
				{
					object obj = ConsoleEx.syncRoot;
					lock (obj)
					{
						if (ConsoleEx.instance == null)
						{
							ConsoleEx.instance = new ConsoleEx();
						}
					}
				}
				return ConsoleEx.instance;
			}
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020A8 File Offset: 0x000002A8
		public void Initialize(ICollection<IFFUDevice> devices)
		{
			this.legacy = (devices.Count == 1);
			if (!this.legacy)
			{
				int num = devices.Count * 7 + 100;
				if (Console.BufferHeight < num)
				{
					Console.SetBufferSize(Console.BufferWidth, num);
				}
			}
			this.deviceRows = new Dictionary<Guid, Tuple<int, ProgressReporter>>();
			int i = 0;
			foreach (IFFUDevice iffudevice in devices)
			{
				Console.WriteLine(Resources.DEVICE_NO, i);
				Console.WriteLine(Resources.NAME, iffudevice.DeviceFriendlyName);
				Console.WriteLine(Resources.ID, iffudevice.DeviceUniqueID);
				Console.WriteLine(Resources.DEVICE_TYPE, iffudevice.DeviceType);
				this.deviceRows[iffudevice.DeviceUniqueID] = new Tuple<int, ProgressReporter>(i, new ProgressReporter());
				if (!this.legacy)
				{
					Console.WriteLine();
					Console.WriteLine();
					Console.WriteLine();
				}
				i++;
			}
			if (this.legacy)
			{
				this.lastcursorTop = 0;
				this.cursorTop = this.lastcursorTop;
				this.lastRow = this.GetDeviceCursorPosition(0, DeviceStatusPosition.DeviceStatus);
			}
			else
			{
				for (i = 0; i < this.RESEVERED_LINES; i++)
				{
					Console.WriteLine();
				}
				this.lastcursorTop = Console.CursorTop - this.RESEVERED_LINES;
				this.cursorTop = this.lastcursorTop - devices.Count * 7;
			}
			foreach (IFFUDevice device in devices)
			{
				this.UpdateStatus(device, DeviceStatus.CONNECTED, null);
			}
			this.error = false;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002258 File Offset: 0x00000458
		public void UpdateProgress(ProgressEventArgs progress)
		{
			Tuple<int, ProgressReporter> tuple = this.deviceRows[progress.Device.DeviceUniqueID];
			string text = tuple.Item2.CreateProgressDisplay(progress.Position, progress.Length);
			object obj = ConsoleEx.syncRoot;
			lock (obj)
			{
				this.WriteLine(text, this.GetDeviceCursorPosition(tuple.Item1, DeviceStatusPosition.DeviceProgress));
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000022D4 File Offset: 0x000004D4
		public void UpdateStatus(IFFUDevice device, DeviceStatus status, object data)
		{
			Tuple<int, ProgressReporter> tuple = this.deviceRows[device.DeviceUniqueID];
			object obj = ConsoleEx.syncRoot;
			lock (obj)
			{
				switch (status)
				{
				case DeviceStatus.CONNECTED:
					this.WriteLine(Resources.STATUS_CONNECTED, this.GetDeviceCursorPosition(tuple.Item1, DeviceStatusPosition.DeviceStatus));
					break;
				case DeviceStatus.FLASHING:
					this.WriteLine(Resources.STATUS_FLASHING, this.GetDeviceCursorPosition(tuple.Item1, DeviceStatusPosition.DeviceStatus));
					break;
				case DeviceStatus.TRANSFER_WIM:
					this.WriteLine(Resources.STATUS_TRANSFER_WIM, this.GetDeviceCursorPosition(tuple.Item1, DeviceStatusPosition.DeviceStatus));
					break;
				case DeviceStatus.BOOTING_WIM:
					this.WriteLine(Resources.STATUS_BOOTING_TO_WIM, this.GetDeviceCursorPosition(tuple.Item1, DeviceStatusPosition.DeviceStatus));
					break;
				case DeviceStatus.DONE:
					this.WriteLine(Resources.STATUS_DONE, this.GetDeviceCursorPosition(tuple.Item1, DeviceStatusPosition.DeviceStatus));
					if (this.legacy)
					{
						Console.WriteLine();
					}
					break;
				case DeviceStatus.EXCEPTION:
				{
					Exception ex = (Exception)data;
					this.WriteLine(Resources.STATUS_ERROR, this.GetDeviceCursorPosition(tuple.Item1, DeviceStatusPosition.DeviceStatus));
					if (this.legacy)
					{
						Console.WriteLine();
						Console.WriteLine(ex.Message);
					}
					else
					{
						if (!this.error)
						{
							Console.SetCursorPosition(0, this.lastcursorTop);
							Console.WriteLine(Resources.ERRORS);
							this.lastcursorTop = Console.CursorTop;
							this.error = true;
						}
						Console.SetCursorPosition(0, this.lastcursorTop);
						Console.WriteLine(Resources.DEVICE_NO, tuple.Item1);
						Console.WriteLine(ex.Message);
						Console.WriteLine();
						this.lastcursorTop = Console.CursorTop;
					}
					break;
				}
				case DeviceStatus.ERROR:
				case DeviceStatus.MESSAGE:
					this.WriteLine(data as string, this.GetDeviceCursorPosition(tuple.Item1, DeviceStatusPosition.DeviceStatus));
					break;
				default:
					throw new Exception(Resources.ERROR_UNEXPECTED_DEVICESTATUS);
				}
				if (!this.legacy)
				{
					Console.SetCursorPosition(0, this.lastcursorTop);
				}
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000024DC File Offset: 0x000006DC
		private int GetDeviceCursorPosition(int index, DeviceStatusPosition position)
		{
			return (int)(this.cursorTop + 7 * index + position);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000024EC File Offset: 0x000006EC
		private void WriteLine(string text, int row)
		{
			if (this.legacy)
			{
				if (row == this.lastRow)
				{
					string str = new string(' ', Console.WindowWidth - 1);
					Console.Write("\r" + str);
					Console.Write("\r" + text);
				}
				else
				{
					Console.WriteLine();
					Console.Write(text);
				}
				this.lastRow = row;
				return;
			}
			string value = new string(' ', Console.WindowWidth);
			Console.SetCursorPosition(0, row);
			Console.WriteLine(value);
			Console.SetCursorPosition(0, row);
			Console.WriteLine(text);
		}

		// Token: 0x04000012 RID: 18
		private static ConsoleEx instance;

		// Token: 0x04000013 RID: 19
		private static object syncRoot = new object();

		// Token: 0x04000014 RID: 20
		private Dictionary<Guid, Tuple<int, ProgressReporter>> deviceRows;

		// Token: 0x04000015 RID: 21
		private int cursorTop;

		// Token: 0x04000016 RID: 22
		private int lastcursorTop;

		// Token: 0x04000017 RID: 23
		private bool error;

		// Token: 0x04000018 RID: 24
		private bool legacy;

		// Token: 0x04000019 RID: 25
		private int lastRow;

		// Token: 0x0400001A RID: 26
		private readonly int RESEVERED_LINES = 30;
	}
}
