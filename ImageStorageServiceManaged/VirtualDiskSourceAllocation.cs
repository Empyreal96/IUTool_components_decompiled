using System;
using System.Reflection;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000040 RID: 64
	internal class VirtualDiskSourceAllocation : ISourceAllocation, IDisposable
	{
		// Token: 0x0600027F RID: 639 RVA: 0x0000B690 File Offset: 0x00009890
		public VirtualDiskSourceAllocation(string virtualDiskPath, uint alignmentSize)
		{
			this._virtualDiskPath = virtualDiskPath;
			this._virtualDisk = new DynamicHardDisk(virtualDiskPath, false);
			this._sectorsPerVirtualBlock = this._virtualDisk.BlockSize / this._virtualDisk.SectorSize;
			if (this._virtualDisk.BlockSize % alignmentSize != 0U)
			{
				throw new ImageStorageException(string.Format("{0}: The virtual disk allocation size (0x{1:x}) is not a multiple of the given alignment size (0x{2:x}).", MethodBase.GetCurrentMethod().Name, this._virtualDisk.BlockSize, alignmentSize));
			}
		}

		// Token: 0x06000280 RID: 640 RVA: 0x0000B714 File Offset: 0x00009914
		public bool BlockIsAllocated(ulong diskByteOffset)
		{
			uint index = (uint)(diskByteOffset / (ulong)this._virtualDisk.SectorSize / (ulong)this._sectorsPerVirtualBlock);
			return this._virtualDisk.AllocationTable[index] != uint.MaxValue;
		}

		// Token: 0x06000281 RID: 641 RVA: 0x0000B750 File Offset: 0x00009950
		public uint GetAllocationSize()
		{
			return this._virtualDisk.BlockSize;
		}

		// Token: 0x06000282 RID: 642 RVA: 0x0000B760 File Offset: 0x00009960
		~VirtualDiskSourceAllocation()
		{
			this.Dispose(false);
		}

		// Token: 0x06000283 RID: 643 RVA: 0x0000B790 File Offset: 0x00009990
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000284 RID: 644 RVA: 0x0000B79F File Offset: 0x0000999F
		protected virtual void Dispose(bool isDisposing)
		{
			if (this._alreadyDisposed)
			{
				return;
			}
			if (isDisposing)
			{
				this._virtualDiskPath = null;
				if (this._virtualDisk != null)
				{
					this._virtualDisk.Dispose();
					this._virtualDisk = null;
				}
			}
			this._alreadyDisposed = true;
		}

		// Token: 0x0400018F RID: 399
		private string _virtualDiskPath;

		// Token: 0x04000190 RID: 400
		private DynamicHardDisk _virtualDisk;

		// Token: 0x04000191 RID: 401
		private uint _sectorsPerVirtualBlock;

		// Token: 0x04000192 RID: 402
		private bool _alreadyDisposed;
	}
}
