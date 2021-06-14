using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces
{
	// Token: 0x02000026 RID: 38
	public interface IPkgProject
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600008D RID: 141
		string TempDirectory { get; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600008E RID: 142
		IPkgLogger Log { get; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600008F RID: 143
		IMacroResolver MacroResolver { get; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000090 RID: 144
		IDictionary<string, string> Attributes { get; }

		// Token: 0x06000091 RID: 145
		IEnumerable<SatelliteId> GetSatelliteValues(SatelliteType type);

		// Token: 0x06000092 RID: 146
		void AddToCapabilities(XElement element);

		// Token: 0x06000093 RID: 147
		void AddToAuthorization(XElement element);
	}
}
