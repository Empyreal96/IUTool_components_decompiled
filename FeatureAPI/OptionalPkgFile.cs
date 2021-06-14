using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.FeatureAPI
{
	// Token: 0x02000018 RID: 24
	public class OptionalPkgFile : PkgFile
	{
		// Token: 0x06000097 RID: 151 RVA: 0x00007104 File Offset: 0x00005304
		public OptionalPkgFile(FeatureManifest.PackageGroups fmGroup) : base(fmGroup)
		{
		}

		// Token: 0x06000098 RID: 152 RVA: 0x0000719B File Offset: 0x0000539B
		public OptionalPkgFile(OptionalPkgFile srcPkg) : base(srcPkg)
		{
			this.FMGroup = srcPkg.FMGroup;
			this.FeatureIDs = srcPkg.FeatureIDs;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x000071BC File Offset: 0x000053BC
		public new void CopyPkgFile(PkgFile srcPkgFile)
		{
			base.CopyPkgFile(srcPkgFile);
			OptionalPkgFile optionalPkgFile = srcPkgFile as OptionalPkgFile;
			this.FeatureIDs = new List<string>(optionalPkgFile.FeatureIDs);
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600009A RID: 154 RVA: 0x000071E8 File Offset: 0x000053E8
		// (set) Token: 0x0600009B RID: 155 RVA: 0x000071FF File Offset: 0x000053FF
		[XmlIgnore]
		public override string GroupValue
		{
			get
			{
				return string.Join(";", this.FeatureIDs.ToArray());
			}
			set
			{
				this.FeatureIDs = value.Split(new char[]
				{
					';'
				}, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
			}
		}

		// Token: 0x0400008F RID: 143
		[XmlArrayItem(ElementName = "FeatureID", Type = typeof(string), IsNullable = false)]
		[XmlArray]
		public List<string> FeatureIDs;
	}
}
