using System;
using System.IO;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000029 RID: 41
	internal class GuidPartitionTableEntry
	{
		// Token: 0x0600015F RID: 351 RVA: 0x0000719E File Offset: 0x0000539E
		public GuidPartitionTableEntry(IULogger logger)
		{
			this._logger = logger;
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000160 RID: 352 RVA: 0x000071AD File Offset: 0x000053AD
		// (set) Token: 0x06000161 RID: 353 RVA: 0x000071B5 File Offset: 0x000053B5
		public Guid PartitionType { get; set; }

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000162 RID: 354 RVA: 0x000071BE File Offset: 0x000053BE
		// (set) Token: 0x06000163 RID: 355 RVA: 0x000071C6 File Offset: 0x000053C6
		public Guid PartitionId { get; set; }

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000164 RID: 356 RVA: 0x000071CF File Offset: 0x000053CF
		// (set) Token: 0x06000165 RID: 357 RVA: 0x000071D7 File Offset: 0x000053D7
		public ulong StartingSector { get; set; }

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000166 RID: 358 RVA: 0x000071E0 File Offset: 0x000053E0
		// (set) Token: 0x06000167 RID: 359 RVA: 0x000071E8 File Offset: 0x000053E8
		public ulong LastSector { get; set; }

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000168 RID: 360 RVA: 0x000071F1 File Offset: 0x000053F1
		// (set) Token: 0x06000169 RID: 361 RVA: 0x000071F9 File Offset: 0x000053F9
		public ulong Attributes { get; set; }

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x0600016A RID: 362 RVA: 0x00007202 File Offset: 0x00005402
		public string PartitionName
		{
			get
			{
				return Encoding.Unicode.GetString(this._partitionName).Split(new char[1])[0];
			}
		}

		// Token: 0x0600016B RID: 363 RVA: 0x00007224 File Offset: 0x00005424
		public void WriteToStream(Stream stream, int bytesPerEntry)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			long position = stream.Position;
			byte[] array = this.PartitionType.ToByteArray();
			binaryWriter.Write(array, 0, array.Length);
			array = this.PartitionId.ToByteArray();
			binaryWriter.Write(array, 0, array.Length);
			binaryWriter.Write(this.StartingSector);
			binaryWriter.Write(this.LastSector);
			binaryWriter.Write(this.Attributes);
			binaryWriter.Write(this._partitionName, 0, this._partitionName.Length);
			if (stream.Position - position < (long)bytesPerEntry)
			{
				int num = (int)((long)bytesPerEntry - (stream.Position - position));
				for (int i = 0; i < num; i++)
				{
					stream.WriteByte(0);
				}
			}
		}

		// Token: 0x0600016C RID: 364 RVA: 0x000072DC File Offset: 0x000054DC
		public void ReadFromStream(Stream stream, int entryByteCount)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			long position = stream.Position;
			this.PartitionType = new Guid(binaryReader.ReadBytes(16));
			this.PartitionId = new Guid(binaryReader.ReadBytes(16));
			this.StartingSector = binaryReader.ReadUInt64();
			this.LastSector = binaryReader.ReadUInt64();
			this.Attributes = binaryReader.ReadUInt64();
			this._partitionName = binaryReader.ReadBytes(72);
			if (stream.Position - position < (long)entryByteCount)
			{
				stream.Position += (long)entryByteCount - (stream.Position - position);
			}
		}

		// Token: 0x0600016D RID: 365 RVA: 0x00007374 File Offset: 0x00005574
		public void LogInfo(ushort indentLevel = 0)
		{
			if (this.PartitionType == Guid.Empty || string.IsNullOrEmpty(this.PartitionName) || this.StartingSector == 0UL)
			{
				return;
			}
			string str = new StringBuilder().Append(' ', (int)indentLevel).ToString();
			this._logger.LogInfo(str + "Partition Name : {0}", new object[]
			{
				this.PartitionName
			});
			this._logger.LogInfo(str + "Partition Type : {{{0}}}", new object[]
			{
				this.PartitionType
			});
			this._logger.LogInfo(str + "Partition Id   : {{{0}}}", new object[]
			{
				this.PartitionId
			});
			this._logger.LogInfo(str + "Starting Sector: 0x{0:x}", new object[]
			{
				this.StartingSector
			});
			this._logger.LogInfo(str + "Last Sector    : 0x{0:x}", new object[]
			{
				this.LastSector
			});
			this._logger.LogInfo(str + "Attributes     : 0x{0:x}", new object[]
			{
				this.Attributes
			});
			this._logger.LogInfo("", new object[0]);
		}

		// Token: 0x0600016E RID: 366 RVA: 0x000074CC File Offset: 0x000056CC
		public void Clean()
		{
			this.PartitionType = Guid.Empty;
			this.PartitionId = Guid.Empty;
			this.StartingSector = 0UL;
			this.LastSector = 0UL;
			this.Attributes = 0UL;
			for (int i = 0; i < 72; i++)
			{
				this._partitionName[i] = 0;
			}
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0000751D File Offset: 0x0000571D
		public override string ToString()
		{
			return this.PartitionName;
		}

		// Token: 0x0400010D RID: 269
		private byte[] _partitionName;

		// Token: 0x0400010E RID: 270
		private IULogger _logger;
	}
}
