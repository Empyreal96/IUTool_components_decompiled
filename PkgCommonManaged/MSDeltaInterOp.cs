using System;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x0200000A RID: 10
	internal static class MSDeltaInterOp
	{
		// Token: 0x0600003C RID: 60
		[DllImport("msdelta", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CreateDeltaW([MarshalAs(UnmanagedType.I8)] DELTA_FILE_TYPE FileTypeSet, [MarshalAs(UnmanagedType.I8)] DELTA_FLAG_TYPE SetFlags, [MarshalAs(UnmanagedType.I8)] DELTA_FLAG_TYPE ResetFlags, string sourcePath, string targetPath, string sourceOption, string targetOption, DELTA_INPUT GlobalOptions, ref ulong lpTargetFileTime, uint HashAlgId, string DeltaName);
	}
}
