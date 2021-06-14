using System;
using System.Xml.Serialization;

namespace Microsoft.CompPlat.PkgBldr.Base
{
	// Token: 0x02000041 RID: 65
	public class Macro
	{
		// Token: 0x17000054 RID: 84
		// (get) Token: 0x0600012F RID: 303 RVA: 0x000085A5 File Offset: 0x000067A5
		// (set) Token: 0x06000130 RID: 304 RVA: 0x000085AD File Offset: 0x000067AD
		[XmlAttribute("id")]
		public string Name { get; set; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000131 RID: 305 RVA: 0x000085B6 File Offset: 0x000067B6
		// (set) Token: 0x06000132 RID: 306 RVA: 0x000085C9 File Offset: 0x000067C9
		[XmlAttribute("value")]
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

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000133 RID: 307 RVA: 0x000085D2 File Offset: 0x000067D2
		// (set) Token: 0x06000134 RID: 308 RVA: 0x000085DA File Offset: 0x000067DA
		[XmlIgnore]
		public object Value { get; set; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000135 RID: 309 RVA: 0x000085E3 File Offset: 0x000067E3
		// (set) Token: 0x06000136 RID: 310 RVA: 0x000085EB File Offset: 0x000067EB
		[XmlIgnore]
		public MacroDelegate Delegate { get; set; }

		// Token: 0x06000137 RID: 311 RVA: 0x000085F4 File Offset: 0x000067F4
		public Macro()
		{
			this.Delegate = ((object x) => x.ToString());
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00008621 File Offset: 0x00006821
		public Macro(string name, object value) : this()
		{
			this.Name = name;
			this.Value = value;
		}

		// Token: 0x06000139 RID: 313 RVA: 0x00008637 File Offset: 0x00006837
		public Macro(string name, object value, MacroDelegate del) : this(name, value)
		{
			this.Delegate = del;
		}
	}
}
