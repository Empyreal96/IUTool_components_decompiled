using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace SecureWim.Properties
{
	// Token: 0x0200000C RID: 12
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		// Token: 0x06000029 RID: 41 RVA: 0x00002AA5 File Offset: 0x00000CA5
		internal Resources()
		{
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600002A RID: 42 RVA: 0x00002EB5 File Offset: 0x000010B5
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					Resources.resourceMan = new ResourceManager("SecureWim.Properties.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600002B RID: 43 RVA: 0x00002EE1 File Offset: 0x000010E1
		// (set) Token: 0x0600002C RID: 44 RVA: 0x00002EE8 File Offset: 0x000010E8
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002EF0 File Offset: 0x000010F0
		internal static string BuildUsageString
		{
			get
			{
				return Resources.ResourceManager.GetString("BuildUsageString", Resources.resourceCulture);
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00002F06 File Offset: 0x00001106
		internal static string ExtractUsageString
		{
			get
			{
				return Resources.ResourceManager.GetString("ExtractUsageString", Resources.resourceCulture);
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600002F RID: 47 RVA: 0x00002F1C File Offset: 0x0000111C
		internal static string ReplaceUsageString
		{
			get
			{
				return Resources.ResourceManager.GetString("ReplaceUsageString", Resources.resourceCulture);
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00002F32 File Offset: 0x00001132
		internal static byte[] sdiData
		{
			get
			{
				return (byte[])Resources.ResourceManager.GetObject("sdiData", Resources.resourceCulture);
			}
		}

		// Token: 0x0400000F RID: 15
		private static ResourceManager resourceMan;

		// Token: 0x04000010 RID: 16
		private static CultureInfo resourceCulture;
	}
}
