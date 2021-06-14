using System;
using System.Collections.Generic;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib.Interfaces
{
	// Token: 0x02000047 RID: 71
	public interface IInboxAppToPkgObjectsMappingStrategy
	{
		// Token: 0x060000F9 RID: 249
		List<PkgObject> Map(IInboxAppPackage appPackage, IPkgProject packageGenerator, OSComponentBuilder osComponent);
	}
}
