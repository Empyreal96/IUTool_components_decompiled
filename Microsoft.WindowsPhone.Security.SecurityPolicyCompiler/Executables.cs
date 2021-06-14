using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Security.SecurityPolicyCompiler
{
	// Token: 0x0200003A RID: 58
	public class Executables : PathElements<Executable>
	{
		// Token: 0x1700006C RID: 108
		// (get) Token: 0x060001F6 RID: 502 RVA: 0x00009CBC File Offset: 0x00007EBC
		// (set) Token: 0x060001F7 RID: 503 RVA: 0x00009CC4 File Offset: 0x00007EC4
		[XmlElement(ElementName = "Executable")]
		public new List<Executable> PathElementCollection
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

		// Token: 0x060001F8 RID: 504 RVA: 0x00009CCD File Offset: 0x00007ECD
		public Executables()
		{
			base.ElementName = "Executable";
		}
	}
}
