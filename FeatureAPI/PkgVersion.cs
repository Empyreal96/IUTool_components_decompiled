using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x0200000D RID: 13
	[CLSCompliant(false)]
	public class PkgVersion
	{
		// Token: 0x04000049 RID: 73
		[XmlAttribute]
		public ushort Major;

		// Token: 0x0400004A RID: 74
		[XmlAttribute]
		public ushort Minor;

		// Token: 0x0400004B RID: 75
		[XmlAttribute]
		public ushort QFE;

		// Token: 0x0400004C RID: 76
		[XmlAttribute]
		public ushort Build;
	}
}
