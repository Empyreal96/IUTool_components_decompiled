using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200002A RID: 42
	internal class MasterBootRecord
	{
		// Token: 0x06000170 RID: 368 RVA: 0x00007525 File Offset: 0x00005725
		private MasterBootRecord()
		{
		}

		// Token: 0x06000171 RID: 369 RVA: 0x00007564 File Offset: 0x00005764
		private MasterBootRecord(MasterBootRecord primaryRecord)
		{
			this._primaryRecord = primaryRecord;
			this._bytesPerSector = this._primaryRecord._bytesPerSector;
			this._logger = this._primaryRecord._logger;
		}

		// Token: 0x06000172 RID: 370 RVA: 0x000075D6 File Offset: 0x000057D6
		public MasterBootRecord(IULogger logger, int bytesPerSector) : this(logger, bytesPerSector, false)
		{
		}

		// Token: 0x06000173 RID: 371 RVA: 0x000075E4 File Offset: 0x000057E4
		public MasterBootRecord(IULogger logger, int bytesPerSector, bool isDesktopImage)
		{
			this._isDesktopImage = isDesktopImage;
			this._logger = logger;
			this._bytesPerSector = bytesPerSector;
			this._metadataPartition = new MasterBootRecordMetadataPartition(logger);
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000174 RID: 372 RVA: 0x0000764E File Offset: 0x0000584E
		// (set) Token: 0x06000175 RID: 373 RVA: 0x00007656 File Offset: 0x00005856
		public uint DiskSignature { get; set; }

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000176 RID: 374 RVA: 0x0000765F File Offset: 0x0000585F
		// (set) Token: 0x06000177 RID: 375 RVA: 0x00007667 File Offset: 0x00005867
		public uint DiskSectorCount { get; set; }

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000178 RID: 376 RVA: 0x00007670 File Offset: 0x00005870
		public List<MbrPartitionEntry> PartitionEntries
		{
			get
			{
				return this._entries;
			}
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000179 RID: 377 RVA: 0x00007678 File Offset: 0x00005878
		public MasterBootRecord ExtendedRecord
		{
			get
			{
				return this._extendedRecord;
			}
		}

		// Token: 0x0600017A RID: 378 RVA: 0x00007680 File Offset: 0x00005880
		public bool ReadFromStream(Stream stream, MasterBootRecord.MbrParseType parseType)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			bool flag = true;
			this._sectorIndex = (uint)(stream.Position / (long)this._bytesPerSector);
			stream.Read(this._codeData, 0, this._codeData.Length);
			this.DiskSignature = binaryReader.ReadUInt32();
			stream.Position += 2L;
			for (int i = 0; i < 4; i++)
			{
				MbrPartitionEntry mbrPartitionEntry = new MbrPartitionEntry();
				mbrPartitionEntry.ReadFromStream(binaryReader);
				if (this.IsExtendedBootRecord())
				{
					mbrPartitionEntry.StartingSectorOffset = this._sectorIndex;
				}
				if (mbrPartitionEntry.TypeIsContainer && parseType == MasterBootRecord.MbrParseType.TruncateAllExtendedRecords)
				{
					mbrPartitionEntry.ZeroData();
				}
				if (mbrPartitionEntry.TypeIsContainer)
				{
					if (this._extendedEntry != null)
					{
						this._logger.LogWarning("{0}: The extended boot record at sector 0x{1:x} contains multiple extended boot records.", new object[]
						{
							MethodBase.GetCurrentMethod().Name,
							this._sectorIndex
						});
						if (this.IsExtendedBootRecord() && parseType == MasterBootRecord.MbrParseType.TruncateInvalidExtendedRecords)
						{
							flag = false;
							break;
						}
						throw new ImageStorageException("There are multiple extended partition entries.");
					}
					else
					{
						long num;
						if (!this.IsExtendedBootRecord())
						{
							num = (long)((ulong)mbrPartitionEntry.StartingSector * (ulong)((long)this._bytesPerSector));
						}
						else
						{
							num = (long)((ulong)(mbrPartitionEntry.StartingSector + this._primaryRecord._extendedEntry.StartingSector) * (ulong)((long)this._bytesPerSector));
						}
						if (mbrPartitionEntry.SectorCount == 0U || mbrPartitionEntry.StartingSector == 0U)
						{
							this._logger.LogWarning("{0}: The boot record at sector 0x{1:x} has an entry with a extended partition type, but the start sector or size is 0.", new object[]
							{
								MethodBase.GetCurrentMethod().Name,
								this._sectorIndex
							});
							mbrPartitionEntry.PartitionType = 0;
						}
						else if (num > stream.Length)
						{
							if (parseType != MasterBootRecord.MbrParseType.TruncateInvalidExtendedRecords)
							{
								throw new ImageStorageException("There are multiple extended partition entries.");
							}
							this._logger.LogDebug("{0}: The extended boot entry at sector 0x{1:x} points beyond the end of the stream.", new object[]
							{
								MethodBase.GetCurrentMethod().Name,
								this._sectorIndex
							});
							if (this.IsExtendedBootRecord())
							{
								flag = false;
								break;
							}
							mbrPartitionEntry.ZeroData();
						}
						else
						{
							this._extendedEntry = mbrPartitionEntry;
						}
					}
				}
				this._entries.Add(mbrPartitionEntry);
			}
			if (!this._isDesktopImage && binaryReader.ReadUInt16() != 43605)
			{
				if (!this.IsExtendedBootRecord() || parseType != MasterBootRecord.MbrParseType.TruncateInvalidExtendedRecords)
				{
					throw new ImageStorageException("The MBR disk signature is invalid.");
				}
				this._logger.LogDebug("{0}: The extended boot record at sector 0x{1:x} has an invalid MBR signature.", new object[]
				{
					MethodBase.GetCurrentMethod().Name,
					this._sectorIndex
				});
				flag = false;
			}
			if (stream.Position % (long)this._bytesPerSector != 0L)
			{
				stream.Position += (long)this._bytesPerSector - stream.Position % (long)this._bytesPerSector;
			}
			if (flag && !this.ReadExtendedPartitions(stream, parseType))
			{
				this._extendedRecord = null;
				this._extendedEntry.ZeroData();
			}
			this.ReadMetadataPartition(stream);
			return flag;
		}

		// Token: 0x0600017B RID: 379 RVA: 0x0000793C File Offset: 0x00005B3C
		private bool ReadExtendedPartitions(Stream stream, MasterBootRecord.MbrParseType parseType)
		{
			long position = stream.Position;
			bool result = true;
			if (this._extendedEntry != null)
			{
				if (!this.IsExtendedBootRecord())
				{
					stream.Position = (long)((ulong)this._extendedEntry.StartingSector * (ulong)((long)this._bytesPerSector));
				}
				else
				{
					stream.Position = (long)((ulong)(this._extendedEntry.StartingSector + this._primaryRecord._extendedEntry.StartingSector) * (ulong)((long)this._bytesPerSector));
				}
				this._extendedRecord = new MasterBootRecord((this._primaryRecord == null) ? this : this._primaryRecord);
				result = this._extendedRecord.ReadFromStream(stream, parseType);
			}
			return result;
		}

		// Token: 0x0600017C RID: 380 RVA: 0x000079D4 File Offset: 0x00005BD4
		public void WriteToStream(Stream stream, bool addCodeData)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			stream.Position = (long)((ulong)this._sectorIndex * (ulong)((long)this._bytesPerSector));
			if (!addCodeData)
			{
				stream.Write(this._codeData, 0, 440);
			}
			else
			{
				stream.Write(this.CodeData, 0, this.CodeData.Length);
			}
			binaryWriter.Write(this.DiskSignature);
			stream.WriteByte(0);
			stream.WriteByte(0);
			foreach (MbrPartitionEntry mbrPartitionEntry in this._entries)
			{
				mbrPartitionEntry.WriteToStream(binaryWriter);
			}
			binaryWriter.Write(43605);
			if (stream.Position % (long)this._bytesPerSector != 0L)
			{
				stream.Position += (long)this._bytesPerSector - stream.Position % (long)this._bytesPerSector;
			}
			if (this._extendedRecord != null)
			{
				this._extendedRecord.WriteToStream(stream, false);
			}
		}

		// Token: 0x0600017D RID: 381 RVA: 0x00007ADC File Offset: 0x00005CDC
		public void LogInfo(IULogger logger, ushort indentLevel = 0)
		{
			string str = new StringBuilder().Append(' ', (int)indentLevel).ToString();
			if (this.IsValidProtectiveMbr())
			{
				logger.LogInfo(str + "Protective Master Boot Record", new object[0]);
			}
			else if (this._primaryRecord == null)
			{
				logger.LogInfo(str + "Master Boot Record", new object[0]);
				logger.LogInfo(str + "  Disk Signature: 0x{0:x}", new object[]
				{
					this.DiskSignature
				});
			}
			else
			{
				logger.LogInfo(str + "Extended Boot Record", new object[0]);
			}
			logger.LogInfo("", new object[0]);
			foreach (MbrPartitionEntry mbrPartitionEntry in this._entries)
			{
				mbrPartitionEntry.LogInfo(logger, this, indentLevel + 2);
			}
			if (this._extendedRecord != null)
			{
				if (!this.IsExtendedBootRecord())
				{
					indentLevel += 2;
				}
				this._extendedRecord.LogInfo(logger, indentLevel);
			}
		}

		// Token: 0x0600017E RID: 382 RVA: 0x00007BF8 File Offset: 0x00005DF8
		public bool IsValidProtectiveMbr()
		{
			for (int i = 0; i < this._entries.Count; i++)
			{
				MbrPartitionEntry mbrPartitionEntry = this._entries[i];
				if (i == 0)
				{
					if (mbrPartitionEntry.StartingSector != 1U)
					{
						return false;
					}
					if (mbrPartitionEntry.SectorCount == 0U)
					{
						return false;
					}
					if (mbrPartitionEntry.PartitionType != 238)
					{
						return false;
					}
				}
				else
				{
					if (mbrPartitionEntry.SectorCount != 0U)
					{
						return false;
					}
					if (mbrPartitionEntry.StartingSector != 0U)
					{
						return false;
					}
					if (mbrPartitionEntry.PartitionType != 0)
					{
						return false;
					}
				}
				if (mbrPartitionEntry.Bootable)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600017F RID: 383 RVA: 0x00007C78 File Offset: 0x00005E78
		public bool IsExtendedBootRecord()
		{
			return this._primaryRecord != null;
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00007C84 File Offset: 0x00005E84
		public MbrPartitionEntry FindPartitionByType(byte partitionType)
		{
			foreach (MbrPartitionEntry mbrPartitionEntry in this._entries)
			{
				if (mbrPartitionEntry.PartitionType == partitionType)
				{
					return mbrPartitionEntry;
				}
			}
			if (this._extendedRecord != null)
			{
				return this._extendedRecord.FindPartitionByType(partitionType);
			}
			return null;
		}

		// Token: 0x06000181 RID: 385 RVA: 0x00007CF8 File Offset: 0x00005EF8
		public ulong FindPartitionOffset(string partitionName)
		{
			ulong result = 0UL;
			if (this._metadataPartition != null)
			{
				foreach (MetadataPartitionEntry metadataPartitionEntry in this._metadataPartition.Entries)
				{
					if (string.Compare(metadataPartitionEntry.Name, partitionName, true, CultureInfo.InvariantCulture) == 0)
					{
						result = metadataPartitionEntry.DiskOffset;
						break;
					}
				}
			}
			return result;
		}

		// Token: 0x06000182 RID: 386 RVA: 0x00007D74 File Offset: 0x00005F74
		public MbrPartitionEntry FindPartitionByName(string partitionName)
		{
			ulong num = this.FindPartitionOffset(partitionName);
			if (num > 0UL)
			{
				return this.FindPartitionByName(partitionName, num);
			}
			return null;
		}

		// Token: 0x06000183 RID: 387 RVA: 0x00007D98 File Offset: 0x00005F98
		private MbrPartitionEntry FindPartitionByName(string partitionName, ulong diskOffset)
		{
			foreach (MbrPartitionEntry mbrPartitionEntry in this._entries)
			{
				if ((ulong)mbrPartitionEntry.AbsoluteStartingSector * (ulong)((long)this._bytesPerSector) == diskOffset)
				{
					return mbrPartitionEntry;
				}
			}
			if (this._extendedRecord != null)
			{
				return this._extendedRecord.FindPartitionByName(partitionName, diskOffset);
			}
			return null;
		}

		// Token: 0x06000184 RID: 388 RVA: 0x00007E14 File Offset: 0x00006014
		public string GetPartitionName(MbrPartitionEntry entry)
		{
			ulong num = (ulong)entry.AbsoluteStartingSector * (ulong)((long)this._bytesPerSector);
			MasterBootRecordMetadataPartition metadataPartition = this._metadataPartition;
			if (this.IsExtendedBootRecord())
			{
				metadataPartition = this._primaryRecord._metadataPartition;
			}
			if (metadataPartition != null)
			{
				foreach (MetadataPartitionEntry metadataPartitionEntry in metadataPartition.Entries)
				{
					if (metadataPartitionEntry.DiskOffset == num)
					{
						return metadataPartitionEntry.Name;
					}
				}
			}
			return string.Empty;
		}

		// Token: 0x06000185 RID: 389 RVA: 0x00007EAC File Offset: 0x000060AC
		public long GetMetadataPartitionOffset()
		{
			long result = 0L;
			MbrPartitionEntry mbrPartitionEntry = this.FindPartitionByType(MasterBootRecordMetadataPartition.PartitonType);
			if (mbrPartitionEntry != null)
			{
				result = (long)((ulong)mbrPartitionEntry.AbsoluteStartingSector * (ulong)((long)this._bytesPerSector));
			}
			return result;
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00007EDC File Offset: 0x000060DC
		public void ReadMetadataPartition(Stream stream)
		{
			if (this._primaryRecord == null && !this.IsValidProtectiveMbr())
			{
				long metadataPartitionOffset = this.GetMetadataPartitionOffset();
				long position = stream.Position;
				if (metadataPartitionOffset > 0L)
				{
					stream.Position = metadataPartitionOffset;
					this._metadataPartition = new MasterBootRecordMetadataPartition(this._logger);
					this._metadataPartition.ReadFromStream(stream);
				}
				stream.Position = position;
			}
		}

		// Token: 0x06000187 RID: 391 RVA: 0x00007F37 File Offset: 0x00006137
		public byte[] GetCodeData()
		{
			return this._codeData;
		}

		// Token: 0x06000188 RID: 392 RVA: 0x00007F40 File Offset: 0x00006140
		public void RemovePartition(string partitionName)
		{
			ulong num = this.FindPartitionOffset(partitionName);
			if (num == 0UL)
			{
				throw new ImageStorageException(string.Format("Partition {0} was not found in the MBR metadata partition.", partitionName));
			}
			this.RemovePartition(partitionName, num);
		}

		// Token: 0x06000189 RID: 393 RVA: 0x00007F74 File Offset: 0x00006174
		private void RemovePartition(string partitionName, ulong partitionOffset)
		{
			bool flag = true;
			foreach (MbrPartitionEntry mbrPartitionEntry in this._entries)
			{
				if ((ulong)mbrPartitionEntry.AbsoluteStartingSector * (ulong)((long)this._bytesPerSector) == partitionOffset)
				{
					mbrPartitionEntry.ZeroData();
					flag = false;
					break;
				}
			}
			if (flag)
			{
				if (this._extendedRecord == null)
				{
					throw new ImageStorageException(string.Format("Partition {0} was in the MBR metadata partition, but the boot record is not found.", partitionName));
				}
				this._extendedRecord.RemovePartition(partitionName, partitionOffset);
			}
		}

		// Token: 0x04000114 RID: 276
		public const ushort Signature = 43605;

		// Token: 0x04000115 RID: 277
		public const int FirstEntryByteOffset = 446;

		// Token: 0x04000116 RID: 278
		public const ushort CodeAreaSize = 440;

		// Token: 0x04000117 RID: 279
		private readonly byte[] CodeData = new byte[]
		{
			51,
			192,
			142,
			208,
			188,
			0,
			124,
			142,
			192,
			142,
			216,
			190,
			0,
			124,
			191,
			0,
			6,
			185,
			0,
			2,
			252,
			243,
			164,
			80,
			104,
			28,
			6,
			203,
			251,
			185,
			4,
			0,
			189,
			190,
			7,
			128,
			126,
			0,
			0,
			124,
			11,
			15,
			133,
			14,
			1,
			131,
			197,
			16,
			226,
			241,
			205,
			24,
			136,
			86,
			0,
			85,
			198,
			70,
			17,
			5,
			198,
			70,
			16,
			0,
			180,
			65,
			187,
			170,
			85,
			205,
			19,
			93,
			114,
			15,
			129,
			251,
			85,
			170,
			117,
			9,
			247,
			193,
			1,
			0,
			116,
			3,
			254,
			70,
			16,
			102,
			96,
			128,
			126,
			16,
			0,
			116,
			38,
			102,
			104,
			0,
			0,
			0,
			0,
			102,
			byte.MaxValue,
			118,
			8,
			104,
			0,
			0,
			104,
			0,
			124,
			104,
			1,
			0,
			104,
			16,
			0,
			180,
			66,
			138,
			86,
			0,
			139,
			244,
			205,
			19,
			159,
			131,
			196,
			16,
			158,
			235,
			20,
			184,
			1,
			2,
			187,
			0,
			124,
			138,
			86,
			0,
			138,
			118,
			1,
			138,
			78,
			2,
			138,
			110,
			3,
			205,
			19,
			102,
			97,
			115,
			28,
			254,
			78,
			17,
			117,
			12,
			128,
			126,
			0,
			128,
			15,
			132,
			138,
			0,
			178,
			128,
			235,
			132,
			85,
			50,
			228,
			138,
			86,
			0,
			205,
			19,
			93,
			235,
			158,
			129,
			62,
			254,
			125,
			85,
			170,
			117,
			110,
			byte.MaxValue,
			118,
			0,
			232,
			141,
			0,
			117,
			23,
			250,
			176,
			209,
			230,
			100,
			232,
			131,
			0,
			176,
			223,
			230,
			96,
			232,
			124,
			0,
			176,
			byte.MaxValue,
			230,
			100,
			232,
			117,
			0,
			251,
			184,
			0,
			187,
			205,
			26,
			102,
			35,
			192,
			117,
			59,
			102,
			129,
			251,
			84,
			67,
			80,
			65,
			117,
			50,
			129,
			249,
			2,
			1,
			114,
			44,
			102,
			104,
			7,
			187,
			0,
			0,
			102,
			104,
			0,
			2,
			0,
			0,
			102,
			104,
			8,
			0,
			0,
			0,
			102,
			83,
			102,
			83,
			102,
			85,
			102,
			104,
			0,
			0,
			0,
			0,
			102,
			104,
			0,
			124,
			0,
			0,
			102,
			97,
			104,
			0,
			0,
			7,
			205,
			26,
			90,
			50,
			246,
			234,
			0,
			124,
			0,
			0,
			205,
			24,
			160,
			183,
			7,
			235,
			8,
			160,
			182,
			7,
			235,
			3,
			160,
			181,
			7,
			50,
			228,
			5,
			0,
			7,
			139,
			240,
			172,
			60,
			0,
			116,
			9,
			187,
			7,
			0,
			180,
			14,
			205,
			16,
			235,
			242,
			244,
			235,
			253,
			43,
			201,
			228,
			100,
			235,
			0,
			36,
			2,
			224,
			248,
			36,
			2,
			195,
			73,
			110,
			118,
			97,
			108,
			105,
			100,
			32,
			112,
			97,
			114,
			116,
			105,
			116,
			105,
			111,
			110,
			32,
			116,
			97,
			98,
			108,
			101,
			0,
			69,
			114,
			114,
			111,
			114,
			32,
			108,
			111,
			97,
			100,
			105,
			110,
			103,
			32,
			111,
			112,
			101,
			114,
			97,
			116,
			105,
			110,
			103,
			32,
			115,
			121,
			115,
			116,
			101,
			109,
			0,
			77,
			105,
			115,
			115,
			105,
			110,
			103,
			32,
			111,
			112,
			101,
			114,
			97,
			116,
			105,
			110,
			103,
			32,
			115,
			121,
			115,
			116,
			101,
			109,
			0,
			0,
			0,
			99,
			123,
			154
		};

		// Token: 0x04000118 RID: 280
		private IULogger _logger;

		// Token: 0x04000119 RID: 281
		private int _bytesPerSector;

		// Token: 0x0400011A RID: 282
		private uint _sectorIndex;

		// Token: 0x0400011B RID: 283
		private MasterBootRecord _primaryRecord;

		// Token: 0x0400011C RID: 284
		private MasterBootRecord _extendedRecord;

		// Token: 0x0400011D RID: 285
		private List<MbrPartitionEntry> _entries = new List<MbrPartitionEntry>();

		// Token: 0x0400011E RID: 286
		private MasterBootRecordMetadataPartition _metadataPartition;

		// Token: 0x0400011F RID: 287
		private byte[] _codeData = new byte[440];

		// Token: 0x04000120 RID: 288
		private bool _isDesktopImage;

		// Token: 0x04000121 RID: 289
		private MbrPartitionEntry _extendedEntry;

		// Token: 0x02000078 RID: 120
		public enum MbrParseType
		{
			// Token: 0x040002AE RID: 686
			Normal,
			// Token: 0x040002AF RID: 687
			TruncateAllExtendedRecords,
			// Token: 0x040002B0 RID: 688
			TruncateInvalidExtendedRecords
		}
	}
}
