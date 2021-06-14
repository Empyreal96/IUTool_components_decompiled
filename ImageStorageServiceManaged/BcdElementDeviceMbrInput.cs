using System;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000070 RID: 112
	public class BcdElementDeviceMbrInput
	{
		// Token: 0x1700011E RID: 286
		// (get) Token: 0x060004BC RID: 1212 RVA: 0x00014B1C File Offset: 0x00012D1C
		// (set) Token: 0x060004BD RID: 1213 RVA: 0x00014B24 File Offset: 0x00012D24
		public string DiskSignature { get; set; }

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x060004BE RID: 1214 RVA: 0x00014B2D File Offset: 0x00012D2D
		// (set) Token: 0x060004BF RID: 1215 RVA: 0x00014B35 File Offset: 0x00012D35
		public string PartitionName { get; set; }
	}
}
