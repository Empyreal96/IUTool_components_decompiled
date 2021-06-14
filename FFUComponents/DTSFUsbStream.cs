using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace FFUComponents
{
	// Token: 0x02000048 RID: 72
	internal class DTSFUsbStream : Stream
	{
		// Token: 0x060001C0 RID: 448 RVA: 0x000082AC File Offset: 0x000064AC
		public DTSFUsbStream(string deviceName, FileShare shareMode, TimeSpan transferTimeout)
		{
			if (string.IsNullOrEmpty(deviceName))
			{
				throw new ArgumentException("Invalid Argument", "deviceName");
			}
			this.isDisposed = false;
			this.deviceName = deviceName;
			try
			{
				int num = 0;
				this.deviceHandle = DTSFUsbStream.CreateDeviceHandle(this.deviceName, shareMode, ref num);
				if (this.deviceHandle.IsInvalid)
				{
					throw new IOException(string.Format(CultureInfo.CurrentCulture, Resources.ERROR_INVALID_HANDLE, new object[]
					{
						deviceName,
						num
					}));
				}
				this.InitializeDevice();
				this.SetTransferTimeout(transferTimeout);
				if (!ThreadPool.BindHandle(this.deviceHandle))
				{
					throw new IOException(string.Format(CultureInfo.CurrentCulture, Resources.ERROR_BINDHANDLE, new object[]
					{
						deviceName
					}));
				}
				this.Connect();
			}
			catch (Exception)
			{
				this.CloseDeviceHandle();
				throw;
			}
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x000083A0 File Offset: 0x000065A0
		public DTSFUsbStream(string deviceName, TimeSpan transferTimeout) : this(deviceName, FileShare.None, transferTimeout)
		{
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060001C2 RID: 450 RVA: 0x000083AB File Offset: 0x000065AB
		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060001C3 RID: 451 RVA: 0x00006863 File Offset: 0x00004A63
		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060001C4 RID: 452 RVA: 0x000083AB File Offset: 0x000065AB
		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060001C5 RID: 453 RVA: 0x000061D0 File Offset: 0x000043D0
		public override long Length
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060001C6 RID: 454 RVA: 0x000061D0 File Offset: 0x000043D0
		// (set) Token: 0x060001C7 RID: 455 RVA: 0x000061D0 File Offset: 0x000043D0
		public override long Position
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x000061D0 File Offset: 0x000043D0
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x000061D0 File Offset: 0x000043D0
		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060001CA RID: 458 RVA: 0x000083AE File Offset: 0x000065AE
		public override void Flush()
		{
		}

		// Token: 0x060001CB RID: 459 RVA: 0x000083B0 File Offset: 0x000065B0
		private void HandleAsyncTimeout(IAsyncResult asyncResult)
		{
			if (NativeMethods.CancelIo(this.deviceHandle))
			{
				asyncResult.AsyncWaitHandle.WaitOne(this.completionTimeout, false);
			}
		}

		// Token: 0x060001CC RID: 460 RVA: 0x000083D4 File Offset: 0x000065D4
		public override int Read(byte[] buffer, int offset, int count)
		{
			IAsyncResult asyncResult = this.BeginRead(buffer, offset, count, null, null);
			int result;
			try
			{
				result = this.EndRead(asyncResult);
			}
			catch (TimeoutException innerException)
			{
				this.HandleAsyncTimeout(asyncResult);
				throw new Win32Exception(Resources.ERROR_CALLBACK_TIMEOUT, innerException);
			}
			return result;
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0000841C File Offset: 0x0000661C
		public override void Write(byte[] buffer, int offset, int count)
		{
			IAsyncResult asyncResult = this.BeginWrite(buffer, offset, count, null, null);
			try
			{
				this.EndWrite(asyncResult);
			}
			catch (TimeoutException innerException)
			{
				this.HandleAsyncTimeout(asyncResult);
				throw new Win32Exception(Resources.ERROR_CALLBACK_TIMEOUT, innerException);
			}
		}

		// Token: 0x060001CE RID: 462 RVA: 0x00008464 File Offset: 0x00006664
		private void RetryRead(uint errorCode, DTSFUsbStreamReadAsyncResult asyncResult, out Exception exception)
		{
			exception = null;
			if (this.IsDeviceDisconnected(errorCode))
			{
				exception = new Win32Exception((int)errorCode);
				return;
			}
			if (asyncResult.RetryCount > 10)
			{
				exception = new Win32Exception((int)errorCode);
				return;
			}
			int num = 0;
			this.ClearPipeStall(this.bulkInPipeId, out num);
			if (num != 0)
			{
				exception = new Win32Exception(num);
				return;
			}
			try
			{
				byte[] buffer = asyncResult.Buffer;
				int offset = asyncResult.Offset;
				int count = asyncResult.Count;
				int num2 = asyncResult.RetryCount;
				asyncResult.RetryCount = num2 + 1;
				this.BeginReadInternal(buffer, offset, count, num2, asyncResult.AsyncCallback, asyncResult.AsyncState);
			}
			catch (Exception ex)
			{
				exception = ex;
			}
		}

		// Token: 0x060001CF RID: 463 RVA: 0x00008504 File Offset: 0x00006704
		private unsafe void ReadIOCompletionCallback(uint errorCode, uint numBytes, NativeOverlapped* nativeOverlapped)
		{
			try
			{
				DTSFUsbStreamReadAsyncResult dtsfusbStreamReadAsyncResult = (DTSFUsbStreamReadAsyncResult)Overlapped.Unpack(nativeOverlapped).AsyncResult;
				Overlapped.Free(nativeOverlapped);
				Exception ex = null;
				if (errorCode != 0U)
				{
					this.RetryRead(errorCode, dtsfusbStreamReadAsyncResult, out ex);
					if (ex != null)
					{
						dtsfusbStreamReadAsyncResult.SetAsCompleted(ex, false);
					}
				}
				else
				{
					dtsfusbStreamReadAsyncResult.SetAsCompleted((int)numBytes, false);
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x00008564 File Offset: 0x00006764
		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback userCallback, object stateObject)
		{
			if (this.deviceHandle.IsClosed)
			{
				throw new ObjectDisposedException(Resources.ERROR_FILE_CLOSED);
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("Argument_InvalidOffLen");
			}
			return this.BeginReadInternal(buffer, offset, count, 10, userCallback, stateObject);
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x000085E4 File Offset: 0x000067E4
		private unsafe IAsyncResult BeginReadInternal(byte[] buffer, int offset, int count, int retryCount, AsyncCallback userCallback, object stateObject)
		{
			NativeOverlapped* ptr = null;
			DTSFUsbStreamReadAsyncResult dtsfusbStreamReadAsyncResult = new DTSFUsbStreamReadAsyncResult(userCallback, stateObject)
			{
				Buffer = buffer,
				Offset = offset,
				Count = count,
				RetryCount = retryCount
			};
			ptr = new Overlapped(0, 0, IntPtr.Zero, dtsfusbStreamReadAsyncResult).Pack(new IOCompletionCallback(this.ReadIOCompletionCallback), buffer);
			fixed (byte* ptr2 = buffer)
			{
				if (!NativeMethods.WinUsbReadPipe(this.usbHandle, this.bulkInPipeId, ptr2 + offset, (uint)count, IntPtr.Zero, ptr))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (997 != lastWin32Error)
					{
						Overlapped.Unpack(ptr);
						Overlapped.Free(ptr);
						throw new Win32Exception(lastWin32Error);
					}
				}
			}
			return dtsfusbStreamReadAsyncResult;
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x00008699 File Offset: 0x00006899
		public override int EndRead(IAsyncResult asyncResult)
		{
			return ((DTSFUsbStreamReadAsyncResult)asyncResult).EndInvoke();
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x000086A8 File Offset: 0x000068A8
		private void RetryWrite(uint errorCode, DTSFUsbStreamWriteAsyncResult asyncResult, out Exception exception)
		{
			exception = null;
			if (this.IsDeviceDisconnected(errorCode))
			{
				exception = new Win32Exception((int)errorCode);
				return;
			}
			if (asyncResult.RetryCount > 10)
			{
				exception = new Win32Exception((int)errorCode);
				return;
			}
			int num = 0;
			this.ClearPipeStall(this.bulkOutPipeId, out num);
			if (num != 0)
			{
				exception = new Win32Exception(num);
				return;
			}
			try
			{
				byte[] buffer = asyncResult.Buffer;
				int offset = asyncResult.Offset;
				int count = asyncResult.Count;
				int num2 = asyncResult.RetryCount;
				asyncResult.RetryCount = num2 + 1;
				this.BeginWriteInternal(buffer, offset, count, num2, asyncResult.AsyncCallback, asyncResult.AsyncState);
			}
			catch (Exception ex)
			{
				exception = ex;
			}
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x00008748 File Offset: 0x00006948
		private unsafe void WriteIOCompletionCallback(uint errorCode, uint numBytes, NativeOverlapped* nativeOverlapped)
		{
			DTSFUsbStreamWriteAsyncResult dtsfusbStreamWriteAsyncResult = (DTSFUsbStreamWriteAsyncResult)Overlapped.Unpack(nativeOverlapped).AsyncResult;
			Overlapped.Free(nativeOverlapped);
			Exception ex = null;
			try
			{
				if (errorCode != 0U)
				{
					this.RetryWrite(errorCode, dtsfusbStreamWriteAsyncResult, out ex);
					if (ex != null)
					{
						dtsfusbStreamWriteAsyncResult.SetAsCompleted(ex, false);
					}
				}
				else
				{
					dtsfusbStreamWriteAsyncResult.SetAsCompleted(ex, false);
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x000087A8 File Offset: 0x000069A8
		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback userCallback, object stateObject)
		{
			if (this.deviceHandle.IsClosed)
			{
				throw new ObjectDisposedException(Resources.ERROR_FILE_CLOSED);
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("Argument_InvalidOffLen");
			}
			return this.BeginWriteInternal(buffer, offset, count, 0, userCallback, stateObject);
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x00008824 File Offset: 0x00006A24
		private unsafe IAsyncResult BeginWriteInternal(byte[] buffer, int offset, int count, int retryCount, AsyncCallback userCallback, object stateObject)
		{
			NativeOverlapped* ptr = null;
			DTSFUsbStreamWriteAsyncResult dtsfusbStreamWriteAsyncResult = new DTSFUsbStreamWriteAsyncResult(userCallback, stateObject)
			{
				Buffer = buffer,
				Offset = offset,
				Count = count,
				RetryCount = retryCount
			};
			ptr = new Overlapped(0, 0, IntPtr.Zero, dtsfusbStreamWriteAsyncResult).Pack(new IOCompletionCallback(this.WriteIOCompletionCallback), buffer);
			fixed (byte* ptr2 = buffer)
			{
				if (!NativeMethods.WinUsbWritePipe(this.usbHandle, this.bulkOutPipeId, ptr2 + offset, (uint)count, IntPtr.Zero, ptr))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (997 != lastWin32Error)
					{
						Overlapped.Unpack(ptr);
						Overlapped.Free(ptr);
						throw new Win32Exception(lastWin32Error);
					}
				}
			}
			return dtsfusbStreamWriteAsyncResult;
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x000088D9 File Offset: 0x00006AD9
		public override void EndWrite(IAsyncResult asyncResult)
		{
			((DTSFUsbStreamWriteAsyncResult)asyncResult).EndInvoke();
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x000088E6 File Offset: 0x00006AE6
		private static SafeFileHandle CreateDeviceHandle(string deviceName, FileShare shareMode, ref int lastError)
		{
			SafeFileHandle result = NativeMethods.CreateFile(deviceName, 3221225472U, (uint)shareMode, IntPtr.Zero, 3U, 1073741952U, IntPtr.Zero);
			lastError = Marshal.GetLastWin32Error();
			return result;
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x0000890C File Offset: 0x00006B0C
		private void CloseDeviceHandle()
		{
			if (IntPtr.Zero != this.usbHandle)
			{
				NativeMethods.WinUsbFree(this.usbHandle);
				this.usbHandle = IntPtr.Zero;
			}
			if (!this.deviceHandle.IsInvalid && !this.deviceHandle.IsClosed)
			{
				this.deviceHandle.Close();
				this.deviceHandle.SetHandleAsInvalid();
			}
		}

		// Token: 0x060001DA RID: 474 RVA: 0x00008974 File Offset: 0x00006B74
		private void InitializeDevice()
		{
			WinUsbInterfaceDescriptor winUsbInterfaceDescriptor = default(WinUsbInterfaceDescriptor);
			WinUsbPipeInformation winUsbPipeInformation = default(WinUsbPipeInformation);
			if (!NativeMethods.WinUsbInitialize(this.deviceHandle, ref this.usbHandle))
			{
				throw new IOException(Resources.ERROR_WINUSB_INITIALIZATION);
			}
			if (!NativeMethods.WinUsbQueryInterfaceSettings(this.usbHandle, 0, ref winUsbInterfaceDescriptor))
			{
				throw new IOException(Resources.ERROR_WINUSB_QUERY_INTERFACE_SETTING);
			}
			for (byte b = 0; b < winUsbInterfaceDescriptor.NumEndpoints; b += 1)
			{
				if (!NativeMethods.WinUsbQueryPipe(this.usbHandle, 0, b, ref winUsbPipeInformation))
				{
					throw new IOException(Resources.ERROR_WINUSB_QUERY_PIPE_INFORMATION);
				}
				WinUsbPipeType pipeType = winUsbPipeInformation.PipeType;
				if (pipeType == WinUsbPipeType.Bulk)
				{
					if (this.IsBulkInEndpoint(winUsbPipeInformation.PipeId))
					{
						this.SetupBulkInEndpoint(winUsbPipeInformation.PipeId);
					}
					else
					{
						if (!this.IsBulkOutEndpoint(winUsbPipeInformation.PipeId))
						{
							throw new IOException(Resources.ERROR_INVALID_ENDPOINT_TYPE);
						}
						this.SetupBulkOutEndpoint(winUsbPipeInformation.PipeId);
					}
				}
			}
		}

		// Token: 0x060001DB RID: 475 RVA: 0x00008A46 File Offset: 0x00006C46
		private bool IsBulkInEndpoint(byte pipeId)
		{
			return (pipeId & 128) == 128;
		}

		// Token: 0x060001DC RID: 476 RVA: 0x00008A56 File Offset: 0x00006C56
		private bool IsBulkOutEndpoint(byte pipeId)
		{
			return (pipeId & 128) == 0;
		}

		// Token: 0x060001DD RID: 477 RVA: 0x00008A64 File Offset: 0x00006C64
		public void PipeReset()
		{
			int num;
			this.ClearPipeStall(this.bulkInPipeId, out num);
			this.ClearPipeStall(this.bulkOutPipeId, out num);
		}

		// Token: 0x060001DE RID: 478 RVA: 0x00008A90 File Offset: 0x00006C90
		public void SetTransferTimeout(TimeSpan transferTimeout)
		{
			uint value = (uint)transferTimeout.TotalMilliseconds;
			this.SetPipePolicy(this.bulkInPipeId, 3U, value);
			this.SetPipePolicy(this.bulkOutPipeId, 3U, value);
		}

		// Token: 0x060001DF RID: 479 RVA: 0x00008AC2 File Offset: 0x00006CC2
		public void SetShortPacketTerminate()
		{
			this.SetPipePolicy(this.bulkOutPipeId, 1U, true);
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x00008AD2 File Offset: 0x00006CD2
		private void SetupBulkInEndpoint(byte pipeId)
		{
			this.bulkInPipeId = pipeId;
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x00008ADB File Offset: 0x00006CDB
		private void SetupBulkOutEndpoint(byte pipeId)
		{
			this.bulkOutPipeId = pipeId;
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x00008AE4 File Offset: 0x00006CE4
		private void SetPipePolicy(byte pipeId, uint policyType, uint value)
		{
			if (!NativeMethods.WinUsbSetPipePolicy(this.usbHandle, pipeId, policyType, (uint)Marshal.SizeOf(typeof(uint)), ref value))
			{
				throw new IOException(Resources.ERROR_WINUSB_SET_PIPE_POLICY);
			}
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x00008B11 File Offset: 0x00006D11
		private void SetPipePolicy(byte pipeId, uint policyType, bool value)
		{
			if (!NativeMethods.WinUsbSetPipePolicy(this.usbHandle, pipeId, policyType, (uint)Marshal.SizeOf(typeof(bool)), ref value))
			{
				throw new IOException(Resources.ERROR_WINUSB_SET_PIPE_POLICY);
			}
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x00008B40 File Offset: 0x00006D40
		private unsafe void ControlTransferSetData(UsbControlRequest request, ushort value)
		{
			WinUsbSetupPacket winUsbSetupPacket = default(WinUsbSetupPacket);
			winUsbSetupPacket.RequestType = 33;
			winUsbSetupPacket.Request = (byte)request;
			winUsbSetupPacket.Value = value;
			winUsbSetupPacket.Index = 0;
			winUsbSetupPacket.Length = 0;
			uint num = 0U;
			fixed (byte* ptr = null)
			{
				if (!NativeMethods.WinUsbControlTransfer(this.usbHandle, winUsbSetupPacket, ptr, (uint)winUsbSetupPacket.Length, ref num, IntPtr.Zero))
				{
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}
			}
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x00008BC4 File Offset: 0x00006DC4
		private unsafe void ControlTransferGetData(UsbControlRequest request, byte[] buffer)
		{
			WinUsbSetupPacket winUsbSetupPacket = default(WinUsbSetupPacket);
			winUsbSetupPacket.RequestType = 161;
			winUsbSetupPacket.Request = (byte)request;
			winUsbSetupPacket.Value = 0;
			winUsbSetupPacket.Index = 0;
			winUsbSetupPacket.Length = ((buffer == null) ? 0 : ((ushort)buffer.Length));
			uint num = 0U;
			fixed (byte* ptr = buffer)
			{
				if (!NativeMethods.WinUsbControlTransfer(this.usbHandle, winUsbSetupPacket, ptr, (uint)winUsbSetupPacket.Length, ref num, IntPtr.Zero))
				{
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}
			}
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x00008C53 File Offset: 0x00006E53
		private void ClearPipeStall(byte pipeId, out int errorCode)
		{
			errorCode = 0;
			if (!NativeMethods.WinUsbAbortPipe(this.usbHandle, pipeId))
			{
				errorCode = Marshal.GetLastWin32Error();
			}
			if (!NativeMethods.WinUsbResetPipe(this.usbHandle, pipeId))
			{
				errorCode = Marshal.GetLastWin32Error();
			}
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x00008C82 File Offset: 0x00006E82
		private bool IsDeviceDisconnected(uint errorCode)
		{
			return errorCode == 2U || errorCode == 1167U || errorCode == 31U || errorCode == 121U || errorCode == 995U;
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x00008CA4 File Offset: 0x00006EA4
		protected override void Dispose(bool disposing)
		{
			if (!this.isDisposed && disposing)
			{
				try
				{
					this.CloseDeviceHandle();
				}
				catch (Exception)
				{
				}
				finally
				{
					base.Dispose(disposing);
					this.isDisposed = true;
				}
			}
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x00008CF4 File Offset: 0x00006EF4
		~DTSFUsbStream()
		{
			this.Dispose(false);
		}

		// Token: 0x060001EA RID: 490 RVA: 0x00008D24 File Offset: 0x00006F24
		private void Connect()
		{
			this.ControlTransferSetData(UsbControlRequest.LineStateSet, 1);
		}

		// Token: 0x04000125 RID: 293
		private const byte UsbEndpointDirectionMask = 128;

		// Token: 0x04000126 RID: 294
		private string deviceName;

		// Token: 0x04000127 RID: 295
		private SafeFileHandle deviceHandle;

		// Token: 0x04000128 RID: 296
		private IntPtr usbHandle;

		// Token: 0x04000129 RID: 297
		private byte bulkInPipeId;

		// Token: 0x0400012A RID: 298
		private byte bulkOutPipeId;

		// Token: 0x0400012B RID: 299
		private bool isDisposed;

		// Token: 0x0400012C RID: 300
		private const int retryCount = 10;

		// Token: 0x0400012D RID: 301
		private TimeSpan completionTimeout = TimeSpan.FromSeconds(5.0);
	}
}
