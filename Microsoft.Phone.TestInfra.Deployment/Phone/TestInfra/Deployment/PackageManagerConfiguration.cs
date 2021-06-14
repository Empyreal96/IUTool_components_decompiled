using System;
using System.Collections.Generic;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000027 RID: 39
	public class PackageManagerConfiguration : ICloneable
	{
		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060001B4 RID: 436 RVA: 0x0000A894 File Offset: 0x00008A94
		// (set) Token: 0x060001B5 RID: 437 RVA: 0x0000A89C File Offset: 0x00008A9C
		public TimeSpan ExpiresIn { get; set; }

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060001B6 RID: 438 RVA: 0x0000A8A5 File Offset: 0x00008AA5
		// (set) Token: 0x060001B7 RID: 439 RVA: 0x0000A8AD File Offset: 0x00008AAD
		public string OutputPath { get; set; }

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060001B8 RID: 440 RVA: 0x0000A8B6 File Offset: 0x00008AB6
		// (set) Token: 0x060001B9 RID: 441 RVA: 0x0000A8BE File Offset: 0x00008ABE
		public IEnumerable<string> RootPaths { get; set; }

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060001BA RID: 442 RVA: 0x0000A8C7 File Offset: 0x00008AC7
		// (set) Token: 0x060001BB RID: 443 RVA: 0x0000A8CF File Offset: 0x00008ACF
		public IEnumerable<string> AlternateRootPaths { get; set; }

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x060001BC RID: 444 RVA: 0x0000A8D8 File Offset: 0x00008AD8
		// (set) Token: 0x060001BD RID: 445 RVA: 0x0000A8E0 File Offset: 0x00008AE0
		public string CachePath { get; set; }

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x060001BE RID: 446 RVA: 0x0000A8E9 File Offset: 0x00008AE9
		// (set) Token: 0x060001BF RID: 447 RVA: 0x0000A8F1 File Offset: 0x00008AF1
		public string PackagesExtractionCachePath { get; set; }

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x060001C0 RID: 448 RVA: 0x0000A8FA File Offset: 0x00008AFA
		// (set) Token: 0x060001C1 RID: 449 RVA: 0x0000A902 File Offset: 0x00008B02
		public bool SourceRootIsVolatile { get; set; }

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x060001C2 RID: 450 RVA: 0x0000A90B File Offset: 0x00008B0B
		// (set) Token: 0x060001C3 RID: 451 RVA: 0x0000A913 File Offset: 0x00008B13
		public bool RecursiveDeployment { get; set; }

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x060001C4 RID: 452 RVA: 0x0000A91C File Offset: 0x00008B1C
		// (set) Token: 0x060001C5 RID: 453 RVA: 0x0000A924 File Offset: 0x00008B24
		public IDictionary<string, string> Macros { get; set; }

		// Token: 0x060001C6 RID: 454 RVA: 0x0000A930 File Offset: 0x00008B30
		public object Clone()
		{
			return new PackageManagerConfiguration
			{
				OutputPath = this.OutputPath,
				RootPaths = this.RootPaths,
				AlternateRootPaths = this.AlternateRootPaths,
				CachePath = this.CachePath,
				PackagesExtractionCachePath = this.PackagesExtractionCachePath,
				SourceRootIsVolatile = this.SourceRootIsVolatile,
				RecursiveDeployment = this.RecursiveDeployment,
				ExpiresIn = this.ExpiresIn
			};
		}
	}
}
