using System;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000010 RID: 16
	public class BcdElementBoolean : BcdElement
	{
		// Token: 0x0600004F RID: 79 RVA: 0x000046CC File Offset: 0x000028CC
		public BcdElementBoolean(byte[] binaryData, BcdElementDataType dataType) : base(dataType)
		{
			base.SetBinaryData(binaryData);
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000050 RID: 80 RVA: 0x000047C4 File Offset: 0x000029C4
		// (set) Token: 0x06000051 RID: 81 RVA: 0x000047D1 File Offset: 0x000029D1
		public bool Value
		{
			get
			{
				return base.GetBinaryData()[0] > 0;
			}
			set
			{
				if (value)
				{
					base.GetBinaryData()[0] = 1;
					return;
				}
				base.GetBinaryData()[0] = 0;
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x000047EC File Offset: 0x000029EC
		[CLSCompliant(false)]
		public override void LogInfo(IULogger logger, int indentLevel)
		{
			string str = new StringBuilder().Append(' ', indentLevel).ToString();
			base.LogInfo(logger, indentLevel);
			logger.LogInfo(str + "Value: {0}", new object[]
			{
				this.Value
			});
		}
	}
}
