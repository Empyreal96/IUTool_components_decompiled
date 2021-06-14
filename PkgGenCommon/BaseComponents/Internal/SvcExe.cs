using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x02000079 RID: 121
	[XmlRoot("Executable", Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00")]
	public class SvcExe : SvcEntry
	{
		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x060002A1 RID: 673 RVA: 0x0000A270 File Offset: 0x00008470
		[XmlIgnore]
		public string ImagePathExpandString
		{
			get
			{
				if (string.IsNullOrEmpty(this.ImagePath))
				{
					string path = (this.Name == null) ? Path.GetFileName(this.SourcePath) : this.Name;
					return Path.Combine(this.DestinationDir.Replace("$(runtime.", "$(env."), path);
				}
				return this.ImagePath;
			}
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x0000A2C8 File Offset: 0x000084C8
		public override void Build(IPackageGenerator pkgGen)
		{
			if (!this.BinaryInOneCorePkg)
			{
				base.Build(pkgGen);
			}
			if (pkgGen.BuildPass != BuildPass.BuildTOC)
			{
				pkgGen.AddRegValue("$(hklm.service)", "ImagePath", RegValueType.ExpandString, this.ImagePathExpandString);
			}
		}

		// Token: 0x040001C1 RID: 449
		[XmlAttribute("ImagePath")]
		public string ImagePath;

		// Token: 0x040001C2 RID: 450
		[XmlAttribute("BinaryInOneCorePkg")]
		public bool BinaryInOneCorePkg;
	}
}
