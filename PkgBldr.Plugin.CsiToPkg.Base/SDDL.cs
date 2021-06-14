using System;

namespace Microsoft.CompPlat.PkgBldr.Plugins.CsiToPkg
{
	// Token: 0x0200000D RID: 13
	internal class SDDL
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600001E RID: 30 RVA: 0x000038F8 File Offset: 0x00001AF8
		// (set) Token: 0x0600001F RID: 31 RVA: 0x00003900 File Offset: 0x00001B00
		public string Owner
		{
			get
			{
				return this.owner;
			}
			set
			{
				this.owner = value;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000020 RID: 32 RVA: 0x00003909 File Offset: 0x00001B09
		// (set) Token: 0x06000021 RID: 33 RVA: 0x00003911 File Offset: 0x00001B11
		public string Group
		{
			get
			{
				return this.group;
			}
			set
			{
				this.group = value;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000022 RID: 34 RVA: 0x0000391A File Offset: 0x00001B1A
		// (set) Token: 0x06000023 RID: 35 RVA: 0x00003922 File Offset: 0x00001B22
		public string Dacl
		{
			get
			{
				return this.dacl;
			}
			set
			{
				this.dacl = value;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000024 RID: 36 RVA: 0x0000392B File Offset: 0x00001B2B
		// (set) Token: 0x06000025 RID: 37 RVA: 0x00003933 File Offset: 0x00001B33
		public string Sacl
		{
			get
			{
				return this.sacl;
			}
			set
			{
				this.sacl = value;
			}
		}

		// Token: 0x04000002 RID: 2
		private string owner;

		// Token: 0x04000003 RID: 3
		private string group;

		// Token: 0x04000004 RID: 4
		private string dacl;

		// Token: 0x04000005 RID: 5
		private string sacl;
	}
}
