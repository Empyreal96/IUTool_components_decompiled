using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200003A RID: 58
	internal class DiskStreamSource : IBlockStreamSource, IDisposable
	{
		// Token: 0x06000243 RID: 579 RVA: 0x0000ABB8 File Offset: 0x00008DB8
		public DiskStreamSource(SafeFileHandle diskHandle, uint blockSize)
		{
			this._blockSize = blockSize;
			this._handle = diskHandle;
			this._buffer = new VirtualMemoryPtr(blockSize);
			ulong sectorCount = NativeImaging.GetSectorCount(IntPtr.Zero, this._handle);
			uint sectorSize = NativeImaging.GetSectorSize(IntPtr.Zero, this._handle);
			this.Length = (long)(sectorCount * (ulong)sectorSize);
		}

		// Token: 0x06000244 RID: 580 RVA: 0x0000AC14 File Offset: 0x00008E14
		public void ReadBlock(uint blockIndex, byte[] buffer, int bufferIndex)
		{
			uint num = 0U;
			long num2 = 0L;
			Win32Exports.SetFilePointerEx(this._handle, (long)((ulong)blockIndex * (ulong)((long)this._blockSize)), out num2, Win32Exports.MoveMethod.FILE_BEGIN);
			Win32Exports.ReadFile(this._handle, this._buffer, this._blockSize, out num);
			Marshal.Copy(this._buffer.AllocatedPointer, buffer, bufferIndex, (int)this._blockSize);
		}

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06000245 RID: 581 RVA: 0x0000AC74 File Offset: 0x00008E74
		// (set) Token: 0x06000246 RID: 582 RVA: 0x0000AC7C File Offset: 0x00008E7C
		public long Length { get; private set; }

		// Token: 0x06000247 RID: 583 RVA: 0x0000AC88 File Offset: 0x00008E88
		~DiskStreamSource()
		{
			this.Dispose(false);
		}

		// Token: 0x06000248 RID: 584 RVA: 0x0000ACB8 File Offset: 0x00008EB8
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000249 RID: 585 RVA: 0x0000ACC7 File Offset: 0x00008EC7
		protected virtual void Dispose(bool isDisposing)
		{
			if (this._alreadyDisposed)
			{
				return;
			}
			if (isDisposing)
			{
				if (this._handle != null)
				{
					this._handle = null;
				}
				if (this._buffer != null)
				{
					this._buffer.Dispose();
					this._buffer = null;
				}
			}
			this._alreadyDisposed = true;
		}

		// Token: 0x04000172 RID: 370
		private SafeFileHandle _handle;

		// Token: 0x04000173 RID: 371
		private VirtualMemoryPtr _buffer;

		// Token: 0x04000174 RID: 372
		private uint _blockSize;

		// Token: 0x04000176 RID: 374
		private bool _alreadyDisposed;
	}
}
