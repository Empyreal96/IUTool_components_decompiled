using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace FFUComponents
{
	// Token: 0x02000041 RID: 65
	public class SimpleIODevice : IFFUDeviceInternal, IFFUDevice, IDisposable
	{
		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000143 RID: 323 RVA: 0x00004E76 File Offset: 0x00003076
		// (set) Token: 0x06000144 RID: 324 RVA: 0x00004E7E File Offset: 0x0000307E
		public string DeviceFriendlyName { get; private set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000145 RID: 325 RVA: 0x00004E87 File Offset: 0x00003087
		// (set) Token: 0x06000146 RID: 326 RVA: 0x00004E8F File Offset: 0x0000308F
		public Guid DeviceUniqueID { get; private set; }

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000147 RID: 327 RVA: 0x00004E98 File Offset: 0x00003098
		public Guid SerialNumber
		{
			get
			{
				if (!this.serialNumberChecked)
				{
					this.serialNumberChecked = true;
					this.serialNumber = this.GetSerialNumberFromDevice();
				}
				return this.serialNumber;
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000148 RID: 328 RVA: 0x00004EBB File Offset: 0x000030BB
		// (set) Token: 0x06000149 RID: 329 RVA: 0x00004EC4 File Offset: 0x000030C4
		public string UsbDevicePath
		{
			get
			{
				return this.usbDevicePath;
			}
			private set
			{
				object obj = this.pathSync;
				lock (obj)
				{
					if (this.syncMutex != null)
					{
						this.syncMutex.Close();
						this.syncMutex = null;
					}
					string str = this.GetPnPIdFromDevicePath(value).Replace('\\', '_');
					this.syncMutex = new Mutex(false, "Global\\FFU_Mutex_" + str);
					this.usbDevicePath = value;
				}
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x0600014A RID: 330 RVA: 0x00004F48 File Offset: 0x00003148
		public string DeviceType
		{
			get
			{
				return "SimpleIODevice";
			}
		}

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x0600014B RID: 331 RVA: 0x00004F50 File Offset: 0x00003150
		// (remove) Token: 0x0600014C RID: 332 RVA: 0x00004F88 File Offset: 0x00003188
		public event EventHandler<ProgressEventArgs> ProgressEvent;

		// Token: 0x0600014D RID: 333 RVA: 0x00004FC0 File Offset: 0x000031C0
		public SimpleIODevice(string devicePath)
		{
			this.fConnected = false;
			this.fOperationStarted = false;
			this.forceClearOnReconnect = true;
			this.usbStream = null;
			this.memStm = new MemoryStream();
			this.connectEvent = new AutoResetEvent(false);
			this.pathSync = new object();
			this.UsbDevicePath = devicePath;
			this.hostLogger = FFUManager.HostLogger;
			this.deviceLogger = FFUManager.DeviceLogger;
			this.packets = new PacketConstructor();
			this.DeviceUniqueID = Guid.Empty;
			this.DeviceFriendlyName = string.Empty;
			this.resetCount = 0;
			this.diskTransferSize = 0;
			this.diskBlockSize = 0U;
			this.diskLastBlock = 0UL;
			this.serialNumber = Guid.Empty;
			this.serialNumberChecked = false;
			this.usbTransactionSize = 16376;
			this.supportsFastFlash = false;
			this.supportsCompatFastFlash = false;
			this.hasCheckedForV2 = false;
			this.clientVersion = 1;
			this.telemetryLogger = FlashingTelemetryLogger.Instance;
			this.errorEvent = new ManualResetEvent(false);
			this.writeEvent = new AutoResetEvent(false);
		}

		// Token: 0x0600014E RID: 334 RVA: 0x000050CC File Offset: 0x000032CC
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x0600014F RID: 335 RVA: 0x000050DC File Offset: 0x000032DC
		private void Dispose(bool fDisposing)
		{
			if (fDisposing)
			{
				FFUManager.DisconnectDevice(this);
				if (this.usbStream != null)
				{
					this.usbStream.Dispose();
					this.usbStream = null;
					this.fConnected = false;
				}
				if (this.memStm != null)
				{
					this.memStm.Dispose();
					this.memStm = null;
				}
				if (this.syncMutex != null)
				{
					this.syncMutex.Close();
					this.syncMutex = null;
				}
				if (this.packets != null)
				{
					this.packets.Dispose();
					this.packets = null;
				}
				if (this.connectEvent != null)
				{
					this.connectEvent.Close();
					this.connectEvent = null;
				}
			}
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00005180 File Offset: 0x00003380
		public void FlashFFUFile(string ffuFilePath)
		{
			this.FlashFFUFile(ffuFilePath, false);
		}

		// Token: 0x06000151 RID: 337 RVA: 0x0000518C File Offset: 0x0000338C
		public void FlashFFUFile(string ffuFilePath, bool optimizeHint)
		{
			bool flag = false;
			if (this.curPosition != 0L)
			{
				throw new FFUException(this.DeviceFriendlyName, this.DeviceUniqueID, Resources.ERROR_ALREADY_RECEIVED_DATA);
			}
			this.lastProgress = 0L;
			this.fConnected = true;
			this.fOperationStarted = true;
			Guid sessionId = Guid.NewGuid();
			try
			{
				using (this.packets.DataStream = this.GetBufferedFileStream(ffuFilePath))
				{
					this.InitFlashingStream(optimizeHint, out flag);
					this.telemetryLogger.LogFlashingInitialized(sessionId, this, optimizeHint, ffuFilePath);
					this.hostLogger.EventWriteDeviceFlashParameters(this.usbTransactionSize, (int)this.packets.PacketDataSize);
					object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyVersionAttribute), false);
					if (customAttributes.Length != 0)
					{
						AssemblyVersionAttribute assemblyVersionAttribute = (AssemblyVersionAttribute)customAttributes[0];
						this.hostLogger.EventWriteFlash_Start(this.DeviceUniqueID, this.DeviceFriendlyName, string.Format(CultureInfo.CurrentCulture, Resources.MODULE_VERSION, new object[]
						{
							assemblyVersionAttribute.ToString()
						}));
					}
					this.telemetryLogger.LogFlashingStarted(sessionId);
					Stopwatch stopwatch = new Stopwatch();
					stopwatch.Start();
					if (flag)
					{
						byte[] array = new byte[this.usbTransactionSize];
						this.usbStream.BeginRead(array, 0, array.Length, new AsyncCallback(this.ErrorCallback), this.errorEvent);
					}
					this.TransferPackets(flag);
					this.WaitForEndResponse(flag);
					stopwatch.Stop();
					this.telemetryLogger.LogFlashingEnded(sessionId, stopwatch, ffuFilePath, this);
					this.hostLogger.EventWriteFlash_Stop(this.DeviceUniqueID, this.DeviceFriendlyName);
				}
			}
			catch (Exception e)
			{
				this.telemetryLogger.LogFlashingException(sessionId, e);
				throw;
			}
			finally
			{
				if (flag)
				{
					this.usbTransactionSize = 16376;
					this.packets.PacketDataSize = PacketConstructor.DefaultPacketDataSize;
				}
				this.fConnected = false;
				FFUManager.DisconnectDevice(this.DeviceUniqueID);
			}
		}

		// Token: 0x06000152 RID: 338 RVA: 0x000053B0 File Offset: 0x000035B0
		public bool WriteWim(string wimPath)
		{
			bool result = false;
			object obj = this.pathSync;
			lock (obj)
			{
				if (this.fConnected || !this.AcquirePathMutex())
				{
					return false;
				}
				this.fOperationStarted = true;
				try
				{
					using (Stream bufferedFileStream = this.GetBufferedFileStream(wimPath))
					{
						using (Stream stream = new MemoryStream(Resources.bootsdi))
						{
							using (this.usbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(1.0)))
							{
								this.usbStream.SetShortPacketTerminate();
								try
								{
									this.WriteWim(stream, bufferedFileStream);
								}
								catch (Win32Exception ex)
								{
									this.hostLogger.EventWriteWimWin32Exception(this.DeviceUniqueID, this.DeviceFriendlyName, ex.NativeErrorCode);
								}
								this.usbStream.SetTransferTimeout(TimeSpan.FromSeconds(15.0));
								result = this.ReadStatus();
							}
						}
					}
				}
				catch (IOException)
				{
					this.hostLogger.EventWriteWimIOException(this.DeviceUniqueID, this.DeviceFriendlyName);
				}
				catch (Win32Exception ex2)
				{
					this.hostLogger.EventWriteWimWin32Exception(this.DeviceUniqueID, this.DeviceFriendlyName, ex2.NativeErrorCode);
				}
				finally
				{
					this.ReleasePathMutex();
				}
			}
			return result;
		}

		// Token: 0x06000153 RID: 339 RVA: 0x000055CC File Offset: 0x000037CC
		public bool EndTransfer()
		{
			bool result = false;
			if (this.curPosition == 0L)
			{
				return true;
			}
			object obj = this.pathSync;
			lock (obj)
			{
				if (this.fConnected || !this.AcquirePathMutex())
				{
					return false;
				}
				try
				{
					using (DTSFUsbStream dtsfusbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(5.0)))
					{
						dtsfusbStream.WriteByte(8);
						byte[] array = new byte[this.usbTransactionSize];
						do
						{
							dtsfusbStream.Read(array, 0, array.Length);
						}
						while (array[0] == 5);
						if (array[0] == 6)
						{
							this.ReadBootmeFromStream(dtsfusbStream);
							if (this.curPosition == 0L)
							{
								result = true;
							}
						}
					}
				}
				catch (IOException)
				{
				}
				catch (Win32Exception)
				{
				}
				finally
				{
					this.ReleasePathMutex();
				}
			}
			return result;
		}

		// Token: 0x06000154 RID: 340 RVA: 0x000056DC File Offset: 0x000038DC
		public bool SkipTransfer()
		{
			bool result = false;
			object obj = this.pathSync;
			lock (obj)
			{
				if (this.curPosition != 0L || this.fConnected || !this.AcquirePathMutex())
				{
					return false;
				}
				this.fOperationStarted = true;
				try
				{
					using (DTSFUsbStream dtsfusbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(5.0)))
					{
						result = this.WriteSkip(dtsfusbStream);
					}
				}
				catch (IOException)
				{
					this.hostLogger.EventWriteSkipIOException(this.DeviceUniqueID, this.DeviceFriendlyName);
				}
				catch (Win32Exception ex)
				{
					this.hostLogger.EventWriteSkipWin32Exception(this.DeviceUniqueID, this.DeviceFriendlyName, ex.NativeErrorCode);
				}
				finally
				{
					this.ReleasePathMutex();
				}
			}
			return result;
		}

		// Token: 0x06000155 RID: 341 RVA: 0x000057EC File Offset: 0x000039EC
		public bool Reboot()
		{
			bool result = false;
			object obj = this.pathSync;
			lock (obj)
			{
				if (this.fConnected || !this.AcquirePathMutex())
				{
					return false;
				}
				try
				{
					using (DTSFUsbStream dtsfusbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(5.0)))
					{
						dtsfusbStream.WriteByte(10);
						result = true;
					}
				}
				catch (IOException)
				{
					this.hostLogger.EventWriteRebootIOException(this.DeviceUniqueID, this.DeviceFriendlyName);
				}
				catch (Win32Exception ex)
				{
					this.hostLogger.EventWriteRebootWin32Exception(this.DeviceUniqueID, this.DeviceFriendlyName, ex.NativeErrorCode);
				}
				finally
				{
					this.ReleasePathMutex();
				}
			}
			return result;
		}

		// Token: 0x06000156 RID: 342 RVA: 0x000058F0 File Offset: 0x00003AF0
		public bool EnterMassStorage()
		{
			bool result = false;
			object obj = this.pathSync;
			lock (obj)
			{
				if (this.fConnected || !this.AcquirePathMutex())
				{
					return false;
				}
				try
				{
					using (DTSFUsbStream dtsfusbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(5.0)))
					{
						dtsfusbStream.WriteByte(11);
						if (dtsfusbStream.ReadByte() == 3)
						{
							result = true;
						}
					}
				}
				catch (IOException)
				{
					this.hostLogger.EventWriteMassStorageIOException(this.DeviceUniqueID, this.DeviceFriendlyName);
				}
				catch (Win32Exception ex)
				{
					this.hostLogger.EventWriteMassStorageWin32Exception(this.DeviceUniqueID, this.DeviceFriendlyName, ex.NativeErrorCode);
				}
				finally
				{
					this.ReleasePathMutex();
				}
			}
			return result;
		}

		// Token: 0x06000157 RID: 343 RVA: 0x000059FC File Offset: 0x00003BFC
		public bool ClearIdOverride()
		{
			bool result = false;
			object obj = this.pathSync;
			lock (obj)
			{
				if (this.fConnected || !this.AcquirePathMutex())
				{
					return false;
				}
				try
				{
					using (DTSFUsbStream dtsfusbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(1.0)))
					{
						dtsfusbStream.WriteByte(15);
						if (dtsfusbStream.ReadByte() == 3)
						{
							result = true;
							this.ReadBootmeFromStream(dtsfusbStream);
						}
					}
				}
				catch (IOException)
				{
					this.hostLogger.EventWriteClearIdIOException(this.DeviceUniqueID, this.DeviceFriendlyName);
				}
				catch (Win32Exception ex)
				{
					this.hostLogger.EventWriteClearIdWin32Exception(this.DeviceUniqueID, this.DeviceFriendlyName, ex.NativeErrorCode);
				}
				finally
				{
					this.ReleasePathMutex();
				}
			}
			return result;
		}

		// Token: 0x06000158 RID: 344 RVA: 0x00005B10 File Offset: 0x00003D10
		public bool GetDiskInfo(out uint blockSize, out ulong lastBlock)
		{
			bool result = false;
			blockSize = 0U;
			lastBlock = 0UL;
			object obj = this.pathSync;
			lock (obj)
			{
				if (this.fConnected || !this.AcquirePathMutex())
				{
					return false;
				}
				try
				{
					using (this.usbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(1.0)))
					{
						this.ReadDiskInfo(out this.diskTransferSize, out this.diskBlockSize, out this.diskLastBlock);
						result = true;
					}
				}
				catch (IOException)
				{
				}
				catch (Win32Exception)
				{
				}
				finally
				{
					this.ReleasePathMutex();
				}
			}
			blockSize = this.diskBlockSize;
			lastBlock = this.diskLastBlock;
			return result;
		}

		// Token: 0x06000159 RID: 345 RVA: 0x00005C08 File Offset: 0x00003E08
		public void ReadDisk(ulong diskOffset, byte[] buffer, int offset, int count)
		{
			object obj = this.pathSync;
			lock (obj)
			{
				if (this.diskTransferSize <= 0 || this.fConnected || !this.AcquirePathMutex())
				{
					throw new FFUDeviceNotReadyException(this);
				}
				ulong num = (this.diskLastBlock + 1UL) * (ulong)this.diskBlockSize;
				if (count <= 0 || diskOffset >= num || num - diskOffset < (ulong)((long)count))
				{
					throw new FFUDeviceDiskReadException(this, Resources.ERROR_UNABLE_TO_READ_REGION, null);
				}
				try
				{
					using (this.usbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(1.0)))
					{
						this.ReadDataToBuffer(diskOffset, buffer, offset, count);
					}
				}
				catch (IOException)
				{
					throw new FFUDeviceNotReadyException(this);
				}
				catch (Win32Exception e)
				{
					throw new FFUDeviceDiskReadException(this, Resources.ERROR_USB_TRANSFER, e);
				}
				finally
				{
					this.ReleasePathMutex();
				}
			}
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00005D20 File Offset: 0x00003F20
		public void WriteDisk(ulong diskOffset, byte[] buffer, int offset, int count)
		{
			object obj = this.pathSync;
			lock (obj)
			{
				if (this.diskTransferSize <= 0 || this.fConnected || !this.AcquirePathMutex())
				{
					throw new FFUDeviceNotReadyException(this);
				}
				ulong num = (this.diskLastBlock + 1UL) * (ulong)this.diskBlockSize;
				if (count <= 0 || diskOffset >= num || num - diskOffset < (ulong)((long)count))
				{
					throw new FFUDeviceDiskReadException(this, Resources.ERROR_UNABLE_TO_READ_REGION, null);
				}
				try
				{
					using (this.usbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(1.0)))
					{
						this.WriteDataFromBuffer(diskOffset, buffer, offset, count);
					}
				}
				catch (IOException)
				{
					throw new FFUDeviceNotReadyException(this);
				}
				catch (Win32Exception e)
				{
					throw new FFUDeviceDiskWriteException(this, Resources.ERROR_USB_TRANSFER, e);
				}
				finally
				{
					this.ReleasePathMutex();
				}
			}
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00005E38 File Offset: 0x00004038
		public uint SetBootMode(uint bootMode, string profileName)
		{
			uint result = 2147483669U;
			object obj = this.pathSync;
			lock (obj)
			{
				if (this.fConnected || !this.AcquirePathMutex())
				{
					return result;
				}
				try
				{
					using (DTSFUsbStream dtsfusbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(5.0)))
					{
						if (Encoding.Unicode.GetByteCount(profileName) >= 128)
						{
							result = 2147483650U;
							throw new Win32Exception(87);
						}
						byte[] array = new byte[132];
						Array.Clear(array, 0, array.Length);
						BitConverter.GetBytes(bootMode).CopyTo(array, 0);
						Encoding.Unicode.GetBytes(profileName).CopyTo(array, 4);
						dtsfusbStream.WriteByte(19);
						dtsfusbStream.Write(array, 0, array.Length);
						byte[] array2 = new byte[4];
						dtsfusbStream.Read(array2, 0, array2.Length);
						result = BitConverter.ToUInt32(array2, 0);
					}
				}
				catch (IOException)
				{
					this.hostLogger.EventWriteBootModeIOException(this.DeviceUniqueID, this.DeviceFriendlyName);
				}
				catch (Win32Exception ex)
				{
					this.hostLogger.EventWriteBootModeWin32Exception(this.DeviceUniqueID, this.DeviceFriendlyName, ex.NativeErrorCode);
				}
				finally
				{
					this.ReleasePathMutex();
				}
			}
			return result;
		}

		// Token: 0x0600015C RID: 348 RVA: 0x00006000 File Offset: 0x00004200
		public string GetServicingLogs(string logFolderPath)
		{
			string result = null;
			object obj = this.pathSync;
			lock (obj)
			{
				if (this.fConnected || !this.AcquirePathMutex())
				{
					throw new FFUDeviceNotReadyException(this);
				}
				try
				{
					using (this.usbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromMinutes(1.0)))
					{
						if (!this.QueryForCommandAvailable(this.usbStream, SimpleIODevice.SioOpcode.SioGetUpdateLogs))
						{
							throw new FFUDeviceCommandNotAvailableException(this);
						}
						if (string.IsNullOrEmpty(logFolderPath))
						{
							throw new ArgumentNullException("logFolderPath");
						}
						this.usbStream.WriteByte(24);
						byte[] array = new byte[262144];
						int num = 0;
						byte[] array2 = new byte[4];
						int num2 = this.usbStream.Read(array2, 0, array2.Length);
						int num3 = BitConverter.ToInt32(array2, 0);
						string text = LongPath.GetFullPath(logFolderPath);
						LongPathDirectory.CreateDirectory(text);
						text = Path.Combine(text, Path.GetRandomFileName() + ".cab");
						using (FileStream fileStream = LongPathFile.Open(text, FileMode.Create, FileAccess.Write))
						{
							do
							{
								Array.Clear(array, 0, array.Length);
								num2 = this.usbStream.Read(array, 0, array.Length);
								num += num2;
								fileStream.Write(array, 0, array.Length);
							}
							while (num != num3);
							result = text;
						}
					}
				}
				finally
				{
					this.ReleasePathMutex();
				}
			}
			return result;
		}

		// Token: 0x0600015D RID: 349 RVA: 0x000061D0 File Offset: 0x000043D0
		public string GetFlashingLogs(string logFolderPath)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600015E RID: 350 RVA: 0x000061D8 File Offset: 0x000043D8
		public void QueryDeviceUnlockId(out byte[] unlockId, out byte[] oemId, out byte[] platformId)
		{
			unlockId = new byte[32];
			oemId = new byte[16];
			platformId = new byte[16];
			object obj = this.pathSync;
			lock (obj)
			{
				if (this.fConnected || !this.AcquirePathMutex())
				{
					throw new FFUDeviceNotReadyException(this);
				}
				try
				{
					using (DTSFUsbStream dtsfusbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(5.0)))
					{
						BinaryWriter binaryWriter = new BinaryWriter(dtsfusbStream);
						BinaryReader binaryReader = new BinaryReader(dtsfusbStream);
						if (!this.QueryForCommandAvailable(dtsfusbStream, SimpleIODevice.SioOpcode.SioQueryDeviceUnlockId))
						{
							throw new FFUDeviceCommandNotAvailableException(this);
						}
						binaryWriter.Write(25);
						int num = binaryReader.ReadInt32();
						binaryReader.ReadUInt32();
						unlockId = binaryReader.ReadBytes(32);
						oemId = binaryReader.ReadBytes(16);
						platformId = binaryReader.ReadBytes(16);
						if (num != 0)
						{
							throw new FFUDeviceRetailUnlockException(this, num);
						}
					}
				}
				catch (IOException)
				{
					throw new FFUDeviceNotReadyException(this);
				}
				catch (Win32Exception e)
				{
					throw new FFUDeviceRetailUnlockException(this, Resources.ERROR_USB_TRANSFER, e);
				}
				finally
				{
					this.ReleasePathMutex();
				}
			}
		}

		// Token: 0x0600015F RID: 351 RVA: 0x0000631C File Offset: 0x0000451C
		public void RelockDeviceUnlockId()
		{
			object obj = this.pathSync;
			lock (obj)
			{
				if (this.fConnected || !this.AcquirePathMutex())
				{
					throw new FFUDeviceNotReadyException(this);
				}
				try
				{
					using (DTSFUsbStream dtsfusbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(5.0)))
					{
						BinaryWriter binaryWriter = new BinaryWriter(dtsfusbStream);
						BinaryReader binaryReader = new BinaryReader(dtsfusbStream);
						if (!this.QueryForCommandAvailable(dtsfusbStream, SimpleIODevice.SioOpcode.SioRelockDeviceUnlockId))
						{
							throw new FFUDeviceCommandNotAvailableException(this);
						}
						binaryWriter.Write(26);
						int num = binaryReader.ReadInt32();
						if (num != 0)
						{
							throw new FFUDeviceRetailUnlockException(this, num);
						}
					}
				}
				catch (IOException)
				{
					throw new FFUDeviceNotReadyException(this);
				}
				catch (Win32Exception e)
				{
					throw new FFUDeviceRetailUnlockException(this, Resources.ERROR_USB_TRANSFER, e);
				}
				finally
				{
					this.ReleasePathMutex();
				}
			}
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00006420 File Offset: 0x00004620
		public uint[] QueryUnlockTokenFiles()
		{
			new byte[16];
			List<uint> list = new List<uint>();
			object obj = this.pathSync;
			lock (obj)
			{
				if (this.fConnected || !this.AcquirePathMutex())
				{
					throw new FFUDeviceNotReadyException(this);
				}
				try
				{
					using (DTSFUsbStream dtsfusbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(5.0)))
					{
						BinaryWriter binaryWriter = new BinaryWriter(dtsfusbStream);
						BinaryReader binaryReader = new BinaryReader(dtsfusbStream);
						if (!this.QueryForCommandAvailable(dtsfusbStream, SimpleIODevice.SioOpcode.SioQueryUnlockTokenFiles))
						{
							throw new FFUDeviceCommandNotAvailableException(this);
						}
						binaryWriter.Write(27);
						int num = binaryReader.ReadInt32();
						binaryReader.ReadUInt32();
						BitArray bitArray = new BitArray(binaryReader.ReadBytes(16));
						uint num2 = 0U;
						while ((ulong)num2 < (ulong)((long)bitArray.Count))
						{
							if (bitArray.Get(Convert.ToInt32(num2)))
							{
								list.Add(num2);
							}
							num2 += 1U;
						}
						if (num != 0)
						{
							throw new FFUDeviceRetailUnlockException(this, num);
						}
					}
				}
				catch (IOException)
				{
					throw new FFUDeviceNotReadyException(this);
				}
				catch (Win32Exception e)
				{
					throw new FFUDeviceRetailUnlockException(this, Resources.ERROR_USB_TRANSFER, e);
				}
				finally
				{
					this.ReleasePathMutex();
				}
			}
			return list.ToArray();
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00006580 File Offset: 0x00004780
		public void WriteUnlockTokenFile(uint unlockTokenId, byte[] fileData)
		{
			uint value = 0U;
			uint value2 = (uint)fileData.Length;
			if (1048576 < fileData.Length)
			{
				throw new ArgumentException("fileData");
			}
			if (127U < unlockTokenId)
			{
				throw new ArgumentException("unlockTokenId");
			}
			object obj = this.pathSync;
			lock (obj)
			{
				if (this.fConnected || !this.AcquirePathMutex())
				{
					throw new FFUDeviceNotReadyException(this);
				}
				try
				{
					using (DTSFUsbStream dtsfusbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(5.0)))
					{
						BinaryWriter binaryWriter = new BinaryWriter(dtsfusbStream);
						BinaryReader binaryReader = new BinaryReader(dtsfusbStream);
						if (!this.QueryForCommandAvailable(dtsfusbStream, SimpleIODevice.SioOpcode.SioWriteUnlockTokenFile))
						{
							throw new FFUDeviceCommandNotAvailableException(this);
						}
						binaryWriter.Write(28);
						binaryWriter.Write(value);
						binaryWriter.Write(value2);
						binaryWriter.Write(unlockTokenId);
						binaryWriter.Write(fileData);
						int num = binaryReader.ReadInt32();
						if (num != 0)
						{
							throw new FFUDeviceRetailUnlockException(this, num);
						}
					}
				}
				catch (IOException)
				{
					throw new FFUDeviceNotReadyException(this);
				}
				catch (Win32Exception e)
				{
					throw new FFUDeviceRetailUnlockException(this, Resources.ERROR_USB_TRANSFER, e);
				}
				finally
				{
					this.ReleasePathMutex();
				}
			}
		}

		// Token: 0x06000162 RID: 354 RVA: 0x000066D8 File Offset: 0x000048D8
		public bool QueryBitlockerState()
		{
			bool result = false;
			object obj = this.pathSync;
			lock (obj)
			{
				if (this.fConnected || !this.AcquirePathMutex())
				{
					throw new FFUDeviceNotReadyException(this);
				}
				try
				{
					using (DTSFUsbStream dtsfusbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(5.0)))
					{
						BinaryWriter binaryWriter = new BinaryWriter(dtsfusbStream);
						BinaryReader binaryReader = new BinaryReader(dtsfusbStream);
						if (!this.QueryForCommandAvailable(dtsfusbStream, SimpleIODevice.SioOpcode.SioQueryBitlockerState))
						{
							throw new FFUDeviceCommandNotAvailableException(this);
						}
						binaryWriter.Write(29);
						int num = binaryReader.ReadInt32();
						result = (binaryReader.ReadByte() != 0);
						if (num != 0)
						{
							throw new FFUDeviceRetailUnlockException(this, num);
						}
					}
				}
				catch (IOException)
				{
					throw new FFUDeviceNotReadyException(this);
				}
				catch (Win32Exception e)
				{
					throw new FFUDeviceRetailUnlockException(this, Resources.ERROR_USB_TRANSFER, e);
				}
				finally
				{
					this.ReleasePathMutex();
				}
			}
			return result;
		}

		// Token: 0x06000163 RID: 355 RVA: 0x000061D0 File Offset: 0x000043D0
		public void Unlock(uint tokenId, string tokenFilePath, string pin)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000164 RID: 356 RVA: 0x000061D0 File Offset: 0x000043D0
		public byte[] GetDeviceProperties()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000165 RID: 357 RVA: 0x000067F0 File Offset: 0x000049F0
		public bool OnConnect(SimpleIODevice device)
		{
			if (device != null && device.UsbDevicePath != this.UsbDevicePath)
			{
				this.UsbDevicePath = device.UsbDevicePath;
			}
			if (this.fConnected)
			{
				this.connectEvent.Set();
				return true;
			}
			return this.ReadBootme();
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00006842 File Offset: 0x00004A42
		public bool IsConnected()
		{
			return this.fConnected || this.ReadBootme();
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00006856 File Offset: 0x00004A56
		public bool NeedsTimer()
		{
			return !this.fOperationStarted;
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00006863 File Offset: 0x00004A63
		public bool OnDisconnect()
		{
			return false;
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00006868 File Offset: 0x00004A68
		private bool AcquirePathMutex()
		{
			TimeoutHelper timeoutHelper = new TimeoutHelper(TimeSpan.FromMinutes(2.0));
			TimeSpan remaining = timeoutHelper.Remaining;
			if (remaining <= TimeSpan.Zero)
			{
				this.hostLogger.EventWriteMutexTimeout(this.DeviceUniqueID, this.DeviceFriendlyName);
				return false;
			}
			bool result;
			try
			{
				if (!this.syncMutex.WaitOne(remaining, false))
				{
					this.hostLogger.EventWriteMutexTimeout(this.DeviceUniqueID, this.DeviceFriendlyName);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			catch (AbandonedMutexException)
			{
				this.hostLogger.EventWriteWaitAbandoned(this.DeviceUniqueID, this.DeviceFriendlyName);
				result = true;
			}
			return result;
		}

		// Token: 0x0600016A RID: 362 RVA: 0x00006918 File Offset: 0x00004B18
		private void ReleasePathMutex()
		{
			this.syncMutex.ReleaseMutex();
		}

		// Token: 0x0600016B RID: 363 RVA: 0x00006928 File Offset: 0x00004B28
		private void InitFlashingStream()
		{
			bool flag = false;
			this.InitFlashingStream(false, out flag);
		}

		// Token: 0x0600016C RID: 364 RVA: 0x00006940 File Offset: 0x00004B40
		private void InitFlashingStream(bool optimizeHint, out bool useOptimize)
		{
			bool flag = false;
			bool flag2 = false;
			useOptimize = false;
			object obj = this.pathSync;
			lock (obj)
			{
				if (!this.AcquirePathMutex())
				{
					throw new FFUException(this.DeviceFriendlyName, this.DeviceUniqueID, Resources.ERROR_ACQUIRE_MUTEX);
				}
				try
				{
					if (this.usbStream != null)
					{
						this.usbStream.Dispose();
					}
					this.usbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromMinutes(1.0));
					if (optimizeHint)
					{
						int num = 0;
						do
						{
							this.ReadBootmeFromStream(this.usbStream);
							num++;
						}
						while (!this.supportsFastFlash && !this.supportsCompatFastFlash && num < 1000);
						flag2 = (this.supportsFastFlash || this.supportsCompatFastFlash);
					}
					if (!flag2)
					{
						this.usbStream.WriteByte(2);
					}
					else if (this.supportsFastFlash)
					{
						this.usbStream.WriteByte(20);
						this.InitFastFlash();
						useOptimize = true;
					}
					else if (this.supportsCompatFastFlash)
					{
						this.usbStream.WriteByte(20);
						this.usbTransactionSize = 8388600;
						this.packets.PacketDataSize = 8388608L;
						useOptimize = true;
					}
				}
				catch (IOException)
				{
					flag = true;
				}
				catch (Win32Exception ex)
				{
					flag = true;
					if (ex.NativeErrorCode == 31)
					{
						this.forceClearOnReconnect = false;
					}
				}
				finally
				{
					this.ReleasePathMutex();
				}
			}
			if (flag)
			{
				this.WaitForReconnect();
			}
		}

		// Token: 0x0600016D RID: 365 RVA: 0x00006AF8 File Offset: 0x00004CF8
		private void InitFastFlash()
		{
			byte[] array = new byte[this.usbTransactionSize];
			this.usbStream.Read(array, 0, array.Length);
			SimpleIODevice.SioOpcode sioOpcode = (SimpleIODevice.SioOpcode)array[0];
			if (sioOpcode != SimpleIODevice.SioOpcode.SioDeviceParams)
			{
				if (sioOpcode == SimpleIODevice.SioOpcode.SioErr)
				{
					this.hostLogger.EventWriteFlash_Error(this.DeviceUniqueID, this.DeviceFriendlyName);
					throw new FFUFlashException(this.DeviceFriendlyName, this.DeviceUniqueID, (FFUFlashException.ErrorCode)this.errId, string.Format(CultureInfo.CurrentCulture, Resources.ERROR_FLASH, new object[]
					{
						this.errInfo
					}));
				}
				throw new FFUFlashException();
			}
			else
			{
				BinaryReader binaryReader = new BinaryReader(new MemoryStream(array));
				binaryReader.ReadByte();
				if (binaryReader.ReadUInt32() != 13U)
				{
					throw new FFUFlashException(Resources.ERROR_INVALID_DEVICE_PARAMS);
				}
				uint num = binaryReader.ReadUInt32();
				if (num < 16376U)
				{
					throw new FFUFlashException(Resources.ERROR_INVALID_DEVICE_PARAMS);
				}
				uint num2 = binaryReader.ReadUInt32();
				if ((ulong)num2 < (ulong)PacketConstructor.DefaultPacketDataSize || (ulong)num2 > (ulong)PacketConstructor.MaxPacketDataSize || (ulong)num2 % (ulong)PacketConstructor.DefaultPacketDataSize != 0UL)
				{
					throw new FFUFlashException(Resources.ERROR_INVALID_DEVICE_PARAMS);
				}
				this.usbTransactionSize = (int)num;
				this.packets.PacketDataSize = (long)((ulong)num2);
				return;
			}
		}

		// Token: 0x0600016E RID: 366 RVA: 0x00006C05 File Offset: 0x00004E05
		private Stream GetBufferedFileStream(string path)
		{
			return new BufferedStream(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read), 5242880);
		}

		// Token: 0x0600016F RID: 367 RVA: 0x00006C1A File Offset: 0x00004E1A
		private Stream GetStringStream(string src)
		{
			MemoryStream memoryStream = new MemoryStream();
			new BinaryWriter(memoryStream, Encoding.BigEndianUnicode).Write(src);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return memoryStream;
		}

		// Token: 0x06000170 RID: 368 RVA: 0x00006C3C File Offset: 0x00004E3C
		private void WriteCallback(IAsyncResult ar)
		{
			(ar.AsyncState as AutoResetEvent).Set();
		}

		// Token: 0x06000171 RID: 369 RVA: 0x00006C50 File Offset: 0x00004E50
		private void SendPacket(byte[] packet, bool optimize)
		{
			bool flag = false;
			WaitHandle[] waitHandles = new WaitHandle[]
			{
				this.writeEvent,
				this.errorEvent
			};
			while (!flag)
			{
				try
				{
					for (int i = 0; i < packet.Length; i += this.usbTransactionSize)
					{
						if (optimize)
						{
							this.usbStream.BeginWrite(packet, i, Math.Min(this.usbTransactionSize, packet.Length - i), new AsyncCallback(this.WriteCallback), this.writeEvent);
							if (WaitHandle.WaitAny(waitHandles) == 1)
							{
								if (this.usbStream != null)
								{
									this.usbStream.Dispose();
									this.usbStream = null;
									this.fConnected = false;
								}
								this.hostLogger.EventWriteFlash_Error(this.DeviceUniqueID, this.DeviceFriendlyName);
								throw new FFUFlashException(this.DeviceFriendlyName, this.DeviceUniqueID, (FFUFlashException.ErrorCode)this.errId, string.Format(CultureInfo.CurrentCulture, Resources.ERROR_FLASH, new object[]
								{
									this.errInfo
								}));
							}
						}
						else
						{
							this.usbStream.Write(packet, i, Math.Min(this.usbTransactionSize, packet.Length - i));
						}
					}
					flag = (optimize || this.WaitForAck());
				}
				catch (Win32Exception ex)
				{
					this.hostLogger.EventWriteTransferException(this.DeviceUniqueID, this.DeviceFriendlyName, ex.NativeErrorCode);
					long position = this.packets.Position;
					this.WaitForReconnect();
					if (position != this.packets.Position)
					{
						break;
					}
				}
			}
		}

		// Token: 0x06000172 RID: 370 RVA: 0x00006DD0 File Offset: 0x00004FD0
		private bool WaitForAck()
		{
			for (;;)
			{
				byte[] array = new byte[this.usbTransactionSize];
				this.usbStream.Read(array, 0, array.Length);
				switch (array[0])
				{
				case 3:
					return true;
				case 5:
				{
					BinaryReader binaryReader = new BinaryReader(new MemoryStream(array));
					binaryReader.ReadByte();
					this.errId = (int)binaryReader.ReadInt16();
					this.deviceLogger.LogDeviceEvent(array, this.DeviceUniqueID, this.DeviceFriendlyName, out this.errInfo);
					if (string.IsNullOrEmpty(this.errInfo))
					{
						this.errId = 0;
						continue;
					}
					continue;
				}
				case 6:
					goto IL_9A;
				}
				break;
			}
			return false;
			IL_9A:
			this.usbStream.Dispose();
			this.usbStream = null;
			this.fConnected = false;
			this.hostLogger.EventWriteFlash_Error(this.DeviceUniqueID, this.DeviceFriendlyName);
			throw new FFUFlashException(this.DeviceFriendlyName, this.DeviceUniqueID, (FFUFlashException.ErrorCode)this.errId, string.Format(CultureInfo.CurrentCulture, Resources.ERROR_FLASH, new object[]
			{
				this.errInfo
			}));
		}

		// Token: 0x06000173 RID: 371 RVA: 0x00006EE1 File Offset: 0x000050E1
		private bool WaitForEndResponse(bool optimize)
		{
			return optimize || this.WaitForAck();
		}

		// Token: 0x06000174 RID: 372 RVA: 0x00006EF0 File Offset: 0x000050F0
		private bool WriteSkip(DTSFUsbStream skipStream)
		{
			skipStream.WriteByte(7);
			int num = skipStream.ReadByte();
			if (num == 3)
			{
				return true;
			}
			this.hostLogger.EventWriteWriteSkipFailed(this.DeviceUniqueID, this.DeviceFriendlyName, num);
			return false;
		}

		// Token: 0x06000175 RID: 373 RVA: 0x00006F2C File Offset: 0x0000512C
		private void WaitForReconnect()
		{
			this.hostLogger.EventWriteDevice_Detach(this.DeviceUniqueID, this.DeviceFriendlyName);
			if (!this.DoWaitForDevice())
			{
				this.hostLogger.EventWriteFlash_Timeout(this.DeviceUniqueID, this.DeviceFriendlyName);
				throw new FFUException(this.DeviceFriendlyName, this.DeviceUniqueID, Resources.ERROR_RECONNECT_TIMEOUT);
			}
			if (this.curPosition == 0L && this.resetCount < 3)
			{
				this.packets.Reset();
				this.resetCount++;
			}
			if (this.packets.Position - this.curPosition > this.packets.PacketDataSize)
			{
				throw new FFUException(this.DeviceFriendlyName, this.DeviceUniqueID, string.Format(CultureInfo.CurrentCulture, Resources.ERROR_RESUME_UNEXPECTED_POSITION, new object[]
				{
					this.packets.Position,
					this.curPosition
				}));
			}
			this.usbStream.WriteByte(2);
			this.hostLogger.EventWriteDevice_Attach(this.DeviceUniqueID, this.DeviceFriendlyName);
		}

		// Token: 0x06000176 RID: 374 RVA: 0x00007040 File Offset: 0x00005240
		private bool DoWaitForDevice()
		{
			bool result = false;
			if (this.usbStream != null)
			{
				this.usbStream.Dispose();
				this.usbStream = null;
			}
			this.connectEvent.WaitOne(30000, false);
			object obj = this.pathSync;
			lock (obj)
			{
				if (!this.AcquirePathMutex())
				{
					throw new FFUException(this.DeviceFriendlyName, this.DeviceUniqueID, Resources.ERROR_ACQUIRE_MUTEX);
				}
				try
				{
					bool flag2 = this.forceClearOnReconnect;
					this.forceClearOnReconnect = true;
					if (flag2)
					{
						using (DTSFUsbStream dtsfusbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromMilliseconds(100.0)))
						{
							this.ClearJunkDataFromStream(dtsfusbStream);
						}
					}
					this.usbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromMinutes(1.0));
					this.ReadBootmeFromStream(this.usbStream);
					result = true;
				}
				catch (IOException)
				{
					this.hostLogger.EventWriteReconnectIOException(this.DeviceUniqueID, this.DeviceFriendlyName);
				}
				catch (Win32Exception ex)
				{
					this.hostLogger.EventWriteReconnectWin32Exception(this.DeviceUniqueID, this.DeviceFriendlyName, ex.NativeErrorCode);
				}
				finally
				{
					this.ReleasePathMutex();
				}
			}
			return result;
		}

		// Token: 0x06000177 RID: 375 RVA: 0x000071A8 File Offset: 0x000053A8
		private void ClearJunkDataFromStream(DTSFUsbStream clearStream)
		{
			this.hostLogger.EventWriteStreamClearStart(this.DeviceUniqueID, this.DeviceFriendlyName);
			try
			{
				clearStream.PipeReset();
				for (int i = 0; i < 3; i++)
				{
					byte[] array = new byte[this.usbTransactionSize];
					for (int j = 0; j < 17; j++)
					{
						try
						{
							clearStream.Write(array, 0, array.Length);
						}
						catch (Win32Exception ex)
						{
							this.hostLogger.EventWriteStreamClearPushWin32Exception(this.DeviceUniqueID, this.DeviceFriendlyName, ex.ErrorCode);
						}
					}
					for (int k = 0; k < 5; k++)
					{
						try
						{
							clearStream.Read(array, 0, array.Length);
						}
						catch (Win32Exception ex2)
						{
							this.hostLogger.EventWriteStreamClearPullWin32Exception(this.DeviceUniqueID, this.DeviceFriendlyName, ex2.ErrorCode);
						}
					}
				}
				clearStream.PipeReset();
			}
			catch (IOException)
			{
				this.hostLogger.EventWriteStreamClearIOException(this.DeviceUniqueID, this.DeviceFriendlyName);
				this.connectEvent.WaitOne(5000, false);
			}
			Thread.Sleep(TimeSpan.FromSeconds(1.0));
			this.hostLogger.EventWriteStreamClearStop(this.DeviceUniqueID, this.DeviceFriendlyName);
		}

		// Token: 0x06000178 RID: 376 RVA: 0x000072F8 File Offset: 0x000054F8
		private string GetPnPIdFromDevicePath(string path)
		{
			string text = path.Replace('#', '\\').Substring(4);
			return text.Remove(text.IndexOf('\\', 22));
		}

		// Token: 0x06000179 RID: 377 RVA: 0x0000731C File Offset: 0x0000551C
		public void ErrorCallback(IAsyncResult ar)
		{
			this.usbStream.EndRead(ar);
			DTSFUsbStreamReadAsyncResult dtsfusbStreamReadAsyncResult = (DTSFUsbStreamReadAsyncResult)ar;
			BinaryReader binaryReader = new BinaryReader(new MemoryStream(dtsfusbStreamReadAsyncResult.Buffer));
			binaryReader.ReadByte();
			this.errId = (int)binaryReader.ReadInt16();
			this.deviceLogger.LogDeviceEvent(dtsfusbStreamReadAsyncResult.Buffer, this.DeviceUniqueID, this.DeviceFriendlyName, out this.errInfo);
			if (string.IsNullOrEmpty(this.errInfo))
			{
				this.errId = 0;
			}
			(ar.AsyncState as ManualResetEvent).Set();
		}

		// Token: 0x0600017A RID: 378 RVA: 0x000073AC File Offset: 0x000055AC
		private void TransferPackets(bool optimize)
		{
			while (this.packets.RemainingData > 0L)
			{
				this.hostLogger.EventWriteFileRead_Start(this.DeviceUniqueID, this.DeviceFriendlyName);
				byte[] packet = this.packets.GetNextPacket(optimize);
				this.hostLogger.EventWriteFileRead_Stop(this.DeviceUniqueID, this.DeviceFriendlyName);
				this.SendPacket(packet, optimize);
				if (this.ProgressEvent != null && (this.packets.Position - this.lastProgress > 1048576L || this.packets.Position == this.packets.Length))
				{
					this.lastProgress = this.packets.Position;
					ProgressEventArgs args = new ProgressEventArgs(this, this.packets.Position, this.packets.Length);
					Task.Factory.StartNew(delegate()
					{
						this.ProgressEvent(this, args);
					});
				}
			}
			if (this.packets.Length % this.packets.PacketDataSize == 0L)
			{
				byte[] packet = this.packets.GetZeroLengthPacket();
				this.SendPacket(packet, optimize);
			}
		}

		// Token: 0x0600017B RID: 379 RVA: 0x000074D8 File Offset: 0x000056D8
		private bool HasWimHeader(Stream wimStream)
		{
			byte[] array = new byte[]
			{
				77,
				83,
				87,
				73,
				77,
				0,
				0,
				0
			};
			byte[] array2 = new byte[array.Length];
			long position = wimStream.Position;
			wimStream.Read(array2, 0, array2.Length);
			wimStream.Position = position;
			return array.SequenceEqual(array2);
		}

		// Token: 0x0600017C RID: 380 RVA: 0x00007520 File Offset: 0x00005720
		private void WriteWim(Stream sdiStream, Stream wimStream)
		{
			int num = 1048576;
			if (this.DeviceFriendlyName.Contains("Nokia.MSM8960.P4301"))
			{
				num = 16376;
			}
			bool flag = this.HasWimHeader(wimStream);
			byte[] array = new byte[12];
			uint value = 0U;
			if (flag)
			{
				value = (uint)sdiStream.Length;
			}
			BitConverter.GetBytes(value).CopyTo(array, 0);
			BitConverter.GetBytes((uint)wimStream.Length).CopyTo(array, 4);
			BitConverter.GetBytes(num).CopyTo(array, 8);
			byte[] buffer = new byte[num];
			Stream[] array2;
			if (flag)
			{
				array2 = new Stream[]
				{
					sdiStream,
					wimStream
				};
			}
			else
			{
				array2 = new Stream[]
				{
					wimStream
				};
			}
			this.usbStream.WriteByte(16);
			this.usbStream.Write(array, 0, array.Length);
			foreach (Stream stream in array2)
			{
				this.hostLogger.EventWriteWimTransferStart(this.DeviceUniqueID, this.DeviceFriendlyName);
				while (stream.Position < stream.Length)
				{
					int num2 = stream.Read(buffer, 0, num);
					this.hostLogger.EventWriteWimPacketStart(this.DeviceUniqueID, this.DeviceFriendlyName, num2);
					this.usbStream.Write(buffer, 0, num2);
					this.hostLogger.EventWriteWimPacketStop(this.DeviceUniqueID, this.DeviceFriendlyName, 0);
				}
				this.hostLogger.EventWriteWimTransferStop(this.DeviceUniqueID, this.DeviceFriendlyName);
			}
		}

		// Token: 0x0600017D RID: 381 RVA: 0x0000768C File Offset: 0x0000588C
		private bool ReadStatus()
		{
			this.hostLogger.EventWriteWimGetStatus(this.DeviceUniqueID, this.DeviceFriendlyName);
			byte[] array = new byte[4];
			this.usbStream.Read(array, 0, array.Length);
			int num = BitConverter.ToInt32(array, 0);
			bool flag = num >= 0;
			if (flag)
			{
				this.hostLogger.EventWriteWimSuccess(this.DeviceUniqueID, this.DeviceFriendlyName, num);
				return flag;
			}
			this.hostLogger.EventWriteWimError(this.DeviceUniqueID, this.DeviceFriendlyName, num);
			throw new FFUException(this.DeviceFriendlyName, this.DeviceUniqueID, string.Format(CultureInfo.CurrentCulture, Resources.ERROR_WIMBOOT, new object[]
			{
				num
			}));
		}

		// Token: 0x0600017E RID: 382 RVA: 0x00007744 File Offset: 0x00005944
		private void ReadDiskInfo(out int transferSize, out uint blockSize, out ulong lastBlock)
		{
			this.usbStream.WriteByte(12);
			byte[] array = new byte[16];
			this.usbStream.Read(array, 0, array.Length);
			int num = 0;
			transferSize = BitConverter.ToInt32(array, num);
			num += 4;
			blockSize = BitConverter.ToUInt32(array, num);
			num += 4;
			lastBlock = BitConverter.ToUInt64(array, num);
			num += 8;
		}

		// Token: 0x0600017F RID: 383 RVA: 0x000077A0 File Offset: 0x000059A0
		private bool NeedsToHandleZLP()
		{
			foreach (string pattern in new string[]
			{
				".*\\.MSM8960\\.*"
			})
			{
				if (Regex.IsMatch(this.DeviceFriendlyName, pattern))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000180 RID: 384 RVA: 0x000077E0 File Offset: 0x000059E0
		private void ReadDataToBuffer(ulong diskOffset, byte[] buffer, int offset, int count)
		{
			ulong value = (ulong)((long)count);
			this.usbStream.WriteByte(13);
			byte[] array = new byte[16];
			BitConverter.GetBytes(diskOffset).CopyTo(array, 0);
			BitConverter.GetBytes(value).CopyTo(array, 8);
			this.usbStream.Write(array, 0, array.Length);
			int i = offset;
			int num = offset + count;
			while (i < num)
			{
				int num2 = this.diskTransferSize;
				if (num2 > num - i)
				{
					num2 = num - i;
				}
				this.usbStream.Read(buffer, i, num2);
				if (num2 % 512 == 0 && this.NeedsToHandleZLP())
				{
					this.usbStream.ReadByte();
				}
				i += num2;
			}
		}

		// Token: 0x06000181 RID: 385 RVA: 0x0000787C File Offset: 0x00005A7C
		private void WriteDataFromBuffer(ulong diskOffset, byte[] buffer, int offset, int count)
		{
			ulong value = (ulong)((long)count);
			this.usbStream.WriteByte(14);
			byte[] array = new byte[16];
			BitConverter.GetBytes(diskOffset).CopyTo(array, 0);
			BitConverter.GetBytes(value).CopyTo(array, 8);
			this.usbStream.Write(array, 0, array.Length);
			int i = offset;
			int num = offset + count;
			while (i < num)
			{
				int num2 = this.diskTransferSize;
				if (num2 > num - i)
				{
					num2 = num - i;
				}
				this.usbStream.Write(buffer, i, num2);
				if (num2 % 512 == 0)
				{
					byte[] array2 = new byte[0];
					this.usbStream.Write(array2, 0, array2.Length);
				}
				i += num2;
			}
			byte[] array3 = new byte[8];
			this.usbStream.Read(array3, 0, array3.Length);
			if ((long)count != (long)BitConverter.ToUInt64(array3, 0))
			{
				throw new FFUDeviceDiskWriteException(this, Resources.ERROR_UNABLE_TO_COMPLETE_WRITE, null);
			}
		}

		// Token: 0x06000182 RID: 386 RVA: 0x00007954 File Offset: 0x00005B54
		private bool QueryForCommandAvailable(DTSFUsbStream idStream, SimpleIODevice.SioOpcode Cmd)
		{
			if (!this.hasCheckedForV2)
			{
				int num = 0;
				do
				{
					this.ReadBootmeFromStream(idStream);
					num++;
				}
				while (!this.supportsFastFlash && !this.supportsCompatFastFlash && this.clientVersion < 2 && num < 1000);
				this.hasCheckedForV2 = true;
			}
			if (this.clientVersion < 2)
			{
				return Cmd < SimpleIODevice.SioOpcode.SioFastFlash || this.supportsFastFlash;
			}
			idStream.WriteByte(23);
			idStream.WriteByte((byte)Cmd);
			return idStream.ReadByte() != 0;
		}

		// Token: 0x06000183 RID: 387 RVA: 0x000079D4 File Offset: 0x00005BD4
		private unsafe void ReadBootmeFromStream(DTSFUsbStream idStream)
		{
			idStream.WriteByte(1);
			BinaryReader binaryReader = new BinaryReader(idStream);
			this.curPosition = binaryReader.ReadInt64();
			Guid guid = new Guid(binaryReader.ReadBytes(sizeof(Guid)));
			byte* ptr = (byte*)(&guid);
			if (*ptr >= 1)
			{
				this.supportsFastFlash = true;
			}
			else if (ptr[15] == 1)
			{
				this.supportsCompatFastFlash = true;
			}
			if (ptr[14] >= 1)
			{
				this.clientVersion = (int)(ptr[14] + 1);
			}
			this.DeviceUniqueID = new Guid(binaryReader.ReadBytes(sizeof(Guid)));
			this.DeviceFriendlyName = binaryReader.ReadString();
		}

		// Token: 0x06000184 RID: 388 RVA: 0x00007A68 File Offset: 0x00005C68
		private bool ReadBootme()
		{
			bool result = false;
			for (int i = 0; i < 3; i++)
			{
				object obj = this.pathSync;
				lock (obj)
				{
					if (this.syncMutex == null || !this.AcquirePathMutex())
					{
						return false;
					}
					try
					{
						if (i > 0)
						{
							using (DTSFUsbStream dtsfusbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromMilliseconds(100.0)))
							{
								this.ClearJunkDataFromStream(dtsfusbStream);
							}
						}
						using (DTSFUsbStream dtsfusbStream2 = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(2.0)))
						{
							this.ReadBootmeFromStream(dtsfusbStream2);
							result = true;
							break;
						}
					}
					catch (IOException)
					{
						this.hostLogger.EventWriteReadBootmeIOException(this.DeviceUniqueID, this.DeviceFriendlyName);
					}
					catch (Win32Exception ex)
					{
						this.hostLogger.EventWriteReadBootmeWin32Exception(this.DeviceUniqueID, this.DeviceFriendlyName, ex.NativeErrorCode);
					}
					finally
					{
						this.ReleasePathMutex();
					}
				}
			}
			return result;
		}

		// Token: 0x06000185 RID: 389 RVA: 0x00007BBC File Offset: 0x00005DBC
		private Guid GetSerialNumberFromDevice()
		{
			Guid result = Guid.Empty;
			object obj = this.pathSync;
			lock (obj)
			{
				if (this.syncMutex == null || !this.AcquirePathMutex())
				{
					return result;
				}
				try
				{
					using (DTSFUsbStream dtsfusbStream = new DTSFUsbStream(this.UsbDevicePath, TimeSpan.FromSeconds(1.0)))
					{
						byte[] array = new byte[16];
						dtsfusbStream.WriteByte(17);
						dtsfusbStream.Read(array, 0, array.Length);
						result = new Guid(array);
					}
				}
				catch (IOException)
				{
				}
				catch (Win32Exception)
				{
				}
				finally
				{
					this.ReleasePathMutex();
				}
			}
			return result;
		}

		// Token: 0x040000E7 RID: 231
		private const int DefaultUSBTransactionSize = 16376;

		// Token: 0x040000E8 RID: 232
		private const int DefaultWIMTransactionSize = 1048576;

		// Token: 0x040000E9 RID: 233
		private const int MaxResets = 3;

		// Token: 0x040000EA RID: 234
		private const int ManufacturingProfileNameSizeInBytes = 128;

		// Token: 0x040000EB RID: 235
		private const int COMPATFLASH_MagicSequence = 1000;

		// Token: 0x040000EC RID: 236
		private const byte INDEX_SUPPORTCOMPATFLASH = 15;

		// Token: 0x040000ED RID: 237
		private const byte INDEX_SUPPORTV2CMDS = 14;

		// Token: 0x040000EE RID: 238
		private const byte INDEX_SUPPORTFASTFLASH = 0;

		// Token: 0x040000EF RID: 239
		private volatile bool fConnected;

		// Token: 0x040000F0 RID: 240
		private volatile bool fOperationStarted;

		// Token: 0x040000F1 RID: 241
		private DTSFUsbStream usbStream;

		// Token: 0x040000F2 RID: 242
		private MemoryStream memStm;

		// Token: 0x040000F3 RID: 243
		private AutoResetEvent connectEvent;

		// Token: 0x040000F4 RID: 244
		private PacketConstructor packets;

		// Token: 0x040000F5 RID: 245
		private FlashingHostLogger hostLogger;

		// Token: 0x040000F6 RID: 246
		private FlashingDeviceLogger deviceLogger;

		// Token: 0x040000F7 RID: 247
		private long curPosition;

		// Token: 0x040000F8 RID: 248
		private Mutex syncMutex;

		// Token: 0x040000F9 RID: 249
		private string usbDevicePath;

		// Token: 0x040000FA RID: 250
		private object pathSync;

		// Token: 0x040000FB RID: 251
		private int errId;

		// Token: 0x040000FC RID: 252
		private string errInfo;

		// Token: 0x040000FD RID: 253
		private int resetCount;

		// Token: 0x040000FE RID: 254
		private int diskTransferSize;

		// Token: 0x040000FF RID: 255
		private uint diskBlockSize;

		// Token: 0x04000100 RID: 256
		private ulong diskLastBlock;

		// Token: 0x04000101 RID: 257
		private long lastProgress;

		// Token: 0x04000102 RID: 258
		private bool forceClearOnReconnect;

		// Token: 0x04000103 RID: 259
		private Guid serialNumber;

		// Token: 0x04000104 RID: 260
		private bool serialNumberChecked;

		// Token: 0x04000105 RID: 261
		private int usbTransactionSize;

		// Token: 0x04000106 RID: 262
		private bool supportsFastFlash;

		// Token: 0x04000107 RID: 263
		private bool supportsCompatFastFlash;

		// Token: 0x04000108 RID: 264
		private bool hasCheckedForV2;

		// Token: 0x04000109 RID: 265
		private int clientVersion;

		// Token: 0x0400010A RID: 266
		private FlashingTelemetryLogger telemetryLogger;

		// Token: 0x0400010B RID: 267
		private ManualResetEvent errorEvent;

		// Token: 0x0400010C RID: 268
		private AutoResetEvent writeEvent;

		// Token: 0x02000053 RID: 83
		private enum SioOpcode : byte
		{
			// Token: 0x0400019F RID: 415
			SioId = 1,
			// Token: 0x040001A0 RID: 416
			SioFlash,
			// Token: 0x040001A1 RID: 417
			SioAck,
			// Token: 0x040001A2 RID: 418
			SioNack,
			// Token: 0x040001A3 RID: 419
			SioLog,
			// Token: 0x040001A4 RID: 420
			SioErr,
			// Token: 0x040001A5 RID: 421
			SioSkip,
			// Token: 0x040001A6 RID: 422
			SioReset,
			// Token: 0x040001A7 RID: 423
			SioFile,
			// Token: 0x040001A8 RID: 424
			SioReboot,
			// Token: 0x040001A9 RID: 425
			SioMassStorage,
			// Token: 0x040001AA RID: 426
			SioGetDiskInfo,
			// Token: 0x040001AB RID: 427
			SioReadDisk,
			// Token: 0x040001AC RID: 428
			SioWriteDisk,
			// Token: 0x040001AD RID: 429
			SioClearIdOverride,
			// Token: 0x040001AE RID: 430
			SioWim,
			// Token: 0x040001AF RID: 431
			SioSerialNumber,
			// Token: 0x040001B0 RID: 432
			SioExternalWim,
			// Token: 0x040001B1 RID: 433
			SioSetBootMode,
			// Token: 0x040001B2 RID: 434
			SioFastFlash,
			// Token: 0x040001B3 RID: 435
			SioDeviceParams,
			// Token: 0x040001B4 RID: 436
			SioDeviceVersion,
			// Token: 0x040001B5 RID: 437
			SioQueryForCmd,
			// Token: 0x040001B6 RID: 438
			SioGetUpdateLogs,
			// Token: 0x040001B7 RID: 439
			SioQueryDeviceUnlockId,
			// Token: 0x040001B8 RID: 440
			SioRelockDeviceUnlockId,
			// Token: 0x040001B9 RID: 441
			SioQueryUnlockTokenFiles,
			// Token: 0x040001BA RID: 442
			SioWriteUnlockTokenFile,
			// Token: 0x040001BB RID: 443
			SioQueryBitlockerState,
			// Token: 0x040001BC RID: 444
			SioLast = 29
		}
	}
}
