using System;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000038 RID: 56
	internal class MbrDataEntryGenerator : IEntryGenerator
	{
		// Token: 0x06000239 RID: 569 RVA: 0x0000A374 File Offset: 0x00008574
		public MbrDataEntryGenerator(IULogger logger, StorePayload storePayload, ISourceAllocation sourceAllocation, SafeFileHandle sourceHandle, uint sourceSectorSize, FullFlashUpdateImage image)
		{
			this._payload = storePayload;
			this._allocation = sourceAllocation;
			this._sourceHandle = sourceHandle;
			this._image = image;
			this._sectorSize = sourceSectorSize;
			this._table = new MasterBootRecord(logger, (int)sourceSectorSize);
			this._finalPartitions = new List<FullFlashUpdateImage.FullFlashUpdatePartition>();
			this._logger = logger;
			if (storePayload.StoreHeader.BytesPerBlock < image.Stores[0].SectorSize)
			{
				throw new ImageStorageException("The data block size is less than the device sector size.");
			}
			if (storePayload.StoreHeader.BytesPerBlock % image.Stores[0].SectorSize != 0U)
			{
				throw new ImageStorageException("The data block size is not a multiple of the device sector size.");
			}
			if (storePayload.StoreHeader.BytesPerBlock > sourceAllocation.GetAllocationSize())
			{
				throw new ImageStorageException("The payload block size is larger than the allocation size of the temp store.");
			}
			if (sourceAllocation.GetAllocationSize() % storePayload.StoreHeader.BytesPerBlock != 0U)
			{
				throw new ImageStorageException("The allocation size of the temp store is not a multiple of the payload block size.");
			}
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0000A45F File Offset: 0x0000865F
		public void GenerateEntries(bool onlyAllocateDefinedGptEntries)
		{
			this._payload.Phase1DataEntries = this.GeneratePhase1Entries();
			this._payload.Phase2DataEntries = this.GeneratePhase2Entries();
			this._payload.Phase3DataEntries = this.GeneratePhase3Entries();
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0000A494 File Offset: 0x00008694
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
						new DiskLocation((uint)i, DiskLocation.DiskAccessMethod.DiskBegin)
					}
				});
			}
			this._payload.StoreHeader.InitialPartitionTableBlockCount = (uint)list.Count;
			return list;
		}

		// Token: 0x0600023C RID: 572 RVA: 0x0000A520 File Offset: 0x00008720
		private List<DataBlockEntry> GeneratePhase2Entries()
		{
			IBlockStreamSource streamSource = new DiskStreamSource(this._sourceHandle, this._payload.StoreHeader.BytesPerBlock);
			List<DataBlockEntry> list = new List<DataBlockEntry>();
			List<string> list2 = new List<string>();
			using (DataBlockStream dataBlockStream = new DataBlockStream(streamSource, this._payload.StoreHeader.BytesPerBlock))
			{
				this._table.ReadFromStream(dataBlockStream, MasterBootRecord.MbrParseType.Normal);
				for (int i = 0; i < this._image.Stores[0].PartitionCount; i++)
				{
					FullFlashUpdateImage.FullFlashUpdatePartition fullFlashUpdatePartition = this._image.Stores[0].Partitions[i];
					if (fullFlashUpdatePartition.RequiredToFlash)
					{
						list2.Add(fullFlashUpdatePartition.Name);
					}
					else
					{
						this._finalPartitions.Add(fullFlashUpdatePartition);
						this._table.RemovePartition(fullFlashUpdatePartition.Name);
					}
				}
				this._table.DiskSignature = ImageConstants.SYSTEM_STORE_SIGNATURE;
				this._table.WriteToStream(dataBlockStream, false);
				this._phase2PartitionTableEntries = dataBlockStream.BlockEntries;
			}
			foreach (string partitionName in list2)
			{
				MbrPartitionEntry mbrPartitionEntry = this._table.FindPartitionByName(partitionName);
				if (mbrPartitionEntry.AbsoluteStartingSector > 0U && mbrPartitionEntry.StartingSector > 0U)
				{
					ulong byteCount = (ulong)mbrPartitionEntry.SectorCount * (ulong)this._sectorSize;
					List<DataBlockEntry> list3 = this.GenerateDataEntriesFromDisk((long)((ulong)mbrPartitionEntry.AbsoluteStartingSector * (ulong)this._sectorSize), (long)byteCount);
					int count = list3.Count;
					this.FilterUnAllocatedDataEntries(list3);
					this._logger.LogInfo("Recording {0} of {1} blocks from partiton {2} ({3} bytes)", new object[]
					{
						list3.Count,
						count,
						this._table.GetPartitionName(mbrPartitionEntry),
						(long)list3.Count * (long)((ulong)this._payload.StoreHeader.BytesPerBlock)
					});
					list.AddRange(list3);
				}
			}
			this.FilterPartitionTablesFromDataBlocks(list);
			this._payload.StoreHeader.FlashOnlyPartitionTableBlockCount = (uint)this._phase2PartitionTableEntries.Count;
			this._payload.StoreHeader.FlashOnlyPartitionTableBlockIndex = this._payload.StoreHeader.InitialPartitionTableBlockCount + (uint)list.Count;
			list.AddRange(this._phase2PartitionTableEntries);
			return list;
		}

		// Token: 0x0600023D RID: 573 RVA: 0x0000A798 File Offset: 0x00008998
		private List<DataBlockEntry> GeneratePhase3Entries()
		{
			List<DataBlockEntry> list = new List<DataBlockEntry>();
			using (DataBlockStream dataBlockStream = new DataBlockStream(new DiskStreamSource(this._sourceHandle, this._payload.StoreHeader.BytesPerBlock), this._payload.StoreHeader.BytesPerBlock))
			{
				this._table = new MasterBootRecord(this._logger, (int)this._sectorSize);
				this._table.ReadFromStream(dataBlockStream, MasterBootRecord.MbrParseType.Normal);
				this._table.DiskSignature = ImageConstants.SYSTEM_STORE_SIGNATURE;
				this._table.WriteToStream(dataBlockStream, false);
				foreach (FullFlashUpdateImage.FullFlashUpdatePartition fullFlashUpdatePartition in this._finalPartitions)
				{
					MbrPartitionEntry mbrPartitionEntry = this._table.FindPartitionByName(fullFlashUpdatePartition.Name);
					if (mbrPartitionEntry.AbsoluteStartingSector > 0U && mbrPartitionEntry.StartingSector > 0U)
					{
						ulong byteCount = (ulong)mbrPartitionEntry.SectorCount * (ulong)this._sectorSize;
						List<DataBlockEntry> list2 = this.GenerateDataEntriesFromDisk((long)((ulong)mbrPartitionEntry.AbsoluteStartingSector * (ulong)this._sectorSize), (long)byteCount);
						int count = list2.Count;
						this.FilterUnAllocatedDataEntries(list2);
						this._logger.LogInfo("Recording {0} of {1} blocks from partiton {2} ({3} bytes)", new object[]
						{
							list2.Count,
							count,
							fullFlashUpdatePartition.Name,
							(long)list2.Count * (long)((ulong)this._payload.StoreHeader.BytesPerBlock)
						});
						list.AddRange(list2);
					}
				}
				this.FilterPartitionTablesFromDataBlocks(list);
				this._payload.StoreHeader.FinalPartitionTableBlockCount = (uint)dataBlockStream.BlockEntries.Count;
				this._payload.StoreHeader.FinalPartitionTableBlockIndex = this._payload.StoreHeader.FlashOnlyPartitionTableBlockIndex + this._payload.StoreHeader.FlashOnlyPartitionTableBlockCount + (uint)list.Count;
				list.AddRange(dataBlockStream.BlockEntries);
			}
			return list;
		}

		// Token: 0x0600023E RID: 574 RVA: 0x0000A9C8 File Offset: 0x00008BC8
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

		// Token: 0x0600023F RID: 575 RVA: 0x0000AAC4 File Offset: 0x00008CC4
		private void FilterUnAllocatedDataEntries(List<DataBlockEntry> dataBlocks)
		{
			uint bytesPerBlock = this._payload.StoreHeader.BytesPerBlock;
			for (int i = 0; i < dataBlocks.Count; i++)
			{
				DataBlockEntry dataBlockEntry = dataBlocks[i];
				if (dataBlockEntry.DataSource.Source == DataBlockSource.DataSource.Disk && !this._allocation.BlockIsAllocated(dataBlockEntry.DataSource.StorageOffset))
				{
					dataBlocks.RemoveAt(i--);
				}
			}
		}

		// Token: 0x06000240 RID: 576 RVA: 0x0000AB2C File Offset: 0x00008D2C
		private List<DataBlockEntry> GenerateDataEntriesFromDisk(long diskOffset, long byteCount)
		{
			uint bytesPerBlock = this._payload.StoreHeader.BytesPerBlock;
			List<DataBlockEntry> list = new List<DataBlockEntry>();
			uint num = (uint)(byteCount / (long)((ulong)bytesPerBlock));
			if (byteCount % (long)((ulong)bytesPerBlock) != 0L)
			{
				num += 1U;
			}
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

		// Token: 0x04000169 RID: 361
		private ISourceAllocation _allocation;

		// Token: 0x0400016A RID: 362
		private StorePayload _payload;

		// Token: 0x0400016B RID: 363
		private SafeFileHandle _sourceHandle;

		// Token: 0x0400016C RID: 364
		private MasterBootRecord _table;

		// Token: 0x0400016D RID: 365
		private FullFlashUpdateImage _image;

		// Token: 0x0400016E RID: 366
		private uint _sectorSize;

		// Token: 0x0400016F RID: 367
		private List<DataBlockEntry> _phase2PartitionTableEntries;

		// Token: 0x04000170 RID: 368
		private List<FullFlashUpdateImage.FullFlashUpdatePartition> _finalPartitions;

		// Token: 0x04000171 RID: 369
		private IULogger _logger;
	}
}
