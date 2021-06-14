using System;
using System.Diagnostics;

namespace Microsoft.Phone.TestInfra
{
	// Token: 0x02000009 RID: 9
	public class TimeoutHelper
	{
		// Token: 0x06000071 RID: 113 RVA: 0x00003C28 File Offset: 0x00001E28
		public TimeoutHelper(int timeoutMilliseconds) : this(TimeSpan.FromMilliseconds((double)timeoutMilliseconds))
		{
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00003C39 File Offset: 0x00001E39
		public TimeoutHelper(TimeSpan timeout)
		{
			this.timeout = timeout;
			this.stopWatch = Stopwatch.StartNew();
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000073 RID: 115 RVA: 0x00003C58 File Offset: 0x00001E58
		public bool HasExpired
		{
			get
			{
				return this.stopWatch.Elapsed > this.timeout;
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000074 RID: 116 RVA: 0x00003C80 File Offset: 0x00001E80
		public TimeSpan Remaining
		{
			get
			{
				return this.timeout - this.stopWatch.Elapsed;
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000075 RID: 117 RVA: 0x00003CA8 File Offset: 0x00001EA8
		public TimeSpan Elapsed
		{
			get
			{
				return this.stopWatch.Elapsed;
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000076 RID: 118 RVA: 0x00003CC8 File Offset: 0x00001EC8
		public string Status
		{
			get
			{
				return string.Format("waited {0:0.000} of {1:0.000} seconds", this.Elapsed.TotalSeconds, this.timeout.TotalSeconds);
			}
		}

		// Token: 0x04000044 RID: 68
		private TimeSpan timeout;

		// Token: 0x04000045 RID: 69
		private Stopwatch stopWatch;
	}
}
