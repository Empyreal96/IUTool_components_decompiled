using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x0200001A RID: 26
	[CLSCompliant(false)]
	[Guid("d06f67bc-b31d-4eba-a8af-638e73e77b4d")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAppxManifestReader2
	{
		// Token: 0x0600007E RID: 126
		IAppxManifestPackageId GetPackageId();

		// Token: 0x0600007F RID: 127
		IAppxManifestProperties GetProperties();

		// Token: 0x06000080 RID: 128
		IAppxManifestPackageDependenciesEnumerator GetPackageDependencies();

		// Token: 0x06000081 RID: 129
		APPX_CAPABILITIES GetCapabilities();

		// Token: 0x06000082 RID: 130
		IAppxManifestResourcesEnumerator GetResources();

		// Token: 0x06000083 RID: 131
		IAppxManifestDeviceCapabilitiesEnumerator GetDeviceCapabilities();

		// Token: 0x06000084 RID: 132
		ulong GetPrerequisite([MarshalAs(UnmanagedType.LPWStr)] [In] string name);

		// Token: 0x06000085 RID: 133
		IAppxManifestApplicationsEnumerator GetApplications();

		// Token: 0x06000086 RID: 134
		IStream GetStream();

		// Token: 0x06000087 RID: 135
		IAppxManifestQualifiedResourcesEnumerator GetQualifiedResources();
	}
}
