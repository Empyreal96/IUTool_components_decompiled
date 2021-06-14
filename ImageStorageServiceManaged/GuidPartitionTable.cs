using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000027 RID: 39
	internal class GuidPartitionTable
	{
		// Token: 0x06000121 RID: 289 RVA: 0x000064A7 File Offset: 0x000046A7
		public GuidPartitionTable(int bytesPerSector, IULogger logger)
		{
			this.Logger = logger;
			this.BytesPerSector = bytesPerSector;
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000122 RID: 290 RVA: 0x000064BD File Offset: 0x000046BD
		// (set) Token: 0x06000123 RID: 291 RVA: 0x000064C5 File Offset: 0x000046C5
		public int BytesPerSector { get; set; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000124 RID: 292 RVA: 0x000064CE File Offset: 0x000046CE
		// (set) Token: 0x06000125 RID: 293 RVA: 0x000064D6 File Offset: 0x000046D6
		public MasterBootRecord ProtectiveMbr { get; private set; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000126 RID: 294 RVA: 0x000064DF File Offset: 0x000046DF
		// (set) Token: 0x06000127 RID: 295 RVA: 0x000064E7 File Offset: 0x000046E7
		public GuidPartitionTableHeader Header { get; private set; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000128 RID: 296 RVA: 0x000064F0 File Offset: 0x000046F0
		// (set) Token: 0x06000129 RID: 297 RVA: 0x000064F8 File Offset: 0x000046F8
		public List<GuidPartitionTableEntry> Entries { get; private set; }

		// Token: 0x0600012A RID: 298 RVA: 0x00006501 File Offset: 0x00004701
		public void ReadFromStream(Stream stream, bool readPrimaryTable)
		{
			this.ReadFromStream(stream, readPrimaryTable, false);
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0000650C File Offset: 0x0000470C
		public void ReadFromStream(Stream stream, bool readPrimaryTable, bool isDesktopImage)
		{
			long position = (long)this.BytesPerSector;
			if (this.BytesPerSector == 0)
			{
				throw new ImageStorageException("BytesPerSector must be initialized before calling GuidPartitionTable.ReadFromStream.");
			}
			if (readPrimaryTable)
			{
				this.ProtectiveMbr = new MasterBootRecord(this.Logger, this.BytesPerSector, isDesktopImage);
				this.ProtectiveMbr.ReadFromStream(stream, MasterBootRecord.MbrParseType.Normal);
			}
			else
			{
				position = stream.Length - (long)this.BytesPerSector;
			}
			stream.Position = position;
			this.Header = new GuidPartitionTableHeader(this.Logger);
			this.Header.ReadFromStream(stream, this.BytesPerSector);
			stream.Position = (long)(this.Header.PartitionEntryStartSector * (ulong)((long)this.BytesPerSector));
			int num = (int)Math.Max(this.Header.PartitionEntryCount, 16384U / this.Header.PartitionEntrySizeInBytes);
			this.Entries = new List<GuidPartitionTableEntry>(num);
			for (int i = 0; i < num; i++)
			{
				GuidPartitionTableEntry guidPartitionTableEntry = new GuidPartitionTableEntry(this.Logger);
				guidPartitionTableEntry.ReadFromStream(stream, (int)this.Header.PartitionEntrySizeInBytes);
				this.Entries.Add(guidPartitionTableEntry);
			}
		}

		// Token: 0x0600012C RID: 300 RVA: 0x00006618 File Offset: 0x00004818
		public void WriteToStream(Stream stream, bool fPrimaryTable, bool onlyAllocateDefinedGptEntries)
		{
			long position = (long)this.BytesPerSector;
			if (fPrimaryTable)
			{
				if (this.ProtectiveMbr == null)
				{
					throw new ImageStorageException("The GuidPartitionTable protective MBR is null.");
				}
				this.ProtectiveMbr.WriteToStream(stream, false);
			}
			else
			{
				position = stream.Length - (long)this.BytesPerSector;
			}
			if (this.Header == null)
			{
				throw new ImageStorageException("The GuidPartitionTable header is null.");
			}
			stream.Position = position;
			if (onlyAllocateDefinedGptEntries)
			{
				this.Header.PartitionEntryCount = 4096U / this.Header.PartitionEntrySizeInBytes;
			}
			else
			{
				this.Header.PartitionEntryCount = Math.Max(this.Header.PartitionEntryCount, 16384U / this.Header.PartitionEntrySizeInBytes);
			}
			this.Header.PartitionEntryArrayCrc32 = this.ComputePartitionEntryCrc(this.Header.PartitionEntryCount);
			this.Header.FixHeaderCrc(this.BytesPerSector);
			this.Header.WriteToStream(stream, this.BytesPerSector);
			stream.Position = (long)(this.Header.PartitionEntryStartSector * (ulong)((long)this.BytesPerSector));
			foreach (GuidPartitionTableEntry guidPartitionTableEntry in this.Entries)
			{
				guidPartitionTableEntry.WriteToStream(stream, (int)this.Header.PartitionEntrySizeInBytes);
			}
		}

		// Token: 0x0600012D RID: 301 RVA: 0x00006770 File Offset: 0x00004970
		public void LogInfo(ushort indentLevel = 0)
		{
			this.ProtectiveMbr.LogInfo(this.Logger, indentLevel);
			this.Header.LogInfo(indentLevel);
			this.Logger.LogInfo("", new object[0]);
			this.Logger.LogInfo("Partition Entry Array", new object[0]);
			foreach (GuidPartitionTableEntry guidPartitionTableEntry in this.Entries)
			{
				guidPartitionTableEntry.LogInfo(indentLevel + 2);
			}
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00006810 File Offset: 0x00004A10
		public Guid SetEntryId(string partitionName, Guid partitionId)
		{
			Guid result = Guid.Empty;
			foreach (GuidPartitionTableEntry guidPartitionTableEntry in this.Entries)
			{
				if (string.CompareOrdinal(guidPartitionTableEntry.PartitionName.Split(new char[1])[0], partitionName) == 0)
				{
					result = guidPartitionTableEntry.PartitionId;
					guidPartitionTableEntry.PartitionId = partitionId;
					break;
				}
			}
			return result;
		}

		// Token: 0x0600012F RID: 303 RVA: 0x00006890 File Offset: 0x00004A90
		public void RemoveEntry(string partitionName)
		{
			foreach (GuidPartitionTableEntry guidPartitionTableEntry in this.Entries)
			{
				if (string.CompareOrdinal(guidPartitionTableEntry.PartitionName.Split(new char[1])[0], partitionName) == 0)
				{
					guidPartitionTableEntry.Clean();
					break;
				}
			}
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00006900 File Offset: 0x00004B00
		public uint ComputePartitionEntryCrc()
		{
			return this.ComputePartitionEntryCrc(this.Header.PartitionEntryCount);
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00006914 File Offset: 0x00004B14
		public uint ComputePartitionEntryCrc(uint partitionEntryCount)
		{
			CRC32 crc = new CRC32();
			MemoryStream memoryStream = new MemoryStream();
			uint num = 0U;
			foreach (GuidPartitionTableEntry guidPartitionTableEntry in this.Entries)
			{
				if ((num += 1U) > partitionEntryCount)
				{
					break;
				}
				guidPartitionTableEntry.WriteToStream(memoryStream, (int)this.Header.PartitionEntrySizeInBytes);
			}
			byte[] array = crc.ComputeHash(memoryStream.GetBuffer());
			return (uint)((int)array[0] << 24 | (int)array[1] << 16 | (int)array[2] << 8 | (int)array[3]);
		}

		// Token: 0x06000132 RID: 306 RVA: 0x000069B4 File Offset: 0x00004BB4
		public void ValidatePartitionEntryCrc()
		{
			uint num = this.ComputePartitionEntryCrc();
			if (this.Header.PartitionEntryArrayCrc32 != num)
			{
				throw new ImageStorageException(string.Format("The partition entry array CRC is invalid.  Actual: {0:x} Expected: {1:x}.", this.Header.PartitionEntryArrayCrc32, num));
			}
		}

		// Token: 0x06000133 RID: 307 RVA: 0x000069FC File Offset: 0x00004BFC
		public void NormalizeGptIds(out Guid originalSystemPartitionId)
		{
			this.Header.DiskId = ImageConstants.SYSTEM_STORE_GUID;
			this.SetEntryId(ImageConstants.MAINOS_PARTITION_NAME, ImageConstants.MAINOS_PARTITION_ID);
			this.SetEntryId(ImageConstants.MMOS_PARTITION_NAME, ImageConstants.MMOS_PARTITION_ID);
			originalSystemPartitionId = this.SetEntryId(ImageConstants.SYSTEM_PARTITION_NAME, ImageConstants.SYSTEM_PARTITION_ID);
		}

		// Token: 0x06000134 RID: 308 RVA: 0x00006A54 File Offset: 0x00004C54
		public void RandomizeGptIds()
		{
			this.Header.DiskId = Guid.NewGuid();
			this.SetEntryId(ImageConstants.MAINOS_PARTITION_NAME, Guid.NewGuid());
			this.SetEntryId(ImageConstants.MMOS_PARTITION_NAME, Guid.NewGuid());
			this.SetEntryId(ImageConstants.SYSTEM_PARTITION_NAME, Guid.NewGuid());
		}

		// Token: 0x06000135 RID: 309 RVA: 0x00006AA4 File Offset: 0x00004CA4
		public void FixCrcs()
		{
			this.Header.PartitionEntryArrayCrc32 = this.ComputePartitionEntryCrc();
			this.Header.FixHeaderCrc(this.BytesPerSector);
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00006AC8 File Offset: 0x00004CC8
		public void ValidateCrcs()
		{
			this.ValidatePartitionEntryCrc();
			this.Header.ValidateHeaderCrc(this.BytesPerSector);
		}

		// Token: 0x06000137 RID: 311 RVA: 0x00006AE4 File Offset: 0x00004CE4
		public GuidPartitionTableEntry GetEntry(string partitionName)
		{
			GuidPartitionTableEntry result = null;
			for (int i = 0; i < this.Entries.Count; i++)
			{
				if (string.Compare(this.Entries[i].PartitionName, partitionName, true, CultureInfo.InvariantCulture) == 0)
				{
					result = this.Entries[i];
					break;
				}
			}
			return result;
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00006B38 File Offset: 0x00004D38
		public static bool IsGuidPartitionStyle(List<DataBlockEntry> blockEntries, int bytesPerSector, int bytesPerBlock)
		{
			int num = bytesPerBlock / bytesPerSector;
			foreach (DataBlockEntry dataBlockEntry in blockEntries)
			{
				for (int i = 0; i < dataBlockEntry.BlockLocationsOnDisk.Count; i++)
				{
					if (dataBlockEntry.BlockLocationsOnDisk[i].AccessMethod == DiskLocation.DiskAccessMethod.DiskBegin && dataBlockEntry.BlockLocationsOnDisk[i].BlockIndex == 0U)
					{
						break;
					}
				}
			}
			return false;
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x0600013A RID: 314 RVA: 0x00006BD1 File Offset: 0x00004DD1
		// (set) Token: 0x06000139 RID: 313 RVA: 0x00006BC8 File Offset: 0x00004DC8
		private IULogger Logger { get; set; }

		// Token: 0x040000F7 RID: 247
		private const int MIN_GPT_PARTITION_ARRAY_SIZE = 16384;
	}
}
