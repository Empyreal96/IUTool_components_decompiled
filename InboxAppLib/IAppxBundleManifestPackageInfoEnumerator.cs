using System;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000017 RID: 23
	[CLSCompliant(false)]
	[Guid("F9B856EE-49A6-4E19-B2B0-6A2406D63A32")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAppxBundleManifestPackageInfoEnumerator
	{
		// Token: 0x0600006D RID: 109
		IAppxBundleManifestPackageInfo GetCurrent();

		// Token: 0x0600006E RID: 110
		bool GetHasCurrent();

		// Token: 0x0600006F RID: 111
		bool MoveNext();
	}
}
