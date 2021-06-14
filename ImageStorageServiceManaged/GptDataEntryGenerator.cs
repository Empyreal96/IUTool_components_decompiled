using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000037 RID: 55
	internal class GptDataEntryGenerator : IEntryGenerator
	{
		// Token: 0x0600022F RID: 559 RVA: 0x00009A40 File Offset: 0x00007C40
		public GptDataEntryGenerator(ImageStorage storage, StorePayload storePayload, ISourceAllocation storageAllocation, SafeFileHandle sourceHandle, uint sourceSectorSize)
		{
			this._payload = storePayload;
			this._storageAllocation = storageAllocation;
			this._sourceHandle = sourceHandle;
			this._image = storage.Image;
			this._sectorSize = sourceSectorSize;
			this._table = new GuidPartitionTable((int)sourceSectorSize, storage.Logger);
			this._finalPartitions = new List<FullFlashUpdateImage.FullFlashUpdatePartition>();
			this._storage = storage;
			if (storePayload.StoreHeader.BytesPerBlock < this._storage.Store.SectorSize)
			{
				throw new ImageStorageException("The data block size is less than the device sector size.");
			}
			if (storePayload.StoreHeader.BytesPerBlock % this._storage.Store.SectorSize != 0U)
			{
				throw new ImageStorageException("The data block size is not a multiple of the device sector size.");
			}
			if (storageAllocation != null)
			{
				if (storePayload.StoreHeader.BytesPerBlock > storageAllocation.GetAllocationSize())
				{
					throw new ImageStorageException("The payload block size is larger than the allocation size of the temp store.");
				}
				if (storageAllocation.GetAllocationSize() % storePayload.StoreHeader.BytesPerBlock != 0U)
				{
					throw new ImageStorageException("The allocation size of the temp store is not a multiple of the payload block size.");
				}
			}
		}

		// Token: 0x06000230 RID: 560 RVA: 0x00009B33 File Offset: 0x00007D33
		public void GenerateEntries(bool onlyAllocateDefinedGptEntries)
		{
			this._payload.Phase1DataEntries = this.GeneratePhase1Entries();
			this._payload.Phase2DataEntries = this.GeneratePhase2Entries(onlyAllocateDefinedGptEntries);
			this._payload.Phase3DataEntries = this.GeneratePhase3Entries(onlyAllocateDefinedGptEntries);
		}

		// Token: 0x06000231 RID: 561 RVA: 0x00009B6C File Offset: 0x00007D6C
		private List<DataBlockEntry> GeneratePhase1Entries()
		{
			List<DataBlockEntry> list = new List<DataBlockEntry>((int)(131072U / this._payload.StoreHeader.BytesPerBlock));
			for (int i = 0; i < list.Capacity; i++)
			{
				list.Add(new DataBlockEntry(this._payload.StoreHeader.BytesPerBlock)
				{
					DataSource = 
					{
						Source = DataBlockSource.DataSource.Zero
					},
					BlockLocationsOnDisk = 
					{
						new DiskLocation((uint)i, DiskLocation.DiskAccessMethod.DiskBegin),
						new DiskLocation((uint)i, DiskLocation.DiskAccessMethod.DiskEnd)
					}
				});
			}
			this._payload.StoreHeader.InitialPartitionTableBlockCount = (uint)list.Count;
			return list;
		}

		// Token: 0x06000232 RID: 562 RVA: 0x00009C0C File Offset: 0x00007E0C
		private List<DataBlockEntry> GeneratePhase2Entries(bool onlyAllocateDefinedGptEntries)
		{
			DataBlockStream dataBlockStream = new DataBlockStream(new DiskStreamSource(this._sourceHandle, this._payload.StoreHeader.BytesPerBlock), this._payload.StoreHeader.BytesPerBlock);
			this._table.ReadFromStream(dataBlockStream, true);
			this._table.ValidateCrcs();
			for (int i = 0; i < this._storage.Store.PartitionCount; i++)
			{
				FullFlashUpdateImage.FullFlashUpdatePartition fullFlashUpdatePartition = this._storage.Store.Partitions[i];
				if (!fullFlashUpdatePartition.RequiredToFlash)
				{
					this._finalPartitions.Add(fullFlashUpdatePartition);
					this._table.RemoveEntry(fullFlashUpdatePartition.Name);
				}
			}
			if (this._storage.IsMainOSStorage)
			{
				this._table.NormalizeGptIds(out FileSystemSourceAllocation.OriginalSystemPartition);
			}
			this._table.FixCrcs();
			this._table.WriteToStream(dataBlockStream, true, onlyAllocateDefinedGptEntries);
			this._phase2PartitionTableEntries = dataBlockStream.BlockEntries;
			List<DataBlockEntry> list = new List<DataBlockEntry>();
			foreach (GuidPartitionTableEntry guidPartitionTableEntry in this._table.Entries)
			{
				if (guidPartitionTableEntry.StartingSector > 0UL)
				{
					this.GenerateDataEntries(list, guidPartitionTableEntry);
				}
			}
			this.FilterPartitionTablesFromDataBlocks(list);
			this._payload.StoreHeader.FlashOnlyPartitionTableBlockCount = (uint)this._phase2PartitionTableEntries.Count;
			this._payload.StoreHeader.FlashOnlyPartitionTableBlockIndex = this._payload.StoreHeader.InitialPartitionTableBlockCount + (uint)list.Count;
			list.AddRange(this._phase2PartitionTableEntries);
			return list;
		}

		// Token: 0x06000233 RID: 563 RVA: 0x00009DB4 File Offset: 0x00007FB4
		private List<DataBlockEntry> GeneratePhase3Entries(bool onlyAllocateDefinedGptEntries)
		{
			List<DataBlockEntry> list = new List<DataBlockEntry>();
			DiskStreamSource streamSource = new DiskStreamSource(this._sourceHandle, this._payload.StoreHeader.BytesPerBlock);
			DataBlockStream dataBlockStream = new DataBlockStream(streamSource, this._payload.StoreHeader.BytesPerBlock);
			this._table.ReadFromStream(dataBlockStream, true);
			this._table.ValidateCrcs();
			while (this._finalPartitions.Count > 0)
			{
				foreach (GuidPartitionTableEntry guidPartitionTableEntry in this._table.Entries)
				{
					if (string.Compare(guidPartitionTableEntry.PartitionName, this._finalPartitions[0].Name, true, CultureInfo.InvariantCulture) == 0)
					{
						this.GenerateDataEntries(list, guidPartitionTableEntry);
						this._finalPartitions.RemoveAt(0);
						break;
					}
				}
			}
			GuidPartitionTableEntry entry = this._table.GetEntry(ImageConstants.MAINOS_PARTITION_NAME);
			if (entry != null)
			{
				entry.Attributes &= ~ImageConstants.GPT_ATTRIBUTE_NO_DRIVE_LETTER;
			}
			if (this._storage.IsMainOSStorage)
			{
				this._table.NormalizeGptIds(out FileSystemSourceAllocation.OriginalSystemPartition);
			}
			this._table.FixCrcs();
			this._table.WriteToStream(dataBlockStream, true, onlyAllocateDefinedGptEntries);
			this.FilterPartitionTablesFromDataBlocks(list);
			this._payload.StoreHeader.FinalPartitionTableBlockCount = (uint)dataBlockStream.BlockEntries.Count;
			this._payload.StoreHeader.FinalPartitionTableBlockIndex = this._payload.StoreHeader.FlashOnlyPartitionTableBlockIndex + this._payload.StoreHeader.FlashOnlyPartitionTableBlockCount + (uint)list.Count;
			list.AddRange(dataBlockStream.BlockEntries);
			dataBlockStream = new DataBlockStream(streamSource, this._payload.StoreHeader.BytesPerBlock);
			this._table.ReadFromStream(dataBlockStream, false);
			List<DataBlockEntry> blockEntries = dataBlockStream.BlockEntries;
			this._payload.StoreHeader.FinalPartitionTableBlockCount += (uint)dataBlockStream.BlockEntries.Count;
			this.ConvertEntriesToUseEndOfDisk(blockEntries, NativeImaging.GetSectorCount(IntPtr.Zero, this._sourceHandle));
			list.AddRange(blockEntries);
			return list;
		}

		// Token: 0x06000234 RID: 564 RVA: 0x00009FD8 File Offset: 0x000081D8
		private void GenerateDataEntries(List<DataBlockEntry> dataEntries, GuidPartitionTableEntry entry)
		{
			List<DataBlockEntry> list;
			if (!this._storage.PartitionIsMountedRaw(entry.PartitionName))
			{
				ulong partitionOffset = NativeImaging.GetPartitionOffset(this._storage.ServiceHandle, this._storage.StoreId, entry.PartitionName) * (ulong)this._sectorSize;
				list = new FileSystemSourceAllocation(this._storage, entry.PartitionName, partitionOffset, this._payload.StoreHeader.BytesPerBlock).GenerateDataEntries();
			}
			else if (!this._storage.IsBackingFileVhdx() || this._storage.IsPartitionTargeted(entry.PartitionName))
			{
				ulong num = entry.LastSector + 1UL - entry.StartingSector;
				num *= (ulong)this._sectorSize;
				list = this.GenerateDataEntriesFromDisk((long)(entry.StartingSector * (ulong)this._sectorSize), (long)num);
				this.FilterUnAllocatedDataEntries(list);
			}
			else
			{
				list = new List<DataBlockEntry>();
			}
			dataEntries.AddRange(list);
		}

		// Token: 0x06000235 RID: 565 RVA: 0x0000A0BC File Offset: 0x000082BC
		private void FilterPartitionTablesFromDataBlocks(List<DataBlockEntry> dataBlocks)
		{
			int bytesPerBlock = (int)this._payload.StoreHeader.BytesPerBlock;
			foreach (DataBlockEntry dataBlockEntry in dataBlocks)
			{
				foreach (DataBlockEntry dataBlockEntry2 in this._phase2PartitionTableEntries)
				{
					if (dataBlockEntry.BlockLocationsOnDisk[0].BlockIndex == dataBlockEntry2.BlockLocationsOnDisk[0].BlockIndex)
					{
						dataBlockEntry.DataSource = new DataBlockSource();
						dataBlockEntry.DataSource.Source = dataBlockEntry2.DataSource.Source;
						dataBlockEntry.DataSource.SetMemoryData(dataBlockEntry2.DataSource.GetMemoryData(), 0, bytesPerBlock);
						break;
					}
				}
			}
		}

		// Token: 0x06000236 RID: 566 RVA: 0x0000A1B8 File Offset: 0x000083B8
		private void FilterUnAllocatedDataEntries(List<DataBlockEntry> dataBlocks)
		{
			uint bytesPerBlock = this._payload.StoreHeader.BytesPerBlock;
			for (int i = 0; i < dataBlocks.Count; i++)
			{
				DataBlockEntry dataBlockEntry = dataBlocks[i];
				if (dataBlockEntry.DataSource.Source == DataBlockSource.DataSource.Disk && this._storageAllocation != null && !this._storageAllocation.BlockIsAllocated(dataBlockEntry.DataSource.StorageOffset))
				{
					dataBlocks.RemoveAt(i--);
				}
			}
		}

		// Token: 0x06000237 RID: 567 RVA: 0x0000A228 File Offset: 0x00008428
		private List<DataBlockEntry> GenerateDataEntriesFromDisk(long diskOffset, long byteCount)
		{
			uint bytesPerBlock = this._payload.StoreHeader.BytesPerBlock;
			List<DataBlockEntry> list = new List<DataBlockEntry>();
			if (diskOffset % (long)((ulong)bytesPerBlock) != 0L)
			{
				throw new ImageStorageException("Parameter 'diskOffset' must be a multiple of the block size.");
			}
			uint num = (uint)((byteCount + (long)((ulong)bytesPerBlock) - 1L) / (long)((ulong)bytesPerBlock));
			uint num2 = (uint)(diskOffset / (long)((ulong)bytesPerBlock));
			for (uint num3 = 0U; num3 < num; num3 += 1U)
			{
				DataBlockEntry dataBlockEntry = new DataBlockEntry(bytesPerBlock);
				dataBlockEntry.BlockLocationsOnDisk.Add(new DiskLocation(num3 + num2));
				DataBlockSource dataSource = dataBlockEntry.DataSource;
				dataSource.Source = DataBlockSource.DataSource.Disk;
				dataSource.StorageOffset = (ulong)(num2 + num3) * (ulong)bytesPerBlock;
				list.Add(dataBlockEntry);
			}
			return list;
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0000A2C0 File Offset: 0x000084C0
		private void ConvertEntriesToUseEndOfDisk(List<DataBlockEntry> entries, ulong totalSectorCount)
		{
			uint num = this._payload.StoreHeader.BytesPerBlock / this._sectorSize;
			if (totalSectorCount / (ulong)num > (ulong)-1)
			{
				throw new ImageStorageException("The image minimum sector count is too large to be addressed with a 32-bit block count.");
			}
			uint num2 = (uint)(totalSectorCount / (ulong)num);
			foreach (DataBlockEntry dataBlockEntry in entries)
			{
				uint blockIndex = dataBlockEntry.BlockLocationsOnDisk[0].BlockIndex;
				uint blockIndex2 = num2 - blockIndex - 1U;
				dataBlockEntry.BlockLocationsOnDisk[0].AccessMethod = DiskLocation.DiskAccessMethod.DiskEnd;
				dataBlockEntry.BlockLocationsOnDisk[0].BlockIndex = blockIndex2;
			}
		}

		// Token: 0x04000160 RID: 352
		private ISourceAllocation _storageAllocation;

		// Token: 0x04000161 RID: 353
		private StorePayload _payload;

		// Token: 0x04000162 RID: 354
		private SafeFileHandle _sourceHandle;

		// Token: 0x04000163 RID: 355
		private GuidPartitionTable _table;

		// Token: 0x04000164 RID: 356
		private FullFlashUpdateImage _image;

		// Token: 0x04000165 RID: 357
		private uint _sectorSize;

		// Token: 0x04000166 RID: 358
		private List<DataBlockEntry> _phase2PartitionTableEntries;

		// Token: 0x04000167 RID: 359
		private List<FullFlashUpdateImage.FullFlashUpdatePartition> _finalPartitions;

		// Token: 0x04000168 RID: 360
		private ImageStorage _storage;
	}
}
