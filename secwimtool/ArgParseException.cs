using System;

namespace SecureWim
{
	// Token: 0x02000002 RID: 2
	internal class ArgParseException : Exception
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public ArgParseException(string message, UsagePrinter.UsageDelegate printUsage) : base(message)
		{
			this.PrintUsage = printUsage;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00002060 File Offset: 0x00000260
		// (set) Token: 0x06000003 RID: 3 RVA: 0x00002068 File Offset: 0x00000268
		public UsagePrinter.UsageDelegate PrintUsage { get; private set; }
	}
}
