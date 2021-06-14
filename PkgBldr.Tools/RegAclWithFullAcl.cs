using System;
using System.Security.AccessControl;
using System.Xml.Serialization;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000026 RID: 38
	public class RegAclWithFullAcl : RegistryAcl
	{
		// Token: 0x06000171 RID: 369 RVA: 0x00006F74 File Offset: 0x00005174
		public RegAclWithFullAcl()
		{
		}

		// Token: 0x06000172 RID: 370 RVA: 0x00006F7C File Offset: 0x0000517C
		public RegAclWithFullAcl(NativeObjectSecurity nos)
		{
			this.m_nos = nos;
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000173 RID: 371 RVA: 0x00006F8B File Offset: 0x0000518B
		// (set) Token: 0x06000174 RID: 372 RVA: 0x00006F93 File Offset: 0x00005193
		[XmlAttribute("FullACL")]
		public string FullRegACL
		{
			get
			{
				return base.FullACL;
			}
			set
			{
			}
		}
	}
}
