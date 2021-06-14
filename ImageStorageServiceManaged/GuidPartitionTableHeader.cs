using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000028 RID: 40
	internal class GuidPartitionTableHeader
	{
		// Token: 0x0600013B RID: 315 RVA: 0x00006BD9 File Offset: 0x00004DD9
		public GuidPartitionTableHeader(IULogger logger)
		{
			this._logger = logger;
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x0600013C RID: 316 RVA: 0x00006BE8 File Offset: 0x00004DE8
		// (set) Token: 0x0600013D RID: 317 RVA: 0x00006BF0 File Offset: 0x00004DF0
		public ulong Signature { get; set; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x0600013E RID: 318 RVA: 0x00006BF9 File Offset: 0x00004DF9
		// (set) Token: 0x0600013F RID: 319 RVA: 0x00006C01 File Offset: 0x00004E01
		public uint Revision { get; set; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000140 RID: 320 RVA: 0x00006C0A File Offset: 0x00004E0A
		// (set) Token: 0x06000141 RID: 321 RVA: 0x00006C12 File Offset: 0x00004E12
		public uint HeaderSize { get; set; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000142 RID: 322 RVA: 0x00006C1B File Offset: 0x00004E1B
		// (set) Token: 0x06000143 RID: 323 RVA: 0x00006C23 File Offset: 0x00004E23
		public uint HeaderCrc32 { get; set; }

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000144 RID: 324 RVA: 0x00006C2C File Offset: 0x00004E2C
		// (set) Token: 0x06000145 RID: 325 RVA: 0x00006C34 File Offset: 0x00004E34
		public uint Reserved { get; set; }

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000146 RID: 326 RVA: 0x00006C3D File Offset: 0x00004E3D
		// (set) Token: 0x06000147 RID: 327 RVA: 0x00006C45 File Offset: 0x00004E45
		public ulong HeaderSector { get; set; }

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000148 RID: 328 RVA: 0x00006C4E File Offset: 0x00004E4E
		// (set) Token: 0x06000149 RID: 329 RVA: 0x00006C56 File Offset: 0x00004E56
		public ulong AlternateHeaderSector { get; set; }

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x0600014A RID: 330 RVA: 0x00006C5F File Offset: 0x00004E5F
		// (set) Token: 0x0600014B RID: 331 RVA: 0x00006C67 File Offset: 0x00004E67
		public ulong FirstUsableSector { get; set; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x0600014C RID: 332 RVA: 0x00006C70 File Offset: 0x00004E70
		// (set) Token: 0x0600014D RID: 333 RVA: 0x00006C78 File Offset: 0x00004E78
		public ulong LastUsableSector { get; set; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x0600014E RID: 334 RVA: 0x00006C81 File Offset: 0x00004E81
		// (set) Token: 0x0600014F RID: 335 RVA: 0x00006C89 File Offset: 0x00004E89
		public Guid DiskId { get; set; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000150 RID: 336 RVA: 0x00006C92 File Offset: 0x00004E92
		// (set) Token: 0x06000151 RID: 337 RVA: 0x00006C9A File Offset: 0x00004E9A
		public ulong PartitionEntryStartSector { get; set; }

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000152 RID: 338 RVA: 0x00006CA3 File Offset: 0x00004EA3
		// (set) Token: 0x06000153 RID: 339 RVA: 0x00006CAB File Offset: 0x00004EAB
		public uint PartitionEntryCount { get; set; }

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000154 RID: 340 RVA: 0x00006CB4 File Offset: 0x00004EB4
		// (set) Token: 0x06000155 RID: 341 RVA: 0x00006CBC File Offset: 0x00004EBC
		public uint PartitionEntrySizeInBytes { get; set; }

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000156 RID: 342 RVA: 0x00006CC5 File Offset: 0x00004EC5
		// (set) Token: 0x06000157 RID: 343 RVA: 0x00006CCD File Offset: 0x00004ECD
		public uint PartitionEntryArrayCrc32 { get; set; }

		// Token: 0x06000158 RID: 344 RVA: 0x00006CD8 File Offset: 0x00004ED8
		public void WriteToStream(Stream stream, int bytesPerSector)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			binaryWriter.Write(this.Signature);
			binaryWriter.Write(this.Revision);
			binaryWriter.Write(this.HeaderSize);
			binaryWriter.Write(this.HeaderCrc32);
			binaryWriter.Write(this.Reserved);
			binaryWriter.Write(this.HeaderSector);
			binaryWriter.Write(this.AlternateHeaderSector);
			binaryWriter.Write(this.FirstUsableSector);
			binaryWriter.Write(this.LastUsableSector);
			byte[] array = this.DiskId.ToByteArray();
			binaryWriter.Write(array, 0, array.Length);
			binaryWriter.Write(this.PartitionEntryStartSector);
			binaryWriter.Write(this.PartitionEntryCount);
			binaryWriter.Write(this.PartitionEntrySizeInBytes);
			binaryWriter.Write(this.PartitionEntryArrayCrc32);
			if (stream.Position % (long)bytesPerSector != 0L)
			{
				stream.Position += (long)bytesPerSector - stream.Position % (long)bytesPerSector;
			}
		}

		// Token: 0x06000159 RID: 345 RVA: 0x00006DC4 File Offset: 0x00004FC4
		public void ReadFromStream(Stream stream, int bytesPerSector)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			this.Signature = binaryReader.ReadUInt64();
			if (this.Signature != 6075990659671082565UL)
			{
				throw new ImageStorageException("The EFI header signature is invalid.");
			}
			this.Revision = binaryReader.ReadUInt32();
			if (this.Revision != 65536U)
			{
				throw new ImageStorageException("The EFI header revision is an unsupported version.");
			}
			this.HeaderSize = binaryReader.ReadUInt32();
			this.HeaderCrc32 = binaryReader.ReadUInt32();
			this.Reserved = binaryReader.ReadUInt32();
			if (this.Reserved != 0U)
			{
				throw new ImageStorageException("The reserved field in the EFI header is not zero.");
			}
			this.HeaderSector = binaryReader.ReadUInt64();
			this.AlternateHeaderSector = binaryReader.ReadUInt64();
			this.FirstUsableSector = binaryReader.ReadUInt64();
			this.LastUsableSector = binaryReader.ReadUInt64();
			this.DiskId = new Guid(binaryReader.ReadBytes(16));
			this.PartitionEntryStartSector = binaryReader.ReadUInt64();
			this.PartitionEntryCount = binaryReader.ReadUInt32();
			this.PartitionEntrySizeInBytes = binaryReader.ReadUInt32();
			this.PartitionEntryArrayCrc32 = binaryReader.ReadUInt32();
			if (stream.Position % (long)bytesPerSector != 0L)
			{
				stream.Position += (long)bytesPerSector - stream.Position % (long)bytesPerSector;
			}
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00006EF4 File Offset: 0x000050F4
		public void LogInfo(ushort indentLevel = 0)
		{
			string str = new StringBuilder().Append(' ', (int)indentLevel).ToString();
			this._logger.LogInfo(str + "GUID Partition Table Header", new object[0]);
			indentLevel += 2;
			str = new StringBuilder().Append(' ', (int)indentLevel).ToString();
			this._logger.LogInfo(str + "Revision                     : 0x{0:x}", new object[]
			{
				this.Revision
			});
			this._logger.LogInfo(str + "Header Size                  : 0x{0:x}", new object[]
			{
				this.HeaderSize
			});
			this._logger.LogInfo(str + "Header Sector                : 0x{0:x}", new object[]
			{
				this.HeaderSector
			});
			this._logger.LogInfo(str + "Alternate Header Sector      : 0x{0:x}", new object[]
			{
				this.AlternateHeaderSector
			});
			this._logger.LogInfo(str + "First Usable Sector          : 0x{0:x}", new object[]
			{
				this.FirstUsableSector
			});
			this._logger.LogInfo(str + "Last Usable Sector           : 0x{0:x}", new object[]
			{
				this.LastUsableSector
			});
			this._logger.LogInfo(str + "Disk Id                      : {{{0}}}", new object[]
			{
				this.DiskId
			});
			this._logger.LogInfo(str + "Partition Entry Start Sector : 0x{0:x}", new object[]
			{
				this.PartitionEntryStartSector
			});
			this._logger.LogInfo(str + "Partition Entry Size In Bytes: 0x{0:x}", new object[]
			{
				this.PartitionEntrySizeInBytes
			});
			this._logger.LogInfo(str + "Partition Entry Array CRC    : 0x{0:x}", new object[]
			{
				this.PartitionEntryArrayCrc32
			});
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00005269 File Offset: 0x00003469
		public bool IsValid(ulong headerSectorIndex, byte[] partitionEntryArray)
		{
			return false;
		}

		// Token: 0x0600015C RID: 348 RVA: 0x000070F0 File Offset: 0x000052F0
		private uint ComputeHeaderCrc(int bytesPerSector)
		{
			MemoryStream memoryStream = new MemoryStream();
			HashAlgorithm hashAlgorithm = new CRC32();
			uint headerCrc = this.HeaderCrc32;
			this.HeaderCrc32 = 0U;
			this.WriteToStream(memoryStream, bytesPerSector);
			byte[] array = hashAlgorithm.ComputeHash(memoryStream.GetBuffer(), 0, (int)this.HeaderSize);
			uint result = (uint)((int)array[0] << 24 | (int)array[1] << 16 | (int)array[2] << 8 | (int)array[3]);
			this.HeaderCrc32 = headerCrc;
			return result;
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00007150 File Offset: 0x00005350
		public void ValidateHeaderCrc(int bytesPerSector)
		{
			uint num = this.ComputeHeaderCrc(bytesPerSector);
			if (this.HeaderCrc32 != num)
			{
				throw new ImageStorageException(string.Format("The GPT header CRC is invalid.  Actual: {0:x} Expected {1:x}.", this.HeaderCrc32, num));
			}
		}

		// Token: 0x0600015E RID: 350 RVA: 0x0000718F File Offset: 0x0000538F
		public void FixHeaderCrc(int bytesPerSector)
		{
			this.HeaderCrc32 = this.ComputeHeaderCrc(bytesPerSector);
		}

		// Token: 0x040000FD RID: 253
		public const ulong HeaderSignature = 6075990659671082565UL;

		// Token: 0x040000FE RID: 254
		private IULogger _logger;
	}
}
