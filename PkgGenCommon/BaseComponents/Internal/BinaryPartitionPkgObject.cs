using System;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x02000058 RID: 88
	[XmlRoot(ElementName = "BinaryPartition", Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00")]
	public sealed class BinaryPartitionPkgObject : PkgObject
	{
		// Token: 0x17000052 RID: 82
		// (get) Token: 0x0600017A RID: 378 RVA: 0x00006730 File Offset: 0x00004930
		// (set) Token: 0x0600017B RID: 379 RVA: 0x00006738 File Offset: 0x00004938
		[XmlAttribute("ImageSource")]
		public string ImageSource { get; set; }

		// Token: 0x0600017C RID: 380 RVA: 0x00006741 File Offset: 0x00004941
		protected override void DoBuild(IPackageGenerator pkgGen)
		{
			pkgGen.AddBinaryPartition(this.ImageSource);
		}
	}
}
