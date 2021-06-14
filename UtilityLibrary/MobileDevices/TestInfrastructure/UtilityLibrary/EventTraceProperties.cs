using System;
using System.Runtime.InteropServices;

namespace Microsoft.MobileDevices.TestInfrastructure.UtilityLibrary
{
	// Token: 0x02000011 RID: 17
	[Serializable]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct EventTraceProperties
	{
		// Token: 0x06000062 RID: 98 RVA: 0x00003EA8 File Offset: 0x000020A8
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
			if (logFilePath != null && logFilePath.Length < 1024)
			{
				result.logFileName = logFilePath;
			}
			result.CoreProperties.LogFileNameOffset = (uint)((int)Marshal.OffsetOf(typeof(EventTraceProperties), "logFileName"));
			if (sessionName != null && sessionName.Length < 1024)
			{
				result.loggerName = sessionName;
			}
			result.CoreProperties.LoggerNameOffset = (uint)((int)Marshal.OffsetOf(typeof(EventTraceProperties), "loggerName"));
			return result;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00004000 File Offset: 0x00002200
		internal static EventTraceProperties CreateProperties()
		{
			return EventTraceProperties.CreateProperties(null, null, (LoggingModeConstant)0U);
		}

		// Token: 0x04000041 RID: 65
		public const int MaxLoggerNameLength = 1024;

		// Token: 0x04000042 RID: 66
		public EventTraceProperties.EventTracePropertiesCore CoreProperties;

		// Token: 0x04000043 RID: 67
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
		private string loggerName;

		// Token: 0x04000044 RID: 68
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
		private string logFileName;

		// Token: 0x02000012 RID: 18
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct WNodeHeader
		{
			// Token: 0x04000045 RID: 69
			public uint BufferSize;

			// Token: 0x04000046 RID: 70
			public uint ProviderId;

			// Token: 0x04000047 RID: 71
			public ulong HistoricalContext;

			// Token: 0x04000048 RID: 72
			public long TimeStamp;

			// Token: 0x04000049 RID: 73
			public Guid Guid;

			// Token: 0x0400004A RID: 74
			public uint ClientContext;

			// Token: 0x0400004B RID: 75
			public uint Flags;
		}

		// Token: 0x02000013 RID: 19
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct EventTracePropertiesCore
		{
			// Token: 0x0400004C RID: 76
			public EventTraceProperties.WNodeHeader Wnode;

			// Token: 0x0400004D RID: 77
			public uint BufferSize;

			// Token: 0x0400004E RID: 78
			public uint MinimumBuffers;

			// Token: 0x0400004F RID: 79
			public uint MaximumBuffers;

			// Token: 0x04000050 RID: 80
			public uint MaximumFileSize;

			// Token: 0x04000051 RID: 81
			public LoggingModeConstant LogFileMode;

			// Token: 0x04000052 RID: 82
			public uint FlushTimer;

			// Token: 0x04000053 RID: 83
			public uint EnableFlags;

			// Token: 0x04000054 RID: 84
			public int AgeLimit;

			// Token: 0x04000055 RID: 85
			public uint NumberOfBuffers;

			// Token: 0x04000056 RID: 86
			public uint FreeBuffers;

			// Token: 0x04000057 RID: 87
			public uint EventsLost;

			// Token: 0x04000058 RID: 88
			public uint BuffersWritten;

			// Token: 0x04000059 RID: 89
			public uint LogBuffersLost;

			// Token: 0x0400005A RID: 90
			public uint RealTimeBuffersLost;

			// Token: 0x0400005B RID: 91
			public IntPtr LoggerThreadId;

			// Token: 0x0400005C RID: 92
			public uint LogFileNameOffset;

			// Token: 0x0400005D RID: 93
			public uint LoggerNameOffset;
		}
	}
}
