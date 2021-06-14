using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000028 RID: 40
	public static class LogUtil
	{
		// Token: 0x0600014C RID: 332
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern void IU_LogTo([MarshalAs(UnmanagedType.FunctionPtr)] LogUtil.InteropLogString ErrorMsgHandler, [MarshalAs(UnmanagedType.FunctionPtr)] LogUtil.InteropLogString WarningMsgHandler, [MarshalAs(UnmanagedType.FunctionPtr)] LogUtil.InteropLogString InfoMsgHandler, [MarshalAs(UnmanagedType.FunctionPtr)] LogUtil.InteropLogString DebugMsgHandler);

		// Token: 0x0600014D RID: 333 RVA: 0x00007EF4 File Offset: 0x000060F4
		static LogUtil()
		{
			LogUtil._msgPrefix.Add(LogUtil.MessageLevel.Debug, "diagnostic");
			LogUtil._msgPrefix.Add(LogUtil.MessageLevel.Message, "info");
			LogUtil._msgPrefix.Add(LogUtil.MessageLevel.Warning, "warning ");
			LogUtil._msgPrefix.Add(LogUtil.MessageLevel.Error, "fatal error ");
			LogUtil._msgColor.Add(LogUtil.MessageLevel.Debug, ConsoleColor.DarkGray);
			LogUtil._msgColor.Add(LogUtil.MessageLevel.Message, ConsoleColor.Gray);
			LogUtil._msgColor.Add(LogUtil.MessageLevel.Warning, ConsoleColor.Yellow);
			LogUtil._msgColor.Add(LogUtil.MessageLevel.Error, ConsoleColor.Red);
			LogUtil.IULogTo(new LogUtil.InteropLogString(LogUtil.Error), new LogUtil.InteropLogString(LogUtil.Warning), new LogUtil.InteropLogString(LogUtil.Message), new LogUtil.InteropLogString(LogUtil.Diagnostic));
		}

		// Token: 0x0600014E RID: 334 RVA: 0x00007FDC File Offset: 0x000061DC
		private static void LogMessage(LogUtil.MessageLevel level, string message)
		{
			if (level != LogUtil.MessageLevel.Debug || LogUtil._verbose)
			{
				foreach (string arg in message.Split(new char[]
				{
					'\r',
					'\n'
				}, StringSplitOptions.RemoveEmptyEntries))
				{
					ConsoleColor foregroundColor = Console.ForegroundColor;
					Console.ForegroundColor = LogUtil._msgColor[level];
					Console.WriteLine("{0}: {1}", LogUtil._msgPrefix[level], arg);
					Console.ForegroundColor = foregroundColor;
				}
			}
		}

		// Token: 0x0600014F RID: 335 RVA: 0x0000804E File Offset: 0x0000624E
		private static void LogMessage(LogUtil.MessageLevel level, string format, params object[] args)
		{
			LogUtil.LogMessage(level, string.Format(format, args));
		}

		// Token: 0x06000150 RID: 336 RVA: 0x0000805D File Offset: 0x0000625D
		public static void SetVerbosity(bool enabled)
		{
			LogUtil._verbose = enabled;
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00008065 File Offset: 0x00006265
		public static void Error(string message)
		{
			LogUtil.LogMessage(LogUtil.MessageLevel.Error, message);
		}

		// Token: 0x06000152 RID: 338 RVA: 0x0000806E File Offset: 0x0000626E
		public static void Error(string format, params object[] args)
		{
			LogUtil.LogMessage(LogUtil.MessageLevel.Error, format, args);
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00008078 File Offset: 0x00006278
		public static void Warning(string message)
		{
			LogUtil.LogMessage(LogUtil.MessageLevel.Warning, message);
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00008081 File Offset: 0x00006281
		public static void Warning(string format, params object[] args)
		{
			LogUtil.LogMessage(LogUtil.MessageLevel.Warning, format, args);
		}

		// Token: 0x06000155 RID: 341 RVA: 0x0000808B File Offset: 0x0000628B
		public static void Message(string input)
		{
			LogUtil.LogMessage(LogUtil.MessageLevel.Message, input);
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00008094 File Offset: 0x00006294
		public static void Message(string format, params object[] args)
		{
			LogUtil.LogMessage(LogUtil.MessageLevel.Message, format, args);
		}

		// Token: 0x06000157 RID: 343 RVA: 0x0000809E File Offset: 0x0000629E
		public static void Diagnostic(string message)
		{
			LogUtil.LogMessage(LogUtil.MessageLevel.Debug, message);
		}

		// Token: 0x06000158 RID: 344 RVA: 0x000080A7 File Offset: 0x000062A7
		public static void Diagnostic(string format, params object[] args)
		{
			LogUtil.LogMessage(LogUtil.MessageLevel.Debug, format, args);
		}

		// Token: 0x06000159 RID: 345 RVA: 0x000080B1 File Offset: 0x000062B1
		public static void LogCopyright()
		{
			Console.WriteLine(CommonUtils.GetCopyrightString());
		}

		// Token: 0x0600015A RID: 346 RVA: 0x000080BD File Offset: 0x000062BD
		public static void IULogTo(LogUtil.InteropLogString errorLogger, LogUtil.InteropLogString warningLogger, LogUtil.InteropLogString msgLogger, LogUtil.InteropLogString debugLogger)
		{
			LogUtil._iuErrorLogger = errorLogger;
			LogUtil._iuWarningLogger = warningLogger;
			LogUtil._iuMsgLogger = msgLogger;
			LogUtil._iuDebugLogger = debugLogger;
			LogUtil.IU_LogTo(errorLogger, warningLogger, msgLogger, debugLogger);
		}

		// Token: 0x04000079 RID: 121
		private static bool _verbose = false;

		// Token: 0x0400007A RID: 122
		private static Dictionary<LogUtil.MessageLevel, string> _msgPrefix = new Dictionary<LogUtil.MessageLevel, string>();

		// Token: 0x0400007B RID: 123
		private static Dictionary<LogUtil.MessageLevel, ConsoleColor> _msgColor = new Dictionary<LogUtil.MessageLevel, ConsoleColor>();

		// Token: 0x0400007C RID: 124
		private static LogUtil.InteropLogString _iuErrorLogger = null;

		// Token: 0x0400007D RID: 125
		private static LogUtil.InteropLogString _iuWarningLogger = null;

		// Token: 0x0400007E RID: 126
		private static LogUtil.InteropLogString _iuMsgLogger = null;

		// Token: 0x0400007F RID: 127
		private static LogUtil.InteropLogString _iuDebugLogger = null;

		// Token: 0x02000069 RID: 105
		private enum MessageLevel
		{
			// Token: 0x04000191 RID: 401
			Error,
			// Token: 0x04000192 RID: 402
			Warning,
			// Token: 0x04000193 RID: 403
			Message,
			// Token: 0x04000194 RID: 404
			Debug
		}

		// Token: 0x0200006A RID: 106
		// (Invoke) Token: 0x060002BE RID: 702
		public delegate void InteropLogString([MarshalAs(UnmanagedType.LPWStr)] string outputStr);
	}
}
