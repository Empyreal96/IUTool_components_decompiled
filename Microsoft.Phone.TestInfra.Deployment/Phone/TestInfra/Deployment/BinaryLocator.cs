using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.MobileDevices.TestInfrastructure.UtilityLibrary;
using Microsoft.Phone.Test.TestMetadata.Helper;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x0200001A RID: 26
	public class BinaryLocator : BaseLocator
	{
		// Token: 0x06000124 RID: 292 RVA: 0x000075B0 File Offset: 0x000057B0
		static BinaryLocator()
		{
			string text = Path.Combine(Constants.AssemblyDirectory, Constants.SupressionFileName);
			bool flag = File.Exists(text);
			if (flag)
			{
				BinaryLocator.depSupress = new DependencySuppression(text);
			}
			else
			{
				Logger.Error("File {0} is missing.", new object[]
				{
					text
				});
			}
			BinaryLocator.generalBinaryLocationCache = BaseLocator.ReadGeneralCacheFile(Constants.GeneralBinaryPackageMappingCacheFileName);
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00007620 File Offset: 0x00005820
		public BinaryLocator(PackageLocator packageLocator)
		{
			bool flag = packageLocator == null;
			if (flag)
			{
				throw new ArgumentNullException("packageLocator");
			}
			this.packageLocator = packageLocator;
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00007650 File Offset: 0x00005850
		public static Dictionary<string, PackageDescription> ScanBuild(string path)
		{
			bool flag = string.IsNullOrEmpty(path);
			if (flag)
			{
				throw new ArgumentNullException("path");
			}
			bool flag2 = !Directory.Exists(path);
			if (flag2)
			{
				throw new InvalidDataException(string.Format("Directory {0} not exist.", path));
			}
			Dictionary<string, PackageDescription> packageContents = new Dictionary<string, PackageDescription>();
			string searchPattern = "*" + Constants.ManifestFileExtension;
			IEnumerable<string> subdirsLower = from x in Constants.DependencyProjects
			select x.ToLowerInvariant();
			IEnumerable<string> enumerable = from x in ReliableDirectory.GetDirectories(path, Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs))
			where subdirsLower.Contains(Path.GetFileName(x).ToLowerInvariant())
			select x;
			Queue queue = new Queue();
			Action action = delegate()
			{
				while (queue.Count > 0)
				{
					string text = (string)queue.Dequeue();
					try
					{
						Logger.Debug("Loading {0}", new object[]
						{
							text
						});
						BinaryLocator.LoadManifest(text, packageContents, path);
					}
					catch (Exception ex2)
					{
						Logger.Warning("Error in Loading file {0}, error: {1}.", new object[]
						{
							text,
							ex2.Message
						});
					}
				}
			};
			queue = Queue.Synchronized(queue);
			foreach (string path2 in enumerable)
			{
				string[] files = ReliableDirectory.GetFiles(path2, searchPattern, SearchOption.AllDirectories, Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs));
				foreach (string obj in files)
				{
					queue.Enqueue(obj);
				}
			}
			List<IAsyncResult> list = new List<IAsyncResult>();
			int num;
			for (int j = 0; j < Constants.NumOfLoaders; j = num + 1)
			{
				list.Add(action.BeginInvoke(null, null));
				num = j;
			}
			foreach (IAsyncResult result in list)
			{
				try
				{
					action.EndInvoke(result);
				}
				catch (Exception ex)
				{
					Logger.Error(ex.ToString(), new object[0]);
				}
			}
			Logger.Info("PERF: Loading Finish for path {0} @ {1}", new object[]
			{
				path,
				DateTime.UtcNow
			});
			return packageContents;
		}

		// Token: 0x06000127 RID: 295 RVA: 0x000078C8 File Offset: 0x00005AC8
		public static SerializableDictionary<string, HashSet<string>> ComputeBinaryPackageMapping(Dictionary<string, PackageDescription> packageContents)
		{
			bool flag = packageContents == null;
			if (flag)
			{
				throw new ArgumentNullException("packageContents");
			}
			SerializableDictionary<string, HashSet<string>> serializableDictionary = new SerializableDictionary<string, HashSet<string>>();
			foreach (KeyValuePair<string, PackageDescription> keyValuePair in packageContents)
			{
				string key = keyValuePair.Key;
				foreach (string text in keyValuePair.Value.Binaries)
				{
					string key2 = text.ToLowerInvariant();
					bool flag2 = serializableDictionary.ContainsKey(key2);
					if (flag2)
					{
						Logger.Debug("File {0} appears in package(s) {1} and {2}. Package owner should be notified.", new object[]
						{
							text,
							string.Join(";", serializableDictionary[key2]),
							keyValuePair.Key
						});
						serializableDictionary[key2].Add(key);
					}
					else
					{
						serializableDictionary.Add(key2, new HashSet<string>
						{
							keyValuePair.Key.ToLowerInvariant()
						});
					}
				}
			}
			return serializableDictionary;
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00007A1C File Offset: 0x00005C1C
		public static void LoadManifest(string manifestFile, Dictionary<string, PackageDescription> packageContents, string rootPath)
		{
			bool flag = packageContents == null;
			if (flag)
			{
				throw new ArgumentNullException("packageContents");
			}
			bool flag2 = !File.Exists(manifestFile);
			if (flag2)
			{
				throw new FileNotFoundException(manifestFile);
			}
			string fileName = Path.GetFileName(manifestFile);
			string fileNameWithoutExtension = PathHelper.GetFileNameWithoutExtension(fileName, Constants.ManifestFileExtension);
			PackageDescription packageDescription = BinaryLocator.ReadPackageDescriptionFromManifestFile(manifestFile, rootPath);
			bool flag3 = packageDescription != null;
			if (flag3)
			{
				lock (packageContents)
				{
					bool flag5 = packageContents.ContainsKey(fileNameWithoutExtension);
					if (flag5)
					{
						Logger.Debug(string.Format("Found more than one package named {0}. The package owner should be notified.", fileNameWithoutExtension), new object[0]);
					}
					else
					{
						packageContents.Add(fileNameWithoutExtension.ToLowerInvariant(), packageDescription);
					}
				}
			}
		}

		// Token: 0x06000129 RID: 297 RVA: 0x00007AE8 File Offset: 0x00005CE8
		public static PackageDescription ReadPackageDescriptionFromManifestFile(string manifestFile, string rootPath)
		{
			bool flag = string.IsNullOrEmpty(manifestFile);
			if (flag)
			{
				throw new ArgumentNullException("manifestFile");
			}
			string fileName = Path.GetFileName(manifestFile);
			string fileNameWithoutExtension = PathHelper.GetFileNameWithoutExtension(fileName, Constants.ManifestFileExtension);
			string directoryName = Path.GetDirectoryName(manifestFile);
			string path = Path.Combine(directoryName, fileNameWithoutExtension + Constants.SpkgFileExtension);
			bool flag2 = BinaryLocator.depSupress != null && BinaryLocator.depSupress.IsPackageSupressed(fileNameWithoutExtension);
			PackageDescription result;
			if (flag2)
			{
				Logger.Debug("Package {0} is suppressed, so skipped.", new object[]
				{
					fileNameWithoutExtension
				});
				result = null;
			}
			else
			{
				bool flag3 = BinaryLocator.ignoreLocaleSpecificPackages && BinaryLocator.IsLocaleSpecificPackage(directoryName);
				if (flag3)
				{
					Logger.Debug("Package {0} is a locale specific package, so skipped.", new object[]
					{
						fileNameWithoutExtension
					});
					result = null;
				}
				else
				{
					bool flag4 = BinaryLocator.ignoreResourceSpecificPackages && BinaryLocator.IsResourceSpecificPackage(directoryName);
					if (flag4)
					{
						Logger.Debug("Package {0} is a resource specific package, so skipped.", new object[]
						{
							fileNameWithoutExtension
						});
						result = null;
					}
					else
					{
						PackageDescription packageDescription = new PackageDescription
						{
							RelativePath = (string.IsNullOrEmpty(rootPath) ? string.Empty : PathHelper.ChangeParent(path, rootPath, string.Empty))
						};
						XmlDocument manifestXml = new XmlDocument();
						RetryHelper.Retry(delegate()
						{
							manifestXml.Load(manifestFile);
						}, Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs));
						XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(manifestXml.NameTable);
						xmlNamespaceManager.AddNamespace("iu", manifestXml.DocumentElement.NamespaceURI);
						XmlNodeList xmlNodeList = manifestXml.SelectNodes("//iu:Package/iu:Files/iu:FileEntry/iu:DevicePath", xmlNamespaceManager);
						foreach (object obj in xmlNodeList)
						{
							XmlNode xmlNode = (XmlNode)obj;
							string fileName2 = Path.GetFileName(xmlNode.InnerText);
							string a = Path.GetExtension(fileName2).ToLowerInvariant();
							bool flag5 = a == ".sys" || a == ".exe" || a == ".dll";
							if (flag5)
							{
								packageDescription.Binaries.Add(fileName2.ToLowerInvariant());
							}
						}
						XmlNodeList xmlNodeList2 = manifestXml.SelectNodes("//iu:Package/iu:Dependencies", xmlNamespaceManager);
						bool flag6 = xmlNodeList2.Count == 0;
						if (flag6)
						{
							Logger.Debug("Manifest File {0} does not contain dependency info. ", new object[0]);
						}
						else
						{
							xmlNodeList2 = manifestXml.SelectNodes("//iu:Package/iu:Dependencies/iu:Binary", xmlNamespaceManager);
							foreach (XmlNode xmlNode2 in xmlNodeList2.Cast<XmlNode>())
							{
								string text = xmlNode2.Attributes["Name"].Value ?? string.Empty;
								bool flag7 = !string.IsNullOrEmpty(text);
								if (flag7)
								{
									packageDescription.Dependencies.Add(new BinaryDependency
									{
										FileName = text.ToLowerInvariant()
									});
								}
							}
							xmlNodeList2 = manifestXml.SelectNodes("//iu:Package/iu:Dependencies/iu:Package", xmlNamespaceManager);
							foreach (XmlNode xmlNode3 in xmlNodeList2.Cast<XmlNode>())
							{
								string text2 = xmlNode3.Attributes["Name"].Value ?? string.Empty;
								bool flag8 = !string.IsNullOrEmpty(text2);
								if (flag8)
								{
									text2 = text2.ToLowerInvariant();
									packageDescription.Dependencies.Add(new PackageDependency
									{
										PkgName = text2,
										RelativePath = string.Empty
									});
								}
							}
							xmlNodeList2 = manifestXml.SelectNodes("//iu:Package/iu:Dependencies/iu:RemoteFile", xmlNamespaceManager);
							foreach (XmlNode xmlNode4 in xmlNodeList2.Cast<XmlNode>())
							{
								packageDescription.Dependencies.Add(new RemoteFileDependency
								{
									SourcePath = ((xmlNode4.Attributes["SourcePath"] != null) ? xmlNode4.Attributes["SourcePath"].Value.ToLowerInvariant() : string.Empty),
									Source = ((xmlNode4.Attributes["Source"] != null) ? xmlNode4.Attributes["Source"].Value.ToLowerInvariant() : string.Empty),
									DestinationPath = ((xmlNode4.Attributes["DestinationPath"] != null) ? xmlNode4.Attributes["DestinationPath"].Value.ToLowerInvariant() : string.Empty),
									Destination = ((xmlNode4.Attributes["Destination"] != null) ? xmlNode4.Attributes["Destination"].Value.ToLowerInvariant() : string.Empty),
									Tags = ((xmlNode4.Attributes["Tags"] != null) ? xmlNode4.Attributes["Tags"].Value.ToLowerInvariant() : string.Empty)
								});
							}
							xmlNodeList2 = manifestXml.SelectNodes("//iu:Package/iu:Dependencies/iu:EnvrionmentPath", xmlNamespaceManager);
							foreach (XmlNode xmlNode5 in xmlNodeList2.Cast<XmlNode>())
							{
								string text3 = xmlNode5.Attributes["Name"].Value ?? string.Empty;
								bool flag9 = !string.IsNullOrEmpty(text3);
								if (flag9)
								{
									packageDescription.Dependencies.Add(new EnvironmentPathDependency
									{
										EnvironmentPath = text3
									});
								}
							}
						}
						result = packageDescription;
					}
				}
			}
			return result;
		}

		// Token: 0x0600012A RID: 298 RVA: 0x00008184 File Offset: 0x00006384
		public PackageInfo FindContainingPackage(string binary)
		{
			bool flag = string.IsNullOrEmpty(binary);
			if (flag)
			{
				throw new ArgumentException("cannot be null or empty", "binary");
			}
			PackageInfo packageInfo = null;
			string text = binary.ToLowerInvariant();
			bool flag2 = BinaryLocator.depSupress != null && BinaryLocator.depSupress.IsFileSupressed(text);
			PackageInfo result;
			if (flag2)
			{
				result = null;
			}
			else
			{
				bool flag3 = this.packageLocator.AltRootPaths != null && this.packageLocator.AltRootPaths.Any<string>();
				if (flag3)
				{
					packageInfo = this.SearchContainingPackageInRootPathSet(text, this.packageLocator.AltRootPaths);
				}
				bool flag4 = packageInfo != null;
				if (flag4)
				{
					result = packageInfo;
				}
				else
				{
					result = this.SearchContainingPackageInRootPathSet(text, this.packageLocator.RootPaths);
				}
			}
			return result;
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0000823C File Offset: 0x0000643C
		internal static void UpdateGeneralBinaryLocatorCacheFile()
		{
			string localGeneralCacheFileFullPath = BaseLocator.GetLocalGeneralCacheFileFullPath(Constants.GeneralBinaryPackageMappingCacheFileName);
			using (ReadWriteResourceLock readWriteResourceLock = BaseLocator.CreateLockForIndexFile(localGeneralCacheFileFullPath))
			{
				readWriteResourceLock.AcquireWriteLock(TimeSpan.FromMinutes(1.0));
				BinaryLocator.generalBinaryLocationCache.SerializeToFile(localGeneralCacheFileFullPath);
			}
		}

		// Token: 0x0600012C RID: 300 RVA: 0x0000829C File Offset: 0x0000649C
		private static HashSet<string> SearchBinaryByScanningBuild(string binary, string path)
		{
			Dictionary<string, PackageDescription> packageContents = BinaryLocator.ScanBuild(path);
			SerializableDictionary<string, HashSet<string>> serializableDictionary = BinaryLocator.ComputeBinaryPackageMapping(packageContents);
			BaseLocator.WriteCacheFile(BaseLocator.GetLocationCacheFilePath(path, Constants.BinaryLocationCacheExtension), serializableDictionary);
			return serializableDictionary.ContainsKey(binary) ? serializableDictionary[binary] : null;
		}

		// Token: 0x0600012D RID: 301 RVA: 0x000082E4 File Offset: 0x000064E4
		private static bool IsLocaleSpecificPackage(string path)
		{
			return path.IndexOf("_Lang_", StringComparison.OrdinalIgnoreCase) != -1;
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00008308 File Offset: 0x00006508
		private static bool IsResourceSpecificPackage(string path)
		{
			return path.IndexOf("_RES_", StringComparison.OrdinalIgnoreCase) != -1;
		}

		// Token: 0x0600012F RID: 303 RVA: 0x0000832C File Offset: 0x0000652C
		private static bool PackageContainsBinary(string binaryName, string packageAbsolutPath)
		{
			bool flag = string.IsNullOrWhiteSpace(binaryName);
			if (flag)
			{
				throw new ArgumentException("cannot be null or empty.", "binaryName");
			}
			bool flag2 = string.IsNullOrWhiteSpace(packageAbsolutPath);
			if (flag2)
			{
				throw new ArgumentException("cannot be null or empty.", "packageAbsolutPath");
			}
			bool flag3 = string.Compare(Path.GetExtension(packageAbsolutPath), Constants.SpkgFileExtension, true) != 0 && string.Compare(Path.GetExtension(packageAbsolutPath), Constants.CabFileExtension, true) != 0;
			if (flag3)
			{
				throw new InvalidDataException(string.Format("{0} is not a spkg or cab file.", packageAbsolutPath));
			}
			bool flag4 = !ReliableFile.Exists(packageAbsolutPath, Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs));
			if (flag4)
			{
				throw new FileNotFoundException(packageAbsolutPath);
			}
			string text = Path.ChangeExtension(packageAbsolutPath, Constants.ManifestFileExtension);
			bool flag5 = !ReliableFile.Exists(text, Settings.Default.ShareAccessRetryCount, TimeSpan.FromMilliseconds((double)Settings.Default.ShareAccessRetryDelayInMs));
			if (flag5)
			{
				throw new FileNotFoundException(text);
			}
			PackageDescription packageDescription = BinaryLocator.ReadPackageDescriptionFromManifestFile(text, string.Empty);
			return packageDescription.Binaries.Any((string x) => string.Compare(x, binaryName, true) == 0);
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00008460 File Offset: 0x00006660
		private PackageInfo SearchContainingPackageInRootPathSet(string binaryName, IEnumerable<string> rootPathSet)
		{
			bool flag = BinaryLocator.generalBinaryLocationCache != null && BinaryLocator.generalBinaryLocationCache.ContainsKey(binaryName);
			if (flag)
			{
				foreach (string package in BinaryLocator.generalBinaryLocationCache[binaryName])
				{
					PackageInfo packageInfo = this.packageLocator.FindPackage(package, rootPathSet);
					bool flag2 = packageInfo != null && BinaryLocator.PackageContainsBinary(binaryName, packageInfo.AbsolutePath);
					if (flag2)
					{
						return packageInfo;
					}
				}
			}
			foreach (string text in rootPathSet)
			{
				HashSet<string> hashSet = this.SearchContainingPackageInRootPath(binaryName, text);
				bool flag3 = hashSet != null && hashSet.Any<string>();
				if (flag3)
				{
					foreach (string package2 in hashSet)
					{
						PackageInfo packageInfo = this.packageLocator.FindPackage(package2, text);
						bool flag4 = packageInfo != null && BinaryLocator.PackageContainsBinary(binaryName, packageInfo.AbsolutePath);
						if (flag4)
						{
							return packageInfo;
						}
					}
				}
			}
			Logger.Warning("Did not find binary {0} in any package. Moving on.", new object[]
			{
				binaryName
			});
			return null;
		}

		// Token: 0x06000131 RID: 305 RVA: 0x000085E8 File Offset: 0x000067E8
		private HashSet<string> SearchContainingPackageInRootPath(string binaryName, string rootPath)
		{
			bool flag = string.IsNullOrWhiteSpace(rootPath);
			if (flag)
			{
				throw new ArgumentException("Cannot be null or empty", "rootPath");
			}
			bool flag2 = !Directory.Exists(rootPath);
			if (flag2)
			{
				throw new DirectoryNotFoundException(rootPath);
			}
			string locationCacheFilePath = BaseLocator.GetLocationCacheFilePath(rootPath, Constants.BinaryLocationCacheExtension);
			bool flag3 = File.Exists(locationCacheFilePath);
			HashSet<string> result;
			if (flag3)
			{
				SerializableDictionary<string, HashSet<string>> serializableDictionary = BaseLocator.ReadCacheFile(locationCacheFilePath);
				result = (serializableDictionary.ContainsKey(binaryName) ? serializableDictionary[binaryName] : null);
			}
			else
			{
				result = BinaryLocator.SearchBinaryByScanningBuild(binaryName, rootPath);
			}
			return result;
		}

		// Token: 0x04000072 RID: 114
		private static bool ignoreResourceSpecificPackages = true;

		// Token: 0x04000073 RID: 115
		private static bool ignoreLocaleSpecificPackages = true;

		// Token: 0x04000074 RID: 116
		private static DependencySuppression depSupress = null;

		// Token: 0x04000075 RID: 117
		private static SerializableDictionary<string, HashSet<string>> generalBinaryLocationCache;

		// Token: 0x04000076 RID: 118
		private PackageLocator packageLocator;
	}
}
