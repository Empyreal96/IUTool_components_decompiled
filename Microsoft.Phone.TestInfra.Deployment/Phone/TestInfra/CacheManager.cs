using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Phone.TestInfra.Deployment;

namespace Microsoft.Phone.TestInfra
{
	// Token: 0x02000008 RID: 8
	public class CacheManager
	{
		// Token: 0x06000030 RID: 48 RVA: 0x000027E4 File Offset: 0x000009E4
		public CacheManager(string cacheRoot)
		{
			this.CacheRoot = cacheRoot;
			this.CacheTimeoutInMs = Settings.Default.CacheTimeoutInMs;
			this.CacheExpirationInDays = Settings.Default.CacheExpirationInDays;
			this.MaxConcurrentDownloads = Settings.Default.MaxConcurrentDownloads;
			this.MaxConcurrentLocalCopies = Settings.Default.MaxConcurrentLocalCopies;
			this.CopyRetryCount = Settings.Default.CopyRetryCount;
			this.CopyRetryDelayInMs = Settings.Default.CopyRetryDelayInMs;
			this.MaxConcurrentReaders = Settings.Default.MaxConcurrentReaders;
			this.DownloadSemaphoreName = Settings.Default.DownloadSemaphoreName;
			this.LocalCopySemaphoreName = Settings.Default.LocalCopySemaphoreName;
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000031 RID: 49 RVA: 0x00002908 File Offset: 0x00000B08
		// (remove) Token: 0x06000032 RID: 50 RVA: 0x00002940 File Offset: 0x00000B40
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<DateTime, TraceLevel, string> LogMessage;

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00002975 File Offset: 0x00000B75
		// (set) Token: 0x06000034 RID: 52 RVA: 0x0000297D File Offset: 0x00000B7D
		public int CacheTimeoutInMs { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000035 RID: 53 RVA: 0x00002986 File Offset: 0x00000B86
		// (set) Token: 0x06000036 RID: 54 RVA: 0x0000298E File Offset: 0x00000B8E
		public int CacheExpirationInDays { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000037 RID: 55 RVA: 0x00002997 File Offset: 0x00000B97
		// (set) Token: 0x06000038 RID: 56 RVA: 0x0000299F File Offset: 0x00000B9F
		public int MaxConcurrentDownloads { get; set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000039 RID: 57 RVA: 0x000029A8 File Offset: 0x00000BA8
		// (set) Token: 0x0600003A RID: 58 RVA: 0x000029B0 File Offset: 0x00000BB0
		public int MaxConcurrentLocalCopies { get; set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600003B RID: 59 RVA: 0x000029B9 File Offset: 0x00000BB9
		// (set) Token: 0x0600003C RID: 60 RVA: 0x000029C1 File Offset: 0x00000BC1
		public int CopyRetryCount { get; set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600003D RID: 61 RVA: 0x000029CA File Offset: 0x00000BCA
		// (set) Token: 0x0600003E RID: 62 RVA: 0x000029D2 File Offset: 0x00000BD2
		public int CopyRetryDelayInMs { get; set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600003F RID: 63 RVA: 0x000029DB File Offset: 0x00000BDB
		// (set) Token: 0x06000040 RID: 64 RVA: 0x000029E3 File Offset: 0x00000BE3
		public int MaxConcurrentReaders { get; set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000041 RID: 65 RVA: 0x000029EC File Offset: 0x00000BEC
		// (set) Token: 0x06000042 RID: 66 RVA: 0x000029F4 File Offset: 0x00000BF4
		public string DownloadSemaphoreName { get; set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000043 RID: 67 RVA: 0x000029FD File Offset: 0x00000BFD
		// (set) Token: 0x06000044 RID: 68 RVA: 0x00002A05 File Offset: 0x00000C05
		public string LocalCopySemaphoreName { get; set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000045 RID: 69 RVA: 0x00002A0E File Offset: 0x00000C0E
		// (set) Token: 0x06000046 RID: 70 RVA: 0x00002A16 File Offset: 0x00000C16
		public bool ContinueOnError { get; set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000047 RID: 71 RVA: 0x00002A1F File Offset: 0x00000C1F
		// (set) Token: 0x06000048 RID: 72 RVA: 0x00002A27 File Offset: 0x00000C27
		public string CacheRoot { get; private set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000049 RID: 73 RVA: 0x00002A30 File Offset: 0x00000C30
		public TimeSpan TotalElapsedTime
		{
			get
			{
				return this.totalElapsedTime.Elapsed;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600004A RID: 74 RVA: 0x00002A50 File Offset: 0x00000C50
		public TimeSpan TimeSpentPurging
		{
			get
			{
				return this.timeSpentPurging.Elapsed;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600004B RID: 75 RVA: 0x00002A70 File Offset: 0x00000C70
		public TimeSpan TimeWaitingOnWriteLock
		{
			get
			{
				return this.timeWaitingOnWritelock.Elapsed;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600004C RID: 76 RVA: 0x00002A90 File Offset: 0x00000C90
		public TimeSpan TimeWaitingOnReadLock
		{
			get
			{
				return this.timeWaitingOnReadlock.Elapsed;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600004D RID: 77 RVA: 0x00002AB0 File Offset: 0x00000CB0
		public TimeSpan TimeWaitingOnNetThrottle
		{
			get
			{
				return this.timeWaitingOnNetThrottle.Elapsed;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600004E RID: 78 RVA: 0x00002AD0 File Offset: 0x00000CD0
		public TimeSpan TimeWaitingOnLocalThrottle
		{
			get
			{
				return this.timeWaitingOnLocalThrottle.Elapsed;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600004F RID: 79 RVA: 0x00002AF0 File Offset: 0x00000CF0
		public TimeSpan TimeCopyingToCache
		{
			get
			{
				return this.timeCopyingToCache.Elapsed;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000050 RID: 80 RVA: 0x00002B10 File Offset: 0x00000D10
		public TimeSpan TimeCopyingToDest
		{
			get
			{
				return this.timeCopyingToDest.Elapsed;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000051 RID: 81 RVA: 0x00002B30 File Offset: 0x00000D30
		public int NumFilesFound
		{
			get
			{
				return this.numFilesFound;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000052 RID: 82 RVA: 0x00002B48 File Offset: 0x00000D48
		public int CacheMisses
		{
			get
			{
				return this.cacheMisses;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000053 RID: 83 RVA: 0x00002B60 File Offset: 0x00000D60
		public int CacheHits
		{
			get
			{
				return this.cacheHits;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000054 RID: 84 RVA: 0x00002B78 File Offset: 0x00000D78
		public int FilesCopiedToCache
		{
			get
			{
				return this.filesCopiedToCache;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000055 RID: 85 RVA: 0x00002B90 File Offset: 0x00000D90
		public int FilesCopiedFromCache
		{
			get
			{
				return this.filesCopiedFromCache;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000056 RID: 86 RVA: 0x00002BA8 File Offset: 0x00000DA8
		public int FilesCopiedFromSource
		{
			get
			{
				return this.filesCopiedFromSource;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00002BC0 File Offset: 0x00000DC0
		public int NumRetriesToCache
		{
			get
			{
				return this.numRetriesToCache;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000058 RID: 88 RVA: 0x00002BD8 File Offset: 0x00000DD8
		public int NumRetriesFromCache
		{
			get
			{
				return this.numRetriesFromCache;
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000059 RID: 89 RVA: 0x00002BF0 File Offset: 0x00000DF0
		public int NumRetriesFromSource
		{
			get
			{
				return this.numRetriesFromSource;
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00002C08 File Offset: 0x00000E08
		public string GetCacheDirectory(string pathToSource)
		{
			pathToSource = Path.GetFullPath(pathToSource);
			bool flag = !CacheManager.ExistsOnDisk(pathToSource);
			if (flag)
			{
				throw new ArgumentException("pathToSource must contain the path to a valid file or directory on disk.");
			}
			bool flag2 = File.Exists(pathToSource);
			if (flag2)
			{
				pathToSource = Path.GetDirectoryName(pathToSource);
			}
			byte[] array = Encoding.ASCII.GetBytes(pathToSource.ToUpperInvariant());
			MD5CryptoServiceProvider obj = this.md5;
			lock (obj)
			{
				array = this.md5.ComputeHash(array);
			}
			string path = BitConverter.ToString(array).Replace("-", string.Empty);
			return Path.Combine(this.CacheRoot, path);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00002CC8 File Offset: 0x00000EC8
		public string GetCacheLocation(string pathToSource)
		{
			string cacheDirectory = this.GetCacheDirectory(pathToSource);
			return Directory.Exists(pathToSource) ? cacheDirectory : Path.Combine(cacheDirectory, Path.GetFileName(pathToSource));
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00002CFC File Offset: 0x00000EFC
		public DateTime GetLastAccess(string pathToSource)
		{
			string timeStampFile = this.GetTimeStampFile(this.GetCacheDirectory(pathToSource));
			bool flag = File.Exists(timeStampFile);
			DateTime result;
			if (flag)
			{
				result = new FileInfo(timeStampFile).LastWriteTime;
			}
			else
			{
				result = DateTime.MinValue;
			}
			return result;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00002D3C File Offset: 0x00000F3C
		public void ResetPerfCounters()
		{
			this.timeCopyingToCache.Reset();
			this.timeCopyingToDest.Reset();
			this.totalElapsedTime.Reset();
			this.timeSpentPurging.Reset();
			this.timeWaitingOnWritelock.Reset();
			this.timeWaitingOnReadlock.Reset();
			this.timeWaitingOnNetThrottle.Reset();
			this.timeWaitingOnLocalThrottle.Reset();
			this.numFilesFound = 0;
			this.cacheMisses = 0;
			this.cacheHits = 0;
			this.filesCopiedToCache = 0;
			this.filesCopiedFromCache = 0;
			this.filesCopiedFromSource = 0;
			this.numRetriesToCache = 0;
			this.numRetriesFromCache = 0;
			this.numRetriesFromSource = 0;
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00002DE9 File Offset: 0x00000FE9
		public void CopyFile(string pathToSource, string pathToDest, Action<string, string> readFromCache, Action<string, string> writeToCache)
		{
			this.CopyFiles(Path.GetDirectoryName(pathToSource), Path.GetFileName(pathToSource), pathToDest, null, readFromCache, writeToCache);
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00002E04 File Offset: 0x00001004
		public void CopyFile(string pathToSource, string pathToDest)
		{
			this.CopyFiles(Path.GetDirectoryName(pathToSource), Path.GetFileName(pathToSource), pathToDest, null);
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00002E1C File Offset: 0x0000101C
		public void CopyFiles(string pathToSource, string pattern, string pathToDest, string exclude)
		{
			this.CopyFiles(pathToSource, pattern, pathToDest, exclude, new Action<string, string>(this.HandleCopy), new Action<string, string>(this.HandleCopy));
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00002E43 File Offset: 0x00001043
		public void CopyFiles(string pathToSource, string pattern, string pathToDest)
		{
			this.CopyFiles(pathToSource, pattern, pathToDest, null, new Action<string, string>(this.HandleCopy), new Action<string, string>(this.HandleCopy));
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00002E6C File Offset: 0x0000106C
		public void CopyFiles(string pathToSource, string pattern, string pathToDest, string exclude, Action<string, string> readFromCache, Action<string, string> writeToCache)
		{
			CacheManager.CheckForNullArguments(pathToSource, pathToDest, readFromCache, writeToCache);
			this.totalElapsedTime.Start();
			TimeoutHelper timeoutHelper = new TimeoutHelper(this.CacheTimeoutInMs);
			pattern = (string.IsNullOrEmpty(pattern) ? "*" : pattern);
			exclude = (exclude ?? string.Empty);
			this.cacheDir = this.GetCacheDirectory(pathToSource);
			bool flag = pattern.Contains("*");
			bool flag2 = flag && File.Exists(pathToDest);
			if (flag2)
			{
				this.totalElapsedTime.Stop();
				throw new InvalidOperationException("Cannot copy multiple source files to a single destination file.");
			}
			string[] array = ReliableDirectory.GetFiles(pathToSource, pattern, this.CopyRetryCount, TimeSpan.FromMilliseconds((double)this.CopyRetryDelayInMs));
			bool flag3 = array.Length == 0;
			if (flag3)
			{
				this.AddToLog(TraceLevel.Info, "No files found in {0} matching pattern {1}.", new object[]
				{
					pathToSource,
					pattern
				});
				this.totalElapsedTime.Stop();
			}
			else
			{
				string[] files = ReliableDirectory.GetFiles(pathToSource, exclude, this.CopyRetryCount, TimeSpan.FromMilliseconds((double)this.CopyRetryDelayInMs));
				bool flag4 = files.Length != 0;
				if (flag4)
				{
					List<string> list = new List<string>(array);
					foreach (string item in files)
					{
						list.Remove(item);
					}
					array = list.ToArray();
				}
				this.numFilesFound += array.Length;
				bool flag5 = flag && !Directory.Exists(pathToDest);
				if (flag5)
				{
					try
					{
						Directory.CreateDirectory(pathToDest);
						this.AddToLog(TraceLevel.Info, "Created destination directory {0}.", new object[]
						{
							pathToDest
						});
					}
					catch
					{
						this.AddToLog(TraceLevel.Warning, "Was unable to create destination directory: {0}.", new object[]
						{
							pathToDest
						});
						bool flag6 = !Directory.Exists(pathToDest);
						if (flag6)
						{
							this.totalElapsedTime.Stop();
							throw;
						}
					}
				}
				bool flag7 = this.CreateCacheRoot();
				string str = CacheManager.MakeSafe(this.cacheDir);
				string text = "Mutex" + str;
				string text2 = "Semaphore" + str;
				bool flag8;
				using (Mutex mutex = new Mutex(false, text, ref flag8))
				{
					Trace.TraceInformation("writeLock {0} was createdNew: {1}.", new object[]
					{
						text,
						flag8
					});
					this.AddToLog(TraceLevel.Verbose, "Waiting to write to cache for source folder {0}, cache folder {1}.", new object[]
					{
						pathToSource,
						this.cacheDir
					});
					this.timeWaitingOnWritelock.Start();
					CacheManager.WaitOneAbandonAware(mutex, text, timeoutHelper);
					this.timeWaitingOnWritelock.Stop();
					bool flag9 = true;
					try
					{
						using (Semaphore semaphore = new Semaphore(this.MaxConcurrentReaders, this.MaxConcurrentReaders, text2, ref flag8))
						{
							Trace.TraceInformation("readLock {0} was createdNew: {1}.", new object[]
							{
								text2,
								flag8
							});
							this.AddToLog(TraceLevel.Verbose, "Waiting to read from cache.", new object[0]);
							this.timeWaitingOnReadlock.Start();
							CacheManager.WaitOneAbandonAware(semaphore, text2, timeoutHelper);
							this.timeWaitingOnReadlock.Stop();
							try
							{
								bool flag10 = flag7;
								if (flag10)
								{
									using (Semaphore semaphore2 = new Semaphore(this.MaxConcurrentDownloads, this.MaxConcurrentDownloads, this.DownloadSemaphoreName, ref flag8))
									{
										Trace.TraceInformation("downloadThrottle {0} was createdNew: {1}.", new object[]
										{
											this.DownloadSemaphoreName,
											flag8
										});
										this.timeWaitingOnNetThrottle.Start();
										this.AddToLog(TraceLevel.Verbose, "Throttling download.", new object[0]);
										CacheManager.WaitOneAbandonAware(semaphore2, this.DownloadSemaphoreName, timeoutHelper);
										this.timeWaitingOnNetThrottle.Stop();
										try
										{
											this.AddToLog(TraceLevel.Verbose, "Copying necessary files to cache.", new object[0]);
											this.CopyFilesToCache(pathToSource, writeToCache, array);
										}
										finally
										{
											int num = semaphore2.Release();
											Trace.TraceInformation("downloadThrottle {0} was releaseCount: {1}.", new object[]
											{
												this.DownloadSemaphoreName,
												num
											});
										}
									}
								}
							}
							finally
							{
								mutex.ReleaseMutex();
								flag9 = false;
							}
							using (Semaphore semaphore3 = new Semaphore(this.MaxConcurrentLocalCopies, this.MaxConcurrentLocalCopies, this.LocalCopySemaphoreName, ref flag8))
							{
								Trace.TraceInformation("localCopyThrottle {0} was createdNew: {1}.", new object[]
								{
									this.LocalCopySemaphoreName,
									flag8
								});
								this.timeWaitingOnLocalThrottle.Start();
								this.AddToLog(TraceLevel.Verbose, "Throttling local copies.", new object[0]);
								CacheManager.WaitOneAbandonAware(semaphore3, this.LocalCopySemaphoreName, timeoutHelper);
								this.timeWaitingOnLocalThrottle.Stop();
								try
								{
									this.AddToLog(TraceLevel.Verbose, "Copying necessary files to destination.", new object[0]);
									this.CopyFilesToDestination(pathToDest, readFromCache, array);
								}
								finally
								{
									int num2 = semaphore3.Release();
									Trace.TraceInformation("localCopyThrottle {0} was releaseCount: {1}.", new object[]
									{
										this.LocalCopySemaphoreName,
										num2
									});
								}
							}
							int num3 = semaphore.Release();
							Trace.TraceInformation("readLock {0} was releaseCount: {1}.", new object[]
							{
								text2,
								num3
							});
						}
					}
					finally
					{
						bool flag11 = flag9;
						if (flag11)
						{
							mutex.ReleaseMutex();
						}
						this.totalElapsedTime.Stop();
					}
				}
			}
		}

		// Token: 0x06000063 RID: 99 RVA: 0x0000346C File Offset: 0x0000166C
		public void Purge(TimeSpan timeout)
		{
			TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
			this.timeSpentPurging.Start();
			this.AddToLog(TraceLevel.Verbose, "Purging cache.", new object[0]);
			foreach (string text in Directory.EnumerateDirectories(this.CacheRoot))
			{
				string timeStampFile = this.GetTimeStampFile(text);
				bool flag = (DateTime.Now - File.GetLastWriteTime(timeStampFile)).TotalDays > (double)this.CacheExpirationInDays;
				if (flag)
				{
					string str = CacheManager.MakeSafe(text);
					string text2 = "Mutex" + str;
					string text3 = "Semaphore" + str;
					bool flag2;
					using (Mutex mutex = new Mutex(false, text2, ref flag2))
					{
						Trace.TraceInformation("writeLock {0} was createdNew: {1}.", new object[]
						{
							text2,
							flag2
						});
						this.AddToLog(TraceLevel.Verbose, "Waiting to write to cache folder {0}.", new object[]
						{
							text
						});
						CacheManager.WaitOneAbandonAware(mutex, text2, timeoutHelper);
						try
						{
							using (Semaphore semaphore = new Semaphore(this.MaxConcurrentReaders, this.MaxConcurrentReaders, text3, ref flag2))
							{
								Trace.TraceInformation("readLock {0} was createdNew: {1}.", new object[]
								{
									text3,
									flag2
								});
								this.AddToLog(TraceLevel.Verbose, "Waiting to read from cache folder.", new object[]
								{
									text
								});
								CacheManager.WaitOneAbandonAware(semaphore, text3, timeoutHelper);
								int num = 0;
								try
								{
								}
								finally
								{
									num = semaphore.Release();
									Trace.TraceInformation("readLock {0} was releaseCount: {1}.", new object[]
									{
										text3,
										num
									});
								}
								bool flag3 = num == this.MaxConcurrentReaders - 1;
								if (flag3)
								{
									timeStampFile = this.GetTimeStampFile(text);
									bool flag4 = (DateTime.Now - File.GetLastWriteTime(timeStampFile)).TotalDays > (double)this.CacheExpirationInDays;
									if (flag4)
									{
										try
										{
											this.AddToLog(TraceLevel.Verbose, "Purging cache folder.", new object[]
											{
												text
											});
											Directory.Delete(text, true);
										}
										catch (Exception ex)
										{
											this.AddToLog(TraceLevel.Warning, "Cache Purge failed for {0} with exception {1}", new object[]
											{
												text,
												ex.Message
											});
										}
									}
								}
							}
						}
						finally
						{
							mutex.ReleaseMutex();
						}
					}
				}
			}
			this.timeSpentPurging.Stop();
		}

		// Token: 0x06000064 RID: 100 RVA: 0x0000376C File Offset: 0x0000196C
		private static void CheckForNullArguments(string pathToSource, string pathToDest, Action<string, string> readFromCache, Action<string, string> writeToCache)
		{
			bool flag = pathToSource == null;
			if (flag)
			{
				throw new ArgumentNullException("pathToSource");
			}
			bool flag2 = pathToDest == null;
			if (flag2)
			{
				throw new ArgumentNullException("pathToDest");
			}
			bool flag3 = readFromCache == null;
			if (flag3)
			{
				throw new ArgumentNullException("readFromCache");
			}
			bool flag4 = writeToCache == null;
			if (flag4)
			{
				throw new ArgumentNullException("writeToCache");
			}
			bool flag5 = !Directory.Exists(pathToSource);
			if (flag5)
			{
				throw new ArgumentException("pathToSource must reference a valid directory.");
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x000037E8 File Offset: 0x000019E8
		private static string FixDestinationName(string pathToSource, string pathToDest)
		{
			return Directory.Exists(pathToDest) ? Path.Combine(pathToDest, Path.GetFileName(pathToSource)) : pathToDest;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00003814 File Offset: 0x00001A14
		private static void WaitOneAbandonAware(WaitHandle wh, string name, TimeoutHelper timeoutHelper)
		{
			try
			{
				TimeSpan remaining = timeoutHelper.Remaining;
				Trace.TraceInformation("Now waiting on {0} remaining milliseconds: {1:0.000} seconds", new object[]
				{
					name,
					remaining.TotalMilliseconds / 1000.0
				});
				bool flag = remaining <= TimeSpan.Zero || !wh.WaitOne(remaining, false);
				if (flag)
				{
					throw new TimeoutException(string.Format(CultureInfo.InvariantCulture, "Timeout waiting for {0} ({1}).", new object[]
					{
						name,
						timeoutHelper.Status
					}));
				}
				Trace.TraceInformation("Wait completed for {0} ({1}).", new object[]
				{
					name,
					timeoutHelper.Status
				});
			}
			catch (AbandonedMutexException)
			{
				Trace.TraceInformation("Wait completed (because the WaitHandle was abandoned) for {0} ({1}).", new object[]
				{
					name,
					timeoutHelper.Status
				});
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x000038F4 File Offset: 0x00001AF4
		private static bool AreMatchingFiles(string source, string destination)
		{
			FileInfo fileInfo = new FileInfo(source);
			FileInfo fileInfo2 = new FileInfo(destination);
			return File.Exists(destination) && fileInfo.Length == fileInfo2.Length && fileInfo.LastWriteTime == fileInfo2.LastWriteTime;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00003940 File Offset: 0x00001B40
		private static string MakeSafe(string fileName)
		{
			return fileName.Replace('\\', '_').Replace('$', '_').Replace(':', '_').ToUpperInvariant();
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00003974 File Offset: 0x00001B74
		private static bool ExistsOnDisk(string pathToSource)
		{
			return File.Exists(pathToSource) || Directory.Exists(pathToSource);
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00003998 File Offset: 0x00001B98
		private bool CreateCacheRoot()
		{
			bool flag = !Directory.Exists(this.CacheRoot);
			if (flag)
			{
				try
				{
					Directory.CreateDirectory(this.CacheRoot);
					this.AddToLog(TraceLevel.Info, "Created cache root {0}.", new object[]
					{
						this.CacheRoot
					});
				}
				catch
				{
					this.AddToLog(TraceLevel.Warning, "Unable to create cache root: {0}. Possibly created by another process.", new object[]
					{
						this.CacheRoot
					});
					bool flag2 = !Directory.Exists(this.CacheRoot);
					if (flag2)
					{
						this.AddToLog(TraceLevel.Warning, "Cache root still does not exist. Caching disabled.", new object[0]);
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00003A48 File Offset: 0x00001C48
		private void CopyFilesToDestination(string pathToDest, Action<string, string> copyFile, string[] sourceFiles)
		{
			try
			{
				this.timeCopyingToDest.Start();
				Parallel.ForEach<string>(sourceFiles, delegate(string sourceFile)
				{
					string cacheLocation = this.GetCacheLocation(sourceFile);
					string text = CacheManager.FixDestinationName(sourceFile, pathToDest);
					bool flag = false;
					bool flag2 = File.Exists(cacheLocation) && !CacheManager.AreMatchingFiles(cacheLocation, text);
					if (flag2)
					{
						int num;
						for (int i = 0; i <= this.CopyRetryCount; i = num)
						{
							try
							{
								copyFile(cacheLocation, text);
								Interlocked.Increment(ref this.filesCopiedFromCache);
								this.AddToLog(TraceLevel.Verbose, "Copied from cache to {0}.", new object[]
								{
									text
								});
								flag = true;
								break;
							}
							catch (Exception ex)
							{
								this.AddToLog(TraceLevel.Warning, "Unable to copy file {0} to the destination due to {1}.", new object[]
								{
									cacheLocation,
									ex.Message
								});
								bool flag3 = (ex is UnauthorizedAccessException || ex is IOException) && i < this.CopyRetryCount;
								if (!flag3)
								{
									break;
								}
								Thread.Sleep(this.CopyRetryDelayInMs);
								Interlocked.Increment(ref this.numRetriesFromCache);
							}
							num = i + 1;
						}
					}
					bool flag4 = !flag;
					if (flag4)
					{
						bool flag5 = !CacheManager.AreMatchingFiles(sourceFile, text);
						if (flag5)
						{
							int j = 0;
							while (j <= this.CopyRetryCount)
							{
								try
								{
									copyFile(sourceFile, text);
									Interlocked.Increment(ref this.filesCopiedFromSource);
									this.AddToLog(TraceLevel.Verbose, "Copied from source to {0}.", new object[]
									{
										text
									});
									break;
								}
								catch (Exception ex2)
								{
									this.AddToLog(TraceLevel.Warning, "Unable to copy file {0} to the destination due to {1}.", new object[]
									{
										sourceFile,
										ex2.Message
									});
									bool flag6 = (ex2 is UnauthorizedAccessException || ex2 is IOException) && j < this.CopyRetryCount;
									if (flag6)
									{
										Thread.Sleep(this.CopyRetryDelayInMs);
										Interlocked.Increment(ref this.numRetriesFromSource);
									}
									else
									{
										bool continueOnError = this.ContinueOnError;
										if (!continueOnError)
										{
											throw;
										}
										this.AddToLog(TraceLevel.Warning, "Unable to copy file {0} to destination. Continuing anyways.", new object[]
										{
											sourceFile
										});
									}
								}
								IL_242:
								int num = j + 1;
								j = num;
								continue;
								goto IL_242;
							}
						}
						else
						{
							this.AddToLog(TraceLevel.Verbose, "Source file {0} exists at destination.", new object[]
							{
								sourceFile
							});
						}
					}
				});
			}
			finally
			{
				this.timeCopyingToDest.Stop();
			}
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00003AB4 File Offset: 0x00001CB4
		private void CopyFilesToCache(string pathToSource, Action<string, string> copyFile, string[] sourceFiles)
		{
			try
			{
				bool flag = !Directory.Exists(this.cacheDir);
				if (flag)
				{
					Directory.CreateDirectory(this.cacheDir);
				}
				this.timeCopyingToCache.Start();
				Parallel.ForEach<string>(sourceFiles, delegate(string sourceFile)
				{
					string cacheLocation = this.GetCacheLocation(sourceFile);
					bool flag2 = CacheManager.AreMatchingFiles(sourceFile, cacheLocation);
					if (flag2)
					{
						this.AddToLog(TraceLevel.Verbose, "Source file {0}, exists in cache.", new object[]
						{
							sourceFile
						});
						Interlocked.Increment(ref this.cacheHits);
					}
					else
					{
						Interlocked.Increment(ref this.cacheMisses);
						int num;
						for (int i = 0; i <= this.CopyRetryCount; i = num)
						{
							try
							{
								copyFile(sourceFile, cacheLocation);
								Interlocked.Increment(ref this.filesCopiedToCache);
								this.AddToLog(TraceLevel.Verbose, "Copied source file {0} to cache.", new object[]
								{
									sourceFile
								});
								break;
							}
							catch (Exception ex)
							{
								this.AddToLog(TraceLevel.Warning, "Unable to copy file {0} to the cache due to {1}.", new object[]
								{
									sourceFile,
									ex.Message
								});
								bool flag3 = (ex is UnauthorizedAccessException || ex is IOException) && i < this.CopyRetryCount;
								if (!flag3)
								{
									break;
								}
								Thread.Sleep(this.CopyRetryDelayInMs);
								Interlocked.Increment(ref this.numRetriesToCache);
							}
							num = i + 1;
						}
					}
				});
			}
			finally
			{
				this.timeCopyingToCache.Stop();
				this.UpdateTimeStamp();
			}
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00003B40 File Offset: 0x00001D40
		private void HandleCopy(string pathToSource, string pathToDest)
		{
			File.Copy(pathToSource, pathToDest, true);
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00003B4C File Offset: 0x00001D4C
		private string GetTimeStampFile(string cacheDir)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(cacheDir);
			return Path.Combine(cacheDir, "TimeStamp_" + directoryInfo.Name.Substring(0, 8));
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00003B84 File Offset: 0x00001D84
		private void UpdateTimeStamp()
		{
			string timeStampFile = this.GetTimeStampFile(this.cacheDir);
			bool flag = timeStampFile != null;
			if (flag)
			{
				FileStream fileStream = File.Create(timeStampFile);
				fileStream.Close();
			}
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00003BB8 File Offset: 0x00001DB8
		private void AddToLog(TraceLevel level, string pattern, params object[] list)
		{
			DateTime now = DateTime.Now;
			string arg = string.Format(pattern, list);
			bool flag = this.LogMessage != null;
			if (flag)
			{
				object obj = this.syncLock;
				lock (obj)
				{
					this.LogMessage(now, level, arg);
				}
			}
		}

		// Token: 0x04000024 RID: 36
		private readonly MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

		// Token: 0x04000025 RID: 37
		private Stopwatch timeCopyingToCache = new Stopwatch();

		// Token: 0x04000026 RID: 38
		private Stopwatch timeCopyingToDest = new Stopwatch();

		// Token: 0x04000027 RID: 39
		private Stopwatch timeWaitingOnWritelock = new Stopwatch();

		// Token: 0x04000028 RID: 40
		private Stopwatch timeWaitingOnReadlock = new Stopwatch();

		// Token: 0x04000029 RID: 41
		private Stopwatch timeWaitingOnNetThrottle = new Stopwatch();

		// Token: 0x0400002A RID: 42
		private Stopwatch timeWaitingOnLocalThrottle = new Stopwatch();

		// Token: 0x0400002B RID: 43
		private Stopwatch timeSpentPurging = new Stopwatch();

		// Token: 0x0400002C RID: 44
		private Stopwatch totalElapsedTime = new Stopwatch();

		// Token: 0x0400002D RID: 45
		private int numFilesFound;

		// Token: 0x0400002E RID: 46
		private int cacheMisses;

		// Token: 0x0400002F RID: 47
		private int cacheHits;

		// Token: 0x04000030 RID: 48
		private int filesCopiedToCache;

		// Token: 0x04000031 RID: 49
		private int filesCopiedFromCache;

		// Token: 0x04000032 RID: 50
		private int filesCopiedFromSource;

		// Token: 0x04000033 RID: 51
		private int numRetriesToCache;

		// Token: 0x04000034 RID: 52
		private int numRetriesFromCache;

		// Token: 0x04000035 RID: 53
		private int numRetriesFromSource;

		// Token: 0x04000036 RID: 54
		private string cacheDir;

		// Token: 0x04000037 RID: 55
		private object syncLock = new object();
	}
}
