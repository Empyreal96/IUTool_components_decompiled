using System;
using System.Diagnostics.CodeAnalysis;

namespace System.Reflection.Adds
{
	// Token: 0x0200001E RID: 30
	public enum MetadataTable
	{
		// Token: 0x04000067 RID: 103
		Module,
		// Token: 0x04000068 RID: 104
		TypeRef,
		// Token: 0x04000069 RID: 105
		TypeDef,
		// Token: 0x0400006A RID: 106
		FieldDef = 4,
		// Token: 0x0400006B RID: 107
		MethodDef = 6,
		// Token: 0x0400006C RID: 108
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param")]
		ParamDef = 8,
		// Token: 0x0400006D RID: 109
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Impl")]
		[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
		InterfaceImpl,
		// Token: 0x0400006E RID: 110
		MemberRef,
		// Token: 0x0400006F RID: 111
		CustomAttribute = 12,
		// Token: 0x04000070 RID: 112
		Permission = 14,
		// Token: 0x04000071 RID: 113
		Signature = 17,
		// Token: 0x04000072 RID: 114
		Event = 20,
		// Token: 0x04000073 RID: 115
		Property = 23,
		// Token: 0x04000074 RID: 116
		ModuleRef = 26,
		// Token: 0x04000075 RID: 117
		TypeSpec,
		// Token: 0x04000076 RID: 118
		Assembly = 32,
		// Token: 0x04000077 RID: 119
		AssemblyRef = 35,
		// Token: 0x04000078 RID: 120
		File = 38,
		// Token: 0x04000079 RID: 121
		ExportedType,
		// Token: 0x0400007A RID: 122
		ManifestResource,
		// Token: 0x0400007B RID: 123
		GenericPar = 42,
		// Token: 0x0400007C RID: 124
		MethodSpec
	}
}
