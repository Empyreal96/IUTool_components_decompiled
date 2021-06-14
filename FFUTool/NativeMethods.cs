using System;
using System.Runtime.InteropServices;

namespace Microsoft.Windows.ImageTools
{
	// Token: 0x02000009 RID: 9
	internal static class NativeMethods
	{
		// Token: 0x06000011 RID: 17
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "StartTraceW", ExactSpelling = true)]
		internal static extern int StartTrace(out ulong traceHandle, [MarshalAs(UnmanagedType.LPWStr)] [In] string sessionName, [In] [Out] ref EventTraceProperties eventTraceProperties);

		// Token: 0x06000012 RID: 18 RVA: 0x0000285C File Offset: 0x00000A5C
		internal static int EnableTraceEx2(ulong traceHandle, Guid providerId, uint controlCode, TraceLevel traceLevel = TraceLevel.Verbose, ulong matchAnyKeyword = 18446744073709551615UL, ulong matchAllKeyword = 0UL, uint timeout = 0U)
		{
			return NativeMethods.EnableTraceEx2(traceHandle, ref providerId, controlCode, traceLevel, matchAnyKeyword, matchAllKeyword, timeout, IntPtr.Zero);
		}

		// Token: 0x06000013 RID: 19
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "StopTraceW", ExactSpelling = true)]
		internal static extern int StopTrace([In] ulong traceHandle, [MarshalAs(UnmanagedType.LPWStr)] [In] string sessionName, out EventTraceProperties eventTraceProperties);

		// Token: 0x06000014 RID: 20
		[DllImport("advapi32.dll")]
		private static extern int EnableTraceEx2([In] ulong traceHandle, [In] ref Guid providerId, [In] uint controlCode, [In] TraceLevel traceLevel, [In] ulong matchAnyKeyword, [In] ulong matchAllKeyword, [In] uint timeout, [In] IntPtr enableTraceParameters);
	}
}
