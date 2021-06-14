using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BasePlugins
{
	// Token: 0x0200002C RID: 44
	[Export(typeof(IPkgPlugin))]
	public class Driver : OSComponent
	{
		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000AE RID: 174 RVA: 0x0000438A File Offset: 0x0000258A
		public override bool UseSecurityCompilerPassthrough
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000AF RID: 175 RVA: 0x0000438D File Offset: 0x0000258D
		public override string XmlSchemaPath
		{
			get
			{
				return PkgPlugin.BaseComponentSchemaPath;
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000B0 RID: 176 RVA: 0x00004750 File Offset: 0x00002950
		public override string XmlElementUniqueXPath
		{
			get
			{
				return "@InfSource";
			}
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00004469 File Offset: 0x00002669
		public override void ValidateEntries(IPkgProject packageGenerator, IEnumerable<XElement> componentEntries)
		{
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00004758 File Offset: 0x00002958
		protected override IEnumerable<PkgObject> ProcessEntry(IPkgProject packageGenerator, XElement componentEntry)
		{
			DriverBuilder driverBuilder = new DriverBuilder(componentEntry.LocalAttribute("InfSource").Value);
			foreach (XElement reference in componentEntry.LocalElements("Reference"))
			{
				driverBuilder.AddReference(reference);
			}
			foreach (XElement security in componentEntry.LocalElements("Security"))
			{
				driverBuilder.AddSecurity(security);
			}
			base.ProcessFiles<DriverPkgObject, DriverBuilder>(componentEntry, driverBuilder);
			base.ProcessRegistry<DriverPkgObject, DriverBuilder>(componentEntry, driverBuilder);
			return new List<PkgObject>
			{
				driverBuilder.ToPkgObject()
			};
		}
	}
}
