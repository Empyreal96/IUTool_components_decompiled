using System;
using System.Diagnostics.Eventing;

namespace FFUComponents
{
	// Token: 0x02000049 RID: 73
	public class FlashingHostLogger : IDisposable
	{
		// Token: 0x060001EB RID: 491 RVA: 0x00008D2F File Offset: 0x00006F2F
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.m_provider.Dispose();
			}
		}

		// Token: 0x060001EC RID: 492 RVA: 0x00008D3F File Offset: 0x00006F3F
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x060001ED RID: 493 RVA: 0x00008D50 File Offset: 0x00006F50
		public FlashingHostLogger()
		{
			this.Flash_Start = new EventDescriptor(0, 0, 0, 4, 1, 1, 0L);
			this.Flash_Stop = new EventDescriptor(1, 0, 0, 4, 2, 1, 0L);
			this.Device_Attach = new EventDescriptor(2, 0, 0, 4, 10, 2, 0L);
			this.Device_Detach = new EventDescriptor(3, 0, 0, 4, 11, 2, 0L);
			this.Device_Remove = new EventDescriptor(4, 0, 0, 4, 12, 2, 0L);
			this.Flash_Error = new EventDescriptor(5, 0, 0, 2, 0, 1, 0L);
			this.Flash_Timeout = new EventDescriptor(6, 0, 0, 2, 0, 1, 0L);
			this.TransferException = new EventDescriptor(7, 0, 0, 2, 0, 3, 0L);
			this.ReconnectIOException = new EventDescriptor(8, 0, 0, 2, 13, 8, 0L);
			this.ReconnectWin32Exception = new EventDescriptor(9, 0, 0, 2, 14, 8, 0L);
			this.ReadBootmeIOException = new EventDescriptor(10, 0, 0, 2, 13, 4, 0L);
			this.ReadBootmeWin32Exception = new EventDescriptor(11, 0, 0, 2, 14, 4, 0L);
			this.SkipIOException = new EventDescriptor(12, 0, 0, 2, 0, 5, 0L);
			this.SkipWin32Exception = new EventDescriptor(13, 0, 0, 2, 0, 5, 0L);
			this.WriteSkipFailed = new EventDescriptor(14, 0, 0, 2, 15, 5, 0L);
			this.USBResetWin32Exception = new EventDescriptor(15, 0, 0, 2, 14, 6, 0L);
			this.RebootIOException = new EventDescriptor(16, 0, 0, 2, 13, 7, 0L);
			this.RebootWin32Exception = new EventDescriptor(17, 0, 0, 2, 14, 7, 0L);
			this.ConnectWin32Exception = new EventDescriptor(18, 0, 0, 2, 14, 9, 0L);
			this.ThreadException = new EventDescriptor(19, 0, 0, 2, 15, 2, 0L);
			this.FileRead_Start = new EventDescriptor(20, 0, 0, 4, 1, 10, 0L);
			this.FileRead_Stop = new EventDescriptor(21, 0, 0, 4, 2, 10, 0L);
			this.WaitAbandoned = new EventDescriptor(22, 0, 0, 2, 2, 11, 0L);
			this.MutexTimeout = new EventDescriptor(23, 0, 0, 2, 2, 11, 0L);
			this.ConnectNotifyException = new EventDescriptor(24, 0, 0, 3, 10, 2, 0L);
			this.DisconnectNotifyException = new EventDescriptor(25, 0, 0, 3, 12, 2, 0L);
			this.InitNotifyException = new EventDescriptor(26, 0, 0, 3, 10, 2, 0L);
			this.MassStorageIOException = new EventDescriptor(27, 0, 0, 2, 13, 12, 0L);
			this.MassStorageWin32Exception = new EventDescriptor(28, 0, 0, 2, 14, 12, 0L);
			this.StreamClearStart = new EventDescriptor(29, 0, 0, 4, 1, 13, 0L);
			this.StreamClearStop = new EventDescriptor(30, 0, 0, 4, 2, 13, 0L);
			this.StreamClearPushWin32Exception = new EventDescriptor(31, 0, 0, 4, 14, 13, 0L);
			this.StreamClearPullWin32Exception = new EventDescriptor(32, 0, 0, 4, 14, 13, 0L);
			this.StreamClearIOException = new EventDescriptor(33, 0, 0, 4, 13, 13, 0L);
			this.ClearIdIOException = new EventDescriptor(34, 0, 0, 2, 13, 14, 0L);
			this.ClearIdWin32Exception = new EventDescriptor(35, 0, 0, 2, 14, 14, 0L);
			this.WimSuccess = new EventDescriptor(36, 0, 0, 4, 16, 15, 0L);
			this.WimError = new EventDescriptor(37, 0, 0, 2, 16, 15, 0L);
			this.WimIOException = new EventDescriptor(38, 0, 0, 2, 13, 15, 0L);
			this.WimWin32Exception = new EventDescriptor(39, 0, 0, 2, 14, 15, 0L);
			this.WimTransferStart = new EventDescriptor(40, 0, 0, 4, 1, 16, 0L);
			this.WimTransferStop = new EventDescriptor(41, 0, 0, 4, 2, 16, 0L);
			this.WimPacketStart = new EventDescriptor(42, 0, 0, 4, 1, 17, 0L);
			this.WimPacketStop = new EventDescriptor(43, 0, 0, 4, 2, 17, 0L);
			this.WimGetStatus = new EventDescriptor(44, 0, 0, 4, 1, 15, 0L);
			this.BootModeIOException = new EventDescriptor(45, 0, 0, 2, 13, 18, 0L);
			this.BootModeWin32Exception = new EventDescriptor(46, 0, 0, 2, 14, 18, 0L);
			this.DeviceFlashParameters = new EventDescriptor(47, 0, 0, 4, 0, 1, 0L);
		}

		// Token: 0x060001EE RID: 494 RVA: 0x00009285 File Offset: 0x00007485
		public bool EventWriteFlash_Start(Guid DeviceId, string DeviceFriendlyName, string AssemblyFileVersion)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEventWithString(ref this.Flash_Start, DeviceId, DeviceFriendlyName, AssemblyFileVersion);
		}

		// Token: 0x060001EF RID: 495 RVA: 0x000092AA File Offset: 0x000074AA
		public bool EventWriteFlash_Stop(Guid DeviceId, string DeviceFriendlyName)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.Flash_Stop, DeviceId, DeviceFriendlyName);
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x000092CE File Offset: 0x000074CE
		public bool EventWriteDevice_Attach(Guid DeviceId, string DeviceFriendlyName)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.Device_Attach, DeviceId, DeviceFriendlyName);
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x000092F2 File Offset: 0x000074F2
		public bool EventWriteDevice_Detach(Guid DeviceId, string DeviceFriendlyName)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.Device_Detach, DeviceId, DeviceFriendlyName);
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x00009316 File Offset: 0x00007516
		public bool EventWriteDevice_Remove(Guid DeviceId, string DeviceFriendlyName)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.Device_Remove, DeviceId, DeviceFriendlyName);
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x0000933A File Offset: 0x0000753A
		public bool EventWriteFlash_Error(Guid DeviceId, string DeviceFriendlyName)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.Flash_Error, DeviceId, DeviceFriendlyName);
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0000935E File Offset: 0x0000755E
		public bool EventWriteFlash_Timeout(Guid DeviceId, string DeviceFriendlyName)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.Flash_Timeout, DeviceId, DeviceFriendlyName);
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x00009382 File Offset: 0x00007582
		public bool EventWriteTransferException(Guid DeviceId, string DeviceFriendlyName, int ErrorCode)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.TransferException, DeviceId, DeviceFriendlyName, ErrorCode);
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x000093A7 File Offset: 0x000075A7
		public bool EventWriteReconnectIOException(Guid DeviceId, string DeviceFriendlyName)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.ReconnectIOException, DeviceId, DeviceFriendlyName);
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x000093CB File Offset: 0x000075CB
		public bool EventWriteReconnectWin32Exception(Guid DeviceId, string DeviceFriendlyName, int ErrorCode)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.ReconnectWin32Exception, DeviceId, DeviceFriendlyName, ErrorCode);
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x000093F0 File Offset: 0x000075F0
		public bool EventWriteReadBootmeIOException(Guid DeviceId, string DeviceFriendlyName)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.ReadBootmeIOException, DeviceId, DeviceFriendlyName);
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x00009414 File Offset: 0x00007614
		public bool EventWriteReadBootmeWin32Exception(Guid DeviceId, string DeviceFriendlyName, int ErrorCode)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.ReadBootmeWin32Exception, DeviceId, DeviceFriendlyName, ErrorCode);
		}

		// Token: 0x060001FA RID: 506 RVA: 0x00009439 File Offset: 0x00007639
		public bool EventWriteSkipIOException(Guid DeviceId, string DeviceFriendlyName)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.SkipIOException, DeviceId, DeviceFriendlyName);
		}

		// Token: 0x060001FB RID: 507 RVA: 0x0000945D File Offset: 0x0000765D
		public bool EventWriteSkipWin32Exception(Guid DeviceId, string DeviceFriendlyName, int ErrorCode)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.SkipWin32Exception, DeviceId, DeviceFriendlyName, ErrorCode);
		}

		// Token: 0x060001FC RID: 508 RVA: 0x00009482 File Offset: 0x00007682
		public bool EventWriteWriteSkipFailed(Guid DeviceId, string DeviceFriendlyName, int ErrorCode)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.WriteSkipFailed, DeviceId, DeviceFriendlyName, ErrorCode);
		}

		// Token: 0x060001FD RID: 509 RVA: 0x000094A7 File Offset: 0x000076A7
		public bool EventWriteUSBResetWin32Exception(Guid DeviceId, string DeviceFriendlyName, int ErrorCode)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.USBResetWin32Exception, DeviceId, DeviceFriendlyName, ErrorCode);
		}

		// Token: 0x060001FE RID: 510 RVA: 0x000094CC File Offset: 0x000076CC
		public bool EventWriteRebootIOException(Guid DeviceId, string DeviceFriendlyName)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.RebootIOException, DeviceId, DeviceFriendlyName);
		}

		// Token: 0x060001FF RID: 511 RVA: 0x000094F0 File Offset: 0x000076F0
		public bool EventWriteRebootWin32Exception(Guid DeviceId, string DeviceFriendlyName, int ErrorCode)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.RebootWin32Exception, DeviceId, DeviceFriendlyName, ErrorCode);
		}

		// Token: 0x06000200 RID: 512 RVA: 0x00009515 File Offset: 0x00007715
		public bool EventWriteConnectWin32Exception(Guid DeviceId, string DeviceFriendlyName, int ErrorCode)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.ConnectWin32Exception, DeviceId, DeviceFriendlyName, ErrorCode);
		}

		// Token: 0x06000201 RID: 513 RVA: 0x0000953A File Offset: 0x0000773A
		public bool EventWriteThreadException(string String)
		{
			return this.m_provider.WriteEvent(ref this.ThreadException, String);
		}

		// Token: 0x06000202 RID: 514 RVA: 0x0000954E File Offset: 0x0000774E
		public bool EventWriteFileRead_Start(Guid DeviceId, string DeviceFriendlyName)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.FileRead_Start, DeviceId, DeviceFriendlyName);
		}

		// Token: 0x06000203 RID: 515 RVA: 0x00009572 File Offset: 0x00007772
		public bool EventWriteFileRead_Stop(Guid DeviceId, string DeviceFriendlyName)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.FileRead_Stop, DeviceId, DeviceFriendlyName);
		}

		// Token: 0x06000204 RID: 516 RVA: 0x00009596 File Offset: 0x00007796
		public bool EventWriteWaitAbandoned(Guid DeviceId, string DeviceFriendlyName)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.WaitAbandoned, DeviceId, DeviceFriendlyName);
		}

		// Token: 0x06000205 RID: 517 RVA: 0x000095BA File Offset: 0x000077BA
		public bool EventWriteMutexTimeout(Guid DeviceId, string DeviceFriendlyName)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.MutexTimeout, DeviceId, DeviceFriendlyName);
		}

		// Token: 0x06000206 RID: 518 RVA: 0x000095DE File Offset: 0x000077DE
		public bool EventWriteConnectNotifyException(string DevicePath, string Exception)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateNotifyException(ref this.ConnectNotifyException, DevicePath, Exception);
		}

		// Token: 0x06000207 RID: 519 RVA: 0x00009602 File Offset: 0x00007802
		public bool EventWriteDisconnectNotifyException(string DevicePath, string Exception)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateNotifyException(ref this.DisconnectNotifyException, DevicePath, Exception);
		}

		// Token: 0x06000208 RID: 520 RVA: 0x00009626 File Offset: 0x00007826
		public bool EventWriteInitNotifyException(string DevicePath, string Exception)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateNotifyException(ref this.InitNotifyException, DevicePath, Exception);
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0000964A File Offset: 0x0000784A
		public bool EventWriteMassStorageIOException(Guid DeviceId, string DeviceFriendlyName)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.MassStorageIOException, DeviceId, DeviceFriendlyName);
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0000966E File Offset: 0x0000786E
		public bool EventWriteMassStorageWin32Exception(Guid DeviceId, string DeviceFriendlyName, int ErrorCode)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.MassStorageWin32Exception, DeviceId, DeviceFriendlyName, ErrorCode);
		}

		// Token: 0x0600020B RID: 523 RVA: 0x00009693 File Offset: 0x00007893
		public bool EventWriteStreamClearStart(Guid DeviceId, string DeviceFriendlyName)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.StreamClearStart, DeviceId, DeviceFriendlyName);
		}

		// Token: 0x0600020C RID: 524 RVA: 0x000096B7 File Offset: 0x000078B7
		public bool EventWriteStreamClearStop(Guid DeviceId, string DeviceFriendlyName)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.StreamClearStop, DeviceId, DeviceFriendlyName);
		}

		// Token: 0x0600020D RID: 525 RVA: 0x000096DB File Offset: 0x000078DB
		public bool EventWriteStreamClearPushWin32Exception(Guid DeviceId, string DeviceFriendlyName, int ErrorCode)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.StreamClearPushWin32Exception, DeviceId, DeviceFriendlyName, ErrorCode);
		}

		// Token: 0x0600020E RID: 526 RVA: 0x00009700 File Offset: 0x00007900
		public bool EventWriteStreamClearPullWin32Exception(Guid DeviceId, string DeviceFriendlyName, int ErrorCode)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.StreamClearPullWin32Exception, DeviceId, DeviceFriendlyName, ErrorCode);
		}

		// Token: 0x0600020F RID: 527 RVA: 0x00009725 File Offset: 0x00007925
		public bool EventWriteStreamClearIOException(Guid DeviceId, string DeviceFriendlyName)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.StreamClearIOException, DeviceId, DeviceFriendlyName);
		}

		// Token: 0x06000210 RID: 528 RVA: 0x00009749 File Offset: 0x00007949
		public bool EventWriteClearIdIOException(Guid DeviceId, string DeviceFriendlyName)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.ClearIdIOException, DeviceId, DeviceFriendlyName);
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0000976D File Offset: 0x0000796D
		public bool EventWriteClearIdWin32Exception(Guid DeviceId, string DeviceFriendlyName, int ErrorCode)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.ClearIdWin32Exception, DeviceId, DeviceFriendlyName, ErrorCode);
		}

		// Token: 0x06000212 RID: 530 RVA: 0x00009792 File Offset: 0x00007992
		public bool EventWriteWimSuccess(Guid DeviceId, string DeviceFriendlyName, int ErrorCode)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.WimSuccess, DeviceId, DeviceFriendlyName, ErrorCode);
		}

		// Token: 0x06000213 RID: 531 RVA: 0x000097B7 File Offset: 0x000079B7
		public bool EventWriteWimError(Guid DeviceId, string DeviceFriendlyName, int ErrorCode)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.WimError, DeviceId, DeviceFriendlyName, ErrorCode);
		}

		// Token: 0x06000214 RID: 532 RVA: 0x000097DC File Offset: 0x000079DC
		public bool EventWriteWimIOException(Guid DeviceId, string DeviceFriendlyName)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.WimIOException, DeviceId, DeviceFriendlyName);
		}

		// Token: 0x06000215 RID: 533 RVA: 0x00009800 File Offset: 0x00007A00
		public bool EventWriteWimWin32Exception(Guid DeviceId, string DeviceFriendlyName, int ErrorCode)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.WimWin32Exception, DeviceId, DeviceFriendlyName, ErrorCode);
		}

		// Token: 0x06000216 RID: 534 RVA: 0x00009825 File Offset: 0x00007A25
		public bool EventWriteWimTransferStart(Guid DeviceId, string DeviceFriendlyName)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.WimTransferStart, DeviceId, DeviceFriendlyName);
		}

		// Token: 0x06000217 RID: 535 RVA: 0x00009849 File Offset: 0x00007A49
		public bool EventWriteWimTransferStop(Guid DeviceId, string DeviceFriendlyName)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.WimTransferStop, DeviceId, DeviceFriendlyName);
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000986D File Offset: 0x00007A6D
		public bool EventWriteWimPacketStart(Guid DeviceId, string DeviceFriendlyName, int TransferSize)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEventWithSize(ref this.WimPacketStart, DeviceId, DeviceFriendlyName, TransferSize);
		}

		// Token: 0x06000219 RID: 537 RVA: 0x00009892 File Offset: 0x00007A92
		public bool EventWriteWimPacketStop(Guid DeviceId, string DeviceFriendlyName, int ErrorCode)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.WimPacketStop, DeviceId, DeviceFriendlyName, ErrorCode);
		}

		// Token: 0x0600021A RID: 538 RVA: 0x000098B7 File Offset: 0x00007AB7
		public bool EventWriteWimGetStatus(Guid DeviceId, string DeviceFriendlyName)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.WimGetStatus, DeviceId, DeviceFriendlyName);
		}

		// Token: 0x0600021B RID: 539 RVA: 0x000098DB File Offset: 0x00007ADB
		public bool EventWriteBootModeIOException(Guid DeviceId, string DeviceFriendlyName)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceSpecificEvent(ref this.BootModeIOException, DeviceId, DeviceFriendlyName);
		}

		// Token: 0x0600021C RID: 540 RVA: 0x000098FF File Offset: 0x00007AFF
		public bool EventWriteBootModeWin32Exception(Guid DeviceId, string DeviceFriendlyName, int ErrorCode)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceEventWithErrorCode(ref this.BootModeWin32Exception, DeviceId, DeviceFriendlyName, ErrorCode);
		}

		// Token: 0x0600021D RID: 541 RVA: 0x00009924 File Offset: 0x00007B24
		public bool EventWriteDeviceFlashParameters(int USBTransactionSize, int PacketDataSize)
		{
			return !this.m_provider.IsEnabled() || this.m_provider.TemplateDeviceFlashParameters(ref this.DeviceFlashParameters, USBTransactionSize, PacketDataSize);
		}

		// Token: 0x0400012E RID: 302
		internal EventProviderVersionTwo m_provider = new EventProviderVersionTwo(new Guid("fb961307-bc64-4de4-8828-81d583524da0"));

		// Token: 0x0400012F RID: 303
		private Guid FlashId = new Guid("80ada65c-a7fa-49f8-a2ed-f67790c8f016");

		// Token: 0x04000130 RID: 304
		private Guid DeviceStatusChangeId = new Guid("3a02d575-c63d-4a76-9adf-9b6b736c66dc");

		// Token: 0x04000131 RID: 305
		private Guid TransferId = new Guid("211e6307-fd7c-49f9-a4db-d9ae5a4adb22");

		// Token: 0x04000132 RID: 306
		private Guid BootmeId = new Guid("a0cd9e55-fb70-452f-ac50-2eb82d2984b5");

		// Token: 0x04000133 RID: 307
		private Guid SkipId = new Guid("4979cb5a-17d4-47c6-9ac6-e97446bd74f4");

		// Token: 0x04000134 RID: 308
		private Guid ResetId = new Guid("768fda16-a5c7-44cf-8a47-03580b28538d");

		// Token: 0x04000135 RID: 309
		private Guid RebootId = new Guid("850eedee-9b52-4171-af6f-73c34d84a893");

		// Token: 0x04000136 RID: 310
		private Guid ReconnectId = new Guid("1a80ed37-3a4f-4b81-a466-accb411f96e1");

		// Token: 0x04000137 RID: 311
		private Guid ConnectId = new Guid("bebe24cb-92b1-40ca-843a-f2f9f0cab947");

		// Token: 0x04000138 RID: 312
		private Guid FileReadId = new Guid("d875a842-f690-40bf-880a-16e7d2a88d85");

		// Token: 0x04000139 RID: 313
		private Guid MutexWaitId = new Guid("3120aadc-6b30-4509-bedf-9696c78ddd9c");

		// Token: 0x0400013A RID: 314
		private Guid MassStorageId = new Guid("1b67e5c6-caab-4424-8d24-5c2c258aff5f");

		// Token: 0x0400013B RID: 315
		private Guid StreamClearId = new Guid("d32ce88a-c858-4ed1-86ac-764c58bf2599");

		// Token: 0x0400013C RID: 316
		private Guid ClearIdId = new Guid("3aa9618a-8ac9-4386-b524-c32f4326e59e");

		// Token: 0x0400013D RID: 317
		private Guid WimId = new Guid("0a86e459-1f85-459f-a9da-dca82415c492");

		// Token: 0x0400013E RID: 318
		private Guid WimTransferId = new Guid("53874dd6-905f-4a4c-ac66-5dadb02f4ce8");

		// Token: 0x0400013F RID: 319
		private Guid WimPacketId = new Guid("6f4a3de2-cddd-40d5-829d-861ccbcaff4d");

		// Token: 0x04000140 RID: 320
		private Guid BootModeId = new Guid("07bacab6-769a-4b6c-a68f-3524423291d2");

		// Token: 0x04000141 RID: 321
		protected EventDescriptor Flash_Start;

		// Token: 0x04000142 RID: 322
		protected EventDescriptor Flash_Stop;

		// Token: 0x04000143 RID: 323
		protected EventDescriptor Device_Attach;

		// Token: 0x04000144 RID: 324
		protected EventDescriptor Device_Detach;

		// Token: 0x04000145 RID: 325
		protected EventDescriptor Device_Remove;

		// Token: 0x04000146 RID: 326
		protected EventDescriptor Flash_Error;

		// Token: 0x04000147 RID: 327
		protected EventDescriptor Flash_Timeout;

		// Token: 0x04000148 RID: 328
		protected EventDescriptor TransferException;

		// Token: 0x04000149 RID: 329
		protected EventDescriptor ReconnectIOException;

		// Token: 0x0400014A RID: 330
		protected EventDescriptor ReconnectWin32Exception;

		// Token: 0x0400014B RID: 331
		protected EventDescriptor ReadBootmeIOException;

		// Token: 0x0400014C RID: 332
		protected EventDescriptor ReadBootmeWin32Exception;

		// Token: 0x0400014D RID: 333
		protected EventDescriptor SkipIOException;

		// Token: 0x0400014E RID: 334
		protected EventDescriptor SkipWin32Exception;

		// Token: 0x0400014F RID: 335
		protected EventDescriptor WriteSkipFailed;

		// Token: 0x04000150 RID: 336
		protected EventDescriptor USBResetWin32Exception;

		// Token: 0x04000151 RID: 337
		protected EventDescriptor RebootIOException;

		// Token: 0x04000152 RID: 338
		protected EventDescriptor RebootWin32Exception;

		// Token: 0x04000153 RID: 339
		protected EventDescriptor ConnectWin32Exception;

		// Token: 0x04000154 RID: 340
		protected EventDescriptor ThreadException;

		// Token: 0x04000155 RID: 341
		protected EventDescriptor FileRead_Start;

		// Token: 0x04000156 RID: 342
		protected EventDescriptor FileRead_Stop;

		// Token: 0x04000157 RID: 343
		protected EventDescriptor WaitAbandoned;

		// Token: 0x04000158 RID: 344
		protected EventDescriptor MutexTimeout;

		// Token: 0x04000159 RID: 345
		protected EventDescriptor ConnectNotifyException;

		// Token: 0x0400015A RID: 346
		protected EventDescriptor DisconnectNotifyException;

		// Token: 0x0400015B RID: 347
		protected EventDescriptor InitNotifyException;

		// Token: 0x0400015C RID: 348
		protected EventDescriptor MassStorageIOException;

		// Token: 0x0400015D RID: 349
		protected EventDescriptor MassStorageWin32Exception;

		// Token: 0x0400015E RID: 350
		protected EventDescriptor StreamClearStart;

		// Token: 0x0400015F RID: 351
		protected EventDescriptor StreamClearStop;

		// Token: 0x04000160 RID: 352
		protected EventDescriptor StreamClearPushWin32Exception;

		// Token: 0x04000161 RID: 353
		protected EventDescriptor StreamClearPullWin32Exception;

		// Token: 0x04000162 RID: 354
		protected EventDescriptor StreamClearIOException;

		// Token: 0x04000163 RID: 355
		protected EventDescriptor ClearIdIOException;

		// Token: 0x04000164 RID: 356
		protected EventDescriptor ClearIdWin32Exception;

		// Token: 0x04000165 RID: 357
		protected EventDescriptor WimSuccess;

		// Token: 0x04000166 RID: 358
		protected EventDescriptor WimError;

		// Token: 0x04000167 RID: 359
		protected EventDescriptor WimIOException;

		// Token: 0x04000168 RID: 360
		protected EventDescriptor WimWin32Exception;

		// Token: 0x04000169 RID: 361
		protected EventDescriptor WimTransferStart;

		// Token: 0x0400016A RID: 362
		protected EventDescriptor WimTransferStop;

		// Token: 0x0400016B RID: 363
		protected EventDescriptor WimPacketStart;

		// Token: 0x0400016C RID: 364
		protected EventDescriptor WimPacketStop;

		// Token: 0x0400016D RID: 365
		protected EventDescriptor WimGetStatus;

		// Token: 0x0400016E RID: 366
		protected EventDescriptor BootModeIOException;

		// Token: 0x0400016F RID: 367
		protected EventDescriptor BootModeWin32Exception;

		// Token: 0x04000170 RID: 368
		protected EventDescriptor DeviceFlashParameters;
	}
}
