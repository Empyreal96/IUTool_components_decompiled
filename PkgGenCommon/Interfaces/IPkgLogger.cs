using System;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces
{
	// Token: 0x02000023 RID: 35
	public interface IPkgLogger
	{
		// Token: 0x06000082 RID: 130
		void Error(string format, params object[] args);

		// Token: 0x06000083 RID: 131
		void Warning(string format, params object[] args);

		// Token: 0x06000084 RID: 132
		void Message(string format, params object[] args);

		// Token: 0x06000085 RID: 133
		void Diagnostic(string format, params object[] args);
	}
}
