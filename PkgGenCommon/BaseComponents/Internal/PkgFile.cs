using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x02000068 RID: 104
	[XmlRoot(ElementName = "File", Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00")]
	public class PkgFile : PkgElement
	{
		// Token: 0x17000073 RID: 115
		// (get) Token: 0x060001DF RID: 479 RVA: 0x00007695 File Offset: 0x00005895
		[XmlIgnore]
		public string DevicePath
		{
			get
			{
				return Path.Combine(this.DestinationDir, (this.Name == null) ? Path.GetFileName(this.SourcePath) : this.Name);
			}
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x000076BD File Offset: 0x000058BD
		public bool ShouldSerializeDestinationDir()
		{
			return !this.DestinationDir.Equals("$(runtime.default)", StringComparison.InvariantCulture);
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x000076D3 File Offset: 0x000058D3
		public bool ShouldSerializeAttributes()
		{
			return this.Attributes != PkgConstants.c_defaultAttributes;
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x000076E5 File Offset: 0x000058E5
		public void Preprocess(IMacroResolver macroResolver)
		{
			this.SourcePath = macroResolver.Resolve(this.SourcePath, MacroResolveOptions.SkipOnUnknownMacro);
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x000076FA File Offset: 0x000058FA
		public override void Build(IPackageGenerator pkgGen, SatelliteId satelliteId)
		{
			pkgGen.AddFile(this.SourcePath, this.DevicePath, this.Attributes, satelliteId, this.EmbeddedSigningCategory);
		}

		// Token: 0x04000165 RID: 357
		[XmlAttribute("Source")]
		public string SourcePath;

		// Token: 0x04000166 RID: 358
		[XmlAttribute("DestinationDir")]
		public string DestinationDir = "$(runtime.default)";

		// Token: 0x04000167 RID: 359
		[XmlAttribute("Name")]
		public string Name;

		// Token: 0x04000168 RID: 360
		[XmlAttribute("Attributes")]
		public FileAttributes Attributes = PkgConstants.c_defaultAttributes;

		// Token: 0x04000169 RID: 361
		[XmlAttribute("EmbeddedSigningCategory")]
		public string EmbeddedSigningCategory = "None";
	}
}
