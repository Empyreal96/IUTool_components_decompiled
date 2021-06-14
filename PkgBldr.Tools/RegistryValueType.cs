using System;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000014 RID: 20
	public enum RegistryValueType : uint
	{
		// Token: 0x0400002E RID: 46
		None,
		// Token: 0x0400002F RID: 47
		String,
		// Token: 0x04000030 RID: 48
		ExpandString,
		// Token: 0x04000031 RID: 49
		Binary,
		// Token: 0x04000032 RID: 50
		DWord,
		// Token: 0x04000033 RID: 51
		DWordBigEndian,
		// Token: 0x04000034 RID: 52
		Link,
		// Token: 0x04000035 RID: 53
		MultiString,
		// Token: 0x04000036 RID: 54
		RegResourceList,
		// Token: 0x04000037 RID: 55
		RegFullResourceDescriptor,
		// Token: 0x04000038 RID: 56
		RegResourceRequirementsList,
		// Token: 0x04000039 RID: 57
		QWord
	}
}
