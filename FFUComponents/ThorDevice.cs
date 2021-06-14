using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Windows.Flashing.Platform;

namespace FFUComponents
{
	// Token: 0x02000043 RID: 67
	public class ThorDevice : IFFUDeviceInternal, IFFUDevice, IDisposable
	{
		// Token: 0x1700004B RID: 75
		// (get) Token: 0x0600018B RID: 395 RVA: 0x00007D0A File Offset: 0x00005F0A
		public string DeviceFriendlyName
		{
			get
			{
				return this.flashingDevice.GetDeviceFriendlyName();
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x0600018C RID: 396 RVA: 0x00007D17 File Offset: 0x00005F17
		public Guid DeviceUniqueID
		{
			get
			{
				return this.flashingDevice.GetDeviceUniqueID();
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x0600018D RID: 397 RVA: 0x00007D24 File Offset: 0x00005F24
		public Guid SerialNumber
		{
			get
			{
				return this.flashingDevice.GetDeviceSerialNumber();
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x0600018E RID: 398 RVA: 0x00007D31 File Offset: 0x00005F31
		// (set) Token: 0x0600018F RID: 399 RVA: 0x00007D39 File Offset: 0x00005F39
		public string UsbDevicePath { get; set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000190 RID: 400 RVA: 0x00007D42 File Offset: 0x00005F42
		public string DeviceType
		{
			get
			{
				return "UFPDevice";
			}
		}

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000191 RID: 401 RVA: 0x00007D4C File Offset: 0x00005F4C
		// (remove) Token: 0x06000192 RID: 402 RVA: 0x00007D84 File Offset: 0x00005F84
		public event EventHandler<ProgressEventArgs> ProgressEvent;

		// Token: 0x06000193 RID: 403 RVA: 0x00007DBC File Offset: 0x00005FBC
		public ThorDevice(FlashingDevice device, string devicePath)
		{
			this.flashingDevice = device;
			this.UsbDevicePath = devicePath;
			try
			{
				USBSpeedChecker usbspeedChecker = new USBSpeedChecker(devicePath);
				this.connectionType = usbspeedChecker.GetConnectionSpeed();
			}
			catch
			{
				this.connectionType = ConnectionType.Unknown;
			}
			this.telemetryLogger = FlashingTelemetryLogger.Instance;
		}

		// Token: 0x06000194 RID: 404 RVA: 0x00007E18 File Offset: 0x00006018
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000195 RID: 405 RVA: 0x00007E27 File Offset: 0x00006027
		private void Dispose(bool fDisposing)
		{
			if (fDisposing)
			{
				FFUManager.DisconnectDevice(this);
			}
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00007E32 File Offset: 0x00006032
		public void FlashFFUFile(string ffuFilePath)
		{
			this.FlashFFUFile(ffuFilePath, false);
		}

		// Token: 0x06000197 RID: 407 RVA: 0x00007E3C File Offset: 0x0000603C
		public void FlashFFUFile(string ffuFilePath, bool optimizeHint)
		{
			Guid sessionId = Guid.NewGuid();
			try
			{
				this.telemetryLogger.LogFlashingInitialized(sessionId, this, optimizeHint, ffuFilePath);
				this.telemetryLogger.LogThorDeviceUSBConnectionType(sessionId, this.connectionType);
				long length = new FileInfo(ffuFilePath).Length;
				ThorDevice.Progress progress = new ThorDevice.Progress(this, length);
				HandleRef handleRef = default(HandleRef);
				this.telemetryLogger.LogFlashingStarted(sessionId);
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				this.flashingDevice.FlashFFUFile(ffuFilePath, 0, progress, handleRef);
				stopwatch.Stop();
				this.telemetryLogger.LogFlashingEnded(sessionId, stopwatch, ffuFilePath, this);
			}
			catch (Exception e)
			{
				this.telemetryLogger.LogFlashingException(sessionId, e);
				throw;
			}
			this.Reboot();
		}

		// Token: 0x06000198 RID: 408 RVA: 0x00007EF8 File Offset: 0x000060F8
		public bool WriteWim(string wimPath)
		{
			long length = new FileInfo(wimPath).Length;
			ThorDevice.Progress progress = new ThorDevice.Progress(this, length);
			this.flashingDevice.WriteWim(wimPath, progress);
			return true;
		}

		// Token: 0x06000199 RID: 409 RVA: 0x00006863 File Offset: 0x00004A63
		public bool EndTransfer()
		{
			return false;
		}

		// Token: 0x0600019A RID: 410 RVA: 0x00007F27 File Offset: 0x00006127
		public bool SkipTransfer()
		{
			this.flashingDevice.SkipTransfer();
			return true;
		}

		// Token: 0x0600019B RID: 411 RVA: 0x00007F35 File Offset: 0x00006135
		public bool Reboot()
		{
			this.flashingDevice.Reboot();
			return true;
		}

		// Token: 0x0600019C RID: 412 RVA: 0x00007F43 File Offset: 0x00006143
		public bool EnterMassStorage()
		{
			this.flashingDevice.EnterMassStorageMode();
			return true;
		}

		// Token: 0x0600019D RID: 413 RVA: 0x000061D0 File Offset: 0x000043D0
		public bool ClearIdOverride()
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600019E RID: 414 RVA: 0x000061D0 File Offset: 0x000043D0
		public bool GetDiskInfo(out uint blockSize, out ulong lastBlock)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600019F RID: 415 RVA: 0x000061D0 File Offset: 0x000043D0
		public void ReadDisk(ulong diskOffset, byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x000061D0 File Offset: 0x000043D0
		public void WriteDisk(ulong diskOffset, byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x00007F51 File Offset: 0x00006151
		public uint SetBootMode(uint bootMode, string profileName)
		{
			this.flashingDevice.SetBootMode(bootMode, profileName);
			this.Reboot();
			return 0U;
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x00007F68 File Offset: 0x00006168
		public string GetServicingLogs(string logFolderPath)
		{
			return this.flashingDevice.GetLogs(2, logFolderPath);
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x00007F77 File Offset: 0x00006177
		public string GetFlashingLogs(string logFolderPath)
		{
			return this.flashingDevice.GetLogs(1, logFolderPath);
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x00007F88 File Offset: 0x00006188
		public void QueryDeviceUnlockId(out byte[] unlockId, out byte[] oemId, out byte[] platformId)
		{
			UNLOCK_ID deviceUnlockID = this.flashingDevice.GetDeviceUnlockID();
			unlockId = new byte[deviceUnlockID.UnlockId.Length];
			oemId = new byte[deviceUnlockID.OemId.Length];
			platformId = new byte[deviceUnlockID.PlatformId.Length];
			Array.Copy(deviceUnlockID.UnlockId, unlockId, deviceUnlockID.UnlockId.Length);
			Array.Copy(deviceUnlockID.OemId, oemId, deviceUnlockID.OemId.Length);
			Array.Copy(deviceUnlockID.PlatformId, platformId, deviceUnlockID.PlatformId.Length);
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0000800D File Offset: 0x0000620D
		public void RelockDeviceUnlockId()
		{
			this.flashingDevice.RelockDevice();
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x0000801C File Offset: 0x0000621C
		public uint[] QueryUnlockTokenFiles()
		{
			UNLOCK_TOKEN_FILES unlock_TOKEN_FILES = this.flashingDevice.QueryUnlockTokenFiles();
			List<uint> list = new List<uint>();
			BitArray bitArray = new BitArray(unlock_TOKEN_FILES.TokenIdBitmask);
			uint num = 0U;
			while ((ulong)num < (ulong)((long)bitArray.Count))
			{
				if (bitArray.Get(Convert.ToInt32(num)))
				{
					list.Add(num);
				}
				num += 1U;
			}
			return list.ToArray();
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x000061D0 File Offset: 0x000043D0
		public void WriteUnlockTokenFile(uint unlockTokenId, byte[] fileData)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x00008073 File Offset: 0x00006273
		public bool QueryBitlockerState()
		{
			return this.flashingDevice.GetBitlockerState() > 0;
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x00008083 File Offset: 0x00006283
		public void Unlock(uint tokenId, string tokenFilePath, string pin)
		{
			this.flashingDevice.UnlockDevice(tokenId, tokenFilePath, pin);
		}

		// Token: 0x060001AA RID: 426 RVA: 0x00008093 File Offset: 0x00006293
		public byte[] GetDeviceProperties()
		{
			return this.flashingDevice.GetDeviceProperties();
		}

		// Token: 0x060001AB RID: 427 RVA: 0x00006863 File Offset: 0x00004A63
		public bool NeedsTimer()
		{
			return false;
		}

		// Token: 0x04000112 RID: 274
		private FlashingDevice flashingDevice;

		// Token: 0x04000113 RID: 275
		private FlashingTelemetryLogger telemetryLogger;

		// Token: 0x04000114 RID: 276
		private ConnectionType connectionType;

		// Token: 0x02000055 RID: 85
		private class Progress : GenericProgress
		{
			// Token: 0x06000263 RID: 611 RVA: 0x0000A89C File Offset: 0x00008A9C
			public Progress(ThorDevice device, long ffuFileSize)
			{
				this.Device = device;
				this.FfuFileSize = ffuFileSize;
			}

			// Token: 0x06000264 RID: 612 RVA: 0x0000A8B4 File Offset: 0x00008AB4
			public override void RegisterProgress(uint progress)
			{
				ProgressEventArgs args = new ProgressEventArgs(this.Device, (long)((ulong)progress * (ulong)this.FfuFileSize / 100UL), this.FfuFileSize);
				Task.Factory.StartNew(delegate()
				{
					this.Device.ProgressEvent(this.Device, args);
				});
			}

			// Token: 0x040001BF RID: 447
			private ThorDevice Device;

			// Token: 0x040001C0 RID: 448
			private long FfuFileSize;
		}
	}
}
