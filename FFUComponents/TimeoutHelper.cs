using System;
using System.Diagnostics;

namespace FFUComponents
{
	// Token: 0x02000042 RID: 66
	internal class TimeoutHelper
	{
		// Token: 0x06000186 RID: 390 RVA: 0x00007CA4 File Offset: 0x00005EA4
		public TimeoutHelper(int timeoutMilliseconds) : this(TimeSpan.FromMilliseconds((double)timeoutMilliseconds))
		{
		}

		// Token: 0x06000187 RID: 391 RVA: 0x00007CB3 File Offset: 0x00005EB3
		public TimeoutHelper(TimeSpan timeout)
		{
			this.timeout = timeout;
			this.stopWatch = Stopwatch.StartNew();
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000188 RID: 392 RVA: 0x00007CCD File Offset: 0x00005ECD
		public bool HasExpired
		{
			get
			{
				return this.stopWatch.Elapsed > this.timeout;
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000189 RID: 393 RVA: 0x00007CE5 File Offset: 0x00005EE5
		public TimeSpan Remaining
		{
			get
			{
				return this.timeout - this.stopWatch.Elapsed;
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x0600018A RID: 394 RVA: 0x00007CFD File Offset: 0x00005EFD
		public TimeSpan Elapsed
		{
			get
			{
				return this.stopWatch.Elapsed;
			}
		}

		// Token: 0x04000110 RID: 272
		private TimeSpan timeout;

		// Token: 0x04000111 RID: 273
		private Stopwatch stopWatch;
	}
}
