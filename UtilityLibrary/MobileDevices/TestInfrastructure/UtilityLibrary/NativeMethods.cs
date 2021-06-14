using System;
using System.Runtime.InteropServices;

namespace Microsoft.MobileDevices.TestInfrastructure.UtilityLibrary
{
	// Token: 0x0200001A RID: 26
	internal static class NativeMethods
	{
		// Token: 0x06000083 RID: 131
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "StartTraceW", ExactSpelling = true)]
		internal static extern int StartTrace(out ulong traceHandle, [MarshalAs(UnmanagedType.LPWStr)] [In] string sessionName, [In] [Out] ref EventTraceProperties eventTraceProperties);

		// Token: 0x06000084 RID: 132 RVA: 0x00004854 File Offset: 0x00002A54
		internal static int EnableTraceEx2(ulong traceHandle, Guid providerId, uint controlCode, TraceLevel traceLevel = TraceLevel.Verbose, ulong matchAnyKeyword = 18446744073709551615UL, ulong matchAllKeyword = 0UL, uint timeout = 0U)
		{
			return NativeMethods.EnableTraceEx2(traceHandle, ref providerId, controlCode, traceLevel, matchAnyKeyword, matchAllKeyword, timeout, IntPtr.Zero);
		}

		// Token: 0x06000085 RID: 133
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "ControlTraceW", ExactSpelling = true, SetLastError = true)]
		internal static extern int ControlTrace(ulong SessionHandle, string SessionName, ref EventTraceProperties Properties, uint ControlCode);

		// Token: 0x06000086 RID: 134
		[DllImport("advapi32.dll")]
		private static extern int EnableTraceEx2([In] ulong traceHandle, [In] ref Guid providerId, [In] uint controlCode, [In] TraceLevel traceLevel, [In] ulong matchAnyKeyword, [In] ulong matchAllKeyword, [In] uint timeout, [In] IntPtr enableTraceParameters);

		// Token: 0x06000087 RID: 135
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		internal static extern IntPtr CreateJobObject([In] NativeMethods.SECURITY_ATTRIBUTES lpJobAttributes, string lpName);

		// Token: 0x06000088 RID: 136
		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool AssignProcessToJobObject(IntPtr hJob, IntPtr hProcess);

		// Token: 0x06000089 RID: 137
		[DllImport("kernel32.dll")]
		internal static extern bool SetInformationJobObject(IntPtr hJob, NativeMethods.JOBOBJECTINFOCLASS JobObjectInfoClass, IntPtr lpJobObjectInfo, uint cbJobObjectInfoLength);

		// Token: 0x0600008A RID: 138
		[DllImport("Kernel32")]
		public static extern bool CloseHandle(IntPtr handle);

		// Token: 0x0600008B RID: 139
		[DllImport("Kernel32")]
		public static extern bool IsProcessInJob(IntPtr hProcess, IntPtr hJob, out bool result);

		// Token: 0x04000063 RID: 99
		internal const ulong InvalidHandle = 18446744073709551615UL;

		// Token: 0x04000064 RID: 100
		internal const uint EventTraceControlQuery = 0U;

		// Token: 0x04000065 RID: 101
		internal const uint EventTraceControlStop = 1U;

		// Token: 0x04000066 RID: 102
		internal const uint EventControlCodeEnableProvider = 1U;

		// Token: 0x0200001B RID: 27
		[StructLayout(LayoutKind.Sequential)]
		public class SECURITY_ATTRIBUTES
		{
			// Token: 0x04000067 RID: 103
			public int nLength;

			// Token: 0x04000068 RID: 104
			public IntPtr lpSecurityDescriptor;

			// Token: 0x04000069 RID: 105
			public int bInheritHandle;
		}

		// Token: 0x0200001C RID: 28
		internal struct JOBOBJECT_BASIC_LIMIT_INFORMATION
		{
			// Token: 0x0400006A RID: 106
			public long PerProcessUserTimeLimit;

			// Token: 0x0400006B RID: 107
			public long PerJobUserTimeLimit;

			// Token: 0x0400006C RID: 108
			public NativeMethods.JOB_OBJECT_LIMIT LimitFlags;

			// Token: 0x0400006D RID: 109
			public UIntPtr MinimumWorkingSetSize;

			// Token: 0x0400006E RID: 110
			public UIntPtr MaximumWorkingSetSize;

			// Token: 0x0400006F RID: 111
			public uint ActiveProcessLimit;

			// Token: 0x04000070 RID: 112
			public long Affinity;

			// Token: 0x04000071 RID: 113
			public uint PriorityClass;

			// Token: 0x04000072 RID: 114
			public uint SchedulingClass;
		}

		// Token: 0x0200001D RID: 29
		internal struct IO_COUNTERS
		{
			// Token: 0x04000073 RID: 115
			public ulong ReadOperationCount;

			// Token: 0x04000074 RID: 116
			public ulong WriteOperationCount;

			// Token: 0x04000075 RID: 117
			public ulong OtherOperationCount;

			// Token: 0x04000076 RID: 118
			public ulong ReadTransferCount;

			// Token: 0x04000077 RID: 119
			public ulong WriteTransferCount;

			// Token: 0x04000078 RID: 120
			public ulong OtherTransferCount;
		}

		// Token: 0x0200001E RID: 30
		internal struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
		{
			// Token: 0x04000079 RID: 121
			public NativeMethods.JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;

			// Token: 0x0400007A RID: 122
			public NativeMethods.IO_COUNTERS IoInfo;

			// Token: 0x0400007B RID: 123
			public UIntPtr ProcessMemoryLimit;

			// Token: 0x0400007C RID: 124
			public UIntPtr JobMemoryLimit;

			// Token: 0x0400007D RID: 125
			public UIntPtr PeakProcessMemoryUsed;

			// Token: 0x0400007E RID: 126
			public UIntPtr PeakJobMemoryUsed;
		}

		// Token: 0x0200001F RID: 31
		internal enum JOBOBJECTINFOCLASS
		{
			// Token: 0x04000080 RID: 128
			AssociateCompletionPortInformation = 7,
			// Token: 0x04000081 RID: 129
			BasicLimitInformation = 2,
			// Token: 0x04000082 RID: 130
			BasicUIRestrictions = 4,
			// Token: 0x04000083 RID: 131
			EndOfJobTimeInformation = 6,
			// Token: 0x04000084 RID: 132
			ExtendedLimitInformation = 9,
			// Token: 0x04000085 RID: 133
			SecurityLimitInformation = 5,
			// Token: 0x04000086 RID: 134
			GroupInformation = 11
		}

		// Token: 0x02000020 RID: 32
		[Flags]
		internal enum JOB_OBJECT_LIMIT : uint
		{
			// Token: 0x04000088 RID: 136
			JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 8192U
		}
	}
}
