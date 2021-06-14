using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.Interfaces;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal
{
	// Token: 0x0200005E RID: 94
	[XmlRoot(ElementName = "ComServer", Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00")]
	public sealed class ComPkgObject : OSComponentPkgObject
	{
		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060001A6 RID: 422 RVA: 0x00006BDF File Offset: 0x00004DDF
		// (set) Token: 0x060001A7 RID: 423 RVA: 0x00006BE7 File Offset: 0x00004DE7
		[XmlElement("Dll")]
		public ComDll ComDll { get; set; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060001A8 RID: 424 RVA: 0x00006BF0 File Offset: 0x00004DF0
		[XmlArray("Classes")]
		[XmlArrayItem(typeof(ComClass), ElementName = "Class")]
		public List<ComClass> Classes { get; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060001A9 RID: 425 RVA: 0x00006BF8 File Offset: 0x00004DF8
		[XmlArray("Interfaces")]
		[XmlArrayItem(typeof(ComInterface), ElementName = "Interface")]
		public List<ComInterface> Interfaces { get; }

		// Token: 0x060001AA RID: 426 RVA: 0x00006C00 File Offset: 0x00004E00
		public ComPkgObject()
		{
			this.Classes = new List<ComClass>();
			this.Interfaces = new List<ComInterface>();
		}

		// Token: 0x060001AB RID: 427 RVA: 0x00006C1E File Offset: 0x00004E1E
		public bool ShouldSerializeClasses()
		{
			return this.Classes != null && this.Classes.Count > 0;
		}

		// Token: 0x060001AC RID: 428 RVA: 0x00006C38 File Offset: 0x00004E38
		public bool ShouldSerializeInterfaces()
		{
			return this.Interfaces != null && this.Interfaces.Count > 0;
		}

		// Token: 0x060001AD RID: 429 RVA: 0x00006C54 File Offset: 0x00004E54
		protected override void DoPreprocess(PackageProject proj, IMacroResolver macroResolver)
		{
			this.ComDll.Preprocess(macroResolver);
			this.Classes.ForEach(delegate(ComClass x)
			{
				x.Preprocess(proj, macroResolver);
			});
			this.Interfaces.ForEach(delegate(ComInterface x)
			{
				x.Preprocess(proj, macroResolver);
			});
			base.DoPreprocess(proj, macroResolver);
		}

		// Token: 0x060001AE RID: 430 RVA: 0x00006CC8 File Offset: 0x00004EC8
		protected override void DoBuild(IPackageGenerator pkgGen)
		{
			base.DoBuild(pkgGen);
			this.ComDll.Build(pkgGen);
			if (pkgGen.BuildPass != BuildPass.BuildTOC)
			{
				if (this.Classes != null)
				{
					this.Classes.ForEach(delegate(ComClass x)
					{
						x.Dll = this.ComDll;
					});
					this.Classes.ForEach(delegate(ComClass x)
					{
						x.Build(pkgGen);
					});
				}
				if (this.Interfaces != null)
				{
					this.Interfaces.ForEach(delegate(ComInterface x)
					{
						x.Build(pkgGen);
					});
				}
			}
		}
	}
}
