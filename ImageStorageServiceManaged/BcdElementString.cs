using System;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200000C RID: 12
	public class BcdElementString : BcdElement
	{
		// Token: 0x06000041 RID: 65 RVA: 0x000044AF File Offset: 0x000026AF
		public BcdElementString(string stringData, BcdElementDataType dataType) : base(dataType)
		{
			base.StringData = stringData;
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000042 RID: 66 RVA: 0x000044BF File Offset: 0x000026BF
		// (set) Token: 0x06000043 RID: 67 RVA: 0x000044C7 File Offset: 0x000026C7
		public string Value
		{
			get
			{
				return base.StringData;
			}
			set
			{
				base.StringData = value;
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000044D0 File Offset: 0x000026D0
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
