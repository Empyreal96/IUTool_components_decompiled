using System;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x0200001F RID: 31
	public enum FileType
	{
		// Token: 0x0400005C RID: 92
		Invalid,
		// Token: 0x0400005D RID: 93
		Regular,
		// Token: 0x0400005E RID: 94
		Registry,
		// Token: 0x0400005F RID: 95
		SecurityPolicy,
		// Token: 0x04000060 RID: 96
		Reserved,
		// Token: 0x04000061 RID: 97
		BinaryPartition,
		// Token: 0x04000062 RID: 98
		Manifest,
		// Token: 0x04000063 RID: 99
		RegistryMultiStringAppend,
		// Token: 0x04000064 RID: 100
		Certificates,
		// Token: 0x04000065 RID: 101
		Catalog,
		// Token: 0x04000066 RID: 102
		DirectoryBackupMetadata,
		// Token: 0x04000067 RID: 103
		FileBackupMetadata
	}
}
