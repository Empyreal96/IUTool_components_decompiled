using System;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000004 RID: 4
	public abstract class QuietCmdHandler : CmdHandler
	{
		// Token: 0x06000043 RID: 67 RVA: 0x00003550 File Offset: 0x00001750
		protected void SetLoggingVerbosity(IULogger logger)
		{
			if (this._cmdLineParser.GetSwitchAsBoolean("quiet"))
			{
				logger.SetLoggingLevel(LoggingLevel.Warning);
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x0000356B File Offset: 0x0000176B
		protected void SetQuietCommand()
		{
			this._cmdLineParser.SetOptionalSwitchBoolean("quiet", "When set only errors and warnings will be logged.", false);
		}

		// Token: 0x04000019 RID: 25
		private const string QUIET_SWITCH_NAME = "quiet";
	}
}
