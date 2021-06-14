using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000065 RID: 101
	[XmlType(IncludeInSchema = false)]
	public enum ValueTypeChoice
	{
		// Token: 0x0400027B RID: 635
		StringValue,
		// Token: 0x0400027C RID: 636
		BooleanValue,
		// Token: 0x0400027D RID: 637
		ObjectValue,
		// Token: 0x0400027E RID: 638
		ObjectListValue,
		// Token: 0x0400027F RID: 639
		IntegerValue,
		// Token: 0x04000280 RID: 640
		IntegerListValue,
		// Token: 0x04000281 RID: 641
		DeviceValue
	}
}
