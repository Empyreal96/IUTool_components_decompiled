using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x0200005B RID: 91
	public abstract class ComBase : PkgObject
	{
		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000180 RID: 384 RVA: 0x000067E0 File Offset: 0x000049E0
		// (set) Token: 0x06000181 RID: 385 RVA: 0x000067E8 File Offset: 0x000049E8
		[XmlAttribute("Id")]
		public string Id { get; set; }

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000182 RID: 386 RVA: 0x000067F1 File Offset: 0x000049F1
		// (set) Token: 0x06000183 RID: 387 RVA: 0x000067F9 File Offset: 0x000049F9
		[XmlAttribute("Version")]
		public string Version { get; set; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000184 RID: 388 RVA: 0x00006802 File Offset: 0x00004A02
		// (set) Token: 0x06000185 RID: 389 RVA: 0x0000680A File Offset: 0x00004A0A
		[XmlAttribute("TypeLib")]
		public string TypeLib { get; set; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000186 RID: 390 RVA: 0x00006813 File Offset: 0x00004A13
		[XmlElement("RegKey")]
		public List<RegistryKey> RegKeys { get; }

		// Token: 0x06000187 RID: 391 RVA: 0x0000681B File Offset: 0x00004A1B
		public ComBase()
		{
			this.RegKeys = new List<RegistryKey>();
		}

		// Token: 0x06000188 RID: 392 RVA: 0x00006830 File Offset: 0x00004A30
		protected override void DoBuild(IPackageGenerator pkgGen)
		{
			base.DoBuild(pkgGen);
			if (this.TypeLib != null)
			{
				pkgGen.AddRegValue(this._defaultKey + "\\TypeLib", "@", RegValueType.String, this.TypeLib);
			}
			if (this.RegKeys != null)
			{
				this.RegKeys.ForEach(delegate(RegistryKey x)
				{
					x.Build(pkgGen);
				});
			}
		}

		// Token: 0x06000189 RID: 393 RVA: 0x000068A4 File Offset: 0x00004AA4
		protected override void DoPreprocess(PackageProject proj, IMacroResolver macroResolver)
		{
			this.Id = macroResolver.Resolve(this.Id, MacroResolveOptions.SkipOnUnknownMacro);
		}

		// Token: 0x0400013A RID: 314
		protected string _defaultKey;
	}
}
