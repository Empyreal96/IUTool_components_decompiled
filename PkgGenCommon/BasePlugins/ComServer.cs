using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BasePlugins
{
	// Token: 0x0200002B RID: 43
	[Export(typeof(IPkgPlugin))]
	public class ComServer : OSComponent
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000A9 RID: 169 RVA: 0x0000438A File Offset: 0x0000258A
		public override bool UseSecurityCompilerPassthrough
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000AA RID: 170 RVA: 0x0000438D File Offset: 0x0000258D
		public override string XmlSchemaPath
		{
			get
			{
				return PkgPlugin.BaseComponentSchemaPath;
			}
		}

		// Token: 0x060000AB RID: 171 RVA: 0x0000454C File Offset: 0x0000274C
		protected override void ValidateEntry(IPkgProject packageGenerator, XElement componentEntry)
		{
			if (componentEntry.LocalElements("Dll").Count<XElement>() != 1)
			{
				throw new PkgXmlException(componentEntry, "ComServers must have one and only one Dll element.", new object[0]);
			}
			foreach (XElement xelement in componentEntry.LocalDescendants("Class"))
			{
				string text = packageGenerator.MacroResolver.Resolve(xelement.LocalAttribute("Id").Value);
				Guid guid;
				if (!Guid.TryParseExact(text, "B", out guid))
				{
					throw new PkgXmlException(xelement, "Invalid COM class ID:'{0}'", new object[]
					{
						text
					});
				}
			}
			foreach (XElement xelement2 in componentEntry.LocalDescendants("Interface"))
			{
				string text2 = packageGenerator.MacroResolver.Resolve(xelement2.LocalAttribute("Id").Value);
				Guid guid2;
				if (!Guid.TryParseExact(text2, "B", out guid2))
				{
					throw new PkgXmlException(xelement2, "Invalid COM interface ID:'{0}'", new object[]
					{
						text2
					});
				}
			}
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00004680 File Offset: 0x00002880
		protected override IEnumerable<PkgObject> ProcessEntry(IPkgProject packageGenerator, XElement componentEntry)
		{
			ComServerBuilder comServerBuilder = new ComServerBuilder();
			comServerBuilder.SetComDll(componentEntry.LocalElement("Dll"));
			foreach (XElement element in componentEntry.LocalDescendants("Class"))
			{
				comServerBuilder.AddClass(element);
			}
			foreach (XElement element2 in componentEntry.LocalDescendants("Interface"))
			{
				comServerBuilder.AddInterface(element2);
			}
			base.ProcessFiles<ComPkgObject, ComServerBuilder>(componentEntry, comServerBuilder);
			base.ProcessRegistry<ComPkgObject, ComServerBuilder>(componentEntry, comServerBuilder);
			return new List<PkgObject>
			{
				comServerBuilder.ToPkgObject()
			};
		}
	}
}
