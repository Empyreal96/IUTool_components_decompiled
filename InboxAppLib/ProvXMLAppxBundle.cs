using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.InboxAppLib.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000034 RID: 52
	public class ProvXMLAppxBundle : ProvXMLAppx
	{
		// Token: 0x060000C8 RID: 200 RVA: 0x00005711 File Offset: 0x00003911
		public ProvXMLAppxBundle(InboxAppParameters parameters, AppManifestAppxBase manifest) : base(parameters, manifest)
		{
			if (!(manifest is AppManifestAppxBundle))
			{
				throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture, "INTERNAL ERROR: The manifest passed into ProvXMLAppxBundle is of type {0}. Only AppxManifestBundle2013 types are allowed.", new object[]
				{
					manifest.GetType()
				}));
			}
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00005748 File Offset: 0x00003948
		public override void Update(string appInstallDestinationPath, string licenseFileDestinationPath)
		{
			base.Update(appInstallDestinationPath, licenseFileDestinationPath);
			if (this._characteristicParamsElements == null || this._characteristicParamsElements.First<XElement>() == null)
			{
				throw new InvalidDataException("INTERNAL ERROR: One or more preconditions for the ProvXMLAppxBundle.Update method are not met.");
			}
			XContainer xcontainer = this._characteristicParamsElements.Ancestors<XElement>().First<XElement>();
			XElement xelement = new XElement("parm");
			xelement.Add(new XAttribute("name", "IsBundle"));
			xelement.Add(new XAttribute("value", true.ToString()));
			xcontainer.Add(xelement);
		}

		// Token: 0x060000CA RID: 202 RVA: 0x000057DB File Offset: 0x000039DB
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Appx Bundle ProvXML: (BasePath)=\"{0}\"", new object[]
			{
				this._parameters.ProvXMLBasePath
			});
		}
	}
}
