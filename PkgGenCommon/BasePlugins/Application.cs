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
	// Token: 0x02000027 RID: 39
	[Export(typeof(IPkgPlugin))]
	public class Application : AppResource
	{
		// Token: 0x06000094 RID: 148 RVA: 0x00004238 File Offset: 0x00002438
		public override void ValidateEntries(IPkgProject packageGenerator, IEnumerable<XElement> componentEntries)
		{
			foreach (XElement componentEntry in componentEntries)
			{
				this.ValidateEntry(packageGenerator, componentEntry);
			}
			foreach (IGrouping<string, XElement> grouping in from x in componentEntries
			group x by string.Format("Name:{0}, Suite:{1}", (string)x.LocalAttribute("Name"), ((string)x.LocalAttribute("Suite")) ?? ""))
			{
				if (grouping.Count<XElement>() > 1)
				{
					throw new PkgXmlException(grouping.First<XElement>(), "Cannot have more than two Application components with same id:{0}", new object[]
					{
						grouping.Key
					});
				}
			}
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00004300 File Offset: 0x00002500
		protected override IEnumerable<PkgObject> ProcessEntry(IPkgProject packageGenerator, XElement componentEntry)
		{
			ApplicationBuilder builder = new ApplicationBuilder();
			builder.SetName(componentEntry.LocalAttribute("Name").Value);
			componentEntry.WithLocalAttribute("Suite", delegate(XAttribute x)
			{
				builder.SetSuite(x.Value);
			});
			base.ProcessFiles<ApplicationPkgObject, ApplicationBuilder>(componentEntry, builder);
			base.ProcessRegistry<ApplicationPkgObject, ApplicationBuilder>(componentEntry, builder);
			return new List<PkgObject>
			{
				builder.ToPkgObject()
			};
		}
	}
}
