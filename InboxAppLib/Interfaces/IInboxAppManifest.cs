using System;
using System.Collections.Generic;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib.Interfaces
{
	// Token: 0x02000045 RID: 69
	public interface IInboxAppManifest
	{
		// Token: 0x060000EC RID: 236
		void ReadManifest();

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000ED RID: 237
		string Filename { get; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000EE RID: 238
		string Title { get; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000EF RID: 239
		string Description { get; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000F0 RID: 240
		string Publisher { get; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000F1 RID: 241
		List<string> Capabilities { get; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000F2 RID: 242
		string ProductID { get; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000F3 RID: 243
		string Version { get; }
	}
}
