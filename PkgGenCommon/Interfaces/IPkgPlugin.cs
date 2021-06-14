using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces
{
	// Token: 0x02000024 RID: 36
	public interface IPkgPlugin
	{
		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000086 RID: 134
		string Name { get; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000087 RID: 135
		string XmlElementName { get; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000088 RID: 136
		string XmlElementUniqueXPath { get; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000089 RID: 137
		string XmlSchemaPath { get; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600008A RID: 138
		bool UseSecurityCompilerPassthrough { get; }

		// Token: 0x0600008B RID: 139
		void ValidateEntries(IPkgProject packageGenerator, IEnumerable<XElement> componentEntries);

		// Token: 0x0600008C RID: 140
		IEnumerable<PkgObject> ProcessEntries(IPkgProject packageGenerator, IEnumerable<XElement> componentEntries);
	}
}
