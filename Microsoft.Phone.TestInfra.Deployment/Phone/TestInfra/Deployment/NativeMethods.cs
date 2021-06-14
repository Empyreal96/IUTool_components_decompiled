using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000022 RID: 34
	internal static class NativeMethods
	{
		// Token: 0x06000182 RID: 386
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool CreateSymbolicLink(string linkFileName, string targetFileName, NativeMethods.SymbolicLinkFlag flags);

		// Token: 0x06000183 RID: 387
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetFileTime(SafeFileHandle hFile, ref long lpCreationTime, ref long lpLastAccessTime, ref long lpLastWriteTime);

		// Token: 0x06000184 RID: 388
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern SafeFileHandle CreateFile([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, [MarshalAs(UnmanagedType.U4)] uint dwDesiredAccess, [MarshalAs(UnmanagedType.U4)] uint dwShareMode, IntPtr lpSecurityAttributes, [MarshalAs(UnmanagedType.U4)] uint dwCreationDisposition, [MarshalAs(UnmanagedType.U4)] uint dwFlagsAndAttributes, IntPtr hTemplateFile);

		// Token: 0x04000098 RID: 152
		public const uint FileWriteAttributes = 256U;

		// Token: 0x04000099 RID: 153
		public const uint ShareModeRead = 1U;

		// Token: 0x0400009A RID: 154
		public const uint ShareModeWrite = 2U;

		// Token: 0x0400009B RID: 155
		public const uint OpenExisting = 3U;

		// Token: 0x0400009C RID: 156
		public const uint FileFlagOpenReparsePoint = 2097152U;

		// Token: 0x0200005A RID: 90
		public enum SymbolicLinkFlag
		{
			// Token: 0x04000160 RID: 352
			File,
			// Token: 0x04000161 RID: 353
			Directory
		}
	}
}
