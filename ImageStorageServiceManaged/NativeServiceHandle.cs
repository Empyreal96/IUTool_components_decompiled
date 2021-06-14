using System;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200004D RID: 77
	public sealed class NativeServiceHandle : SafeHandle
	{
		// Token: 0x06000398 RID: 920 RVA: 0x0001113B File Offset: 0x0000F33B
		public NativeServiceHandle(LogFunction logError) : base(IntPtr.Zero, true)
		{
			this._serviceHandle = NativeImaging.CreateImageStorageService(logError);
			if (this._serviceHandle == IntPtr.Zero)
			{
				throw new ImageStorageException("Unable to create the image storage service.");
			}
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x06000399 RID: 921 RVA: 0x00011172 File Offset: 0x0000F372
		public IntPtr ServiceHandle
		{
			get
			{
				return this._serviceHandle;
			}
		}

		// Token: 0x0600039A RID: 922 RVA: 0x00011172 File Offset: 0x0000F372
		public static implicit operator IntPtr(NativeServiceHandle virtualServiceHandle)
		{
			return virtualServiceHandle._serviceHandle;
		}

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x0600039B RID: 923 RVA: 0x0001117A File Offset: 0x0000F37A
		public override bool IsInvalid
		{
			get
			{
				return this._disposed;
			}
		}

		// Token: 0x0600039C RID: 924 RVA: 0x00011182 File Offset: 0x0000F382
		protected override bool ReleaseHandle()
		{
			if (!this._disposed)
			{
				this._disposed = true;
				GC.SuppressFinalize(this);
				if (this._serviceHandle != IntPtr.Zero)
				{
					NativeImaging.CloseImageStorageService(this._serviceHandle);
				}
			}
			return true;
		}

		// Token: 0x040001EE RID: 494
		private readonly IntPtr _serviceHandle;

		// Token: 0x040001EF RID: 495
		private bool _disposed;
	}
}
