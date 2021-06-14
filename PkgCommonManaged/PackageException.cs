using System;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000019 RID: 25
	public class PackageException : IUException
	{
		// Token: 0x06000158 RID: 344 RVA: 0x00007654 File Offset: 0x00005854
		public PackageException(string msg) : base(msg)
		{
		}

		// Token: 0x06000159 RID: 345 RVA: 0x0000765D File Offset: 0x0000585D
		public PackageException(string message, params object[] args) : base(message, args)
		{
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00007667 File Offset: 0x00005867
		public PackageException(Exception inner, string msg) : base(inner, msg)
		{
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00007671 File Offset: 0x00005871
		public PackageException(Exception innerException, string message, params object[] args) : base(innerException, message, args)
		{
		}
	}
}
