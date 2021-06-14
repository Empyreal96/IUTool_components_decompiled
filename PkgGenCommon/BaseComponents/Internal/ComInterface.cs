using System;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x0200005D RID: 93
	[XmlRoot(ElementName = "Interface", Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00")]
	public sealed class ComInterface : ComBase
	{
		// Token: 0x1700005F RID: 95
		// (get) Token: 0x0600019C RID: 412 RVA: 0x00006AAC File Offset: 0x00004CAC
		// (set) Token: 0x0600019D RID: 413 RVA: 0x00006AB4 File Offset: 0x00004CB4
		[XmlAttribute("Name")]
		public string Name { get; set; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x0600019E RID: 414 RVA: 0x00006ABD File Offset: 0x00004CBD
		// (set) Token: 0x0600019F RID: 415 RVA: 0x00006AC5 File Offset: 0x00004CC5
		[XmlAttribute("ProxyStubClsId")]
		public string ProxyStubClsId { get; set; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060001A0 RID: 416 RVA: 0x00006ACE File Offset: 0x00004CCE
		// (set) Token: 0x060001A1 RID: 417 RVA: 0x00006AD6 File Offset: 0x00004CD6
		[XmlAttribute("ProxyStubClsId32")]
		public string ProxyStubClsId32 { get; set; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060001A2 RID: 418 RVA: 0x00006ADF File Offset: 0x00004CDF
		// (set) Token: 0x060001A3 RID: 419 RVA: 0x00006AE7 File Offset: 0x00004CE7
		[XmlAttribute("NumMethods")]
		public string NumMethods { get; set; }

		// Token: 0x060001A4 RID: 420 RVA: 0x00006AF0 File Offset: 0x00004CF0
		public ComInterface()
		{
			this._defaultKey = "$(hkcr.iid)";
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x00006B04 File Offset: 0x00004D04
		protected override void DoBuild(IPackageGenerator pkgGen)
		{
			base.DoBuild(pkgGen);
			if (this.ProxyStubClsId != null)
			{
				pkgGen.AddRegValue(this._defaultKey + "\\ProxyStubClsId", "@", RegValueType.String, this.ProxyStubClsId);
			}
			if (this.ProxyStubClsId32 != null)
			{
				pkgGen.AddRegValue(this._defaultKey + "\\ProxyStubClsId32", "@", RegValueType.String, this.ProxyStubClsId32);
			}
			if (this.Name != null)
			{
				pkgGen.AddRegValue("$(hkcr.iid)", "@", RegValueType.String, this.Name);
			}
			if (base.Version != null)
			{
				pkgGen.AddRegValue(this._defaultKey + "\\TypeLib", "Version", RegValueType.String, base.Version);
			}
			if (this.NumMethods != null)
			{
				pkgGen.AddRegValue(this._defaultKey + "\\NumMethods", "@", RegValueType.String, this.NumMethods);
			}
		}
	}
}
