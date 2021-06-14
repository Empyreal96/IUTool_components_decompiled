using System;
using System.Runtime.InteropServices;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x0200002E RID: 46
	public class NativeSecurityMethods
	{
		// Token: 0x06000192 RID: 402
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ConvertSecurityDescriptorToStringSecurityDescriptor([In] byte[] pBinarySecurityDescriptor, int RequestedStringSDRevision, SecurityInformationFlags SecurityInformation, out IntPtr StringSecurityDescriptor, out int StringSecurityDescriptorLen);

		// Token: 0x06000193 RID: 403
		[DllImport("AdvAPI32.DLL", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool GetFileSecurity(string lpFileName, SecurityInformationFlags RequestedInformation, IntPtr pSecurityDescriptor, int nLength, ref int lpnLengthNeeded);

		// Token: 0x06000194 RID: 404
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int IU_AdjustProcessPrivilege(string strPrivilegeName, bool fEnabled);
	}
}
