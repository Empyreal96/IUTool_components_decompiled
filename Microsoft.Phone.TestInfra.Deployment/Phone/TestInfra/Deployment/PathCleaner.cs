using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.MobileDevices.TestInfrastructure.UtilityLibrary;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x0200002D RID: 45
	public static class PathCleaner
	{
		// Token: 0x06000210 RID: 528 RVA: 0x0000D4CC File Offset: 0x0000B6CC
		public static void RegisterForCleanup(IEnumerable<string> paths, TimeSpan expiresIn, TimeSpan timeout)
		{
			bool flag = paths == null || !paths.Any<string>();
			if (flag)
			{
				throw new ArgumentNullException("paths");
			}
			TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
			foreach (string path in paths)
			{
				PathCleaner.RegisterForCleanup(path, expiresIn, timeoutHelper.Remaining);
			}
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0000D548 File Offset: 0x0000B748
		public static void RegisterForCleanup(string path, TimeSpan expiresIn, TimeSpan timeout)
		{
			bool flag = string.IsNullOrEmpty(path);
			if (flag)
			{
				throw new ArgumentNullException("path");
			}
			bool flag2 = expiresIn < TimeSpan.Zero || expiresIn < timeout;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("expiresIn", expiresIn, "ExpiresIn is negative or less than timeout");
			}
			TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
			IList<PathCleaner.IndexEntry> index;
			using (ReadWriteResourceLock readWriteResourceLock = new ReadWriteResourceLock("PC_DirectoriesIndex"))
			{
				readWriteResourceLock.AcquireReadLock(timeoutHelper.Remaining);
				index = PathCleaner.ReadDirectoriesIndex().ToList<PathCleaner.IndexEntry>();
			}
			path = new DirectoryInfo(path).FullName;
			HashSet<PathCleaner.IndexEntry> hashSet = new HashSet<PathCleaner.IndexEntry>();
			List<PathCleaner.IndexEntry> source = PathCleaner.GetParentDirectoriesFromIndex(path, index).ToList<PathCleaner.IndexEntry>();
			List<PathCleaner.IndexEntry> source2 = PathCleaner.GetChildDirectoriesFromIndex(path, index).ToList<PathCleaner.IndexEntry>();
			IEnumerable<Mutex> enumerable = null;
			try
			{
				enumerable = PathCleaner.LockDirectories(new List<string>(from e in source
				select e.Path)
				{
					path
				}, timeoutHelper.Remaining);
				long num;
				if (!source2.Any<PathCleaner.IndexEntry>())
				{
					num = 0L;
				}
				else
				{
					num = source2.Max((PathCleaner.IndexEntry child) => child.Timestamp);
				}
				long val = num;
				long timeToSet = Math.Max(DateTime.UtcNow.Add(expiresIn).ToBinary(), val);
				hashSet.Add(new PathCleaner.IndexEntry
				{
					Path = path,
					Timestamp = timeToSet
				});
				hashSet.UnionWith(from p in source
				where p.Timestamp < timeToSet
				select new PathCleaner.IndexEntry
				{
					Path = p.Path,
					Timestamp = timeToSet
				});
				Directory.CreateDirectory(path);
				using (ReadWriteResourceLock readWriteResourceLock2 = new ReadWriteResourceLock("PC_DirectoriesIndex"))
				{
					readWriteResourceLock2.AcquireWriteLock(timeoutHelper.Remaining);
					HashSet<PathCleaner.IndexEntry> hashSet2 = new HashSet<PathCleaner.IndexEntry>();
					hashSet2.UnionWith(hashSet);
					hashSet2.UnionWith(PathCleaner.ReadDirectoriesIndex());
					PathCleaner.WriteDirectoriesIndex(hashSet2);
				}
			}
			finally
			{
				bool flag3 = enumerable != null;
				if (flag3)
				{
					foreach (Mutex mutex in enumerable)
					{
						mutex.ReleaseMutex();
						mutex.Dispose();
					}
				}
			}
		}

		// Token: 0x06000212 RID: 530 RVA: 0x0000D81C File Offset: 0x0000BA1C
		public static void CleanupExpiredDirectories(TimeSpan timeout)
		{
			PerformanceCounters.Instance.TimeSpentPurging.Start();
			try
			{
				TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
				IList<PathCleaner.IndexEntry> list;
				using (ReadWriteResourceLock readWriteResourceLock = new ReadWriteResourceLock("PC_DirectoriesIndex"))
				{
					bool flag = !readWriteResourceLock.TryToAcquireReadLock();
					if (flag)
					{
						return;
					}
					list = PathCleaner.ReadDirectoriesIndex().ToList<PathCleaner.IndexEntry>();
				}
				List<PathCleaner.IndexEntry> list2 = new List<PathCleaner.IndexEntry>();
				foreach (PathCleaner.IndexEntry indexEntry in list)
				{
					bool isExpired = timeoutHelper.IsExpired;
					if (isExpired)
					{
						break;
					}
					using (Mutex mutex = PathCleaner.CreateDirectoryMutex(indexEntry.Path))
					{
						bool flag2 = !mutex.TryToAcquire();
						if (!flag2)
						{
							try
							{
								bool flag3 = PathCleaner.IsExpired(indexEntry, timeoutHelper.Remaining) && PathCleaner.CleanupDirectory(indexEntry.Path);
								if (flag3)
								{
									list2.Add(indexEntry);
								}
							}
							finally
							{
								mutex.ReleaseMutex();
							}
						}
					}
				}
				using (ReadWriteResourceLock readWriteResourceLock2 = new ReadWriteResourceLock("PC_DirectoriesIndex"))
				{
					bool flag4 = !readWriteResourceLock2.TryToAcquireWriteLock();
					if (!flag4)
					{
						list = PathCleaner.ReadDirectoriesIndex().ToList<PathCleaner.IndexEntry>();
						foreach (PathCleaner.IndexEntry item in list2)
						{
							list.Remove(item);
						}
						PathCleaner.WriteDirectoriesIndex(list);
					}
				}
			}
			finally
			{
				PerformanceCounters.Instance.TimeSpentPurging.Stop();
			}
		}

		// Token: 0x06000213 RID: 531 RVA: 0x0000DA74 File Offset: 0x0000BC74
		private static bool IsExpired(PathCleaner.IndexEntry cachedIndex, TimeSpan timeout)
		{
			bool flag = DateTime.UtcNow.ToBinary() < cachedIndex.Timestamp;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				using (ReadWriteResourceLock readWriteResourceLock = new ReadWriteResourceLock("PC_DirectoriesIndex"))
				{
					try
					{
						readWriteResourceLock.AcquireReadLock(timeout);
					}
					catch (TimeoutException)
					{
						return false;
					}
					List<PathCleaner.IndexEntry> list = PathCleaner.ReadDirectoriesIndex().ToList<PathCleaner.IndexEntry>();
					IEnumerable<PathCleaner.IndexEntry> source = list;
					Func<PathCleaner.IndexEntry, bool> <>9__0;
					Func<PathCleaner.IndexEntry, bool> predicate;
					if ((predicate = <>9__0) == null)
					{
						predicate = (<>9__0 = ((PathCleaner.IndexEntry directoryIndex) => string.Equals(directoryIndex.Path, cachedIndex.Path, StringComparison.OrdinalIgnoreCase)));
					}
					using (IEnumerator<PathCleaner.IndexEntry> enumerator = source.Where(predicate).GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							PathCleaner.IndexEntry indexEntry = enumerator.Current;
							return DateTime.UtcNow.ToBinary() >= indexEntry.Timestamp;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000214 RID: 532 RVA: 0x0000DB94 File Offset: 0x0000BD94
		private static IEnumerable<Mutex> LockDirectories(IEnumerable<string> directories, TimeSpan timeout)
		{
			TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
			Mutex mutex = new Mutex(false, "PC_Directories");
			mutex.Acquire(timeoutHelper);
			List<Mutex> list = new List<Mutex>();
			try
			{
				list.AddRange(directories.Select(new Func<string, Mutex>(PathCleaner.CreateDirectoryMutex)));
				WaitHandleHelper.AcquireAll(list, timeoutHelper);
			}
			finally
			{
				mutex.ReleaseMutex();
				mutex.Dispose();
			}
			return list;
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0000DC10 File Offset: 0x0000BE10
		private static bool CleanupDirectory(string path)
		{
			Logger.Info("Directory is expired and will be cleaned up: {0}", new object[]
			{
				path
			});
			bool result;
			try
			{
				Directory.Delete(path, true);
				result = true;
			}
			catch (DirectoryNotFoundException)
			{
				result = true;
			}
			catch (Exception ex)
			{
				Logger.Warning("Unable to delete directory {0}: {1}", new object[]
				{
					path,
					ex
				});
				result = false;
			}
			return result;
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0000DC84 File Offset: 0x0000BE84
		private static Mutex CreateDirectoryMutex(string directory)
		{
			return new Mutex(false, string.Format(CultureInfo.InvariantCulture, "PC_Cleanup_{0}", new object[]
			{
				directory.ToLowerInvariant().GetHashCode()
			}));
		}

		// Token: 0x06000217 RID: 535 RVA: 0x0000DCC4 File Offset: 0x0000BEC4
		private static IEnumerable<PathCleaner.IndexEntry> ReadDirectoriesIndex()
		{
			return RetryHelper.Retry<HashSet<PathCleaner.IndexEntry>>(delegate()
			{
				HashSet<PathCleaner.IndexEntry> hashSet = new HashSet<PathCleaner.IndexEntry>();
				bool flag = File.Exists(PathCleaner.DirectoriesIndexFile);
				if (flag)
				{
					foreach (string text in from line in File.ReadAllLines(PathCleaner.DirectoriesIndexFile)
					where !string.IsNullOrEmpty(line)
					select line)
					{
						try
						{
							PathCleaner.IndexEntry item = PathCleaner.ParseIndexEntry(text.Trim());
							hashSet.Add(item);
						}
						catch (InvalidOperationException)
						{
						}
					}
				}
				return hashSet;
			}, 5, PathCleaner.DefaultRetryDelay, new Type[]
			{
				typeof(IOException)
			}, null);
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000DD14 File Offset: 0x0000BF14
		private static void WriteDirectoriesIndex(IEnumerable<PathCleaner.IndexEntry> entries)
		{
			bool flag = entries == null;
			if (flag)
			{
				entries = new PathCleaner.IndexEntry[0];
			}
			RetryHelper.Retry(delegate()
			{
				Directory.CreateDirectory(Path.GetDirectoryName(PathCleaner.DirectoriesIndexFile));
				File.WriteAllLines(PathCleaner.DirectoriesIndexFile, from e in entries
				select string.Format(CultureInfo.InvariantCulture, "{0}|{1}", new object[]
				{
					e.Path,
					e.Timestamp
				}));
			}, 5, PathCleaner.DefaultRetryDelay, new Type[]
			{
				typeof(IOException)
			});
		}

		// Token: 0x06000219 RID: 537 RVA: 0x0000DD78 File Offset: 0x0000BF78
		private static IEnumerable<PathCleaner.IndexEntry> GetParentDirectoriesFromIndex(string path, IEnumerable<PathCleaner.IndexEntry> index)
		{
			string pathWithSeparator = PathHelper.EndWithDirectorySeparator(path);
			return from curEntry in index
			where pathWithSeparator.StartsWith(PathHelper.EndWithDirectorySeparator(curEntry.Path), StringComparison.OrdinalIgnoreCase)
			select curEntry;
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0000DDB0 File Offset: 0x0000BFB0
		private static IEnumerable<PathCleaner.IndexEntry> GetChildDirectoriesFromIndex(string path, IEnumerable<PathCleaner.IndexEntry> index)
		{
			string pathWithSeparator = PathHelper.EndWithDirectorySeparator(path);
			return from curEntry in index
			where PathHelper.EndWithDirectorySeparator(curEntry.Path).StartsWith(pathWithSeparator, StringComparison.OrdinalIgnoreCase)
			select curEntry;
		}

		// Token: 0x0600021B RID: 539 RVA: 0x0000DDE8 File Offset: 0x0000BFE8
		private static PathCleaner.IndexEntry ParseIndexEntry(string line)
		{
			string[] array = line.Split(new char[]
			{
				'|'
			}, StringSplitOptions.RemoveEmptyEntries);
			bool flag = array.Length != 2;
			if (flag)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Invalid entry: {0}", new object[]
				{
					line
				}));
			}
			PathCleaner.IndexEntry result = new PathCleaner.IndexEntry
			{
				Path = array[0]
			};
			long timestamp;
			bool flag2 = !long.TryParse(array[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out timestamp);
			if (flag2)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Invalid timestamp: {0}", new object[]
				{
					array[1]
				}));
			}
			result.Timestamp = timestamp;
			return result;
		}

		// Token: 0x040000E6 RID: 230
		private const string DirectoriesIndexLockName = "PC_DirectoriesIndex";

		// Token: 0x040000E7 RID: 231
		private const string DirectoriesListMutexName = "PC_Directories";

		// Token: 0x040000E8 RID: 232
		private const string DirectoryMutexNameFormat = "PC_Cleanup_{0}";

		// Token: 0x040000E9 RID: 233
		private const int DefaultRetryCount = 5;

		// Token: 0x040000EA RID: 234
		private static readonly TimeSpan DefaultRetryDelay = TimeSpan.FromMilliseconds(200.0);

		// Token: 0x040000EB RID: 235
		private static readonly string DirectoriesIndexFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft\\PathCleanup\\DirectoriesToCleanup.txt");

		// Token: 0x0200006C RID: 108
		private struct IndexEntry
		{
			// Token: 0x170000B7 RID: 183
			// (get) Token: 0x060002EB RID: 747 RVA: 0x0000FC0F File Offset: 0x0000DE0F
			// (set) Token: 0x060002EC RID: 748 RVA: 0x0000FC17 File Offset: 0x0000DE17
			public string Path { get; set; }

			// Token: 0x170000B8 RID: 184
			// (get) Token: 0x060002ED RID: 749 RVA: 0x0000FC20 File Offset: 0x0000DE20
			// (set) Token: 0x060002EE RID: 750 RVA: 0x0000FC28 File Offset: 0x0000DE28
			public long Timestamp { get; set; }

			// Token: 0x060002EF RID: 751 RVA: 0x0000FC34 File Offset: 0x0000DE34
			public bool Equals(PathCleaner.IndexEntry other)
			{
				return string.Equals(this.Path, other.Path, StringComparison.OrdinalIgnoreCase);
			}

			// Token: 0x060002F0 RID: 752 RVA: 0x0000FC5C File Offset: 0x0000DE5C
			public override bool Equals(object obj)
			{
				bool flag = obj == null;
				return !flag && obj is PathCleaner.IndexEntry && this.Equals((PathCleaner.IndexEntry)obj);
			}

			// Token: 0x060002F1 RID: 753 RVA: 0x0000FC94 File Offset: 0x0000DE94
			public override int GetHashCode()
			{
				return (this.Path != null) ? this.Path.ToLowerInvariant().GetHashCode() : 0;
			}
		}
	}
}
