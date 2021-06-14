using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000034 RID: 52
	internal class MergeGroup
	{
		// Token: 0x0600021B RID: 539 RVA: 0x0000942C File Offset: 0x0000762C
		private IPkgBuilder GetResultBuilder()
		{
			IPkgBuilder pkgBuilder = Package.Create();
			pkgBuilder.Owner = this.Owner;
			pkgBuilder.OwnerType = this.OwnerType;
			pkgBuilder.Component = this.Component;
			pkgBuilder.SubComponent = this.SubComponent;
			pkgBuilder.Partition = this.Partition;
			pkgBuilder.Platform = this.Platform;
			pkgBuilder.GroupingKey = this.GroupingKey;
			pkgBuilder.ReleaseType = this.ReleaseType;
			pkgBuilder.CpuType = this.CpuType;
			pkgBuilder.BuildType = this.BuildType;
			pkgBuilder.Version = this.Version;
			return pkgBuilder;
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x0600021C RID: 540 RVA: 0x000094C2 File Offset: 0x000076C2
		// (set) Token: 0x0600021D RID: 541 RVA: 0x000094CA File Offset: 0x000076CA
		public string Owner { get; set; }

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x0600021E RID: 542 RVA: 0x000094D3 File Offset: 0x000076D3
		// (set) Token: 0x0600021F RID: 543 RVA: 0x000094DB File Offset: 0x000076DB
		public OwnerType OwnerType { get; set; }

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x06000220 RID: 544 RVA: 0x000094E4 File Offset: 0x000076E4
		// (set) Token: 0x06000221 RID: 545 RVA: 0x000094EC File Offset: 0x000076EC
		public string Component { get; set; }

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x06000222 RID: 546 RVA: 0x000094F5 File Offset: 0x000076F5
		// (set) Token: 0x06000223 RID: 547 RVA: 0x000094FD File Offset: 0x000076FD
		public string SubComponent { get; set; }

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000224 RID: 548 RVA: 0x00009506 File Offset: 0x00007706
		// (set) Token: 0x06000225 RID: 549 RVA: 0x0000950E File Offset: 0x0000770E
		public string Partition { get; set; }

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x06000226 RID: 550 RVA: 0x00009517 File Offset: 0x00007717
		public string Platform { get; }

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x06000227 RID: 551 RVA: 0x0000951F File Offset: 0x0000771F
		// (set) Token: 0x06000228 RID: 552 RVA: 0x00009527 File Offset: 0x00007727
		public string GroupingKey { get; set; }

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x06000229 RID: 553 RVA: 0x00009530 File Offset: 0x00007730
		// (set) Token: 0x0600022A RID: 554 RVA: 0x00009538 File Offset: 0x00007738
		public ReleaseType ReleaseType { get; set; }

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x0600022B RID: 555 RVA: 0x00009541 File Offset: 0x00007741
		// (set) Token: 0x0600022C RID: 556 RVA: 0x00009549 File Offset: 0x00007749
		public BuildType BuildType { get; set; }

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x0600022D RID: 557 RVA: 0x00009552 File Offset: 0x00007752
		// (set) Token: 0x0600022E RID: 558 RVA: 0x0000955A File Offset: 0x0000775A
		public CpuId CpuType { get; set; }

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x0600022F RID: 559 RVA: 0x00009563 File Offset: 0x00007763
		// (set) Token: 0x06000230 RID: 560 RVA: 0x0000956B File Offset: 0x0000776B
		public VersionInfo Version { get; set; }

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000231 RID: 561 RVA: 0x00009574 File Offset: 0x00007774
		// (set) Token: 0x06000232 RID: 562 RVA: 0x0000957C File Offset: 0x0000777C
		public bool IsFeatureIdentifier { get; set; }

		// Token: 0x06000233 RID: 563 RVA: 0x00009588 File Offset: 0x00007788
		public void AddPkg(KeyValuePair<string, IPkgInfo> pkg)
		{
			List<string> list = null;
			if (!string.IsNullOrEmpty(pkg.Value.Culture))
			{
				if (!this._langPkgs.TryGetValue(pkg.Value.Culture, out list))
				{
					list = new List<string>();
					this._langPkgs.Add(pkg.Value.Culture, list);
				}
			}
			else if (!string.IsNullOrEmpty(pkg.Value.Resolution))
			{
				if (!this._resPkgs.TryGetValue(pkg.Value.Resolution, out list))
				{
					list = new List<string>();
					this._resPkgs.Add(pkg.Value.Resolution, list);
				}
			}
			else
			{
				list = this._basePkgs;
			}
			list.Add(pkg.Key);
		}

		// Token: 0x06000234 RID: 564 RVA: 0x00009648 File Offset: 0x00007848
		public MergeResult Merge(string outputDir, bool compress, bool incremental)
		{
			MergeResult mergeResult = new MergeResult();
			using (IPkgBuilder resultBuilder = this.GetResultBuilder())
			{
				mergeResult.FilePath = Path.Combine(outputDir, resultBuilder.Name + PkgConstants.c_strPackageExtension);
				MergeWorker.Merge(resultBuilder, this._basePkgs, mergeResult.FilePath, compress, incremental);
			}
			mergeResult.Languages = this._langPkgs.Keys.ToArray<string>();
			foreach (KeyValuePair<string, List<string>> keyValuePair in this._langPkgs)
			{
				using (IPkgBuilder resultBuilder2 = this.GetResultBuilder())
				{
					resultBuilder2.Culture = keyValuePair.Key;
					MergeWorker.Merge(resultBuilder2, keyValuePair.Value, Path.Combine(outputDir, resultBuilder2.Name + PkgConstants.c_strPackageExtension), compress, incremental);
				}
			}
			mergeResult.Resolutions = this._resPkgs.Keys.ToArray<string>();
			foreach (KeyValuePair<string, List<string>> keyValuePair2 in this._resPkgs)
			{
				using (IPkgBuilder resultBuilder3 = this.GetResultBuilder())
				{
					resultBuilder3.Resolution = keyValuePair2.Key;
					MergeWorker.Merge(resultBuilder3, keyValuePair2.Value, Path.Combine(outputDir, resultBuilder3.Name + PkgConstants.c_strPackageExtension), compress, incremental);
				}
			}
			mergeResult.FeatureIdentifierPackage = this.IsFeatureIdentifier;
			return mergeResult;
		}

		// Token: 0x040000E0 RID: 224
		private List<string> _basePkgs = new List<string>();

		// Token: 0x040000E1 RID: 225
		private Dictionary<string, List<string>> _langPkgs = new Dictionary<string, List<string>>();

		// Token: 0x040000E2 RID: 226
		private Dictionary<string, List<string>> _resPkgs = new Dictionary<string, List<string>>();
	}
}
