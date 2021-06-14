using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000025 RID: 37
	public class InputStringRule : InputRule
	{
		// Token: 0x04000100 RID: 256
		[XmlArrayItem(ElementName = "Value", Type = typeof(string), IsNullable = false)]
		[XmlArray("List")]
		public string[] Values;
	}
}
