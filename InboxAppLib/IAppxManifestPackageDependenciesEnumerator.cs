using System;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x0200000F RID: 15
	[CLSCompliant(false)]
	[Guid("b43bbcf9-65a6-42dd-bac0-8c6741e7f5a4")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAppxManifestPackageDependenciesEnumerator
	{
		// Token: 0x06000050 RID: 80
		IAppxManifestPackageDependency GetCurrent();

		// Token: 0x06000051 RID: 81
		bool GetHasCurrent();

		// Token: 0x06000052 RID: 82
		bool MoveNext();
	}
}
