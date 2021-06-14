using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.OemCustomizationTool
{
	// Token: 0x02000007 RID: 7
	internal class CustomizationPkgBuilder
	{
		// Token: 0x0600002A RID: 42 RVA: 0x00002DE4 File Offset: 0x00000FE4
		public CustomizationPkgBuilder(Customization cust, Configuration conf)
		{
			this.customization = cust;
			this.config = conf;
			this.regFileHandler = new RegFileHandler();
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002E08 File Offset: 0x00001008
		internal void GenerateCustomizationPackage()
		{
			try
			{
				foreach (RegFilePartitionInfo regFilePartitionInfo in this.GenerateCustomizationRegFiles())
				{
					using (IPkgBuilder pkgBuilder = Package.Create())
					{
						pkgBuilder.Owner = Settings.PackageAttributes.Owner;
						pkgBuilder.OwnerType = Settings.PackageAttributes.OwnerType;
						pkgBuilder.Component = regFilePartitionInfo.partition + ".ImageCustomization";
						pkgBuilder.SubComponent = "RegistryCustomization";
						pkgBuilder.Partition = regFilePartitionInfo.partition;
						pkgBuilder.ReleaseType = Settings.PackageAttributes.ReleaseType;
						pkgBuilder.CpuType = Settings.PackageAttributes.CpuType;
						pkgBuilder.Version = Settings.PackageAttributes.Version;
						pkgBuilder.BuildType = Settings.PackageAttributes.BuildType;
						pkgBuilder.AddFile(FileType.Registry, regFilePartitionInfo.regFilename, Settings.PackageAttributes.DeviceRegFilePath + "OEMSettings.reg", FileAttributes.System, null, "None");
						foreach (XmlFile xmlFile in this.customization.XmlFiles)
						{
							pkgBuilder.AddFile(FileType.Regular, xmlFile.Filename, Settings.PackageAttributes.DeviceLogFilePath + Path.GetFileName(xmlFile.Filename), FileAttributes.System, null, "None");
						}
						foreach (XmlFile xmlFile2 in this.config.XmlFiles)
						{
							pkgBuilder.AddFile(FileType.Regular, xmlFile2.Filename, Settings.PackageAttributes.DeviceLogFilePath + Path.GetFileName(xmlFile2.Filename), FileAttributes.System, null, "None");
						}
						TraceLogger.LogMessage(TraceLevel.Info, "Output Directory: " + Settings.OutputPkgFilePath, true);
						TraceLogger.LogMessage(TraceLevel.Info, "Output File Name: " + pkgBuilder.Name + PkgConstants.c_strPackageExtension, true);
						string cabPath = Path.Combine(Path.GetFullPath(Settings.OutputPkgFilePath), pkgBuilder.Name + PkgConstants.c_strPackageExtension);
						pkgBuilder.SaveCab(cabPath);
					}
				}
			}
			catch (Exception ex)
			{
				TraceLogger.LogMessage(TraceLevel.Error, ex.ToString(), true);
			}
		}

		// Token: 0x0600002C RID: 44 RVA: 0x0000309C File Offset: 0x0000129C
		private List<RegFilePartitionInfo> GenerateCustomizationRegFiles()
		{
			XContainer customizationXmlDoc = this.customization.CustomizationXmlDoc;
			XDocument configXmlDoc = this.config.ConfigXmlDoc;
			IEnumerable<XElement> enumerable = customizationXmlDoc.Descendants("Component");
			foreach (XElement xelement in enumerable)
			{
				TraceLogger.LogMessage(TraceLevel.Info, xelement.ToString(), true);
			}
			foreach (string text in from comp in enumerable
			select comp.Attribute("ComponentName").Value)
			{
				TraceLogger.LogMessage(TraceLevel.Info, text.ToString(), true);
			}
			IEnumerable<XElement> enumerable2 = enumerable.Descendants("SettingsGroup");
			foreach (XElement xelement2 in enumerable2)
			{
				TraceLogger.LogMessage(TraceLevel.Info, xelement2.ToString(), true);
			}
			using (var enumerator3 = (from custSetting in enumerable2.Descendants("Setting")
			select new
			{
				Name = custSetting.Attribute("Name").Value,
				Value = custSetting.Attribute("Value").Value,
				CustomValueName = ((custSetting.Attribute("CustomName") == null) ? string.Empty : custSetting.Attribute("CustomName").Value),
				CustomKeyName = ((custSetting.Parent.Attribute("Key") == null) ? string.Empty : custSetting.Parent.Attribute("Key").Value),
				Component = custSetting.Ancestors("Component").First<XElement>().Attribute("ComponentName").Value,
				Partition = (string)custSetting.Parent.Attribute("Partition")
			}).GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					<>f__AnonymousType0<string, string, string, string, string, string> custSetting = enumerator3.Current;
					var enumerable3 = from confSetting in configXmlDoc.Descendants("SettingMapping")
					where confSetting.Attribute("Name").Value == custSetting.Name && confSetting.Ancestors("ComponentMapping").First<XElement>().Attribute("ComponentName").Value == custSetting.Component && (string)confSetting.Parent.Attribute("Partition") == custSetting.Partition && confSetting.Attribute("RegKeyName") != null && confSetting.Attribute("RegName") != null
					select new
					{
						RegKeyName = confSetting.Attribute("RegKeyName").Value,
						RegName = ((confSetting.Attribute("RegName") != null) ? confSetting.Attribute("RegName").Value : confSetting.Attribute("Name").Value),
						RegType = confSetting.Attribute("RegType").Value,
						Partition = custSetting.Partition
					};
					if (enumerable3 == null || enumerable3.Count() == 0)
					{
						if (custSetting.CustomValueName.Equals(string.Empty) && custSetting.CustomKeyName.Equals(string.Empty))
						{
							string text2 = string.Concat(new string[]
							{
								"There is no config entry for the specified customization: CustSettingName = ",
								custSetting.Name,
								"; CustSettingValue = ",
								custSetting.Value,
								". Are you sure this customization is allowed?"
							});
							if (!Settings.WarnOnMappingNotFound)
							{
								throw new ConfigXmlException(text2);
							}
							TraceLogger.LogMessage(TraceLevel.Warn, text2, true);
						}
					}
					else
					{
						foreach (var <>f__AnonymousType in enumerable3.Distinct())
						{
							TraceLogger.LogMessage(TraceLevel.Info, string.Concat(new string[]
							{
								"CustSettingName = ",
								custSetting.Name,
								"; CustSettingValue = ",
								custSetting.Value,
								"; ConfRegKeyName = ",
								<>f__AnonymousType.RegKeyName,
								"; ConfRegName = ",
								<>f__AnonymousType.RegName,
								"; ConfRegType = ",
								<>f__AnonymousType.RegType
							}), true);
							RegValueInfo regValueInfo = new RegValueInfo
							{
								KeyName = <>f__AnonymousType.RegKeyName,
								ValueName = <>f__AnonymousType.RegName,
								Value = custSetting.Value
							};
							regValueInfo.SetRegValueType(<>f__AnonymousType.RegType);
							this.regFileHandler.AddRegValue(regValueInfo, <>f__AnonymousType.Partition);
						}
					}
				}
			}
			using (var enumerator3 = (from setting in enumerable2.Descendants("Setting")
			where setting.Attribute("CustomName") != null
			select new
			{
				Name = setting.Attribute("Name").Value,
				Value = setting.Attribute("Value").Value,
				CustomValueName = setting.Attribute("CustomName").Value,
				CustomKeyName = ((setting.Parent.Attribute("Key") == null) ? string.Empty : setting.Parent.Attribute("Key").Value),
				Component = setting.Ancestors("Component").First<XElement>().Attribute("ComponentName").Value,
				Partition = (string)setting.Parent.Attribute("Partition")
			}).GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					<>f__AnonymousType0<string, string, string, string, string, string> custSetting = enumerator3.Current;
					var enumerable4 = from confSetting in configXmlDoc.Descendants("SettingMapping")
					where confSetting.Attribute("Name").Value == custSetting.Name && confSetting.Ancestors("ComponentMapping").First<XElement>().Attribute("ComponentName").Value == custSetting.Component && (string)confSetting.Parent.Attribute("Partition") == custSetting.Partition && confSetting.Attribute("RegKeyName") != null && confSetting.Attribute("RegName") == null
					select new
					{
						RegKeyName = confSetting.Attribute("RegKeyName").Value,
						RegName = custSetting.CustomValueName,
						RegType = confSetting.Attribute("RegType").Value,
						Partition = custSetting.Partition
					};
					if (enumerable4 == null || enumerable4.Count() == 0)
					{
						if (custSetting.CustomKeyName.Equals(string.Empty))
						{
							string text3 = string.Concat(new string[]
							{
								"There is no config entry for the specified customization: CustSettingName = ",
								custSetting.Name,
								"; CustSettingValue = ",
								custSetting.Value,
								". Are you sure this customization is allowed?"
							});
							if (!Settings.WarnOnMappingNotFound)
							{
								throw new ConfigXmlException(text3);
							}
							TraceLogger.LogMessage(TraceLevel.Warn, text3, true);
						}
					}
					else
					{
						foreach (var <>f__AnonymousType2 in enumerable4.Distinct())
						{
							TraceLogger.LogMessage(TraceLevel.Info, string.Concat(new string[]
							{
								"CustSettingName = ",
								custSetting.Name,
								"; CustSettingValue = ",
								custSetting.Value,
								"; ConfRegKeyName = ",
								<>f__AnonymousType2.RegKeyName,
								"; ConfRegName = ",
								<>f__AnonymousType2.RegName,
								"; ConfRegType = ",
								<>f__AnonymousType2.RegType
							}), true);
							RegValueInfo regValueInfo2 = new RegValueInfo
							{
								KeyName = <>f__AnonymousType2.RegKeyName,
								ValueName = <>f__AnonymousType2.RegName,
								Value = custSetting.Value
							};
							regValueInfo2.SetRegValueType(<>f__AnonymousType2.RegType);
							this.regFileHandler.AddRegValue(regValueInfo2, <>f__AnonymousType2.Partition);
						}
					}
				}
			}
			using (var enumerator5 = (from setting in enumerable2.Descendants("Setting")
			where setting.Parent.Attribute("Key") != null
			select new
			{
				Name = setting.Attribute("Name").Value,
				Value = setting.Attribute("Value").Value,
				CustomKeyName = setting.Parent.Attribute("Key").Value,
				Component = setting.Ancestors("Component").First<XElement>().Attribute("ComponentName").Value,
				Partition = (string)setting.Parent.Attribute("Partition")
			}).GetEnumerator())
			{
				while (enumerator5.MoveNext())
				{
					<>f__AnonymousType2<string, string, string, string, string> custSetting = enumerator5.Current;
					var enumerable5 = from confSetting in configXmlDoc.Descendants("SettingMapping")
					where confSetting.Attribute("Name").Value == custSetting.Name && confSetting.Ancestors("ComponentMapping").First<XElement>().Attribute("ComponentName").Value == custSetting.Component && (string)confSetting.Parent.Attribute("Partition") == custSetting.Partition && confSetting.Parent.Attribute("RegKeyBaseName") != null && confSetting.Parent.Attribute("RegKeyBaseName").Value.Contains("%%KEY%%") && confSetting.Attribute("RegKeyName") == null
					select new
					{
						RegKeyName = confSetting.Parent.Attribute("RegKeyBaseName").Value.Replace("%%KEY%%", custSetting.CustomKeyName),
						RegName = ((confSetting.Attribute("RegName") != null) ? confSetting.Attribute("RegName").Value : confSetting.Attribute("Name").Value),
						RegType = confSetting.Attribute("RegType").Value,
						Partition = custSetting.Partition
					};
					if (enumerable5 == null || enumerable5.Count() == 0)
					{
						string text4 = string.Concat(new string[]
						{
							"There is no config entry for the specified customization: CustSettingName = ",
							custSetting.Name,
							"; CustSettingValue = ",
							custSetting.Value,
							". Are you sure this customization is allowed?"
						});
						if (!Settings.WarnOnMappingNotFound)
						{
							throw new ConfigXmlException(text4);
						}
						TraceLogger.LogMessage(TraceLevel.Warn, text4, true);
					}
					else
					{
						foreach (var <>f__AnonymousType3 in enumerable5.Distinct())
						{
							TraceLogger.LogMessage(TraceLevel.Info, string.Concat(new string[]
							{
								"CustSettingName = ",
								custSetting.Name,
								"; CustSettingValue = ",
								custSetting.Value,
								"; ConfRegKeyName = ",
								<>f__AnonymousType3.RegKeyName,
								"; ConfRegName = ",
								<>f__AnonymousType3.RegName,
								"; ConfRegType = ",
								<>f__AnonymousType3.RegType
							}), true);
							RegValueInfo regValueInfo3 = new RegValueInfo
							{
								KeyName = <>f__AnonymousType3.RegKeyName,
								ValueName = <>f__AnonymousType3.RegName,
								Value = custSetting.Value
							};
							regValueInfo3.SetRegValueType(<>f__AnonymousType3.RegType);
							this.regFileHandler.AddRegValue(regValueInfo3, <>f__AnonymousType3.Partition);
						}
					}
				}
			}
			return this.regFileHandler.Write();
		}

		// Token: 0x0400002B RID: 43
		public const string c_strOemCustomizationToolRegkeys = "OemCustomizationToolRegkeys";

		// Token: 0x0400002C RID: 44
		public const string c_strRegKeys = "RegKeys";

		// Token: 0x0400002D RID: 45
		public const string c_strRegKey = "RegKey";

		// Token: 0x0400002E RID: 46
		public const string c_strKeyName = "KeyName";

		// Token: 0x0400002F RID: 47
		public const string c_strRegValue = "RegValue";

		// Token: 0x04000030 RID: 48
		public const string c_strName = "Name";

		// Token: 0x04000031 RID: 49
		public const string c_strValue = "Value";

		// Token: 0x04000032 RID: 50
		public const string c_strType = "Type";

		// Token: 0x04000033 RID: 51
		private Customization customization;

		// Token: 0x04000034 RID: 52
		private Configuration config;

		// Token: 0x04000035 RID: 53
		private RegFileHandler regFileHandler;
	}
}
