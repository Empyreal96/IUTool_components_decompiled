using System;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents
{
	// Token: 0x02000042 RID: 66
	internal class PackageLogger : IPkgLogger
	{
		// Token: 0x06000105 RID: 261 RVA: 0x00005958 File Offset: 0x00003B58
		public void Error(string format, params object[] args)
		{
			LogUtil.Error(format, args);
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00005961 File Offset: 0x00003B61
		public void Warning(string format, params object[] args)
		{
			LogUtil.Warning(format, args);
		}

		// Token: 0x06000107 RID: 263 RVA: 0x0000596A File Offset: 0x00003B6A
		public void Message(string format, params object[] args)
		{
			LogUtil.Message(format, args);
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00005973 File Offset: 0x00003B73
		public void Diagnostic(string format, params object[] args)
		{
			LogUtil.Diagnostic(format, args);
		}
	}
}
