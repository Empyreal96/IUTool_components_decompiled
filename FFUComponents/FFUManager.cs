using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Windows.Flashing.Platform;

namespace FFUComponents
{
	// Token: 0x02000023 RID: 35
	public static class FFUManager
	{
		// Token: 0x14000002 RID: 2
		// (add) Token: 0x060000CD RID: 205 RVA: 0x00003D4C File Offset: 0x00001F4C
		// (remove) Token: 0x060000CE RID: 206 RVA: 0x00003D80 File Offset: 0x00001F80
		public static event EventHandler<ConnectEventArgs> DeviceConnectEvent;

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x060000CF RID: 207 RVA: 0x00003DB4 File Offset: 0x00001FB4
		// (remove) Token: 0x060000D0 RID: 208 RVA: 0x00003DE8 File Offset: 0x00001FE8
		public static event EventHandler<DisconnectEventArgs> DeviceDisconnectEvent;

		// Token: 0x060000D1 RID: 209 RVA: 0x00003E1C File Offset: 0x0000201C
		internal static void DisconnectDevice(Guid id)
		{
			List<IFFUDeviceInternal> list = new List<IFFUDeviceInternal>(FFUManager.activeFFUDevices.Count);
			IList<IFFUDeviceInternal> obj = FFUManager.activeFFUDevices;
			lock (obj)
			{
				for (int i = 0; i < FFUManager.activeFFUDevices.Count; i++)
				{
					if (FFUManager.activeFFUDevices[i].DeviceUniqueID == id)
					{
						list.Add(FFUManager.activeFFUDevices[i]);
						FFUManager.activeFFUDevices.RemoveAt(i);
					}
				}
			}
			foreach (IFFUDeviceInternal device in list)
			{
				FFUManager.disconnectTimer.StopTimer(device);
				FFUManager.OnDisconnect(device);
			}
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00003EFC File Offset: 0x000020FC
		internal static void DisconnectDevice(SimpleIODevice deviceToRemove)
		{
			IFFUDeviceInternal iffudeviceInternal = null;
			IList<IFFUDeviceInternal> obj = FFUManager.activeFFUDevices;
			lock (obj)
			{
				if (FFUManager.activeFFUDevices.Remove(deviceToRemove))
				{
					iffudeviceInternal = deviceToRemove;
				}
			}
			if (iffudeviceInternal != null)
			{
				FFUManager.disconnectTimer.StopTimer(iffudeviceInternal);
				FFUManager.OnDisconnect(iffudeviceInternal);
			}
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00003F5C File Offset: 0x0000215C
		internal static void DisconnectDevice(ThorDevice deviceToRemove)
		{
			ThorDevice thorDevice = null;
			IList<IFFUDeviceInternal> obj = FFUManager.activeFFUDevices;
			lock (obj)
			{
				if (FFUManager.activeFFUDevices.Remove(deviceToRemove))
				{
					thorDevice = deviceToRemove;
				}
			}
			if (thorDevice != null)
			{
				FFUManager.OnDisconnect(thorDevice);
			}
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00003FB0 File Offset: 0x000021B0
		private static bool DevicePresent(Guid id)
		{
			bool result = false;
			IList<IFFUDeviceInternal> obj = FFUManager.activeFFUDevices;
			lock (obj)
			{
				for (int i = 0; i < FFUManager.activeFFUDevices.Count; i++)
				{
					if (FFUManager.activeFFUDevices[i].DeviceUniqueID == id)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00004020 File Offset: 0x00002220
		private static void StartTimerIfNecessary(IFFUDeviceInternal device)
		{
			if (device.NeedsTimer())
			{
				DisconnectTimer disconnectTimer = FFUManager.disconnectTimer;
				if (disconnectTimer != null)
				{
					disconnectTimer.StartTimer(device);
				}
			}
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00004045 File Offset: 0x00002245
		private static void OnConnect(IFFUDeviceInternal device)
		{
			if (device != null)
			{
				if (FFUManager.DeviceConnectEvent != null)
				{
					FFUManager.DeviceConnectEvent(null, new ConnectEventArgs(device));
				}
				FFUManager.HostLogger.EventWriteDevice_Attach(device.DeviceUniqueID, device.DeviceFriendlyName);
			}
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x0000407C File Offset: 0x0000227C
		private static void OnDisconnect(IFFUDeviceInternal device)
		{
			if (device != null && !FFUManager.DevicePresent(device.DeviceUniqueID))
			{
				if (FFUManager.DeviceDisconnectEvent != null)
				{
					FFUManager.DeviceDisconnectEvent(null, new DisconnectEventArgs(device.DeviceUniqueID));
				}
				FFUManager.HostLogger.EventWriteDevice_Remove(device.DeviceUniqueID, device.DeviceFriendlyName);
			}
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x000040D0 File Offset: 0x000022D0
		internal static void OnDeviceConnect(string usbDevicePath)
		{
			IList<IFFUDeviceInternal> obj = FFUManager.activeFFUDevices;
			lock (obj)
			{
				IFFUDeviceInternal device2 = null;
				IFFUDeviceInternal iffudeviceInternal = null;
				foreach (string value in FFUManager.ThorDevicePids)
				{
					if (usbDevicePath.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0 && FFUManager.flashingPlatform.CreateConnectedDevice(usbDevicePath).CanFlash())
					{
						FlashingDevice flashingDevice = null;
						try
						{
							flashingDevice = FFUManager.flashingPlatform.CreateFlashingDevice(usbDevicePath);
						}
						catch (Exception)
						{
						}
						if (flashingDevice != null)
						{
							iffudeviceInternal = new ThorDevice(flashingDevice, usbDevicePath);
							FFUManager.activeFFUDevices.Add(iffudeviceInternal);
							FFUManager.OnConnect(iffudeviceInternal);
						}
					}
				}
				if (iffudeviceInternal == null)
				{
					SimpleIODevice device = new SimpleIODevice(usbDevicePath);
					if (device.OnConnect(device))
					{
						IFFUDeviceInternal iffudeviceInternal2 = FFUManager.activeFFUDevices.SingleOrDefault((IFFUDeviceInternal deviceInstance) => deviceInstance.DeviceUniqueID == device.DeviceUniqueID);
						IFFUDeviceInternal iffudeviceInternal3 = FFUManager.disconnectTimer.StopTimer(device);
						if (iffudeviceInternal2 == null && iffudeviceInternal3 != null)
						{
							FFUManager.activeFFUDevices.Add(iffudeviceInternal3);
							iffudeviceInternal2 = iffudeviceInternal3;
							iffudeviceInternal = iffudeviceInternal3;
						}
						if (iffudeviceInternal2 != null && !((SimpleIODevice)iffudeviceInternal2).OnConnect(device))
						{
							FFUManager.activeFFUDevices.Remove(iffudeviceInternal2);
							device2 = iffudeviceInternal2;
							iffudeviceInternal2 = null;
						}
						if (iffudeviceInternal2 == null)
						{
							iffudeviceInternal = device;
							FFUManager.activeFFUDevices.Add(device);
						}
						FFUManager.OnDisconnect(device2);
						FFUManager.OnConnect(iffudeviceInternal);
					}
				}
			}
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00004274 File Offset: 0x00002474
		internal static void OnDeviceDisconnect(string usbDevicePath)
		{
			FFUManager.<>c__DisplayClass24_0 CS$<>8__locals1 = new FFUManager.<>c__DisplayClass24_0();
			CS$<>8__locals1.usbDevicePath = usbDevicePath;
			List<IFFUDeviceInternal> list = new List<IFFUDeviceInternal>();
			IList<IFFUDeviceInternal> obj = FFUManager.activeFFUDevices;
			lock (obj)
			{
				IEnumerable<IFFUDeviceInternal> source = FFUManager.activeFFUDevices;
				Func<IFFUDeviceInternal, bool> predicate;
				if ((predicate = CS$<>8__locals1.<>9__0) == null)
				{
					FFUManager.<>c__DisplayClass24_0 CS$<>8__locals2 = CS$<>8__locals1;
					predicate = (CS$<>8__locals2.<>9__0 = ((IFFUDeviceInternal d) => d.UsbDevicePath.Equals(CS$<>8__locals2.usbDevicePath, StringComparison.OrdinalIgnoreCase)));
				}
				foreach (IFFUDeviceInternal iffudeviceInternal in source.Where(predicate))
				{
					if (iffudeviceInternal != null)
					{
						list.Add(iffudeviceInternal);
					}
				}
				foreach (IFFUDeviceInternal iffudeviceInternal2 in list)
				{
					FFUManager.activeFFUDevices.Remove(iffudeviceInternal2);
					FFUManager.StartTimerIfNecessary(iffudeviceInternal2);
				}
			}
			foreach (IFFUDeviceInternal device in list)
			{
				FFUManager.OnDisconnect(device);
			}
		}

		// Token: 0x060000DA RID: 218 RVA: 0x000043B4 File Offset: 0x000025B4
		static FFUManager()
		{
			FFUManager.ffuManagerLock = new object();
			FFUManager.activeFFUDevices = new List<IFFUDeviceInternal>();
			FFUManager.HostLogger = new FlashingHostLogger();
			FFUManager.DeviceLogger = new FlashingDeviceLogger();
			string text = null;
			if (Environment.GetEnvironmentVariable("FFUComponents_NoLog") == null)
			{
				text = FFUManager.GetUFPLogPath();
			}
			FFUManager.flashingPlatform = new FlashingPlatform(text);
			FFUManager.deviceNotification = null;
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000DB RID: 219 RVA: 0x0000446C File Offset: 0x0000266C
		// (set) Token: 0x060000DC RID: 220 RVA: 0x00004473 File Offset: 0x00002673
		internal static FlashingHostLogger HostLogger { get; private set; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000DD RID: 221 RVA: 0x0000447B File Offset: 0x0000267B
		// (set) Token: 0x060000DE RID: 222 RVA: 0x00004482 File Offset: 0x00002682
		internal static FlashingDeviceLogger DeviceLogger { get; private set; }

		// Token: 0x060000DF RID: 223 RVA: 0x0000448C File Offset: 0x0000268C
		public static void Start()
		{
			object obj = FFUManager.ffuManagerLock;
			lock (obj)
			{
				if (!FFUManager.isStarted)
				{
					FFUManager.disconnectTimer = new DisconnectTimer();
					DeviceNotificationCallback deviceNotificationCallback = null;
					NotificationCallback notificationCallback = new NotificationCallback();
					List<Guid> list = new List<Guid>();
					list.Add(FFUManager.SimpleIOGuid);
					list.Add(FFUManager.WinUSBClassGuid);
					list.Add(FFUManager.WinUSBFlashingIfGuid);
					list.Add(FlashingPlatform.GuidDevinterfaceUfp);
					FFUManager.flashingPlatform.RegisterDeviceNotificationCallback(list.ToArray(), null, notificationCallback, ref deviceNotificationCallback);
					FFUManager.deviceNotification = notificationCallback;
					FFUManager.isStarted = true;
				}
			}
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00004538 File Offset: 0x00002738
		public static void Stop()
		{
			object obj = FFUManager.ffuManagerLock;
			lock (obj)
			{
				if (FFUManager.isStarted)
				{
					DeviceNotificationCallback deviceNotificationCallback = null;
					NotificationCallback notificationCallback = null;
					FFUManager.flashingPlatform.RegisterDeviceNotificationCallback(null, null, null, ref deviceNotificationCallback);
					FFUManager.deviceNotification = notificationCallback;
					IList<IFFUDeviceInternal> obj2 = FFUManager.activeFFUDevices;
					lock (obj2)
					{
						FFUManager.activeFFUDevices.Clear();
					}
					Interlocked.Exchange<DisconnectTimer>(ref FFUManager.disconnectTimer, null).StopAllTimers();
					FFUManager.isStarted = false;
				}
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000E1 RID: 225 RVA: 0x000045DC File Offset: 0x000027DC
		public static ICollection<IFFUDevice> FlashableDevices
		{
			get
			{
				ICollection<IFFUDevice> result = new List<IFFUDevice>();
				FFUManager.GetFlashableDevices(ref result);
				return result;
			}
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x000045F8 File Offset: 0x000027F8
		public static void GetFlashableDevices(ref ICollection<IFFUDevice> devices)
		{
			object obj = FFUManager.ffuManagerLock;
			lock (obj)
			{
				if (!FFUManager.isStarted)
				{
					throw new FFUManagerException(Resources.ERROR_FFUMANAGER_NOT_STARTED);
				}
				devices.Clear();
				IList<IFFUDeviceInternal> obj2 = FFUManager.activeFFUDevices;
				lock (obj2)
				{
					foreach (IFFUDeviceInternal item in FFUManager.activeFFUDevices)
					{
						devices.Add(item);
					}
				}
			}
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x000046B4 File Offset: 0x000028B4
		public static IFFUDevice GetFlashableDevice(string instancePath, bool enableFallback)
		{
			SimpleIODevice simpleIODevice = new SimpleIODevice(instancePath);
			SimpleIODevice simpleIODevice2 = simpleIODevice;
			if (simpleIODevice2.OnConnect(simpleIODevice2))
			{
				return simpleIODevice;
			}
			if (enableFallback)
			{
				string fallbackInstancePath = FFUManager.GetFallbackInstancePath(instancePath);
				if (!string.IsNullOrEmpty(fallbackInstancePath))
				{
					simpleIODevice = new SimpleIODevice(fallbackInstancePath);
					SimpleIODevice simpleIODevice3 = simpleIODevice;
					if (simpleIODevice3.OnConnect(simpleIODevice3))
					{
						return simpleIODevice;
					}
				}
			}
			return null;
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x000046F8 File Offset: 0x000028F8
		public static string GetUFPLogPath()
		{
			string str = Process.GetCurrentProcess().ProcessName + Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture);
			return Path.Combine(Path.GetTempPath(), str + ".log");
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00004744 File Offset: 0x00002944
		private static string ReplaceUsbSerial(Match match)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string value = match.Groups["serial"].Value;
			if (Regex.IsMatch(value, "[a-f0-9]{32}", RegexOptions.IgnoreCase))
			{
				for (int i = 0; i < 8; i++)
				{
					stringBuilder.AppendFormat("{0}{1}", value.ElementAt(i + 1), value.ElementAt(i));
					i++;
				}
				stringBuilder.Append("-");
				for (int j = 0; j < 3; j++)
				{
					for (int k = 0; k < 4; k++)
					{
						stringBuilder.AppendFormat("{0}{1}", value.ElementAt(8 + 4 * j + k + 1), value.ElementAt(8 + 4 * j + k));
						k++;
					}
					stringBuilder.Append("-");
				}
				for (int l = 0; l < 12; l++)
				{
					stringBuilder.AppendFormat("{0}{1}", value.ElementAt(20 + l + 1), value.ElementAt(20 + l));
					l++;
				}
				return match.ToString().Replace(value, stringBuilder.ToString());
			}
			if (Regex.IsMatch(value, "[a-f0-9]{8}(?:-[a-f0-9]{4}){3}-[a-f0-9]{12}", RegexOptions.IgnoreCase))
			{
				for (int m = 0; m < value.Length - 1; m++)
				{
					if (value.ElementAt(m) != '-')
					{
						stringBuilder.AppendFormat("{0}{1}", value.ElementAt(m + 1), value.ElementAt(m));
						m++;
					}
				}
				return match.ToString().Replace(value, stringBuilder.ToString());
			}
			return null;
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x000048F0 File Offset: 0x00002AF0
		private static string GetFallbackInstancePath(string instancePath)
		{
			MatchEvaluator evaluator = new MatchEvaluator(FFUManager.ReplaceUsbSerial);
			return Regex.Replace(instancePath, "\\\\\\?\\\\usb#vid_[a-zA-Z0-9]{4}&pid_[a-zA-Z0-9]{4}#(?<serial>.+)#{[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}}\\z", evaluator, RegexOptions.IgnoreCase);
		}

		// Token: 0x04000050 RID: 80
		public static FlashingPlatform flashingPlatform;

		// Token: 0x04000051 RID: 81
		public static NotificationCallback deviceNotification;

		// Token: 0x04000054 RID: 84
		private static object ffuManagerLock;

		// Token: 0x04000055 RID: 85
		private static IList<IFFUDeviceInternal> activeFFUDevices;

		// Token: 0x04000056 RID: 86
		private static DisconnectTimer disconnectTimer;

		// Token: 0x04000057 RID: 87
		private static bool isStarted = false;

		// Token: 0x04000058 RID: 88
		public static readonly Guid SimpleIOGuid = new Guid("{67EA0A90-FF06-417D-AB66-6676DCE879CD}");

		// Token: 0x04000059 RID: 89
		public static readonly Guid WinUSBClassGuid = new Guid("{88BAE032-5A81-49F0-BC3D-A4FF138216D6}");

		// Token: 0x0400005A RID: 90
		public static readonly Guid WinUSBFlashingIfGuid = new Guid("{82809DD0-51F5-11E1-B86C-0800200C9A66}");

		// Token: 0x0400005B RID: 91
		private static readonly string[] ThorDevicePids = new string[]
		{
			"pid_0658",
			"pid_066e",
			"pid_0714",
			"pid_0a02"
		};
	}
}
