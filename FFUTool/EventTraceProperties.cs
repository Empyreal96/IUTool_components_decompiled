using System;
using System.Runtime.InteropServices;

namespace Microsoft.Windows.ImageTools
{
	// Token: 0x02000008 RID: 8
	[Serializable]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct EventTraceProperties
	{
		// Token: 0x0600000F RID: 15 RVA: 0x0000271C File Offset: 0x0000091C
		internal static EventTraceProperties CreateProperties(string sessionName, string logFilePath, LoggingModeConstant logMode)
		{
			uint bufferSize = (uint)Marshal.SizeOf(typeof(EventTraceProperties));
			EventTraceProperties result = default(EventTraceProperties);
			result.CoreProperties.Wnode = default(EventTraceProperties.WNodeHeader);
			result.CoreProperties.Wnode.BufferSize = bufferSize;
			result.CoreProperties.Wnode.Flags = 131072U;
			result.CoreProperties.Wnode.Guid = Guid.NewGuid();
			result.CoreProperties.BufferSize = 64U;
			result.CoreProperties.MinimumBuffers = 5U;
			result.CoreProperties.MaximumBuffers = 200U;
			result.CoreProperties.FlushTimer = 0U;
			result.CoreProperties.LogFileMode = logMode;
			if (logFilePath != null && logFilePath.Length < 260)
			{
				result.logFileName = logFilePath;
			}
			result.CoreProperties.LogFileNameOffset = (uint)((int)Marshal.OffsetOf(typeof(EventTraceProperties), "logFileName"));
			if (sessionName != null && sessionName.Length < 260)
			{
				result.loggerName = sessionName;
			}
			result.CoreProperties.LoggerNameOffset = (uint)((int)Marshal.OffsetOf(typeof(EventTraceProperties), "loggerName"));
			return result;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002852 File Offset: 0x00000A52
		internal static EventTraceProperties CreateProperties()
		{
			return EventTraceProperties.CreateProperties(null, null, (LoggingModeConstant)0U);
		}

		// Token: 0x04000027 RID: 39
		public const int MaxLoggerNameLength = 260;

		// Token: 0x04000028 RID: 40
		[NonSerialized]
		public EventTraceProperties.EventTracePropertiesCore CoreProperties;

		// Token: 0x04000029 RID: 41
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		private string loggerName;

		// Token: 0x0400002A RID: 42
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		private string logFileName;

		// Token: 0x02000010 RID: 16
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct WNodeHeader
		{
			// Token: 0x0400004C RID: 76
			public uint BufferSize;

			// Token: 0x0400004D RID: 77
			public uint ProviderId;

			// Token: 0x0400004E RID: 78
			public ulong HistoricalContext;

			// Token: 0x0400004F RID: 79
			public long TimeStamp;

			// Token: 0x04000050 RID: 80
			public Guid Guid;

			// Token: 0x04000051 RID: 81
			public uint ClientContext;

			// Token: 0x04000052 RID: 82
			public uint Flags;
		}

		// Token: 0x02000011 RID: 17
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct EventTracePropertiesCore
		{
			// Token: 0x04000053 RID: 83
			public EventTraceProperties.WNodeHeader Wnode;

			// Token: 0x04000054 RID: 84
			public uint BufferSize;

			// Token: 0x04000055 RID: 85
			public uint MinimumBuffers;

			// Token: 0x04000056 RID: 86
			public uint MaximumBuffers;

			// Token: 0x04000057 RID: 87
			public uint MaximumFileSize;

			// Token: 0x04000058 RID: 88
			public LoggingModeConstant LogFileMode;

			// Token: 0x04000059 RID: 89
			public uint FlushTimer;

			// Token: 0x0400005A RID: 90
			public uint EnableFlags;

			// Token: 0x0400005B RID: 91
			public int AgeLimit;

			// Token: 0x0400005C RID: 92
			public uint NumberOfBuffers;

			// Token: 0x0400005D RID: 93
			public uint FreeBuffers;

			// Token: 0x0400005E RID: 94
			public uint EventsLost;

			// Token: 0x0400005F RID: 95
			public uint BuffersWritten;

			// Token: 0x04000060 RID: 96
			public uint LogBuffersLost;

			// Token: 0x04000061 RID: 97
			public uint RealTimeBuffersLost;

			// Token: 0x04000062 RID: 98
			public IntPtr LoggerThreadId;

			// Token: 0x04000063 RID: 99
			public uint LogFileNameOffset;

			// Token: 0x04000064 RID: 100
			public uint LoggerNameOffset;
		}
	}
}
