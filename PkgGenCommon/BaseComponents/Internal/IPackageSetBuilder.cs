using System;
using System.Collections.Generic;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x02000071 RID: 113
	public interface IPackageSetBuilder
	{
		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000222 RID: 546
		// (set) Token: 0x06000223 RID: 547
		string Owner { get; set; }

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000224 RID: 548
		// (set) Token: 0x06000225 RID: 549
		string Component { get; set; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000226 RID: 550
		// (set) Token: 0x06000227 RID: 551
		string SubComponent { get; set; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000228 RID: 552
		string Name { get; }

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x06000229 RID: 553
		// (set) Token: 0x0600022A RID: 554
		OwnerType OwnerType { get; set; }

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x0600022B RID: 555
		// (set) Token: 0x0600022C RID: 556
		ReleaseType ReleaseType { get; set; }

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x0600022D RID: 557
		// (set) Token: 0x0600022E RID: 558
		string Partition { get; set; }

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x0600022F RID: 559
		// (set) Token: 0x06000230 RID: 560
		string Platform { get; set; }

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000231 RID: 561
		// (set) Token: 0x06000232 RID: 562
		string GroupingKey { get; set; }

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000233 RID: 563
		// (set) Token: 0x06000234 RID: 564
		string Description { get; set; }

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x06000235 RID: 565
		List<SatelliteId> Resolutions { get; }

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06000236 RID: 566
		BuildType BuildType { get; }

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000237 RID: 567
		CpuId CpuType { get; }

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06000238 RID: 568
		VersionInfo Version { get; }

		// Token: 0x06000239 RID: 569
		void AddFile(SatelliteId satelliteId, FileInfo fileInfo);

		// Token: 0x0600023A RID: 570
		void AddRegValue(SatelliteId satelliteId, RegValueInfo valueInfo);

		// Token: 0x0600023B RID: 571
		void AddMultiSzSegment(string keyName, string valueName, params string[] valueSegments);

		// Token: 0x0600023C RID: 572
		void Save(string outputDir);
	}
}
