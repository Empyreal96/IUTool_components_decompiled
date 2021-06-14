using System;
using System.Security.AccessControl;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000045 RID: 69
	public class RegAclWithFullAcl : RegistryAcl
	{
		// Token: 0x060001C3 RID: 451 RVA: 0x0000962C File Offset: 0x0000782C
		public RegAclWithFullAcl()
		{
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x00009634 File Offset: 0x00007834
		public RegAclWithFullAcl(NativeObjectSecurity nos)
		{
			this.m_nos = nos;
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060001C5 RID: 453 RVA: 0x00009643 File Offset: 0x00007843
		// (set) Token: 0x060001C6 RID: 454 RVA: 0x0000964B File Offset: 0x0000784B
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
