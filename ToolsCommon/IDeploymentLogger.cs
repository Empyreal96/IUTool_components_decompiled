using System;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000053 RID: 83
	public interface IDeploymentLogger
	{
		// Token: 0x0600024E RID: 590
		void Log(LoggingLevel level, string format, params object[] list);

		// Token: 0x0600024F RID: 591
		void LogException(Exception exp);

		// Token: 0x06000250 RID: 592
		void LogException(Exception exp, LoggingLevel level);

		// Token: 0x06000251 RID: 593
		void LogDebug(string format, params object[] list);

		// Token: 0x06000252 RID: 594
		void LogInfo(string format, params object[] list);

		// Token: 0x06000253 RID: 595
		void LogWarning(string format, params object[] list);

		// Token: 0x06000254 RID: 596
		void LogError(string format, params object[] list);
	}
}
