using System;
using System.Runtime.InteropServices;

namespace FFUComponents
{
	// Token: 0x0200000E RID: 14
	[Guid("323459AA-B365-44FE-A763-AEACCBCA8880")]
	[ComVisible(true)]
	public interface IFlashableDeviceNotify
	{
		// Token: 0x0600005C RID: 92
		[DispId(1)]
		void Progress(long position, long length);
	}
}
