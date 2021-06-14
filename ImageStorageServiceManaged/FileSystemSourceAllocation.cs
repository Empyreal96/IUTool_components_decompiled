using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000041 RID: 65
	internal class FileSystemSourceAllocation : ISourceAllocation
	{
		// Token: 0x06000285 RID: 645 RVA: 0x0000B7D5 File Offset: 0x000099D5
		public FileSystemSourceAllocation(ImageStorage imageService, string partitionName, ulong partitionOffset, uint blockSize) : this(imageService, partitionName, partitionOffset, blockSize, false)
		{
		}

		// Token: 0x06000286 RID: 646 RVA: 0x0000B7E4 File Offset: 0x000099E4
		public FileSystemSourceAllocation(ImageStorage imageService, string partitionName, ulong partitionOffset, uint blockSize, bool isDesktopImage)
		{
			this._isDestkopImage = isDesktopImage;
			this._storage = imageService;
			this._blockSize = blockSize;
			this._partitionOffset = partitionOffset;
			this._partitionName = partitionName;
			ulong partitionSize = NativeImaging.GetPartitionSize(this._storage.ServiceHandle, this._storage.StoreId, partitionName);
			uint sectorSize = NativeImaging.GetSectorSize(this._storage.ServiceHandle, this._storage.StoreId);
			ulong num = partitionSize * (ulong)sectorSize;
			if (num / (ulong)sectorSize != partitionSize)
			{
				throw new ImageStorageException(string.Format("Volume {0} is too large to be byte-addressed with a 64-bit value.", partitionName));
			}
			this._blockCount = (uint)((num + (ulong)blockSize - 1UL) / (ulong)blockSize);
			if ((ulong)this._blockCount * (ulong)blockSize < num)
			{
				throw new ImageStorageException(string.Format("Volume {0} is too large to access with a 32-bit block count.", partitionName));
			}
			this._blockAllocationBitmap = new byte[(this._blockCount + 7U) / 8U];
			NativeImaging.GetBlockAllocationBitmap(this._storage.ServiceHandle, this._storage.StoreId, partitionName, this._blockSize, this._blockAllocationBitmap);
		}

		// Token: 0x06000287 RID: 647 RVA: 0x0000B8F0 File Offset: 0x00009AF0
		public uint GetAllocationSize()
		{
			return this._blockSize;
		}

		// Token: 0x06000288 RID: 648 RVA: 0x0000B8F8 File Offset: 0x00009AF8
		public bool BlockIsAllocated(ulong diskByteOffset)
		{
			uint blockIndex = (uint)((diskByteOffset - this._partitionOffset) / (ulong)this._blockSize);
			return this[blockIndex];
		}

		// Token: 0x170000C4 RID: 196
		public bool this[uint blockIndex]
		{
			get
			{
				byte b = (byte)(1 << (int)(blockIndex & 7U));
				return (this._blockAllocationBitmap[(int)(blockIndex / 8U)] & b) > 0;
			}
		}

		// Token: 0x0600028A RID: 650 RVA: 0x0000B948 File Offset: 0x00009B48
		public List<DataBlockEntry> GenerateDataEntries()
		{
			List<DataBlockEntry> list = new List<DataBlockEntry>();
			uint num = (uint)(this._partitionOffset / (ulong)this._blockSize);
			for (uint num2 = 0U; num2 < this._blockCount; num2 += 1U)
			{
				if (this[num2])
				{
					DataBlockEntry dataBlockEntry = new DataBlockEntry(this._blockSize);
					dataBlockEntry.BlockLocationsOnDisk.Add(new DiskLocation(num2 + num));
					DataBlockSource dataSource = dataBlockEntry.DataSource;
					dataSource.Source = DataBlockSource.DataSource.Disk;
					dataSource.StorageOffset = (ulong)(num + num2) * (ulong)this._blockSize;
					list.Add(dataBlockEntry);
				}
			}
			return list;
		}

		// Token: 0x0600028B RID: 651 RVA: 0x0000B9CC File Offset: 0x00009BCC
		[Conditional("DEBUG")]
		public void ValidateDataEntries(List<DataBlockEntry> entries)
		{
			byte[] array = new byte[(this._blockCount + 7U) / 8U];
			Array.Clear(array, 0, (int)((this._blockCount & 7U) / 8U));
			uint num = (uint)(this._partitionOffset / (ulong)this._blockSize);
			foreach (DataBlockEntry dataBlockEntry in entries)
			{
				uint num2 = dataBlockEntry.BlockLocationsOnDisk[0].BlockIndex - num;
				array[(int)(num2 / 8U)] = (byte)((int)array[(int)(num2 / 8U)] | 1 << (int)(num2 % 8U));
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != this._blockAllocationBitmap[i])
				{
					throw new ImageStorageException(string.Format("The block bitmap generated from the volume doesn't match the bitmap generated from the data entries at offset {0}", i));
				}
			}
		}

		// Token: 0x04000193 RID: 403
		private ImageStorage _storage;

		// Token: 0x04000194 RID: 404
		private string _partitionName;

		// Token: 0x04000195 RID: 405
		private uint _blockSize;

		// Token: 0x04000196 RID: 406
		private byte[] _blockAllocationBitmap;

		// Token: 0x04000197 RID: 407
		private uint _blockCount;

		// Token: 0x04000198 RID: 408
		private ulong _partitionOffset;

		// Token: 0x04000199 RID: 409
		private bool _isDestkopImage;

		// Token: 0x0400019A RID: 410
		internal static Guid OriginalSystemPartition = Guid.Empty;
	}
}
