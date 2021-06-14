using System;
using System.Collections.Generic;

namespace Microsoft.WindowsPhone.ImageUpdate.Customization
{
	// Token: 0x02000007 RID: 7
	public class CustomizationError
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000070 RID: 112 RVA: 0x000056E9 File Offset: 0x000038E9
		// (set) Token: 0x06000071 RID: 113 RVA: 0x000056F1 File Offset: 0x000038F1
		public CustomizationErrorSeverity Severity { get; private set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000072 RID: 114 RVA: 0x000056FA File Offset: 0x000038FA
		// (set) Token: 0x06000073 RID: 115 RVA: 0x00005702 File Offset: 0x00003902
		public IEnumerable<IDefinedIn> FilesInvolved { get; private set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000074 RID: 116 RVA: 0x0000570B File Offset: 0x0000390B
		// (set) Token: 0x06000075 RID: 117 RVA: 0x00005713 File Offset: 0x00003913
		public string Message { get; private set; }

		// Token: 0x06000076 RID: 118 RVA: 0x0000571C File Offset: 0x0000391C
		public CustomizationError(CustomizationErrorSeverity severity, IEnumerable<IDefinedIn> filesInvolved, string format, params object[] args)
		{
			this.Severity = severity;
			this.FilesInvolved = filesInvolved;
			this.Message = string.Format(format, args);
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00005740 File Offset: 0x00003940
		public CustomizationError(CustomizationErrorSeverity severity, IEnumerable<IDefinedIn> filesInvolved, string message)
		{
			this.Severity = severity;
			this.FilesInvolved = filesInvolved;
			this.Message = message;
		}
	}
}
