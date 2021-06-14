using System;
using System.Runtime.InteropServices;

namespace Microsoft.Composition.Packaging
{
	// Token: 0x02000003 RID: 3
	internal class NativeMethods
	{
		// Token: 0x06000050 RID: 80
		[DllImport("ParseManifestLite.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U1)]
		public static extern bool ParseManaged(IntPtr parsemanifestSession, [MarshalAs(UnmanagedType.LPWStr)] string manifestPath);

		// Token: 0x06000051 RID: 81
		[DllImport("ParseManifestLite.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U1)]
		public static extern bool InitWcpManaged();

		// Token: 0x06000052 RID: 82
		[DllImport("ParseManifestLite.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int GetFileCountManaged(IntPtr parsemanifestSession);

		// Token: 0x06000053 RID: 83
		[DllImport("ParseManifestLite.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int ReplaceMacrosManaged(IntPtr parsemanifestSession, [MarshalAs(UnmanagedType.LPWStr)] string valueWithVariables, out IntPtr outputString);

		// Token: 0x06000054 RID: 84
		[DllImport("ParseManifestLite.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int ReplaceFilePathWithMacroPath([MarshalAs(UnmanagedType.LPWStr)] string filePath, out IntPtr outputString);

		// Token: 0x06000055 RID: 85
		[DllImport("ParseManifestLite.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.U1)]
		public static extern bool ParsePackageManaged([MarshalAs(UnmanagedType.LPWStr)] string updateMumPath, out IntPtr parsemanifestSession);

		// Token: 0x06000056 RID: 86
		[DllImport("ParseManifestLite.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int GetSourceNameManaged(IntPtr parsemanifestSession, int index, out IntPtr outputString);

		// Token: 0x06000057 RID: 87
		[DllImport("ParseManifestLite.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int GetSourcePathManaged(IntPtr parsemanifestSession, int index, out IntPtr outputString);

		// Token: 0x06000058 RID: 88
		[DllImport("ParseManifestLite.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int GetDestinationNameManaged(IntPtr parsemanifestSession, int index, out IntPtr outputString);

		// Token: 0x06000059 RID: 89
		[DllImport("ParseManifestLite.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int GetDestinationPathManaged(IntPtr parsemanifestSession, int index, out IntPtr outputString);

		// Token: 0x0600005A RID: 90
		[DllImport("ParseManifestLite.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern void ManagedMemoryFree(IntPtr outputString);

		// Token: 0x0600005B RID: 91
		[DllImport("ParseManifestLite.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern void ManagedFreeSession(IntPtr parsemanifestSession);

		// Token: 0x04000006 RID: 6
		private const string PARSE_DLL = "ParseManifestLite.dll";

		// Token: 0x04000007 RID: 7
		private const CallingConvention CALLING_CONVENTION = CallingConvention.Cdecl;

		// Token: 0x04000008 RID: 8
		private const CharSet CHAR_SET = CharSet.Unicode;
	}
}
