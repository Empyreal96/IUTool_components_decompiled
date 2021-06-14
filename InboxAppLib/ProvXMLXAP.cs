using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.InboxAppLib.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000037 RID: 55
	public class ProvXMLXAP : ProvXMLBase
	{
		// Token: 0x060000DE RID: 222 RVA: 0x00005C40 File Offset: 0x00003E40
		public ProvXMLXAP(InboxAppParameters parameters, AppManifestXAP manifest) : base(parameters)
		{
			this._manifest = manifest;
			this._licenseDestinationPath = this.DetermineLicenseDestinationPath();
			this._provXMLDestinationPath = this.DetermineProvXMLDestinationPath();
			if (this._parameters.UpdateValue != UpdateType.UpdateNotNeeded)
			{
				this._updateProvXMLDestinationPath = base.GetMxipFileDestinationPath(this._parameters.ProvXMLBasePath, this._parameters.Category, this._parameters.UpdateValue, null);
			}
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00005CB0 File Offset: 0x00003EB0
		public override void ReadProvXML()
		{
			this._document = XDocument.Load(this._parameters.ProvXMLBasePath);
			if (!this.GetDetailsForXapPackage() && !this.GetDetailsForXapInfused() && !this.GetDetailsForXapLegacy() && this._originalProvXmlCharacteristic == ProvXMLXAP.Characteristic_XAP.Unknown)
			{
				throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture, "The application package and the provxml do not agree in type, as no <characteristic type=\"{0}\"|\"{1}\"> was found in the provxml file. Please ensure that the provxml file is the correct one for the application package.", new object[]
				{
					"XapPackage",
					"XapInfused"
				}));
			}
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00005D20 File Offset: 0x00003F20
		public override void Update(string appInstallDestinationPath, string licenseFileDestinationPath)
		{
			if (string.IsNullOrWhiteSpace(appInstallDestinationPath))
			{
				throw new ArgumentNullException("appInstallDestinationPath", "INTERNAL ERROR: appInstallDestinationPath is null!");
			}
			if (string.IsNullOrWhiteSpace(licenseFileDestinationPath))
			{
				throw new ArgumentNullException("licenseFileDestinationPath", "INTERNAL ERROR: licenseFileDestinationPath is null!");
			}
			if (this._document == null || this._manifestPathElement == null || this._licensePathElement == null)
			{
				throw new InvalidDataException("INTERNAL ERROR: One or more preconditions for the ProvXmlXAP.Update method are not met.");
			}
			this._manifestPathElement.Attribute("name").Value = "XapManifestPath";
			this._manifestPathElement.Attribute("value").Value = Path.Combine(appInstallDestinationPath, "WMAppManifest.xml");
			this._licensePathElement.Attribute("value").Value = licenseFileDestinationPath;
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00005DE0 File Offset: 0x00003FE0
		protected override string DetermineProvXMLDestinationPath()
		{
			string path = string.Empty;
			if (this._parameters.InfuseIntoDataPartition)
			{
				path = "$(runtime.data)\\SharedData\\Provisioning\\";
			}
			else
			{
				path = "$(runtime.commonfiles)\\Provisioning\\";
			}
			path = Path.Combine(path, base.Category.ToString());
			return Path.Combine(path, Path.GetFileName(this._parameters.ProvXMLBasePath));
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00005E44 File Offset: 0x00004044
		protected override string DetermineLicenseDestinationPath()
		{
			string path = string.Empty;
			if (this._parameters.InfuseIntoDataPartition)
			{
				path = "$(runtime.data)\\SharedData\\Provisioning\\";
			}
			else
			{
				path = "$(runtime.commonfiles)\\Provisioning\\";
			}
			string path2 = Path.GetFileNameWithoutExtension(this._parameters.ProvXMLBasePath) + "_" + Path.GetFileName(this._parameters.LicenseBasePath);
			path = Path.Combine(path, base.Category.ToString());
			return Path.Combine(path, path2);
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00005EC4 File Offset: 0x000040C4
		private bool GetDetailsForXapPackage()
		{
			bool result = false;
			IEnumerable<XElement> enumerable = from c in this._document.Descendants("characteristic")
			where c.Attribute("type").Value.Equals("XapPackage", StringComparison.OrdinalIgnoreCase)
			select c;
			if (enumerable != null && enumerable.Count<XElement>() > 0)
			{
				XElement xelement = enumerable.First<XElement>();
				xelement.Attribute("type").Value = "XapInfused";
				IEnumerable<XElement> enumerable2 = xelement.Descendants();
				if (enumerable2 != null && enumerable2.Count<XElement>() > 0)
				{
					this._originalProvXmlCharacteristic = ProvXMLXAP.Characteristic_XAP.XapPackage;
					this.ValidateContents(enumerable2, this._manifest, this._originalProvXmlCharacteristic);
					result = true;
					LogUtil.Diagnostic("Provxml {0} is of a XapPackage type", new object[]
					{
						this._parameters.ProvXMLBasePath
					});
				}
			}
			return result;
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x00005F88 File Offset: 0x00004188
		private bool GetDetailsForXapInfused()
		{
			bool result = false;
			IEnumerable<XElement> enumerable = (from c in this._document.Descendants("characteristic")
			where c.Attribute("type").Value.Equals("XapInfused", StringComparison.OrdinalIgnoreCase)
			select c).Descendants<XElement>();
			if (enumerable != null && enumerable.Count<XElement>() > 0)
			{
				this._originalProvXmlCharacteristic = ProvXMLXAP.Characteristic_XAP.XapInfused;
				this.ValidateContents(enumerable, this._manifest, this._originalProvXmlCharacteristic);
				result = true;
				LogUtil.Diagnostic("Provxml {0} is of a XapInfused type", new object[]
				{
					this._parameters.ProvXMLBasePath
				});
			}
			return result;
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00006020 File Offset: 0x00004220
		private bool GetDetailsForXapLegacy()
		{
			bool result = false;
			IEnumerable<XElement> enumerable = (from c in this._document.Descendants("characteristic")
			where c.Attribute("type").Value.Equals("AppInstall", StringComparison.OrdinalIgnoreCase)
			select c).Descendants<XElement>();
			if (enumerable != null && enumerable.Count<XElement>() > 0)
			{
				IEnumerable<XElement> enumerable2 = from c in enumerable.Descendants("parm")
				where c.Attribute("name").Value.Equals("INSTALLINFO", StringComparison.OrdinalIgnoreCase)
				select c;
				if (enumerable2 != null && enumerable2.Count<XElement>() > 0)
				{
					this._originalProvXmlCharacteristic = ProvXMLXAP.Characteristic_XAP.XapLegacy;
					this.ValidateAndRearrangeDocumentForXapInfused(enumerable, this._manifest, this._originalProvXmlCharacteristic);
					result = true;
					LogUtil.Diagnostic("Provxml {0} is of a XapLegacy type", new object[]
					{
						this._parameters.ProvXMLBasePath
					});
				}
			}
			return result;
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x000060FC File Offset: 0x000042FC
		private void ValidateContents(IEnumerable<XElement> characteristicNode, AppManifestXAP manifest, ProvXMLXAP.Characteristic_XAP characteristic)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (characteristic == ProvXMLXAP.Characteristic_XAP.XapPackage || characteristic == ProvXMLXAP.Characteristic_XAP.XapInfused)
			{
				using (IEnumerator<XElement> enumerator = characteristicNode.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						XElement xelement = enumerator.Current;
						if (xelement.HasAttributes)
						{
							string text = xelement.Attribute("name").Value.ToUpper(CultureInfo.InvariantCulture);
							if (text == null || text.Length != 0)
							{
								if (!(text == "PRODUCTID"))
								{
									if (!(text == "XAPPATH"))
									{
										if (!(text == "XAPMANIFESTPATH"))
										{
											if (!(text == "LICENSEPATH"))
											{
												if (!(text == "INSTANCEID"))
												{
													LogUtil.Diagnostic(string.Format(CultureInfo.InvariantCulture, "Unexpected parameter to be ignored: name=\"{0}\", value=\"{1}\"", new object[]
													{
														xelement.Attribute("name").Value,
														xelement.Attribute("value").Value
													}));
												}
												else if (string.IsNullOrWhiteSpace(xelement.Attribute("value").Value))
												{
													stringBuilder.AppendLine("Instance ID is blank or omitted");
												}
											}
											else
											{
												string fileName = Path.GetFileName(xelement.Attribute("value").Value);
												if (fileName != null && string.Compare(Path.GetFileName(this._parameters.LicenseBasePath), fileName, StringComparison.OrdinalIgnoreCase) == 0)
												{
													this._licensePathElement = xelement;
												}
												else
												{
													stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "Mismatch between manifest license file: {0} and provXML license file: {1}", new object[]
													{
														Path.GetFileName(this._parameters.LicenseBasePath),
														fileName
													}));
												}
											}
										}
										else if (string.Compare(Path.GetFileName(xelement.Attribute("value").Value), manifest.Filename, StringComparison.OrdinalIgnoreCase) == 0)
										{
											this._manifestPathElement = xelement;
										}
										else
										{
											stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "Mismatch between manifest file: {0}, and provXML manifest file: {1}", new object[]
											{
												manifest.Filename,
												xelement.Attribute("value").Value
											}));
										}
									}
									else if (string.Compare(Path.GetFileName(xelement.Attribute("value").Value), Path.GetFileName(this._parameters.PackageBasePath), StringComparison.OrdinalIgnoreCase) == 0)
									{
										this._manifestPathElement = xelement;
									}
									else
									{
										stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "Mismatch between XAP filename: {0}, and provXML XAP filename: {1}", new object[]
										{
											this._parameters.PackageBasePath,
											xelement.Attribute("value").Value
										}));
									}
								}
								else if (!xelement.Attribute("value").Value.Equals(manifest.ProductID, StringComparison.OrdinalIgnoreCase))
								{
									stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "Mismatch between manifest product ID: {0}, and provXML product ID: {1}", new object[]
									{
										manifest.ProductID,
										xelement.Attribute("value").Value
									}));
								}
							}
						}
					}
					goto IL_348;
				}
				goto IL_319;
				IL_348:
				if (!string.IsNullOrEmpty(stringBuilder.ToString()))
				{
					LogUtil.Error(stringBuilder.ToString());
					throw new InvalidDataException(stringBuilder.ToString());
				}
				return;
			}
			IL_319:
			throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture, "INTERNAL ERROR: Characteristic \"{0}\" is not supported by ValidateContents", new object[]
			{
				this._originalProvXmlCharacteristic.ToString()
			}));
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00006494 File Offset: 0x00004694
		private void ValidateAndRearrangeDocumentForXapInfused(IEnumerable<XElement> characteristicNode, AppManifestXAP manifest, ProvXMLXAP.Characteristic_XAP characteristic)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = "";
			string value = "";
			string text2 = "";
			string value2 = "";
			string value3 = "";
			string text3 = "";
			if (characteristic != ProvXMLXAP.Characteristic_XAP.XapLegacy)
			{
				throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture, "INTERNAL ERROR: Characteristic \"{0}\" is not supported by ValidateAndRearrangeDocumentForXapInfused", new object[]
				{
					this._originalProvXmlCharacteristic.ToString()
				}));
			}
			foreach (XElement xelement in characteristicNode)
			{
				if (xelement.Name.LocalName.Equals("characteristic", StringComparison.OrdinalIgnoreCase))
				{
					IEnumerable<XAttribute> enumerable = xelement.Attributes("type");
					if (enumerable != null && enumerable.Count<XAttribute>() > 0)
					{
						text = enumerable.First<XAttribute>().Value;
						if (!text.Equals(manifest.ProductID, StringComparison.OrdinalIgnoreCase))
						{
							stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "Mismatch between manifest product ID: {0}, and provXML product ID: {1}", new object[]
							{
								manifest.ProductID,
								text
							}));
						}
					}
				}
				else if (xelement.Name.LocalName.Equals("parm", StringComparison.OrdinalIgnoreCase) && xelement.Attribute("name") != null && xelement.Attribute("value") != null)
				{
					string value4 = xelement.Attribute("value").Value;
					string[] array = value4.Split(new char[]
					{
						';'
					}, 5, StringSplitOptions.RemoveEmptyEntries);
					LogUtil.Diagnostic("installInfos.Length = {0}, \"{1}\"", new object[]
					{
						array.Length,
						value4
					});
					if (array.Length >= 1)
					{
						value = array[0];
					}
					if (array.Length >= 2)
					{
						text2 = array[1];
						string fileName = Path.GetFileName(text2);
						if (fileName != null && !Path.GetFileName(this._parameters.LicenseBasePath).Equals(fileName, StringComparison.OrdinalIgnoreCase))
						{
							stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "Mismatch between manifest license file: {0} and provXML license file: {1}", new object[]
							{
								Path.GetFileName(this._parameters.LicenseBasePath),
								fileName
							}));
						}
					}
					if (array.Length >= 3)
					{
						value2 = array[2];
					}
					if (array.Length >= 4)
					{
						value3 = array[3];
					}
					if (array.Length >= 5)
					{
						text3 = array[4];
					}
					if (string.IsNullOrWhiteSpace(value))
					{
						stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "XapPath is blank in the provXML file \"{0}\"", new object[]
						{
							this._parameters.ProvXMLBasePath
						}));
					}
					if (string.IsNullOrWhiteSpace(text2))
					{
						stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "LicensePath is blank in the provXML file \"{0}\"", new object[]
						{
							this._parameters.ProvXMLBasePath
						}));
					}
					if (string.IsNullOrWhiteSpace(value2))
					{
						stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "InstanceID is blank in the provXML file \"{0}\"", new object[]
						{
							this._parameters.ProvXMLBasePath
						}));
					}
					if (string.IsNullOrWhiteSpace(value3))
					{
						stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "OfferID is blank in the provXML file \"{0}\"", new object[]
						{
							this._parameters.ProvXMLBasePath
						}));
					}
				}
			}
			this._document.RemoveNodes();
			XElement xelement2 = new XElement("characteristic");
			xelement2.Add(new XAttribute("type", "AppInstall"));
			this._document.Add(xelement2);
			XElement xelement3 = new XElement("characteristic");
			xelement3.Add(new XAttribute("type", "XapInfused"));
			xelement2.Add(xelement3);
			XElement xelement4 = new XElement("parm");
			xelement4.Add(new XAttribute("name", "ProductID"));
			xelement4.Add(new XAttribute("value", text));
			xelement3.Add(xelement4);
			XElement xelement5 = new XElement("parm");
			xelement5.Add(new XAttribute("name", "XapManifestPath"));
			xelement5.Add(new XAttribute("value", value));
			xelement3.Add(xelement5);
			this._manifestPathElement = xelement5;
			XElement xelement6 = new XElement("parm");
			xelement6.Add(new XAttribute("name", "LicensePath"));
			xelement6.Add(new XAttribute("value", text2));
			xelement3.Add(xelement6);
			this._licensePathElement = xelement6;
			XElement xelement7 = new XElement("parm");
			xelement7.Add(new XAttribute("name", "InstanceID"));
			xelement7.Add(new XAttribute("value", value2));
			xelement3.Add(xelement7);
			XElement xelement8 = new XElement("parm");
			xelement8.Add(new XAttribute("name", "OfferID"));
			xelement8.Add(new XAttribute("value", value3));
			xelement3.Add(xelement8);
			if (!string.IsNullOrWhiteSpace(text3) && text3.Equals(true.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				XElement xelement9 = new XElement("parm");
				xelement9.Add(new XAttribute("name", "UninstallDisabled"));
				xelement9.Add(new XAttribute("value", true.ToString()));
				xelement3.Add(xelement9);
			}
			if (!string.IsNullOrEmpty(stringBuilder.ToString()))
			{
				LogUtil.Error(stringBuilder.ToString());
				throw new InvalidDataException();
			}
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00006A6C File Offset: 0x00004C6C
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "XAP ProvXML: (BasePath)=\"{0}\"", new object[]
			{
				this._parameters.ProvXMLBasePath
			});
		}

		// Token: 0x0400004B RID: 75
		protected ProvXMLXAP.Characteristic_XAP _originalProvXmlCharacteristic;

		// Token: 0x0400004C RID: 76
		protected XElement _manifestPathElement;

		// Token: 0x0400004D RID: 77
		protected XElement _licensePathElement;

		// Token: 0x0400004E RID: 78
		protected AppManifestXAP _manifest;

		// Token: 0x02000055 RID: 85
		protected enum Characteristic_XAP
		{
			// Token: 0x040000FD RID: 253
			Unknown,
			// Token: 0x040000FE RID: 254
			XapLegacy,
			// Token: 0x040000FF RID: 255
			XapPackage,
			// Token: 0x04000100 RID: 256
			XapInfused
		}
	}
}
