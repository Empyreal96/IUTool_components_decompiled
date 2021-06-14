using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BasePlugins
{
	// Token: 0x02000029 RID: 41
	[Export(typeof(IPkgPlugin))]
	public class BCDStore : PkgPlugin
	{
		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600009D RID: 157 RVA: 0x0000438A File Offset: 0x0000258A
		public override bool UseSecurityCompilerPassthrough
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600009E RID: 158 RVA: 0x0000438D File Offset: 0x0000258D
		public override string XmlSchemaPath
		{
			get
			{
				return PkgPlugin.BaseComponentSchemaPath;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600009F RID: 159 RVA: 0x00004462 File Offset: 0x00002662
		public override string XmlElementUniqueXPath
		{
			get
			{
				return "@Source";
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00004469 File Offset: 0x00002669
		public override void ValidateEntries(IPkgProject packageGenerator, IEnumerable<XElement> componentEntries)
		{
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x0000446C File Offset: 0x0000266C
		public override IEnumerable<PkgObject> ProcessEntries(IPkgProject packageGenerator, IEnumerable<XElement> componentEntries)
		{
			List<PkgObject> list = new List<PkgObject>();
			foreach (XElement source in componentEntries)
			{
				list.Add(new BCDStoreBuilder(source.LocalAttribute("Source").Value).ToPkgObject());
			}
			return list;
		}
	}
}
