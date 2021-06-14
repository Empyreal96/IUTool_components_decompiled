using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.DeviceManagement.MCSF
{
	// Token: 0x02000003 RID: 3
	[Export(typeof(IPkgPlugin))]
	public class SettingsGroup : PkgPlugin
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600001A RID: 26 RVA: 0x00003438 File Offset: 0x00001638
		public override string XmlSchemaPath
		{
			get
			{
				return "Microsoft.WindowsPhone.DeviceManagement.MCSF.ProjSchema.xsd";
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x0000343F File Offset: 0x0000163F
		public override void ValidateEntries(IPkgProject packageGenerator, IEnumerable<XElement> componentEntries)
		{
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00003444 File Offset: 0x00001644
		public override IEnumerable<PkgObject> ProcessEntries(IPkgProject packageGenerator, IEnumerable<XElement> componentEntries)
		{
			if (packageGenerator == null)
			{
				throw new ArgumentNullException("packageGenerator");
			}
			if (componentEntries == null)
			{
				throw new ArgumentNullException("componentEntries");
			}
			string tempDirectory = packageGenerator.TempDirectory;
			PkgRegWriter pkgRegWriter = new PkgRegWriter(packageGenerator.Attributes["Name"], tempDirectory, packageGenerator.MacroResolver);
			pkgRegWriter.GenerateReadablePolicyXML = true;
			foreach (XElement xelement in componentEntries)
			{
				if (xelement.Name.LocalName.Equals("SettingsGroup", StringComparison.Ordinal))
				{
					pkgRegWriter.WriteSettingsGroup(xelement);
				}
			}
			return new List<PkgObject>
			{
				pkgRegWriter.ToPkgObject()
			};
		}
	}
}
