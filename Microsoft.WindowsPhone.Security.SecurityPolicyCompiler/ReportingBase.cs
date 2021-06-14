using System;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x0200000D RID: 13
	public abstract class ReportingBase
	{
		// Token: 0x06000029 RID: 41
		public abstract void ErrorLine(string errorMsg);

		// Token: 0x0600002A RID: 42
		public abstract void Debug(string debugMsg);

		// Token: 0x0600002B RID: 43
		public abstract void DebugLine(string debugMsg);

		// Token: 0x0600002C RID: 44
		public abstract void XmlElementLine(string indentation, string element);

		// Token: 0x0600002D RID: 45
		public abstract void XmlAttributeLine(string indentation, string element, string value);

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00002FBE File Offset: 0x000011BE
		// (set) Token: 0x0600002F RID: 47 RVA: 0x00002FC5 File Offset: 0x000011C5
		public static bool UseInternalLogger
		{
			get
			{
				return ReportingBase.useInternalLogger;
			}
			set
			{
				ReportingBase.useInternalLogger = value;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00002FCD File Offset: 0x000011CD
		// (set) Token: 0x06000031 RID: 49 RVA: 0x00002FD4 File Offset: 0x000011D4
		public static bool EnableDebugMessage
		{
			get
			{
				return ReportingBase.enableDebugMessage;
			}
			set
			{
				ReportingBase.enableDebugMessage = value;
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002FDC File Offset: 0x000011DC
		public static ReportingBase GetInstance()
		{
			if (ReportingBase.instance == null)
			{
				object obj = ReportingBase.syncRoot;
				lock (obj)
				{
					if (ReportingBase.instance == null)
					{
						if (ReportingBase.UseInternalLogger)
						{
							ReportingBase.instance = new ConsoleReport();
						}
						else
						{
							ReportingBase.instance = new LogUtilityReport();
						}
					}
				}
			}
			return ReportingBase.instance;
		}

		// Token: 0x040000B6 RID: 182
		private static bool useInternalLogger = true;

		// Token: 0x040000B7 RID: 183
		private static bool enableDebugMessage = true;

		// Token: 0x040000B8 RID: 184
		private static volatile ReportingBase instance;

		// Token: 0x040000B9 RID: 185
		private static object syncRoot = new object();
	}
}
