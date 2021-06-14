using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000011 RID: 17
	[CLSCompliant(false)]
	[Guid("BBA65864-965F-4A5F-855F-F074BDBF3A7B")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAppxBundleFactory
	{
		// Token: 0x06000056 RID: 86
		IAppxBundleWriter CreateBundleWriter([In] IStream outputStream, [In] ulong bundleVersion);

		// Token: 0x06000057 RID: 87
		IAppxBundleReader CreateBundleReader([In] IStream inputStream);

		// Token: 0x06000058 RID: 88
		IAppxBundleManifestReader CreateBundleManifestReader([In] IStream inputStream);
	}
}
