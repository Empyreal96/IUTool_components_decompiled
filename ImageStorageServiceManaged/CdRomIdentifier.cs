using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200001D RID: 29
	public class CdRomIdentifier : BaseIdentifier, IBlockIoIdentifier, IDeviceIdentifier
	{
		// Token: 0x060000A4 RID: 164 RVA: 0x0000548D File Offset: 0x0000368D
		public void ReadFromStream(BinaryReader reader)
		{
			this.CdRomNumber = reader.ReadUInt32();
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00005402 File Offset: 0x00003602
		public void WriteToStream(BinaryWriter writer)
		{
			throw new ImageStorageException(string.Format("{0}: This function isn't implemented.", MethodBase.GetCurrentMethod().Name));
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x0000549C File Offset: 0x0000369C
		[CLSCompliant(false)]
		public void LogInfo(IULogger logger, int indentLevel)
		{
			string str = new StringBuilder().Append(' ', indentLevel).ToString();
			logger.LogInfo(str + "Identifier: CdRom", new object[0]);
			logger.LogInfo(str + "CdRom Number:  {0}", new object[]
			{
				this.CdRomNumber
			});
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000A7 RID: 167 RVA: 0x00005269 File Offset: 0x00003469
		[CLSCompliant(false)]
		public uint Size
		{
			get
			{
				return 0U;
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x00005269 File Offset: 0x00003469
		[CLSCompliant(false)]
		public BlockIoType BlockType
		{
			get
			{
				return BlockIoType.HardDisk;
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000A9 RID: 169 RVA: 0x000054F8 File Offset: 0x000036F8
		// (set) Token: 0x060000AA RID: 170 RVA: 0x00005500 File Offset: 0x00003700
		[CLSCompliant(false)]
		public uint CdRomNumber { get; set; }
	}
}
