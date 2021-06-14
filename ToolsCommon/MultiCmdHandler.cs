using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000005 RID: 5
	public class MultiCmdHandler
	{
		// Token: 0x06000046 RID: 70 RVA: 0x0000358C File Offset: 0x0000178C
		private void ShowUsage()
		{
			LogUtil.Message("Usage: {0} <command> <parameters>", new object[]
			{
				this.appName
			});
			LogUtil.Message("\t available command:");
			foreach (KeyValuePair<string, CmdHandler> keyValuePair in this._allHandlers)
			{
				LogUtil.Message("\t\t{0}:{1}", new object[]
				{
					keyValuePair.Value.Command,
					keyValuePair.Value.Description
				});
			}
			LogUtil.Message("\t Run {0} <command> /? to check command line parameters for each command", new object[]
			{
				this.appName
			});
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003644 File Offset: 0x00001844
		public void AddCmdHandler(CmdHandler cmdHandler)
		{
			this._allHandlers.Add(cmdHandler.Command, cmdHandler);
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00003658 File Offset: 0x00001858
		public int Run(string[] args)
		{
			if (args.Length < 1)
			{
				this.ShowUsage();
				return -1;
			}
			int result = -1;
			string cmdParams = (args.Length > 1) ? string.Join(" ", args.Skip(1)) : string.Empty;
			CmdHandler cmdHandler = null;
			if (!this._allHandlers.TryGetValue(args[0], out cmdHandler))
			{
				this.ShowUsage();
			}
			else
			{
				result = cmdHandler.Execute(cmdParams, this.appName);
			}
			return result;
		}

		// Token: 0x0400001A RID: 26
		private string appName = new FileInfo(Environment.GetCommandLineArgs()[0]).Name;

		// Token: 0x0400001B RID: 27
		private Dictionary<string, CmdHandler> _allHandlers = new Dictionary<string, CmdHandler>(StringComparer.OrdinalIgnoreCase);
	}
}
