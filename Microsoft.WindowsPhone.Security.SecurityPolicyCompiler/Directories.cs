using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x0200003B RID: 59
	public class Directories : PathElements<Directory>
	{
		// Token: 0x1700006D RID: 109
		// (get) Token: 0x060001F9 RID: 505 RVA: 0x00009CE0 File Offset: 0x00007EE0
		// (set) Token: 0x060001FA RID: 506 RVA: 0x00009CE8 File Offset: 0x00007EE8
		[XmlElement(ElementName = "Directory")]
		public new List<Directory> PathElementCollection
		{
			get
			{
				return base.PathElementCollection;
			}
			set
			{
				base.PathElementCollection = value;
			}
		}

		// Token: 0x060001FB RID: 507 RVA: 0x00009CF1 File Offset: 0x00007EF1
		public Directories()
		{
			base.ElementName = "Directory";
		}
	}
}
