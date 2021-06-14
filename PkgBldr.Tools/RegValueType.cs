using System;
using System.Xml.Serialization;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x0200001A RID: 26
	public enum RegValueType
	{
		// Token: 0x04000048 RID: 72
		[XmlEnum(Name = "REG_SZ")]
		String,
		// Token: 0x04000049 RID: 73
		[XmlEnum(Name = "REG_EXPAND_SZ")]
		ExpandString,
		// Token: 0x0400004A RID: 74
		[XmlEnum(Name = "REG_BINARY")]
		Binary,
		// Token: 0x0400004B RID: 75
		[XmlEnum(Name = "REG_DWORD")]
		DWord,
		// Token: 0x0400004C RID: 76
		[XmlEnum(Name = "REG_MULTI_SZ")]
		MultiString,
		// Token: 0x0400004D RID: 77
		[XmlEnum(Name = "REG_QWORD")]
		QWord,
		// Token: 0x0400004E RID: 78
		[XmlEnum(Name = "REG_HEX")]
		Hex
	}
}
