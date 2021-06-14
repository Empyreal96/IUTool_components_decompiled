using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000011 RID: 17
	public class BcdElementIntegerList : BcdElement
	{
		// Token: 0x06000053 RID: 83 RVA: 0x000046CC File Offset: 0x000028CC
		public BcdElementIntegerList(byte[] binaryData, BcdElementDataType dataType) : base(dataType)
		{
			base.SetBinaryData(binaryData);
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000054 RID: 84 RVA: 0x0000483C File Offset: 0x00002A3C
		[CLSCompliant(false)]
		public List<ulong> Value
		{
			get
			{
				byte[] binaryData = base.GetBinaryData();
				int num = binaryData.Length / 8;
				ulong[] array = new ulong[num];
				for (int i = 0; i < num; i++)
				{
					int num2 = i * 8;
					uint num3 = (uint)((int)binaryData[num2 + 7] << 24 | (int)binaryData[num2 + 6] << 16 | (int)binaryData[num2 + 5] << 8 | (int)binaryData[num2 + 4]);
					uint num4 = (uint)((int)binaryData[num2 + 3] << 24 | (int)binaryData[num2 + 2] << 16 | (int)binaryData[num2 + 1] << 8 | (int)binaryData[num2]);
					array[i] = ((ulong)num3 << 32 | (ulong)num4);
				}
				return new List<ulong>(array);
			}
		}

		// Token: 0x06000055 RID: 85 RVA: 0x000048CC File Offset: 0x00002ACC
		[CLSCompliant(false)]
		public override void LogInfo(IULogger logger, int indentLevel)
		{
			string str = new StringBuilder().Append(' ', indentLevel).ToString();
			base.LogInfo(logger, indentLevel);
			foreach (ulong num in this.Value)
			{
				logger.LogInfo(str + "Value: 0x{0:x16}", new object[]
				{
					num
				});
			}
		}
	}
}
