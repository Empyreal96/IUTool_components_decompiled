using System;
using System.Collections.Generic;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000025 RID: 37
	public class PackageDeployerOutput
	{
		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060001A9 RID: 425 RVA: 0x0000A1EA File Offset: 0x000083EA
		// (set) Token: 0x060001AA RID: 426 RVA: 0x0000A1F2 File Offset: 0x000083F2
		public bool Success { get; set; }

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060001AB RID: 427 RVA: 0x0000A1FB File Offset: 0x000083FB
		// (set) Token: 0x060001AC RID: 428 RVA: 0x0000A203 File Offset: 0x00008403
		public string ErrorMessage { get; set; }

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060001AD RID: 429 RVA: 0x0000A20C File Offset: 0x0000840C
		// (set) Token: 0x060001AE RID: 430 RVA: 0x0000A214 File Offset: 0x00008414
		public List<ConfigCommand> ConfigurationCommands { get; set; }
	}
}
