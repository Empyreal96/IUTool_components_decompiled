using System;

namespace Microsoft.TestInfra.UtilityLibrary
{
	// Token: 0x0200002B RID: 43
	public class TypicalException<T> : Exception
	{
		// Token: 0x060000CA RID: 202 RVA: 0x00005E31 File Offset: 0x00004031
		public TypicalException()
		{
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00005E3C File Offset: 0x0000403C
		public TypicalException(string message) : base(message)
		{
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00005E48 File Offset: 0x00004048
		public TypicalException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
