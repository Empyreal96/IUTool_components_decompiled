using System;

namespace Microsoft.Phone.Test.TestMetadata.CommandLine
{
	// Token: 0x0200001A RID: 26
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class CommandAttribute : Attribute
	{
		// Token: 0x06000089 RID: 137 RVA: 0x00003B07 File Offset: 0x00001D07
		public CommandAttribute(string name)
		{
			this.Name = name;
			this.BriefUsage = string.Empty;
			this.GeneralInformation = string.Empty;
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600008A RID: 138 RVA: 0x00003B30 File Offset: 0x00001D30
		public string Name { get; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600008B RID: 139 RVA: 0x00003B38 File Offset: 0x00001D38
		// (set) Token: 0x0600008C RID: 140 RVA: 0x00003B40 File Offset: 0x00001D40
		public string BriefDescription { get; set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600008D RID: 141 RVA: 0x00003B49 File Offset: 0x00001D49
		// (set) Token: 0x0600008E RID: 142 RVA: 0x00003B51 File Offset: 0x00001D51
		public string BriefUsage { get; set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600008F RID: 143 RVA: 0x00003B5A File Offset: 0x00001D5A
		// (set) Token: 0x06000090 RID: 144 RVA: 0x00003B62 File Offset: 0x00001D62
		public string GeneralInformation { get; set; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000091 RID: 145 RVA: 0x00003B6B File Offset: 0x00001D6B
		// (set) Token: 0x06000092 RID: 146 RVA: 0x00003B73 File Offset: 0x00001D73
		public bool AllowNoNameOptions { get; set; }
	}
}
