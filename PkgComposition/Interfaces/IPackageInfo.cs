using System;
using Microsoft.Composition.ToolBox;

namespace Microsoft.Composition.Packaging.Interfaces
{
	// Token: 0x0200000C RID: 12
	public interface IPackageInfo
	{
		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000179 RID: 377
		string PackageName { get; }

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x0600017A RID: 378
		string Owner { get; }

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x0600017B RID: 379
		PhoneReleaseType PhoneReleaseType { get; }

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x0600017C RID: 380
		string Component { get; }

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x0600017D RID: 381
		string SubComponent { get; }

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x0600017E RID: 382
		Version Version { get; }

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x0600017F RID: 383
		PhoneOwnerType OwnerType { get; }

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000180 RID: 384
		string ReleaseType { get; }

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x06000181 RID: 385
		BuildType BuildType { get; }

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x06000182 RID: 386
		CpuArch HostArch { get; }

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x06000183 RID: 387
		CpuArch GuestArch { get; }

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x06000184 RID: 388
		string CpuString { get; }

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x06000185 RID: 389
		bool IsWow { get; }

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x06000186 RID: 390
		string Culture { get; }

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000187 RID: 391
		string PublicKey { get; }

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x06000188 RID: 392
		string GroupingKey { get; }

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x06000189 RID: 393
		string Partition { get; }

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x0600018A RID: 394
		bool? BinaryPartition { get; }

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x0600018B RID: 395
		PackageType PackageType { get; }

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x0600018C RID: 396
		PackageMetrics Metrics { get; }

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x0600018D RID: 397
		bool IsFIPPackage { get; }
	}
}
