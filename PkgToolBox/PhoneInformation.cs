using System;
using Microsoft.Composition.ToolBox.Reflection;

namespace Microsoft.Composition.ToolBox
{
	// Token: 0x0200000D RID: 13
	public class PhoneInformation : ReflectiveObject
	{
		// Token: 0x06000036 RID: 54 RVA: 0x000029BF File Offset: 0x00000BBF
		public PhoneInformation()
		{
			this.phoneComponent = string.Empty;
			this.phoneSubComponent = string.Empty;
			this.phoneGroupingKey = string.Empty;
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000037 RID: 55 RVA: 0x000029E8 File Offset: 0x00000BE8
		// (set) Token: 0x06000038 RID: 56 RVA: 0x000029F0 File Offset: 0x00000BF0
		public PhoneReleaseType PhoneReleaseType { get; set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000039 RID: 57 RVA: 0x000029F9 File Offset: 0x00000BF9
		// (set) Token: 0x0600003A RID: 58 RVA: 0x00002A01 File Offset: 0x00000C01
		public string PhoneOwner { get; set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00002A0A File Offset: 0x00000C0A
		// (set) Token: 0x0600003C RID: 60 RVA: 0x00002A12 File Offset: 0x00000C12
		public PhoneOwnerType PhoneOwnerType { get; set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600003D RID: 61 RVA: 0x00002A1B File Offset: 0x00000C1B
		// (set) Token: 0x0600003E RID: 62 RVA: 0x00002A23 File Offset: 0x00000C23
		public string PhoneComponent
		{
			get
			{
				return this.phoneComponent;
			}
			set
			{
				this.phoneComponent = string.Empty;
				if (value != null)
				{
					this.phoneComponent = value;
				}
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600003F RID: 63 RVA: 0x00002A3A File Offset: 0x00000C3A
		// (set) Token: 0x06000040 RID: 64 RVA: 0x00002A42 File Offset: 0x00000C42
		public string PhoneSubComponent
		{
			get
			{
				return this.phoneSubComponent;
			}
			set
			{
				this.phoneSubComponent = string.Empty;
				if (value != null)
				{
					this.phoneSubComponent = value;
				}
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000041 RID: 65 RVA: 0x00002A59 File Offset: 0x00000C59
		// (set) Token: 0x06000042 RID: 66 RVA: 0x00002A61 File Offset: 0x00000C61
		public string PhoneGroupingKey
		{
			get
			{
				return this.phoneGroupingKey;
			}
			set
			{
				this.phoneGroupingKey = string.Empty;
				if (value != null)
				{
					this.phoneGroupingKey = value;
				}
			}
		}

		// Token: 0x04000035 RID: 53
		private string phoneComponent;

		// Token: 0x04000036 RID: 54
		private string phoneSubComponent;

		// Token: 0x04000037 RID: 55
		private string phoneGroupingKey;
	}
}
