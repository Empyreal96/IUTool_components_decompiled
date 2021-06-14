using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.WindowsPhone.Multivariant.Offline
{
	// Token: 0x0200000C RID: 12
	public class MVVariant
	{
		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000054 RID: 84 RVA: 0x00003861 File Offset: 0x00001A61
		// (set) Token: 0x06000055 RID: 85 RVA: 0x00003869 File Offset: 0x00001A69
		public string Name { get; private set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000056 RID: 86 RVA: 0x00003872 File Offset: 0x00001A72
		// (set) Token: 0x06000057 RID: 87 RVA: 0x0000387A File Offset: 0x00001A7A
		public List<MVCondition> Conditions { get; private set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000058 RID: 88 RVA: 0x00003883 File Offset: 0x00001A83
		// (set) Token: 0x06000059 RID: 89 RVA: 0x0000388B File Offset: 0x00001A8B
		public List<MVSettingGroup> SettingsGroups { get; private set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600005A RID: 90 RVA: 0x00003894 File Offset: 0x00001A94
		// (set) Token: 0x0600005B RID: 91 RVA: 0x0000389C File Offset: 0x00001A9C
		public List<KeyValuePair<Guid, XElement>> Applications { get; private set; }

		// Token: 0x0600005C RID: 92 RVA: 0x000038A5 File Offset: 0x00001AA5
		public MVVariant(string name)
		{
			this.Name = name;
			this.Conditions = new List<MVCondition>();
			this.SettingsGroups = new List<MVSettingGroup>();
			this.Applications = new List<KeyValuePair<Guid, XElement>>();
		}
	}
}
