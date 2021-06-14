using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.Tools;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x0200002C RID: 44
	public static class Package
	{
		// Token: 0x060001DF RID: 479 RVA: 0x000079F7 File Offset: 0x00005BF7
		public static IPkgBuilder Create()
		{
			return new PkgBuilder();
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x00007A00 File Offset: 0x00005C00
		public static Hashtable LoadRegistry(string cabPath)
		{
			Hashtable registryKeysMerged = new Hashtable();
			string tempDirectory = FileUtils.GetTempDirectory();
			IPkgInfo pkgInfo = Package.LoadFromCab(cabPath);
			ArrayList arrayList = new ArrayList();
			foreach (IFileEntry fileEntry in pkgInfo.Files)
			{
				if (fileEntry.CabPath.EndsWith(".manifest", StringComparison.CurrentCultureIgnoreCase) && !fileEntry.CabPath.Contains("deployment"))
				{
					arrayList.Add(fileEntry.CabPath);
				}
			}
			if (arrayList.Count == 0)
			{
				return registryKeysMerged;
			}
			try
			{
				CabApiWrapper.ExtractSelected(cabPath, tempDirectory, arrayList.OfType<string>());
				object reg_lock = new object();
				ArrayList arrayList2 = new ArrayList();
				ParallelOptions parallelOptions = new ParallelOptions();
				int num = 0;
				string[] files = Directory.GetFiles(tempDirectory);
				parallelOptions.MaxDegreeOfParallelism = PkgConstants.c_iMaxPackagingThreads;
				if (files.Count<string>() < PkgConstants.c_iMaxPackagingThreads)
				{
					parallelOptions.MaxDegreeOfParallelism = files.Count<string>();
				}
				string[] array;
				for (int i = 0; i < parallelOptions.MaxDegreeOfParallelism - 1; i++)
				{
					array = new string[files.Count<string>() / parallelOptions.MaxDegreeOfParallelism];
					Array.Copy(files, num, array, 0, files.Count<string>() / parallelOptions.MaxDegreeOfParallelism);
					num += files.Count<string>() / parallelOptions.MaxDegreeOfParallelism;
					arrayList2.Add(array);
				}
				array = new string[files.Count<string>() - num];
				Array.Copy(files, num, array, 0, files.Count<string>() - num);
				arrayList2.Add(array);
				Parallel.ForEach<string[]>(arrayList2.OfType<string[]>(), delegate(string[] file_range)
				{
					Hashtable hashtable = new Hashtable();
					for (int j = 0; j < file_range.Length; j++)
					{
						XDocument xdocument = XDocument.Load(file_range[j]);
						XNamespace @namespace = xdocument.Root.Name.Namespace;
						foreach (XElement xelement in xdocument.Root.Elements(@namespace + "registryKeys"))
						{
							if (xelement != null)
							{
								foreach (XElement xelement2 in xelement.Elements(@namespace + "registryKey"))
								{
									Hashtable hashtable2 = new Hashtable();
									foreach (XElement xelement3 in xelement2.Elements(@namespace + "registryValue"))
									{
										if (xelement3.Attribute("name") != null)
										{
											string value = null;
											if (xelement3.Attribute("value") != null)
											{
												value = xelement3.Attribute("value").Value;
											}
											hashtable2.Add(xelement3.Attribute("name").Value, value);
										}
									}
									if (!hashtable.ContainsKey(xelement2.Attribute("keyName").Value))
									{
										hashtable.Add(xelement2.Attribute("keyName").Value, hashtable2);
									}
									else
									{
										foreach (object obj in hashtable2)
										{
											DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
											((Hashtable)hashtable[xelement2.Attribute("keyName").Value])[dictionaryEntry.Key] = dictionaryEntry.Value;
										}
									}
								}
							}
						}
					}
					if (hashtable.Count > 0)
					{
						object reg_lock = reg_lock;
						lock (reg_lock)
						{
							foreach (object obj2 in hashtable)
							{
								DictionaryEntry dictionaryEntry2 = (DictionaryEntry)obj2;
								registryKeysMerged[dictionaryEntry2.Key] = dictionaryEntry2.Value;
							}
						}
					}
				});
			}
			finally
			{
				FileUtils.CleanDirectory(tempDirectory);
			}
			return registryKeysMerged;
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x00007BFC File Offset: 0x00005DFC
		public static bool RegistryKeyExist(Hashtable regTable, string keyName)
		{
			return Package.RegistryKeyExist(regTable, keyName, null);
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x00007C06 File Offset: 0x00005E06
		public static bool RegistryKeyExist(Hashtable regTable, string keyName, string valueName)
		{
			return regTable.ContainsKey(keyName) && (string.IsNullOrEmpty(valueName) || ((Hashtable)regTable[keyName]).ContainsKey(valueName));
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x00007C32 File Offset: 0x00005E32
		public static string RegistryKeyValue(Hashtable regTable, string keyName, string valueName)
		{
			if (!regTable.ContainsKey(keyName))
			{
				return null;
			}
			if (!((Hashtable)regTable[keyName]).ContainsKey(valueName))
			{
				return null;
			}
			return (string)((Hashtable)regTable[keyName])[valueName];
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x00007C6C File Offset: 0x00005E6C
		public static IPkgBuilder Create(string cabPath)
		{
			if (string.IsNullOrEmpty(cabPath))
			{
				throw new ArgumentException("Create: cab path can not be null or empty", "cabPath");
			}
			if (!LongPathFile.Exists(cabPath))
			{
				throw new PackageException("Create: cab file '{0}' doesn't exist", new object[]
				{
					cabPath
				});
			}
			return PkgBuilder.Load(cabPath);
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x00007CA9 File Offset: 0x00005EA9
		public static IPkgInfo LoadFromFolder(string folderPath)
		{
			if (string.IsNullOrEmpty(folderPath))
			{
				throw new ArgumentException("LoadFromFolder: cab path can not be null or empty", "folderPath");
			}
			if (!Directory.Exists(folderPath))
			{
				throw new PackageException("LoadFromFolder: folderPath '{0}' doesn't exist", new object[]
				{
					folderPath
				});
			}
			return WPExtractedPackage.Load(folderPath);
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x00007CE8 File Offset: 0x00005EE8
		public static IPkgInfo LoadFromCab(string cabPath)
		{
			if (string.IsNullOrEmpty(cabPath))
			{
				throw new ArgumentException("LoadFromCab: cab path can not be null or empty", "cabPath");
			}
			if (!LongPathFile.Exists(cabPath))
			{
				throw new PackageException("LoadFromCab: Cab file '{0}' doesn't exist", new object[]
				{
					cabPath
				});
			}
			string[] source = null;
			try
			{
				source = CabApiWrapper.GetFileList(cabPath);
			}
			catch (CabException innerException)
			{
				throw new PackageException(innerException, "LoadFromCab: Failed to load cab file '{0}'", new object[]
				{
					cabPath
				});
			}
			catch (IOException innerException2)
			{
				throw new PackageException(innerException2, "LoadFromCab: Failed to load cab file '{0}'", new object[]
				{
					cabPath
				});
			}
			if (!source.Contains(PkgConstants.c_strDsmFile, StringComparer.OrdinalIgnoreCase) && !source.Contains(PkgConstants.c_strMumFile, StringComparer.OrdinalIgnoreCase) && !source.Contains(PkgConstants.c_strCIX, StringComparer.OrdinalIgnoreCase))
			{
				throw new PackageException("LoadFromCab: No package manifest found in cab file '{0}'", new object[]
				{
					cabPath
				});
			}
			if (source.Contains(PkgConstants.c_strDiffDsmFile, StringComparer.OrdinalIgnoreCase) || source.Contains(PkgConstants.c_strCIX, StringComparer.OrdinalIgnoreCase))
			{
				return DiffPkg.LoadFromCab(cabPath);
			}
			return WPCanonicalPackage.LoadFromCab(cabPath);
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x00007DF8 File Offset: 0x00005FF8
		public static IPkgInfo LoadInstalledPackage(string manifestPath, string installationDir)
		{
			if (string.IsNullOrEmpty(manifestPath))
			{
				throw new ArgumentException("LoadInstalledPackage: Package manifest path can not be null or empty", "manifestPath");
			}
			if (string.IsNullOrEmpty(installationDir))
			{
				throw new ArgumentException("LoadInstalledPackage: Package root directory can not be null or empty", "installationDir");
			}
			if (!LongPathFile.Exists(manifestPath))
			{
				throw new PackageException("LoadInstalledPackage: Package manifest file '{0}' doesn't exist", new object[]
				{
					manifestPath
				});
			}
			if (!LongPathDirectory.Exists(installationDir))
			{
				throw new PackageException("LoadInstalledPackage: Package root directory '{0}' doesn't exist", new object[]
				{
					installationDir
				});
			}
			return WPCanonicalPackage.LoadFromInstallationDir(manifestPath, installationDir);
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x00007E78 File Offset: 0x00006078
		public static void CreatePKR(string sourceCab, string outputCab)
		{
			if (string.IsNullOrEmpty(sourceCab))
			{
				throw new ArgumentException("CreatePKR: path of source package can't be null or empty", "sourceCab");
			}
			if (string.IsNullOrEmpty(outputCab))
			{
				throw new ArgumentException("CreatePKR: path of the output package can't be null or empty", "outputCab");
			}
			if (!LongPathFile.Exists(sourceCab))
			{
				throw new PackageException("CreatePKR: source package '{0}' doesn't exist", new object[]
				{
					sourceCab
				});
			}
			PKRBuilder.Create(sourceCab, outputCab);
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x00007ED9 File Offset: 0x000060D9
		public static IDiffPkg CreateDiff(IPkgInfo source, IPkgInfo target, DiffOptions diffOptions, string outputDir)
		{
			throw new NotImplementedException("CreateDiff with IPkgInfo instances is deprecated");
		}

		// Token: 0x060001EA RID: 490 RVA: 0x00007EE5 File Offset: 0x000060E5
		public static DiffError CreateDiff(string sourceCab, string targetCab, DiffOptions diffOptions, string outputCab)
		{
			return Package.CreateDiff(sourceCab, targetCab, diffOptions, null, outputCab);
		}

		// Token: 0x060001EB RID: 491 RVA: 0x00007EF4 File Offset: 0x000060F4
		public static DiffError CreateDiff(string sourceCab, string targetCab, DiffOptions diffOptions, Dictionary<DiffOptions, object> diffOptionValues, string outputCab)
		{
			if (string.IsNullOrEmpty(sourceCab))
			{
				throw new ArgumentException("CreateDiff: path of source package can't be null or empty", "sourceCab");
			}
			if (string.IsNullOrEmpty(targetCab))
			{
				throw new ArgumentException("CreateDiff: path of target package can't be null or empty", "targetCab");
			}
			if (string.IsNullOrEmpty(outputCab))
			{
				throw new ArgumentException("CreateDiff: path of the output package can't be null or empty", "outputCab");
			}
			if (!LongPathFile.Exists(sourceCab))
			{
				throw new PackageException("CreateDiff: source package '{0}' doesn't exist", new object[]
				{
					sourceCab
				});
			}
			if (!LongPathFile.Exists(targetCab))
			{
				throw new PackageException("CreateDiff: target package '{0}' doesn't exist", new object[]
				{
					targetCab
				});
			}
			return DiffPkgBuilder.CreateDiff(sourceCab, targetCab, diffOptions, diffOptionValues, outputCab);
		}

		// Token: 0x060001EC RID: 492 RVA: 0x00007F90 File Offset: 0x00006190
		public static void MergePackage(string[] inputPkgs, string outputDir, VersionInfo? version, ReleaseType releaseType, CpuId cpuType, BuildType buildType, bool compress)
		{
			if (inputPkgs == null)
			{
				throw new ArgumentNullException("inputPkgs", "MergePackage: inputPkgs can not be null");
			}
			if (string.IsNullOrEmpty(outputDir))
			{
				throw new ArgumentException("MergePackage: outputDir can not be null or empty", "outputDir");
			}
			for (int i = 0; i < inputPkgs.Length; i++)
			{
				if (string.IsNullOrEmpty(inputPkgs[i]))
				{
					throw new ArgumentException("MergePackage: inputPkgs can't contain null or empty paths", "inputPkgs");
				}
			}
			PkgMerger.Merge(inputPkgs, version, releaseType, cpuType, buildType, outputDir, compress, Package.Logger);
		}

		// Token: 0x060001ED RID: 493 RVA: 0x00008008 File Offset: 0x00006208
		public static MergeResult[] MergePackage(string[] inputPkgs, string outputDir, string featureKey, VersionInfo version, string ownerOverride, OwnerType ownerTypeOverride, ReleaseType releaseType, CpuId cpuType, BuildType buildType, bool compress, bool incremental)
		{
			if (inputPkgs == null)
			{
				throw new ArgumentNullException("inputPkgs", "MergePackage: inputPkgs can not be null");
			}
			if (string.IsNullOrEmpty(outputDir))
			{
				throw new ArgumentException("MergePackage: outputDir can not be null or empty", "outputDir");
			}
			if (string.IsNullOrEmpty(featureKey))
			{
				throw new ArgumentException("MergePackage: featureKey can not be null or empty", "featureKey");
			}
			if (!string.IsNullOrEmpty(ownerOverride) && ownerTypeOverride == OwnerType.Invalid)
			{
				throw new ArgumentException("MergePackage: OwnerType override can not be invalid", "ownerTypeOverride");
			}
			for (int i = 0; i < inputPkgs.Length; i++)
			{
				if (string.IsNullOrEmpty(inputPkgs[i]))
				{
					throw new ArgumentException("MergePackage: inputPkgs can't contain null or empty paths", "inputPkgs");
				}
			}
			return FBMerger.Merge(inputPkgs, featureKey, version, ownerOverride, ownerTypeOverride, releaseType, cpuType, buildType, outputDir, compress, incremental);
		}

		// Token: 0x0400008A RID: 138
		public static CompressionType DefaultCompressionType = CompressionType.FastLZX;

		// Token: 0x0400008B RID: 139
		public static IDeploymentLogger Logger = new IULogger();
	}
}
