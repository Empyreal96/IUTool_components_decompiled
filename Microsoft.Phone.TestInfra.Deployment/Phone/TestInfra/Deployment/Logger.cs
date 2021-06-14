using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000021 RID: 33
	public static class Logger
	{
		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000173 RID: 371 RVA: 0x00009350 File Offset: 0x00007550
		// (remove) Token: 0x06000174 RID: 372 RVA: 0x00009384 File Offset: 0x00007584
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event EventHandler<LogEventArgs> OnLogMessage;

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000175 RID: 373 RVA: 0x000093B8 File Offset: 0x000075B8
		public static TextWriter Writer
		{
			get
			{
				return Logger.fileWriter;
			}
		}

		// Token: 0x06000176 RID: 374 RVA: 0x000093D0 File Offset: 0x000075D0
		public static void Configure(TraceLevel consoleLogLevel, TraceLevel fileLogLevel, string logFile, bool append)
		{
			object syncRoot = Logger.SyncRoot;
			lock (syncRoot)
			{
				Logger.Close();
				Logger.consoleLevel = consoleLogLevel;
				Logger.fileLevel = fileLogLevel;
				bool flag2 = string.IsNullOrEmpty(logFile);
				if (!flag2)
				{
					string directoryName = Path.GetDirectoryName(logFile);
					bool flag3 = !string.IsNullOrEmpty(directoryName);
					if (flag3)
					{
						Directory.CreateDirectory(directoryName);
					}
					Logger.fileWriter = new StreamWriter(logFile, append)
					{
						AutoFlush = true
					};
				}
			}
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00009468 File Offset: 0x00007668
		public static void Close()
		{
			object syncRoot = Logger.SyncRoot;
			lock (syncRoot)
			{
				Logger.consoleLevel = TraceLevel.Off;
				Logger.fileLevel = TraceLevel.Off;
				bool flag2 = Logger.fileWriter != null;
				if (flag2)
				{
					Logger.fileWriter.Dispose();
					Logger.fileWriter = null;
				}
			}
		}

		// Token: 0x06000178 RID: 376 RVA: 0x000094D4 File Offset: 0x000076D4
		public static void Debug(string message, params object[] args)
		{
			Logger.LogMessage(TraceLevel.Verbose, message, args);
		}

		// Token: 0x06000179 RID: 377 RVA: 0x000094E0 File Offset: 0x000076E0
		public static void Info(string message, params object[] args)
		{
			Logger.LogMessage(TraceLevel.Info, message, args);
		}

		// Token: 0x0600017A RID: 378 RVA: 0x000094EC File Offset: 0x000076EC
		public static void Warning(string message, params object[] args)
		{
			Logger.LogMessage(TraceLevel.Warning, message, args);
		}

		// Token: 0x0600017B RID: 379 RVA: 0x000094F8 File Offset: 0x000076F8
		public static void Error(string message, params object[] args)
		{
			Logger.LogMessage(TraceLevel.Error, message, args);
		}

		// Token: 0x0600017C RID: 380 RVA: 0x00009504 File Offset: 0x00007704
		private static void LogMessage(TraceLevel logLevel, string message, params object[] args)
		{
			string message2 = string.Format(CultureInfo.InvariantCulture, message, args);
			string message3 = Logger.FormatMessage(logLevel, message2, new object[0]);
			Logger.LogToConsole(logLevel, message2);
			Logger.LogToTrace(logLevel, message3);
			Logger.LogToFile(logLevel, message3);
			bool flag = Logger.OnLogMessage != null;
			if (flag)
			{
				Logger.OnLogMessage(null, new LogEventArgs(message3, logLevel));
			}
		}

		// Token: 0x0600017D RID: 381 RVA: 0x00009568 File Offset: 0x00007768
		private static void LogToConsole(TraceLevel logLevel, string message)
		{
			bool flag = logLevel <= Logger.consoleLevel;
			if (flag)
			{
				ConsoleColor foregroundColor = Console.ForegroundColor;
				try
				{
					switch (logLevel)
					{
					case TraceLevel.Off:
						goto IL_73;
					case TraceLevel.Error:
						Console.ForegroundColor = ConsoleColor.Red;
						Console.Error.WriteLine(message);
						goto IL_73;
					case TraceLevel.Warning:
						Console.ForegroundColor = ConsoleColor.Yellow;
						break;
					case TraceLevel.Info:
					case TraceLevel.Verbose:
						break;
					default:
						throw new ArgumentOutOfRangeException("logLevel", logLevel, null);
					}
					Console.WriteLine(message);
					IL_73:;
				}
				finally
				{
					Console.ForegroundColor = foregroundColor;
				}
			}
		}

		// Token: 0x0600017E RID: 382 RVA: 0x00009608 File Offset: 0x00007808
		private static void LogToTrace(TraceLevel logLevel, string message)
		{
			switch (logLevel)
			{
			case TraceLevel.Error:
				Trace.TraceError(message);
				break;
			case TraceLevel.Warning:
				Trace.TraceWarning(message);
				break;
			case TraceLevel.Info:
				Trace.TraceInformation(message);
				break;
			default:
				Trace.WriteLine(message);
				break;
			}
		}

		// Token: 0x0600017F RID: 383 RVA: 0x00009654 File Offset: 0x00007854
		private static void LogToFile(TraceLevel logLevel, string message)
		{
			bool flag = Logger.fileWriter != null;
			if (flag)
			{
				bool flag2 = Logger.fileLevel > TraceLevel.Off && logLevel <= Logger.fileLevel;
				if (flag2)
				{
					Logger.fileWriter.WriteLine(message);
				}
			}
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00009698 File Offset: 0x00007898
		private static string FormatMessage(TraceLevel logLevel, string message, params object[] args)
		{
			string str = string.Format(CultureInfo.InvariantCulture, "{0:MMM dd HH:mm:ss.ffff} {1}: ", new object[]
			{
				DateTime.Now,
				logLevel
			});
			bool flag = args != null && args.Any<object>();
			if (flag)
			{
				try
				{
					message = string.Format(CultureInfo.InvariantCulture, message, args);
				}
				catch
				{
				}
			}
			return str + message;
		}

		// Token: 0x04000093 RID: 147
		private static readonly object SyncRoot = new object();

		// Token: 0x04000094 RID: 148
		private static StreamWriter fileWriter;

		// Token: 0x04000095 RID: 149
		private static TraceLevel consoleLevel = TraceLevel.Off;

		// Token: 0x04000096 RID: 150
		private static TraceLevel fileLevel = TraceLevel.Off;
	}
}
