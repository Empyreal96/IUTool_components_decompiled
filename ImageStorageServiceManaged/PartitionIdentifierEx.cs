using System;
using System.IO;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000022 RID: 34
	public class PartitionIdentifierEx : BaseIdentifier, IDeviceIdentifier
	{
		// Token: 0x060000E2 RID: 226 RVA: 0x00005C79 File Offset: 0x00003E79
		public void ReadFromStream(BinaryReader reader)
		{
			this._rawData = reader.ReadBytes(16);
			this.ParentIdentifier = BlockIoIdentifierFactory.CreateFromStream(reader);
			this.ParentIdentifier.ReadFromStream(reader);
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00005CA4 File Offset: 0x00003EA4
		public void WriteToStream(BinaryWriter writer)
		{
			long position = writer.BaseStream.Position;
			writer.Write(this._rawData);
			this.ParentIdentifier.WriteToStream(writer);
			while (writer.BaseStream.Position < position + (long)((ulong)this.Size))
			{
				writer.Write(0);
			}
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x00005CF4 File Offset: 0x00003EF4
		[CLSCompliant(false)]
		public void LogInfo(IULogger logger, int indentLevel)
		{
			string str = new StringBuilder().Append(' ', indentLevel).ToString();
			logger.LogInfo(str + "Identifier: PartitionEx", new object[0]);
			if (this.ParentIdentifier != null)
			{
				if (this.ParentIdentifier.BlockType == BlockIoType.HardDisk || this.ParentIdentifier.BlockType == BlockIoType.VirtualHardDisk)
				{
					HardDiskIdentifier hardDiskIdentifier = this.ParentIdentifier as HardDiskIdentifier;
					if (hardDiskIdentifier == null)
					{
						hardDiskIdentifier = (this.ParentIdentifier as VirtualDiskIdentifier).InternalIdentifer;
					}
					switch (hardDiskIdentifier.PartitionStyle)
					{
					case PartitionFormat.Gpt:
						logger.LogInfo(str + "GPT Partition Identifier: {{{0}}}", new object[]
						{
							this.GptValue
						});
						break;
					case PartitionFormat.Mbr:
						logger.LogInfo(str + "MBR Partition Offset: 0x{0:x}", new object[]
						{
							this.MbrPartitionOffset
						});
						break;
					case PartitionFormat.Raw:
						throw new ImageStorageException("Cannot use a partition identifier on a RAW disk.");
					}
				}
				else if (this.ParentIdentifier.BlockType == BlockIoType.CdRom)
				{
					logger.LogInfo(str + "El Torito Value: {0}", new object[]
					{
						this.ElToritoValue
					});
				}
				else
				{
					logger.LogInfo(str + "Value: Unsure of the partition style.", new object[0]);
				}
			}
			else
			{
				logger.LogInfo(str + "Value: Unsure of the partition style.", new object[0]);
			}
			if (this.ParentIdentifier != null)
			{
				logger.LogInfo("", new object[0]);
				this.ParentIdentifier.LogInfo(logger, checked(indentLevel + 2));
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060000E5 RID: 229 RVA: 0x00005E7B File Offset: 0x0000407B
		[CLSCompliant(false)]
		public uint Size
		{
			get
			{
				if (this.ParentIdentifier != null)
				{
					return 16U + Math.Max(this.ParentIdentifier.Size, BlockIoIdentifierFactory.SizeOnDisk);
				}
				return 16U + BlockIoIdentifierFactory.SizeOnDisk;
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x00005EA6 File Offset: 0x000040A6
		// (set) Token: 0x060000E7 RID: 231 RVA: 0x00005ED4 File Offset: 0x000040D4
		[CLSCompliant(false)]
		public uint ElToritoValue
		{
			get
			{
				return (uint)((int)this._rawData[3] << 24 | (int)this._rawData[2] << 16 | (int)this._rawData[1] << 8 | (int)this._rawData[0]);
			}
			set
			{
				this._rawData[3] = (byte)((value & 4278190080U) >> 24);
				this._rawData[2] = (byte)((value & 16711680U) >> 16);
				this._rawData[1] = (byte)((value & 65280U) >> 8);
				this._rawData[0] = (byte)(value & 255U);
				for (int i = 4; i < 16; i++)
				{
					this._rawData[i] = 0;
				}
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x00005F3F File Offset: 0x0000413F
		// (set) Token: 0x060000E9 RID: 233 RVA: 0x00005F4C File Offset: 0x0000414C
		public Guid GptValue
		{
			get
			{
				return new Guid(this._rawData);
			}
			set
			{
				this._rawData = value.ToByteArray();
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060000EA RID: 234 RVA: 0x00005F5C File Offset: 0x0000415C
		// (set) Token: 0x060000EB RID: 235 RVA: 0x00005FC4 File Offset: 0x000041C4
		[CLSCompliant(false)]
		public ulong MbrPartitionOffset
		{
			get
			{
				return (ulong)((long)((int)this._rawData[7] << 24 | (int)this._rawData[6] << 16 | (int)this._rawData[5] << 8 | (int)this._rawData[4] | (int)this._rawData[3] << 24 | (int)this._rawData[2] << 16 | (int)this._rawData[1] << 8 | (int)this._rawData[0]));
			}
			set
			{
				this._rawData[7] = (byte)((value & (ulong)-16777216) >> 56);
				this._rawData[6] = (byte)((value & 16711680UL) >> 48);
				this._rawData[5] = (byte)((value & 65280UL) >> 40);
				this._rawData[4] = (byte)((value & 255UL) >> 32);
				this._rawData[3] = (byte)((value & (ulong)-16777216) >> 24);
				this._rawData[2] = (byte)((value & 16711680UL) >> 16);
				this._rawData[1] = (byte)((value & 65280UL) >> 8);
				this._rawData[0] = (byte)(value & 255UL);
				for (int i = 8; i < 16; i++)
				{
					this._rawData[i] = 0;
				}
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060000EC RID: 236 RVA: 0x00006083 File Offset: 0x00004283
		// (set) Token: 0x060000ED RID: 237 RVA: 0x0000608B File Offset: 0x0000428B
		[CLSCompliant(false)]
		public IBlockIoIdentifier ParentIdentifier { get; set; }

		// Token: 0x060000EE RID: 238 RVA: 0x00006094 File Offset: 0x00004294
		[CLSCompliant(false)]
		public static PartitionIdentifierEx CreateSimpleMbr(ulong partitionOffset, uint diskSignature)
		{
			return new PartitionIdentifierEx
			{
				_rawData = new byte[16],
				MbrPartitionOffset = partitionOffset,
				ParentIdentifier = HardDiskIdentifier.CreateSimpleMbr(diskSignature)
			};
		}

		// Token: 0x060000EF RID: 239 RVA: 0x000060BB File Offset: 0x000042BB
		[CLSCompliant(false)]
		public static PartitionIdentifierEx CreateSimpleGpt(Guid diskId, Guid partitionId)
		{
			return new PartitionIdentifierEx
			{
				_rawData = new byte[16],
				GptValue = partitionId,
				ParentIdentifier = HardDiskIdentifier.CreateSimpleGpt(diskId)
			};
		}

		// Token: 0x040000E4 RID: 228
		private byte[] _rawData;
	}
}
