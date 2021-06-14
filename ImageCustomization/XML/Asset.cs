using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.MCSF.Offline;

namespace Microsoft.WindowsPhone.ImageUpdate.Customization.XML
{
	// Token: 0x02000016 RID: 22
	[XmlRoot(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class Asset : IDefinedIn
	{
		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000157 RID: 343 RVA: 0x00007EDC File Offset: 0x000060DC
		// (set) Token: 0x06000158 RID: 344 RVA: 0x00007EE4 File Offset: 0x000060E4
		[XmlIgnore]
		public string DefinedInFile { get; set; }

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000159 RID: 345 RVA: 0x00007EED File Offset: 0x000060ED
		// (set) Token: 0x0600015A RID: 346 RVA: 0x00007EF5 File Offset: 0x000060F5
		[XmlAttribute]
		public string Name { get; set; }

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x0600015B RID: 347 RVA: 0x00007EFE File Offset: 0x000060FE
		// (set) Token: 0x0600015C RID: 348 RVA: 0x00007F06 File Offset: 0x00006106
		[XmlAttribute]
		public string Source { get; set; }

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x0600015D RID: 349 RVA: 0x00007F0F File Offset: 0x0000610F
		[XmlIgnore]
		public string ExpandedSourcePath
		{
			get
			{
				return ImageCustomizations.ExpandPath(this.Source);
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x0600015E RID: 350 RVA: 0x00007F1C File Offset: 0x0000611C
		// (set) Token: 0x0600015F RID: 351 RVA: 0x00007F24 File Offset: 0x00006124
		[XmlAttribute]
		public string TargetFileName { get; set; }

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06000160 RID: 352 RVA: 0x00007F2D File Offset: 0x0000612D
		// (set) Token: 0x06000161 RID: 353 RVA: 0x00007F35 File Offset: 0x00006135
		[XmlAttribute]
		public string DisplayName { get; set; }

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000162 RID: 354 RVA: 0x00007F3E File Offset: 0x0000613E
		// (set) Token: 0x06000163 RID: 355 RVA: 0x00007F46 File Offset: 0x00006146
		[XmlAttribute]
		public CustomizationAssetOwner Type { get; set; }

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000164 RID: 356 RVA: 0x00007F4F File Offset: 0x0000614F
		[XmlIgnore]
		public string Id
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(this.TargetFileName))
				{
					return this.TargetFileName;
				}
				return Path.GetFileName(this.Source);
			}
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00007F70 File Offset: 0x00006170
		public Asset()
		{
			this.Type = CustomizationAssetOwner.OEM;
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00007F7F File Offset: 0x0000617F
		public string GetDevicePath(string deviceRoot)
		{
			return Path.Combine(deviceRoot, this.Id);
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00007F90 File Offset: 0x00006190
		public string GetDevicePathWithMacros(PolicyAssetInfo policy)
		{
			string text = this.GetDevicePath(policy.TargetDir);
			if (policy.HasOEMMacros)
			{
				text = new PolicyMacroTable(policy.Name, this.Name).ReplaceMacros(text);
			}
			return text;
		}

		// Token: 0x04000073 RID: 115
		[XmlIgnore]
		public static readonly string SourceFieldName = Strings.txtAssetSource;
	}
}
