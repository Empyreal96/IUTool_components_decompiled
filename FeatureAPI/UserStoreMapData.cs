using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x02000004 RID: 4
	public class UserStoreMapData
	{
		// Token: 0x04000022 RID: 34
		[XmlAttribute("SourceDir")]
		public string SourceDir;

		// Token: 0x04000023 RID: 35
		[XmlAttribute("UserStoreDir")]
		public string UserStoreDir;
	}
}
