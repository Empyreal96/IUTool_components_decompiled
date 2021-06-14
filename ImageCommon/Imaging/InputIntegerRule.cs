using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000024 RID: 36
	public class InputIntegerRule : InputRule
	{
		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000182 RID: 386 RVA: 0x0000DAD6 File Offset: 0x0000BCD6
		// (set) Token: 0x06000183 RID: 387 RVA: 0x0000DADE File Offset: 0x0000BCDE
		public ulong? Max { get; set; }

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000184 RID: 388 RVA: 0x0000DAE7 File Offset: 0x0000BCE7
		// (set) Token: 0x06000185 RID: 389 RVA: 0x0000DAEF File Offset: 0x0000BCEF
		public ulong? Min { get; set; }

		// Token: 0x040000FF RID: 255
		[XmlArrayItem(ElementName = "Value", Type = typeof(ulong), IsNullable = false)]
		[XmlArray("List")]
		public ulong[] Values;
	}
}
