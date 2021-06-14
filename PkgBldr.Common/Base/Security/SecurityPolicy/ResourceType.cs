using System;

namespace Microsoft.CompPlat.PkgBldr.Base.Security.SecurityPolicy
{
	// Token: 0x0200004A RID: 74
	public enum ResourceType
	{
		// Token: 0x040000DA RID: 218
		File,
		// Token: 0x040000DB RID: 219
		Directory,
		// Token: 0x040000DC RID: 220
		Registry,
		// Token: 0x040000DD RID: 221
		TransientObject,
		// Token: 0x040000DE RID: 222
		ServiceAccess,
		// Token: 0x040000DF RID: 223
		ComLaunch,
		// Token: 0x040000E0 RID: 224
		ComAccess,
		// Token: 0x040000E1 RID: 225
		WinRt,
		// Token: 0x040000E2 RID: 226
		EtwProvider,
		// Token: 0x040000E3 RID: 227
		Wnf,
		// Token: 0x040000E4 RID: 228
		SdReg,
		// Token: 0x040000E5 RID: 229
		Driver
	}
}
