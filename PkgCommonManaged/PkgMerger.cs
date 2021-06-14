using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000036 RID: 54
	internal class PkgMerger : IDisposable
	{
		// Token: 0x06000244 RID: 580 RVA: 0x0000A3A8 File Offset: 0x000085A8
		private static string BuildComponentString(IPkgInfo pkgInfo)
		{
			if (string.IsNullOrEmpty(pkgInfo.Partition))
			{
				throw new PackageException("Unexpected empty partition for package '{0}'", new object[]
				{
					pkgInfo.Name
				});
			}
			if (string.IsNullOrEmpty(pkgInfo.Platform))
			{
				return pkgInfo.Partition;
			}
			return string.Format("{0}_{1}", pkgInfo.Platform, pkgInfo.Partition);
		}

		// Token: 0x06000245 RID: 581 RVA: 0x0000A408 File Offset: 0x00008608
		private static string BuildSubComponentString(IPkgInfo pkgInfo)
		{
			if (string.IsNullOrEmpty(pkgInfo.GroupingKey))
			{
				return pkgInfo.ReleaseType.ToString();
			}
			return string.Format("{0}_{1}", pkgInfo.GroupingKey, pkgInfo.ReleaseType);
		}

		// Token: 0x06000246 RID: 582 RVA: 0x0000A452 File Offset: 0x00008652
		private void Error(string format, params object[] args)
		{
			LogUtil.Error(format, args);
			this._hasError = true;
		}

		// Token: 0x06000247 RID: 583 RVA: 0x0000A464 File Offset: 0x00008664
		private Tuple<IPkgBuilder, StringBuilder> FindMergedPackage(IPkgInfo pkg)
		{
			string key = string.Format("{0}.{1}.{2}.{3}.{4}.{5}.{6}.{7}.{8}", new object[]
			{
				pkg.Owner,
				pkg.Partition,
				pkg.Platform,
				pkg.GroupingKey,
				pkg.ReleaseType,
				pkg.BuildType,
				pkg.CpuType,
				pkg.Culture,
				pkg.Resolution
			});
			Tuple<IPkgBuilder, StringBuilder> tuple = null;
			if (!this._allMergedPackages.TryGetValue(key, out tuple))
			{
				IPkgBuilder pkgBuilder = Package.Create();
				pkgBuilder.Owner = pkg.Owner;
				pkgBuilder.OwnerType = pkg.OwnerType;
				pkgBuilder.Component = PkgMerger.BuildComponentString(pkg);
				pkgBuilder.SubComponent = PkgMerger.BuildSubComponentString(pkg);
				pkgBuilder.Resolution = pkg.Resolution;
				pkgBuilder.Culture = pkg.Culture;
				pkgBuilder.Partition = pkg.Partition;
				pkgBuilder.Platform = pkg.Platform;
				pkgBuilder.CpuType = pkg.CpuType;
				pkgBuilder.ReleaseType = pkg.ReleaseType;
				pkgBuilder.BuildType = pkg.BuildType;
				pkgBuilder.GroupingKey = pkg.GroupingKey;
				tuple = Tuple.Create<IPkgBuilder, StringBuilder>(pkgBuilder, new StringBuilder());
				this._allMergedPackages.Add(key, tuple);
			}
			return tuple;
		}

		// Token: 0x06000248 RID: 584 RVA: 0x0000A5A8 File Offset: 0x000087A8
		private void AddPackage(string pkgFile)
		{
			if (!LongPathFile.Exists(pkgFile))
			{
				this.Error("Package '{0}' doesn't exist", new object[]
				{
					pkgFile
				});
				return;
			}
			string outputDir = Path.Combine(this._tmpDir, this._allPackages.Count<KeyValuePair<string, string>>().ToString());
			WPExtractedPackage wpextractedPackage = WPCanonicalPackage.ExtractAndLoad(pkgFile, outputDir);
			if (wpextractedPackage.IsBinaryPartition)
			{
				LogUtil.Warning("Package '{0}' skipped because it's a binary partition package", new object[]
				{
					pkgFile
				});
				return;
			}
			if (wpextractedPackage.Version != this._finalVersion)
			{
				if (this._finalVersion != default(VersionInfo))
				{
					LogUtil.Message("Package '{0}' has inconsistent version '{1}', expecting '{2}', choosing higher one", new object[]
					{
						pkgFile,
						wpextractedPackage.Version,
						this._finalVersion
					});
				}
				if (wpextractedPackage.Version > this._finalVersion)
				{
					this._finalVersion = wpextractedPackage.Version;
				}
			}
			if (string.IsNullOrEmpty(wpextractedPackage.Partition))
			{
				this.Error("Package '{0}' contains empty partition name", new object[]
				{
					pkgFile
				});
				return;
			}
			if (wpextractedPackage.ReleaseType != this._releaseType)
			{
				this.Error("Package '{0}' has unexpected release type '{1}', expecting '{2}'", new object[]
				{
					pkgFile,
					wpextractedPackage.ReleaseType,
					this._releaseType
				});
				return;
			}
			if (wpextractedPackage.CpuType != this._cpuType)
			{
				this.Error("Package '{0}' has unexpected cpu type '{1}', expecting '{2}'", new object[]
				{
					pkgFile,
					wpextractedPackage.CpuType,
					this._cpuType
				});
				return;
			}
			if (wpextractedPackage.BuildType != this._buildType)
			{
				this.Error("Package '{0}' has unexpected build type '{1}', expecting '{2}'", new object[]
				{
					pkgFile,
					wpextractedPackage.BuildType,
					this._buildType
				});
				return;
			}
			string key = string.Format("Name:{0}, Partition:{1}", wpextractedPackage.Name, wpextractedPackage.Partition);
			if (this._allPackages.ContainsKey(key))
			{
				this.Error("Package '{0}' and package '{1}' have same package name and target partition", new object[]
				{
					pkgFile,
					this._allPackages[key]
				});
				return;
			}
			this._allPackages.Add(key, pkgFile);
			Tuple<IPkgBuilder, StringBuilder> tuple = this.FindMergedPackage(wpextractedPackage);
			IPkgBuilder item = tuple.Item1;
			if (item.OwnerType != wpextractedPackage.OwnerType)
			{
				this.Error("Package '{0}' has inconsistent owner type '{1}', expecting '{2}'", new object[]
				{
					pkgFile,
					wpextractedPackage.OwnerType,
					item.OwnerType
				});
				return;
			}
			foreach (IFileEntry fileEntry in wpextractedPackage.Files)
			{
				FileEntry fileEntry2 = (FileEntry)fileEntry;
				if (fileEntry2.FileType != FileType.Manifest && fileEntry2.FileType != FileType.Catalog)
				{
					IFileEntry fileEntry3 = item.FindFile(fileEntry2.DevicePath);
					if (fileEntry3 != null)
					{
						this.Error("Package '{0}' and package with name '{1}' both contain file with same device path '{2}'", new object[]
						{
							pkgFile,
							fileEntry3.SourcePackage,
							fileEntry2.DevicePath
						});
					}
					else
					{
						item.AddFile(fileEntry2.FileType, fileEntry2.SourcePath, fileEntry2.DevicePath, fileEntry2.Attributes, wpextractedPackage.Name, "None");
					}
				}
			}
			tuple.Item2.AppendLine(string.Format("{0},{1}", pkgFile, wpextractedPackage.Version));
		}

		// Token: 0x06000249 RID: 585 RVA: 0x0000A90C File Offset: 0x00008B0C
		private void Save(string outputDir, VersionInfo? outputVersion, bool compress)
		{
			if (this._hasError)
			{
				throw new PackageException("Errors occurred during merging, check console output for details");
			}
			if (outputVersion != null)
			{
				this._finalVersion = outputVersion.Value;
			}
			foreach (Tuple<IPkgBuilder, StringBuilder> tuple in this._allMergedPackages.Values)
			{
				IPkgBuilder item = tuple.Item1;
				item.Version = this._finalVersion;
				item.SaveCab(Path.Combine(outputDir, item.Name + PkgConstants.c_strPackageExtension), compress);
				StringBuilder item2 = tuple.Item2;
				LongPathFile.WriteAllText(Path.Combine(outputDir, item.Name + ".merged.txt"), item2.ToString());
			}
		}

		// Token: 0x0600024A RID: 586 RVA: 0x0000A9DC File Offset: 0x00008BDC
		private PkgMerger(ReleaseType releaseType, CpuId cpuType, BuildType buildType)
		{
			this._releaseType = releaseType;
			this._cpuType = cpuType;
			this._buildType = buildType;
			this._tmpDir = FileUtils.GetTempDirectory();
			this._allPackages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			this._allMergedPackages = new Dictionary<string, Tuple<IPkgBuilder, StringBuilder>>(StringComparer.OrdinalIgnoreCase);
		}

		// Token: 0x0600024B RID: 587 RVA: 0x0000AA60 File Offset: 0x00008C60
		internal static void Merge(IEnumerable<string> inputPkgs, VersionInfo? outputVersion, ReleaseType expectedReleaseType, CpuId expectedCpuType, BuildType expectedBuildType, string outputDir, bool compress, IDeploymentLogger _logger)
		{
			using (PkgMerger pkgMerger = new PkgMerger(expectedReleaseType, expectedCpuType, expectedBuildType))
			{
				foreach (string text in inputPkgs)
				{
					_logger.LogInfo("Adding package '{0}' ...", new object[]
					{
						text
					});
					pkgMerger.AddPackage(text);
				}
				_logger.LogInfo("Merging and saving results to directory '{0}' ...", new object[]
				{
					outputDir
				});
				pkgMerger.Save(outputDir, outputVersion, compress);
				_logger.LogInfo("Done.", new object[0]);
			}
		}

		// Token: 0x0600024C RID: 588 RVA: 0x0000AB14 File Offset: 0x00008D14
		public void Dispose()
		{
			foreach (Tuple<IPkgBuilder, StringBuilder> tuple in this._allMergedPackages.Values)
			{
				tuple.Item1.Dispose();
			}
			try
			{
				FileUtils.DeleteTree(this._tmpDir);
			}
			catch (IOException)
			{
			}
		}

		// Token: 0x040000F8 RID: 248
		private bool _hasError;

		// Token: 0x040000F9 RID: 249
		private string _tmpDir;

		// Token: 0x040000FA RID: 250
		private VersionInfo _finalVersion = new VersionInfo(0, 0, 0, 0);

		// Token: 0x040000FB RID: 251
		private CpuId _cpuType;

		// Token: 0x040000FC RID: 252
		private BuildType _buildType;

		// Token: 0x040000FD RID: 253
		private ReleaseType _releaseType;

		// Token: 0x040000FE RID: 254
		private Dictionary<string, string> _allPackages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x040000FF RID: 255
		private Dictionary<string, Tuple<IPkgBuilder, StringBuilder>> _allMergedPackages = new Dictionary<string, Tuple<IPkgBuilder, StringBuilder>>(StringComparer.OrdinalIgnoreCase);
	}
}
