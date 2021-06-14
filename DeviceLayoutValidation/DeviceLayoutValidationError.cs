using System;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000002 RID: 2
	public enum DeviceLayoutValidationError
	{
		// Token: 0x04000002 RID: 2
		Pass,
		// Token: 0x04000003 RID: 3
		UnknownInternalError,
		// Token: 0x04000004 RID: 4
		DeviceLayoutNotMsOwned,
		// Token: 0x04000005 RID: 5
		DeviceLayoutNotOEMOwned,
		// Token: 0x04000006 RID: 6
		DeviceLayoutNotProductionSigned,
		// Token: 0x04000007 RID: 7
		DeviceLayoutValidationManifestNotProductionSigned,
		// Token: 0x04000008 RID: 8
		DeviceLayoutAttributeMismatch,
		// Token: 0x04000009 RID: 9
		PartitionNotFound,
		// Token: 0x0400000A RID: 10
		PartitionPositionMismatch,
		// Token: 0x0400000B RID: 11
		PartitionAttributeValueMismatch,
		// Token: 0x0400000C RID: 12
		BackupPartitionNotFound,
		// Token: 0x0400000D RID: 13
		BackupPartitionSizeMismatch,
		// Token: 0x0400000E RID: 14
		PartitionInvalidName
	}
}
