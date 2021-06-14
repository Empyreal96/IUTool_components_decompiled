using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.CompDB
{
	// Token: 0x02000007 RID: 7
	public class CompDBFeature
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000030 RID: 48 RVA: 0x000042E4 File Offset: 0x000024E4
		[XmlIgnore]
		public string FeatureIDWithFMID
		{
			get
			{
				string text = this.FeatureID;
				if (!string.IsNullOrEmpty(this.FMID))
				{
					text = text + "." + this.FMID;
				}
				return text;
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00004318 File Offset: 0x00002518
		public bool ShouldSerializePackages()
		{
			return this.Packages != null && this.Packages.Count<CompDBFeaturePackage>() > 0;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00004332 File Offset: 0x00002532
		public CompDBFeature()
		{
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00004345 File Offset: 0x00002545
		public CompDBFeature(string featureID, string fmID, CompDBFeature.CompDBFeatureTypes type, string group)
		{
			this.FeatureID = featureID;
			this.FMID = fmID;
			this.Group = group;
			this.Type = type;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00004378 File Offset: 0x00002578
		public CompDBFeature(CompDBFeature srcFeature)
		{
			this.FeatureID = srcFeature.FeatureID;
			this.FMID = srcFeature.FMID;
			this.Group = srcFeature.Group;
			this.Type = srcFeature.Type;
			this.Packages = new List<CompDBFeaturePackage>();
			foreach (CompDBFeaturePackage srcPkg in srcFeature.Packages)
			{
				this.Packages.Add(new CompDBFeaturePackage(srcPkg));
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00004424 File Offset: 0x00002624
		public CompDBFeaturePackage FindPackage(string packageID)
		{
			return this.Packages.FirstOrDefault((CompDBFeaturePackage pkg) => pkg.ID.Equals(packageID, StringComparison.OrdinalIgnoreCase));
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00004458 File Offset: 0x00002658
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.FeatureIDWithFMID,
				" : ",
				this.Group,
				" : Count=",
				this.Packages.Count<CompDBFeaturePackage>()
			});
		}

		// Token: 0x04000027 RID: 39
		[XmlAttribute]
		[DefaultValue(CompDBFeature.CompDBFeatureTypes.None)]
		public CompDBFeature.CompDBFeatureTypes Type;

		// Token: 0x04000028 RID: 40
		[XmlAttribute]
		public string FeatureID;

		// Token: 0x04000029 RID: 41
		[XmlAttribute]
		public string FMID;

		// Token: 0x0400002A RID: 42
		[XmlAttribute]
		public string Group;

		// Token: 0x0400002B RID: 43
		[XmlArrayItem(ElementName = "Package", Type = typeof(CompDBFeaturePackage), IsNullable = false)]
		[XmlArray]
		public List<CompDBFeaturePackage> Packages = new List<CompDBFeaturePackage>();

		// Token: 0x02000045 RID: 69
		public enum CompDBFeatureTypes
		{
			// Token: 0x040001B4 RID: 436
			None,
			// Token: 0x040001B5 RID: 437
			MobileFeature,
			// Token: 0x040001B6 RID: 438
			DesktopMedia,
			// Token: 0x040001B7 RID: 439
			OptionalFeature,
			// Token: 0x040001B8 RID: 440
			OnDemandFeature,
			// Token: 0x040001B9 RID: 441
			LanguagePack,
			// Token: 0x040001BA RID: 442
			GDR,
			// Token: 0x040001BB RID: 443
			CritGDR,
			// Token: 0x040001BC RID: 444
			Tool
		}
	}
}
