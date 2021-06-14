using System;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200000F RID: 15
	public class BcdElementInteger : BcdElement
	{
		// Token: 0x0600004C RID: 76 RVA: 0x000046CC File Offset: 0x000028CC
		public BcdElementInteger(byte[] binaryData, BcdElementDataType dataType) : base(dataType)
		{
			base.SetBinaryData(binaryData);
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600004D RID: 77 RVA: 0x000046DC File Offset: 0x000028DC
		[CLSCompliant(false)]
		public ulong Value
		{
			get
			{
				byte[] binaryData = base.GetBinaryData();
				uint num = 0U;
				for (int i = 4; i < Math.Min(binaryData.Length, 8); i++)
				{
					num |= (uint)((uint)binaryData[i] << (i - 4) * 8);
				}
				uint num2 = 0U;
				for (int j = 0; j < Math.Min(binaryData.Length, 4); j++)
				{
					num2 |= (uint)((uint)binaryData[j] << j * 8);
				}
				return (ulong)num << 32 | (ulong)num2;
			}
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00004748 File Offset: 0x00002948
		[CLSCompliant(false)]
		public override void LogInfo(IULogger logger, int indentLevel)
		{
			string str = new StringBuilder().Append(' ', indentLevel).ToString();
			base.LogInfo(logger, indentLevel);
			try
			{
				logger.LogInfo(str + "Value: 0x{0:x}", new object[]
				{
					this.Value
				});
			}
			catch (ImageStorageException)
			{
				logger.LogInfo(str + "Value: <invalid data>", new object[0]);
			}
		}
	}
}
