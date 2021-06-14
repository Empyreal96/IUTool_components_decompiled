using System;

namespace Microsoft.Phone.Test.TestMetadata.Helper
{
	// Token: 0x02000009 RID: 9
	internal struct ImageNtHeaders
	{
		// Token: 0x04000027 RID: 39
		public uint Signature;

		// Token: 0x04000028 RID: 40
		public ImageFileHeader FileHeader;

		// Token: 0x04000029 RID: 41
		public ImageOptionalHeader32 OptionalHeader;
	}
}
