using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace FFUComponents
{
	// Token: 0x02000011 RID: 17
	[Guid("FAD741FC-3AEA-4FAB-9B8D-CBBF5E265D1B")]
	[ComVisible(true)]
	public interface IFlashingManager
	{
		// Token: 0x0600006A RID: 106
		[DispId(1)]
		bool Start();

		// Token: 0x0600006B RID: 107
		[DispId(2)]
		bool Stop();

		// Token: 0x0600006C RID: 108
		[DispId(3)]
		bool GetFlashableDevices(ref IEnumerator result);

		// Token: 0x0600006D RID: 109
		[DispId(4)]
		IFlashableDevice GetFlashableDevice(string instancePath, bool enableFallback);
	}
}
