using System;
using System.Reflection.Mock;

namespace System.Reflection.Adds
{
	// Token: 0x02000011 RID: 17
	internal interface ITypeSignatureBlob : ITypeProxy
	{
		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000082 RID: 130
		byte[] Blob { get; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000083 RID: 131
		Module DeclaringScope { get; }
	}
}
