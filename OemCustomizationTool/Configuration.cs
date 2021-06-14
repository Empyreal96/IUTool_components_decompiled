using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.WindowsPhone.ImageUpdate.OemCustomizationTool
{
	// Token: 0x02000005 RID: 5
	internal class Configuration
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600001C RID: 28 RVA: 0x00002891 File Offset: 0x00000A91
		// (set) Token: 0x0600001D RID: 29 RVA: 0x00002899 File Offset: 0x00000A99
		public List<XmlFile> XmlFiles
		{
			get
			{
				return this.xmlFiles;
			}
			set
			{
				this.xmlFiles = value;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600001E RID: 30 RVA: 0x000028A2 File Offset: 0x00000AA2
		// (set) Token: 0x0600001F RID: 31 RVA: 0x000028AA File Offset: 0x00000AAA
		public XDocument ConfigXmlDoc
		{
			get
			{
				return this.configXmlDoc;
			}
			set
			{
				this.configXmlDoc = value;
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000028B4 File Offset: 0x00000AB4
		public Configuration(List<XmlFile> files)
		{
			if (files == null || files.Count == 0)
			{
				TraceLogger.LogMessage(TraceLevel.Error, "Received an empty config file list.", true);
				return;
			}
			try
			{
				List<XmlFile> list = files;
				this.ConfigXmlDoc = XmlFileHandler.LoadXmlDoc(ref list);
				this.XmlFiles = list;
				this.FindDuplicateEntries();
				this.IsConfigValid = true;
			}
			catch (Exception ex)
			{
				TraceLogger.LogMessage(TraceLevel.Error, ex.ToString(), true);
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002928 File Offset: 0x00000B28
		private void FindDuplicateEntries()
		{
			TraceLogger.LogMessage(TraceLevel.Info, "Checking for duplicates in Config files.", true);
			IEnumerable<XElement> enumerable = this.configXmlDoc.Descendants("Mapping");
			foreach (XElement xelement in enumerable)
			{
				TraceLogger.LogMessage(TraceLevel.Info, xelement.ToString(), true);
			}
			IEnumerable<XElement> enumerable2 = enumerable.Descendants("SettingMapping");
			Configuration.SettingMappingComparer comparer = new Configuration.SettingMappingComparer();
			IEnumerable<XElement> enumerable3 = enumerable2.Distinct(comparer);
			foreach (XElement xelement2 in enumerable3)
			{
				TraceLogger.LogMessage(TraceLevel.Info, xelement2.ToString(), true);
			}
			if (enumerable2.Count<XElement>() != enumerable3.Count<XElement>())
			{
				IEnumerable<XElement> enumerable4 = enumerable2.Except(enumerable3);
				TraceLogger.LogMessage(TraceLevel.Error, "Found duplicate settings in config:", true);
				foreach (XElement xelement3 in enumerable4)
				{
					TraceLogger.LogMessage(TraceLevel.Error, xelement3.ToString(), true);
				}
				throw new CustomizationXmlException("There are duplicate SettingMapping entries in your config.");
			}
		}

		// Token: 0x04000010 RID: 16
		public const string c_strMapping = "Mapping";

		// Token: 0x04000011 RID: 17
		public const string c_strSettingMapping = "SettingMapping";

		// Token: 0x04000012 RID: 18
		public const string c_strRegKeyName = "RegKeyName";

		// Token: 0x04000013 RID: 19
		public const string c_strSettingsGroup = "SettingsGroup";

		// Token: 0x04000014 RID: 20
		public const string c_strSettingsGroupMapping = "SettingsGroupMapping";

		// Token: 0x04000015 RID: 21
		public const string c_strRegKeyBaseName = "RegKeyBaseName";

		// Token: 0x04000016 RID: 22
		public const string c_strRegName = "RegName";

		// Token: 0x04000017 RID: 23
		public const string c_strRegType = "RegType";

		// Token: 0x04000018 RID: 24
		public const string c_strName = "Name";

		// Token: 0x04000019 RID: 25
		public const string c_strComponentMapping = "ComponentMapping";

		// Token: 0x0400001A RID: 26
		public const string c_strComponentName = "ComponentName";

		// Token: 0x0400001B RID: 27
		public const string c_strPartition = "Partition";

		// Token: 0x0400001C RID: 28
		private List<XmlFile> xmlFiles;

		// Token: 0x0400001D RID: 29
		private XDocument configXmlDoc;

		// Token: 0x0400001E RID: 30
		public bool IsConfigValid;

		// Token: 0x02000014 RID: 20
		private class SettingMappingComparer : IEqualityComparer<XElement>
		{
			// Token: 0x0600006F RID: 111 RVA: 0x00004CC4 File Offset: 0x00002EC4
			public bool Equals(XElement x, XElement y)
			{
				bool flag = false;
				bool flag2 = false;
				if (x.Attribute("RegKeyName") != null && y.Attribute("RegKeyName") != null)
				{
					flag2 = (x.Attribute("RegKeyName").Value == y.Attribute("RegKeyName").Value);
				}
				else if (x.Ancestors("SettingsGroupMapping").First<XElement>().Attribute("RegKeyBaseName") != null && y.Ancestors("SettingsGroupMapping").First<XElement>().Attribute("RegKeyBaseName") != null)
				{
					flag2 = (x.Ancestors("SettingsGroupMapping").First<XElement>().Attribute("RegKeyBaseName").Value == y.Ancestors("SettingsGroupMapping").First<XElement>().Attribute("RegKeyBaseName").Value);
				}
				else
				{
					flag = false;
				}
				if (flag2)
				{
					if (x.Attribute("RegName") != null && y.Attribute("RegName") != null)
					{
						flag = (x.Attribute("RegName").Value == y.Attribute("RegName").Value);
					}
					else
					{
						flag = (x.Attribute("Name").Value == y.Attribute("Name").Value);
					}
				}
				if (!flag && x.Attribute("Name").Value == y.Attribute("Name").Value)
				{
					flag = (x.Ancestors("ComponentMapping").First<XElement>().Attribute("ComponentName").Value == y.Ancestors("ComponentMapping").First<XElement>().Attribute("ComponentName").Value);
				}
				if (flag)
				{
					string a = (x.Ancestors("SettingsGroupMapping").First<XElement>().Attribute("Partition") != null) ? x.Ancestors("SettingsGroupMapping").First<XElement>().Attribute("Partition").Value : Settings.PackageAttributes.mainOSPartitionString;
					string b = (y.Ancestors("SettingsGroupMapping").First<XElement>().Attribute("Partition") != null) ? y.Ancestors("SettingsGroupMapping").First<XElement>().Attribute("Partition").Value : Settings.PackageAttributes.mainOSPartitionString;
					flag = (a == b);
				}
				return flag;
			}

			// Token: 0x06000070 RID: 112 RVA: 0x00004FAF File Offset: 0x000031AF
			public int GetHashCode(XElement obj)
			{
				return base.GetHashCode();
			}
		}
	}
}
