using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200003D RID: 61
	internal class DiskStream : Stream, IDisposable
	{
		// Token: 0x06000266 RID: 614 RVA: 0x0000B298 File Offset: 0x00009498
		~DiskStream()
		{
			this.Dispose(false);
		}

		// Token: 0x06000267 RID: 615 RVA: 0x0000B2C8 File Offset: 0x000094C8
		protected override void Dispose(bool isDisposing)
		{
			if (this._alreadyDisposed)
			{
				return;
			}
			if (isDisposing)
			{
				if (this._diskHandle != null)
				{
					if (this._ownsDiskHandle)
					{
						this._diskHandle.Dispose();
					}
					this._diskHandle = null;
				}
				if (this._buffer != null)
				{
					this._buffer.Dispose();
					this._buffer = null;
				}
			}
			this._alreadyDisposed = true;
		}

		// Token: 0x06000268 RID: 616 RVA: 0x0000B324 File Offset: 0x00009524
		private void Initialize(SafeFileHandle diskHandle, bool ownsDiskHandle, bool canRead, bool canWrite)
		{
			this._ownsDiskHandle = ownsDiskHandle;
			this._diskHandle = diskHandle;
			this._canRead = canRead;
			this._canWrite = canWrite;
			long num = 0L;
			Win32Exports.SetFilePointerEx(diskHandle, 0L, out num, Win32Exports.MoveMethod.FILE_BEGIN);
			this._sectorCount = NativeImaging.GetSectorCount(IntPtr.Zero, this._diskHandle);
			this._sectorSize = NativeImaging.GetSectorSize(IntPtr.Zero, this._diskHandle);
			this._sizeInBytes = this._sectorCount * (ulong)this._sectorSize;
			this._buffer = new VirtualMemoryPtr(65536U);
		}

		// Token: 0x06000269 RID: 617 RVA: 0x0000B3AC File Offset: 0x000095AC
		public DiskStream(SafeFileHandle diskHandle, bool canRead, bool canWrite)
		{
			this.Initialize(diskHandle, false, canRead, canWrite);
		}

		// Token: 0x0600026A RID: 618 RVA: 0x0000B3C8 File Offset: 0x000095C8
		public DiskStream(string diskPath, Win32Exports.DesiredAccess desiredAccess, Win32Exports.ShareMode shareMode)
		{
			SafeFileHandle diskHandle = Win32Exports.CreateFile(diskPath, desiredAccess, shareMode, Win32Exports.CreationDisposition.OPEN_EXISTING, Win32Exports.FlagsAndAttributes.FILE_ATTRIBUTE_NORMAL);
			this.Initialize(diskHandle, true, (desiredAccess | (Win32Exports.DesiredAccess)2147483648U) > (Win32Exports.DesiredAccess)0U, (desiredAccess | Win32Exports.DesiredAccess.GENERIC_WRITE) > (Win32Exports.DesiredAccess)0U);
		}

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x0600026B RID: 619 RVA: 0x0000B40E File Offset: 0x0000960E
		public override bool CanRead
		{
			get
			{
				return this._canRead;
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x0600026C RID: 620 RVA: 0x0000B416 File Offset: 0x00009616
		public override bool CanWrite
		{
			get
			{
				return this._canWrite;
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x0600026D RID: 621 RVA: 0x00005269 File Offset: 0x00003469
		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x0600026E RID: 622 RVA: 0x0000B41E File Offset: 0x0000961E
		public override long Length
		{
			get
			{
				return (long)(this._sectorCount * (ulong)this._sectorSize);
			}
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x0600026F RID: 623 RVA: 0x0000B42E File Offset: 0x0000962E
		// (set) Token: 0x06000270 RID: 624 RVA: 0x0000B436 File Offset: 0x00009636
		public override long Position
		{
			get
			{
				return this._position;
			}
			set
			{
				if (value > (long)this._sizeInBytes)
				{
					throw new ImageStorageException("The specified osition is beyond the end of the disk.");
				}
				this._position = value;
			}
		}

		// Token: 0x06000271 RID: 625 RVA: 0x0000B453 File Offset: 0x00009653
		public override void Flush()
		{
			Win32Exports.FlushFileBuffers(this._diskHandle);
		}

		// Token: 0x06000272 RID: 626 RVA: 0x0000B460 File Offset: 0x00009660
		private void FillBuffer(uint sectorIndex)
		{
			ulong num = (ulong)(sectorIndex * this._sectorSize);
			uint bytesToRead = 65536U;
			uint num2 = 0U;
			if (this._sizeInBytes - num < 65536UL)
			{
				Marshal.Copy(new byte[65536], 0, this._buffer, 65536);
				bytesToRead = (uint)(this._sizeInBytes - num);
			}
			long num3;
			Win32Exports.SetFilePointerEx(this._diskHandle, (long)num, out num3, Win32Exports.MoveMethod.FILE_BEGIN);
			Win32Exports.ReadFile(this._diskHandle, this._buffer, bytesToRead, out num2);
			this._bufferOffsetOnDisk = num;
		}

		// Token: 0x06000273 RID: 627 RVA: 0x0000B4E8 File Offset: 0x000096E8
		private bool OffsetIsInBuffer(ulong diskOffset)
		{
			return diskOffset >= this._bufferOffsetOnDisk && diskOffset - this._bufferOffsetOnDisk <= 65536UL;
		}

		// Token: 0x06000274 RID: 628 RVA: 0x0000B508 File Offset: 0x00009708
		private uint BytesInBuffer(ulong diskOffset)
		{
			if (!this.OffsetIsInBuffer(diskOffset))
			{
				throw new ImageStorageException("Attempt to copy from outside the buffer range.");
			}
			return (uint)(65536UL - (diskOffset - this._bufferOffsetOnDisk));
		}

		// Token: 0x06000275 RID: 629 RVA: 0x0000B530 File Offset: 0x00009730
		private void CopyFromBuffer(byte[] destination, int destinationOffset, int count, ulong diskOffset, out uint bytesCopied)
		{
			if (!this.OffsetIsInBuffer(diskOffset))
			{
				throw new ImageStorageException("Attempt to copy from outside the buffer range.");
			}
			uint num = Math.Min(this.BytesInBuffer(diskOffset), (uint)count);
			uint cbSize = (uint)(diskOffset - this._bufferOffsetOnDisk);
			Marshal.Copy(this._buffer.Increment((int)cbSize), destination, destinationOffset, (int)num);
			bytesCopied = num;
		}

		// Token: 0x06000276 RID: 630 RVA: 0x0000B58C File Offset: 0x0000978C
		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = 0;
			if (this._sizeInBytes - (ulong)count < (ulong)offset)
			{
				throw new ImageStorageException("Attempt to read beyond end of the disk.");
			}
			while (count > 0)
			{
				if (!this.OffsetIsInBuffer((ulong)this._position))
				{
					this.FillBuffer((uint)(this._position / (long)((ulong)this._sectorSize)));
				}
				uint num2 = 0U;
				this.CopyFromBuffer(buffer, offset + num, count, (ulong)this._position, out num2);
				num += (int)num2;
				count -= (int)num2;
				this._position += (long)((ulong)num2);
			}
			return num;
		}

		// Token: 0x06000277 RID: 631 RVA: 0x0000B608 File Offset: 0x00009808
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new ImageStorageException("This operation is not supported.");
		}

		// Token: 0x06000278 RID: 632 RVA: 0x0000B614 File Offset: 0x00009814
		public override void SetLength(long value)
		{
			throw new ImageStorageException("Cannot set the length of a disk stream.");
		}

		// Token: 0x06000279 RID: 633 RVA: 0x0000B620 File Offset: 0x00009820
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new ImageStorageException("This operation is not implemented.");
		}

		// Token: 0x04000183 RID: 387
		private SafeFileHandle _diskHandle;

		// Token: 0x04000184 RID: 388
		private VirtualMemoryPtr _buffer;

		// Token: 0x04000185 RID: 389
		private ulong _bufferOffsetOnDisk = ulong.MaxValue;

		// Token: 0x04000186 RID: 390
		private bool _ownsDiskHandle;

		// Token: 0x04000187 RID: 391
		private bool _canRead;

		// Token: 0x04000188 RID: 392
		private bool _canWrite;

		// Token: 0x04000189 RID: 393
		private ulong _sectorCount;

		// Token: 0x0400018A RID: 394
		private uint _sectorSize;

		// Token: 0x0400018B RID: 395
		private ulong _sizeInBytes;

		// Token: 0x0400018C RID: 396
		private long _position;

		// Token: 0x0400018D RID: 397
		private const uint BUFFER_SIZE = 65536U;

		// Token: 0x0400018E RID: 398
		private bool _alreadyDisposed;
	}
}
