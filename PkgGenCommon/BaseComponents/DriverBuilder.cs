using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents
{
	// Token: 0x0200003B RID: 59
	public sealed class DriverBuilder : OSComponentBuilder<DriverPkgObject, DriverBuilder>
	{
		// Token: 0x060000E1 RID: 225 RVA: 0x0000548F File Offset: 0x0000368F
		public DriverBuilder(string infSource)
		{
			this.pkgObject.InfSource = infSource;
			this.references = new List<Reference>();
			this.security = new List<Security>();
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x000054B9 File Offset: 0x000036B9
		public DriverBuilder AddReference(string source, string stagingSubDir)
		{
			this.references.Add(new Reference(source, stagingSubDir));
			return this;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x000054CE File Offset: 0x000036CE
		public DriverBuilder AddReference(XElement reference)
		{
			this.references.Add(reference.FromXElement<Reference>());
			return this;
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x000054E2 File Offset: 0x000036E2
		public DriverBuilder AddSecurity(string infSectionName)
		{
			this.security.Add(new Security(infSectionName));
			return this;
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x000054F6 File Offset: 0x000036F6
		public DriverBuilder AddSecurity(XElement security)
		{
			this.security.Add(security.FromXElement<Security>());
			return this;
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x0000550C File Offset: 0x0000370C
		public override DriverPkgObject ToPkgObject()
		{
			base.RegisterMacro("runtime.default", "$(runtime.drivers)");
			base.RegisterMacro("env.default", "$(env.drivers)");
			this.pkgObject.References.AddRange(this.references);
			this.pkgObject.InfSecurity.AddRange(this.security);
			return base.ToPkgObject();
		}

		// Token: 0x040000E7 RID: 231
		private List<Reference> references;

		// Token: 0x040000E8 RID: 232
		private List<Security> security;
	}
}
