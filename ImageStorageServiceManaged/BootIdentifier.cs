using System;
using System.IO;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000016 RID: 22
	public class BootIdentifier : BaseIdentifier, IDeviceIdentifier
	{
		// Token: 0x06000082 RID: 130 RVA: 0x0000500B File Offset: 0x0000320B
		public void ReadFromStream(BinaryReader reader)
		{
			reader.ReadBytes((int)this.Size);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x0000501C File Offset: 0x0000321C
		public void WriteToStream(BinaryWriter writer)
		{
			byte[] array = new byte[this.Size];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = 0;
			}
			writer.Write(array, 0, array.Length);
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00005054 File Offset: 0x00003254
		[CLSCompliant(false)]
		public void LogInfo(IULogger logger, int indentLevel)
		{
			string str = new StringBuilder().Append(' ', indentLevel).ToString();
			logger.LogInfo(str + "Boot Identifier", new object[0]);
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000085 RID: 133 RVA: 0x0000508B File Offset: 0x0000328B
		[CLSCompliant(false)]
		public uint Size
		{
			get
			{
				return BcdElementBootDevice.BaseBootDeviceSizeInBytes - BcdElementBootDevice.BaseSize;
			}
		}
	}
}
