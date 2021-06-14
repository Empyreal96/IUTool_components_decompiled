using System;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents
{
	// Token: 0x02000052 RID: 82
	[Flags]
	public enum AuthenticationCapabitities
	{
		// Token: 0x04000125 RID: 293
		None = 0,
		// Token: 0x04000126 RID: 294
		MutualAuth = 1,
		// Token: 0x04000127 RID: 295
		StaticCloaking = 32,
		// Token: 0x04000128 RID: 296
		DynamicCloaking = 64,
		// Token: 0x04000129 RID: 297
		AnyAuthority = 128,
		// Token: 0x0400012A RID: 298
		MakeFullSIC = 256,
		// Token: 0x0400012B RID: 299
		Default = 2048,
		// Token: 0x0400012C RID: 300
		SecureRefs = 2,
		// Token: 0x0400012D RID: 301
		AccessControl = 4,
		// Token: 0x0400012E RID: 302
		AppId = 8,
		// Token: 0x0400012F RID: 303
		Dynamic = 16,
		// Token: 0x04000130 RID: 304
		RequireFullSIC = 512,
		// Token: 0x04000131 RID: 305
		AutoImpersonate = 1024,
		// Token: 0x04000132 RID: 306
		NoCustomMarshal = 8192,
		// Token: 0x04000133 RID: 307
		DisableAAA = 4096
	}
}
