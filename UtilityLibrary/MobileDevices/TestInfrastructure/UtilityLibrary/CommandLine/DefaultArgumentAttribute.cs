using System;

namespace Microsoft.MobileDevices.TestInfrastructure.UtilityLibrary.CommandLine
{
	// Token: 0x02000005 RID: 5
	[AttributeUsage(AttributeTargets.Field)]
	public class DefaultArgumentAttribute : ArgumentAttribute
	{
		// Token: 0x06000011 RID: 17 RVA: 0x00002202 File Offset: 0x00000402
		public DefaultArgumentAttribute(ArgumentType type) : base(type)
		{
		}
	}
}
