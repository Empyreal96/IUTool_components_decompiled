using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x02000008 RID: 8
	[XmlRoot(ElementName = "UpdateEvent", IsNullable = false)]
	public class UpdateEvent
	{
		// Token: 0x0400002A RID: 42
		public int Sequence;

		// Token: 0x0400002B RID: 43
		public string DateTime;

		// Token: 0x0400002C RID: 44
		public string Summary;

		// Token: 0x0400002D RID: 45
		[XmlElement("UpdateOSOutput")]
		public UpdateOSOutput UpdateResults;
	}
}
