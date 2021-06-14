using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x0200001F RID: 31
	public class PerformanceCounters
	{
		// Token: 0x06000150 RID: 336 RVA: 0x00008D98 File Offset: 0x00006F98
		private PerformanceCounters()
		{
			this.TimeCopyingToCache = new Stopwatch();
			this.TimeCopyingToDest = new Stopwatch();
			this.TimeSpentPurging = new Stopwatch();
			this.TimeWaitingOnWritelock = new Stopwatch();
			this.TimeWaitingOnReadlock = new Stopwatch();
			this.TimeWaitingOnNetThrottle = new Stopwatch();
			this.TimeWaitingOnLocalThrottle = new Stopwatch();
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000151 RID: 337 RVA: 0x00008E04 File Offset: 0x00007004
		public static PerformanceCounters Instance
		{
			get
			{
				return PerformanceCounters.LazyInstance.Value;
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000152 RID: 338 RVA: 0x00008E20 File Offset: 0x00007020
		// (set) Token: 0x06000153 RID: 339 RVA: 0x00008E28 File Offset: 0x00007028
		public Stopwatch TimeCopyingToCache { get; private set; }

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000154 RID: 340 RVA: 0x00008E31 File Offset: 0x00007031
		// (set) Token: 0x06000155 RID: 341 RVA: 0x00008E39 File Offset: 0x00007039
		public Stopwatch TimeCopyingToDest { get; private set; }

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000156 RID: 342 RVA: 0x00008E42 File Offset: 0x00007042
		// (set) Token: 0x06000157 RID: 343 RVA: 0x00008E4A File Offset: 0x0000704A
		public Stopwatch TimeWaitingOnWritelock { get; private set; }

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000158 RID: 344 RVA: 0x00008E53 File Offset: 0x00007053
		// (set) Token: 0x06000159 RID: 345 RVA: 0x00008E5B File Offset: 0x0000705B
		public Stopwatch TimeWaitingOnReadlock { get; private set; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600015A RID: 346 RVA: 0x00008E64 File Offset: 0x00007064
		// (set) Token: 0x0600015B RID: 347 RVA: 0x00008E6C File Offset: 0x0000706C
		public Stopwatch TimeWaitingOnNetThrottle { get; private set; }

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x0600015C RID: 348 RVA: 0x00008E75 File Offset: 0x00007075
		// (set) Token: 0x0600015D RID: 349 RVA: 0x00008E7D File Offset: 0x0000707D
		public Stopwatch TimeWaitingOnLocalThrottle { get; private set; }

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x0600015E RID: 350 RVA: 0x00008E86 File Offset: 0x00007086
		// (set) Token: 0x0600015F RID: 351 RVA: 0x00008E8E File Offset: 0x0000708E
		public Stopwatch TimeSpentPurging { get; private set; }

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000160 RID: 352 RVA: 0x00008E98 File Offset: 0x00007098
		public int NumFilesFound
		{
			get
			{
				return this.numFilesFound;
			}
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000161 RID: 353 RVA: 0x00008EB0 File Offset: 0x000070B0
		public int CacheMisses
		{
			get
			{
				return this.cacheMisses;
			}
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000162 RID: 354 RVA: 0x00008EC8 File Offset: 0x000070C8
		public int CacheHits
		{
			get
			{
				return this.cacheHits;
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000163 RID: 355 RVA: 0x00008EE0 File Offset: 0x000070E0
		public int FilesCopiedToCache
		{
			get
			{
				return this.filesCopiedToCache;
			}
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000164 RID: 356 RVA: 0x00008EF8 File Offset: 0x000070F8
		public int FilesCopiedFromCache
		{
			get
			{
				return this.filesCopiedFromCache;
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000165 RID: 357 RVA: 0x00008F10 File Offset: 0x00007110
		public int FilesCopiedFromSource
		{
			get
			{
				return this.filesCopiedFromSource;
			}
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00008F28 File Offset: 0x00007128
		public void Reset()
		{
			this.TimeCopyingToCache.Reset();
			this.TimeCopyingToDest.Reset();
			this.TimeSpentPurging.Reset();
			this.TimeWaitingOnWritelock.Reset();
			this.TimeWaitingOnReadlock.Reset();
			this.TimeWaitingOnNetThrottle.Reset();
			this.TimeWaitingOnLocalThrottle.Reset();
			this.numFilesFound = 0;
			this.cacheMisses = 0;
			this.cacheHits = 0;
			this.filesCopiedToCache = 0;
			this.filesCopiedFromCache = 0;
			this.filesCopiedFromSource = 0;
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00008FB4 File Offset: 0x000071B4
		public void AddNumFilesFound(int value)
		{
			Interlocked.Add(ref this.numFilesFound, value);
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00008FC4 File Offset: 0x000071C4
		public void AddCacheMisses(int value)
		{
			Interlocked.Add(ref this.cacheMisses, value);
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00008FD4 File Offset: 0x000071D4
		public void AddCacheHits(int value)
		{
			Interlocked.Add(ref this.cacheHits, value);
		}

		// Token: 0x0600016A RID: 362 RVA: 0x00008FE4 File Offset: 0x000071E4
		public void AddFilesCopiedToCache(int value)
		{
			Interlocked.Add(ref this.filesCopiedToCache, value);
		}

		// Token: 0x0600016B RID: 363 RVA: 0x00008FF4 File Offset: 0x000071F4
		public void AddFilesCopiedFromCache(int value)
		{
			Interlocked.Add(ref this.filesCopiedFromCache, value);
		}

		// Token: 0x0600016C RID: 364 RVA: 0x00009004 File Offset: 0x00007204
		public void AddFilesCopiedFromSource(int value)
		{
			Interlocked.Add(ref this.filesCopiedFromSource, value);
		}

		// Token: 0x04000085 RID: 133
		private static readonly Lazy<PerformanceCounters> LazyInstance = new Lazy<PerformanceCounters>(() => new PerformanceCounters());

		// Token: 0x04000086 RID: 134
		private int numFilesFound;

		// Token: 0x04000087 RID: 135
		private int cacheMisses;

		// Token: 0x04000088 RID: 136
		private int cacheHits;

		// Token: 0x04000089 RID: 137
		private int filesCopiedToCache;

		// Token: 0x0400008A RID: 138
		private int filesCopiedFromCache;

		// Token: 0x0400008B RID: 139
		private int filesCopiedFromSource;
	}
}
