using System;
using Microsoft.CompPlat.PkgBldr.Tools;

namespace Microsoft.CompPlat.PkgBldr.Base
{
	// Token: 0x02000033 RID: 51
	public class PkgGenException : IUException
	{
		// Token: 0x060000D6 RID: 214 RVA: 0x00007AE8 File Offset: 0x00005CE8
		public PkgGenException(string msg) : base(msg)
		{
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00007AF1 File Offset: 0x00005CF1
		public PkgGenException(string msg, params object[] args) : base(msg, args)
		{
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00007AFB File Offset: 0x00005CFB
		public PkgGenException(Exception innerException, string msg, params object[] args) : base(innerException, msg, args)
		{
		}
	}
}
