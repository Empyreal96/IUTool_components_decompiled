using System;
using System.Collections.Generic;

namespace Microsoft.Phone.Tools.MtbfReportGenerator
{
	// Token: 0x02000004 RID: 4
	public class MtbfMixGroup
	{
		// Token: 0x0600000A RID: 10 RVA: 0x000020FC File Offset: 0x000002FC
		public MtbfMixGroup()
		{
			this.Commands = new List<MtbfMixCommand>();
			this.Name = string.Empty;
		}

		// Token: 0x0600000B RID: 11 RVA: 0x0000211A File Offset: 0x0000031A
		public MtbfMixGroup(string name)
		{
			this.Commands = new List<MtbfMixCommand>();
			this.Name = name;
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000C RID: 12 RVA: 0x00002134 File Offset: 0x00000334
		// (set) Token: 0x0600000D RID: 13 RVA: 0x0000213C File Offset: 0x0000033C
		public string Name { get; private set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000E RID: 14 RVA: 0x00002145 File Offset: 0x00000345
		// (set) Token: 0x0600000F RID: 15 RVA: 0x0000214D File Offset: 0x0000034D
		public List<MtbfMixCommand> Commands { get; private set; }

		// Token: 0x06000010 RID: 16 RVA: 0x00002158 File Offset: 0x00000358
		public MtbfMixCommand AddCommand(string sectionNumber, int expectedCount)
		{
			MtbfMixCommand mtbfMixCommand = new MtbfMixCommand(sectionNumber, expectedCount);
			this.Commands.Add(mtbfMixCommand);
			return mtbfMixCommand;
		}
	}
}
