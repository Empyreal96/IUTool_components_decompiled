using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Tools.IO;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x0200002B RID: 43
	public class PackageLocator : BaseLocator
	{
		// Token: 0x060001DE RID: 478 RVA: 0x0000AEF8 File Offset: 0x000090F8
		public PackageLocator(IEnumerable<string> rootPaths, IEnumerable<string> altRootPaths = null)
		{
			bool flag = rootPaths == null || !rootPaths.Any<string>() || rootPaths.Any(new Func<string, bool>(string.IsNullOrEmpty));
			if (flag)
			{
				throw new ArgumentNullException("rootPaths");
			}
			this.RootPaths = from x in rootPaths
			select string.IsNullOrEmpty(PathHelper.GetPrebuiltPath(x)) ? x : PathHelper.GetPrebuiltPath(x);
			this.AltRootPaths = altRootPaths;
			bool flag2 = this.AltRootPaths != null && this.AltRootPaths.Any<string>();
			if (flag2)
			{
				this.AltRootPaths = from x in this.AltRootPaths
				select string.IsNullOrEmpty(PathHelper.GetPrebuiltPath(x)) ? x : PathHelper.GetPrebuiltPath(x);
			}
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x060001DF RID: 479 RVA: 0x0000AFD9 File Offset: 0x000091D9
		// (set) Token: 0x060001E0 RID: 480 RVA: 0x0000AFE1 File Offset: 0x000091E1
		public IEnumerable<string> RootPaths { get; private set; }

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060001E1 RID: 481 RVA: 0x0000AFEA File Offset: 0x000091EA
		// (set) Token: 0x060001E2 RID: 482 RVA: 0x0000AFF2 File Offset: 0x000091F2
		public IEnumerable<string> AltRootPaths { get; private set; }

		// Token: 0x060001E3 RID: 483 RVA: 0x0000AFFC File Offset: 0x000091FC
		public PackageInfo FindPackage(string package)
		{
			bool flag = string.IsNullOrEmpty(package);
			if (flag)
			{
				throw new ArgumentNullException("package");
			}
			bool flag2 = this.AltRootPaths != null && this.AltRootPaths.Count<string>() > 0;
			if (flag2)
			{
				PackageInfo packageInfo = this.FindPackage(package, this.AltRootPaths);
				bool flag3 = packageInfo != null;
				if (flag3)
				{
					return packageInfo;
				}
			}
			return this.FindPackage(package, this.RootPaths);
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x0000B070 File Offset: 0x00009270
		public PackageInfo FindPackage(string package, string directory)
		{
			return this.FindPackage(package, new string[]
			{
				directory
			});
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x0000B094 File Offset: 0x00009294
		public PackageInfo FindPackage(string package, IEnumerable<string> directories)
		{
			this.PopulateFromCache(directories);
			PackageInfo packageInfo = this.GetPackageInfoFromDictionary(package);
			bool flag = packageInfo != null;
			PackageInfo result;
			if (flag)
			{
				result = packageInfo;
			}
			else
			{
				packageInfo = this.GetPackageInfoFromGeneralCache(package, directories);
				bool flag2 = packageInfo != null;
				if (flag2)
				{
					result = packageInfo;
				}
				else
				{
					foreach (string text in directories)
					{
						string locationCacheFilePath = BaseLocator.GetLocationCacheFilePath(text, Constants.PackageLocationCacheExtension);
						bool flag3 = this.IsDirectoryCached(text);
						if (flag3)
						{
							Logger.Info("Cache File {0} for root {1} already loaded. Not scanning this directory again.", new object[]
							{
								locationCacheFilePath,
								text
							});
						}
						else
						{
							bool flag4 = File.Exists(locationCacheFilePath);
							if (flag4)
							{
								Logger.Info("Loading newly created cache file {0} for root {1}. ", new object[]
								{
									locationCacheFilePath,
									text
								});
								this.PopulateFromCache(text);
							}
							else
							{
								Logger.Info("Scanning {0} for packages...", new object[]
								{
									text
								});
								this.ScanDirectory(text);
							}
							packageInfo = this.GetPackageInfoFromDictionary(package);
							bool flag5 = packageInfo != null;
							if (flag5)
							{
								return packageInfo;
							}
						}
					}
					foreach (string text2 in directories)
					{
						string locationCacheFilePath2 = BaseLocator.GetLocationCacheFilePath(text2, Constants.PackageLocationCacheExtension);
						bool flag6 = File.Exists(locationCacheFilePath2);
						if (flag6)
						{
							Logger.Info("Saving package location cache file {0} for root {1} for bug analysis.", new object[]
							{
								locationCacheFilePath2,
								text2
							});
							File.Copy(locationCacheFilePath2, Path.Combine(Constants.AssemblyDirectory, Path.GetFileName(locationCacheFilePath2)), true);
						}
						else
						{
							Logger.Warning("package location cache file {0} for root {1} is missing.", new object[]
							{
								locationCacheFilePath2,
								text2
							});
						}
					}
					result = null;
				}
			}
			return result;
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0000B27C File Offset: 0x0000947C
		internal static void UpdateGeneralPackageLocatorCacheFile()
		{
			string localGeneralCacheFileFullPath = BaseLocator.GetLocalGeneralCacheFileFullPath(Constants.GeneralPackageLocationCacheFileName);
			using (ReadWriteResourceLock readWriteResourceLock = BaseLocator.CreateLockForIndexFile(localGeneralCacheFileFullPath))
			{
				readWriteResourceLock.AcquireWriteLock(TimeSpan.FromMinutes(1.0));
				PackageLocator.generalPackageLocationCache.SerializeToFile(localGeneralCacheFileFullPath);
			}
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x0000B2DC File Offset: 0x000094DC
		private PackageInfo GetPackageInfoFromDictionary(string package)
		{
			string fileNameWithoutExtension = PathHelper.GetFileNameWithoutExtension(package, ".spkg");
			bool flag = !this.packageDictionary.ContainsKey(fileNameWithoutExtension);
			PackageInfo result;
			if (flag)
			{
				result = null;
			}
			else
			{
				PackageInfo packageInfo = this.packageDictionary[fileNameWithoutExtension];
				Logger.Info("Found package {0} in {1}", new object[]
				{
					package,
					LongPathPath.GetDirectoryName(packageInfo.AbsolutePath)
				});
				bool flag2 = packageInfo.Count > 1;
				if (flag2)
				{
					Logger.Warning("Package {0} found {1} times, using one from {2}", new object[]
					{
						package,
						packageInfo.Count,
						LongPathPath.GetDirectoryName(packageInfo.AbsolutePath)
					});
				}
				result = packageInfo;
			}
			return result;
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x0000B388 File Offset: 0x00009588
		private void ScanDirectory(string rootDirectory)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(rootDirectory);
			bool flag = !ReliableDirectory.Exists(rootDirectory, 10, PackageLocator.DefaultDirectoryEnumerateRetryDelay);
			if (flag)
			{
				throw new DirectoryNotFoundException(string.Format(CultureInfo.InvariantCulture, "Directory {0} cannot be found", new object[]
				{
					rootDirectory
				}));
			}
			string fullName = directoryInfo.FullName;
			IEnumerable<string> packageFilesUnderPath = PathHelper.GetPackageFilesUnderPath(fullName);
			List<PackageInfo> list = new List<PackageInfo>();
			SerializableDictionary<string, HashSet<string>> serializableDictionary = new SerializableDictionary<string, HashSet<string>>();
			foreach (string text in packageFilesUnderPath)
			{
				PackageInfo packageInfo = new PackageInfo(rootDirectory, PathHelper.ChangeParent(text, rootDirectory, string.Empty));
				this.AddPackageInfo(packageInfo);
				list.Add(packageInfo);
				string key = PathHelper.GetPackageNameWithoutExtension(text).ToLowerInvariant();
				bool flag2 = serializableDictionary.ContainsKey(key);
				if (flag2)
				{
					foreach (string text2 in serializableDictionary[key])
					{
						Logger.Debug("There are more than one packages named {0} under {1}. Package owner should be notified.", new object[]
						{
							LongPathPath.GetFileName(text),
							fullName
						});
					}
					serializableDictionary[key].Add(packageInfo.AbsolutePath);
				}
				else
				{
					serializableDictionary.Add(key, new HashSet<string>
					{
						packageInfo.AbsolutePath
					});
				}
			}
			this.AddCachedDirectory(rootDirectory);
			try
			{
				BaseLocator.WriteCacheFile(BaseLocator.GetLocationCacheFilePath(fullName, Constants.PackageLocationCacheExtension), serializableDictionary);
				foreach (PackageInfo packageInfo2 in list)
				{
					string key2 = PathHelper.GetFileNameWithoutExtension(packageInfo2.PackageName, Constants.SpkgFileExtension).ToLowerInvariant();
					string item = PathHelper.ChangeParent(LongPathPath.GetDirectoryName(packageInfo2.AbsolutePath), fullName, string.Empty).ToLowerInvariant();
					bool flag3 = PackageLocator.generalPackageLocationCache.ContainsKey(key2);
					if (flag3)
					{
						PackageLocator.generalPackageLocationCache[key2].Add(item);
					}
					else
					{
						PackageLocator.generalPackageLocationCache[key2] = new HashSet<string>
						{
							item
						};
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Unable to create package location cache file: {0}", new object[]
				{
					ex
				});
			}
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000B624 File Offset: 0x00009824
		private void AddCachedDirectory(string directory)
		{
			this.cachedDirectories.Add(directory.ToLower(CultureInfo.InvariantCulture));
		}

		// Token: 0x060001EA RID: 490 RVA: 0x0000B640 File Offset: 0x00009840
		private bool IsDirectoryCached(string directory)
		{
			return this.cachedDirectories.Contains(directory.ToLower(CultureInfo.InvariantCulture));
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0000B668 File Offset: 0x00009868
		private void PopulateFromCache(string directory)
		{
			try
			{
				string locationCacheFilePath = BaseLocator.GetLocationCacheFilePath(directory, Constants.PackageLocationCacheExtension);
				bool flag = !File.Exists(locationCacheFilePath);
				if (!flag)
				{
					SerializableDictionary<string, HashSet<string>> serializableDictionary = BaseLocator.ReadCacheFile(locationCacheFilePath);
					bool flag2 = serializableDictionary.Any<KeyValuePair<string, HashSet<string>>>();
					if (flag2)
					{
						this.AddCachedDirectory(directory);
						foreach (KeyValuePair<string, HashSet<string>> keyValuePair in serializableDictionary)
						{
							PackageInfo packageInfo = new PackageInfo(directory, PathHelper.ChangeParent(keyValuePair.Value.ElementAt(0), directory, string.Empty));
							this.AddPackageInfo(packageInfo);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Unable to get cached info: {0}", new object[]
				{
					ex
				});
			}
		}

		// Token: 0x060001EC RID: 492 RVA: 0x0000B748 File Offset: 0x00009948
		private void PopulateFromCache(IEnumerable<string> directories)
		{
			foreach (string directory in directories)
			{
				this.PopulateFromCache(directory);
			}
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0000B798 File Offset: 0x00009998
		private void AddPackageInfo(PackageInfo packageInfo)
		{
			bool flag = this.packageDictionary.ContainsKey(packageInfo.PackageName);
			if (flag)
			{
				bool flag2 = !this.packageDictionary[packageInfo.PackageName].Equals(packageInfo);
				if (flag2)
				{
					Logger.Debug("Duplicate package {0} found in {1} and {2}", new object[]
					{
						packageInfo.PackageName,
						LongPathPath.GetDirectoryName(this.packageDictionary[packageInfo.PackageName].AbsolutePath),
						LongPathPath.GetDirectoryName(packageInfo.AbsolutePath)
					});
					PackageInfo packageInfo2 = this.packageDictionary[packageInfo.PackageName];
					int count = packageInfo2.Count;
					packageInfo2.Count = count + 1;
				}
			}
			else
			{
				this.packageDictionary[packageInfo.PackageName] = packageInfo;
			}
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000B860 File Offset: 0x00009A60
		private PackageInfo GetPackageInfoFromGeneralCache(string package, IEnumerable<string> paths)
		{
			bool flag = PackageLocator.generalPackageLocationCache == null;
			if (flag)
			{
				throw new InvalidDataException("General Package Location Cache is null");
			}
			string text = package + Constants.SpkgFileExtension;
			string text2 = package + Constants.CabFileExtension;
			bool flag2 = !PackageLocator.generalPackageLocationCache.ContainsKey(text) && !PackageLocator.generalPackageLocationCache.ContainsKey(text2);
			PackageInfo result;
			if (flag2)
			{
				result = null;
			}
			else
			{
				foreach (string path in paths)
				{
					string prebuiltPath = PathHelper.GetPrebuiltPath(path);
					bool flag3 = string.IsNullOrWhiteSpace(prebuiltPath);
					if (!flag3)
					{
						HashSet<string> hashSet = PackageLocator.generalPackageLocationCache[package];
						foreach (string text3 in hashSet)
						{
							string path2 = Path.Combine(prebuiltPath, text3, text2);
							string path3 = Path.Combine(prebuiltPath, text3, text);
							bool flag4 = ReliableFile.Exists(path3, Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs));
							if (flag4)
							{
								return new PackageInfo(prebuiltPath, LongPathPath.Combine(text3, text));
							}
							bool flag5 = ReliableFile.Exists(path2, Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs));
							if (flag5)
							{
								return new PackageInfo(prebuiltPath, Path.Combine(text3, text2));
							}
						}
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x040000CB RID: 203
		private const int DefaultDirectoryEnumerateRetryCount = 10;

		// Token: 0x040000CC RID: 204
		private static readonly TimeSpan DefaultDirectoryEnumerateRetryDelay = TimeSpan.FromMinutes(1.0);

		// Token: 0x040000CD RID: 205
		private static SerializableDictionary<string, HashSet<string>> generalPackageLocationCache = BaseLocator.ReadGeneralCacheFile(Constants.GeneralPackageLocationCacheFileName);

		// Token: 0x040000CE RID: 206
		private readonly List<string> cachedDirectories = new List<string>();

		// Token: 0x040000CF RID: 207
		private readonly IDictionary<string, PackageInfo> packageDictionary = new Dictionary<string, PackageInfo>(StringComparer.OrdinalIgnoreCase);
	}
}
