using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.CompPlat.PkgBldr.Base;

namespace Microsoft.CompPlat.PkgBldr.Interfaces
{
	// Token: 0x02000002 RID: 2
	public interface IPkgPlugin
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1
		string Name { get; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000002 RID: 2
		string XmlElementName { get; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000003 RID: 3
		string XmlElementUniqueXPath { get; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000004 RID: 4
		string XmlSchemaPath { get; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000005 RID: 5
		string XmlSchemaNameSpace { get; }

		// Token: 0x06000006 RID: 6
		void ConvertEntries(XElement parent, Dictionary<string, IPkgPlugin> plugins, Config enviorn, XElement component);

		// Token: 0x06000007 RID: 7
		bool Pass(BuildPass pass);
	}
}
