using System;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x0200000F RID: 15
	public sealed class LogUtilityReport : ReportingBase
	{
		// Token: 0x0600003B RID: 59 RVA: 0x00003068 File Offset: 0x00001268
		internal LogUtilityReport()
		{
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000030F8 File Offset: 0x000012F8
		public override void ErrorLine(string errorMsg)
		{
			LogUtil.Error(errorMsg);
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003100 File Offset: 0x00001300
		public override void Debug(string debugMsg)
		{
			if (ReportingBase.EnableDebugMessage)
			{
				LogUtil.Diagnostic(debugMsg);
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003100 File Offset: 0x00001300
		public override void DebugLine(string debugMsg)
		{
			if (ReportingBase.EnableDebugMessage)
			{
				LogUtil.Diagnostic(debugMsg);
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x000030AA File Offset: 0x000012AA
		public override void XmlElementLine(string indentation, string elememt)
		{
			this.DebugLine(string.Format(GlobalVariables.Culture, "{0}{1}", new object[]
			{
				indentation,
				elememt
			}));
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000030CF File Offset: 0x000012CF
		public override void XmlAttributeLine(string indentation, string elememt, string value)
		{
			this.DebugLine(string.Format(GlobalVariables.Culture, "{0}{1}=\"{2}\"", new object[]
			{
				indentation,
				elememt,
				value
			}));
		}
	}
}
