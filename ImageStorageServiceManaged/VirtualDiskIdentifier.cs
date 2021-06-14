using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000020 RID: 32
	public class VirtualDiskIdentifier : BaseIdentifier, IBlockIoIdentifier, IDeviceIdentifier
	{
		// Token: 0x060000CB RID: 203 RVA: 0x000059B3 File Offset: 0x00003BB3
		public void ReadFromStream(BinaryReader reader)
		{
			this.InternalIdentifer = new HardDiskIdentifier();
			this.InternalIdentifer.ReadFromStream(reader);
			this.InternalIdentifer.AsVirtualDisk = true;
			this.FileDevice = new BcdElementBootDevice();
			this.FileDevice.ReadFromStream(reader);
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00005402 File Offset: 0x00003602
		public void WriteToStream(BinaryWriter writer)
		{
			throw new ImageStorageException(string.Format("{0}: This function isn't implemented.", MethodBase.GetCurrentMethod().Name));
		}

		// Token: 0x060000CD RID: 205 RVA: 0x000059F0 File Offset: 0x00003BF0
		[CLSCompliant(false)]
		public void LogInfo(IULogger logger, int indentLevel)
		{
			string str = new StringBuilder().Append(' ', indentLevel).ToString();
			logger.LogInfo(str + "Identifier: Virtual Hard Disk", new object[0]);
			checked
			{
				this.InternalIdentifer.LogInfo(logger, indentLevel + 2);
				logger.LogInfo("", new object[0]);
				this.FileDevice.LogInfo(logger, indentLevel + 2);
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000CE RID: 206 RVA: 0x00005A56 File Offset: 0x00003C56
		[CLSCompliant(false)]
		public BlockIoType BlockType
		{
			get
			{
				return BlockIoType.VirtualHardDisk;
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000CF RID: 207 RVA: 0x00005A59 File Offset: 0x00003C59
		[CLSCompliant(false)]
		public uint Size
		{
			get
			{
				return this.InternalIdentifer.Size + this.FileDevice.Size;
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000D0 RID: 208 RVA: 0x00005A72 File Offset: 0x00003C72
		// (set) Token: 0x060000D1 RID: 209 RVA: 0x00005A7A File Offset: 0x00003C7A
		public HardDiskIdentifier InternalIdentifer { get; set; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000D2 RID: 210 RVA: 0x00005A83 File Offset: 0x00003C83
		// (set) Token: 0x060000D3 RID: 211 RVA: 0x00005A8B File Offset: 0x00003C8B
		public BcdElementBootDevice FileDevice { get; set; }
	}
}
