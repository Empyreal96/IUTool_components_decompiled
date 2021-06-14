using System;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x0200002B RID: 43
	public class MergeResult
	{
		// Token: 0x060001D2 RID: 466 RVA: 0x00007982 File Offset: 0x00005B82
		public MergeResult()
		{
			this.IsNonMergedPackage = false;
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x060001D3 RID: 467 RVA: 0x00007991 File Offset: 0x00005B91
		// (set) Token: 0x060001D4 RID: 468 RVA: 0x00007999 File Offset: 0x00005B99
		public IPkgInfo PkgInfo { get; set; }

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060001D5 RID: 469 RVA: 0x000079A2 File Offset: 0x00005BA2
		// (set) Token: 0x060001D6 RID: 470 RVA: 0x000079AA File Offset: 0x00005BAA
		public string FilePath { get; set; }

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060001D7 RID: 471 RVA: 0x000079B3 File Offset: 0x00005BB3
		// (set) Token: 0x060001D8 RID: 472 RVA: 0x000079BB File Offset: 0x00005BBB
		public string[] Languages { get; set; }

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060001D9 RID: 473 RVA: 0x000079C4 File Offset: 0x00005BC4
		// (set) Token: 0x060001DA RID: 474 RVA: 0x000079CC File Offset: 0x00005BCC
		public string[] Resolutions { get; set; }

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060001DB RID: 475 RVA: 0x000079D5 File Offset: 0x00005BD5
		// (set) Token: 0x060001DC RID: 476 RVA: 0x000079DD File Offset: 0x00005BDD
		public bool FeatureIdentifierPackage { get; set; }

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060001DD RID: 477 RVA: 0x000079E6 File Offset: 0x00005BE6
		// (set) Token: 0x060001DE RID: 478 RVA: 0x000079EE File Offset: 0x00005BEE
		public bool IsNonMergedPackage { get; set; }
	}
}
