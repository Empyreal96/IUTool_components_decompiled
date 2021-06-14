using System;
using System.Collections.Generic;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib.Interfaces
{
	// Token: 0x02000046 RID: 70
	public interface IInboxAppPackage
	{
		// Token: 0x060000F4 RID: 244
		void OpenPackage();

		// Token: 0x060000F5 RID: 245
		List<string> GetCapabilities();

		// Token: 0x060000F6 RID: 246
		IInboxAppManifest GetManifest();

		// Token: 0x060000F7 RID: 247
		IInboxAppToPkgObjectsMappingStrategy GetPkgObjectsMappingStrategy();

		// Token: 0x060000F8 RID: 248
		string GetInstallDestinationPath(bool isTopLevelPackage);
	}
}
