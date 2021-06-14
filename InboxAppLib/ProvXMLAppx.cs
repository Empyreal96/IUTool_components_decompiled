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
	// Token: 0x02000033 RID: 51
	public class ProvXMLAppx : ProvXMLBase
	{
		// Token: 0x060000BE RID: 190 RVA: 0x00004CC8 File Offset: 0x00002EC8
		public static ProvXMLAppx CreateAppxProvXML(InboxAppParameters parameters, AppManifestAppxBase manifest)
		{
			ProvXMLAppx result;
			if (manifest.IsFramework)
			{
				result = new ProvXMLAppxFramework(parameters, manifest);
			}
			else if (manifest.IsBundle)
			{
				result = new ProvXMLAppxBundle(parameters, manifest);
			}
			else
			{
				result = new ProvXMLAppx(parameters, manifest);
			}
			return result;
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00004D04 File Offset: 0x00002F04
		public ProvXMLAppx(InboxAppParameters parameters, AppManifestAppxBase manifest) : base(parameters)
		{
			if (manifest == null)
			{
				throw new ArgumentNullException("manifest", "INTERNAL ERROR: The manifest passed into the ProvXMLAppx constructor is null!");
			}
			this._manifest = manifest;
			if (!this._manifest.IsFramework)
			{
				this._licenseDestinationPath = this.DetermineLicenseDestinationPath();
			}
			this._provXMLDestinationPath = this.DetermineProvXMLDestinationPath();
			if (this._parameters.UpdateValue != UpdateType.UpdateNotNeeded)
			{
				this._updateProvXMLDestinationPath = base.GetMxipFileDestinationPath(this._parameters.ProvXMLBasePath, this._parameters.Category, this._parameters.UpdateValue, this._manifest);
			}
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00004D98 File Offset: 0x00002F98
		public override void ReadProvXML()
		{
			this._document = XDocument.Load(this._parameters.ProvXMLBasePath);
			if (!this.GetDetailsForAppxPackage() && !this.GetDetailsForAppxInfused() && this._originalProvXmlCharacteristic == ProvXMLAppx.Characteristic_Appx.Unknown)
			{
				throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture, "The application package and the provxml do not agree in type, as no <characteristic type=\"{0}\"|\"{1}\"> was found in the provxml file. Please ensure that the provxml file is the correct one for the application package.", new object[]
				{
					"AppxPackage",
					"AppxInfused"
				}));
			}
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00004E00 File Offset: 0x00003000
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
			if (this._document == null || this._manifestPathElement == null || this._licensePathElement == null || this._productIDElement == null)
			{
				throw new InvalidDataException("INTERNAL ERROR: One or more preconditions for the ProvXMLAppx.Update method are not met.");
			}
			this._manifestPathElement.Attribute("name").Value = "AppXManifestPath";
			if (this._manifest.IsBundle)
			{
				string path = Path.Combine(appInstallDestinationPath, "AppxMetadata");
				this._manifestPathElement.Attribute("value").Value = Path.Combine(path, "AppxBundleManifest.xml");
			}
			else
			{
				this._manifestPathElement.Attribute("value").Value = Path.Combine(appInstallDestinationPath, "AppxManifest.xml");
			}
			this._document.Add(new XComment(string.Format(CultureInfo.InvariantCulture, "Dependency hashes {0}", new object[]
			{
				this._packageHash
			})));
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00004F1C File Offset: 0x0000311C
		protected override string DetermineProvXMLDestinationPath()
		{
			string path = string.Empty;
			string path2 = string.Empty;
			if (this._parameters.InfuseIntoDataPartition)
			{
				path = "$(runtime.data)\\SharedData\\Provisioning\\";
			}
			else
			{
				path = "$(runtime.commonfiles)\\Provisioning\\";
			}
			path2 = "MPAP_" + Path.GetFileName(this._parameters.ProvXMLBasePath).CleanFileName();
			return Path.Combine(path, base.Category.ToString(), path2);
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00004F90 File Offset: 0x00003190
		protected override string DetermineLicenseDestinationPath()
		{
			string path = string.Empty;
			if (this._parameters.InfuseIntoDataPartition)
			{
				path = "$(runtime.data)\\SharedData\\Provisioning\\";
			}
			else
			{
				path = "$(runtime.commonfiles)\\Xaps\\";
			}
			return Path.Combine(path, Path.GetFileName(this._parameters.LicenseBasePath));
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00004FD8 File Offset: 0x000031D8
		private bool GetDetailsForAppxPackage()
		{
			bool result = false;
			IEnumerable<XElement> enumerable = from c in this._document.Descendants("characteristic")
			where c.Attribute("type").Value.Equals("AppxPackage", StringComparison.OrdinalIgnoreCase)
			select c;
			if (enumerable != null && enumerable.Count<XElement>() > 0)
			{
				enumerable.First<XElement>().Attribute("type").Value = "AppxInfused";
				IEnumerable<XElement> enumerable2 = enumerable.Descendants<XElement>();
				if (enumerable2 != null && enumerable2.Count<XElement>() > 0)
				{
					this._originalProvXmlCharacteristic = ProvXMLAppx.Characteristic_Appx.AppxPackage;
					this._characteristicParamsElements = enumerable2;
					this.ValidateContents(enumerable2, this._manifest);
					result = true;
					LogUtil.Diagnostic("Provxml {0} is of a AppxPackage type", new object[]
					{
						this._parameters.ProvXMLBasePath
					});
				}
			}
			return result;
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x0000509C File Offset: 0x0000329C
		protected bool GetDetailsForAppxInfused()
		{
			bool result = false;
			IEnumerable<XElement> enumerable = (from c in this._document.Descendants("characteristic")
			where c.Attribute("type").Value.Equals("AppxInfused", StringComparison.OrdinalIgnoreCase)
			select c).Descendants<XElement>();
			if (enumerable != null && enumerable.Count<XElement>() > 0)
			{
				this._originalProvXmlCharacteristic = ProvXMLAppx.Characteristic_Appx.AppxInfused;
				this._characteristicParamsElements = enumerable;
				this.ValidateContents(enumerable, this._manifest);
				result = true;
				LogUtil.Diagnostic("Provxml {0} is of a AppxInfused type", new object[]
				{
					this._parameters.ProvXMLBasePath
				});
			}
			return result;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00005134 File Offset: 0x00003334
		protected virtual void ValidateContents(IEnumerable<XElement> characteristicNode, AppManifestAppxBase manifest)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (XElement xelement in characteristicNode)
			{
				if (xelement.HasAttributes)
				{
					string text = xelement.Attribute("name").Value.ToUpper(CultureInfo.InvariantCulture);
					uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
					if (num <= 2405275073U)
					{
						if (num <= 379477275U)
						{
							if (num != 348809376U)
							{
								if (num == 379477275U)
								{
									if (text == "APPXPATH")
									{
										if (this._manifestPathElement != null)
										{
											LogUtil.Warning(string.Format(CultureInfo.InvariantCulture, "This provXML file has an earlier '{0}' attribute with value '{1}'. The '{2}' attribute will be ignored.", new object[]
											{
												"APPXMANIFESTPATH",
												xelement.Attribute("value").Value
											}), new object[]
											{
												"APPXPATH"
											});
											continue;
										}
										if (!string.IsNullOrWhiteSpace(xelement.Attribute("value").Value))
										{
											this._manifestPathElement = xelement;
											continue;
										}
										continue;
									}
								}
							}
							else if (text == "PAYLOADID")
							{
								continue;
							}
						}
						else if (num != 2166136261U)
						{
							if (num == 2405275073U)
							{
								if (text == "PRODUCTID")
								{
									this._productIDElement = xelement;
									if (manifest.IsBundle || manifest.IsResource || string.IsNullOrWhiteSpace(manifest.ProductID))
									{
										continue;
									}
									Guid a = default(Guid);
									Guid b = default(Guid);
									string value = xelement.Attribute("value").Value;
									bool flag = Guid.TryParse(value, out a);
									bool flag2 = Guid.TryParse(manifest.ProductID, out b);
									if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(manifest.ProductID))
									{
										stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "The provXML productID {0} and/or the manifest product ID {1} are blank. Please make sure they are specified.", new object[]
										{
											value,
											manifest.ProductID
										}));
										continue;
									}
									if (flag2)
									{
										if (!flag)
										{
											stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "provXML productID = \"{0}\" is not a GUID. It should be the same as the manifest product ID \"{1}\"", new object[]
											{
												value,
												manifest.ProductID
											}));
											continue;
										}
										if (a != b)
										{
											this._productIDElement = null;
											stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "Mismatch between manifest product ID: {0}, and provXML product ID: {1}", new object[]
											{
												manifest.ProductID,
												value
											}));
											continue;
										}
										continue;
									}
									else
									{
										if (string.Equals(value, manifest.ProductID, StringComparison.OrdinalIgnoreCase))
										{
											LogUtil.Warning("provXML product \"{0}\" and manifest product ID \"{1}\" match but are not GUIDs.", new object[]
											{
												value,
												manifest.ProductID
											});
											continue;
										}
										this._productIDElement = null;
										stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "Mismatch between manifest product ID: {0}, and provXML product ID: {1}", new object[]
										{
											manifest.ProductID,
											value
										}));
										continue;
									}
								}
							}
						}
						else if (text != null)
						{
							if (text.Length == 0)
							{
								continue;
							}
						}
					}
					else if (num <= 2699001288U)
					{
						if (num != 2416439135U)
						{
							if (num == 2699001288U)
							{
								if (text == "OFFERID")
								{
									continue;
								}
							}
						}
						else if (text == "INSTANCEID")
						{
							if (string.IsNullOrWhiteSpace(xelement.Attribute("value").Value))
							{
								stringBuilder.AppendLine("Instance ID is blank or omitted");
								continue;
							}
							this._instanceIDElement = xelement;
							continue;
						}
					}
					else if (num != 3464016850U)
					{
						if (num == 3552393555U)
						{
							if (text == "LICENSEPATH")
							{
								string fileName = Path.GetFileName(xelement.Attribute("value").Value);
								if (fileName != null && string.Compare(Path.GetFileName(this._parameters.LicenseBasePath), fileName, StringComparison.OrdinalIgnoreCase) == 0)
								{
									this._licensePathElement = xelement;
									continue;
								}
								stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "Mismatch between manifest license file: {0} and provXML license file: {1}", new object[]
								{
									Path.GetFileName(this._parameters.LicenseBasePath),
									fileName
								}));
								continue;
							}
						}
					}
					else if (text == "APPXMANIFESTPATH")
					{
						if (this._manifestPathElement != null)
						{
							LogUtil.Warning(string.Format(CultureInfo.InvariantCulture, "This provXML file has an earlier '{0}' attribute with value '{1}'. The '{2}' attribute will be ignored.", new object[]
							{
								"APPXPATH",
								xelement.Attribute("value").Value
							}), new object[]
							{
								"APPXMANIFESTPATH"
							});
							continue;
						}
						if (string.Compare(Path.GetFileName(xelement.Attribute("value").Value), manifest.Filename, StringComparison.OrdinalIgnoreCase) == 0)
						{
							this._manifestPathElement = xelement;
							continue;
						}
						stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "Mismatch between manifest file: {0}, and provXML manifest file: {1}", new object[]
						{
							manifest.Filename,
							xelement.Attribute("value").Value
						}));
						continue;
					}
					LogUtil.Diagnostic(string.Format(CultureInfo.InvariantCulture, "Unexpected parameter to be ignored: name=\"{0}\", value=\"{1}\"", new object[]
					{
						xelement.Attribute("name").Value,
						xelement.Attribute("value").Value
					}));
				}
			}
			if (!string.IsNullOrEmpty(stringBuilder.ToString()))
			{
				LogUtil.Error(stringBuilder.ToString());
				throw new InvalidDataException(stringBuilder.ToString());
			}
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x000056EC File Offset: 0x000038EC
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Appx ProvXML: (BasePath)=\"{0}\"", new object[]
			{
				this._parameters.ProvXMLBasePath
			});
		}

		// Token: 0x0400003E RID: 62
		protected ProvXMLAppx.Characteristic_Appx _originalProvXmlCharacteristic;

		// Token: 0x0400003F RID: 63
		protected IEnumerable<XElement> _characteristicParamsElements;

		// Token: 0x04000040 RID: 64
		protected XElement _manifestPathElement;

		// Token: 0x04000041 RID: 65
		protected XElement _licensePathElement;

		// Token: 0x04000042 RID: 66
		protected XElement _instanceIDElement;

		// Token: 0x04000043 RID: 67
		protected XElement _productIDElement;

		// Token: 0x04000044 RID: 68
		protected AppManifestAppxBase _manifest;

		// Token: 0x02000052 RID: 82
		protected enum Characteristic_Appx
		{
			// Token: 0x040000F3 RID: 243
			Unknown,
			// Token: 0x040000F4 RID: 244
			AppxPackage,
			// Token: 0x040000F5 RID: 245
			AppxInfused,
			// Token: 0x040000F6 RID: 246
			FrameworkPackage
		}
	}
}
