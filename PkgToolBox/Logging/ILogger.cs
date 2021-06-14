using System;

namespace Microsoft.Composition.ToolBox.Logging
{
	// Token: 0x02000015 RID: 21
	public interface ILogger
	{
		// Token: 0x0600005B RID: 91
		void Log(LoggingLevel level, string format, params object[] list);

		// Token: 0x0600005C RID: 92
		void LogException(Exception exp);

		// Token: 0x0600005D RID: 93
		void LogException(Exception exp, LoggingLevel level);

		// Token: 0x0600005E RID: 94
		void LogInfo(string format, params object[] list);

		// Token: 0x0600005F RID: 95
		void LogWarning(string format, params object[] list);

		// Token: 0x06000060 RID: 96
		void LogError(string format, params object[] list);
	}
}
