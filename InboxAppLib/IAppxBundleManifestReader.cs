using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000015 RID: 21
	[CLSCompliant(false)]
	[Guid("CF0EBBC1-CC99-4106-91EB-E67462E04FB0")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAppxBundleManifestReader
	{
		// Token: 0x06000064 RID: 100
		IAppxManifestPackageId GetPackageId();

		// Token: 0x06000065 RID: 101
		IAppxBundleManifestPackageInfoEnumerator GetPackageInfoItems();

		// Token: 0x06000066 RID: 102
		IStream GetStream();
	}
}
