using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x0200006D RID: 109
	public class PackageTocBuilder : PackageSetBuilderBase, IPackageSetBuilder
	{
		// Token: 0x0600020F RID: 527 RVA: 0x00007C88 File Offset: 0x00005E88
		private void BuildPackageTOC(SatelliteId satelliteId, IEnumerable<FileInfo> files, string outputDir)
		{
			IPkgBuilder pkgBuilder = base.CreatePackage(satelliteId);
			string text = Path.Combine(outputDir, pkgBuilder.Name + ".TOC");
			LogUtil.Message("Building package content list '{0}'", new object[]
			{
				text
			});
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Partition={0}", pkgBuilder.Partition);
			stringBuilder.AppendLine();
			foreach (FileInfo fileInfo in files)
			{
				if (fileInfo.Type == FileType.Regular)
				{
					stringBuilder.AppendFormat("{0},{1}", fileInfo.SourcePath, fileInfo.DevicePath);
					stringBuilder.AppendLine();
				}
			}
			LongPathFile.WriteAllText(text, stringBuilder.ToString());
			LogUtil.Message("Done package content list '{0}'", new object[]
			{
				text
			});
		}

		// Token: 0x06000210 RID: 528 RVA: 0x00004469 File Offset: 0x00002669
		public void AddRegValue(SatelliteId satelliteId, RegValueInfo valueInfo)
		{
		}

		// Token: 0x06000211 RID: 529 RVA: 0x00004469 File Offset: 0x00002669
		public void AddMultiSzSegment(string keyName, string valueName, params string[] valueSegments)
		{
		}

		// Token: 0x06000212 RID: 530 RVA: 0x00007D68 File Offset: 0x00005F68
		public void Save(string outputDir)
		{
			foreach (IGrouping<SatelliteId, KeyValuePair<SatelliteId, FileInfo>> grouping in from x in this._allFiles
			group x by x.Key)
			{
				this.BuildPackageTOC(grouping.Key, grouping.Select((KeyValuePair<SatelliteId, FileInfo> x) => x.Value), outputDir);
			}
		}

		// Token: 0x06000213 RID: 531 RVA: 0x00007E04 File Offset: 0x00006004
		public PackageTocBuilder(CpuId cpuType, BuildType bldType, VersionInfo version) : base(cpuType, bldType, version)
		{
		}

		// Token: 0x0400017F RID: 383
		private const string c_strPkgTocExtension = ".TOC";
	}
}
