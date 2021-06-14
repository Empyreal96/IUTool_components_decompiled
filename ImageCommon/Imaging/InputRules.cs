using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000022 RID: 34
	public class InputRules
	{
		// Token: 0x040000F9 RID: 249
		[XmlArrayItem(ElementName = "IntegerRule", Type = typeof(InputIntegerRule), IsNullable = false)]
		[XmlArray]
		public InputIntegerRule[] IntegerRules;

		// Token: 0x040000FA RID: 250
		[XmlArrayItem(ElementName = "StringRule", Type = typeof(InputStringRule), IsNullable = false)]
		[XmlArray]
		public InputStringRule[] StringRules;
	}
}
