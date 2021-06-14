using System;
using System.IO;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200001B RID: 27
	public class HardDiskIdentifier : BaseIdentifier, IBlockIoIdentifier, IDeviceIdentifier
	{
		// Token: 0x0600008A RID: 138 RVA: 0x00005133 File Offset: 0x00003333
		public void ReadFromStream(BinaryReader reader)
		{
			this.PartitionStyle = (PartitionFormat)reader.ReadUInt32();
			this._rawIdentifier = reader.ReadBytes(16);
		}

		// Token: 0x0600008B RID: 139 RVA: 0x0000514F File Offset: 0x0000334F
		public void WriteToStream(BinaryWriter writer)
		{
			writer.Write((uint)this.BlockType);
			writer.Write((uint)this.PartitionStyle);
			writer.Write(this._rawIdentifier);
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00005178 File Offset: 0x00003378
		[CLSCompliant(false)]
		public void LogInfo(IULogger logger, int indentLevel)
		{
			string str = new StringBuilder().Append(' ', indentLevel).ToString();
			logger.LogInfo(str + "Identifier: Hard Disk", new object[0]);
			logger.LogInfo(str + string.Format("Partition Style:   {0}", this.PartitionStyle), new object[0]);
			switch (this.PartitionStyle)
			{
			case PartitionFormat.Gpt:
				logger.LogInfo(str + "GPT Guid:          {{{0}}}", new object[]
				{
					this.GptSignature
				});
				return;
			case PartitionFormat.Mbr:
				logger.LogInfo(str + string.Format("MBR Signature:     0x{0:x}", this.MbrSignature), new object[0]);
				return;
			case PartitionFormat.Raw:
				logger.LogInfo(str + string.Format("Raw Disk Number:   {0}", this.RawDiskNumber), new object[0]);
				return;
			default:
				return;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600008D RID: 141 RVA: 0x00005265 File Offset: 0x00003465
		[CLSCompliant(false)]
		public uint Size
		{
			get
			{
				return 24U;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600008E RID: 142 RVA: 0x00005269 File Offset: 0x00003469
		[CLSCompliant(false)]
		public BlockIoType BlockType
		{
			get
			{
				return BlockIoType.HardDisk;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600008F RID: 143 RVA: 0x0000526C File Offset: 0x0000346C
		// (set) Token: 0x06000090 RID: 144 RVA: 0x00005274 File Offset: 0x00003474
		[CLSCompliant(false)]
		public PartitionFormat PartitionStyle { get; set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000091 RID: 145 RVA: 0x0000527D File Offset: 0x0000347D
		// (set) Token: 0x06000092 RID: 146 RVA: 0x000052AC File Offset: 0x000034AC
		[CLSCompliant(false)]
		public uint MbrSignature
		{
			get
			{
				return (uint)((int)this._rawIdentifier[3] << 24 | (int)this._rawIdentifier[2] << 16 | (int)this._rawIdentifier[1] << 8 | (int)this._rawIdentifier[0]);
			}
			set
			{
				this._rawIdentifier[3] = (byte)((value & 4278190080U) >> 24);
				this._rawIdentifier[2] = (byte)((value & 16711680U) >> 16);
				this._rawIdentifier[1] = (byte)((value & 65280U) >> 8);
				this._rawIdentifier[0] = (byte)(value & 255U);
				for (int i = 4; i < 16; i++)
				{
					this._rawIdentifier[i] = 0;
				}
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000093 RID: 147 RVA: 0x00005317 File Offset: 0x00003517
		// (set) Token: 0x06000094 RID: 148 RVA: 0x00005324 File Offset: 0x00003524
		public Guid GptSignature
		{
			get
			{
				return new Guid(this._rawIdentifier);
			}
			set
			{
				this._rawIdentifier = value.ToByteArray();
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000095 RID: 149 RVA: 0x0000527D File Offset: 0x0000347D
		// (set) Token: 0x06000096 RID: 150 RVA: 0x00005334 File Offset: 0x00003534
		[CLSCompliant(false)]
		public uint RawDiskNumber
		{
			get
			{
				return (uint)((int)this._rawIdentifier[3] << 24 | (int)this._rawIdentifier[2] << 16 | (int)this._rawIdentifier[1] << 8 | (int)this._rawIdentifier[0]);
			}
			set
			{
				this._rawIdentifier[3] = (byte)((value & 4278190080U) >> 24);
				this._rawIdentifier[2] = (byte)((value & 16711680U) >> 16);
				this._rawIdentifier[1] = (byte)((value & 65280U) >> 8);
				this._rawIdentifier[0] = (byte)(value & 255U);
				for (int i = 4; i < 16; i++)
				{
					this._rawIdentifier[i] = 0;
				}
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000097 RID: 151 RVA: 0x0000539F File Offset: 0x0000359F
		// (set) Token: 0x06000098 RID: 152 RVA: 0x000053A7 File Offset: 0x000035A7
		internal bool AsVirtualDisk { get; set; }

		// Token: 0x06000099 RID: 153 RVA: 0x000053B0 File Offset: 0x000035B0
		[CLSCompliant(false)]
		public static HardDiskIdentifier CreateSimpleMbr(uint diskSignature)
		{
			return new HardDiskIdentifier
			{
				PartitionStyle = PartitionFormat.Mbr,
				_rawIdentifier = new byte[16],
				MbrSignature = diskSignature
			};
		}

		// Token: 0x0600009A RID: 154 RVA: 0x000053D2 File Offset: 0x000035D2
		[CLSCompliant(false)]
		public static HardDiskIdentifier CreateSimpleGpt(Guid diskId)
		{
			return new HardDiskIdentifier
			{
				PartitionStyle = PartitionFormat.Gpt,
				_rawIdentifier = new byte[16],
				GptSignature = diskId
			};
		}

		// Token: 0x040000D1 RID: 209
		private byte[] _rawIdentifier;
	}
}
