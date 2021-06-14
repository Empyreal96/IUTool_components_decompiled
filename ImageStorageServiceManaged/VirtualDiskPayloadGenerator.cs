using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000043 RID: 67
	internal class VirtualDiskPayloadGenerator : IDisposable
	{
		// Token: 0x06000293 RID: 659 RVA: 0x0000BCB0 File Offset: 0x00009EB0
		public VirtualDiskPayloadGenerator(IULogger logger, uint bytesPerBlock, ImageStorage storage, ushort storeHeaderVersion, ushort numOfStores, ushort storeIndex)
		{
			this._sourceHandle = storage.SafeStoreHandle;
			this._blockSize = bytesPerBlock;
			this._storage = storage;
			if (storage.VirtualHardDiskSectorSize == ImageConstants.DefaultVirtualHardDiskSectorSize)
			{
				this._virtualDiskAllocator = new VirtualDiskSourceAllocation(storage.VirtualDiskFilePath, bytesPerBlock);
			}
			else
			{
				this._virtualDiskAllocator = null;
			}
			this._logger = logger;
			this._storeHeaderVersion = storeHeaderVersion;
			this._numOfStores = numOfStores;
			this._storeIndex = storeIndex;
			this._payload = new StorePayload();
		}

		// Token: 0x06000294 RID: 660 RVA: 0x0000BD30 File Offset: 0x00009F30
		public void GenerateStorePayload(ImageStorage storage)
		{
			if (this._storeHeaderVersion == 2)
			{
				this._payload.StoreHeader.Initialize2(FullFlashUpdateType.FullUpdate, this._blockSize, storage.Image, this._numOfStores, this._storeIndex, storage.Store.DevicePath);
			}
			else
			{
				this._payload.StoreHeader.Initialize(FullFlashUpdateType.FullUpdate, this._blockSize, storage.Image);
			}
			this.GenerateDataEntries(this._sourceHandle, storage, storage.VirtualHardDiskSectorSize, this._virtualDiskAllocator);
			this._payload.StoreHeader.StoreDataEntryCount = (uint)(this._payload.Phase1DataEntries.Count + this._payload.Phase2DataEntries.Count + this._payload.Phase3DataEntries.Count);
			if (this._storeHeaderVersion == 2)
			{
				this._payload.StoreHeader.StorePayloadSize = (ulong)this.GetBlockDataSize();
			}
		}

		// Token: 0x06000295 RID: 661 RVA: 0x0000BE14 File Offset: 0x0000A014
		public void WriteMetadata(IPayloadWrapper payloadWrapper)
		{
			byte[] metadata = this._payload.GetMetadata(this._blockSize);
			payloadWrapper.Write(metadata);
		}

		// Token: 0x06000296 RID: 662 RVA: 0x0000BE3C File Offset: 0x0000A03C
		public void WriteStorePayload(IPayloadWrapper payloadWrapper)
		{
			new byte[this._blockSize];
			uint num = 0U;
			using (VirtualMemoryPtr virtualMemoryPtr = new VirtualMemoryPtr(this._payload.StoreHeader.BytesPerBlock))
			{
				for (StorePayload.BlockPhase blockPhase = StorePayload.BlockPhase.Phase1; blockPhase != StorePayload.BlockPhase.Invalid; blockPhase++)
				{
					foreach (DataBlockEntry dataBlockEntry in this._payload.GetPhaseEntries(blockPhase))
					{
						byte[] array = new byte[this._blockSize];
						if (dataBlockEntry.DataSource.Source == DataBlockSource.DataSource.Disk)
						{
							long num2 = 0L;
							Win32Exports.SetFilePointerEx(this._sourceHandle, (long)dataBlockEntry.DataSource.StorageOffset, out num2, Win32Exports.MoveMethod.FILE_BEGIN);
							Win32Exports.ReadFile(this._sourceHandle, virtualMemoryPtr.AllocatedPointer, this._payload.StoreHeader.BytesPerBlock, out num);
							Marshal.Copy(virtualMemoryPtr.AllocatedPointer, array, 0, (int)this._payload.StoreHeader.BytesPerBlock);
						}
						else if (dataBlockEntry.DataSource.Source == DataBlockSource.DataSource.Memory)
						{
							array = dataBlockEntry.DataSource.GetMemoryData();
						}
						this.ReplaceGptDiskId(array, FileSystemSourceAllocation.OriginalSystemPartition, ImageConstants.SYSTEM_PARTITION_ID);
						payloadWrapper.Write(array);
					}
				}
			}
		}

		// Token: 0x06000297 RID: 663 RVA: 0x0000BFB0 File Offset: 0x0000A1B0
		public void Finalize(IPayloadWrapper payloadWrapper)
		{
			payloadWrapper.FinalizeWrapper();
		}

		// Token: 0x06000298 RID: 664 RVA: 0x0000BFB8 File Offset: 0x0000A1B8
		public uint GetPaddingSizeInBytes(long currentSize)
		{
			return this._blockSize - (uint)(currentSize % (long)((ulong)this._blockSize));
		}

		// Token: 0x06000299 RID: 665 RVA: 0x0000BFCC File Offset: 0x0000A1CC
		private void ReplaceGptDiskId(byte[] data, Guid originalPartitionId, Guid newPartitionId)
		{
			if (originalPartitionId != Guid.Empty && newPartitionId != Guid.Empty)
			{
				byte[] array = originalPartitionId.ToByteArray();
				byte[] array2 = newPartitionId.ToByteArray();
				for (int i = 0; i < data.Length - array.Length; i++)
				{
					bool flag = true;
					for (int j = 0; j < array.Length; j++)
					{
						if (data[i + j] != array[j])
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						for (int k = 0; k < array2.Length; k++)
						{
							data[i + k] = array2[k];
						}
						i += array2.Length - 1;
					}
				}
			}
		}

		// Token: 0x0600029A RID: 666 RVA: 0x0000C060 File Offset: 0x0000A260
		private void GenerateDataEntries(SafeFileHandle sourceHandle, ImageStorage storage, uint sourceSectorSize, ISourceAllocation sourceAllocation)
		{
			using (DataBlockStream dataBlockStream = new DataBlockStream(new DiskStreamSource(sourceHandle, this._blockSize), this._blockSize))
			{
				MasterBootRecord masterBootRecord = new MasterBootRecord(this._logger, (int)sourceSectorSize);
				masterBootRecord.ReadFromStream(dataBlockStream, MasterBootRecord.MbrParseType.Normal);
				IEntryGenerator entryGenerator;
				if (masterBootRecord.IsValidProtectiveMbr())
				{
					entryGenerator = new GptDataEntryGenerator(storage, this._payload, sourceAllocation, sourceHandle, sourceSectorSize);
				}
				else
				{
					entryGenerator = new MbrDataEntryGenerator(this._logger, this._payload, sourceAllocation, sourceHandle, sourceSectorSize, storage.Image);
				}
				entryGenerator.GenerateEntries(storage.Store.OnlyAllocateDefinedGptEntries);
			}
		}

		// Token: 0x0600029B RID: 667 RVA: 0x0000C100 File Offset: 0x0000A300
		~VirtualDiskPayloadGenerator()
		{
			this.Dispose(false);
		}

		// Token: 0x0600029C RID: 668 RVA: 0x0000C130 File Offset: 0x0000A330
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x0600029D RID: 669 RVA: 0x0000C13F File Offset: 0x0000A33F
		protected virtual void Dispose(bool isDisposing)
		{
			if (this._alreadyDisposed)
			{
				return;
			}
			if (isDisposing)
			{
				this._payload = null;
				this._sourceHandle = null;
				if (this._virtualDiskAllocator != null)
				{
					this._virtualDiskAllocator.Dispose();
					this._virtualDiskAllocator = null;
				}
			}
			this._alreadyDisposed = true;
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x0600029E RID: 670 RVA: 0x0000C17C File Offset: 0x0000A37C
		internal long TotalSize
		{
			get
			{
				long num = (long)this._payload.GetMetadataSize();
				num += (long)((ulong)this.GetPaddingSizeInBytes(num));
				return num + this.GetBlockDataSize();
			}
		}

		// Token: 0x0600029F RID: 671 RVA: 0x0000C1AC File Offset: 0x0000A3AC
		private long GetBlockDataSize()
		{
			long num = 0L;
			for (StorePayload.BlockPhase blockPhase = StorePayload.BlockPhase.Phase1; blockPhase != StorePayload.BlockPhase.Invalid; blockPhase++)
			{
				List<DataBlockEntry> phaseEntries = this._payload.GetPhaseEntries(blockPhase);
				num += (long)((ulong)this._payload.StoreHeader.BytesPerBlock * (ulong)((long)phaseEntries.Count));
			}
			return num;
		}

		// Token: 0x040001A0 RID: 416
		private ImageStorage _storage;

		// Token: 0x040001A1 RID: 417
		private StorePayload _payload;

		// Token: 0x040001A2 RID: 418
		private VirtualDiskSourceAllocation _virtualDiskAllocator;

		// Token: 0x040001A3 RID: 419
		private SafeFileHandle _sourceHandle;

		// Token: 0x040001A4 RID: 420
		private uint _blockSize;

		// Token: 0x040001A5 RID: 421
		private IULogger _logger;

		// Token: 0x040001A6 RID: 422
		private ushort _storeHeaderVersion;

		// Token: 0x040001A7 RID: 423
		private ushort _numOfStores;

		// Token: 0x040001A8 RID: 424
		private ushort _storeIndex;

		// Token: 0x040001A9 RID: 425
		private bool _alreadyDisposed;
	}
}
