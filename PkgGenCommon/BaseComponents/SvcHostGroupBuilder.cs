using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents.Internal;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon.BaseComponents
{
	// Token: 0x02000053 RID: 83
	public class SvcHostGroupBuilder : PkgObjectBuilder<SvcHostGroup, SvcHostGroupBuilder>
	{
		// Token: 0x0600015A RID: 346 RVA: 0x00006383 File Offset: 0x00004583
		public SvcHostGroupBuilder(string name)
		{
			this.pkgObject = new SvcHostGroup();
			this.pkgObject.Name = name;
		}

		// Token: 0x0600015B RID: 347 RVA: 0x000063A4 File Offset: 0x000045A4
		public SvcHostGroupBuilder(XElement svcHostElement)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(SvcHostGroup));
			using (XmlReader xmlReader = svcHostElement.CreateReader())
			{
				this.pkgObject = (SvcHostGroup)xmlSerializer.Deserialize(xmlReader);
			}
		}

		// Token: 0x0600015C RID: 348 RVA: 0x000063FC File Offset: 0x000045FC
		public SvcHostGroupBuilder SetCoInitializeSecurityParam(bool flag)
		{
			this.pkgObject.CoInitializeSecurityParam = flag;
			return this;
		}

		// Token: 0x0600015D RID: 349 RVA: 0x0000640B File Offset: 0x0000460B
		public SvcHostGroupBuilder SetCoInitializeSecurityAllowLowBox(bool flag)
		{
			this.pkgObject.CoInitializeSecurityAllowLowBox = flag;
			return this;
		}

		// Token: 0x0600015E RID: 350 RVA: 0x0000641A File Offset: 0x0000461A
		public SvcHostGroupBuilder SetCoInitializeSecurityAppId(string appId)
		{
			this.pkgObject.CoInitializeSecurityAppId = appId;
			return this;
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00006429 File Offset: 0x00004629
		public SvcHostGroupBuilder SetDefaultRpcStackSize(int size)
		{
			this.pkgObject.DefaultRpcStackSize = size;
			return this;
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00006438 File Offset: 0x00004638
		public SvcHostGroupBuilder SetSystemCritical(bool flag)
		{
			this.pkgObject.SystemCritical = flag;
			return this;
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00006447 File Offset: 0x00004647
		public SvcHostGroupBuilder SetAuthenticationLevel(AuthenticationLevel level)
		{
			this.pkgObject.AuthenticationLevel = level;
			return this;
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00006456 File Offset: 0x00004656
		public SvcHostGroupBuilder SetAuthenticationCapabilities(AuthenticationCapabitities capabilities)
		{
			this.pkgObject.AuthenticationCapabitities = capabilities;
			return this;
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00006465 File Offset: 0x00004665
		public SvcHostGroupBuilder SetImpersonationLevel(ImpersonationLevel level)
		{
			this.pkgObject.ImpersonationLevel = level;
			return this;
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00006474 File Offset: 0x00004674
		public override SvcHostGroup ToPkgObject()
		{
			base.RegisterMacro("hklm.svchostgroup", "$(hklm.svchost)\\" + this.pkgObject.Name);
			return base.ToPkgObject();
		}
	}
}
