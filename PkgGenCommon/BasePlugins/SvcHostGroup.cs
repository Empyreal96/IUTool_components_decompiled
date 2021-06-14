using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BasePlugins
{
	// Token: 0x0200002F RID: 47
	[Export(typeof(IPkgPlugin))]
	public class SvcHostGroup : PkgPlugin
	{
		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x0000438A File Offset: 0x0000258A
		public override bool UseSecurityCompilerPassthrough
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000C2 RID: 194 RVA: 0x0000438D File Offset: 0x0000258D
		public override string XmlSchemaPath
		{
			get
			{
				return PkgPlugin.BaseComponentSchemaPath;
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000C3 RID: 195 RVA: 0x00004394 File Offset: 0x00002594
		public override string XmlElementUniqueXPath
		{
			get
			{
				return "@Name";
			}
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00004469 File Offset: 0x00002669
		public override void ValidateEntries(IPkgProject packageGenerator, IEnumerable<XElement> componentEntries)
		{
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00004E5C File Offset: 0x0000305C
		public override IEnumerable<PkgObject> ProcessEntries(IPkgProject packageGenerator, IEnumerable<XElement> componentEntries)
		{
			List<PkgObject> list = new List<PkgObject>();
			foreach (XElement svcHostElement in componentEntries)
			{
				list.Add(new SvcHostGroupBuilder(svcHostElement).ToPkgObject());
			}
			return list;
		}
	}
}
