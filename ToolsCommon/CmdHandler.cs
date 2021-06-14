using System;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000003 RID: 3
	public abstract class CmdHandler
	{
		// Token: 0x0600003E RID: 62
		protected abstract int DoExecution();

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600003F RID: 63
		public abstract string Command { get; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000040 RID: 64
		public abstract string Description { get; }

		// Token: 0x06000041 RID: 65 RVA: 0x000034EC File Offset: 0x000016EC
		public int Execute(string cmdParams, string applicationName)
		{
			if (!this._cmdLineParser.ParseString("appName " + cmdParams, true))
			{
				string appName = applicationName + " " + this.Command;
				LogUtil.Message(this._cmdLineParser.UsageString(appName));
				return -1;
			}
			return this.DoExecution();
		}

		// Token: 0x04000018 RID: 24
		protected CommandLineParser _cmdLineParser = new CommandLineParser();
	}
}
