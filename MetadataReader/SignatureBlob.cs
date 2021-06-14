using System;
using System.Reflection.Adds;

namespace Microsoft.MetadataReader
{
	// Token: 0x0200003E RID: 62
	internal class SignatureBlob
	{
		// Token: 0x06000466 RID: 1126 RVA: 0x0000EAEB File Offset: 0x0000CCEB
		private SignatureBlob(byte[] data)
		{
			this._signature = data;
		}

		// Token: 0x06000467 RID: 1127 RVA: 0x0000EAFC File Offset: 0x0000CCFC
		public static SignatureBlob ReadSignature(MetadataFile storage, EmbeddedBlobPointer pointer, int countBytes)
		{
			return new SignatureBlob(storage.ReadEmbeddedBlob(pointer, countBytes));
		}

		// Token: 0x06000468 RID: 1128 RVA: 0x0000EB1C File Offset: 0x0000CD1C
		public byte[] GetSignatureAsByteArray()
		{
			return this._signature;
		}

		// Token: 0x040000DB RID: 219
		private readonly byte[] _signature;
	}
}
