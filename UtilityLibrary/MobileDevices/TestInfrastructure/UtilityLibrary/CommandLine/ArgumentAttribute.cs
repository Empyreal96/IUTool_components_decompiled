using System;

namespace Microsoft.MobileDevices.TestInfrastructure.UtilityLibrary.CommandLine
{
	// Token: 0x02000004 RID: 4
	[AttributeUsage(AttributeTargets.Field)]
	public class ArgumentAttribute : Attribute
	{
		// Token: 0x06000003 RID: 3 RVA: 0x000020D0 File Offset: 0x000002D0
		public ArgumentAttribute(ArgumentType type)
		{
			this.type = type;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000004 RID: 4 RVA: 0x000020E4 File Offset: 0x000002E4
		public ArgumentType Type
		{
			get
			{
				return this.type;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000005 RID: 5 RVA: 0x000020FC File Offset: 0x000002FC
		public bool DefaultShortName
		{
			get
			{
				return null == this.shortName;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000006 RID: 6 RVA: 0x00002118 File Offset: 0x00000318
		// (set) Token: 0x06000007 RID: 7 RVA: 0x00002130 File Offset: 0x00000330
		public string ShortName
		{
			get
			{
				return this.shortName;
			}
			set
			{
				this.shortName = value;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000008 RID: 8 RVA: 0x0000213C File Offset: 0x0000033C
		public bool DefaultLongName
		{
			get
			{
				return null == this.longName;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000009 RID: 9 RVA: 0x00002158 File Offset: 0x00000358
		// (set) Token: 0x0600000A RID: 10 RVA: 0x00002170 File Offset: 0x00000370
		public string LongName
		{
			get
			{
				return this.longName;
			}
			set
			{
				this.longName = value;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600000B RID: 11 RVA: 0x0000217C File Offset: 0x0000037C
		// (set) Token: 0x0600000C RID: 12 RVA: 0x00002194 File Offset: 0x00000394
		public object DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
			set
			{
				this.defaultValue = value;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600000D RID: 13 RVA: 0x000021A0 File Offset: 0x000003A0
		public bool HasDefaultValue
		{
			get
			{
				return null != this.defaultValue;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600000E RID: 14 RVA: 0x000021C0 File Offset: 0x000003C0
		public bool HasHelpText
		{
			get
			{
				return null != this.helpText;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600000F RID: 15 RVA: 0x000021E0 File Offset: 0x000003E0
		// (set) Token: 0x06000010 RID: 16 RVA: 0x000021F8 File Offset: 0x000003F8
		public string HelpText
		{
			get
			{
				return this.helpText;
			}
			set
			{
				this.helpText = value;
			}
		}

		// Token: 0x04000009 RID: 9
		private string shortName;

		// Token: 0x0400000A RID: 10
		private string longName;

		// Token: 0x0400000B RID: 11
		private string helpText;

		// Token: 0x0400000C RID: 12
		private object defaultValue;

		// Token: 0x0400000D RID: 13
		private ArgumentType type;
	}
}
