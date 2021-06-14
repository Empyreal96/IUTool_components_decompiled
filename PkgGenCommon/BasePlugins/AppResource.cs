using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BasePlugins
{
	// Token: 0x02000028 RID: 40
	[Export(typeof(IPkgPlugin))]
	public class AppResource : OSComponent
	{
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000097 RID: 151 RVA: 0x0000438A File Offset: 0x0000258A
		public override bool UseSecurityCompilerPassthrough
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000098 RID: 152 RVA: 0x0000438D File Offset: 0x0000258D
		public override string XmlSchemaPath
		{
			get
			{
				return PkgPlugin.BaseComponentSchemaPath;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000099 RID: 153 RVA: 0x00004394 File Offset: 0x00002594
		public override string XmlElementUniqueXPath
		{
			get
			{
				return "@Name";
			}
		}

		// Token: 0x0600009A RID: 154 RVA: 0x0000439C File Offset: 0x0000259C
		protected override void ValidateEntry(IPkgProject packageGenerator, XElement componentEntry)
		{
			XAttribute xattribute = componentEntry.LocalAttribute("Name");
			if (xattribute == null || string.IsNullOrEmpty(xattribute.Value))
			{
				throw new PkgXmlException(componentEntry, "Name needs to be specified for Application or AppResource objects", new object[0]);
			}
		}

		// Token: 0x0600009B RID: 155 RVA: 0x000043D8 File Offset: 0x000025D8
		protected override IEnumerable<PkgObject> ProcessEntry(IPkgProject packageGenerator, XElement componentEntry)
		{
			AppResourceBuilder builder = new AppResourceBuilder();
			builder.SetName(componentEntry.LocalAttribute("Name").Value);
			componentEntry.WithLocalAttribute("Suite", delegate(XAttribute x)
			{
				builder.SetSuite(x.Value);
			});
			base.ProcessFiles<AppResourcePkgObject, AppResourceBuilder>(componentEntry, builder);
			base.ProcessRegistry<AppResourcePkgObject, AppResourceBuilder>(componentEntry, builder);
			return new List<PkgObject>
			{
				builder.ToPkgObject()
			};
		}
	}
}
