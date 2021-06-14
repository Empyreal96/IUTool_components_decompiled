using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Microsoft.Composition.ToolBox.Logging
{
	// Token: 0x02000016 RID: 22
	public class Logger : ILogger
	{
		// Token: 0x06000061 RID: 97 RVA: 0x00003014 File Offset: 0x00001214
		public Logger()
		{
			this.minLogLevel = LoggingLevel.Debug;
			this.loggingMessage.Add(LoggingLevel.Debug, "DEBUG");
			this.loggingMessage.Add(LoggingLevel.Info, "INFO");
			this.loggingMessage.Add(LoggingLevel.Warning, "WARNING");
			this.loggingMessage.Add(LoggingLevel.Error, "ERROR");
			this.loggingFunctions.Add(LoggingLevel.Debug, new LoggingFunc(Logger.LogToConsole));
			this.loggingFunctions.Add(LoggingLevel.Info, new LoggingFunc(Logger.LogToConsole));
			this.loggingFunctions.Add(LoggingLevel.Warning, new LoggingFunc(Logger.LogToConsole));
			this.loggingFunctions.Add(LoggingLevel.Error, new LoggingFunc(Logger.LogtoError));
			this.loggingColors.Add(LoggingLevel.Debug, ConsoleColor.DarkGray);
			this.loggingColors.Add(LoggingLevel.Info, ConsoleColor.Gray);
			this.loggingColors.Add(LoggingLevel.Warning, ConsoleColor.Yellow);
			this.loggingColors.Add(LoggingLevel.Error, ConsoleColor.Red);
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00003129 File Offset: 0x00001329
		public static void LogToConsole(string format, params object[] list)
		{
			Console.WriteLine(string.Format(CultureInfo.CurrentCulture, format, list));
		}

		// Token: 0x06000063 RID: 99 RVA: 0x0000313C File Offset: 0x0000133C
		public static void LogToNull(string format, params object[] list)
		{
		}

		// Token: 0x06000064 RID: 100 RVA: 0x0000313E File Offset: 0x0000133E
		public void SetLoggingLevel(LoggingLevel level)
		{
			this.minLogLevel = level;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00003147 File Offset: 0x00001347
		public void SetLogFunction(LoggingLevel level, LoggingFunc logFunc)
		{
			if (logFunc == null)
			{
				this.loggingFunctions[level] = new LoggingFunc(Logger.LogToNull);
				return;
			}
			this.loggingFunctions[level] = logFunc;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00003174 File Offset: 0x00001374
		public void Log(LoggingLevel level, string format, params object[] list)
		{
			if (level >= this.minLogLevel)
			{
				string format2 = string.Format("ThreadId{0} {1}: {2} ", Thread.CurrentThread.ManagedThreadId.ToString(), level.ToString().ToUpper(), format);
				ConsoleColor foregroundColor = Console.ForegroundColor;
				Console.ForegroundColor = this.loggingColors[level];
				this.loggingFunctions[level](format2, list);
				Console.ForegroundColor = foregroundColor;
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x000031E8 File Offset: 0x000013E8
		public void LogException(Exception exp)
		{
			this.LogException(exp, LoggingLevel.Error);
		}

		// Token: 0x06000068 RID: 104 RVA: 0x000031F4 File Offset: 0x000013F4
		public void LogException(Exception exp, LoggingLevel level)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StackTrace stackTrace = new StackTrace(exp, true);
			if (stackTrace.FrameCount > 0)
			{
				StackTrace stackTrace2 = stackTrace;
				StackFrame frame = stackTrace2.GetFrame(stackTrace2.FrameCount - 1);
				if (frame != null)
				{
					string arg = string.Format("{0}({1},{2}):", frame.GetFileName(), frame.GetFileLineNumber(), frame.GetFileColumnNumber());
					stringBuilder.Append(string.Format("{0}{1}", arg, Environment.NewLine));
				}
			}
			stringBuilder.Append(string.Format("{0}: {1}{2}", this.loggingMessage[level], "0x" + Marshal.GetHRForException(exp).ToString("X"), Environment.NewLine));
			stringBuilder.Append(string.Format("{0}:{1}", Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().ProcessName), Environment.NewLine));
			stringBuilder.Append(string.Format("EXCEPTION: {0}{1}", exp.Message, Environment.NewLine));
			stringBuilder.Append(string.Format("STACKTRACE:{1}{0}{1}", exp.StackTrace, Environment.NewLine));
			this.Log(level, stringBuilder.ToString(), new object[0]);
		}

		// Token: 0x06000069 RID: 105 RVA: 0x0000331A File Offset: 0x0000151A
		public void LogError(string format, params object[] list)
		{
			this.Log(LoggingLevel.Error, format, list);
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00003325 File Offset: 0x00001525
		public void LogWarning(string format, params object[] list)
		{
			this.Log(LoggingLevel.Warning, format, list);
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00003330 File Offset: 0x00001530
		public void LogInfo(string format, params object[] list)
		{
			this.Log(LoggingLevel.Info, format, list);
		}

		// Token: 0x0600006C RID: 108 RVA: 0x0000333B File Offset: 0x0000153B
		public void LogDebug(string format, params object[] list)
		{
			this.Log(LoggingLevel.Debug, format, list);
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00003346 File Offset: 0x00001546
		private static void LogtoError(string format, params object[] list)
		{
			Console.Error.WriteLine(string.Format(CultureInfo.CurrentCulture, format, list));
		}

		// Token: 0x0400005A RID: 90
		private LoggingLevel minLogLevel;

		// Token: 0x0400005B RID: 91
		private Dictionary<LoggingLevel, string> loggingMessage = new Dictionary<LoggingLevel, string>();

		// Token: 0x0400005C RID: 92
		private Dictionary<LoggingLevel, LoggingFunc> loggingFunctions = new Dictionary<LoggingLevel, LoggingFunc>();

		// Token: 0x0400005D RID: 93
		private Dictionary<LoggingLevel, ConsoleColor> loggingColors = new Dictionary<LoggingLevel, ConsoleColor>();
	}
}
