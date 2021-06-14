using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x02000026 RID: 38
	public class Edition
	{
		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000D2 RID: 210 RVA: 0x00007EDC File Offset: 0x000060DC
		[XmlIgnore]
		public string MSPackageRoot
		{
			get
			{
				if (string.IsNullOrEmpty(this._msPackageRoot) && !string.IsNullOrEmpty(this.InstallRoot))
				{
					string text = Path.Combine(this.InstallRoot, "MSPackageRoot");
					if (LongPathDirectory.Exists(text))
					{
						this._msPackageRoot = text;
					}
					else
					{
						text = Path.Combine(this.InstallRoot, "Prebuilt");
						if (LongPathDirectory.Exists(text))
						{
							this._msPackageRoot = text;
						}
					}
				}
				return this._msPackageRoot;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000D3 RID: 211 RVA: 0x00007F4B File Offset: 0x0000614B
		[XmlIgnore]
		public bool IsInstalled
		{
			get
			{
				return this.InstalledCPUTypes != null && this.InstalledCPUTypes.Count > 0;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000D4 RID: 212 RVA: 0x00007F68 File Offset: 0x00006168
		[XmlIgnore]
		public string InstallRoot
		{
			get
			{
				if (string.IsNullOrEmpty(this._installRoot))
				{
					foreach (EditionLookup editionLookup in this.UISettings.Lookups)
					{
						if (!string.IsNullOrWhiteSpace(editionLookup.InstallPath))
						{
							foreach (string cpuType in from cpuitem in this.SupportedCPUTypes
							select cpuitem.CpuType)
							{
								if (this.FMPackagesFound(Path.Combine(editionLookup.InstallPath, editionLookup.MSPackageDirectoryName), cpuType))
								{
									this._installRoot = editionLookup.InstallPath;
									this._msPackageRoot = Path.Combine(this._installRoot, editionLookup.MSPackageDirectoryName);
									break;
								}
							}
						}
					}
				}
				return this._installRoot;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000D5 RID: 213 RVA: 0x00008080 File Offset: 0x00006280
		[XmlIgnore]
		public List<string> InstalledCPUTypes
		{
			get
			{
				if (this._installedCPUTypes == null && !string.IsNullOrWhiteSpace(this.InstallRoot))
				{
					List<string> list = new List<string>();
					foreach (string text in from cpuitem in this.SupportedCPUTypes
					select cpuitem.CpuType)
					{
						if (this.FMPackagesFound(this.MSPackageRoot, text))
						{
							list.Add(text);
						}
					}
					if (list.Count > 0)
					{
						this._installedCPUTypes = list;
					}
				}
				return this._installedCPUTypes;
			}
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00008134 File Offset: 0x00006334
		public List<CpuId> GetSupportedWowCpuTypes(CpuId hostCpuType)
		{
			SupportedCPUType supportedCPUType = this.SupportedCPUTypes.FirstOrDefault((SupportedCPUType cpuitem) => cpuitem.CpuType.Equals(hostCpuType.ToString(), StringComparison.OrdinalIgnoreCase));
			if (supportedCPUType == null)
			{
				return new List<CpuId>();
			}
			return supportedCPUType.WowGuestCpuIds;
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00008178 File Offset: 0x00006378
		private bool FMPackagesFound(string msPackageRoot, string cpuType)
		{
			bool result = true;
			using (List<EditionPackage>.Enumerator enumerator = this.CoreFeatureManifestPackages.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.ExistsUnder(msPackageRoot, cpuType, "fre"))
					{
						result = false;
					}
				}
			}
			using (List<EditionPackage>.Enumerator enumerator = this.OptionalFeatureManifestPackages.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.ExistsUnder(msPackageRoot, cpuType, "fre"))
					{
						result = false;
					}
				}
			}
			return result;
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00008220 File Offset: 0x00006420
		public override string ToString()
		{
			return this.Name;
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00008228 File Offset: 0x00006428
		public bool IsProduct(string productName)
		{
			return productName.Equals(this.Name, StringComparison.OrdinalIgnoreCase) || productName.Equals(this.AlternateName, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x040000B5 RID: 181
		public const string MSPACKAGEROOT = "MSPackageRoot";

		// Token: 0x040000B6 RID: 182
		public const string PREBUILT = "Prebuilt";

		// Token: 0x040000B7 RID: 183
		public const string ENV_OS_ROOT = "OSCONTENTROOT";

		// Token: 0x040000B8 RID: 184
		[XmlAttribute]
		public string Name;

		// Token: 0x040000B9 RID: 185
		[XmlAttribute]
		public string AlternateName = string.Empty;

		// Token: 0x040000BA RID: 186
		[XmlAttribute]
		public bool AllowOEMCustomizations = true;

		// Token: 0x040000BB RID: 187
		[XmlAttribute]
		public bool RequiresKeyboard;

		// Token: 0x040000BC RID: 188
		[XmlAttribute]
		public ReleaseType ReleaseType = ReleaseType.Test;

		// Token: 0x040000BD RID: 189
		[XmlAttribute]
		[DefaultValue(false)]
		public uint MinimumUserStoreSize;

		// Token: 0x040000BE RID: 190
		[XmlAttribute]
		public string InternalProductDir;

		// Token: 0x040000BF RID: 191
		[XmlArrayItem(ElementName = "Package", Type = typeof(EditionPackage), IsNullable = false)]
		[XmlArray]
		public List<EditionPackage> CoreFeatureManifestPackages;

		// Token: 0x040000C0 RID: 192
		[XmlArrayItem(ElementName = "Package", Type = typeof(EditionPackage), IsNullable = false)]
		[XmlArray]
		public List<EditionPackage> OptionalFeatureManifestPackages;

		// Token: 0x040000C1 RID: 193
		[XmlArrayItem(ElementName = "CPUType", Type = typeof(SupportedCPUType), IsNullable = false)]
		[XmlArray]
		public List<SupportedCPUType> SupportedCPUTypes;

		// Token: 0x040000C2 RID: 194
		private string _msPackageRoot = string.Empty;

		// Token: 0x040000C3 RID: 195
		public EditionUISettings UISettings;

		// Token: 0x040000C4 RID: 196
		private string _installRoot = string.Empty;

		// Token: 0x040000C5 RID: 197
		private List<string> _installedCPUTypes;
	}
}
