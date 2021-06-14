using System;

namespace SecureWim
{
	// Token: 0x0200000B RID: 11
	internal class UsagePrinter : IToolCommand
	{
		// Token: 0x06000026 RID: 38 RVA: 0x00002E65 File Offset: 0x00001065
		public UsagePrinter(UsagePrinter.UsageDelegate usageToPrint)
		{
			this.printUsage = usageToPrint;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002E74 File Offset: 0x00001074
		public UsagePrinter(string failureMessage, UsagePrinter.UsageDelegate usageToPrint)
		{
			this.printUsage = usageToPrint;
			this.failureMessage = failureMessage;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002E8A File Offset: 0x0000108A
		public int Run()
		{
			if (this.failureMessage != null)
			{
				Console.WriteLine(this.failureMessage);
			}
			this.printUsage();
			if (this.failureMessage != null)
			{
				return -1;
			}
			return 0;
		}

		// Token: 0x0400000D RID: 13
		private UsagePrinter.UsageDelegate printUsage;

		// Token: 0x0400000E RID: 14
		private string failureMessage;

		// Token: 0x02000010 RID: 16
		// (Invoke) Token: 0x06000039 RID: 57
		public delegate void UsageDelegate();
	}
}
