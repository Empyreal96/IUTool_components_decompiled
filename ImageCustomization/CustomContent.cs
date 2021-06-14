using System;
using System.Collections.Generic;

namespace Microsoft.WindowsPhone.ImageUpdate.Customization
{
	// Token: 0x02000004 RID: 4
	public class CustomContent
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600001C RID: 28 RVA: 0x00002238 File Offset: 0x00000438
		// (set) Token: 0x0600001D RID: 29 RVA: 0x00002240 File Offset: 0x00000440
		public IEnumerable<CustomizationError> CustomizationErrors { get; internal set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600001E RID: 30 RVA: 0x00002249 File Offset: 0x00000449
		// (set) Token: 0x0600001F RID: 31 RVA: 0x00002251 File Offset: 0x00000451
		public IEnumerable<string> PackagePaths { get; internal set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000020 RID: 32 RVA: 0x0000225A File Offset: 0x0000045A
		// (set) Token: 0x06000021 RID: 33 RVA: 0x00002262 File Offset: 0x00000462
		public List<KeyValuePair<string, string>> DataContent { get; internal set; }
	}
}
