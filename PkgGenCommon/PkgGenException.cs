using System;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon
{
	// Token: 0x0200000D RID: 13
	public class PkgGenException : IUException
	{
		// Token: 0x06000032 RID: 50 RVA: 0x00002596 File Offset: 0x00000796
		public PkgGenException(string msg, params object[] args) : base(msg, args)
		{
		}

		// Token: 0x06000033 RID: 51 RVA: 0x000025A0 File Offset: 0x000007A0
		public PkgGenException(Exception innerException, string msg, params object[] args) : base(innerException, msg, args)
		{
		}
	}
}
