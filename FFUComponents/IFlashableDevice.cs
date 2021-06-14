using System;
using System.Runtime.InteropServices;

namespace FFUComponents
{
	// Token: 0x0200000D RID: 13
	[Guid("4EE1152F-246E-4BA3-84D1-2B6C96170E18")]
	[ComVisible(true)]
	public interface IFlashableDevice
	{
		// Token: 0x06000058 RID: 88
		[DispId(1)]
		string GetFriendlyName();

		// Token: 0x06000059 RID: 89
		[DispId(2)]
		string GetUniqueIDStr();

		// Token: 0x0600005A RID: 90
		[DispId(3)]
		string GetSerialNumberStr();

		// Token: 0x0600005B RID: 91
		[DispId(4)]
		bool FlashFFU(string filePath);
	}
}
