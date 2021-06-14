using System;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000014 RID: 20
	[Flags]
	internal enum CorTypeAttr
	{
		// Token: 0x04000023 RID: 35
		tdVisibilityMask = 7,
		// Token: 0x04000024 RID: 36
		tdNotPublic = 0,
		// Token: 0x04000025 RID: 37
		tdPublic = 1,
		// Token: 0x04000026 RID: 38
		tdNestedPublic = 2,
		// Token: 0x04000027 RID: 39
		tdNestedPrivate = 3,
		// Token: 0x04000028 RID: 40
		tdNestedFamily = 4,
		// Token: 0x04000029 RID: 41
		tdNestedAssembly = 5,
		// Token: 0x0400002A RID: 42
		tdNestedFamANDAssem = 6,
		// Token: 0x0400002B RID: 43
		tdNestedFamORAssem = 7,
		// Token: 0x0400002C RID: 44
		tdLayoutMask = 24,
		// Token: 0x0400002D RID: 45
		tdAutoLayout = 0,
		// Token: 0x0400002E RID: 46
		tdSequentialLayout = 8,
		// Token: 0x0400002F RID: 47
		tdExplicitLayout = 16,
		// Token: 0x04000030 RID: 48
		tdClassSemanticsMask = 32,
		// Token: 0x04000031 RID: 49
		tdClass = 0,
		// Token: 0x04000032 RID: 50
		tdInterface = 32,
		// Token: 0x04000033 RID: 51
		tdAbstract = 128,
		// Token: 0x04000034 RID: 52
		tdSealed = 256,
		// Token: 0x04000035 RID: 53
		tdSpecialName = 1024,
		// Token: 0x04000036 RID: 54
		tdImport = 4096,
		// Token: 0x04000037 RID: 55
		tdSerializable = 8192,
		// Token: 0x04000038 RID: 56
		tdStringFormatMask = 196608,
		// Token: 0x04000039 RID: 57
		tdAnsiClass = 0,
		// Token: 0x0400003A RID: 58
		tdUnicodeClass = 65536,
		// Token: 0x0400003B RID: 59
		tdAutoClass = 131072,
		// Token: 0x0400003C RID: 60
		tdCustomFormatClass = 196608,
		// Token: 0x0400003D RID: 61
		tdCustomFormatMask = 12582912,
		// Token: 0x0400003E RID: 62
		tdBeforeFieldInit = 1048576,
		// Token: 0x0400003F RID: 63
		tdForwarder = 2097152,
		// Token: 0x04000040 RID: 64
		tdReservedMask = 264192,
		// Token: 0x04000041 RID: 65
		tdRTSpecialName = 2048,
		// Token: 0x04000042 RID: 66
		tdHasSecurity = 262144
	}
}
