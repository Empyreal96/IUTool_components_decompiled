using System;
using System.Diagnostics.CodeAnalysis;

namespace System.Reflection.Adds
{
	// Token: 0x0200001D RID: 29
	public enum TokenType
	{
		// Token: 0x0400004C RID: 76
		Module,
		// Token: 0x0400004D RID: 77
		TypeRef = 16777216,
		// Token: 0x0400004E RID: 78
		TypeDef = 33554432,
		// Token: 0x0400004F RID: 79
		FieldDef = 67108864,
		// Token: 0x04000050 RID: 80
		MethodDef = 100663296,
		// Token: 0x04000051 RID: 81
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param")]
		ParamDef = 134217728,
		// Token: 0x04000052 RID: 82
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Impl")]
		[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
		InterfaceImpl = 150994944,
		// Token: 0x04000053 RID: 83
		MemberRef = 167772160,
		// Token: 0x04000054 RID: 84
		CustomAttribute = 201326592,
		// Token: 0x04000055 RID: 85
		Permission = 234881024,
		// Token: 0x04000056 RID: 86
		Signature = 285212672,
		// Token: 0x04000057 RID: 87
		Event = 335544320,
		// Token: 0x04000058 RID: 88
		Property = 385875968,
		// Token: 0x04000059 RID: 89
		ModuleRef = 436207616,
		// Token: 0x0400005A RID: 90
		TypeSpec = 452984832,
		// Token: 0x0400005B RID: 91
		Assembly = 536870912,
		// Token: 0x0400005C RID: 92
		AssemblyRef = 587202560,
		// Token: 0x0400005D RID: 93
		File = 637534208,
		// Token: 0x0400005E RID: 94
		ExportedType = 654311424,
		// Token: 0x0400005F RID: 95
		ManifestResource = 671088640,
		// Token: 0x04000060 RID: 96
		GenericPar = 704643072,
		// Token: 0x04000061 RID: 97
		MethodSpec = 721420288,
		// Token: 0x04000062 RID: 98
		String = 1879048192,
		// Token: 0x04000063 RID: 99
		Name = 1895825408,
		// Token: 0x04000064 RID: 100
		BaseType = 1912602624,
		// Token: 0x04000065 RID: 101
		Invalid = 2147483647
	}
}
