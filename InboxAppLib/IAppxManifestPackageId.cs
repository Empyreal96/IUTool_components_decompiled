using System;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x0200001B RID: 27
	[CLSCompliant(false)]
	[Guid("283ce2d7-7153-4a91-9649-7a0f7240945f")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAppxManifestPackageId
	{
		// Token: 0x06000088 RID: 136
		[return: MarshalAs(UnmanagedType.LPWStr)]
		string GetName();

		// Token: 0x06000089 RID: 137
		APPX_PACKAGE_ARCHITECTURE GetArchitecture();

		// Token: 0x0600008A RID: 138
		[return: MarshalAs(UnmanagedType.LPWStr)]
		string GetPublisher();

		// Token: 0x0600008B RID: 139
		ulong GetVersion();

		// Token: 0x0600008C RID: 140
		[return: MarshalAs(UnmanagedType.LPWStr)]
		string GetResourceId();

		// Token: 0x0600008D RID: 141
		bool ComparePublisher([MarshalAs(UnmanagedType.LPWStr)] [In] string otherPublisher);

		// Token: 0x0600008E RID: 142
		[return: MarshalAs(UnmanagedType.LPWStr)]
		string GetPackageFullName();

		// Token: 0x0600008F RID: 143
		[return: MarshalAs(UnmanagedType.LPWStr)]
		string GetPackageFamilyName();
	}
}
