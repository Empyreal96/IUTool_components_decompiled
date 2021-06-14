using System;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000013 RID: 19
	public enum RegistryValueType : uint
	{
		// Token: 0x0400002D RID: 45
		None,
		// Token: 0x0400002E RID: 46
		String,
		// Token: 0x0400002F RID: 47
		ExpandString,
		// Token: 0x04000030 RID: 48
		Binary,
		// Token: 0x04000031 RID: 49
		DWord,
		// Token: 0x04000032 RID: 50
		DWordBigEndian,
		// Token: 0x04000033 RID: 51
		Link,
		// Token: 0x04000034 RID: 52
		MultiString,
		// Token: 0x04000035 RID: 53
		RegResourceList,
		// Token: 0x04000036 RID: 54
		RegFullResourceDescriptor,
		// Token: 0x04000037 RID: 55
		RegResourceRequirementsList,
		// Token: 0x04000038 RID: 56
		QWord
	}
}
