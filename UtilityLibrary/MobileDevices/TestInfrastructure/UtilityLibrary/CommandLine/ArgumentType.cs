using System;

namespace Microsoft.MobileDevices.TestInfrastructure.UtilityLibrary.CommandLine
{
	// Token: 0x02000003 RID: 3
	[Flags]
	public enum ArgumentType
	{
		// Token: 0x04000002 RID: 2
		Required = 1,
		// Token: 0x04000003 RID: 3
		Unique = 2,
		// Token: 0x04000004 RID: 4
		Multiple = 4,
		// Token: 0x04000005 RID: 5
		Hidden = 8,
		// Token: 0x04000006 RID: 6
		AtMostOnce = 0,
		// Token: 0x04000007 RID: 7
		LastOccurenceWins = 4,
		// Token: 0x04000008 RID: 8
		MultipleUnique = 6
	}
}
