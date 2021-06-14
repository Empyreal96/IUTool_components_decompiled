using System;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000007 RID: 7
	[CLSCompliant(false)]
	public enum ApplicationTypeValue : uint
	{
		// Token: 0x04000095 RID: 149
		FirmwareBootManager = 1U,
		// Token: 0x04000096 RID: 150
		WindowsBootManager,
		// Token: 0x04000097 RID: 151
		WindowsBootLoader,
		// Token: 0x04000098 RID: 152
		WindowsResumeApplication,
		// Token: 0x04000099 RID: 153
		MemoryTester,
		// Token: 0x0400009A RID: 154
		LegacyNtLdr,
		// Token: 0x0400009B RID: 155
		LegacySetupLdr,
		// Token: 0x0400009C RID: 156
		BootSector,
		// Token: 0x0400009D RID: 157
		StartupModule,
		// Token: 0x0400009E RID: 158
		GenericApplication,
		// Token: 0x0400009F RID: 159
		Reserved = 1048575U
	}
}
