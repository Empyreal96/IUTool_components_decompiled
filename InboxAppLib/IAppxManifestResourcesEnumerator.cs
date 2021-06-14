using System;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x0200001E RID: 30
	[Guid("de4dfbbd-881a-48bb-858c-d6f2baeae6ed")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAppxManifestResourcesEnumerator
	{
		// Token: 0x06000097 RID: 151
		[return: MarshalAs(UnmanagedType.LPWStr)]
		string GetCurrent();

		// Token: 0x06000098 RID: 152
		bool GetHasCurrent();

		// Token: 0x06000099 RID: 153
		bool MoveNext();
	}
}
