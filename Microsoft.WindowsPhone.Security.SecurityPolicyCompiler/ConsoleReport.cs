using System;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x0200000E RID: 14
	public sealed class ConsoleReport : ReportingBase
	{
		// Token: 0x06000035 RID: 53 RVA: 0x00003068 File Offset: 0x00001268
		internal ConsoleReport()
		{
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00003070 File Offset: 0x00001270
		public override void ErrorLine(string errorMsg)
		{
			Console.WriteLine(errorMsg);
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003078 File Offset: 0x00001278
		public override void Debug(string debugMsg)
		{
			if (ReportingBase.EnableDebugMessage)
			{
				Console.Write("Debug: " + debugMsg);
			}
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00003091 File Offset: 0x00001291
		public override void DebugLine(string debugMsg)
		{
			if (ReportingBase.EnableDebugMessage)
			{
				Console.WriteLine("Debug: " + debugMsg);
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x000030AA File Offset: 0x000012AA
		public override void XmlElementLine(string indentation, string elememt)
		{
			this.DebugLine(string.Format(GlobalVariables.Culture, "{0}{1}", new object[]
			{
				indentation,
				elememt
			}));
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000030CF File Offset: 0x000012CF
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
