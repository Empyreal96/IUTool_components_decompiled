using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.WindowsPhone.ImageUpdate.OemCustomizationTool
{
	// Token: 0x02000006 RID: 6
	internal class Customization
	{
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000022 RID: 34 RVA: 0x00002A74 File Offset: 0x00000C74
		// (set) Token: 0x06000023 RID: 35 RVA: 0x00002A7C File Offset: 0x00000C7C
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

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000024 RID: 36 RVA: 0x00002A85 File Offset: 0x00000C85
		// (set) Token: 0x06000025 RID: 37 RVA: 0x00002A8D File Offset: 0x00000C8D
		public XDocument CustomizationXmlDoc
		{
			get
			{
				return this.customizationXmlDoc;
			}
			set
			{
				this.customizationXmlDoc = value;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000026 RID: 38 RVA: 0x00002A96 File Offset: 0x00000C96
		// (set) Token: 0x06000027 RID: 39 RVA: 0x00002A9E File Offset: 0x00000C9E
		public bool IsCustomizationValid { get; set; }

		// Token: 0x06000028 RID: 40 RVA: 0x00002AA8 File Offset: 0x00000CA8
		public Customization(List<XmlFile> files)
		{
			if (files == null || files.Count == 0)
			{
				TraceLogger.LogMessage(TraceLevel.Error, "Received an empty customization file list.", true);
				return;
			}
			try
			{
				List<XmlFile> list = files;
				this.customizationXmlDoc = XmlFileHandler.LoadXmlDoc(ref list);
				this.XmlFiles = list;
				this.ParsePackageAttributes();
				this.IsCustomizationValid = true;
			}
			catch (Exception ex)
			{
				TraceLogger.LogMessage(TraceLevel.Error, ex.ToString(), true);
			}
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002B1C File Offset: 0x00000D1C
		private void ParsePackageAttributes()
		{
			IEnumerable<XElement> enumerable = this.customizationXmlDoc.Descendants("OEMCustomizationPackage");
			foreach (XElement xelement in enumerable)
			{
				TraceLogger.LogMessage(TraceLevel.Info, xelement.ToString(), true);
			}
			TraceLogger.LogMessage(TraceLevel.Info, "Getting package owner names:", true);
			IEnumerable<string> enumerable2 = from item in enumerable
			where item.Name.LocalName == "OEMCustomizationPackage"
			select item.Attribute("Owner").Value;
			foreach (string text in enumerable2)
			{
				TraceLogger.LogMessage(TraceLevel.Info, text.ToString(), true);
			}
			TraceLogger.LogMessage(TraceLevel.Warn, "Found multiple owner names (likely due to includes). Using '" + enumerable2.Last<string>() + "' to generate package.", enumerable2.Count<string>() > 1);
			Settings.PackageAttributes.Owner = enumerable2.Last<string>();
			TraceLogger.LogMessage(TraceLevel.Info, "Getting package owner types:", true);
			IEnumerable<string> enumerable3 = from item in enumerable
			where item.Name.LocalName == "OEMCustomizationPackage"
			select item.Attribute("OwnerType").Value;
			foreach (string text2 in enumerable3)
			{
				TraceLogger.LogMessage(TraceLevel.Info, text2.ToString(), true);
			}
			TraceLogger.LogMessage(TraceLevel.Warn, "Found multiple owner types (likely due to includes). Using '" + enumerable3.Last<string>() + "' to generate package.", enumerable3.Count<string>() > 1);
			Settings.PackageAttributes.OwnerTypeString = enumerable3.Last<string>();
			TraceLogger.LogMessage(TraceLevel.Info, "Getting package release types:", true);
			IEnumerable<string> enumerable4 = from item in enumerable
			where item.Name.LocalName == "OEMCustomizationPackage"
			select item.Attribute("ReleaseType").Value;
			foreach (string text3 in enumerable4)
			{
				TraceLogger.LogMessage(TraceLevel.Info, text3.ToString(), true);
			}
			TraceLogger.LogMessage(TraceLevel.Warn, "Found multiple release types (likely due to includes). Using '" + enumerable4.Last<string>() + "' to generate package.", enumerable4.Count<string>() > 1);
			Settings.PackageAttributes.ReleaseTypeString = enumerable4.Last<string>();
		}

		// Token: 0x0400001F RID: 31
		public const string c_strOwner = "Owner";

		// Token: 0x04000020 RID: 32
		public const string c_strOwnerType = "OwnerType";

		// Token: 0x04000021 RID: 33
		public const string c_strReleaseType = "ReleaseType";

		// Token: 0x04000022 RID: 34
		public const string c_strOEMCustomizationPackage = "OEMCustomizationPackage";

		// Token: 0x04000023 RID: 35
		public const string c_strComponent = "Component";

		// Token: 0x04000024 RID: 36
		public const string c_strComponentName = "ComponentName";

		// Token: 0x04000025 RID: 37
		public const string c_strSetting = "Setting";

		// Token: 0x04000026 RID: 38
		public const string c_strCustomName = "CustomName";

		// Token: 0x04000027 RID: 39
		public const string c_strKey = "Key";

		// Token: 0x04000028 RID: 40
		private List<XmlFile> xmlFiles;

		// Token: 0x04000029 RID: 41
		private XDocument customizationXmlDoc;
	}
}
