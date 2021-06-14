using System;

namespace Microsoft.Phone.Tools.MtbfReportGenerator
{
	// Token: 0x02000005 RID: 5
	public class MtbfMixCommand
	{
		// Token: 0x06000011 RID: 17 RVA: 0x0000217A File Offset: 0x0000037A
		public MtbfMixCommand()
		{
			this.SectionNumber = string.Empty;
			this.ExpectedCount = 0;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002194 File Offset: 0x00000394
		public MtbfMixCommand(string sectionNumber, int expectedCount)
		{
			this.SectionNumber = sectionNumber;
			this.ExpectedCount = expectedCount;
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000013 RID: 19 RVA: 0x000021AA File Offset: 0x000003AA
		// (set) Token: 0x06000014 RID: 20 RVA: 0x000021B2 File Offset: 0x000003B2
		public string SectionNumber { get; private set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000015 RID: 21 RVA: 0x000021BB File Offset: 0x000003BB
		// (set) Token: 0x06000016 RID: 22 RVA: 0x000021C3 File Offset: 0x000003C3
		public int ExpectedCount { get; private set; }
	}
}
