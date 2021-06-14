using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.InboxAppLib.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000035 RID: 53
	public class ProvXMLAppxFramework : ProvXMLAppx
	{
		// Token: 0x060000CB RID: 203 RVA: 0x00005800 File Offset: 0x00003A00
		public ProvXMLAppxFramework(InboxAppParameters parameters, AppManifestAppxBase manifest) : base(parameters, manifest)
		{
		}

		// Token: 0x060000CC RID: 204 RVA: 0x0000580C File Offset: 0x00003A0C
		public override void ReadProvXML()
		{
			this._document = XDocument.Load(this._parameters.ProvXMLBasePath);
			if (!this.GetDetailsForFrameworkPackage() && !base.GetDetailsForAppxInfused() && this._originalProvXmlCharacteristic == ProvXMLAppx.Characteristic_Appx.Unknown)
			{
				throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture, "The application package and the provxml do not agree in type, as no <characteristic type=\"{0}\"|\"{1}\"> was found in the provxml file. Please ensure that the provxml file is the correct one for the application package.", new object[]
				{
					"FrameworkPackage",
					"AppxInfused"
				}));
			}
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00005874 File Offset: 0x00003A74
		public override void Update(string appInstallDestinationPath, string licenseFileDestinationPath)
		{
			if (string.IsNullOrWhiteSpace(appInstallDestinationPath))
			{
				throw new ArgumentNullException("appInstallDestinationPath", "INTERNAL ERROR: appInstallDestinationPath is null!");
			}
			if (this._document == null || !this._manifest.IsFramework || this._manifestPathElement == null || this._productIDElement == null)
			{
				throw new InvalidDataException("INTERNAL ERROR: One or more preconditions for the ProvXMLAppxFramework.Update method are not met.");
			}
			this._manifestPathElement.Attribute("name").Value = "AppXManifestPath";
			this._manifestPathElement.Attribute("value").Value = Path.Combine(appInstallDestinationPath, "AppxManifest.xml");
			this._document.Add(new XComment(string.Format(CultureInfo.InvariantCulture, "Dependency hashes {0}", new object[]
			{
				this._packageHash
			})));
		}

		// Token: 0x060000CE RID: 206 RVA: 0x0000593C File Offset: 0x00003B3C
		protected override string DetermineProvXMLDestinationPath()
		{
			string path = string.Empty;
			string path2 = string.Empty;
			if (this._parameters.InfuseIntoDataPartition)
			{
				throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Infusing framework packages onto the data partition is not supported. Please remove the {0} attribute from the pkg.xml file for all framework packages.", new object[]
				{
					"InfuseIntoDataPartition"
				}));
			}
			path = "$(runtime.coldBootProvxmlMS)\\";
			path2 = "mxipcold_appframework_" + Path.GetFileName(this._parameters.ProvXMLBasePath).CleanFileName();
			return Path.Combine(path, path2);
		}

		// Token: 0x060000CF RID: 207 RVA: 0x000059B4 File Offset: 0x00003BB4
		private bool GetDetailsForFrameworkPackage()
		{
			bool result = false;
			IEnumerable<XElement> enumerable = from c in this._document.Descendants("characteristic")
			where c.Attribute("type").Value.Equals("FrameworkPackage", StringComparison.OrdinalIgnoreCase)
			select c;
			if (enumerable != null && enumerable.Count<XElement>() > 0)
			{
				enumerable.First<XElement>().Attribute("type").Value = "AppxInfused";
				IEnumerable<XElement> enumerable2 = enumerable.Descendants<XElement>();
				if (enumerable2 != null && enumerable2.Count<XElement>() > 0)
				{
					this._originalProvXmlCharacteristic = ProvXMLAppx.Characteristic_Appx.FrameworkPackage;
					this._characteristicParamsElements = enumerable2;
					this.ValidateContents(enumerable2, this._manifest);
					result = true;
					LogUtil.Diagnostic("Provxml {0} is of a FrameworkPackage type", new object[]
					{
						this._parameters.ProvXMLBasePath
					});
				}
			}
			return result;
		}
	}
}
