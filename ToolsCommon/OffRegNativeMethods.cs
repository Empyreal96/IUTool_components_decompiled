using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000018 RID: 24
	internal static class OffRegNativeMethods
	{
		// Token: 0x060000E1 RID: 225
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int ORCreateHive(ref IntPtr handle);

		// Token: 0x060000E2 RID: 226
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int OROpenHive([MarshalAs(UnmanagedType.LPWStr)] string Path, ref IntPtr handle);

		// Token: 0x060000E3 RID: 227
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int ORCloseHive(IntPtr handle);

		// Token: 0x060000E4 RID: 228
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int ORSaveHive(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string path, int osMajor, int osMinor);

		// Token: 0x060000E5 RID: 229
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int OROpenKey(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string subKeyName, ref IntPtr subkeyHandle);

		// Token: 0x060000E6 RID: 230
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int ORCreateKey(IntPtr handle, string subKeyName, string className, uint dwOptions, byte[] secbuf, ref IntPtr keyHandle, ref uint dwDisposition);

		// Token: 0x060000E7 RID: 231
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int ORCloseKey(IntPtr handle);

		// Token: 0x060000E8 RID: 232
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int OREnumKey(IntPtr handle, uint dwIndex, StringBuilder name, ref uint count, StringBuilder classname, ref uint classnamecount, ref IntPtr filetimeptr);

		// Token: 0x060000E9 RID: 233
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		public static extern int ORQueryInfoKey(IntPtr handle, StringBuilder classname, ref uint lpcClass, out uint lpcSubKeys, out uint lpcMaxSubKeyLen, out uint lpcMaxClassLen, out uint lpcValues, out uint lpcMaxValueNameLen, out uint lpcMaxValueLen, out uint lpcbSecurityDescriptor, IntPtr filetimeptr);

		// Token: 0x060000EA RID: 234
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int ORGetValue(IntPtr Handle, string lpSubKey, string lpValue, out uint pdwType, byte[] pvData, ref uint pcbData);

		// Token: 0x060000EB RID: 235
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int ORSetValue(IntPtr Handle, string lpValueName, uint dwType, byte[] pvData, uint cbData);

		// Token: 0x060000EC RID: 236
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int ORDeleteValue(IntPtr Handle, string lpValueName);

		// Token: 0x060000ED RID: 237
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int ORDeleteKey(IntPtr Handle, string lpKeyName);

		// Token: 0x060000EE RID: 238
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int ORGetVirtualFlags(IntPtr Handle, ref int pbFlags);

		// Token: 0x060000EF RID: 239
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int OREnumValue(IntPtr Handle, uint index, StringBuilder lpValueName, ref uint lpcValueName, out uint lpType, IntPtr pvData, IntPtr lpcbData);

		// Token: 0x060000F0 RID: 240
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int ORGetKeySecurity(IntPtr handle, SecurityInformationFlags secinfo, byte[] lpSecBuf, ref uint size);

		// Token: 0x060000F1 RID: 241
		[DllImport("Offreg.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int ORSetKeySecurity(IntPtr handle, SecurityInformationFlags secinfo, byte[] lpSecBuf);
	}
}
