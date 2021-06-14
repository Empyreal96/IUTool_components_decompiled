using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200001C RID: 28
	public class RemovableDiskIdentifier : BaseIdentifier, IBlockIoIdentifier, IDeviceIdentifier
	{
		// Token: 0x0600009C RID: 156 RVA: 0x000053F4 File Offset: 0x000035F4
		public void ReadFromStream(BinaryReader reader)
		{
			this.DriveNumber = reader.ReadUInt32();
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00005402 File Offset: 0x00003602
		public void WriteToStream(BinaryWriter writer)
		{
			throw new ImageStorageException(string.Format("{0}: This function isn't implemented.", MethodBase.GetCurrentMethod().Name));
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00005420 File Offset: 0x00003620
		[CLSCompliant(false)]
		public void LogInfo(IULogger logger, int indentLevel)
		{
			string str = new StringBuilder().Append(' ', indentLevel).ToString();
			logger.LogInfo(str + "Identifier: Removable Disk", new object[0]);
			logger.LogInfo(str + "Drive Number:  {0}", new object[]
			{
				this.DriveNumber
			});
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600009F RID: 159 RVA: 0x00005269 File Offset: 0x00003469
		[CLSCompliant(false)]
		public uint Size
		{
			get
			{
				return 0U;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000A0 RID: 160 RVA: 0x00005269 File Offset: 0x00003469
		[CLSCompliant(false)]
		public BlockIoType BlockType
		{
			get
			{
				return BlockIoType.HardDisk;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000A1 RID: 161 RVA: 0x0000547C File Offset: 0x0000367C
		// (set) Token: 0x060000A2 RID: 162 RVA: 0x00005484 File Offset: 0x00003684
		[CLSCompliant(false)]
		public uint DriveNumber { get; set; }
	}
}
