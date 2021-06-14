using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000006 RID: 6
	[XmlType(Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
	[XmlRoot(ElementName = "DeviceLayout", Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate", IsNullable = false)]
	public class DeviceLayoutValidationScope
	{
		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600003F RID: 63 RVA: 0x00002296 File Offset: 0x00000496
		// (set) Token: 0x06000040 RID: 64 RVA: 0x0000229E File Offset: 0x0000049E
		public string Scope { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000041 RID: 65 RVA: 0x000022A7 File Offset: 0x000004A7
		[XmlArrayItem(ElementName = "Scope", Type = typeof(string), IsNullable = false)]
		[XmlArray]
		public List<string> ExcludedScopes
		{
			get
			{
				return this._excludedScopes;
			}
		}

		// Token: 0x0400002C RID: 44
		private List<string> _excludedScopes = new List<string>();
	}
}
