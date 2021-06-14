using System;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000016 RID: 22
	[CLSCompliant(false)]
	[Guid("54CD06C1-268F-40BB-8ED2-757A9EBAEC8D")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAppxBundleManifestPackageInfo
	{
		// Token: 0x06000067 RID: 103
		APPX_BUNDLE_PAYLOAD_PACKAGE_TYPE GetPackageType();

		// Token: 0x06000068 RID: 104
		IAppxManifestPackageId GetPackageId();

		// Token: 0x06000069 RID: 105
		[return: MarshalAs(UnmanagedType.LPWStr)]
		string GetFileName();

		// Token: 0x0600006A RID: 106
		[return: MarshalAs(UnmanagedType.LPWStr)]
		ulong GetOffset();

		// Token: 0x0600006B RID: 107
		ulong GetSize();

		// Token: 0x0600006C RID: 108
		IAppxManifestQualifiedResourcesEnumerator GetResources();
	}
}
