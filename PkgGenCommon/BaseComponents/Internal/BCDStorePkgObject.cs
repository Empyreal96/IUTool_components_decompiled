using System;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x02000057 RID: 87
	[XmlRoot(ElementName = "BCDStore", Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00")]
	public sealed class BCDStorePkgObject : PkgObject
	{
		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000176 RID: 374 RVA: 0x00006709 File Offset: 0x00004909
		// (set) Token: 0x06000177 RID: 375 RVA: 0x00006711 File Offset: 0x00004911
		[XmlAttribute("Source")]
		public string Source { get; set; }

		// Token: 0x06000178 RID: 376 RVA: 0x0000671A File Offset: 0x0000491A
		protected override void DoBuild(IPackageGenerator pkgGen)
		{
			pkgGen.AddBCDStore(this.Source);
		}
	}
}
