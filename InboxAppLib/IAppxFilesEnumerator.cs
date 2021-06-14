using System;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x0200000E RID: 14
	[CLSCompliant(false)]
	[Guid("f007eeaf-9831-411c-9847-917cdc62d1fe")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAppxFilesEnumerator
	{
		// Token: 0x0600004D RID: 77
		IAppxFile GetCurrent();

		// Token: 0x0600004E RID: 78
		bool GetHasCurrent();

		// Token: 0x0600004F RID: 79
		bool MoveNext();
	}
}
