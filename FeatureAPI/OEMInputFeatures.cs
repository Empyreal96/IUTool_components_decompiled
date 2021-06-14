using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x02000006 RID: 6
	public class OEMInputFeatures
	{
		// Token: 0x04000027 RID: 39
		[XmlArrayItem(ElementName = "Feature", Type = typeof(string), IsNullable = false)]
		[XmlArray]
		public List<string> Microsoft;

		// Token: 0x04000028 RID: 40
		[XmlArrayItem(ElementName = "Feature", Type = typeof(string), IsNullable = false)]
		[XmlArray]
		public List<string> OEM;
	}
}
