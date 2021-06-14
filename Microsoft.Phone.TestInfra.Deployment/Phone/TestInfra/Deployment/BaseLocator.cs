using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.MobileDevices.TestInfrastructure.UtilityLibrary;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000019 RID: 25
	public class BaseLocator
	{
		// Token: 0x0600011A RID: 282 RVA: 0x00006FC0 File Offset: 0x000051C0
		static BaseLocator()
		{
			BaseLocator.immutableCacheFolder = Path.Combine(BaseLocator.BinaryLocationCacheFolder, BaseLocator.ImmutableCacheFolderName);
			BaseLocator.CleanCacheFiles();
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00007048 File Offset: 0x00005248
		internal static void WriteCacheFile(string cacheFile, SerializableDictionary<string, HashSet<string>> locatorInfo)
		{
			bool flag = locatorInfo == null;
			if (flag)
			{
				throw new ArgumentNullException("locatorInfo");
			}
			using (ReadWriteResourceLock readWriteResourceLock = BaseLocator.CreateLockForIndexFile(cacheFile))
			{
				readWriteResourceLock.AcquireWriteLock(TimeSpan.FromSeconds(15.0));
				RetryHelper.Retry(delegate()
				{
					locatorInfo.SerializeToFile(cacheFile);
				}, 6, BaseLocator.DefaultRetryDelay, new Type[]
				{
					typeof(IOException)
				});
			}
		}

		// Token: 0x0600011C RID: 284 RVA: 0x000070F0 File Offset: 0x000052F0
		internal static string GetLocalGeneralCacheFileFullPath(string cacheFileName)
		{
			return Path.Combine(BaseLocator.BinaryLocationCacheFolder, cacheFileName);
		}

		// Token: 0x0600011D RID: 285 RVA: 0x00007110 File Offset: 0x00005310
		internal static SerializableDictionary<string, HashSet<string>> ReadGeneralCacheFile(string cacheFileName)
		{
			string text = Path.Combine(Constants.AssemblyDirectory, cacheFileName);
			string localGeneralCacheFileFullPath = BaseLocator.GetLocalGeneralCacheFileFullPath(cacheFileName);
			bool flag = !File.Exists(text) && !File.Exists(localGeneralCacheFileFullPath);
			SerializableDictionary<string, HashSet<string>> result;
			if (flag)
			{
				Logger.Info("Cache file {0} is not available, probably it is not generated yet. Moving on", new object[]
				{
					cacheFileName
				});
				result = new SerializableDictionary<string, HashSet<string>>();
			}
			else
			{
				string cacheFile = string.Empty;
				bool flag2 = File.Exists(text);
				if (flag2)
				{
					cacheFile = text;
					bool flag3 = File.Exists(localGeneralCacheFileFullPath);
					if (flag3)
					{
						DateTime lastWriteTimeUtc = File.GetLastWriteTimeUtc(text);
						DateTime lastWriteTimeUtc2 = File.GetLastWriteTimeUtc(localGeneralCacheFileFullPath);
						bool flag4 = DateTime.Compare(lastWriteTimeUtc, lastWriteTimeUtc2) < 0;
						if (flag4)
						{
							cacheFile = localGeneralCacheFileFullPath;
						}
					}
				}
				else
				{
					cacheFile = localGeneralCacheFileFullPath;
				}
				result = BaseLocator.ReadCacheFile(cacheFile);
			}
			return result;
		}

		// Token: 0x0600011E RID: 286 RVA: 0x000071C4 File Offset: 0x000053C4
		internal static SerializableDictionary<string, HashSet<string>> ReadCacheFile(string cacheFile)
		{
			SerializableDictionary<string, HashSet<string>> serializableDictionary = new SerializableDictionary<string, HashSet<string>>();
			bool flag = !File.Exists(cacheFile);
			SerializableDictionary<string, HashSet<string>> result;
			if (flag)
			{
				Logger.Info("Cache file {0} is not available, probably it is not generated yet. Moving on", new object[]
				{
					cacheFile
				});
				result = serializableDictionary;
			}
			else
			{
				using (ReadWriteResourceLock readWriteResourceLock = BaseLocator.CreateLockForIndexFile(cacheFile))
				{
					readWriteResourceLock.AcquireReadLock(TimeSpan.FromSeconds(15.0));
					try
					{
						SerializableDictionary<string, IEnumerable<string>> serializableDictionary2 = SerializableDictionary<string, IEnumerable<string>>.DeserializeFile(cacheFile);
						foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair in serializableDictionary2)
						{
							HashSet<string> hashSet = new HashSet<string>();
							hashSet.UnionWith(keyValuePair.Value);
							serializableDictionary.Add(keyValuePair.Key, hashSet);
						}
						result = serializableDictionary;
					}
					catch (Exception ex)
					{
						Logger.Debug("Error occurred in loading the cache file {0}, skipped. Error: {0}. ", new object[]
						{
							cacheFile,
							ex.ToString()
						});
						result = serializableDictionary;
					}
				}
			}
			return result;
		}

		// Token: 0x0600011F RID: 287 RVA: 0x000072E0 File Offset: 0x000054E0
		internal static bool IsPathImmutable(string rootPath)
		{
			bool flag = string.IsNullOrEmpty(rootPath);
			if (flag)
			{
				throw new ArgumentNullException(rootPath);
			}
			bool flag2 = !ReliableDirectory.Exists(rootPath, Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs));
			if (flag2)
			{
				throw new InvalidDataException(string.Format("Directory {0} not exist.", rootPath));
			}
			PathType pathType = PathHelper.GetPathType(rootPath);
			return pathType == PathType.PhoneBuildPath || pathType == PathType.WinbBuildPath;
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00007350 File Offset: 0x00005550
		internal static string GetLocationCacheFilePath(string rootPath, string fileExtension)
		{
			bool flag = string.IsNullOrEmpty(rootPath);
			if (flag)
			{
				throw new ArgumentNullException(rootPath);
			}
			bool flag2 = !ReliableDirectory.Exists(rootPath, Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs));
			if (flag2)
			{
				throw new InvalidDataException(string.Format("Directory {0} not exist.", rootPath));
			}
			string text = rootPath.ToLowerInvariant().GetHashCode().ToString() + fileExtension;
			bool flag3 = BaseLocator.IsPathImmutable(rootPath);
			if (flag3)
			{
				text = Path.Combine(BaseLocator.immutableCacheFolder, text);
			}
			else
			{
				text = Path.Combine(BaseLocator.volatileCacheFolder, text);
			}
			return text;
		}

		// Token: 0x06000121 RID: 289 RVA: 0x000073F4 File Offset: 0x000055F4
		internal static void CleanCacheFiles()
		{
			try
			{
				bool flag = !Directory.Exists(BaseLocator.BinaryLocationCacheFolder);
				if (!flag)
				{
					using (ReadWriteResourceLock readWriteResourceLock = BaseLocator.CreateLockForIndexFile(BaseLocator.BinaryLocationCacheFolder))
					{
						readWriteResourceLock.AcquireReadLock(TimeSpan.FromSeconds(15.0));
						foreach (string path in Directory.GetDirectories(BaseLocator.BinaryLocationCacheFolder))
						{
							bool flag2 = string.Compare(Path.GetFileName(path), BaseLocator.ImmutableCacheFolderName, true) != 0;
							if (flag2)
							{
								Directory.Delete(path, true);
							}
						}
						bool flag3 = Directory.Exists(BaseLocator.immutableCacheFolder);
						if (flag3)
						{
							string[] files = Directory.GetFiles(BaseLocator.immutableCacheFolder, "*.*", SearchOption.AllDirectories);
							foreach (string fileName in files)
							{
								FileInfo fileInfo = new FileInfo(fileName);
								bool flag4 = fileInfo.LastAccessTime < DateTime.Now.AddDays((double)(0 - Settings.Default.LocationCacheExpiresInDays));
								if (flag4)
								{
									fileInfo.Delete();
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in removing expired binary location files. Error: {0}. Moving on", new object[]
				{
					ex.Message
				});
			}
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00007570 File Offset: 0x00005770
		protected static ReadWriteResourceLock CreateLockForIndexFile(string cacheFile)
		{
			return new ReadWriteResourceLock(string.Format(CultureInfo.InvariantCulture, "DeployTestLocator_{0}", new object[]
			{
				Path.GetFileNameWithoutExtension(cacheFile)
			}));
		}

		// Token: 0x0400006B RID: 107
		private const string DeployTestCacheFileLockFormat = "DeployTestLocator_{0}";

		// Token: 0x0400006C RID: 108
		private const int DefaultRetryCount = 6;

		// Token: 0x0400006D RID: 109
		private static readonly TimeSpan DefaultRetryDelay = TimeSpan.FromMilliseconds(200.0);

		// Token: 0x0400006E RID: 110
		private static readonly string BinaryLocationCacheFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DeployTest\\LocationCache");

		// Token: 0x0400006F RID: 111
		private static readonly string ImmutableCacheFolderName = "Immutable";

		// Token: 0x04000070 RID: 112
		private static string immutableCacheFolder;

		// Token: 0x04000071 RID: 113
		private static string volatileCacheFolder = Path.Combine(BaseLocator.BinaryLocationCacheFolder, Guid.NewGuid().GetHashCode().ToString());
	}
}
