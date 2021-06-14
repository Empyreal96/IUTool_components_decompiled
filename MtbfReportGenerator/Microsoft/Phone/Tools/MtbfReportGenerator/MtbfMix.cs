using System;
using System.Collections.Generic;

namespace Microsoft.Phone.Tools.MtbfReportGenerator
{
	// Token: 0x02000003 RID: 3
	public class MtbfMix
	{
		// Token: 0x06000005 RID: 5 RVA: 0x00002075 File Offset: 0x00000275
		public MtbfMix()
		{
			this.CommandGroups = new List<MtbfMixGroup>();
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000006 RID: 6 RVA: 0x00002088 File Offset: 0x00000288
		// (set) Token: 0x06000007 RID: 7 RVA: 0x00002090 File Offset: 0x00000290
		public List<MtbfMixGroup> CommandGroups { get; private set; }

		// Token: 0x06000008 RID: 8 RVA: 0x0000209C File Offset: 0x0000029C
		public MtbfMixGroup GetGroup(string groupName)
		{
			return this.CommandGroups.Find((MtbfMixGroup item) => item.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase));
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000020D0 File Offset: 0x000002D0
		public MtbfMixGroup AddGroup(string groupName)
		{
			MtbfMixGroup mtbfMixGroup = this.GetGroup(groupName);
			if (mtbfMixGroup == null)
			{
				mtbfMixGroup = new MtbfMixGroup(groupName);
				this.CommandGroups.Add(mtbfMixGroup);
			}
			return mtbfMixGroup;
		}
	}
}
