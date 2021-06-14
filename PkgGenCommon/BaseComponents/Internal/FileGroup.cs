using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x02000064 RID: 100
	[XmlRoot(ElementName = "Files", Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00")]
	public class FileGroup : FilterGroup
	{
		// Token: 0x060001CF RID: 463 RVA: 0x00007534 File Offset: 0x00005734
		public FileGroup()
		{
			this.Files = new List<PkgFile>();
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x00007548 File Offset: 0x00005748
		public void Preprocess(IMacroResolver macroResolver)
		{
			this.Files.ForEach(delegate(PkgFile x)
			{
				x.Preprocess(macroResolver);
			});
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x0000757C File Offset: 0x0000577C
		public override void Build(IPackageGenerator pkgGen, SatelliteId satelliteId)
		{
			this.Files.ForEach(delegate(PkgFile x)
			{
				x.Build(pkgGen, satelliteId);
			});
		}

		// Token: 0x04000160 RID: 352
		[XmlElement("File")]
		public List<PkgFile> Files;
	}
}
