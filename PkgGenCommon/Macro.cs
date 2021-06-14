using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgGenCommon
{
	// Token: 0x02000003 RID: 3
	public class Macro
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000005 RID: 5 RVA: 0x00002050 File Offset: 0x00000250
		// (set) Token: 0x06000006 RID: 6 RVA: 0x00002058 File Offset: 0x00000258
		[XmlAttribute("Id")]
		public string Name { get; set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000007 RID: 7 RVA: 0x00002061 File Offset: 0x00000261
		// (set) Token: 0x06000008 RID: 8 RVA: 0x00002074 File Offset: 0x00000274
		[XmlAttribute("Value")]
		public string StringValue
		{
			get
			{
				return this.Delegate(this.Value);
			}
			set
			{
				this.Value = value;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000009 RID: 9 RVA: 0x0000207D File Offset: 0x0000027D
		// (set) Token: 0x0600000A RID: 10 RVA: 0x00002085 File Offset: 0x00000285
		[XmlIgnore]
		public object Value { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000B RID: 11 RVA: 0x0000208E File Offset: 0x0000028E
		// (set) Token: 0x0600000C RID: 12 RVA: 0x00002096 File Offset: 0x00000296
		[XmlIgnore]
		public MacroDelegate Delegate { get; set; }

		// Token: 0x0600000D RID: 13 RVA: 0x0000209F File Offset: 0x0000029F
		public Macro()
		{
			this.Delegate = ((object x) => x.ToString());
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000020CC File Offset: 0x000002CC
		public Macro(string name, object value) : this()
		{
			this.Name = name;
			this.Value = value;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000020E2 File Offset: 0x000002E2
		public Macro(string name, object value, MacroDelegate del) : this(name, value)
		{
			this.Delegate = del;
		}
	}
}
