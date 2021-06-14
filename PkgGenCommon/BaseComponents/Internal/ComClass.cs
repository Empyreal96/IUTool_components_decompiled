using System;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x0200005C RID: 92
	[XmlRoot(ElementName = "Class", Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00")]
	public sealed class ComClass : ComBase
	{
		// Token: 0x17000057 RID: 87
		// (get) Token: 0x0600018A RID: 394 RVA: 0x000068B9 File Offset: 0x00004AB9
		// (set) Token: 0x0600018B RID: 395 RVA: 0x000068C1 File Offset: 0x00004AC1
		[XmlAttribute("ThreadingModel")]
		public string ThreadingModel { get; set; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x0600018C RID: 396 RVA: 0x000068CA File Offset: 0x00004ACA
		// (set) Token: 0x0600018D RID: 397 RVA: 0x000068D2 File Offset: 0x00004AD2
		[XmlAttribute("ProgId")]
		public string ProgId { get; set; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x0600018E RID: 398 RVA: 0x000068DB File Offset: 0x00004ADB
		// (set) Token: 0x0600018F RID: 399 RVA: 0x000068E3 File Offset: 0x00004AE3
		[XmlAttribute("VersionIndependentProgId")]
		public string VersionIndependentProgId { get; set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000190 RID: 400 RVA: 0x000068EC File Offset: 0x00004AEC
		// (set) Token: 0x06000191 RID: 401 RVA: 0x000068F4 File Offset: 0x00004AF4
		[XmlAttribute("Description")]
		public string Description { get; set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000192 RID: 402 RVA: 0x000068FD File Offset: 0x00004AFD
		// (set) Token: 0x06000193 RID: 403 RVA: 0x00006905 File Offset: 0x00004B05
		[XmlAttribute("DefaultIcon")]
		public string DefaultIcon { get; set; }

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000194 RID: 404 RVA: 0x0000690E File Offset: 0x00004B0E
		// (set) Token: 0x06000195 RID: 405 RVA: 0x00006916 File Offset: 0x00004B16
		[XmlAttribute("AppId")]
		public string AppId { get; set; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000196 RID: 406 RVA: 0x0000691F File Offset: 0x00004B1F
		// (set) Token: 0x06000197 RID: 407 RVA: 0x00006927 File Offset: 0x00004B27
		[XmlAttribute("SkipInProcServer32")]
		public bool SkipInProcServer32 { get; set; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000198 RID: 408 RVA: 0x00006930 File Offset: 0x00004B30
		// (set) Token: 0x06000199 RID: 409 RVA: 0x00006938 File Offset: 0x00004B38
		[XmlIgnore]
		public PkgFile Dll { get; set; }

		// Token: 0x0600019A RID: 410 RVA: 0x00006941 File Offset: 0x00004B41
		public ComClass()
		{
			this.ThreadingModel = "Both";
			this._defaultKey = "$(hkcr.clsid)";
		}

		// Token: 0x0600019B RID: 411 RVA: 0x00006960 File Offset: 0x00004B60
		protected override void DoBuild(IPackageGenerator pkgGen)
		{
			base.DoBuild(pkgGen);
			if (this.Description != null)
			{
				pkgGen.AddRegValue(this._defaultKey, "@", RegValueType.String, this.Description);
			}
			if (this.AppId != null)
			{
				pkgGen.AddRegValue(this._defaultKey, "AppId", RegValueType.String, this.AppId);
			}
			if (this.ProgId != null)
			{
				pkgGen.AddRegValue(this._defaultKey + "\\ProgId", "@", RegValueType.String, this.ProgId);
			}
			if (this.VersionIndependentProgId != null)
			{
				pkgGen.AddRegValue(this._defaultKey + "\\VersionIndependentProgId", "@", RegValueType.String, this.VersionIndependentProgId);
			}
			if (this.DefaultIcon != null)
			{
				pkgGen.AddRegValue(this._defaultKey + "\\DefaultIcon", "@", RegValueType.ExpandString, this.DefaultIcon);
			}
			if (base.Version != null)
			{
				pkgGen.AddRegValue(this._defaultKey + "\\Version", "@", RegValueType.String, base.Version);
			}
			if (!this.SkipInProcServer32)
			{
				pkgGen.AddRegExpandValue(this._defaultKey + "\\InProcServer32", "@", this.Dll.DevicePath);
				pkgGen.AddRegValue(this._defaultKey + "\\InProcServer32", "ThreadingModel", RegValueType.String, this.ThreadingModel);
			}
		}
	}
}
