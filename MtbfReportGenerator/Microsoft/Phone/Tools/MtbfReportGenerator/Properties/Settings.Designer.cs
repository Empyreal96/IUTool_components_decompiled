using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.Phone.Tools.MtbfReportGenerator.Properties
{
	// Token: 0x02000018 RID: 24
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
	internal sealed partial class Settings : ApplicationSettingsBase
	{
		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000090 RID: 144 RVA: 0x00003D4C File Offset: 0x00001F4C
		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000091 RID: 145 RVA: 0x00003D53 File Offset: 0x00001F53
		[ApplicationScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("NA")]
		public string UnknownSectionName
		{
			get
			{
				return (string)this["UnknownSectionName"];
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000092 RID: 146 RVA: 0x00003D65 File Offset: 0x00001F65
		[ApplicationScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("MtbfReportHtml.xslt")]
		public string DefaultHtmlTemplate
		{
			get
			{
				return (string)this["DefaultHtmlTemplate"];
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000093 RID: 147 RVA: 0x00003D77 File Offset: 0x00001F77
		[ApplicationScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("Stability Time Per Section")]
		public string ExcelTimeSheetName
		{
			get
			{
				return (string)this["ExcelTimeSheetName"];
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000094 RID: 148 RVA: 0x00003D89 File Offset: 0x00001F89
		[ApplicationScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("Completion Percentage")]
		public string ExcelCompletionSheetName
		{
			get
			{
				return (string)this["ExcelCompletionSheetName"];
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000095 RID: 149 RVA: 0x00003D9B File Offset: 0x00001F9B
		[ApplicationScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("7")]
		public int ExcelMaxDevices
		{
			get
			{
				return (int)this["ExcelMaxDevices"];
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000096 RID: 150 RVA: 0x00003DAD File Offset: 0x00001FAD
		[ApplicationScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("20")]
		public int ExcelMaxLoops
		{
			get
			{
				return (int)this["ExcelMaxLoops"];
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000097 RID: 151 RVA: 0x00003DBF File Offset: 0x00001FBF
		[ApplicationScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("12")]
		public int ExcelMaxSections
		{
			get
			{
				return (int)this["ExcelMaxSections"];
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000098 RID: 152 RVA: 0x00003DD1 File Offset: 0x00001FD1
		[ApplicationScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("h\\:mm\\:ss")]
		public string ExcelTimeFormat
		{
			get
			{
				return (string)this["ExcelTimeFormat"];
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000099 RID: 153 RVA: 0x00003DE3 File Offset: 0x00001FE3
		[ApplicationScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("mtbf")]
		public string MtbfLogsFolderName
		{
			get
			{
				return (string)this["MtbfLogsFolderName"];
			}
		}

		// Token: 0x0400003F RID: 63
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}
}
