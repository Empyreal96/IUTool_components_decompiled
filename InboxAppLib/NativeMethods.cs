using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000032 RID: 50
	public static class NativeMethods
	{
		// Token: 0x060000B9 RID: 185
		[DllImport("Kernel32.dll")]
		internal static extern int PackageFullNameFromId(ref NativeMethods.PACKAGE_ID packageId, ref uint packageFullNameLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder packageFullName);

		// Token: 0x060000BA RID: 186
		[DllImport("AppxPackaging.dll", EntryPoint = "#4")]
		internal static extern int Unbundle([MarshalAs(UnmanagedType.LPWStr)] string inputBundlePath, [MarshalAs(UnmanagedType.LPWStr)] string outputDirectoryPath, [MarshalAs(UnmanagedType.Bool)] bool createMonikerSubdirectory, IntPtr messageHandler, [MarshalAs(UnmanagedType.LPWStr)] ref string destinationDirectory);

		// Token: 0x060000BB RID: 187
		[DllImport("AppxPackaging.dll", EntryPoint = "#3")]
		internal static extern int Unpack([MarshalAs(UnmanagedType.LPWStr)] string inputPackagePath, [MarshalAs(UnmanagedType.LPWStr)] string outputDirectoryPath, [MarshalAs(UnmanagedType.Bool)] bool createMonikerSubdirectory, IntPtr messageHandler, [MarshalAs(UnmanagedType.LPWStr)] ref string destinationDirectory);

		// Token: 0x060000BC RID: 188
		[DllImport("BCP47Langs.dll")]
		internal static extern int Bcp47GetNlsForm([MarshalAs(UnmanagedType.HString)] string languageTag, [MarshalAs(UnmanagedType.HString)] ref string nlsForm);

		// Token: 0x0400003D RID: 61
		private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

		// Token: 0x02000051 RID: 81
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct PACKAGE_ID
		{
			// Token: 0x040000E8 RID: 232
			public uint reserved;

			// Token: 0x040000E9 RID: 233
			public uint processorArchitecture;

			// Token: 0x040000EA RID: 234
			public short Revision;

			// Token: 0x040000EB RID: 235
			public short Build;

			// Token: 0x040000EC RID: 236
			public short Minor;

			// Token: 0x040000ED RID: 237
			public short Major;

			// Token: 0x040000EE RID: 238
			public string name;

			// Token: 0x040000EF RID: 239
			public string publisher;

			// Token: 0x040000F0 RID: 240
			public string resourceId;

			// Token: 0x040000F1 RID: 241
			public string publisherId;
		}
	}
}
