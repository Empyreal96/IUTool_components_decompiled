using System;

namespace Microsoft.SecureBoot
{
	// Token: 0x02000006 RID: 6
	internal class SignInfoEntry
	{
		// Token: 0x0600001C RID: 28 RVA: 0x0000254A File Offset: 0x0000074A
		public SignInfoEntry(byte[] binaryIDHash, string filename)
		{
			this.BinaryIDHash = binaryIDHash;
			this.SignInfoFilename = filename;
		}

		// Token: 0x0400000E RID: 14
		public byte[] BinaryIDHash;

		// Token: 0x0400000F RID: 15
		public string SignInfoFilename;
	}
}
