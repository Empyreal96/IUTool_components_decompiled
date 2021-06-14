using System;

namespace Microsoft.MobileDevices.TestInfrastructure.UtilityLibrary.CommandLine
{
	// Token: 0x02000002 RID: 2
	public interface IArgumentHolder
	{
		// Token: 0x06000001 RID: 1
		string GetUsageString();

		// Token: 0x06000002 RID: 2
		void ValidateArguments();
	}
}
