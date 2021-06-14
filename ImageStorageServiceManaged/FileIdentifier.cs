using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200001E RID: 30
	public class FileIdentifier : BaseIdentifier, IBlockIoIdentifier, IDeviceIdentifier
	{
		// Token: 0x060000AC RID: 172 RVA: 0x00005509 File Offset: 0x00003709
		public FileIdentifier(string filePath, BcdElementBootDevice parentDevice)
		{
			this.Path = filePath;
			this.ParentDevice = parentDevice;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x0000551F File Offset: 0x0000371F
		[CLSCompliant(false)]
		public void ReplaceParentDeviceIdentifier(IDeviceIdentifier identifier)
		{
			this.ParentDevice.ReplaceIdentifier(identifier);
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00005530 File Offset: 0x00003730
		public void ReadFromStream(BinaryReader reader)
		{
			long position = reader.BaseStream.Position;
			this.Version = reader.ReadUInt32();
			uint num = reader.ReadUInt32();
			this.Type = reader.ReadUInt32();
			this.ParentDevice.ReadFromStream(reader);
			if (reader.BaseStream.Position - position >= (long)((ulong)this.Length))
			{
				throw new ImageStorageException(string.Format("{0}: The FileIdentifier appears to be invalid at position: 0x{1:x}  AND  {2] {3} {4}", new object[]
				{
					MethodBase.GetCurrentMethod().Name,
					position,
					this.Version,
					this.Length,
					this.Type
				}));
			}
			byte[] bytes = reader.ReadBytes((int)(num - (uint)(reader.BaseStream.Position - position)));
			this.Path = Encoding.Unicode.GetString(bytes);
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00005608 File Offset: 0x00003808
		public void WriteToStream(BinaryWriter writer)
		{
			writer.Write(1U);
			writer.Write(this.Length);
			writer.Write(5U);
			this.ParentDevice.WriteToStream(writer);
			foreach (char c in this.Path)
			{
				writer.Write((short)c);
			}
			writer.Write(0);
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x0000566C File Offset: 0x0000386C
		[CLSCompliant(false)]
		public void LogInfo(IULogger logger, int indentLevel)
		{
			string str = new StringBuilder().Append(' ', indentLevel).ToString();
			logger.LogInfo(str + "Block IO Type: File", new object[0]);
			logger.LogInfo(str + "Version:       {0}", new object[]
			{
				this.Version
			});
			logger.LogInfo(str + "Length:        {0}", new object[]
			{
				this.Length
			});
			logger.LogInfo(str + "Type:          {0}", new object[]
			{
				this.Type
			});
			logger.LogInfo(str + "Path:          {0}", new object[]
			{
				this.Path
			});
			logger.LogInfo("", new object[0]);
			this.ParentDevice.LogInfo(logger, checked(indentLevel + 2));
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x00005752 File Offset: 0x00003952
		[CLSCompliant(false)]
		public uint Size
		{
			get
			{
				return this.Length;
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x0000575A File Offset: 0x0000395A
		[CLSCompliant(false)]
		public BlockIoType BlockType
		{
			get
			{
				return BlockIoType.File;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x0000575D File Offset: 0x0000395D
		// (set) Token: 0x060000B4 RID: 180 RVA: 0x00005765 File Offset: 0x00003965
		[CLSCompliant(false)]
		public uint Version { get; set; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x00005770 File Offset: 0x00003970
		[CLSCompliant(false)]
		public uint Length
		{
			get
			{
				uint num = 12U;
				num += 2U;
				if (this.Path != null)
				{
					num += (uint)(2 * this.Path.Length);
				}
				if (this.ParentDevice != null)
				{
					num += this.ParentDevice.CalculatedSize;
				}
				return num;
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x000057B3 File Offset: 0x000039B3
		// (set) Token: 0x060000B7 RID: 183 RVA: 0x000057BB File Offset: 0x000039BB
		[CLSCompliant(false)]
		public uint Type { get; set; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x000057C4 File Offset: 0x000039C4
		// (set) Token: 0x060000B9 RID: 185 RVA: 0x000057CC File Offset: 0x000039CC
		public string Path { get; private set; }

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000BA RID: 186 RVA: 0x000057D5 File Offset: 0x000039D5
		// (set) Token: 0x060000BB RID: 187 RVA: 0x000057DD File Offset: 0x000039DD
		public BcdElementBootDevice ParentDevice { get; set; }
	}
}
