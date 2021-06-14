using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x02000005 RID: 5
	public class SupportedLangs
	{
		// Token: 0x04000024 RID: 36
		[XmlArrayItem(ElementName = "Language", Type = typeof(string), IsNullable = false)]
		[XmlArray]
		[DefaultValue(null)]
		public List<string> UserInterface;

		// Token: 0x04000025 RID: 37
		[XmlArrayItem(ElementName = "Language", Type = typeof(string), IsNullable = true)]
		[XmlArray]
		[DefaultValue(null)]
		public List<string> Keyboard;

		// Token: 0x04000026 RID: 38
		[XmlArrayItem(ElementName = "Language", Type = typeof(string), IsNullable = true)]
		[XmlArray]
		[DefaultValue(null)]
		public List<string> Speech;
	}
}
