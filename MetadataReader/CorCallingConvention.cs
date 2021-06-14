using System;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000011 RID: 17
	internal enum CorCallingConvention
	{
		// Token: 0x0400000F RID: 15
		Default,
		// Token: 0x04000010 RID: 16
		VarArg = 5,
		// Token: 0x04000011 RID: 17
		Field,
		// Token: 0x04000012 RID: 18
		LocalSig,
		// Token: 0x04000013 RID: 19
		Property,
		// Token: 0x04000014 RID: 20
		Unmanaged,
		// Token: 0x04000015 RID: 21
		GenericInst,
		// Token: 0x04000016 RID: 22
		NativeVarArg,
		// Token: 0x04000017 RID: 23
		Mask = 15,
		// Token: 0x04000018 RID: 24
		HasThis = 32,
		// Token: 0x04000019 RID: 25
		ExplicitThis = 64,
		// Token: 0x0400001A RID: 26
		Generic = 16
	}
}
