using System;

namespace USB_Test_Infrastructure
{
	// Token: 0x0200001C RID: 28
	internal class DTSFUsbStreamReadAsyncResult : AsyncResult<int>
	{
		// Token: 0x0600003C RID: 60 RVA: 0x000025EC File Offset: 0x000007EC
		public DTSFUsbStreamReadAsyncResult(AsyncCallback callback, object state) : base(callback, state)
		{
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600003D RID: 61 RVA: 0x000025F6 File Offset: 0x000007F6
		// (set) Token: 0x0600003E RID: 62 RVA: 0x000025FE File Offset: 0x000007FE
		public byte[] Buffer { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600003F RID: 63 RVA: 0x00002607 File Offset: 0x00000807
		// (set) Token: 0x06000040 RID: 64 RVA: 0x0000260F File Offset: 0x0000080F
		public int Offset { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000041 RID: 65 RVA: 0x00002618 File Offset: 0x00000818
		// (set) Token: 0x06000042 RID: 66 RVA: 0x00002620 File Offset: 0x00000820
		public int Count { get; set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000043 RID: 67 RVA: 0x00002629 File Offset: 0x00000829
		// (set) Token: 0x06000044 RID: 68 RVA: 0x00002631 File Offset: 0x00000831
		public int RetryCount { get; set; }
	}
}
