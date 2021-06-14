using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000019 RID: 25
	internal static class OffRegNativeMethods
	{
		// Token: 0x06000105 RID: 261
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int ORCreateHive(ref IntPtr handle);

		// Token: 0x06000106 RID: 262
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int OROpenHive([MarshalAs(UnmanagedType.LPWStr)] string Path, ref IntPtr handle);

		// Token: 0x06000107 RID: 263
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int ORCloseHive(IntPtr handle);

		// Token: 0x06000108 RID: 264
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int ORSaveHive(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string path, int osMajor, int osMinor);

		// Token: 0x06000109 RID: 265
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int OROpenKey(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string subKeyName, ref IntPtr subkeyHandle);

		// Token: 0x0600010A RID: 266
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int ORCreateKey(IntPtr handle, string subKeyName, string className, uint dwOptions, byte[] secbuf, ref IntPtr keyHandle, ref uint dwDisposition);

		// Token: 0x0600010B RID: 267
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int ORCloseKey(IntPtr handle);

		// Token: 0x0600010C RID: 268
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int OREnumKey(IntPtr handle, uint dwIndex, StringBuilder name, ref uint count, StringBuilder classname, ref uint classnamecount, ref IntPtr filetimeptr);

		// Token: 0x0600010D RID: 269
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		public static extern int ORQueryInfoKey(IntPtr handle, StringBuilder classname, ref uint lpcClass, out uint lpcSubKeys, out uint lpcMaxSubKeyLen, out uint lpcMaxClassLen, out uint lpcValues, out uint lpcMaxValueNameLen, out uint lpcMaxValueLen, out uint lpcbSecurityDescriptor, IntPtr filetimeptr);

		// Token: 0x0600010E RID: 270
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int ORGetValue(IntPtr Handle, string lpSubKey, string lpValue, out uint pdwType, byte[] pvData, ref uint pcbData);

		// Token: 0x0600010F RID: 271
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int ORSetValue(IntPtr Handle, string lpValueName, uint dwType, byte[] pvData, uint cbData);

		// Token: 0x06000110 RID: 272
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int ORDeleteValue(IntPtr Handle, string lpValueName);

		// Token: 0x06000111 RID: 273
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int ORDeleteKey(IntPtr Handle, string lpKeyName);

		// Token: 0x06000112 RID: 274
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int ORGetVirtualFlags(IntPtr Handle, ref int pbFlags);

		// Token: 0x06000113 RID: 275
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int OREnumValue(IntPtr Handle, uint index, StringBuilder lpValueName, ref uint lpcValueName, out uint lpType, IntPtr pvData, IntPtr lpcbData);

		// Token: 0x06000114 RID: 276
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int ORGetKeySecurity(IntPtr handle, SecurityInformationFlags secinfo, byte[] lpSecBuf, ref uint size);

		// Token: 0x06000115 RID: 277
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int ORSetKeySecurity(IntPtr handle, SecurityInformationFlags secinfo, byte[] lpSecBuf);
	}
}
