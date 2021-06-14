using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Composition.Packaging;
using Microsoft.Composition.PkgCondenser;
using Microsoft.Composition.ToolBox;
using Microsoft.Composition.ToolBox.IO;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.FeatureMerger
{
	// Token: 0x02000003 RID: 3
	public class Merger
	{
		// Token: 0x0600001C RID: 28 RVA: 0x00004B68 File Offset: 0x00002D68
		public static void SetCBSFeatureInfo(string pkgPath, string featureId, string fmID, string pkgGroup)
		{
			if (Path.GetExtension(pkgPath).Equals(Microsoft.Composition.ToolBox.PkgConstants.SPKGPackageExtension, StringComparison.CurrentCultureIgnoreCase))
			{
				if (!File.Exists(Path.ChangeExtension(pkgPath, Microsoft.Composition.ToolBox.PkgConstants.CBSPackageExtension)))
				{
					Merger.GenerateCBSPackage(pkgPath);
				}
			}
			else if (!File.Exists(pkgPath))
			{
				Merger.GenerateCBSPackage(Path.ChangeExtension(pkgPath, Microsoft.Composition.ToolBox.PkgConstants.SPKGPackageExtension));
			}
			pkgPath = Path.ChangeExtension(pkgPath, Microsoft.Composition.ToolBox.PkgConstants.CBSPackageExtension);
			CbsPackage cbsPackage = new CbsPackage(pkgPath);
			cbsPackage.SetCBSFeatureInfo(fmID, featureId, pkgGroup, new List<string>());
			cbsPackage.SaveCab(pkgPath);
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00004BE4 File Offset: 0x00002DE4
		public static MergeResult[] MergePackages(List<string> fmPackageList, string outputPackageDir, string featureId, string fmID, Version version, string owner, OwnerType ownerType, ReleaseType releaseType, CpuId cpuId, bool incremental, IULogger logger)
		{
			List<string> list = new List<string>();
			HashSet<string> hashSet = new HashSet<string>();
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			string fipPkg = string.Empty;
			Dictionary<string, HashSet<string>> dictionary2 = new Dictionary<string, HashSet<string>>(StringComparer.InvariantCultureIgnoreCase);
			Dictionary<string, HashSet<string>> dictionary3 = new Dictionary<string, HashSet<string>>(StringComparer.InvariantCultureIgnoreCase);
			Dictionary<string, List<string>> mergedPackageList = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);
			List<MergeResult> list2 = new List<MergeResult>();
			string featureKey = featureId;
			if (!string.IsNullOrEmpty(fmID))
			{
				featureKey = featureKey + "." + fmID;
			}
			foreach (string text in fmPackageList)
			{
				if (Path.GetExtension(text).Equals(Microsoft.Composition.ToolBox.PkgConstants.SPKGPackageExtension, StringComparison.CurrentCultureIgnoreCase))
				{
					if (!File.Exists(Path.ChangeExtension(text, Microsoft.Composition.ToolBox.PkgConstants.CBSPackageExtension)))
					{
						Merger.GenerateCBSPackage(text);
					}
				}
				else if (!File.Exists(text))
				{
					Merger.GenerateCBSPackage(Path.ChangeExtension(text, Microsoft.Composition.ToolBox.PkgConstants.SPKGPackageExtension));
				}
				list.Add(Path.ChangeExtension(text, Microsoft.Composition.ToolBox.PkgConstants.CBSPackageExtension));
			}
			if (string.IsNullOrEmpty(owner))
			{
				owner = Merger.GenerateOwnerName(list);
			}
			string key = Merger.BuildPackageName(featureKey, owner, ManifestToolBox.ConvertReleaseType(releaseType.ToString()), "MainOS");
			string text2 = Merger.GenerateEmptyMainOSPkg(list[0], outputPackageDir, featureKey, version, owner, ownerType, releaseType);
			if (!string.IsNullOrEmpty(text2))
			{
				list.Insert(0, text2);
			}
			foreach (string text3 in list)
			{
				string text4 = string.Empty;
				CbsPackageInfo cbsPackageInfo = new CbsPackageInfo(text3);
				if (!Merger.IsSpecialPkg(cbsPackageInfo))
				{
					text4 = Merger.BuildPackageName(featureKey, owner, ManifestToolBox.ConvertReleaseType(releaseType.ToString()), cbsPackageInfo);
					if (!mergedPackageList.ContainsKey(text4))
					{
						mergedPackageList.Add(text4, new List<string>());
					}
					mergedPackageList[text4].Add(text3);
					string[] array = Regex.Split(text4, "_LANG_", RegexOptions.IgnoreCase);
					string[] array2 = Regex.Split(text4, "_RES_", RegexOptions.IgnoreCase);
					string text5 = array[0];
					if (array2.Length > 1 && !string.IsNullOrEmpty(array2[1]))
					{
						text5 = array2[0];
					}
					if (string.IsNullOrEmpty(fipPkg) && cbsPackageInfo.Partition.Equals("MainOS", StringComparison.CurrentCultureIgnoreCase))
					{
						fipPkg = text5;
					}
					if (!fipPkg.Equals(text4, StringComparison.InvariantCultureIgnoreCase))
					{
						hashSet.Add(Path.Combine(outputPackageDir, string.Format("{0}{1}", Merger.BuildCABName(text4), Microsoft.Composition.ToolBox.PkgConstants.CBSPackageExtension)));
					}
					string value = Path.Combine(outputPackageDir, string.Format("{0}{1}", Merger.BuildCABName(text5), Microsoft.Composition.ToolBox.PkgConstants.CBSPackageExtension));
					if (!dictionary.ContainsKey(text5))
					{
						dictionary[text5] = value;
						dictionary2[text5] = new HashSet<string>();
						dictionary3[text5] = new HashSet<string>();
					}
					if (array.Length > 1 && !string.IsNullOrEmpty(array[1]))
					{
						dictionary2[text5].Add(array[1]);
					}
					if (array2.Length > 1 && !string.IsNullOrEmpty(array2[1]))
					{
						dictionary3[text5].Add(array2[1]);
					}
				}
				else
				{
					logger.LogInfo("Special package: " + cbsPackageInfo.PackageName, new object[0]);
					MergeResult mergeResult = new MergeResult();
					mergeResult.PkgInfo = Package.LoadFromCab(text3);
					mergeResult.FilePath = text3;
					mergeResult.IsNonMergedPackage = true;
					hashSet.Add(text3);
					list2.Add(mergeResult);
				}
			}
			if (mergedPackageList[key].Count > 1)
			{
				mergedPackageList[key].Remove(text2);
			}
			CbsPackage fipPackage = null;
			Parallel.ForEach<string>(mergedPackageList.Keys, delegate(string pkgName)
			{
				if (!incremental || !Merger.IsTargetUpToDate(featureKey, pkgName, mergedPackageList[pkgName], outputPackageDir))
				{
					CbsPackage cbsPackage = new CbsPackage(mergedPackageList[pkgName].First<string>());
					cbsPackage.PackageName = pkgName;
					Merger.SetPhoneInformation(featureKey, owner, ownerType, releaseType, cbsPackage);
					cbsPackage.Version = new Version(version.ToString());
					foreach (string sourcePath in mergedPackageList[pkgName].Skip(1))
					{
						CbsPackage cbsPackage2 = new CbsPackage(sourcePath);
						cbsPackage.AddFile(cbsPackage2.GetRoot());
					}
					PkgCondenser.CondensePackage(cbsPackage);
					if (cbsPackage.PackageName.Equals("Microsoft.MainOS.Production", StringComparison.InvariantCultureIgnoreCase))
					{
						cbsPackage.ReleaseType = "Product";
					}
					if (!cbsPackage.PackageName.Equals(fipPkg, StringComparison.InvariantCultureIgnoreCase))
					{
						string cabPath = Path.Combine(outputPackageDir, string.Format("{0}{1}", Merger.BuildCABName(cbsPackage.PackageName), Microsoft.Composition.ToolBox.PkgConstants.CBSPackageExtension));
						cbsPackage.SaveCab(cabPath);
					}
					else
					{
						fipPackage = cbsPackage;
						cbsPackage.GetRoot().SaveManifest(DirectoryToolBox.GetDirectoryFromFilePath(cbsPackage.GetRoot().SourcePath));
					}
					Merger.WriteMergePackageList(pkgName, mergedPackageList[pkgName], outputPackageDir);
					return;
				}
				logger.LogInfo("Skipping package '{0}' because is has not changed", new object[]
				{
					outputPackageDir + pkgName
				});
			});
			if (fipPackage != null)
			{
				fipPackage.SetCBSFeatureInfo(fmID, featureId, string.Empty, hashSet.ToList<string>());
				fipPackage.SaveCab(Path.Combine(outputPackageDir, string.Format("{0}{1}", Merger.BuildCABName(fipPackage.PackageName), Microsoft.Composition.ToolBox.PkgConstants.CBSPackageExtension)));
			}
			List<MergeResult> list3 = new List<MergeResult>();
			foreach (string text6 in dictionary.Keys)
			{
				MergeResult mergeResult2 = new MergeResult();
				mergeResult2.PkgInfo = Package.LoadFromCab(dictionary[text6]);
				mergeResult2.FilePath = dictionary[text6];
				mergeResult2.Languages = dictionary2[text6].ToArray<string>();
				mergeResult2.Resolutions = dictionary3[text6].ToArray<string>();
				if (fipPkg.Equals(text6))
				{
					mergeResult2.FeatureIdentifierPackage = Microsoft.Composition.ToolBox.PkgConstants.MainOsPartition.Equals(mergeResult2.PkgInfo.Partition, StringComparison.CurrentCultureIgnoreCase);
				}
				list3.Add(mergeResult2);
			}
			list3.AddRange(list2);
			return list3.ToArray();
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000051C4 File Offset: 0x000033C4
		public static void GenerateCBSPackage(string packageFile)
		{
			Merger.GenerateCBSPackages(new List<string>
			{
				packageFile
			});
		}

		// Token: 0x0600001F RID: 31 RVA: 0x000051D8 File Offset: 0x000033D8
		public static void GenerateCBSPackages(List<string> packageList)
		{
			if (packageList.Count<string>() == 0)
			{
				return;
			}
			string tempDirectory = FileUtils.GetTempDirectory();
			bool overrideExisting = true;
			foreach (string text in packageList)
			{
				PkgConvertDSM.ConvertPackagesToCBS(new List<string>
				{
					text
				}, tempDirectory);
				string path = Path.GetFileNameWithoutExtension(text) + Microsoft.Composition.ToolBox.PkgConstants.CBSPackageExtension;
				string source = Path.Combine(tempDirectory, path);
				string dest = Path.Combine(Path.GetDirectoryName(text), path);
				FileToolBox.Copy(source, dest, overrideExisting);
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00005274 File Offset: 0x00003474
		private static string GenerateOwnerName(List<string> packageList)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
			foreach (string sourcePath in packageList)
			{
				CbsPackageInfo cbsPackageInfo = new CbsPackageInfo(sourcePath);
				hashSet.Add(cbsPackageInfo.Owner);
			}
			if (hashSet.Count > 1)
			{
				throw new InvalidDataException("Packages having multiple 'OwnerType' values are not allowed to be merged");
			}
			return hashSet.ElementAt(0);
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000052F4 File Offset: 0x000034F4
		private static string GenerateEmptyMainOSPkg(string templatePkgPath, string outputPackageDir, string featureKey, Version version, string owner, OwnerType ownerType, ReleaseType releaseType)
		{
			CbsPackageInfo cbsPackageInfo = new CbsPackageInfo(templatePkgPath);
			Keyform keyform = new Keyform(cbsPackageInfo.PackageName, cbsPackageInfo.HostArch, cbsPackageInfo.GuestArch, cbsPackageInfo.BuildType, cbsPackageInfo.ReleaseType, cbsPackageInfo.Culture, "nonSxS", cbsPackageInfo.Version, cbsPackageInfo.PublicKey);
			string text = Merger.BuildPackageName(featureKey, owner, (PhoneReleaseType)releaseType, "MainOS");
			string text2 = Path.Combine(outputPackageDir, string.Format("{0}{1}", Merger.BuildCABName(text), Microsoft.Composition.ToolBox.PkgConstants.CBSPackageExtension));
			if (!FileToolBox.Exists(text2))
			{
				string text3 = Path.Combine(PathToolBox.GetTemporaryPath(), string.Format("{0}{1}", Merger.BuildCABName(text), Microsoft.Composition.ToolBox.PkgConstants.CBSPackageExtension));
				Keyform keyform2 = keyform;
				keyform2.Name = text;
				CbsPackage cbsPackage = new CbsPackage(keyform2);
				cbsPackage.Partition = "MainOS";
				Merger.SetPhoneInformation(featureKey, owner, ownerType, releaseType, cbsPackage);
				cbsPackage.Version = new Version(version.ToString());
				cbsPackage.SaveCab(text3);
				text2 = text3;
			}
			return text2;
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000053E0 File Offset: 0x000035E0
		private static bool IsTargetUpToDate(string featureKey, string inputPkgName, List<string> inputPackagePaths, string outputPackageDir)
		{
			string text = PathToolBox.Combine(outputPackageDir, inputPkgName + Microsoft.Composition.ToolBox.PkgConstants.CBSPackageExtension);
			if (!FileToolBox.Exists(text))
			{
				return false;
			}
			DateTime lastWriteTimeUtc = new FileInfo(text).LastWriteTimeUtc;
			foreach (string text2 in inputPackagePaths)
			{
				if (!text.Equals(text2, StringComparison.InvariantCultureIgnoreCase) && new FileInfo(text2).LastWriteTimeUtc > lastWriteTimeUtc)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00005478 File Offset: 0x00003678
		private static void WriteMergePackageList(string pkgName, List<string> mergePackageList, string outputDirectory)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string value in mergePackageList)
			{
				stringBuilder.AppendLine(value);
			}
			File.WriteAllText(Path.Combine(outputDirectory, pkgName + ".merged.txt"), stringBuilder.ToString());
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000054EC File Offset: 0x000036EC
		private static string GetResolution(string pkgName)
		{
			int num = pkgName.IndexOf("_RES_");
			if (num < 0)
			{
				return string.Empty;
			}
			return pkgName.Substring(num + "_RES_".Count<char>());
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00005524 File Offset: 0x00003724
		private static bool IsSpecialPkg(CbsPackageInfo package)
		{
			if (package.BinaryPartition.GetValueOrDefault())
			{
				return true;
			}
			if (package.OwnerType == PhoneOwnerType.Microsoft)
			{
				if ("MobileCore".Equals(package.GroupingKey, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				if ("MobileCoreMFGOS".Equals(package.GroupingKey, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				if ("FactoryOS".Equals(package.GroupingKey, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				if ("Andromeda".Equals(package.GroupingKey, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				if ("OneCore".Equals(package.GroupingKey, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				if ("IoTUAP".Equals(package.GroupingKey, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				if (Merger.specialPackages.Contains(package.PackageName, StringComparer.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return (package.OwnerType != PhoneOwnerType.Microsoft && "RegistryCustomization".Equals(package.SubComponent, StringComparison.OrdinalIgnoreCase)) || (package.OwnerType != PhoneOwnerType.Microsoft && package.SubComponent.StartsWith("Customizations.", StringComparison.OrdinalIgnoreCase) && package.SubComponent.EndsWith("." + package.Partition, StringComparison.OrdinalIgnoreCase)) || (package.Partition.Equals(Microsoft.Composition.ToolBox.PkgConstants.UpdateOsPartition, StringComparison.OrdinalIgnoreCase) && package.OwnerType != PhoneOwnerType.Microsoft);
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00005660 File Offset: 0x00003860
		private static string BuildPackageName(string featureKey, string owner, PhoneReleaseType releaseType, string partition)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string arg;
			string text;
			if (releaseType == PhoneReleaseType.Production && string.Equals(featureKey, "BASE", StringComparison.OrdinalIgnoreCase))
			{
				arg = partition;
				text = releaseType.ToString();
			}
			else
			{
				arg = featureKey;
				text = partition;
			}
			stringBuilder.AppendFormat("{0}.{1}", owner, arg);
			if (!string.IsNullOrEmpty(text))
			{
				stringBuilder.AppendFormat(".{0}", text);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000027 RID: 39 RVA: 0x000056C4 File Offset: 0x000038C4
		private static string BuildPackageName(string featureKey, string owner, PhoneReleaseType releaseType, CbsPackageInfo pkgInfo)
		{
			StringBuilder stringBuilder = new StringBuilder(Merger.BuildPackageName(featureKey, owner, releaseType, pkgInfo.Partition));
			string resolution = Merger.GetResolution(pkgInfo.PackageName);
			string culture = pkgInfo.Culture;
			if (pkgInfo.IsWow)
			{
				stringBuilder.Append(".Guest");
			}
			if (!string.IsNullOrEmpty(culture) && !culture.Equals("neutral", StringComparison.InvariantCultureIgnoreCase))
			{
				stringBuilder.AppendFormat("_Lang_{0}", culture);
			}
			if (!string.IsNullOrEmpty(resolution))
			{
				stringBuilder.AppendFormat("_RES_{0}", resolution);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000028 RID: 40 RVA: 0x0000574C File Offset: 0x0000394C
		private static void SetPhoneInformation(string featureKey, string owner, OwnerType ownerType, ReleaseType releaseType, CbsPackage package)
		{
			string partition = package.Partition;
			string component;
			string subComponent;
			if (releaseType == ReleaseType.Production && string.Equals(featureKey, "BASE", StringComparison.OrdinalIgnoreCase))
			{
				component = partition;
				subComponent = releaseType.ToString();
			}
			else
			{
				component = featureKey;
				subComponent = partition;
			}
			package.PhoneReleaseType = (PhoneReleaseType)Enum.Parse(typeof(PhoneReleaseType), releaseType.ToString(), true);
			package.OwnerType = (PhoneOwnerType)Enum.Parse(typeof(PhoneOwnerType), ownerType.ToString(), true);
			package.Owner = owner;
			package.Component = component;
			package.SubComponent = subComponent;
			package.GroupingKey = featureKey;
			if (package.BinaryPartition == null)
			{
				package.BinaryPartition = new bool?(false);
			}
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00005819 File Offset: 0x00003A19
		private static string BuildCABName(string packageName)
		{
			return packageName.Replace("_RES_", "_Res_");
		}

		// Token: 0x04000027 RID: 39
		private static string[] specialPackages = new string[]
		{
			"Microsoft.BCD.bootlog.winload",
			"Microsoft.BCD.bootlog.bootmgr",
			"Microsoft.Net.FakeModem",
			"Microsoft.Net.FakeWwan"
		};
	}
}
