using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x02000023 RID: 35
	public class EditionUISettings
	{
		// Token: 0x040000A9 RID: 169
		[XmlAttribute]
		public UIDisplayType DisplayType;

		// Token: 0x040000AA RID: 170
		[XmlArrayItem(ElementName = "Lookup", Type = typeof(EditionLookup), IsNullable = false)]
		[XmlArray]
		public List<EditionLookup> Lookups;
	}
}
