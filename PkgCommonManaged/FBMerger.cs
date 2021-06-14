using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000035 RID: 53
	public class FBMerger
	{
		// Token: 0x06000236 RID: 566 RVA: 0x00009835 File Offset: 0x00007A35
		public FBMerger()
		{
		}

		// Token: 0x06000237 RID: 567 RVA: 0x00009858 File Offset: 0x00007A58
		private MergeGroup NewGroup(string partition, bool isFeatureIdentifierGroup = false)
		{
			MergeGroup mergeGroup = new MergeGroup();
			if (this._ownerTypeOverride == OwnerType.Microsoft && this._expectedRelease == ReleaseType.Production && string.Equals(this._featureKey, "BASE", StringComparison.OrdinalIgnoreCase))
			{
				mergeGroup.Component = partition;
				mergeGroup.SubComponent = this._expectedRelease.ToString();
			}
			else
			{
				mergeGroup.Component = this._featureKey;
				mergeGroup.SubComponent = partition;
			}
			mergeGroup.Owner = this._ownerOverride;
			mergeGroup.OwnerType = this._ownerTypeOverride;
			mergeGroup.Partition = partition;
			mergeGroup.GroupingKey = this._featureKey;
			mergeGroup.ReleaseType = this._expectedRelease;
			mergeGroup.BuildType = this._expectedBuild;
			mergeGroup.CpuType = this._expectedCpu;
			mergeGroup.Version = this._finalVersion;
			mergeGroup.IsFeatureIdentifier = isFeatureIdentifierGroup;
			return mergeGroup;
		}

		// Token: 0x06000238 RID: 568 RVA: 0x00009928 File Offset: 0x00007B28
		private MergeGroup FindMergeGroup(IPkgInfo pkgInfo)
		{
			MergeGroup mergeGroup = null;
			string partition = pkgInfo.Partition;
			if (!this._allGroups.TryGetValue(partition, out mergeGroup))
			{
				mergeGroup = this.NewGroup(pkgInfo.Partition, false);
				this._allGroups.Add(partition, mergeGroup);
			}
			return mergeGroup;
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000996C File Offset: 0x00007B6C
		public bool IsSpecialPkg(KeyValuePair<string, IPkgInfo> pkg)
		{
			if (pkg.Value.IsBinaryPartition)
			{
				return true;
			}
			if (Path.GetExtension(pkg.Key).Equals(PkgConstants.c_strCBSPackageExtension, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			if (pkg.Value.OwnerType == OwnerType.Microsoft)
			{
				if ("MobileCore".Equals(pkg.Value.GroupingKey, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				if ("MobileCoreMFGOS".Equals(pkg.Value.GroupingKey, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				if ("FactoryOS".Equals(pkg.Value.GroupingKey, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				if ("OneCore".Equals(pkg.Value.GroupingKey, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				if ("IoTUAP".Equals(pkg.Value.GroupingKey, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				if (FBMerger._specialPackages.Contains(pkg.Value.Name, StringComparer.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			if (pkg.Value.OwnerType != OwnerType.Microsoft && "RegistryCustomization".Equals(pkg.Value.SubComponent, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			if (pkg.Value.OwnerType != OwnerType.Microsoft && pkg.Value.SubComponent.StartsWith("Customizations.", StringComparison.OrdinalIgnoreCase) && pkg.Value.SubComponent.EndsWith("." + pkg.Value.Partition, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			if (pkg.Value.Partition.Equals(PkgConstants.c_strUpdateOsPartition, StringComparison.OrdinalIgnoreCase))
			{
				if (pkg.Value.OwnerType != OwnerType.Microsoft)
				{
					return true;
				}
				if (!string.IsNullOrEmpty(this._ownerOverride) && this._ownerTypeOverride != OwnerType.Microsoft)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600023A RID: 570 RVA: 0x00009B20 File Offset: 0x00007D20
		private bool Validate(IEnumerable<KeyValuePair<string, IPkgInfo>> allPkgs)
		{
			bool result = true;
			if (string.IsNullOrEmpty(this._ownerOverride))
			{
				IEnumerable<IGrouping<string, KeyValuePair<string, IPkgInfo>>> enumerable = from x in allPkgs
				group x by x.Value.Owner;
				if (enumerable.Count<IGrouping<string, KeyValuePair<string, IPkgInfo>>>() > 1)
				{
					MergeErrors.Instance.Add("Packages for feature '{0}' having multiple 'Owner' values are not allowed to be merged:", new object[]
					{
						this._featureKey
					});
					foreach (IGrouping<string, KeyValuePair<string, IPkgInfo>> grouping in enumerable)
					{
						MergeErrors.Instance.Add("\t {0}", new object[]
						{
							grouping.Key
						});
						foreach (KeyValuePair<string, IPkgInfo> keyValuePair in grouping)
						{
							MergeErrors.Instance.Add("\t\t{0}", new object[]
							{
								keyValuePair.Key
							});
						}
					}
					result = false;
				}
				IEnumerable<IGrouping<OwnerType, KeyValuePair<string, IPkgInfo>>> enumerable2 = from x in allPkgs
				group x by x.Value.OwnerType;
				if (enumerable2.Count<IGrouping<OwnerType, KeyValuePair<string, IPkgInfo>>>() > 1)
				{
					MergeErrors.Instance.Add("Packages having multiple 'OwnerType' values are not allowed to be merged:");
					foreach (IGrouping<OwnerType, KeyValuePair<string, IPkgInfo>> grouping2 in enumerable2)
					{
						MergeErrors.Instance.Add("\t {0}", new object[]
						{
							grouping2.Key
						});
						foreach (KeyValuePair<string, IPkgInfo> keyValuePair2 in grouping2)
						{
							MergeErrors.Instance.Add("\t\t{0}", new object[]
							{
								keyValuePair2.Key
							});
						}
					}
					result = false;
				}
				this._ownerOverride = enumerable.First<IGrouping<string, KeyValuePair<string, IPkgInfo>>>().Key;
				this._ownerTypeOverride = enumerable2.First<IGrouping<OwnerType, KeyValuePair<string, IPkgInfo>>>().Key;
			}
			IEnumerable<KeyValuePair<string, IPkgInfo>> enumerable3 = from x in allPkgs
			where x.Value.CpuType != this._expectedCpu
			select x;
			if (enumerable3.Count<KeyValuePair<string, IPkgInfo>>() > 0)
			{
				foreach (KeyValuePair<string, IPkgInfo> keyValuePair3 in enumerable3)
				{
					MergeErrors.Instance.Add("Unexpected CPU type '{0}' in package '{1}', expecting '{2}'", new object[]
					{
						keyValuePair3.Value.CpuType,
						keyValuePair3.Key,
						this._expectedCpu
					});
				}
				result = false;
			}
			if (this._ownerTypeOverride != OwnerType.OEM || this._expectedBuild == BuildType.Retail)
			{
				IEnumerable<KeyValuePair<string, IPkgInfo>> enumerable4 = from x in allPkgs
				where x.Value.BuildType != this._expectedBuild
				select x;
				if (enumerable4.Count<KeyValuePair<string, IPkgInfo>>() > 0)
				{
					foreach (KeyValuePair<string, IPkgInfo> keyValuePair4 in enumerable4)
					{
						MergeErrors.Instance.Add("Unexpected Build type '{0}' in package '{1}', expecting '{2}'", new object[]
						{
							keyValuePair4.Value.BuildType,
							keyValuePair4.Key,
							this._expectedBuild
						});
					}
					result = false;
				}
			}
			if (this._expectedRelease == ReleaseType.Production)
			{
				IEnumerable<KeyValuePair<string, IPkgInfo>> enumerable5 = from x in allPkgs
				where x.Value.ReleaseType != this._expectedRelease
				select x;
				if (enumerable5.Count<KeyValuePair<string, IPkgInfo>>() > 0)
				{
					foreach (KeyValuePair<string, IPkgInfo> keyValuePair5 in enumerable5)
					{
						MergeErrors.Instance.Add("Unexpected release type '{0}' in package '{1}', expecting '{2}'", new object[]
						{
							keyValuePair5.Value.ReleaseType,
							keyValuePair5.Key,
							this._expectedRelease
						});
					}
					result = false;
				}
			}
			return result;
		}

		// Token: 0x0600023B RID: 571 RVA: 0x00009F3C File Offset: 0x0000813C
		private FBMerger(string featureKey, VersionInfo outputVersion, string ownerOverride, OwnerType ownerTypeOverride, ReleaseType expectedRelease, CpuId expectedCpu, BuildType expectedBuild)
		{
			this._finalVersion = outputVersion;
			this._expectedRelease = expectedRelease;
			this._expectedBuild = expectedBuild;
			this._expectedCpu = expectedCpu;
			this._ownerOverride = ownerOverride;
			this._ownerTypeOverride = ownerTypeOverride;
			this._featureKey = featureKey;
		}

		// Token: 0x0600023C RID: 572 RVA: 0x00009FA0 File Offset: 0x000081A0
		private MergeResult[] ProcessExcludedPackages(IEnumerable<KeyValuePair<string, IPkgInfo>> pkgs, string outputDir)
		{
			List<MergeResult> list = new List<MergeResult>();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, IPkgInfo> keyValuePair in pkgs)
			{
				list.Add(new MergeResult
				{
					FilePath = keyValuePair.Key,
					IsNonMergedPackage = true
				});
				LogUtil.Message("Skipping special package '{0}'", new object[]
				{
					keyValuePair.Key
				});
				stringBuilder.AppendFormat("{0},{1}", keyValuePair.Key, keyValuePair.Value.Version);
				stringBuilder.AppendLine();
			}
			if (list.Count > 0)
			{
				File.WriteAllText(Path.Combine(outputDir, string.Format("unmerged_{0}.txt", this._featureKey)), stringBuilder.ToString());
			}
			return list.ToArray();
		}

		// Token: 0x0600023D RID: 573 RVA: 0x0000A088 File Offset: 0x00008288
		private MergeResult[] Merge(IEnumerable<string> inputPkgs, string outputDir, bool compress, bool incremental)
		{
			LogUtil.Message("Merging packages for feature '{0}'", new object[]
			{
				this._featureKey
			});
			List<MergeResult> list = new List<MergeResult>();
			string[] array = (from x in inputPkgs
			group x by x into x
			where x.Count<string>() > 1
			select x.Key).ToArray<string>();
			if (array.Length != 0)
			{
				throw new PackageException("Duplicated packages detected for feature {0}:\n\t{1}", new object[]
				{
					this._featureKey,
					string.Join("\n\t", array)
				});
			}
			Dictionary<string, IPkgInfo> dictionary = inputPkgs.ToDictionary((string x) => x, (string x) => Package.LoadFromCab(x), StringComparer.OrdinalIgnoreCase);
			IEnumerable<KeyValuePair<string, IPkgInfo>> enumerable = from x in dictionary
			where this.IsSpecialPkg(x)
			select x;
			IEnumerable<KeyValuePair<string, IPkgInfo>> enumerable2 = dictionary.Except(enumerable);
			if (!this.Validate(enumerable2))
			{
				MergeErrors.Instance.Add("Some packages failed to pass global validation");
			}
			list.AddRange(this.ProcessExcludedPackages(enumerable, outputDir));
			MergeGroup value = this.NewGroup(PkgConstants.c_strMainOsPartition, true);
			this._allGroups.Add(PkgConstants.c_strMainOsPartition, value);
			foreach (KeyValuePair<string, IPkgInfo> pkg in enumerable2)
			{
				this.FindMergeGroup(pkg.Value).AddPkg(pkg);
			}
			foreach (MergeGroup mergeGroup in this._allGroups.Values)
			{
				list.Add(mergeGroup.Merge(outputDir, compress, incremental));
			}
			MergeErrors instance = MergeErrors.Instance;
			MergeErrors.Clear();
			instance.CheckResult();
			foreach (MergeResult mergeResult in list)
			{
				mergeResult.PkgInfo = Package.LoadFromCab(mergeResult.FilePath);
			}
			LogUtil.Message("Done.");
			return list.ToArray();
		}

		// Token: 0x0600023E RID: 574 RVA: 0x0000A308 File Offset: 0x00008508
		public static MergeResult[] Merge(IEnumerable<string> inputPkgs, string featureKey, VersionInfo outputVersion, string ownerOverride, OwnerType ownerTypeOverride, ReleaseType expectedReleaseType, CpuId expectedCpuType, BuildType expectedBuildType, string outputDir, bool compress, bool incremental)
		{
			return new FBMerger(featureKey, outputVersion, ownerOverride, ownerTypeOverride, expectedReleaseType, expectedCpuType, expectedBuildType).Merge(inputPkgs, outputDir, compress, incremental);
		}

		// Token: 0x040000EF RID: 239
		private VersionInfo _finalVersion = VersionInfo.Empty;

		// Token: 0x040000F0 RID: 240
		private CpuId _expectedCpu;

		// Token: 0x040000F1 RID: 241
		private ReleaseType _expectedRelease;

		// Token: 0x040000F2 RID: 242
		private BuildType _expectedBuild;

		// Token: 0x040000F3 RID: 243
		private string _ownerOverride;

		// Token: 0x040000F4 RID: 244
		private OwnerType _ownerTypeOverride;

		// Token: 0x040000F5 RID: 245
		private string _featureKey;

		// Token: 0x040000F6 RID: 246
		private Dictionary<string, MergeGroup> _allGroups = new Dictionary<string, MergeGroup>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x040000F7 RID: 247
		private static string[] _specialPackages = new string[]
		{
			"Microsoft.BCD.bootlog.winload",
			"Microsoft.BCD.bootlog.bootmgr",
			"Microsoft.Net.FakeModem",
			"Microsoft.Net.FakeWwan"
		};
	}
}
