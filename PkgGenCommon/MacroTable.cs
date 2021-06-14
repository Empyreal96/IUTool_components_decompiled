using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon
{
	// Token: 0x02000004 RID: 4
	[XmlRoot(Namespace = "urn:Microsoft.WindowsPhone/PackageSchema.v8.00", ElementName = "Macros")]
	public class MacroTable
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000010 RID: 16 RVA: 0x000020F3 File Offset: 0x000002F3
		// (set) Token: 0x06000011 RID: 17 RVA: 0x000020FB File Offset: 0x000002FB
		[XmlElement(ElementName = "Macro")]
		public List<Macro> Macros { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000012 RID: 18 RVA: 0x00002104 File Offset: 0x00000304
		[XmlIgnore]
		public IEnumerable<KeyValuePair<string, Macro>> Values
		{
			get
			{
				return from x in this.Macros
				select new KeyValuePair<string, Macro>(x.Name, x);
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002130 File Offset: 0x00000330
		public MacroTable()
		{
			this.Macros = new List<Macro>();
		}
	}
}
