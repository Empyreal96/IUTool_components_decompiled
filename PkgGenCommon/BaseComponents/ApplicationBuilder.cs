using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents
{
	// Token: 0x02000034 RID: 52
	public sealed class ApplicationBuilder : AppResourceBuilder<ApplicationPkgObject, ApplicationBuilder>
	{
		// Token: 0x060000CD RID: 205 RVA: 0x000051C4 File Offset: 0x000033C4
		public ApplicationBuilder SetRequiredCapabilities(IEnumerable<XElement> requiredCapabilities)
		{
			this.pkgObject.RequiredCapabilities = new XElement(XName.Get("RequiredCapabilities", "urn:Microsoft.WindowsPhone/PackageSchema.v8.00"), requiredCapabilities);
			return this;
		}

		// Token: 0x060000CE RID: 206 RVA: 0x000051E7 File Offset: 0x000033E7
		public ApplicationBuilder SetPrivateResources(IEnumerable<XElement> privateResources)
		{
			this.pkgObject.PrivateResources = new XElement(XName.Get("PrivateResources", "urn:Microsoft.WindowsPhone/PackageSchema.v8.00"), privateResources);
			return this;
		}
	}
}
