using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000034 RID: 52
	internal class StorePayload
	{
		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000212 RID: 530 RVA: 0x00009588 File Offset: 0x00007788
		// (set) Token: 0x06000213 RID: 531 RVA: 0x00009590 File Offset: 0x00007790
		public ImageStoreHeader StoreHeader { get; set; }

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000214 RID: 532 RVA: 0x00009599 File Offset: 0x00007799
		// (set) Token: 0x06000215 RID: 533 RVA: 0x000095A1 File Offset: 0x000077A1
		public List<ValidationEntry> ValidationEntries { get; set; }

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000216 RID: 534 RVA: 0x000095AA File Offset: 0x000077AA
		// (set) Token: 0x06000217 RID: 535 RVA: 0x000095B2 File Offset: 0x000077B2
		public List<DataBlockEntry> Phase1DataEntries { get; set; }

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000218 RID: 536 RVA: 0x000095BB File Offset: 0x000077BB
		// (set) Token: 0x06000219 RID: 537 RVA: 0x000095C3 File Offset: 0x000077C3
		public List<DataBlockEntry> Phase2DataEntries { get; set; }

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x0600021A RID: 538 RVA: 0x000095CC File Offset: 0x000077CC
		// (set) Token: 0x0600021B RID: 539 RVA: 0x000095D4 File Offset: 0x000077D4
		public List<DataBlockEntry> Phase3DataEntries { get; set; }

		// Token: 0x0600021C RID: 540 RVA: 0x000095DD File Offset: 0x000077DD
		public StorePayload()
		{
			this.StoreHeader = new ImageStoreHeader();
			this.Phase1DataEntries = new List<DataBlockEntry>();
			this.Phase2DataEntries = new List<DataBlockEntry>();
			this.Phase3DataEntries = new List<DataBlockEntry>();
		}

		// Token: 0x0600021D RID: 541 RVA: 0x00009614 File Offset: 0x00007814
		public List<DataBlockEntry> GetPhaseEntries(StorePayload.BlockPhase phase)
		{
			List<DataBlockEntry> result = null;
			switch (phase)
			{
			case StorePayload.BlockPhase.Phase1:
				result = this.Phase1DataEntries;
				break;
			case StorePayload.BlockPhase.Phase2:
				result = this.Phase2DataEntries;
				break;
			case StorePayload.BlockPhase.Phase3:
				result = this.Phase3DataEntries;
				break;
			}
			return result;
		}

		// Token: 0x0600021E RID: 542 RVA: 0x00009654 File Offset: 0x00007854
		private int GetDescriptorSize()
		{
			int num = 0;
			for (StorePayload.BlockPhase blockPhase = StorePayload.BlockPhase.Phase1; blockPhase != StorePayload.BlockPhase.Invalid; blockPhase++)
			{
				foreach (DataBlockEntry dataBlockEntry in this.GetPhaseEntries(blockPhase))
				{
					num += dataBlockEntry.SizeInBytes;
				}
			}
			return num;
		}

		// Token: 0x0600021F RID: 543 RVA: 0x000096BC File Offset: 0x000078BC
		public int GetMetadataSize()
		{
			return this.StoreHeader.GetStructureSize() + this.GetDescriptorSize();
		}

		// Token: 0x06000220 RID: 544 RVA: 0x000096D0 File Offset: 0x000078D0
		public byte[] GetMetadata(uint alignment)
		{
			MemoryStream memoryStream = new MemoryStream();
			if (this.StoreHeader.StoreDataSizeInBytes == 0U)
			{
				this.StoreHeader.StoreDataSizeInBytes = (uint)this.GetDescriptorSize();
			}
			this.StoreHeader.WriteToStream(memoryStream);
			for (StorePayload.BlockPhase blockPhase = StorePayload.BlockPhase.Phase1; blockPhase != StorePayload.BlockPhase.Invalid; blockPhase++)
			{
				foreach (DataBlockEntry dataBlockEntry in this.GetPhaseEntries(blockPhase))
				{
					dataBlockEntry.WriteEntryToStream(memoryStream);
				}
			}
			long num = memoryStream.Length % (long)((ulong)alignment);
			if (num != 0L)
			{
				MemoryStream memoryStream2 = memoryStream;
				memoryStream2.SetLength(memoryStream2.Length + (long)((ulong)alignment) - num);
			}
			return memoryStream.ToArray();
		}

		// Token: 0x06000221 RID: 545 RVA: 0x00009784 File Offset: 0x00007984
		public void ReadMetadataFromStream(Stream sourceStream)
		{
			this.StoreHeader = ImageStoreHeader.ReadFromStream(sourceStream);
			uint num = this.StoreHeader.InitialPartitionTableBlockIndex + this.StoreHeader.InitialPartitionTableBlockCount;
			uint num2 = this.StoreHeader.FlashOnlyPartitionTableBlockIndex + this.StoreHeader.FlashOnlyPartitionTableBlockCount;
			uint num3 = this.StoreHeader.FinalPartitionTableBlockIndex + this.StoreHeader.FinalPartitionTableBlockCount;
			BinaryReader reader = new BinaryReader(sourceStream);
			uint num4;
			for (num4 = 0U; num4 < num; num4 += 1U)
			{
				DataBlockEntry dataBlockEntry = new DataBlockEntry(this.StoreHeader.BytesPerBlock);
				dataBlockEntry.ReadEntryFromStream(reader, num4);
				this.Phase1DataEntries.Add(dataBlockEntry);
			}
			while (num4 < num2)
			{
				DataBlockEntry dataBlockEntry2 = new DataBlockEntry(this.StoreHeader.BytesPerBlock);
				dataBlockEntry2.ReadEntryFromStream(reader, num4);
				this.Phase2DataEntries.Add(dataBlockEntry2);
				num4 += 1U;
			}
			while (num4 < num3)
			{
				DataBlockEntry dataBlockEntry3 = new DataBlockEntry(this.StoreHeader.BytesPerBlock);
				dataBlockEntry3.ReadEntryFromStream(reader, num4);
				this.Phase3DataEntries.Add(dataBlockEntry3);
				num4 += 1U;
			}
		}

		// Token: 0x06000222 RID: 546 RVA: 0x00009894 File Offset: 0x00007A94
		public void LogInfo(IULogger logger, bool logStoreHeader, bool logDataEntries)
		{
			if (logStoreHeader)
			{
				this.StoreHeader.LogInfo(logger);
			}
			if (logDataEntries)
			{
				for (StorePayload.BlockPhase blockPhase = StorePayload.BlockPhase.Phase1; blockPhase != StorePayload.BlockPhase.Invalid; blockPhase++)
				{
					logger.LogInfo("  {0} entries", new object[]
					{
						blockPhase
					});
					foreach (DataBlockEntry dataBlockEntry in this.GetPhaseEntries(blockPhase))
					{
						dataBlockEntry.LogInfo(logger, false, 4);
					}
					logger.LogInfo("", new object[0]);
				}
			}
		}

		// Token: 0x0200007A RID: 122
		public enum BlockPhase
		{
			// Token: 0x040002B5 RID: 693
			Phase1,
			// Token: 0x040002B6 RID: 694
			Phase2,
			// Token: 0x040002B7 RID: 695
			Phase3,
			// Token: 0x040002B8 RID: 696
			Invalid
		}
	}
}
