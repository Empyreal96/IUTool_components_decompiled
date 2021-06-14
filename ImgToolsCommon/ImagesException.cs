using System;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Images
{
	// Token: 0x02000009 RID: 9
	public class ImagesException : IUException
	{
		// Token: 0x06000074 RID: 116 RVA: 0x00006573 File Offset: 0x00004773
		public ImagesException(string msg) : base(msg)
		{
		}

		// Token: 0x06000075 RID: 117 RVA: 0x0000657C File Offset: 0x0000477C
		public ImagesException(string message, params object[] args) : base(message, args)
		{
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00006586 File Offset: 0x00004786
		public ImagesException(string msg, Exception inner) : base(msg, inner)
		{
		}
	}
}
