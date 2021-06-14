using System;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000014 RID: 20
	[CLSCompliant(false)]
	[Guid("DD75B8C0-BA76-43B0-AE0F-68656A1DC5C8")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAppxBundleReader
	{
		// Token: 0x0600005F RID: 95
		IAppxFile GetFootprintFile([In] APPX_BUNDLE_FOOTPRINT_FILE_TYPE fileType);

		// Token: 0x06000060 RID: 96
		IAppxBlockMapReader GetBlockMap();

		// Token: 0x06000061 RID: 97
		IAppxBundleManifestReader GetManifest();

		// Token: 0x06000062 RID: 98
		IAppxFilesEnumerator GetPayloadPackages();

		// Token: 0x06000063 RID: 99
		IAppxFile GetPayloadPackage([MarshalAs(UnmanagedType.LPWStr)] [In] string fileName);
	}
}
