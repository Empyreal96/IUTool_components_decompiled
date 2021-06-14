using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Microsoft.Tools.IO;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x0200000C RID: 12
	public class CacheManager
	{
		// Token: 0x060000AD RID: 173 RVA: 0x00004470 File Offset: 0x00002670
		public CacheManager(string cacheRoot, TimeSpan? cacheTimeout = null)
		{
			bool flag = string.IsNullOrEmpty(cacheRoot);
			if (flag)
			{
				throw new ArgumentNullException("cacheRoot");
			}
			this.CacheRoot = cacheRoot;
			this.CacheTimeout = ((cacheTimeout != null) ? cacheTimeout.Value : TimeSpan.FromMilliseconds((double)Settings.Default.CacheTimeoutInMs));
			this.CopyRetryCount = Settings.Default.CopyRetryCount;
			this.CopyRetryDelay = TimeSpan.FromMilliseconds((double)Settings.Default.CopyRetryDelayInMs);
			this.MaxConcurrentDownloads = Settings.Default.MaxConcurrentDownloads;
			this.DownloadSemaphoreName = Settings.Default.DownloadSemaphoreName;
			this.MaxConcurrentLocalCopies = Settings.Default.MaxConcurrentLocalCopies;
			this.LocalCopySemaphoreName = Settings.Default.LocalCopySemaphoreName;
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060000AE RID: 174 RVA: 0x00004544 File Offset: 0x00002744
		// (set) Token: 0x060000AF RID: 175 RVA: 0x0000455C File Offset: 0x0000275C
		public TimeSpan CacheTimeout
		{
			get
			{
				return this.cacheTimeout;
			}
			set
			{
				bool flag = value < TimeSpan.Zero;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("value", value, "Cache timeout is negative");
				}
				this.cacheTimeout = value;
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060000B0 RID: 176 RVA: 0x00004598 File Offset: 0x00002798
		// (set) Token: 0x060000B1 RID: 177 RVA: 0x000045B0 File Offset: 0x000027B0
		public int CopyRetryCount
		{
			get
			{
				return this.copyRetryCount;
			}
			set
			{
				bool flag = value < 0;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("value", value, "Retry count is negative");
				}
				this.copyRetryCount = value;
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x000045E4 File Offset: 0x000027E4
		// (set) Token: 0x060000B3 RID: 179 RVA: 0x000045FC File Offset: 0x000027FC
		public TimeSpan CopyRetryDelay
		{
			get
			{
				return this.copyRetryDelay;
			}
			set
			{
				bool flag = value < TimeSpan.Zero;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("value", value, "Retry delay is negative");
				}
				this.copyRetryDelay = value;
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x00004638 File Offset: 0x00002838
		// (set) Token: 0x060000B5 RID: 181 RVA: 0x00004650 File Offset: 0x00002850
		public int MaxConcurrentDownloads
		{
			get
			{
				return this.maxConcurrentDownloads;
			}
			set
			{
				bool flag = value < 1;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("value", value, "Max concurrent download is zero or negative");
				}
				this.maxConcurrentDownloads = value;
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x00004684 File Offset: 0x00002884
		// (set) Token: 0x060000B7 RID: 183 RVA: 0x0000469C File Offset: 0x0000289C
		public string DownloadSemaphoreName
		{
			get
			{
				return this.downloadSemaphoreName;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentNullException("value");
				}
				this.downloadSemaphoreName = value;
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x000046C8 File Offset: 0x000028C8
		// (set) Token: 0x060000B9 RID: 185 RVA: 0x000046E0 File Offset: 0x000028E0
		public int MaxConcurrentLocalCopies
		{
			get
			{
				return this.maxConcurrentLocalCopies;
			}
			set
			{
				bool flag = value < 1;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("value", value, "Max concurrent local copies is zero or negative");
				}
				this.maxConcurrentLocalCopies = value;
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060000BA RID: 186 RVA: 0x00004714 File Offset: 0x00002914
		// (set) Token: 0x060000BB RID: 187 RVA: 0x0000472C File Offset: 0x0000292C
		public string LocalCopySemaphoreName
		{
			get
			{
				return this.localCopySemaphoreName;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentNullException("value");
				}
				this.localCopySemaphoreName = value;
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060000BC RID: 188 RVA: 0x00004757 File Offset: 0x00002957
		// (set) Token: 0x060000BD RID: 189 RVA: 0x0000475F File Offset: 0x0000295F
		public string CacheRoot { get; private set; }

		// Token: 0x060000BE RID: 190 RVA: 0x00004768 File Offset: 0x00002968
		public void AddFileToCache(string filePath, Action<string, string> callback)
		{
			bool flag = string.IsNullOrEmpty(filePath);
			if (flag)
			{
				throw new ArgumentNullException("filePath");
			}
			bool flag2 = !LongPathFile.Exists(filePath);
			if (flag2)
			{
				throw new FileNotFoundException(string.Format(CultureInfo.InvariantCulture, "Source file does not exist: {0}", new object[]
				{
					filePath
				}));
			}
			this.CacheFiles(LongPathPath.GetDirectoryName(filePath), LongPathPath.GetFileName(filePath), new TimeoutHelper(this.CacheTimeout), callback);
		}

		// Token: 0x060000BF RID: 191 RVA: 0x000047D8 File Offset: 0x000029D8
		public void AddFilesToCache(string directory, string pattern, bool recursive, Action<string, string> callback)
		{
			bool flag = directory == null;
			if (flag)
			{
				throw new ArgumentNullException("directory");
			}
			bool flag2 = !Directory.Exists(directory);
			if (flag2)
			{
				throw new DirectoryNotFoundException(string.Format(CultureInfo.InvariantCulture, "Source directory does not exist: {0}", new object[]
				{
					directory
				}));
			}
			pattern = (string.IsNullOrEmpty(pattern) ? "*" : pattern);
			bool flag3 = pattern.Contains("*");
			TimeoutHelper timeoutHelper = new TimeoutHelper(this.CacheTimeout);
			this.CacheFiles(directory, pattern, timeoutHelper, callback);
			bool flag4 = flag3 && recursive;
			if (flag4)
			{
				foreach (string directory2 in Directory.EnumerateDirectories(directory, "*", SearchOption.AllDirectories))
				{
					this.CacheFiles(directory2, pattern, timeoutHelper, callback);
				}
			}
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x000048C0 File Offset: 0x00002AC0
		private void CacheFiles(string directory, string pattern, TimeoutHelper timeoutHelper, Action<string, string> callback)
		{
			string cacheDir = this.CreateCacheDirectory(directory, timeoutHelper);
			using (ReadWriteResourceLock readWriteResourceLock = this.CreateDirectoryLock(cacheDir))
			{
				readWriteResourceLock.AcquireWriteLock(timeoutHelper.Remaining);
				IEnumerable<string> filesAffected = new string[0];
				this.DoWithNetThrottle(delegate
				{
					try
					{
						IEnumerable<string> source;
						filesAffected = FileCopyHelper.CopyFiles(directory, cacheDir, pattern, false, this.copyRetryCount, this.copyRetryDelay, out source);
						int num = filesAffected.Count<string>();
						int num2 = source.Count<string>();
						PerformanceCounters.Instance.AddNumFilesFound(num);
						PerformanceCounters.Instance.AddFilesCopiedToCache(num - num2);
						PerformanceCounters.Instance.AddFilesCopiedFromSource(num - num2);
						PerformanceCounters.Instance.AddCacheHits(num2);
						PerformanceCounters.Instance.AddCacheMisses(num - num2);
					}
					catch (Exception ex)
					{
						Logger.Error("Unable to copy files to cache: {0}", new object[]
						{
							ex
						});
						throw;
					}
				}, timeoutHelper);
				bool flag = callback == null;
				if (!flag)
				{
					this.DoWithLocalThrottle(delegate
					{
						foreach (string text in filesAffected)
						{
							string sourceFile = PathHelper.ChangeParent(text, cacheDir, directory);
							this.InvokeCallback(callback, sourceFile, text);
						}
					}, timeoutHelper);
				}
			}
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00004994 File Offset: 0x00002B94
		private void DoWithNetThrottle(Action copyToCacheAction, TimeoutHelper timeoutHelper)
		{
			using (Semaphore semaphore = new Semaphore(this.MaxConcurrentDownloads, this.MaxConcurrentDownloads, this.DownloadSemaphoreName))
			{
				PerformanceCounters.Instance.TimeWaitingOnNetThrottle.Start();
				try
				{
					semaphore.Acquire(timeoutHelper);
				}
				finally
				{
					PerformanceCounters.Instance.TimeWaitingOnNetThrottle.Stop();
				}
				PerformanceCounters.Instance.TimeCopyingToCache.Start();
				try
				{
					copyToCacheAction();
				}
				finally
				{
					PerformanceCounters.Instance.TimeCopyingToCache.Stop();
					semaphore.Release();
				}
			}
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00004A58 File Offset: 0x00002C58
		private void DoWithLocalThrottle(Action copyToDestinationAction, TimeoutHelper timeoutHelper)
		{
			using (Semaphore semaphore = new Semaphore(this.MaxConcurrentLocalCopies, this.MaxConcurrentLocalCopies, this.LocalCopySemaphoreName))
			{
				PerformanceCounters.Instance.TimeWaitingOnLocalThrottle.Start();
				try
				{
					semaphore.Acquire(timeoutHelper);
				}
				finally
				{
					PerformanceCounters.Instance.TimeWaitingOnLocalThrottle.Stop();
				}
				PerformanceCounters.Instance.TimeCopyingToDest.Start();
				try
				{
					copyToDestinationAction();
					PerformanceCounters.Instance.AddFilesCopiedFromCache(1);
				}
				finally
				{
					PerformanceCounters.Instance.TimeCopyingToDest.Stop();
					semaphore.Release();
				}
			}
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00004B28 File Offset: 0x00002D28
		private void InvokeCallback(Action<string, string> callback, string sourceFile, string cachedFile)
		{
			try
			{
				callback(sourceFile, cachedFile);
			}
			catch (Exception ex)
			{
				Logger.Error("Callback error: {0}", new object[]
				{
					ex
				});
			}
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00004B70 File Offset: 0x00002D70
		private ReadWriteResourceLock CreateDirectoryLock(string path)
		{
			return new ReadWriteResourceLock(string.Format(CultureInfo.InvariantCulture, "CM_{0}", new object[]
			{
				path.ToLowerInvariant().GetHashCode()
			}));
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00004BB0 File Offset: 0x00002DB0
		private string CreateCacheDirectory(string sourceDir, TimeoutHelper timeoutHelper)
		{
			byte[] array = Encoding.Unicode.GetBytes(PathHelper.EndWithDirectorySeparator(sourceDir).ToLowerInvariant());
			MD5CryptoServiceProvider obj = this.md5;
			lock (obj)
			{
				array = this.md5.ComputeHash(array);
			}
			string path = BitConverter.ToString(array).Replace("-", string.Empty);
			string path2 = Path.Combine(this.CacheRoot, path);
			Directory.CreateDirectory(path2);
			PathCleaner.RegisterForCleanup(path2, this.CacheTimeout, timeoutHelper.Remaining);
			return PathHelper.EndWithDirectorySeparator(path2);
		}

		// Token: 0x04000048 RID: 72
		private const string DirectoryLockFormat = "CM_{0}";

		// Token: 0x04000049 RID: 73
		private readonly MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

		// Token: 0x0400004A RID: 74
		private int copyRetryCount;

		// Token: 0x0400004B RID: 75
		private TimeSpan copyRetryDelay;

		// Token: 0x0400004C RID: 76
		private TimeSpan cacheTimeout;

		// Token: 0x0400004D RID: 77
		private int maxConcurrentDownloads;

		// Token: 0x0400004E RID: 78
		private string downloadSemaphoreName;

		// Token: 0x0400004F RID: 79
		private int maxConcurrentLocalCopies;

		// Token: 0x04000050 RID: 80
		private string localCopySemaphoreName;
	}
}
