using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace USB_Test_Infrastructure
{
	// Token: 0x0200001E RID: 30
	public class DTSFUsbStream : Stream
	{
		// Token: 0x0600004E RID: 78 RVA: 0x00002680 File Offset: 0x00000880
		public DTSFUsbStream(string deviceName)
		{
			if (string.IsNullOrEmpty(deviceName))
			{
				throw new ArgumentException("Invalid Argument", "deviceName");
			}
			this.isDisposed = false;
			this.deviceName = deviceName;
			try
			{
				this.deviceHandle = DTSFUsbStream.CreateDeviceHandle(this.deviceName);
				if (this.deviceHandle.IsInvalid)
				{
					throw new IOException(string.Format("Handle for {0} is invalid.", deviceName));
				}
				this.InitializeDevice();
				if (!ThreadPool.BindHandle(this.deviceHandle))
				{
					throw new IOException(string.Format("BindHandle on device {0} failed.", deviceName));
				}
				this.Connect();
			}
			catch (Exception)
			{
				this.CloseDeviceHandle();
				throw;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600004F RID: 79 RVA: 0x00002730 File Offset: 0x00000930
		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000050 RID: 80 RVA: 0x00002733 File Offset: 0x00000933
		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000051 RID: 81 RVA: 0x00002730 File Offset: 0x00000930
		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000052 RID: 82 RVA: 0x00002736 File Offset: 0x00000936
		public override long Length
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000053 RID: 83 RVA: 0x00002736 File Offset: 0x00000936
		// (set) Token: 0x06000054 RID: 84 RVA: 0x00002736 File Offset: 0x00000936
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

		// Token: 0x06000055 RID: 85 RVA: 0x00002736 File Offset: 0x00000936
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00002736 File Offset: 0x00000936
		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000057 RID: 87 RVA: 0x0000273D File Offset: 0x0000093D
		public override void Flush()
		{
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00002740 File Offset: 0x00000940
		public override int Read(byte[] buffer, int offset, int count)
		{
			IAsyncResult asyncResult = this.BeginRead(buffer, offset, count, null, null);
			return this.EndRead(asyncResult);
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00002760 File Offset: 0x00000960
		public override void Write(byte[] buffer, int offset, int count)
		{
			IAsyncResult asyncResult = this.BeginWrite(buffer, offset, count, null, null);
			this.EndWrite(asyncResult);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00002780 File Offset: 0x00000980
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

		// Token: 0x0600005B RID: 91 RVA: 0x00002820 File Offset: 0x00000A20
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

		// Token: 0x0600005C RID: 92 RVA: 0x00002880 File Offset: 0x00000A80
		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback userCallback, object stateObject)
		{
			if (this.deviceHandle.IsClosed)
			{
				throw new ObjectDisposedException("File closed");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("array");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("numBytes", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("Argument_InvalidOffLen");
			}
			return this.BeginReadInternal(buffer, offset, count, 10, userCallback, stateObject);
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00002900 File Offset: 0x00000B00
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

		// Token: 0x0600005E RID: 94 RVA: 0x000029B5 File Offset: 0x00000BB5
		public override int EndRead(IAsyncResult asyncResult)
		{
			return ((DTSFUsbStreamReadAsyncResult)asyncResult).EndInvoke();
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000029C4 File Offset: 0x00000BC4
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

		// Token: 0x06000060 RID: 96 RVA: 0x00002A64 File Offset: 0x00000C64
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

		// Token: 0x06000061 RID: 97 RVA: 0x00002AC4 File Offset: 0x00000CC4
		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback userCallback, object stateObject)
		{
			if (this.deviceHandle.IsClosed)
			{
				throw new ObjectDisposedException("File closed");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("array");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("numBytes", "ArgumentOutOfRange_NeedNonNegNum");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("Argument_InvalidOffLen");
			}
			return this.BeginWriteInternal(buffer, offset, count, 0, userCallback, stateObject);
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00002B40 File Offset: 0x00000D40
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

		// Token: 0x06000063 RID: 99 RVA: 0x00002BF5 File Offset: 0x00000DF5
		public override void EndWrite(IAsyncResult asyncResult)
		{
			((DTSFUsbStreamWriteAsyncResult)asyncResult).EndInvoke();
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00002C02 File Offset: 0x00000E02
		private static SafeFileHandle CreateDeviceHandle(string deviceName)
		{
			return NativeMethods.CreateFile(deviceName, 3221225472U, 3U, IntPtr.Zero, 3U, 1073741952U, IntPtr.Zero);
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00002C20 File Offset: 0x00000E20
		private void CloseDeviceHandle()
		{
			if (IntPtr.Zero != this.usbHandle)
			{
				NativeMethods.WinUsbFree(this.usbHandle);
			}
			if (!this.deviceHandle.IsInvalid && !this.deviceHandle.IsClosed)
			{
				NativeMethods.CloseHandle(this.deviceHandle);
			}
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00002C70 File Offset: 0x00000E70
		private void InitializeDevice()
		{
			WinUsbInterfaceDescriptor winUsbInterfaceDescriptor = default(WinUsbInterfaceDescriptor);
			WinUsbPipeInformation winUsbPipeInformation = default(WinUsbPipeInformation);
			if (!NativeMethods.WinUsbInitialize(this.deviceHandle, ref this.usbHandle))
			{
				throw new IOException("WinUsb Initialization failed.");
			}
			if (!NativeMethods.WinUsbQueryInterfaceSettings(this.usbHandle, 0, ref winUsbInterfaceDescriptor))
			{
				throw new IOException("WinUsb Query Interface Settings failed.");
			}
			for (byte b = 0; b < winUsbInterfaceDescriptor.NumEndpoints; b += 1)
			{
				if (!NativeMethods.WinUsbQueryPipe(this.usbHandle, 0, b, ref winUsbPipeInformation))
				{
					throw new IOException(string.Format("WinUsb Query Pipe Information failed", new object[0]));
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
							throw new IOException("Invalid Endpoint Type.");
						}
						this.SetupBulkOutEndpoint(winUsbPipeInformation.PipeId);
					}
				}
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00002D50 File Offset: 0x00000F50
		private bool IsBulkInEndpoint(byte pipeId)
		{
			return (pipeId & 128) == 128;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00002D60 File Offset: 0x00000F60
		private bool IsBulkOutEndpoint(byte pipeId)
		{
			return (pipeId & 128) == 0;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00002D6C File Offset: 0x00000F6C
		private void SetupBulkInEndpoint(byte pipeId)
		{
			this.bulkInPipeId = pipeId;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00002D75 File Offset: 0x00000F75
		private void SetupBulkOutEndpoint(byte pipeId)
		{
			this.bulkOutPipeId = pipeId;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00002D7E File Offset: 0x00000F7E
		private void SetPipePolicy(byte pipeId, uint policyType, uint value)
		{
			if (!NativeMethods.WinUsbSetPipePolicy(this.usbHandle, pipeId, policyType, (uint)Marshal.SizeOf(typeof(uint)), ref value))
			{
				throw new IOException("WinUsb SetPipe Policy failed.");
			}
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00002DAB File Offset: 0x00000FAB
		private void SetPipePolicy(byte pipeId, uint policyType, bool value)
		{
			if (!NativeMethods.WinUsbSetPipePolicy(this.usbHandle, pipeId, policyType, (uint)Marshal.SizeOf(typeof(bool)), ref value))
			{
				throw new IOException("WinUsb SetPipe Policy failed.");
			}
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00002DD8 File Offset: 0x00000FD8
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

		// Token: 0x0600006E RID: 110 RVA: 0x00002E5C File Offset: 0x0000105C
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

		// Token: 0x0600006F RID: 111 RVA: 0x00002EEB File Offset: 0x000010EB
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

		// Token: 0x06000070 RID: 112 RVA: 0x00002F1A File Offset: 0x0000111A
		private bool IsDeviceDisconnected(uint errorCode)
		{
			return errorCode == 2U || errorCode == 1167U;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00002F2C File Offset: 0x0000112C
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

		// Token: 0x06000072 RID: 114 RVA: 0x00002F7C File Offset: 0x0000117C
		~DTSFUsbStream()
		{
			this.Dispose(false);
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00002FAC File Offset: 0x000011AC
		private void Connect()
		{
			this.ControlTransferSetData(UsbControlRequest.Connect, 1);
		}

		// Token: 0x04000095 RID: 149
		private const byte UsbEndpointDirectionMask = 128;

		// Token: 0x04000096 RID: 150
		private string deviceName;

		// Token: 0x04000097 RID: 151
		private SafeFileHandle deviceHandle;

		// Token: 0x04000098 RID: 152
		private IntPtr usbHandle;

		// Token: 0x04000099 RID: 153
		private byte bulkInPipeId;

		// Token: 0x0400009A RID: 154
		private byte bulkOutPipeId;

		// Token: 0x0400009B RID: 155
		private bool isDisposed;

		// Token: 0x0400009C RID: 156
		private const int retryCount = 10;
	}
}
