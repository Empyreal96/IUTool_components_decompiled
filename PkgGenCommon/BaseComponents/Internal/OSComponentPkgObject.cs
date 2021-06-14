using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x0200006B RID: 107
	[XmlRoot(ElementName = "OSComponent", Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00")]
	public class OSComponentPkgObject : PkgObject
	{
		// Token: 0x17000074 RID: 116
		// (get) Token: 0x060001EC RID: 492 RVA: 0x000077F5 File Offset: 0x000059F5
		[XmlElement("Files")]
		public List<FileGroup> FileGroups { get; }

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x060001ED RID: 493 RVA: 0x000077FD File Offset: 0x000059FD
		[XmlElement("RegKeys")]
		public List<RegGroup> KeyGroups { get; }

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x060001EE RID: 494 RVA: 0x00007805 File Offset: 0x00005A05
		[XmlElement("RegImport")]
		public List<RegImport> RegImports { get; }

		// Token: 0x060001EF RID: 495 RVA: 0x0000780D File Offset: 0x00005A0D
		public OSComponentPkgObject()
		{
			this.FileGroups = new List<FileGroup>();
			this.KeyGroups = new List<RegGroup>();
			this.RegImports = new List<RegImport>();
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x00007838 File Offset: 0x00005A38
		protected override void DoPreprocess(PackageProject proj, IMacroResolver macroResolver)
		{
			this.FileGroups.ForEach(delegate(FileGroup x)
			{
				x.Preprocess(macroResolver);
			});
			this.RegImports.ForEach(delegate(RegImport x)
			{
				x.Preprocess(macroResolver);
			});
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x00007880 File Offset: 0x00005A80
		protected override void DoBuild(IPackageGenerator pkgGen)
		{
			this.FileGroups.ForEach(delegate(FileGroup x)
			{
				x.Build(pkgGen);
			});
			if (pkgGen.BuildPass != BuildPass.BuildTOC)
			{
				this.KeyGroups.ForEach(delegate(RegGroup x)
				{
					x.Build(pkgGen);
				});
				this.RegImports.ForEach(delegate(RegImport x)
				{
					x.Build(pkgGen);
				});
			}
		}
	}
}
