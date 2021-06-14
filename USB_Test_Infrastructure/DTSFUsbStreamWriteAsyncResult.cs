using System;

namespace USB_Test_Infrastructure
{
	// Token: 0x0200001D RID: 29
	internal class DTSFUsbStreamWriteAsyncResult : AsyncResultNoResult
	{
		// Token: 0x06000045 RID: 69 RVA: 0x00002050 File Offset: 0x00000250
		public DTSFUsbStreamWriteAsyncResult(AsyncCallback callback, object state) : base(callback, state)
		{
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000046 RID: 70 RVA: 0x0000263A File Offset: 0x0000083A
		// (set) Token: 0x06000047 RID: 71 RVA: 0x00002642 File Offset: 0x00000842
		public byte[] Buffer { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000048 RID: 72 RVA: 0x0000264B File Offset: 0x0000084B
		// (set) Token: 0x06000049 RID: 73 RVA: 0x00002653 File Offset: 0x00000853
		public int Offset { get; set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600004A RID: 74 RVA: 0x0000265C File Offset: 0x0000085C
		// (set) Token: 0x0600004B RID: 75 RVA: 0x00002664 File Offset: 0x00000864
		public int Count { get; set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600004C RID: 76 RVA: 0x0000266D File Offset: 0x0000086D
		// (set) Token: 0x0600004D RID: 77 RVA: 0x00002675 File Offset: 0x00000875
		public int RetryCount { get; set; }
	}
}
