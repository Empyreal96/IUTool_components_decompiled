using System;

namespace Microsoft.Diagnostics.Telemetry
{
	// Token: 0x02000006 RID: 6
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
	internal sealed class EventProviderAttribute : Attribute
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000024 RID: 36 RVA: 0x0000267A File Offset: 0x0000087A
		// (set) Token: 0x06000025 RID: 37 RVA: 0x00002682 File Offset: 0x00000882
		public string Provider { get; private set; }

		// Token: 0x06000026 RID: 38 RVA: 0x0000268B File Offset: 0x0000088B
		public EventProviderAttribute(string providerName)
		{
			this.Provider = providerName;
		}

		// Token: 0x04000012 RID: 18
		public const string TelemetryGroupId = "{4f50731a-89cf-4782-b3e0-dce8c90476ba}";
	}
}
