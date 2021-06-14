using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DeviceHealth
{
	// Token: 0x02000002 RID: 2
	public class Log
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
		// (set) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static Log.Level Verbosity
		{
			get
			{
				return Log.s_level;
			}
			set
			{
				Log.s_level = value;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000004 RID: 4 RVA: 0x0000206D File Offset: 0x0000026D
		// (set) Token: 0x06000003 RID: 3 RVA: 0x0000205F File Offset: 0x0000025F
		public static string Source
		{
			get
			{
				return Log.s_LogSource;
			}
			set
			{
				Log.s_LogSource = value;
				Log.RefreshEventLogSettings();
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000006 RID: 6 RVA: 0x00002082 File Offset: 0x00000282
		// (set) Token: 0x06000005 RID: 5 RVA: 0x00002074 File Offset: 0x00000274
		public static string Name
		{
			get
			{
				return Log.s_LogName;
			}
			set
			{
				Log.s_LogName = value;
				Log.RefreshEventLogSettings();
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000007 RID: 7 RVA: 0x00002089 File Offset: 0x00000289
		// (set) Token: 0x06000008 RID: 8 RVA: 0x00002090 File Offset: 0x00000290
		public static bool EventLog
		{
			get
			{
				return Log.s_fEventLog;
			}
			set
			{
				Log.s_fEventLog = value;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000009 RID: 9 RVA: 0x00002098 File Offset: 0x00000298
		// (set) Token: 0x0600000A RID: 10 RVA: 0x0000209F File Offset: 0x0000029F
		public static bool Prefix
		{
			get
			{
				return Log.s_fPrefix;
			}
			set
			{
				Log.s_fPrefix = value;
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000020A7 File Offset: 0x000002A7
		private static EventLog RefreshEventLogSettings()
		{
			if (Log.s_eventLog != null)
			{
				Log.s_eventLog.Close();
			}
			Log.s_eventLog = new EventLog(Log.s_LogName, ".", Log.s_LogSource);
			return Log.s_eventLog;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000020D8 File Offset: 0x000002D8
		public static bool SetVerbosityLevel(string strVerbosity)
		{
			Regex regex = new Regex("^n(one)?$", RegexOptions.IgnoreCase);
			Regex regex2 = new Regex("^f(atal)?$", RegexOptions.IgnoreCase);
			Regex regex3 = new Regex("^e(rr(or)?)?$", RegexOptions.IgnoreCase);
			Regex regex4 = new Regex("^w(arn(ing)?)?$", RegexOptions.IgnoreCase);
			Regex regex5 = new Regex("^info(rmation)?$", RegexOptions.IgnoreCase);
			Regex regex6 = new Regex("^t(race)?$", RegexOptions.IgnoreCase);
			Regex regex7 = new Regex("^v(erbose)?$", RegexOptions.IgnoreCase);
			bool result;
			if (regex.IsMatch(strVerbosity.Trim()))
			{
				Log.s_level = Log.Level.NONE;
				result = true;
			}
			else if (regex2.IsMatch(strVerbosity.Trim()))
			{
				Log.s_level = Log.Level.FATAL;
				result = true;
			}
			else if (regex3.IsMatch(strVerbosity.Trim()))
			{
				Log.s_level = Log.Level.ERROR;
				result = true;
			}
			else if (regex4.IsMatch(strVerbosity.Trim()))
			{
				Log.s_level = Log.Level.WARNING;
				result = true;
			}
			else if (regex5.IsMatch(strVerbosity.Trim()))
			{
				Log.s_level = Log.Level.INFO;
				result = true;
			}
			else if (regex6.IsMatch(strVerbosity.Trim()))
			{
				Log.s_level = Log.Level.TRACE;
				result = true;
			}
			else if (regex7.IsMatch(strVerbosity.Trim()))
			{
				Log.s_level = Log.Level.VERBOSE;
				result = true;
			}
			else
			{
				Log.s_level = Log.Level.INFO;
				result = false;
			}
			return result;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002204 File Offset: 0x00000404
		private static void logfn(EventLogEntryType type, string strPrefix, string strMsg)
		{
			if (Log.s_fEventLog)
			{
				try
				{
					Log.s_eventLog.WriteEntry(strMsg, type);
				}
				catch (Exception arg)
				{
					Console.WriteLine("Log.logfn: {0}", arg);
				}
			}
			if (Log.s_fPrefix)
			{
				strMsg = string.Format("{0} - {1}: {2}", Log.s_LogSource, strPrefix, strMsg);
			}
			else
			{
				strMsg = string.Format("{0}", strMsg);
			}
			Console.WriteLine(strMsg);
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002274 File Offset: 0x00000474
		private static void LogInternal(Log.Level level, EventLogEntryType type, string strPrefix, params object[] args)
		{
			if (level < Log.s_level)
			{
				return;
			}
			if (args.Length == 0)
			{
				return;
			}
			if (1 == args.Length)
			{
				Log.logfn(type, strPrefix, string.Format(args[0].ToString(), new object[0]));
				return;
			}
			object[] array = new object[args.Length - 1];
			Array.Copy(args, 1, array, 0, args.Length - 1);
			Log.logfn(type, strPrefix, string.Format(args[0].ToString(), array));
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000022DE File Offset: 0x000004DE
		public static void Fatal(params object[] args)
		{
			Log.LogInternal(Log.Level.FATAL, EventLogEntryType.Error, "FATAL ERROR", args);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000022F1 File Offset: 0x000004F1
		public static void Error(params object[] args)
		{
			Log.LogInternal(Log.Level.ERROR, EventLogEntryType.Error, "ERROR", args);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002301 File Offset: 0x00000501
		public static void Warning(params object[] args)
		{
			Log.LogInternal(Log.Level.WARNING, EventLogEntryType.Warning, "WARNING", args);
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002311 File Offset: 0x00000511
		public static void Info(params object[] args)
		{
			Log.LogInternal(Log.Level.INFO, EventLogEntryType.Information, "INFO", args);
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002321 File Offset: 0x00000521
		public static void Trace(params object[] args)
		{
			Log.LogInternal(Log.Level.TRACE, EventLogEntryType.Information, "TRACE", args);
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002330 File Offset: 0x00000530
		public static void Verbose(params object[] args)
		{
			Log.LogInternal(Log.Level.VERBOSE, EventLogEntryType.Information, "VERBOSE", args);
		}

		// Token: 0x04000001 RID: 1
		private static Log.Level s_level = Log.Level.INFO;

		// Token: 0x04000002 RID: 2
		private static string s_LogSource = "LOGUTILS";

		// Token: 0x04000003 RID: 3
		private static string s_LogName = "Test Log";

		// Token: 0x04000004 RID: 4
		private static EventLog s_eventLog = Log.RefreshEventLogSettings();

		// Token: 0x04000005 RID: 5
		private static bool s_fEventLog = false;

		// Token: 0x04000006 RID: 6
		private static bool s_fPrefix = false;

		// Token: 0x02000003 RID: 3
		public enum Level : ushort
		{
			// Token: 0x04000008 RID: 8
			NONE = 255,
			// Token: 0x04000009 RID: 9
			FATAL = 128,
			// Token: 0x0400000A RID: 10
			ERROR = 64,
			// Token: 0x0400000B RID: 11
			WARNING = 32,
			// Token: 0x0400000C RID: 12
			INFO = 16,
			// Token: 0x0400000D RID: 13
			TRACE = 8,
			// Token: 0x0400000E RID: 14
			VERBOSE = 4
		}
	}
}
