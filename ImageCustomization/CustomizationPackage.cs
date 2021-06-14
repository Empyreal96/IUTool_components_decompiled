using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Microsoft.Composition.Packaging;
using Microsoft.Composition.ToolBox;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;

namespace Microsoft.WindowsPhone.ImageUpdate.Customization
{
	// Token: 0x0200000A RID: 10
	internal class CustomizationPackage
	{
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000084 RID: 132 RVA: 0x0000580A File Offset: 0x00003A0A
		// (set) Token: 0x06000085 RID: 133 RVA: 0x00005812 File Offset: 0x00003A12
		public string Owner { get; set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000086 RID: 134 RVA: 0x0000581B File Offset: 0x00003A1B
		// (set) Token: 0x06000087 RID: 135 RVA: 0x00005823 File Offset: 0x00003A23
		public string Component { get; set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000088 RID: 136 RVA: 0x0000582C File Offset: 0x00003A2C
		// (set) Token: 0x06000089 RID: 137 RVA: 0x00005834 File Offset: 0x00003A34
		public string SubComponent { get; set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600008A RID: 138 RVA: 0x0000583D File Offset: 0x00003A3D
		// (set) Token: 0x0600008B RID: 139 RVA: 0x00005845 File Offset: 0x00003A45
		public OwnerType OwnerType { get; set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600008C RID: 140 RVA: 0x0000584E File Offset: 0x00003A4E
		// (set) Token: 0x0600008D RID: 141 RVA: 0x00005856 File Offset: 0x00003A56
		public ReleaseType ReleaseType { get; set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600008E RID: 142 RVA: 0x0000585F File Offset: 0x00003A5F
		// (set) Token: 0x0600008F RID: 143 RVA: 0x00005867 File Offset: 0x00003A67
		public string Partition { get; set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000090 RID: 144 RVA: 0x00005870 File Offset: 0x00003A70
		// (set) Token: 0x06000091 RID: 145 RVA: 0x00005878 File Offset: 0x00003A78
		public CpuId CpuType { get; set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000092 RID: 146 RVA: 0x00005881 File Offset: 0x00003A81
		// (set) Token: 0x06000093 RID: 147 RVA: 0x00005889 File Offset: 0x00003A89
		public Microsoft.WindowsPhone.ImageUpdate.PkgCommon.BuildType BuildType { get; set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000094 RID: 148 RVA: 0x00005892 File Offset: 0x00003A92
		// (set) Token: 0x06000095 RID: 149 RVA: 0x0000589A File Offset: 0x00003A9A
		public VersionInfo Version { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000096 RID: 150 RVA: 0x000058A3 File Offset: 0x00003AA3
		// (set) Token: 0x06000097 RID: 151 RVA: 0x000058AB File Offset: 0x00003AAB
		public List<CustomizationFile> Files { get; private set; }

		// Token: 0x06000098 RID: 152 RVA: 0x000058B4 File Offset: 0x00003AB4
		public CustomizationPackage() : this(Microsoft.WindowsPhone.ImageUpdate.PkgCommon.PkgConstants.c_strMainOsPartition)
		{
		}

		// Token: 0x06000099 RID: 153 RVA: 0x000058C4 File Offset: 0x00003AC4
		public CustomizationPackage(string partition)
		{
			this.Files = new List<CustomizationFile>();
			this.Component = "Device";
			this.SubComponent = "Customizations";
			this.OwnerType = OwnerType.OEM;
			this.BuildType = Microsoft.WindowsPhone.ImageUpdate.PkgCommon.BuildType.Retail;
			this.ReleaseType = ReleaseType.Production;
			this.Partition = partition;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00005914 File Offset: 0x00003B14
		public void AddFile(string sourcePath, string destinationPath)
		{
			this.AddFile(Microsoft.WindowsPhone.ImageUpdate.PkgCommon.FileType.Regular, sourcePath, destinationPath);
		}

		// Token: 0x0600009B RID: 155 RVA: 0x0000591F File Offset: 0x00003B1F
		public void AddFile(Microsoft.WindowsPhone.ImageUpdate.PkgCommon.FileType type, string sourcePath, string destinationPath)
		{
			this.Files.Add(new CustomizationFile(type, sourcePath, destinationPath));
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00005934 File Offset: 0x00003B34
		public void AddFile(CustomizationFile customizationFile)
		{
			this.AddFile(customizationFile.Source, customizationFile.Destination);
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00005948 File Offset: 0x00003B48
		public void AddFiles(IEnumerable<CustomizationFile> customizationFiles)
		{
			foreach (CustomizationFile customizationFile in customizationFiles)
			{
				this.AddFile(customizationFile);
			}
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00005990 File Offset: 0x00003B90
		public string SavePackage(string folderPath)
		{
			CbsPackage cbsPackage = new CbsPackage();
			cbsPackage.Owner = this.Owner;
			cbsPackage.OwnerType = ManifestToolBox.ConvertOwnerType(this.OwnerType.ToString());
			cbsPackage.Component = this.Component;
			cbsPackage.SubComponent = this.SubComponent + "." + this.Partition;
			cbsPackage.Partition = this.Partition;
			cbsPackage.PhoneReleaseType = ManifestToolBox.ConvertReleaseType(this.ReleaseType.ToString());
			cbsPackage.HostArch = ManifestToolBox.ConvertCpuIdToCpuArch(this.CpuType.ToString());
			cbsPackage.Version = new Version(this.Version.ToString());
			cbsPackage.BuildType = ManifestToolBox.ConvertBuildType(this.BuildType.ToString());
			foreach (CustomizationFile customizationFile in this.Files)
			{
				cbsPackage.AddFile(ManifestToolBox.ConvertFileType(customizationFile.FileType.ToString()), customizationFile.Source, customizationFile.Destination, null);
			}
			string text = Path.Combine(Path.GetFullPath(folderPath), cbsPackage.PackageName + Microsoft.Composition.ToolBox.PkgConstants.CBSPackageExtension);
			cbsPackage.SaveCab(text);
			return text;
		}

		// Token: 0x0400001C RID: 28
		public static readonly XNamespace PackageNamespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00";

		// Token: 0x0400001D RID: 29
		public const string StaticApplicationsName = "StaticApps";

		// Token: 0x0400001E RID: 30
		public const string VariantApplicationsName = "VariantApps";

		// Token: 0x0400001F RID: 31
		public const string ShadowRegFileName = "OEMSettings.reg";

		// Token: 0x04000020 RID: 32
		public static readonly string ShadowRegFilePath = Microsoft.WindowsPhone.ImageUpdate.PkgCommon.PkgConstants.c_strRguDeviceFolder + "\\OEMSettings.reg";

		// Token: 0x04000021 RID: 33
		private const string PkgGen = "PkgGen.exe";

		// Token: 0x04000022 RID: 34
		private const string PkgGenArguments = "\"{0}\" /output:\"{1}\" /build:{2} /cpu:{3} /version:{4}";

		// Token: 0x04000023 RID: 35
		private const string PackageXmlExtension = ".pkg.xml";
	}
}
