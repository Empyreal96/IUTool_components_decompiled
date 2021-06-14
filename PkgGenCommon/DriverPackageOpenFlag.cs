using System;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon
{
	// Token: 0x0200001B RID: 27
	[Flags]
	internal enum DriverPackageOpenFlag : uint
	{
		// Token: 0x040000A7 RID: 167
		VersionOnly = 1U,
		// Token: 0x040000A8 RID: 168
		FilesOnly = 2U,
		// Token: 0x040000A9 RID: 169
		DefaultLanguage = 4U,
		// Token: 0x040000AA RID: 170
		LocalizableStrings = 8U,
		// Token: 0x040000AB RID: 171
		TargetOSVersion = 16U,
		// Token: 0x040000AC RID: 172
		StrictValidation = 32U,
		// Token: 0x040000AD RID: 173
		ClassSchemaOnly = 64U
	}
}
