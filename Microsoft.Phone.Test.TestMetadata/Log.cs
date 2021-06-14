using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Phone.Test.TestMetadata
{
	// Token: 0x02000002 RID: 2
	public static class Log
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		// (set) Token: 0x06000002 RID: 2 RVA: 0x00002057 File Offset: 0x00000257
		public static bool LogErrorAsWarning { get; set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000003 RID: 3 RVA: 0x0000205F File Offset: 0x0000025F
		// (set) Token: 0x06000004 RID: 4 RVA: 0x00002066 File Offset: 0x00000266
		public static string LogFile { get; set; }

		// Token: 0x06000005 RID: 5 RVA: 0x00002070 File Offset: 0x00000270
		static Log()
		{
			Log.s_msgPrefix.Add(Log.MessageLevel.Debug, "diagnostic");
			Log.s_msgPrefix.Add(Log.MessageLevel.Message, "info");
			Log.s_msgPrefix.Add(Log.MessageLevel.Warning, "warning ");
			Log.s_msgPrefix.Add(Log.MessageLevel.Error, "fatal error ");
			Log.s_msgColor.Add(Log.MessageLevel.Debug, ConsoleColor.DarkGray);
			Log.s_msgColor.Add(Log.MessageLevel.Message, ConsoleColor.Gray);
			Log.s_msgColor.Add(Log.MessageLevel.Warning, ConsoleColor.Yellow);
			Log.s_msgColor.Add(Log.MessageLevel.Error, ConsoleColor.Red);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002120 File Offset: 0x00000320
		private static void LogMessage(Log.MessageLevel level, string message)
		{
			bool flag = level != Log.MessageLevel.Debug || Log.s_verbose;
			if (flag)
			{
				string[] array = message.Split(new char[]
				{
					'\r',
					'\n'
				}, StringSplitOptions.RemoveEmptyEntries);
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string arg in array)
				{
					stringBuilder.AppendLine(string.Format("{0}: {1}", Log.s_msgPrefix[level], arg));
				}
				string text = stringBuilder.ToString();
				object obj = Log.s_logConsoleLock;
				lock (obj)
				{
					ConsoleColor foregroundColor = Console.ForegroundColor;
					Console.ForegroundColor = Log.s_msgColor[level];
					Console.Write(text);
					Console.ForegroundColor = foregroundColor;
				}
				bool flag3 = Log.LogFile != null && (level == Log.MessageLevel.Error || level == Log.MessageLevel.Warning);
				if (flag3)
				{
					object obj2 = Log.s_logFileLock;
					lock (obj2)
					{
						File.AppendAllText(Log.LogFile, text);
					}
				}
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000225C File Offset: 0x0000045C
		private static void LogMessage(Log.MessageLevel level, string format, params object[] args)
		{
			Log.LogMessage(level, string.Format(CultureInfo.InvariantCulture, format, args));
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002272 File Offset: 0x00000472
		public static void SetVerbosity(bool enabled)
		{
			Log.s_verbose = enabled;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x0000227C File Offset: 0x0000047C
		public static void Error(string format, params object[] args)
		{
			Log.MessageLevel level = Log.MessageLevel.Error;
			bool logErrorAsWarning = Log.LogErrorAsWarning;
			if (logErrorAsWarning)
			{
				level = Log.MessageLevel.Warning;
			}
			Log.LogMessage(level, format, args);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000022A2 File Offset: 0x000004A2
		public static void Warning(string format, params object[] args)
		{
			Log.LogMessage(Log.MessageLevel.Warning, format, args);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000022AE File Offset: 0x000004AE
		public static void Message(string format, params object[] args)
		{
			Log.LogMessage(Log.MessageLevel.Message, format, args);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000022BA File Offset: 0x000004BA
		public static void Diagnostic(string format, params object[] args)
		{
			Log.LogMessage(Log.MessageLevel.Debug, format, args);
		}

		// Token: 0x04000001 RID: 1
		private static bool s_verbose;

		// Token: 0x04000002 RID: 2
		private static readonly Dictionary<Log.MessageLevel, string> s_msgPrefix = new Dictionary<Log.MessageLevel, string>();

		// Token: 0x04000003 RID: 3
		private static readonly Dictionary<Log.MessageLevel, ConsoleColor> s_msgColor = new Dictionary<Log.MessageLevel, ConsoleColor>();

		// Token: 0x04000006 RID: 6
		private static readonly object s_logConsoleLock = new object();

		// Token: 0x04000007 RID: 7
		private static readonly object s_logFileLock = new object();

		// Token: 0x0200002C RID: 44
		private enum MessageLevel
		{
			// Token: 0x040000B9 RID: 185
			Error,
			// Token: 0x040000BA RID: 186
			Warning,
			// Token: 0x040000BB RID: 187
			Message,
			// Token: 0x040000BC RID: 188
			Debug
		}
	}
}
