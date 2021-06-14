using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000019 RID: 25
	[CLSCompliant(false)]
	[Guid("4e1bd148-55a0-4480-a3d1-15544710637c")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAppxManifestReader
	{
		// Token: 0x06000075 RID: 117
		IAppxManifestPackageId GetPackageId();

		// Token: 0x06000076 RID: 118
		IAppxManifestProperties GetProperties();

		// Token: 0x06000077 RID: 119
		IAppxManifestPackageDependenciesEnumerator GetPackageDependencies();

		// Token: 0x06000078 RID: 120
		APPX_CAPABILITIES GetCapabilities();

		// Token: 0x06000079 RID: 121
		IAppxManifestResourcesEnumerator GetResources();

		// Token: 0x0600007A RID: 122
		IAppxManifestDeviceCapabilitiesEnumerator GetDeviceCapabilities();

		// Token: 0x0600007B RID: 123
		ulong GetPrerequisite([MarshalAs(UnmanagedType.LPWStr)] [In] string name);

		// Token: 0x0600007C RID: 124
		IAppxManifestApplicationsEnumerator GetApplications();

		// Token: 0x0600007D RID: 125
		IStream GetStream();
	}
}
