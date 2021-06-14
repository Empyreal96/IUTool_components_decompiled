using System;

namespace FFUComponents
{
	// Token: 0x02000047 RID: 71
	internal class DTSFUsbStreamWriteAsyncResult : AsyncResultNoResult
	{
		// Token: 0x060001B7 RID: 439 RVA: 0x000030D2 File Offset: 0x000012D2
		public DTSFUsbStreamWriteAsyncResult(AsyncCallback callback, object state) : base(callback, state)
		{
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060001B8 RID: 440 RVA: 0x00008266 File Offset: 0x00006466
		// (set) Token: 0x060001B9 RID: 441 RVA: 0x0000826E File Offset: 0x0000646E
		public byte[] Buffer { get; set; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060001BA RID: 442 RVA: 0x00008277 File Offset: 0x00006477
		// (set) Token: 0x060001BB RID: 443 RVA: 0x0000827F File Offset: 0x0000647F
		public int Offset { get; set; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060001BC RID: 444 RVA: 0x00008288 File Offset: 0x00006488
		// (set) Token: 0x060001BD RID: 445 RVA: 0x00008290 File Offset: 0x00006490
		public int Count { get; set; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060001BE RID: 446 RVA: 0x00008299 File Offset: 0x00006499
		// (set) Token: 0x060001BF RID: 447 RVA: 0x000082A1 File Offset: 0x000064A1
		public int RetryCount { get; set; }
	}
}
