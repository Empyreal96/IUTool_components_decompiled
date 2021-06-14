using System;

namespace Microsoft.Diagnostics.Telemetry
{
	// Token: 0x02000007 RID: 7
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
	internal sealed class EventProviderAttribute : Attribute
	{
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600002C RID: 44 RVA: 0x000028B7 File Offset: 0x00000AB7
		// (set) Token: 0x0600002D RID: 45 RVA: 0x000028BF File Offset: 0x00000ABF
		public string Provider { get; private set; }

		// Token: 0x0600002E RID: 46 RVA: 0x000028C8 File Offset: 0x00000AC8
		public EventProviderAttribute(string providerName)
		{
			this.Provider = providerName;
		}

		// Token: 0x04000016 RID: 22
		public const string TelemetryGroupId = "{4f50731a-89cf-4782-b3e0-dce8c90476ba}";
	}
}
