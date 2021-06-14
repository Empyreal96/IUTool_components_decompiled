using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;

namespace Microsoft.WindowsPhone.ImageUpdate.Customization.XML
{
	// Token: 0x0200000F RID: 15
	[XmlRoot(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class Application : IDefinedIn
	{
		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060000F1 RID: 241 RVA: 0x0000637F File Offset: 0x0000457F
		// (set) Token: 0x060000F2 RID: 242 RVA: 0x00006387 File Offset: 0x00004587
		[XmlIgnore]
		public string DefinedInFile { get; set; }

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060000F3 RID: 243 RVA: 0x00006390 File Offset: 0x00004590
		// (set) Token: 0x060000F4 RID: 244 RVA: 0x00006398 File Offset: 0x00004598
		[XmlAttribute]
		public string Source { get; set; }

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060000F5 RID: 245 RVA: 0x000063A1 File Offset: 0x000045A1
		[XmlIgnore]
		public string ExpandedSourcePath
		{
			get
			{
				return ImageCustomizations.ExpandPath(this.Source);
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x060000F6 RID: 246 RVA: 0x000063AE File Offset: 0x000045AE
		// (set) Token: 0x060000F7 RID: 247 RVA: 0x000063B6 File Offset: 0x000045B6
		[XmlAttribute]
		public string License { get; set; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060000F8 RID: 248 RVA: 0x000063BF File Offset: 0x000045BF
		[XmlIgnore]
		public string ExpandedLicensePath
		{
			get
			{
				return ImageCustomizations.ExpandPath(this.License);
			}
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060000F9 RID: 249 RVA: 0x000063CC File Offset: 0x000045CC
		// (set) Token: 0x060000FA RID: 250 RVA: 0x000063D4 File Offset: 0x000045D4
		[XmlAttribute]
		public string ProvXML { get; set; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x060000FB RID: 251 RVA: 0x000063DD File Offset: 0x000045DD
		[XmlIgnore]
		public string ExpandedProvXMLPath
		{
			get
			{
				return ImageCustomizations.ExpandPath(this.ProvXML);
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x060000FC RID: 252 RVA: 0x000063EC File Offset: 0x000045EC
		[XmlIgnore]
		public string DeviceDestination
		{
			get
			{
				if (this.Source == null)
				{
					return null;
				}
				string fileName = Path.GetFileName(this.Source);
				if (this.TargetPartition.Equals(PkgConstants.c_strMainOsPartition, StringComparison.OrdinalIgnoreCase))
				{
					if (this.StaticApp)
					{
						return Path.Combine("Programs\\CommonFiles\\xaps", fileName);
					}
					return Path.Combine("Programs\\CommonFiles\\Multivariant\\Apps", fileName);
				}
				else
				{
					if (!this.TargetPartition.Equals(PkgConstants.c_strDataPartition, StringComparison.OrdinalIgnoreCase))
					{
						throw new InvalidOperationException("Unknown target partition while querying for a static device destination!");
					}
					if (this.StaticApp)
					{
						return Path.Combine("SharedData\\Provisioning\\OEM\\Public", fileName);
					}
					return Path.Combine("SharedData\\Multivariant\\Apps", fileName);
				}
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x060000FD RID: 253 RVA: 0x00006480 File Offset: 0x00004680
		[XmlIgnore]
		public string DeviceLicense
		{
			get
			{
				string fileName = Path.GetFileName(this.License);
				if (this.TargetPartition.Equals(PkgConstants.c_strMainOsPartition, StringComparison.OrdinalIgnoreCase))
				{
					if (this.StaticApp)
					{
						return Path.Combine("Programs\\CommonFiles\\xaps", fileName);
					}
					return Path.Combine("Programs\\CommonFiles\\Multivariant\\Apps", fileName);
				}
				else
				{
					if (!this.TargetPartition.Equals(PkgConstants.c_strDataPartition, StringComparison.OrdinalIgnoreCase))
					{
						throw new InvalidOperationException("Unknown target partition while querying for a static device destination!");
					}
					if (this.StaticApp)
					{
						return Path.Combine("SharedData\\Provisioning\\OEM\\Public", fileName);
					}
					return Path.Combine("SharedData\\Multivariant\\Apps", fileName);
				}
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x060000FE RID: 254 RVA: 0x0000650C File Offset: 0x0000470C
		[XmlIgnore]
		public string DeviceProvXML
		{
			get
			{
				string fileName = Path.GetFileName(this.ProvXML);
				if (!this.StaticApp)
				{
					throw new InvalidOperationException("Non-static applications should not query for a destination path here!");
				}
				if (this.TargetPartition.Equals(PkgConstants.c_strMainOsPartition, StringComparison.OrdinalIgnoreCase))
				{
					return Path.Combine("Programs\\CommonFiles\\Provisioning\\Microsoft", fileName);
				}
				if (this.TargetPartition.Equals(PkgConstants.c_strDataPartition, StringComparison.OrdinalIgnoreCase))
				{
					return Path.Combine("SharedData\\Provisioning\\OEM", fileName);
				}
				throw new InvalidOperationException("Unknown target partition while querying for a static device provXML path!");
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x060000FF RID: 255 RVA: 0x00006580 File Offset: 0x00004780
		// (set) Token: 0x06000100 RID: 256 RVA: 0x00006588 File Offset: 0x00004788
		[XmlIgnore]
		public bool StaticApp { get; set; }

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000101 RID: 257 RVA: 0x00006591 File Offset: 0x00004791
		// (set) Token: 0x06000102 RID: 258 RVA: 0x000065BA File Offset: 0x000047BA
		[XmlAttribute]
		public string TargetPartition
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(this._targetPartition))
				{
					return this._targetPartition;
				}
				if (!this.StaticApp)
				{
					return Application.DefaultVariantPartition;
				}
				return Application.DefaultStaticPartition;
			}
			set
			{
				this._targetPartition = value;
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000103 RID: 259 RVA: 0x000065C3 File Offset: 0x000047C3
		[XmlIgnore]
		public static IEnumerable<string> ValidPartitions
		{
			get
			{
				return new List<string>
				{
					PkgConstants.c_strMainOsPartition,
					PkgConstants.c_strDataPartition
				};
			}
		}

		// Token: 0x06000104 RID: 260 RVA: 0x000065E0 File Offset: 0x000047E0
		public KeyValuePair<Guid, XElement> UpdateProvXml(XElement rootNode)
		{
			XElement xelement = new XElement(rootNode);
			XElement xelement2 = xelement.Elements().Single<XElement>().Elements().First<XElement>();
			string path;
			if (this.TargetPartition.Equals(PkgConstants.c_strMainOsPartition, StringComparison.OrdinalIgnoreCase))
			{
				path = Application.MainOSPartitionRoot;
			}
			else
			{
				if (!this.TargetPartition.Equals(PkgConstants.c_strDataPartition, StringComparison.OrdinalIgnoreCase))
				{
					throw new CustomizationException("Creating provxml to an invalid partition! This should have been caught in verification.");
				}
				path = Application.DataPartitionRoot;
			}
			Guid key;
			if (xelement2.Attribute("type") != null && Guid.TryParse(xelement2.Attribute("type").Value, out key))
			{
				XAttribute xattribute = xelement2.Elements().SingleOrDefault((XElement x) => x.Attribute("name").Value.Equals("InstallInfo", StringComparison.OrdinalIgnoreCase)).Attribute("value");
				string[] array = xattribute.Value.Split(new char[]
				{
					';'
				});
				if (this.DeviceDestination != null)
				{
					array[0] = Path.Combine(path, this.DeviceDestination);
				}
				array[1] = Path.Combine(path, this.DeviceLicense);
				xattribute.Value = string.Join(";", array);
				return new KeyValuePair<Guid, XElement>(key, xelement);
			}
			XElement xelement3 = xelement2.Elements().Single((XElement x) => x.Attribute("name").Value.Equals("ProductID", StringComparison.OrdinalIgnoreCase));
			key = new Guid(xelement3.Attribute("value").Value);
			XElement xelement4 = xelement2.Elements().SingleOrDefault((XElement x) => x.Attribute("name").Value.Equals("LicensePath", StringComparison.OrdinalIgnoreCase));
			if (xelement4 != null)
			{
				xelement4.Remove();
			}
			xelement4 = new XElement("parm");
			xelement4.Add(new XAttribute("name", "LicensePath"));
			xelement4.Add(new XAttribute("value", Path.Combine(path, this.DeviceLicense)));
			xelement2.Add(xelement4);
			if (string.IsNullOrWhiteSpace(this.Source))
			{
				return new KeyValuePair<Guid, XElement>(key, xelement);
			}
			string extension = Path.GetExtension(this.Source);
			string packageType = extension.Equals(".xap", StringComparison.OrdinalIgnoreCase) ? "XapPath" : "AppxPath";
			XElement xelement5 = xelement2.Elements().SingleOrDefault((XElement x) => x.Attribute("name").Value.Equals(packageType, StringComparison.OrdinalIgnoreCase));
			if (xelement5 != null)
			{
				xelement5.Remove();
			}
			xelement5 = new XElement("parm");
			xelement5.Add(new XAttribute("name", packageType));
			string value = Path.Combine(path, this.DeviceDestination);
			xelement5.Add(new XAttribute("value", value));
			xelement2.Add(xelement5);
			return new KeyValuePair<Guid, XElement>(key, xelement);
		}

		// Token: 0x06000105 RID: 261 RVA: 0x000068C0 File Offset: 0x00004AC0
		public IEnumerable<CustomizationError> VerifyProvXML()
		{
			List<CustomizationError> list = new List<CustomizationError>();
			XElement xelement;
			try
			{
				xelement = XElement.Load(this.ExpandedProvXMLPath);
			}
			catch (XmlException ex)
			{
				list.Add(new CustomizationError(CustomizationErrorSeverity.Error, this.ToEnumerable<Application>(), "Failed to parse Application ProvXML at {0}: {1}", new object[]
				{
					this.ProvXML,
					ex.Message
				}));
				return list;
			}
			IEnumerable<XElement> source = from x in xelement.Elements()
			where x.Attribute("type") != null && x.Attribute("type").Value.Equals("AppInstall", StringComparison.OrdinalIgnoreCase)
			select x;
			if (xelement.Elements().Count<XElement>() != 1 || source.Count<XElement>() != 1)
			{
				list.Add(new CustomizationError(CustomizationErrorSeverity.Error, this.ToEnumerable<Application>(), "Application ProvXML at '{0}' should only have a single characteristic: AppInstall", new object[]
				{
					this.ProvXML
				}));
				return list;
			}
			XElement xelement2 = source.Single<XElement>().Elements().First<XElement>();
			Guid guid;
			if (xelement2.Attribute("type") == null || !Guid.TryParse(xelement2.Attribute("type").Value, out guid))
			{
				if (!string.IsNullOrWhiteSpace(this.Source))
				{
					bool flag = Path.GetExtension(this.Source).Equals(".xap", StringComparison.OrdinalIgnoreCase);
					string value = (xelement2.Attribute("type") != null) ? xelement2.Attribute("type").Value : null;
					string text = flag ? "XAPPackage" : "APPXPackage";
					if (!text.Equals(value, StringComparison.OrdinalIgnoreCase))
					{
						list.Add(new CustomizationError(CustomizationErrorSeverity.Error, this.ToEnumerable<Application>(), "Application ProvXML at '{0}' has a missing or mismatched package type (expecting {1})", new object[]
						{
							this.ProvXML,
							text
						}));
					}
					string pathType = flag ? "XapPath" : "AppxPath";
					if (xelement2.Elements().Count((XElement x) => x.Attribute("name") != null && x.Attribute("name").Value.Equals(pathType, StringComparison.OrdinalIgnoreCase)) > 1)
					{
						list.Add(new CustomizationError(CustomizationErrorSeverity.Error, this.ToEnumerable<Application>(), "Application ProvXML at '{0}' defines multiple application paths", new object[]
						{
							this.ProvXML
						}));
					}
				}
				IEnumerable<XElement> source2 = from x in xelement2.Elements()
				where x.Attribute("name") != null && x.Attribute("name").Value.Equals("ProductID", StringComparison.OrdinalIgnoreCase)
				select x;
				if (source2.Count<XElement>() != 1 || source2.Single<XElement>().Attribute("value") == null)
				{
					list.Add(new CustomizationError(CustomizationErrorSeverity.Error, this.ToEnumerable<Application>(), "Application ProvXML at '{0}' does not define exactly one product ID", new object[]
					{
						this.ProvXML
					}));
				}
				else if (!Guid.TryParse(source2.Single<XElement>().Attribute("value").Value, out guid))
				{
					list.Add(new CustomizationError(CustomizationErrorSeverity.Error, this.ToEnumerable<Application>(), "Application ProvXML at '{0}' does not have a valid Product ID", new object[]
					{
						this.ProvXML
					}));
				}
				if (xelement2.Elements().Count((XElement x) => x.Attribute("name") != null && x.Attribute("name").Value.Equals("LicensePath", StringComparison.OrdinalIgnoreCase)) > 1)
				{
					list.Add(new CustomizationError(CustomizationErrorSeverity.Error, this.ToEnumerable<Application>(), "Application ProvXML at '{0}' defines multiple license paths", new object[]
					{
						this.ProvXML
					}));
				}
				return list;
			}
			IEnumerable<XElement> source3 = from x in xelement2.Elements()
			where x.Attribute("name") != null && x.Attribute("name").Value.Equals("InstallInfo", StringComparison.OrdinalIgnoreCase)
			select x;
			if (xelement2.Elements().Count<XElement>() != 1 || source3.Count<XElement>() != 1)
			{
				list.Add(new CustomizationError(CustomizationErrorSeverity.Error, this.ToEnumerable<Application>(), "Application ProvXML at '{0}' should only have a single element under the GUID characteristic: InstallInfo", new object[]
				{
					this.ProvXML
				}));
				return list;
			}
			XAttribute xattribute = source3.Single<XElement>().Attribute("value");
			if (xattribute == null)
			{
				list.Add(new CustomizationError(CustomizationErrorSeverity.Error, this.ToEnumerable<Application>(), "Application ProvXML at '{0}' has an invalid InstallInfo value.", new object[]
				{
					this.ProvXML
				}));
				return list;
			}
			if (xattribute.Value.Split(new char[]
			{
				';'
			}).Count<string>() < 2)
			{
				list.Add(new CustomizationError(CustomizationErrorSeverity.Error, this.ToEnumerable<Application>(), "Application ProvXML at '{0}' has an invalid InstallInfo value.", new object[]
				{
					this.ProvXML
				}));
				return list;
			}
			return list;
		}

		// Token: 0x04000031 RID: 49
		private static readonly string MainOSPartitionRoot = PkgConstants.c_strDefaultDrive + Path.DirectorySeparatorChar.ToString();

		// Token: 0x04000032 RID: 50
		private static readonly string DataPartitionRoot = PkgConstants.c_strDefaultDrive + PkgConstants.c_strDataPartitionRoot;

		// Token: 0x04000033 RID: 51
		private const string VariantMainOSDestinationRoot = "Programs\\CommonFiles\\Multivariant\\Apps";

		// Token: 0x04000034 RID: 52
		private const string VariantDataDestinationRoot = "SharedData\\Multivariant\\Apps";

		// Token: 0x04000035 RID: 53
		private const string StaticLicenseMainOSDestinationRoot = "Programs\\CommonFiles\\xaps";

		// Token: 0x04000036 RID: 54
		private const string StaticLicenseDataDestinationRoot = "SharedData\\Provisioning\\OEM\\Public";

		// Token: 0x04000037 RID: 55
		private const string StaticDeviceMainOSDestinationRoot = "Programs\\CommonFiles\\xaps";

		// Token: 0x04000038 RID: 56
		private const string StaticDeviceDataDestinationRoot = "SharedData\\Provisioning\\OEM\\Public";

		// Token: 0x04000039 RID: 57
		public const string StaticProvXMLMxipupdatePath = "Windows\\System32\\Migrators\\DuMigrationProvisionerMicrosoft\\provxml";

		// Token: 0x0400003A RID: 58
		private const string StaticProvXMLMainOSDestinationRoot = "Programs\\CommonFiles\\Provisioning\\Microsoft";

		// Token: 0x0400003B RID: 59
		private const string StaticProvXMLDataDestinationRoot = "SharedData\\Provisioning\\OEM";

		// Token: 0x0400003C RID: 60
		private const string XapExtension = ".xap";

		// Token: 0x0400003D RID: 61
		private const string AppInstall = "AppInstall";

		// Token: 0x0400003E RID: 62
		private const string AppxPackage = "APPXPackage";

		// Token: 0x0400003F RID: 63
		private const string XapPackage = "XAPPackage";

		// Token: 0x04000040 RID: 64
		private const string InstallInfo = "InstallInfo";

		// Token: 0x04000041 RID: 65
		private const string CharacteristicElementName = "characteristic";

		// Token: 0x04000042 RID: 66
		private const string CharacteristicType = "type";

		// Token: 0x04000043 RID: 67
		private const string LicensePath = "LicensePath";

		// Token: 0x04000044 RID: 68
		private const string XapPath = "XapPath";

		// Token: 0x04000045 RID: 69
		private const string AppxPath = "AppxPath";

		// Token: 0x04000046 RID: 70
		private const string ParmElementName = "parm";

		// Token: 0x04000047 RID: 71
		private const string ParmName = "name";

		// Token: 0x04000048 RID: 72
		private const string ParmValue = "value";

		// Token: 0x04000049 RID: 73
		private const string ProductID = "ProductID";

		// Token: 0x0400004A RID: 74
		private static readonly string DefaultStaticPartition = PkgConstants.c_strMainOsPartition;

		// Token: 0x0400004B RID: 75
		private static readonly string DefaultVariantPartition = PkgConstants.c_strDataPartition;

		// Token: 0x0400004E RID: 78
		[XmlIgnore]
		public static readonly string SourceFieldName = Strings.txtApplicationSource;

		// Token: 0x04000050 RID: 80
		[XmlIgnore]
		public static readonly string LicenseFieldName = Strings.txtApplicationLicense;

		// Token: 0x04000052 RID: 82
		[XmlIgnore]
		public static readonly string ProvXMLFieldName = Strings.txtApplicationProvXML;

		// Token: 0x04000054 RID: 84
		private string _targetPartition;
	}
}
