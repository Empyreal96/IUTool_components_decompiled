using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BasePlugins
{
	// Token: 0x0200002A RID: 42
	[Export(typeof(IPkgPlugin))]
	public class BinaryPartition : PkgPlugin
	{
		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000A3 RID: 163 RVA: 0x0000438A File Offset: 0x0000258A
		public override bool UseSecurityCompilerPassthrough
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x0000438D File Offset: 0x0000258D
		public override string XmlSchemaPath
		{
			get
			{
				return PkgPlugin.BaseComponentSchemaPath;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x000044DC File Offset: 0x000026DC
		public override string XmlElementUniqueXPath
		{
			get
			{
				return "@ImageSource";
			}
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00004469 File Offset: 0x00002669
		public override void ValidateEntries(IPkgProject packageGenerator, IEnumerable<XElement> componentEntries)
		{
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x000044E4 File Offset: 0x000026E4
		public override IEnumerable<PkgObject> ProcessEntries(IPkgProject packageGenerator, IEnumerable<XElement> componentEntries)
		{
			List<PkgObject> list = new List<PkgObject>();
			foreach (XElement source in componentEntries)
			{
				list.Add(new BinaryPartitionBuilder(source.LocalAttribute("ImageSource").Value).ToPkgObject());
			}
			return list;
		}
	}
}
