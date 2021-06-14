using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000054 RID: 84
	public class IULogger : IDeploymentLogger
	{
		// Token: 0x06000255 RID: 597 RVA: 0x0000B364 File Offset: 0x00009564
		public IULogger()
		{
			this.MinLogLevel = LoggingLevel.Debug;
			this.LoggingMessage.Add(LoggingLevel.Debug, "DEBUG");
			this.LoggingMessage.Add(LoggingLevel.Info, "INFO");
			this.LoggingMessage.Add(LoggingLevel.Warning, "WARNING");
			this.LoggingMessage.Add(LoggingLevel.Error, "ERROR");
			this.LoggingFunctions.Add(LoggingLevel.Debug, new LogString(IULogger.LogToConsole));
			this.LoggingFunctions.Add(LoggingLevel.Info, new LogString(IULogger.LogToConsole));
			this.LoggingFunctions.Add(LoggingLevel.Warning, new LogString(IULogger.LogToError));
			this.LoggingFunctions.Add(LoggingLevel.Error, new LogString(IULogger.LogToError));
			this.LoggingColors.Add(LoggingLevel.Debug, ConsoleColor.DarkGray);
			this.LoggingColors.Add(LoggingLevel.Info, ConsoleColor.Gray);
			this.LoggingColors.Add(LoggingLevel.Warning, ConsoleColor.Yellow);
			this.LoggingColors.Add(LoggingLevel.Error, ConsoleColor.Red);
		}

		// Token: 0x06000256 RID: 598 RVA: 0x0000B479 File Offset: 0x00009679
		public static void LogToConsole(string format, params object[] list)
		{
			if (list.Length != 0)
			{
				Console.WriteLine(string.Format(CultureInfo.CurrentCulture, format, list));
				return;
			}
			Console.WriteLine(format);
		}

		// Token: 0x06000257 RID: 599 RVA: 0x0000B497 File Offset: 0x00009697
		public static void LogToError(string format, params object[] list)
		{
			if (list.Length != 0)
			{
				Console.Error.WriteLine(string.Format(CultureInfo.CurrentCulture, format, list));
				return;
			}
			Console.Error.WriteLine(format);
		}

		// Token: 0x06000258 RID: 600 RVA: 0x0000964B File Offset: 0x0000784B
		public static void LogToNull(string format, params object[] list)
		{
		}

		// Token: 0x06000259 RID: 601 RVA: 0x0000B4BF File Offset: 0x000096BF
		public void SetLoggingLevel(LoggingLevel level)
		{
			this.MinLogLevel = level;
		}

		// Token: 0x0600025A RID: 602 RVA: 0x0000B4C8 File Offset: 0x000096C8
		public void SetLogFunction(LoggingLevel level, LogString logFunc)
		{
			if (logFunc == null)
			{
				this.LoggingFunctions[level] = new LogString(IULogger.LogToNull);
				return;
			}
			this.LoggingFunctions[level] = logFunc;
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x0600025B RID: 603 RVA: 0x0000B4F3 File Offset: 0x000096F3
		// (set) Token: 0x0600025C RID: 604 RVA: 0x0000B4FB File Offset: 0x000096FB
		public ConsoleColor OverrideColor
		{
			get
			{
				return this._overrideColor;
			}
			set
			{
				this._overrideColor = value;
			}
		}

		// Token: 0x0600025D RID: 605 RVA: 0x0000B504 File Offset: 0x00009704
		public void ResetOverrideColor()
		{
			this._overrideColor = ConsoleColor.Black;
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x0600025E RID: 606 RVA: 0x0000B50D File Offset: 0x0000970D
		public bool UseOverrideColor
		{
			get
			{
				return this._overrideColor > ConsoleColor.Black;
			}
		}

		// Token: 0x0600025F RID: 607 RVA: 0x0000B518 File Offset: 0x00009718
		public void Log(LoggingLevel level, string format, params object[] list)
		{
			if (level >= this.MinLogLevel)
			{
				ConsoleColor foregroundColor = Console.ForegroundColor;
				Console.ForegroundColor = (this.UseOverrideColor ? this._overrideColor : this.LoggingColors[level]);
				this.LoggingFunctions[level](format, list);
				Console.ForegroundColor = foregroundColor;
			}
		}

		// Token: 0x06000260 RID: 608 RVA: 0x0000B56C File Offset: 0x0000976C
		public void LogException(Exception exp)
		{
			this.LogException(exp, LoggingLevel.Error);
		}

		// Token: 0x06000261 RID: 609 RVA: 0x0000B578 File Offset: 0x00009778
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
			stringBuilder.Append(string.Format("{0}: {1}{2}", this.LoggingMessage[level], "0x" + Marshal.GetHRForException(exp).ToString("X"), Environment.NewLine));
			stringBuilder.Append(string.Format("{0}:{1}", Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().ProcessName), Environment.NewLine));
			stringBuilder.Append(string.Format("EXCEPTION: {0}{1}", exp.ToString(), Environment.NewLine));
			this.Log(level, stringBuilder.ToString(), new object[0]);
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0000B682 File Offset: 0x00009882
		public void LogError(string format, params object[] list)
		{
			this.Log(LoggingLevel.Error, format, list);
		}

		// Token: 0x06000263 RID: 611 RVA: 0x0000B68D File Offset: 0x0000988D
		public void LogWarning(string format, params object[] list)
		{
			this.Log(LoggingLevel.Warning, format, list);
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0000B698 File Offset: 0x00009898
		public void LogInfo(string format, params object[] list)
		{
			this.Log(LoggingLevel.Info, format, list);
		}

		// Token: 0x06000265 RID: 613 RVA: 0x0000B6A3 File Offset: 0x000098A3
		public void LogDebug(string format, params object[] list)
		{
			this.Log(LoggingLevel.Debug, format, list);
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000266 RID: 614 RVA: 0x0000B6AE File Offset: 0x000098AE
		// (set) Token: 0x06000267 RID: 615 RVA: 0x0000B6BC File Offset: 0x000098BC
		public LogString ErrorLogger
		{
			get
			{
				return this.LoggingFunctions[LoggingLevel.Error];
			}
			set
			{
				this.SetLogFunction(LoggingLevel.Error, value);
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000268 RID: 616 RVA: 0x0000B6C6 File Offset: 0x000098C6
		// (set) Token: 0x06000269 RID: 617 RVA: 0x0000B6D4 File Offset: 0x000098D4
		public LogString WarningLogger
		{
			get
			{
				return this.LoggingFunctions[LoggingLevel.Warning];
			}
			set
			{
				this.SetLogFunction(LoggingLevel.Warning, value);
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x0600026A RID: 618 RVA: 0x0000B6DE File Offset: 0x000098DE
		// (set) Token: 0x0600026B RID: 619 RVA: 0x0000B6EC File Offset: 0x000098EC
		public LogString InformationLogger
		{
			get
			{
				return this.LoggingFunctions[LoggingLevel.Info];
			}
			set
			{
				this.SetLogFunction(LoggingLevel.Info, value);
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x0600026C RID: 620 RVA: 0x0000B6F6 File Offset: 0x000098F6
		// (set) Token: 0x0600026D RID: 621 RVA: 0x0000B704 File Offset: 0x00009904
		public LogString DebugLogger
		{
			get
			{
				return this.LoggingFunctions[LoggingLevel.Debug];
			}
			set
			{
				this.SetLogFunction(LoggingLevel.Debug, value);
			}
		}

		// Token: 0x04000117 RID: 279
		private LoggingLevel MinLogLevel;

		// Token: 0x04000118 RID: 280
		private Dictionary<LoggingLevel, string> LoggingMessage = new Dictionary<LoggingLevel, string>();

		// Token: 0x04000119 RID: 281
		private Dictionary<LoggingLevel, LogString> LoggingFunctions = new Dictionary<LoggingLevel, LogString>();

		// Token: 0x0400011A RID: 282
		private Dictionary<LoggingLevel, ConsoleColor> LoggingColors = new Dictionary<LoggingLevel, ConsoleColor>();

		// Token: 0x0400011B RID: 283
		private ConsoleColor _overrideColor;
	}
}
