using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000036 RID: 54
	public static class LogUtil
	{
		// Token: 0x060001BF RID: 447
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern void IU_LogTo([MarshalAs(UnmanagedType.FunctionPtr)] LogUtil.InteropLogString ErrorMsgHandler, [MarshalAs(UnmanagedType.FunctionPtr)] LogUtil.InteropLogString WarningMsgHandler, [MarshalAs(UnmanagedType.FunctionPtr)] LogUtil.InteropLogString InfoMsgHandler, [MarshalAs(UnmanagedType.FunctionPtr)] LogUtil.InteropLogString DebugMsgHandler);

		// Token: 0x060001C0 RID: 448
		[DllImport("ConvertDSMDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern void ConvertDSM_LogTo([MarshalAs(UnmanagedType.FunctionPtr)] LogUtil.InteropLogString ErrorMsgHandler, [MarshalAs(UnmanagedType.FunctionPtr)] LogUtil.InteropLogString WarningMsgHandler, [MarshalAs(UnmanagedType.FunctionPtr)] LogUtil.InteropLogString InfoMsgHandler, [MarshalAs(UnmanagedType.FunctionPtr)] LogUtil.InteropLogString DebugMsgHandler);

		// Token: 0x060001C1 RID: 449 RVA: 0x00008194 File Offset: 0x00006394
		static LogUtil()
		{
			LogUtil._msgPrefix.Add(LogUtil.MessageLevel.Debug, "diagnostic ");
			LogUtil._msgPrefix.Add(LogUtil.MessageLevel.Message, "info");
			LogUtil._msgPrefix.Add(LogUtil.MessageLevel.Warning, "warning ");
			LogUtil._msgPrefix.Add(LogUtil.MessageLevel.Error, "fatal error PKG");
			LogUtil._msgColor.Add(LogUtil.MessageLevel.Debug, ConsoleColor.DarkGray);
			LogUtil._msgColor.Add(LogUtil.MessageLevel.Message, ConsoleColor.Gray);
			LogUtil._msgColor.Add(LogUtil.MessageLevel.Warning, ConsoleColor.Yellow);
			LogUtil._msgColor.Add(LogUtil.MessageLevel.Error, ConsoleColor.Red);
			LogUtil.IULogTo(new LogUtil.InteropLogString(LogUtil.Error), new LogUtil.InteropLogString(LogUtil.Warning), new LogUtil.InteropLogString(LogUtil.Message), new LogUtil.InteropLogString(LogUtil.Diagnostic));
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000827C File Offset: 0x0000647C
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

		// Token: 0x060001C3 RID: 451 RVA: 0x000082EE File Offset: 0x000064EE
		private static void LogMessage(LogUtil.MessageLevel level, string format, params object[] args)
		{
			LogUtil.LogMessage(level, string.Format(CultureInfo.InvariantCulture, format, args));
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x00008302 File Offset: 0x00006502
		public static void SetVerbosity(bool enabled)
		{
			LogUtil._verbose = enabled;
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0000830A File Offset: 0x0000650A
		public static void Error(string message)
		{
			LogUtil.LogMessage(LogUtil.MessageLevel.Error, message);
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x00008313 File Offset: 0x00006513
		public static void Error(string format, params object[] args)
		{
			LogUtil.LogMessage(LogUtil.MessageLevel.Error, format, args);
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000831D File Offset: 0x0000651D
		public static void Warning(string message)
		{
			message = "PKG-W: " + message;
			LogUtil.LogMessage(LogUtil.MessageLevel.Warning, message);
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x00008333 File Offset: 0x00006533
		public static void Warning(string format, params object[] args)
		{
			format = "PKG-W: " + format;
			LogUtil.LogMessage(LogUtil.MessageLevel.Warning, format, args);
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0000834A File Offset: 0x0000654A
		public static void Message(string msg)
		{
			LogUtil.LogMessage(LogUtil.MessageLevel.Message, msg);
		}

		// Token: 0x060001CA RID: 458 RVA: 0x00008353 File Offset: 0x00006553
		public static void Message(string format, params object[] args)
		{
			LogUtil.LogMessage(LogUtil.MessageLevel.Message, format, args);
		}

		// Token: 0x060001CB RID: 459 RVA: 0x0000835D File Offset: 0x0000655D
		public static void Diagnostic(string message)
		{
			LogUtil.LogMessage(LogUtil.MessageLevel.Debug, message);
		}

		// Token: 0x060001CC RID: 460 RVA: 0x00008366 File Offset: 0x00006566
		public static void Diagnostic(string format, params object[] args)
		{
			LogUtil.LogMessage(LogUtil.MessageLevel.Debug, format, args);
		}

		// Token: 0x060001CD RID: 461 RVA: 0x00008370 File Offset: 0x00006570
		public static void LogCopyright()
		{
			Console.WriteLine(CommonUtils.GetCopyrightString());
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000837C File Offset: 0x0000657C
		public static void IULogTo(LogUtil.InteropLogString errorLogger, LogUtil.InteropLogString warningLogger, LogUtil.InteropLogString msgLogger, LogUtil.InteropLogString debugLogger)
		{
			LogUtil._iuErrorLogger = errorLogger;
			LogUtil._iuWarningLogger = warningLogger;
			LogUtil._iuMsgLogger = msgLogger;
			LogUtil._iuDebugLogger = debugLogger;
			LogUtil.IU_LogTo(errorLogger, warningLogger, msgLogger, debugLogger);
			LogUtil.ConvertDSM_LogTo(errorLogger, warningLogger, msgLogger, debugLogger);
		}

		// Token: 0x04000090 RID: 144
		private static bool _verbose = false;

		// Token: 0x04000091 RID: 145
		private static Dictionary<LogUtil.MessageLevel, string> _msgPrefix = new Dictionary<LogUtil.MessageLevel, string>();

		// Token: 0x04000092 RID: 146
		private static Dictionary<LogUtil.MessageLevel, ConsoleColor> _msgColor = new Dictionary<LogUtil.MessageLevel, ConsoleColor>();

		// Token: 0x04000093 RID: 147
		private static LogUtil.InteropLogString _iuErrorLogger = null;

		// Token: 0x04000094 RID: 148
		private static LogUtil.InteropLogString _iuWarningLogger = null;

		// Token: 0x04000095 RID: 149
		private static LogUtil.InteropLogString _iuMsgLogger = null;

		// Token: 0x04000096 RID: 150
		private static LogUtil.InteropLogString _iuDebugLogger = null;

		// Token: 0x0200004E RID: 78
		private enum MessageLevel
		{
			// Token: 0x04000119 RID: 281
			Error,
			// Token: 0x0400011A RID: 282
			Warning,
			// Token: 0x0400011B RID: 283
			Message,
			// Token: 0x0400011C RID: 284
			Debug
		}

		// Token: 0x0200004F RID: 79
		// (Invoke) Token: 0x06000201 RID: 513
		public delegate void InteropLogString([MarshalAs(UnmanagedType.LPWStr)] string outputStr);
	}
}
