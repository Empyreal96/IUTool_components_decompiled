using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000021 RID: 33
	public class PartitionIdentifier : BaseIdentifier, IDeviceIdentifier
	{
		// Token: 0x060000D5 RID: 213 RVA: 0x00005A94 File Offset: 0x00003C94
		public void ReadFromStream(BinaryReader reader)
		{
			byte[] array = reader.ReadBytes(16);
			this.GptValue = new Guid(array);
			this.ElToritoValue = (uint)((int)array[3] << 24 | (int)array[2] << 16 | (int)array[1] << 8 | (int)array[0]);
			this.MbrPartitionNumber = (uint)((int)array[3] << 24 | (int)array[2] << 16 | (int)array[1] << 8 | (int)array[0]);
			this.ParentIdentifier = BlockIoIdentifierFactory.CreateFromStream(reader);
			this.ParentIdentifier.ReadFromStream(reader);
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00005402 File Offset: 0x00003602
		public void WriteToStream(BinaryWriter writer)
		{
			throw new ImageStorageException(string.Format("{0}: This function isn't implemented.", MethodBase.GetCurrentMethod().Name));
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00005B08 File Offset: 0x00003D08
		[CLSCompliant(false)]
		public void LogInfo(IULogger logger, int indentLevel)
		{
			string str = new StringBuilder().Append(' ', indentLevel).ToString();
			logger.LogInfo(str + "Identifier: Partition", new object[0]);
			if (this.ParentIdentifier == null)
			{
				logger.LogInfo(str + "Value: Unsure of the partition style.", new object[0]);
				return;
			}
			if (this.ParentIdentifier.BlockType == BlockIoType.HardDisk)
			{
				switch ((this.ParentIdentifier as HardDiskIdentifier).PartitionStyle)
				{
				case PartitionFormat.Gpt:
					logger.LogInfo(str + "GPT Partition Identifier: {{{0}}}", new object[]
					{
						this.GptValue
					});
					return;
				case PartitionFormat.Mbr:
					logger.LogInfo(str + "MBR Partition Number: {0}", new object[]
					{
						this.MbrPartitionNumber
					});
					return;
				case PartitionFormat.Raw:
					throw new ImageStorageException("Cannot use a partition identifier on a RAW disk.");
				default:
					return;
				}
			}
			else
			{
				if (this.ParentIdentifier.BlockType == BlockIoType.CdRom)
				{
					logger.LogInfo(str + "El Torito Value: {0}", new object[]
					{
						this.ElToritoValue
					});
					return;
				}
				logger.LogInfo(str + "Value: Unsure of the partition style.", new object[0]);
				return;
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x00005269 File Offset: 0x00003469
		[CLSCompliant(false)]
		public uint Size
		{
			get
			{
				return 0U;
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000D9 RID: 217 RVA: 0x00005C35 File Offset: 0x00003E35
		// (set) Token: 0x060000DA RID: 218 RVA: 0x00005C3D File Offset: 0x00003E3D
		[CLSCompliant(false)]
		public uint ElToritoValue { get; set; }

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000DB RID: 219 RVA: 0x00005C46 File Offset: 0x00003E46
		// (set) Token: 0x060000DC RID: 220 RVA: 0x00005C4E File Offset: 0x00003E4E
		public Guid GptValue { get; set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000DD RID: 221 RVA: 0x00005C57 File Offset: 0x00003E57
		// (set) Token: 0x060000DE RID: 222 RVA: 0x00005C5F File Offset: 0x00003E5F
		[CLSCompliant(false)]
		public uint MbrPartitionNumber { get; set; }

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060000DF RID: 223 RVA: 0x00005C68 File Offset: 0x00003E68
		// (set) Token: 0x060000E0 RID: 224 RVA: 0x00005C70 File Offset: 0x00003E70
		[CLSCompliant(false)]
		public IBlockIoIdentifier ParentIdentifier { get; set; }
	}
}
