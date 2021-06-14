using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.OemCustomizationTool
{
	// Token: 0x02000011 RID: 17
	internal class TraceLogger
	{
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600005B RID: 91 RVA: 0x00004541 File Offset: 0x00002741
		// (set) Token: 0x0600005C RID: 92 RVA: 0x00004548 File Offset: 0x00002748
		public static TraceLevel TraceLevel
		{
			get
			{
				return TraceLogger.traceLevel;
			}
			set
			{
				TraceLogger.traceLevel = value;
				LogUtil.SetVerbosity(value == TraceLevel.Info);
			}
		}

		// Token: 0x0600005D RID: 93 RVA: 0x0000455C File Offset: 0x0000275C
		public static void LogMessage(TraceLevel reqLevel, string msg, bool condition = true)
		{
			if (!condition)
			{
				return;
			}
			StackFrame stackFrame = new StackFrame(1);
			string name = stackFrame.GetMethod().Name;
			string name2 = stackFrame.GetMethod().DeclaringType.Name;
			string str = string.Concat(new string[]
			{
				"[OCT|",
				name2,
				".",
				name,
				"()]: "
			});
			if (reqLevel == TraceLevel.Error)
			{
				LogUtil.Error(str + msg);
			}
			else if (reqLevel == TraceLevel.Warn)
			{
				LogUtil.Warning(str + msg);
			}
			else if (reqLevel == TraceLevel.Info)
			{
				LogUtil.Diagnostic(str + msg);
			}
			if (reqLevel <= TraceLogger.TraceLevel)
			{
				string.Concat(new string[]
				{
					"[OCT|",
					reqLevel.ToString().ToUpper(CultureInfo.InvariantCulture),
					"|",
					name2,
					".",
					name,
					"()]: "
				});
			}
		}

		// Token: 0x0400004E RID: 78
		private static TraceLevel traceLevel = TraceLevel.Warn;
	}
}
