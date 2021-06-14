using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using Microsoft.Win32.SafeHandles;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000044 RID: 68
	internal class PayloadReader
	{
		// Token: 0x060002A0 RID: 672 RVA: 0x0000C1F4 File Offset: 0x0000A3F4
		public PayloadReader(FileStream payloadStream)
		{
			this._payloadStream = payloadStream;
			this._payloadOffsets = new List<PayloadReader.PayloadOffset>();
			int num = 1;
			for (int i = 1; i <= num; i++)
			{
				StorePayload storePayload = new StorePayload();
				storePayload.ReadMetadataFromStream(payloadStream);
				long num2 = (long)((ulong)storePayload.StoreHeader.BytesPerBlock - (ulong)(this._payloadStream.Position % (long)((ulong)storePayload.StoreHeader.BytesPerBlock)));
				this._payloadStream.Position += num2;
				this._payloadOffsets.Add(new PayloadReader.PayloadOffset
				{
					Payload = storePayload
				});
				if (storePayload.StoreHeader.MajorVersion >= 2)
				{
					num = (int)storePayload.StoreHeader.NumberOfStores;
				}
			}
			long num3 = this._payloadStream.Position;
			for (int j = 0; j < num; j++)
			{
				PayloadReader.PayloadOffset payloadOffset = this._payloadOffsets[j];
				payloadOffset.Offset = num3;
				ImageStoreHeader storeHeader = payloadOffset.Payload.StoreHeader;
				num3 += (long)((ulong)(storeHeader.BytesPerBlock * storeHeader.StoreDataEntryCount));
			}
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x0000C2F4 File Offset: 0x0000A4F4
		public void WriteToDisk(SafeFileHandle diskHandle, StorePayload payload)
		{
			uint bytesPerBlock = payload.StoreHeader.BytesPerBlock;
			long num = 0L;
			uint num2 = 0U;
			uint num3 = 0U;
			long sectorCount = (long)NativeImaging.GetSectorCount(IntPtr.Zero, diskHandle);
			uint sectorSize = NativeImaging.GetSectorSize(IntPtr.Zero, diskHandle);
			long num4 = sectorCount * (long)((ulong)sectorSize);
			PayloadReader.PayloadOffset payloadOffset = this.FindPayloadOffset(payload);
			if (payloadOffset == null)
			{
				throw new ImageStorageException("Unable to find store payload.");
			}
			this._payloadStream.Position = payloadOffset.Offset;
			SafeFileHandle safeFileHandle = this._payloadStream.SafeFileHandle;
			using (VirtualMemoryPtr virtualMemoryPtr = new VirtualMemoryPtr(bytesPerBlock))
			{
				for (StorePayload.BlockPhase blockPhase = StorePayload.BlockPhase.Phase1; blockPhase != StorePayload.BlockPhase.Invalid; blockPhase++)
				{
					foreach (DataBlockEntry dataBlockEntry in payload.GetPhaseEntries(blockPhase))
					{
						Win32Exports.ReadFile(safeFileHandle, virtualMemoryPtr, bytesPerBlock, out num3);
						for (int i = 0; i < dataBlockEntry.BlockLocationsOnDisk.Count; i++)
						{
							long num5 = (long)((ulong)dataBlockEntry.BlockLocationsOnDisk[i].BlockIndex * (ulong)bytesPerBlock);
							if (dataBlockEntry.BlockLocationsOnDisk[i].AccessMethod == DiskLocation.DiskAccessMethod.DiskEnd)
							{
								num5 = num4 - num5 - (long)((ulong)bytesPerBlock);
							}
							Win32Exports.SetFilePointerEx(diskHandle, num5, out num, Win32Exports.MoveMethod.FILE_BEGIN);
							Win32Exports.WriteFile(diskHandle, virtualMemoryPtr, bytesPerBlock, out num2);
						}
					}
				}
			}
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x0000C46C File Offset: 0x0000A66C
		public DataBlockStream GetDataBlockStream(StorePayload payload, int sectorSize, long totalByteCount)
		{
			PayloadReader.PayloadOffset payloadOffset = this.FindPayloadOffset(payload);
			if (payloadOffset == null)
			{
				throw new ImageStorageException("Unable to find store payload.");
			}
			return new DataBlockStream(new ImagePayloadSource(this._payloadStream, payload, payloadOffset.Offset, totalByteCount), payload.StoreHeader.BytesPerBlock);
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x0000C4B4 File Offset: 0x0000A6B4
		public void ValidatePayloadPartitions(int sectorSize, long totalByteCount, StorePayload payload, uint partitionStyle, bool isMainOSStore, IULogger logger)
		{
			DataBlockStream dataBlockStream = this.GetDataBlockStream(payload, sectorSize, totalByteCount);
			if (partitionStyle == ImageConstants.PartitionTypeGpt)
			{
				GuidPartitionTable guidPartitionTable = new GuidPartitionTable(sectorSize, logger);
				guidPartitionTable.ReadFromStream(dataBlockStream, true);
				if (isMainOSStore && guidPartitionTable.GetEntry(ImageConstants.MAINOS_PARTITION_NAME) == null)
				{
					throw new ImageStorageException(string.Format("{0}: The given FFU does not contain the partition '{1}'.", MethodBase.GetCurrentMethod().Name, ImageConstants.MAINOS_PARTITION_NAME));
				}
			}
			else
			{
				if (partitionStyle != ImageConstants.PartitionTypeMbr)
				{
					throw new ImageStorageException("The payload contains an invalid partition style.");
				}
				MasterBootRecord masterBootRecord = new MasterBootRecord(logger, sectorSize);
				masterBootRecord.ReadFromStream(dataBlockStream, MasterBootRecord.MbrParseType.Normal);
				if (masterBootRecord.FindPartitionByType(ImageConstants.MBR_METADATA_PARTITION_TYPE) == null)
				{
					throw new ImageStorageException(string.Format("{0}: The given FFU does not contain the partition '{1}'.", MethodBase.GetCurrentMethod().Name, ImageConstants.MBR_METADATA_PARTITION_NAME));
				}
				if (masterBootRecord.FindPartitionByName(ImageConstants.MAINOS_PARTITION_NAME) == null)
				{
					throw new ImageStorageException(string.Format("{0}: The given FFU does not contain the partition '{1}'.", MethodBase.GetCurrentMethod().Name, ImageConstants.MAINOS_PARTITION_NAME));
				}
			}
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x0000C598 File Offset: 0x0000A798
		public void LogPayload(IULogger logger, bool logStoreHeader, bool logDataEntries)
		{
			foreach (PayloadReader.PayloadOffset payloadOffset in this._payloadOffsets)
			{
				payloadOffset.Payload.LogInfo(logger, logStoreHeader, logDataEntries);
			}
		}

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x060002A5 RID: 677 RVA: 0x0000C5F0 File Offset: 0x0000A7F0
		public ReadOnlyCollection<StorePayload> Payloads
		{
			get
			{
				List<StorePayload> list = new List<StorePayload>();
				foreach (PayloadReader.PayloadOffset payloadOffset in this._payloadOffsets)
				{
					list.Add(payloadOffset.Payload);
				}
				return list.AsReadOnly();
			}
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x0000C654 File Offset: 0x0000A854
		private PayloadReader.PayloadOffset FindPayloadOffset(StorePayload payload)
		{
			for (int i = 0; i < this._payloadOffsets.Count; i++)
			{
				PayloadReader.PayloadOffset payloadOffset = this._payloadOffsets[i];
				if (payloadOffset.Payload == payload)
				{
					return payloadOffset;
				}
			}
			return null;
		}

		// Token: 0x040001AA RID: 426
		private List<PayloadReader.PayloadOffset> _payloadOffsets;

		// Token: 0x040001AB RID: 427
		private FileStream _payloadStream;

		// Token: 0x0200007E RID: 126
		private class PayloadOffset
		{
			// Token: 0x17000123 RID: 291
			// (get) Token: 0x060004D7 RID: 1239 RVA: 0x00014DF6 File Offset: 0x00012FF6
			// (set) Token: 0x060004D8 RID: 1240 RVA: 0x00014DFE File Offset: 0x00012FFE
			public StorePayload Payload { get; set; }

			// Token: 0x17000124 RID: 292
			// (get) Token: 0x060004D9 RID: 1241 RVA: 0x00014E07 File Offset: 0x00013007
			// (set) Token: 0x060004DA RID: 1242 RVA: 0x00014E0F File Offset: 0x0001300F
			public long Offset { get; set; }
		}
	}
}
