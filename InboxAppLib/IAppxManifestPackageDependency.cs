using System;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000010 RID: 16
	[CLSCompliant(false)]
	[Guid("e4946b59-733e-43f0-a724-3bde4c1285a0")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAppxManifestPackageDependency
	{
		// Token: 0x06000053 RID: 83
		[return: MarshalAs(UnmanagedType.LPWStr)]
		string GetName();

		// Token: 0x06000054 RID: 84
		[return: MarshalAs(UnmanagedType.LPWStr)]
		string GetPublisher();

		// Token: 0x06000055 RID: 85
		ulong GetMinVersion();
	}
}
