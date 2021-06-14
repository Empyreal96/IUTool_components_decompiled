using System;

namespace Microsoft.Phone.Test.TestMetadata.Helper
{
	// Token: 0x0200000F RID: 15
	public class PortableExecutableDependency
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000037 RID: 55 RVA: 0x00002B4C File Offset: 0x00000D4C
		// (set) Token: 0x06000038 RID: 56 RVA: 0x00002B54 File Offset: 0x00000D54
		public BinaryDependencyType Type { get; set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000039 RID: 57 RVA: 0x00002B5D File Offset: 0x00000D5D
		// (set) Token: 0x0600003A RID: 58 RVA: 0x00002B65 File Offset: 0x00000D65
		public string Name { get; set; }
	}
}
