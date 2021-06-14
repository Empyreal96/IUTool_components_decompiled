using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x0200004C RID: 76
	public enum RegValueType
	{
		// Token: 0x04000100 RID: 256
		[XmlEnum(Name = "REG_SZ")]
		String,
		// Token: 0x04000101 RID: 257
		[XmlEnum(Name = "REG_EXPAND_SZ")]
		ExpandString,
		// Token: 0x04000102 RID: 258
		[XmlEnum(Name = "REG_BINARY")]
		Binary,
		// Token: 0x04000103 RID: 259
		[XmlEnum(Name = "REG_DWORD")]
		DWord,
		// Token: 0x04000104 RID: 260
		[XmlEnum(Name = "REG_MULTI_SZ")]
		MultiString,
		// Token: 0x04000105 RID: 261
		[XmlEnum(Name = "REG_QWORD")]
		QWord,
		// Token: 0x04000106 RID: 262
		[XmlEnum(Name = "REG_HEX")]
		Hex
	}
}
