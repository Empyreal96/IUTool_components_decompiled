using System;

namespace FFUComponents
{
	// Token: 0x02000046 RID: 70
	internal class DTSFUsbStreamReadAsyncResult : AsyncResult<int>
	{
		// Token: 0x060001AE RID: 430 RVA: 0x00008218 File Offset: 0x00006418
		public DTSFUsbStreamReadAsyncResult(AsyncCallback callback, object state) : base(callback, state)
		{
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060001AF RID: 431 RVA: 0x00008222 File Offset: 0x00006422
		// (set) Token: 0x060001B0 RID: 432 RVA: 0x0000822A File Offset: 0x0000642A
		public byte[] Buffer { get; set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060001B1 RID: 433 RVA: 0x00008233 File Offset: 0x00006433
		// (set) Token: 0x060001B2 RID: 434 RVA: 0x0000823B File Offset: 0x0000643B
		public int Offset { get; set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060001B3 RID: 435 RVA: 0x00008244 File Offset: 0x00006444
		// (set) Token: 0x060001B4 RID: 436 RVA: 0x0000824C File Offset: 0x0000644C
		public int Count { get; set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060001B5 RID: 437 RVA: 0x00008255 File Offset: 0x00006455
		// (set) Token: 0x060001B6 RID: 438 RVA: 0x0000825D File Offset: 0x0000645D
		public int RetryCount { get; set; }
	}
}
