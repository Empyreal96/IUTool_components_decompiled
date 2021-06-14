using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200003B RID: 59
	internal class ImagePayloadSource : IBlockStreamSource
	{
		// Token: 0x0600024A RID: 586 RVA: 0x0000AD05 File Offset: 0x00008F05
		public ImagePayloadSource(Stream imageStream, StorePayload payload, long firstDataBlockIndex, long diskSizeInBytes)
		{
			this._payload = payload;
			this._blockSize = payload.StoreHeader.BytesPerBlock;
			this._stream = imageStream;
			this.Length = diskSizeInBytes;
			this._firstDataByte = firstDataBlockIndex;
		}

		// Token: 0x0600024B RID: 587 RVA: 0x0000AD3B File Offset: 0x00008F3B
		private long GetLocationDiskOffset(DiskLocation location)
		{
			if (location.AccessMethod == DiskLocation.DiskAccessMethod.DiskBegin)
			{
				return (long)((ulong)location.BlockIndex * (ulong)this._blockSize);
			}
			return this.Length - (long)((ulong)((location.BlockIndex + 1U) * this._blockSize));
		}

		// Token: 0x0600024C RID: 588 RVA: 0x0000AD6C File Offset: 0x00008F6C
		private long GetDataBlockOffsetForDiskOffset(long diskOffset)
		{
			long num = -1L;
			uint num2 = this._payload.StoreHeader.StoreDataEntryCount - 1U;
			StorePayload.BlockPhase blockPhase = StorePayload.BlockPhase.Invalid;
			do
			{
				blockPhase--;
				List<DataBlockEntry> phaseEntries = this._payload.GetPhaseEntries(blockPhase);
				for (int i = 0; i < phaseEntries.Count; i++)
				{
					List<DataBlockEntry> list = phaseEntries;
					DataBlockEntry dataBlockEntry = list[list.Count - (i + 1)];
					for (int j = 0; j < dataBlockEntry.BlockLocationsOnDisk.Count; j++)
					{
						if (diskOffset == this.GetLocationDiskOffset(dataBlockEntry.BlockLocationsOnDisk[j]))
						{
							num = (long)((ulong)this._blockSize * (ulong)num2);
							break;
						}
					}
					if (num > 0L)
					{
						break;
					}
					num2 -= 1U;
				}
			}
			while (num < 0L && blockPhase != StorePayload.BlockPhase.Phase1);
			return num;
		}

		// Token: 0x0600024D RID: 589 RVA: 0x0000AE20 File Offset: 0x00009020
		public void ReadBlock(uint blockIndex, byte[] buffer, int bufferIndex)
		{
			long num = (long)((ulong)blockIndex * (ulong)this._blockSize);
			if (num > this.Length)
			{
				throw new ImageStorageException("Attempting to read beyond the end of the disk.");
			}
			long dataBlockOffsetForDiskOffset = this.GetDataBlockOffsetForDiskOffset(num);
			if (dataBlockOffsetForDiskOffset == -1L)
			{
				Array.Clear(buffer, bufferIndex, (int)this._blockSize);
				return;
			}
			this._stream.Position = this._firstDataByte + dataBlockOffsetForDiskOffset;
			this._stream.Read(buffer, bufferIndex, (int)this._blockSize);
		}

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x0600024E RID: 590 RVA: 0x0000AE8E File Offset: 0x0000908E
		// (set) Token: 0x0600024F RID: 591 RVA: 0x0000AE96 File Offset: 0x00009096
		public long Length { get; private set; }

		// Token: 0x04000177 RID: 375
		private StorePayload _payload;

		// Token: 0x04000178 RID: 376
		private Stream _stream;

		// Token: 0x04000179 RID: 377
		private long _firstDataByte;

		// Token: 0x0400017A RID: 378
		private uint _blockSize;
	}
}
