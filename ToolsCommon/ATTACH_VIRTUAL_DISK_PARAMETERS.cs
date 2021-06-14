using System;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000039 RID: 57
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct ATTACH_VIRTUAL_DISK_PARAMETERS
	{
		// Token: 0x040000BC RID: 188
		public ATTACH_VIRTUAL_DISK_VERSION Version;

		// Token: 0x040000BD RID: 189
		public int Reserved;
	}
}
