using System;

namespace Microsoft.Diagnostics.Telemetry
{
	// Token: 0x02000009 RID: 9
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
	internal sealed class EventProviderAttribute : Attribute
	{
		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000041 RID: 65 RVA: 0x00002FB9 File Offset: 0x000011B9
		// (set) Token: 0x06000042 RID: 66 RVA: 0x00002FC1 File Offset: 0x000011C1
		public string Provider { get; private set; }

		// Token: 0x06000043 RID: 67 RVA: 0x00002FCA File Offset: 0x000011CA
		public EventProviderAttribute(string providerName)
		{
			this.Provider = providerName;
		}

		// Token: 0x04000023 RID: 35
		public const string TelemetryGroupId = "{4f50731a-89cf-4782-b3e0-dce8c90476ba}";
	}
}
