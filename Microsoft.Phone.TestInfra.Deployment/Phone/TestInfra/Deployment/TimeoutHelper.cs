using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x0200000D RID: 13
	public class TimeoutHelper
	{
		// Token: 0x060000C6 RID: 198 RVA: 0x00004C60 File Offset: 0x00002E60
		public TimeoutHelper(int timeoutMilliseconds) : this(TimeSpan.FromMilliseconds((double)timeoutMilliseconds))
		{
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00004C74 File Offset: 0x00002E74
		public TimeoutHelper(TimeSpan timeout)
		{
			bool flag = timeout < TimeoutHelper.InfiniteTimeSpan;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("timeout", timeout, "Timeout is a negative number other than -1 milliseconds, which represents an infinite time-out.");
			}
			bool flag2 = timeout.TotalMilliseconds > 2147483647.0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("timeout", timeout, "Timeout is greater than Int32.MaxValue.");
			}
			this.timeout = timeout;
			this.stopWatch = Stopwatch.StartNew();
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060000C8 RID: 200 RVA: 0x00004CF0 File Offset: 0x00002EF0
		public bool IsExpired
		{
			get
			{
				bool flag = this.timeout == TimeoutHelper.InfiniteTimeSpan || this.timeout == TimeSpan.Zero;
				return !flag && this.timeout < this.stopWatch.Elapsed;
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060000C9 RID: 201 RVA: 0x00004D48 File Offset: 0x00002F48
		public TimeSpan Remaining
		{
			get
			{
				bool flag = this.timeout == TimeoutHelper.InfiniteTimeSpan || this.timeout == TimeSpan.Zero;
				TimeSpan result;
				if (flag)
				{
					result = this.timeout;
				}
				else
				{
					TimeSpan timeSpan = this.timeout - this.stopWatch.Elapsed;
					result = ((timeSpan == TimeoutHelper.InfiniteTimeSpan) ? TimeSpan.FromMilliseconds(-2.0) : timeSpan);
				}
				return result;
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060000CA RID: 202 RVA: 0x00004DC4 File Offset: 0x00002FC4
		public TimeSpan Elapsed
		{
			get
			{
				return this.stopWatch.Elapsed;
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060000CB RID: 203 RVA: 0x00004DE4 File Offset: 0x00002FE4
		public TimeSpan Timeout
		{
			get
			{
				return this.timeout;
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060000CC RID: 204 RVA: 0x00004DFC File Offset: 0x00002FFC
		public string Status
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "Waited {0:0.000} of {1:0.000} seconds", new object[]
				{
					this.Elapsed.TotalSeconds,
					this.timeout.TotalSeconds
				});
			}
		}

		// Token: 0x04000052 RID: 82
		public static readonly TimeSpan InfiniteTimeSpan = TimeSpan.FromMilliseconds(-1.0);

		// Token: 0x04000053 RID: 83
		private readonly TimeSpan timeout;

		// Token: 0x04000054 RID: 84
		private readonly Stopwatch stopWatch;
	}
}
