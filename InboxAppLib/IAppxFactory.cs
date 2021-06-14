using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000013 RID: 19
	[CLSCompliant(false)]
	[Guid("beb94909-e451-438b-b5a7-d79e767b75d8")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAppxFactory
	{
		// Token: 0x0600005A RID: 90
		IAppxPackageWriter CreatePackageWriter([In] IStream outputStream, [In] APPX_PACKAGE_SETTINGS settings);

		// Token: 0x0600005B RID: 91
		IAppxPackageReader CreatePackageReader([In] IStream inputStream);

		// Token: 0x0600005C RID: 92
		IAppxManifestReader CreateManifestReader([In] IStream inputStream);

		// Token: 0x0600005D RID: 93
		IAppxBlockMapReader CreateBlockMapReader([In] IStream inputStream);

		// Token: 0x0600005E RID: 94
		IAppxBlockMapReader CreateValidatedBlockMapReader([In] IStream blockMapStream, [In] string signatureFileName);
	}
}
