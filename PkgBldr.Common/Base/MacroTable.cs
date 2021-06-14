using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.CompPlat.PkgBldr.Base
{
	// Token: 0x02000042 RID: 66
	[XmlRoot(Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00", ElementName = "macros")]
	public class MacroTable
	{
		// Token: 0x17000058 RID: 88
		// (get) Token: 0x0600013A RID: 314 RVA: 0x00008648 File Offset: 0x00006848
		// (set) Token: 0x0600013B RID: 315 RVA: 0x00008650 File Offset: 0x00006850
		[SuppressMessage("Microsoft.Usage", "CA2227", Justification = "Not a big enough issue to justify the risk associated with changing a public API on a shipping tool.")]
		[XmlElement(ElementName = "macro")]
		public List<Macro> Macros { get; set; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x0600013C RID: 316 RVA: 0x00008659 File Offset: 0x00006859
		[XmlIgnore]
		public IEnumerable<KeyValuePair<string, Macro>> Values
		{
			get
			{
				return from x in this.Macros
				select new KeyValuePair<string, Macro>(x.Name, x);
			}
		}

		// Token: 0x0600013D RID: 317 RVA: 0x00008685 File Offset: 0x00006885
		public MacroTable()
		{
			this.Macros = new List<Macro>();
		}
	}
}
