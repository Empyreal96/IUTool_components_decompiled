using System;
using System.IO;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200001F RID: 31
	public class RamDiskIdentifier : BaseIdentifier, IBlockIoIdentifier, IDeviceIdentifier
	{
		// Token: 0x060000BC RID: 188 RVA: 0x000057E6 File Offset: 0x000039E6
		public RamDiskIdentifier(string filePath, BcdElementBootDevice parentDevice)
		{
			this.Source = new FileIdentifier(filePath, parentDevice);
		}

		// Token: 0x060000BD RID: 189 RVA: 0x000057FB File Offset: 0x000039FB
		public void ReadFromStream(BinaryReader reader)
		{
			reader.ReadUInt32();
			this.ImageBase = (ulong)reader.ReadUInt32();
			this.ImageSize = reader.ReadUInt64();
			this.ImageOffset = reader.ReadUInt32();
			this.Source.ReadFromStream(reader);
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00005835 File Offset: 0x00003A35
		[CLSCompliant(false)]
		public void ReplaceParentDeviceIdentifier(IDeviceIdentifier identifier)
		{
			this.Source.ReplaceParentDeviceIdentifier(identifier);
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00005843 File Offset: 0x00003A43
		public void WriteToStream(BinaryWriter writer)
		{
			writer.Write(3UL);
			writer.Write(0U);
			writer.Write(0UL);
			writer.Write(0U);
			this.Source.WriteToStream(writer);
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00005870 File Offset: 0x00003A70
		[CLSCompliant(false)]
		public void LogInfo(IULogger logger, int indentLevel)
		{
			string str = new StringBuilder().Append(' ', indentLevel).ToString();
			logger.LogInfo(str + "Block IO Type: RamDisk", new object[0]);
			logger.LogInfo(str + "ImageBase:     0x{0:x}", new object[]
			{
				this.ImageBase
			});
			logger.LogInfo(str + "ImageSize:     0x{0:x}", new object[]
			{
				this.ImageSize
			});
			logger.LogInfo(str + "ImageOffset:   0x{0:x}", new object[]
			{
				this.ImageOffset
			});
			logger.LogInfo(str + "File Path:     {0}", new object[]
			{
				this.Source.Path
			});
			if (this.Source.ParentDevice != null)
			{
				this.Source.ParentDevice.LogInfo(logger, checked(indentLevel + 2));
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x0000595C File Offset: 0x00003B5C
		[CLSCompliant(false)]
		public uint Size
		{
			get
			{
				return this.Source.Size + 24U;
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000C2 RID: 194 RVA: 0x0000596C File Offset: 0x00003B6C
		[CLSCompliant(false)]
		public BlockIoType BlockType
		{
			get
			{
				return BlockIoType.RamDisk;
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000C3 RID: 195 RVA: 0x0000596F File Offset: 0x00003B6F
		// (set) Token: 0x060000C4 RID: 196 RVA: 0x00005977 File Offset: 0x00003B77
		[CLSCompliant(false)]
		public ulong ImageBase { get; set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x00005980 File Offset: 0x00003B80
		// (set) Token: 0x060000C6 RID: 198 RVA: 0x00005988 File Offset: 0x00003B88
		[CLSCompliant(false)]
		public ulong ImageSize { get; set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x00005991 File Offset: 0x00003B91
		// (set) Token: 0x060000C8 RID: 200 RVA: 0x00005999 File Offset: 0x00003B99
		[CLSCompliant(false)]
		public uint ImageOffset { get; set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000C9 RID: 201 RVA: 0x000059A2 File Offset: 0x00003BA2
		// (set) Token: 0x060000CA RID: 202 RVA: 0x000059AA File Offset: 0x00003BAA
		public FileIdentifier Source { get; set; }
	}
}
