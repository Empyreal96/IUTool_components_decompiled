using System;
using System.IO;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200002B RID: 43
	internal class MbrPartitionEntry
	{
		// Token: 0x1700007A RID: 122
		// (get) Token: 0x0600018A RID: 394 RVA: 0x00008008 File Offset: 0x00006208
		// (set) Token: 0x0600018B RID: 395 RVA: 0x00008010 File Offset: 0x00006210
		public bool Bootable { get; set; }

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x0600018C RID: 396 RVA: 0x00008019 File Offset: 0x00006219
		// (set) Token: 0x0600018D RID: 397 RVA: 0x00008021 File Offset: 0x00006221
		public byte PartitionType { get; set; }

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x0600018E RID: 398 RVA: 0x0000802A File Offset: 0x0000622A
		// (set) Token: 0x0600018F RID: 399 RVA: 0x00008032 File Offset: 0x00006232
		public uint StartingSector { get; set; }

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000190 RID: 400 RVA: 0x0000803B File Offset: 0x0000623B
		// (set) Token: 0x06000191 RID: 401 RVA: 0x00008043 File Offset: 0x00006243
		public uint SectorCount { get; set; }

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000192 RID: 402 RVA: 0x0000804C File Offset: 0x0000624C
		public bool TypeIsContainer
		{
			get
			{
				return this.PartitionType == 5 || this.PartitionType == 19;
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000193 RID: 403 RVA: 0x00008063 File Offset: 0x00006263
		// (set) Token: 0x06000194 RID: 404 RVA: 0x0000806B File Offset: 0x0000626B
		public uint StartingSectorOffset { get; set; }

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000195 RID: 405 RVA: 0x00008074 File Offset: 0x00006274
		public uint AbsoluteStartingSector
		{
			get
			{
				return this.StartingSectorOffset + this.StartingSector;
			}
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00008084 File Offset: 0x00006284
		public void ReadFromStream(BinaryReader reader)
		{
			this.Bootable = ((reader.ReadByte() & 128) > 0);
			reader.ReadBytes(3);
			this.PartitionType = reader.ReadByte();
			reader.ReadBytes(3);
			this.StartingSector = reader.ReadUInt32();
			this.SectorCount = reader.ReadUInt32();
		}

		// Token: 0x06000197 RID: 407 RVA: 0x000080DC File Offset: 0x000062DC
		public void WriteToStream(BinaryWriter writer)
		{
			writer.Write(this.Bootable ? 128 : 0);
			writer.Write(0);
			writer.Write(0);
			writer.Write(0);
			writer.Write(this.PartitionType);
			writer.Write(0);
			writer.Write(0);
			writer.Write(0);
			writer.Write(this.StartingSector);
			writer.Write(this.SectorCount);
		}

		// Token: 0x06000198 RID: 408 RVA: 0x0000814E File Offset: 0x0000634E
		public void ZeroData()
		{
			this.PartitionType = 0;
			this.StartingSector = 0U;
			this.StartingSectorOffset = 0U;
			this.SectorCount = 0U;
		}

		// Token: 0x06000199 RID: 409 RVA: 0x0000816C File Offset: 0x0000636C
		public void LogInfo(IULogger logger, MasterBootRecord masterBootRecord, ushort indentLevel = 0)
		{
			if (this.StartingSector == 0U || this.SectorCount == 0U || this.PartitionType == 0)
			{
				return;
			}
			string str = new StringBuilder().Append(' ', (int)indentLevel).ToString();
			string text = "<EBR>";
			if (!this.TypeIsContainer)
			{
				text = masterBootRecord.GetPartitionName(this);
			}
			logger.LogInfo(str + "Partition Name : {0}", new object[]
			{
				text
			});
			logger.LogInfo(str + "Partition Type : 0x{0:x}", new object[]
			{
				this.PartitionType
			});
			logger.LogInfo(str + "Starting Sector: 0x{0:x}", new object[]
			{
				this.StartingSector
			});
			logger.LogInfo(str + "Sector Count   : 0x{0:x}", new object[]
			{
				this.SectorCount
			});
			if (masterBootRecord.IsExtendedBootRecord())
			{
				logger.LogInfo(str + "Absolute Starting Sector: 0x{0:x}", new object[]
				{
					this.AbsoluteStartingSector
				});
			}
			logger.LogInfo("", new object[0]);
		}

		// Token: 0x04000124 RID: 292
		public const int SizeInBytes = 16;
	}
}
