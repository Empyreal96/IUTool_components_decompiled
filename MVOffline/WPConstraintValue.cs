using System;

namespace Microsoft.WindowsPhone.Multivariant.Offline
{
	// Token: 0x02000005 RID: 5
	public class WPConstraintValue
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000B RID: 11 RVA: 0x0000238D File Offset: 0x0000058D
		// (set) Token: 0x0600000C RID: 12 RVA: 0x00002395 File Offset: 0x00000595
		public string KeyValue { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000D RID: 13 RVA: 0x0000239E File Offset: 0x0000059E
		// (set) Token: 0x0600000E RID: 14 RVA: 0x000023A6 File Offset: 0x000005A6
		public bool IsWildCard { get; set; }

		// Token: 0x0600000F RID: 15 RVA: 0x000023AF File Offset: 0x000005AF
		public WPConstraintValue(string value, bool iswildcard)
		{
			this.KeyValue = value;
			this.IsWildCard = iswildcard;
		}
	}
}
