using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000050 RID: 80
	public sealed class SafeVolumeHandle : SafeHandle
	{
		// Token: 0x060003BB RID: 955 RVA: 0x0001168F File Offset: 0x0000F88F
		public SafeVolumeHandle(ImageStorage storage, string partitionName) : base(IntPtr.Zero, true)
		{
			this._storage = storage;
			this._volumeHandle = storage.OpenVolumeHandle(partitionName);
		}

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x060003BC RID: 956 RVA: 0x000116B1 File Offset: 0x0000F8B1
		public SafeFileHandle VolumeHandle
		{
			get
			{
				return this._volumeHandle;
			}
		}

		// Token: 0x060003BD RID: 957 RVA: 0x000116B9 File Offset: 0x0000F8B9
		public static implicit operator IntPtr(SafeVolumeHandle safeVolumeHandle)
		{
			return safeVolumeHandle._volumeHandle.DangerousGetHandle();
		}

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x060003BE RID: 958 RVA: 0x000116C6 File Offset: 0x0000F8C6
		public override bool IsInvalid
		{
			get
			{
				return this._disposed;
			}
		}

		// Token: 0x060003BF RID: 959 RVA: 0x000116CE File Offset: 0x0000F8CE
		protected override bool ReleaseHandle()
		{
			if (!this._disposed)
			{
				this._disposed = true;
				GC.SuppressFinalize(this);
				if (this._volumeHandle != null)
				{
					this._volumeHandle.Close();
					this._volumeHandle = null;
				}
				this._storage = null;
			}
			return true;
		}

		// Token: 0x040001F8 RID: 504
		private ImageStorage _storage;

		// Token: 0x040001F9 RID: 505
		private SafeFileHandle _volumeHandle;

		// Token: 0x040001FA RID: 506
		private bool _disposed;
	}
}
